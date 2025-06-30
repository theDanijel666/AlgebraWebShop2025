using AlgebraWebShop2025.Data;
using AlgebraWebShop2025.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlgebraWebShop2025.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductCategory
        public async Task<IActionResult> Index(int productId)
        {
            if (productId == 0 || productId<0)
                return RedirectToAction("Index", "Product");
            var product=_context.Product.Where(p=>p.Id == productId).FirstOrDefault();
            if (product == null) return RedirectToAction("Index", "Product");

            List<ProductCategory> list = await _context.ProductCategory.
                Where(p=>p.ProductId==productId).ToListAsync();
            foreach (var item in list)
            {
                item.ProductTitle = _context.Product.Where(p => p.Id == item.ProductId).First().Title;
                item.CategoryTitle = _context.Category.Where(c => c.Id == item.CategoryId).First().Title;
            }

            ViewBag.ProductId = productId;

            return View(list);
        }

        // GET: Admin/ProductCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // GET: Admin/ProductCategory/Create
        public IActionResult Create(int productId)
        {
            if (productId == 0 || productId < 0)
                return RedirectToAction("Index", "Product");
            ViewBag.Categories=_context.Category.Select(
                cat=> new SelectListItem
                {
                    Value=cat.Id.ToString(),
                    Text=cat.Title
                }).ToList();

            ViewBag.ProductId = productId;

            return View();
        }

        // POST: Admin/ProductCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,ProductId")] ProductCategory productCategory)
        {
            ModelState.Remove("ProductTitle");
            ModelState.Remove("CategoryTitle");
            if (ModelState.IsValid)
            {
                _context.Add(productCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),new { productId = productCategory.ProductId});
            }
            return View(productCategory);
        }

        // GET: Admin/ProductCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Category.Select(
                cat => new SelectListItem
                {
                    Value = cat.Id.ToString(),
                    Text = cat.Title
                }).ToList();

            return View(productCategory);
        }

        // POST: Admin/ProductCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,ProductId")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return NotFound();
            }

            ModelState.Remove("ProductTitle");
            ModelState.Remove("CategoryTitle");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { productId = productCategory.ProductId });
            }

            ViewBag.Categories = _context.Category.Select(
                cat => new SelectListItem
                {
                    Value = cat.Id.ToString(),
                    Text = cat.Title
                }).ToList();

            return View(productCategory);
        }

        // GET: Admin/ProductCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            productCategory.ProductTitle = _context.Product.Find(productCategory.ProductId).Title;
            productCategory.CategoryTitle = _context.Category.Find(productCategory.CategoryId).Title;
            return View(productCategory);
        }

        // POST: Admin/ProductCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productCategory = await _context.ProductCategory.FindAsync(id);
            int pid=0;
            if (productCategory != null)
            {
                pid = productCategory.ProductId;
                _context.ProductCategory.Remove(productCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new { productId = pid});
        }

        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategory.Any(e => e.Id == id);
        }
    }
}
