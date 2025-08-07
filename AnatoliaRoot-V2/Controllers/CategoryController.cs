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
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // Public actions - Herkes erişebilir
        [Route("gida")]
        public IActionResult Food()
        {
            ViewData["Title"] = "Gıda Ürünleri";
            return View();
        }

        [Route("insaat")]
        public IActionResult Construction()
        {
            ViewData["Title"] = "İnşaat Hizmetleri";
            return View();
        }

        [Route("ithalat-ihracat")]
        public IActionResult Trade()
        {
            ViewData["Title"] = "İthalat & İhracat Hizmetleri";
            return View();
        }

        [Route("fikirler")]
        public IActionResult Ideas()
        {
            ViewData["Title"] = "Fikirler ve Projeler";
            return View();
        }

        // Dashboard - Public erişim
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
            ViewBag.ParentCategories = await _context.Categories.ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
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
                .Where(c => c.Id != id) // Kendisini parent olarak seçemez
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

            if (ModelState.IsValid)
            {
                try
                {
                    // Mevcut kategoriyi bul
                    var existingCategory = await _context.Categories.FindAsync(id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    // Sadece değişen alanları güncelle
                    existingCategory.Name = category.Name;
                    
                    // Ana kategori ise ParentCategoryId'yi null olarak koru
                    if (existingCategory.ParentCategoryId == null)
                    {
                        existingCategory.ParentCategoryId = null; // Ana kategori kalır
                    }
                    else
                    {
                        // Alt kategori ise ParentCategoryId'yi güncelle
                        existingCategory.ParentCategoryId = category.ParentCategoryId;
                    }

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
                .Where(c => c.Id != id)
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

                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Kategori başarıyla silindi." });
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("REFERENCE constraint") == true)
                {
                    return Json(new { success = false, message = "Bu kategoriye bağlı ürünler bulunmaktadır. Lütfen önce bu kategoriye bağlı ürünleri silip tekrar deneyiniz." });
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