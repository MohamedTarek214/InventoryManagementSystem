
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == product.CategoryId);

                if (!categoryExists)
                {
                    ModelState.AddModelError(
                        nameof(product.CategoryId),
                        "The selected category does not exist.");

                    await LoadCategories(product.CategoryId);
                    return View(product);
                }

                product.CreatedAt = DateTime.UtcNow;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await LoadCategories(product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            await LoadCategories(product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadCategories(product.CategoryId);
                return View(product);
            }

            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null)
                return NotFound();

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == product.CategoryId);

            if (!categoryExists)
            {
                ModelState.AddModelError(
                    nameof(product.CategoryId),
                    "The selected category does not exist.");

                await LoadCategories(product.CategoryId);
                return View(product);
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.QuantityInStock = product.QuantityInStock;
            existingProduct.Supplier = product.Supplier;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories(int? selectedCategoryId = null)
        {
            ViewData["CategoryId"] = new SelectList(
                await _context.Categories.AsNoTracking().ToListAsync(),
                "Id",
                "Name",
                selectedCategoryId);
        }
    }
}
