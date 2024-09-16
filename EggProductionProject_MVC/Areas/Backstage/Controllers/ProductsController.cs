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
            ViewData["Title"] = "GOOD EGG 員工管理-商品管理";
            ViewData["Company"] = new SelectList(_context.Stores, "StoreSid", "Company");
            var eggPlatformContext = _context.Products
                .Include(p => p.PublicStatusNoNavigation)
                .Include(p => p.StoreS)
                .Include(p => p.SubcategoryNoNavigation)
                .OrderByDescending(p => p.LaunchTime); // 按照 LaunchTime 降序排序
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
        public async Task<IActionResult> Create([Bind("ProductSid,ProductNo,ProductName,Price,Stock,SubcategoryNo,SubcategoryName,ItemNo,StoreSid,,Description,Origin,Quanitity,Weight,Component,LaunchTime,PublicStatusNo,Company")] Product product)
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
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription");
            //ViewData["StoreSid"] = new SelectList(_context.Stores, "StoreSid", "Company");
            //因為網頁編輯的時候，賣場名稱只能唯獨，改成下面方式處理，不能直接使用了 SelectList 而是從 ViewBag.StoreSid 中提取具體的賣場名稱
            ViewBag.StoreSid = _context.Stores
                           .Where(s => s.StoreSid == product.StoreSid)
                           .Select(s => s.Company) // 取得賣場名稱
                           .FirstOrDefault();
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryName");
            return View(product);
        }

        // POST: Backstage/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductSid,ProductNo,ProductName,Price,Stock,SubcategoryNo,ItemNo,StoreSid,,Description,Origin,Quanitity,Weight,Component,LaunchTime,PublicStatusNo,DiscountPercent")] Product product)
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
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription");
            ViewData["StoreSid"] = new SelectList(_context.Stores, "StoreSid", "Company");
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryName");
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

        //商品根據賣場id做篩選
        public IActionResult GetProductsByStoreSid(int StoreSid)
        {
            var products = _context.Products
                                   .Where(p => p.StoreSid == StoreSid)
                                   .Include(p => p.PublicStatusNoNavigation) // 加入關聯的 PublicStatus 表
                                   .ToList();
            return PartialView("_ProductListPartial", products);
        }

        //商品名稱查詢功能
        public async Task<IActionResult> SearchProducts(string searchString)
        {
            var products = from p in _context.Products
                           .Include(p => p.PublicStatusNoNavigation)
                           .Include(p => p.StoreS)
                           .Include(p => p.SubcategoryNoNavigation)
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.ProductName.Contains(searchString));
            }

            return PartialView("_ProductListPartial", await products.ToListAsync());
        }

        //顯示所有商品
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products
                           .Include(p => p.PublicStatusNoNavigation)
                           .Include(p => p.StoreS)
                           .Include(p => p.SubcategoryNoNavigation)
                           .ToListAsync();

            return PartialView("_ProductListPartial", products);
        }
    }
}
