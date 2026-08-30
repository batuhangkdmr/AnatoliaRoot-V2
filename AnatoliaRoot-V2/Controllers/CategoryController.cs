using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Data;
using AnatoliaRoot_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnatoliaRoot_V2.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // Public actions - Herkes erişebilir
        [AllowAnonymous]
        [Route("gida")]
        public IActionResult Food()
        {
            ViewData["Title"] = "Gıda Ürünleri";
            return View();
        }

        [AllowAnonymous]
        [Route("insaat")]
        public IActionResult Construction()
        {
            ViewData["Title"] = "İnşaat Hizmetleri";
            return View();
        }

        [AllowAnonymous]
        [Route("ithalat-ihracat")]
        public IActionResult Trade()
        {
            ViewData["Title"] = "İthalat & İhracat Hizmetleri";
            return View();
        }

        [AllowAnonymous]
        [Route("fikirler")]
        public IActionResult Ideas()
        {
            ViewData["Title"] = "Fikirler ve Projeler";
            return View();
        }

        // Admin kategori yönetimi
        [Route("kategoriler")]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // Admin actions - Sadece giriş yapmış kullanıcılar
        [Authorize]
        public async Task<IActionResult> Create(int? parentCategoryId = null)
        {
            var model = new CategoryCreateViewModel
            {
                ParentCategoryId = parentCategoryId
            };

            // Mevcut kategorileri ViewBag'e ekle
            ViewBag.ParentCategories = await _context.Categories
                .Where(category => category.ParentCategoryId == null)
                .ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            model.Name = model.Name?.Trim();

            if (model.ParentCategoryId.HasValue)
            {
                var parentIsRoot = await _context.Categories.AnyAsync(category =>
                    category.Id == model.ParentCategoryId.Value && category.ParentCategoryId == null);
                if (!parentIsRoot)
                    ModelState.AddModelError("ParentCategoryId", "Alt kategori yalnızca bir ana kategoriye bağlanabilir.");
            }

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                var duplicateExists = await _context.Categories.AnyAsync(category =>
                    category.ParentCategoryId == model.ParentCategoryId && category.Name == model.Name);
                if (duplicateExists)
                    ModelState.AddModelError("Name", "Aynı seviyede bu kategori adı zaten kullanılıyor.");
            }

            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name,
                    ParentCategoryId = model.ParentCategoryId
                };
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategori başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ParentCategories = await _context.Categories
                .Where(category => category.ParentCategoryId == null)
                .ToListAsync();
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (category == null)
            {
                return NotFound();
            }

            // Parent kategorileri ViewBag'e ekle
            ViewBag.ParentCategories = await _context.Categories
                .Where(c => c.Id != id && c.ParentCategoryId == null)
                .ToListAsync();

            return View(category);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ParentCategoryId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
                return NotFound();

            category.Name = category.Name?.Trim();
            var targetParentId = existingCategory.ParentCategoryId == null ? null : category.ParentCategoryId;

            if (existingCategory.ParentCategoryId != null)
            {
                var parentIsRoot = targetParentId.HasValue && await _context.Categories.AnyAsync(parent =>
                    parent.Id == targetParentId.Value && parent.ParentCategoryId == null && parent.Id != id);
                if (!parentIsRoot)
                    ModelState.AddModelError("ParentCategoryId", "Alt kategori yalnızca bir ana kategoriye bağlanabilir.");
            }

            if (!string.IsNullOrWhiteSpace(category.Name))
            {
                var duplicateExists = await _context.Categories.AnyAsync(item =>
                    item.Id != id && item.ParentCategoryId == targetParentId && item.Name == category.Name);
                if (duplicateExists)
                    ModelState.AddModelError("Name", "Aynı seviyede bu kategori adı zaten kullanılıyor.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Sadece değişen alanları güncelle
                    existingCategory.Name = category.Name;
                    existingCategory.ParentCategoryId = targetParentId;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Kategori başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Hata durumunda parent kategorileri tekrar yükle
            ViewBag.ParentCategories = await _context.Categories
                .Where(c => c.Id != id && c.ParentCategoryId == null)
                .ToListAsync();

            return View(category);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                try
                {
                    // Kategoriye bağlı ürün var mı kontrol et
                    var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
                    if (hasProducts)
                    {
                        return Json(new { success = false, message = "Bu kategoriye bağlı ürünler bulunmaktadır. Lütfen önce bu kategoriye bağlı ürünleri silip tekrar deneyiniz." });
                    }

                    var hasSubCategories = await _context.Categories.AnyAsync(item => item.ParentCategoryId == id);
                    if (hasSubCategories)
                    {
                        return Json(new { success = false, message = "Bu kategoriye bağlı alt kategoriler bulunmaktadır. Lütfen önce alt kategorileri siliniz." });
                    }

                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Kategori başarıyla silindi." });
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("REFERENCE constraint") == true)
                {
                    return Json(new { success = false, message = "Bu kategori başka kayıtlar tarafından kullanıldığı için silinemiyor." });
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Kategori silinirken bir hata oluştu." });
                }
            }

            return Json(new { success = false, message = "Kategori bulunamadı." });
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
