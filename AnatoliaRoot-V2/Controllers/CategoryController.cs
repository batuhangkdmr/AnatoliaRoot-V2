using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Data;
using AnatoliaRoot_V2.Models;

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
        public IActionResult Food()
        {
            ViewData["Title"] = "Gıda Ürünleri";
            return View();
        }

        public IActionResult Construction()
        {
            ViewData["Title"] = "İnşaat Hizmetleri";
            return View();
        }

        public IActionResult Import()
        {
            ViewData["Title"] = "İthalat Hizmetleri";
            return View();
        }

        public IActionResult Export()
        {
            ViewData["Title"] = "İhracat Hizmetleri";
            return View();
        }

        public IActionResult Ideas()
        {
            ViewData["Title"] = "Fikirler ve Projeler";
            return View();
        }

        // Dashboard - Public erişim
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // Admin actions - Sadece giriş yapmış kullanıcılar
        [Authorize]
        public IActionResult Create(int? parentCategoryId = null)
        {
            var model = new CategoryCreateViewModel
            {
                ParentCategoryId = parentCategoryId
            };
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

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
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
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategori başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
} 