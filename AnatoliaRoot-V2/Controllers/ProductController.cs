using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Data;
using AnatoliaRoot_V2.Models;
using AnatoliaRoot_V2.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnatoliaRoot_V2.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductController(AppDbContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        // GET: Product - Sadece ürünleri göster
        [AllowAnonymous]
        [Route("urunler")]
        public async Task<IActionResult> Index(int? categoryId = null, string sortOrder = null, int pageNumber = 1)
        {
            var categories = await _context.Categories.ToListAsync();
            IQueryable<Product> query = _context.Products.Include(p => p.Category);

            if (categoryId.HasValue)
            {
                var selectedCategory = categories.FirstOrDefault(c => c.Id == categoryId.Value);
                if (selectedCategory != null)
                {
                    if (selectedCategory.ParentCategoryId == null)
                    {
                        var altKategoriIdler = categories
                            .Where(c => c.ParentCategoryId == selectedCategory.Id)
                            .Select(c => c.Id)
                            .ToList();
                        altKategoriIdler.Add(selectedCategory.Id);
                        query = query.Where(p => altKategoriIdler.Contains(p.CategoryId));
                    }
                    else
                    {
                        query = query.Where(p => p.CategoryId == selectedCategory.Id);
                    }
                }
            }

            // Sıralama
            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(p => p.Name);
                    break;
                case "name_asc":
                default:
                    query = query.OrderBy(p => p.Name);
                    sortOrder = "name_asc";
                    break;
            }

            // Sayfalama (Pagination - Sayfa başına 24 ürün)
            int pageSize = 24;
            int totalProducts = await query.CountAsync();
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalProducts / (double)pageSize));
            pageNumber = Math.Clamp(pageNumber, 1, totalPages);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ProductIndexViewModel
            {
                Categories = categories,
                Products = products,
                SelectedCategoryId = categoryId,
                SortOrder = sortOrder,
                PageNumber = pageNumber,
                TotalPages = totalPages
            };
            return View(viewModel);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            var anaKategoriler = await _context.Categories.Where(c => c.ParentCategoryId == null).ToListAsync();
            var altKategoriler = await _context.Categories.Where(c => c.ParentCategoryId != null).ToListAsync();
            var viewModel = new ProductCreateViewModel
            {
                AnaKategoriler = anaKategoriler,
                AltKategoriler = altKategoriler
            };
            return View(viewModel);
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            var anaKategoriler = await _context.Categories.Where(c => c.ParentCategoryId == null).ToListAsync();
            var altKategoriler = await _context.Categories.Where(c => c.ParentCategoryId != null).ToListAsync();
            model.AnaKategoriler = anaKategoriler;
            model.AltKategoriler = altKategoriler;
            ModelState.Remove("AnaKategoriler");
            ModelState.Remove("AltKategoriler");
            if (model.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Resim yüklenmesi zorunludur.");
            }
            if (!await IsValidCategorySelectionAsync(model.AnaCategoryId, model.AltCategoryId))
            {
                ModelState.AddModelError("AltCategoryId", "Seçilen alt kategori ana kategoriyle eşleşmiyor.");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(model);
            }
            var imageUrl = await _cloudinaryService.UploadImageAsync(model.ImageFile);
            if (string.IsNullOrEmpty(imageUrl))
            {
                ModelState.AddModelError("ImageFile", "Resim yüklenemedi.");
                return View(model);
            }
            var product = new Product
            {
                Name = model.Name.Trim(),
                Description = model.Description?.Trim(),
                CategoryId = model.AltCategoryId.Value,
                ImageUrl = imageUrl
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminIndex));
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            var altKategori = await _context.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
            var anaKategori = altKategori?.ParentCategoryId;
            var anaKategoriler = await _context.Categories.Where(c => c.ParentCategoryId == null).ToListAsync();
            var altKategoriler = await _context.Categories.Where(c => c.ParentCategoryId != null).ToListAsync();
            var viewModel = new ProductCreateViewModel
            {
                Name = product.Name,
                Description = product.Description,
                AnaCategoryId = anaKategori,
                AltCategoryId = product.CategoryId,
                AnaKategoriler = anaKategoriler,
                AltKategoriler = altKategoriler
            };
            ViewBag.ImageUrl = product.ImageUrl;
            return View(viewModel);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCreateViewModel model)
        {
            var anaKategoriler = await _context.Categories.Where(c => c.ParentCategoryId == null).ToListAsync();
            var altKategoriler = await _context.Categories.Where(c => c.ParentCategoryId != null).ToListAsync();
            model.AnaKategoriler = anaKategoriler;
            model.AltKategoriler = altKategoriler;
            ViewBag.ImageUrl = await _context.Products
                .Where(product => product.Id == id)
                .Select(product => product.ImageUrl)
                .FirstOrDefaultAsync();
            ModelState.Remove("AnaKategoriler");
            ModelState.Remove("AltKategoriler");
            ModelState.Remove("ImageFile");
            if (!await IsValidCategorySelectionAsync(model.AnaCategoryId, model.AltCategoryId))
            {
                ModelState.AddModelError("AltCategoryId", "Seçilen alt kategori ana kategoriyle eşleşmiyor.");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(model);
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            product.Name = model.Name.Trim();
            product.Description = model.Description?.Trim();
            product.CategoryId = model.AltCategoryId.Value;
            if (model.ImageFile != null)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(model.ImageFile);
                if (!string.IsNullOrEmpty(imageUrl))
                    product.ImageUrl = imageUrl;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminIndex));
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/AdminIndex (sadece admin paneli için)
        public async Task<IActionResult> AdminIndex()
        {
            var categories = await _context.Categories.ToListAsync();
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            var viewModel = new ProductIndexViewModel
            {
                Categories = categories,
                Products = products
            };
            ViewBag.AdminPanel = true;
            return View("AdminIndex", viewModel);
        }

        private async Task<bool> IsValidCategorySelectionAsync(int? parentCategoryId, int? childCategoryId)
        {
            if (!parentCategoryId.HasValue || !childCategoryId.HasValue)
                return false;

            var parentExists = await _context.Categories
                .AnyAsync(category => category.Id == parentCategoryId.Value && category.ParentCategoryId == null);
            if (!parentExists)
                return false;

            return await _context.Categories.AnyAsync(category =>
                category.Id == childCategoryId.Value && category.ParentCategoryId == parentCategoryId.Value);
        }
    }
}
