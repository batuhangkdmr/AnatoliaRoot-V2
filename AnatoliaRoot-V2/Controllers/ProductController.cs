using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Data;
using AnatoliaRoot_V2.Models;
using AnatoliaRoot_V2.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AnatoliaRoot_V2.Controllers
{
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
        public async Task<IActionResult> Index()
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
                Name = model.Name,
                Description = model.Description,
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
                AltKategoriler = altKategoriler,
                ExistingImageUrl = product.ImageUrl
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
            ModelState.Remove("AnaKategoriler");
            ModelState.Remove("AltKategoriler");
            ModelState.Remove("ImageFile");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(model);
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            product.Name = model.Name;
            product.Description = model.Description;
            product.CategoryId = model.AltCategoryId.Value;
            if (model.ImageFile != null)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(model.ImageFile);
                if (!string.IsNullOrEmpty(imageUrl))
                    product.ImageUrl = imageUrl;
            }
            else
            {
                product.ImageUrl = model.ExistingImageUrl;
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
    }
} 