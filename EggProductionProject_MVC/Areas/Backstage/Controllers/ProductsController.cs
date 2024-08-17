using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class ProductsController : Controller
    {
        private readonly EggPlatformContext _context;

        public ProductsController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Backstage/Products
        public async Task<IActionResult> Index()
        {
            ViewData["Company"] = new SelectList(_context.Stores, "StoreSid", "Company");
            var eggPlatformContext = _context.Products.Include(p => p.PublicStatusNoNavigation).Include(p => p.StoreS).Include(p => p.SubcategoryNoNavigation);
            return View(await eggPlatformContext.ToListAsync());
        }

        // GET: Backstage/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.PublicStatusNoNavigation)
                .Include(p => p.StoreS)
                .Include(p => p.SubcategoryNoNavigation)
                .FirstOrDefaultAsync(m => m.ProductSid == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Backstage/Products/Create
        public IActionResult Create()
        {
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo");
            ViewData["StoreSid"] = new SelectList(_context.Stores, "StoreSid", "StoreSid");
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryNo");
            return View();
        }

        // POST: Backstage/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductSid,ProductNo,ProductName,Price,Stock,SubcategoryNo,ItemNo,StoreSid,Description,Origin,Quanitity,Weight,Component,LaunchTime,PublicStatusNo")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", product.PublicStatusNo);
            ViewData["StoreSid"] = new SelectList(_context.Stores, "StoreSid", "StoreSid", product.StoreSid);
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryNo", product.SubcategoryNo);
            return View(product);
        }

        // GET: Backstage/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", product.PublicStatusNo);
            ViewData["StoreSid"] = new SelectList(_context.Stores, "StoreSid", "StoreSid", product.StoreSid);
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryNo", product.SubcategoryNo);
            return View(product);
        }

        // POST: Backstage/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductSid,ProductNo,ProductName,Price,Stock,SubcategoryNo,ItemNo,StoreSid,Description,Origin,Quanitity,Weight,Component,LaunchTime,PublicStatusNo")] Product product)
        {
            if (id != product.ProductSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductSid))
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
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", product.PublicStatusNo);
            ViewData["StoreSid"] = new SelectList(_context.Stores, "StoreSid", "StoreSid", product.StoreSid);
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryNo", product.SubcategoryNo);
            return View(product);
        }

        // GET: Backstage/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.PublicStatusNoNavigation)
                .Include(p => p.StoreS)
                .Include(p => p.SubcategoryNoNavigation)
                .FirstOrDefaultAsync(m => m.ProductSid == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Backstage/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductSid == id);
        }
    }
}
