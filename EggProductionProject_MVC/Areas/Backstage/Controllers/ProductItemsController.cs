using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Areas.Backstage.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class ProductItemsController : Controller
    {
        private readonly EggPlatformContext _context;

        public ProductItemsController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Backstage/ProductItems
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.ProductItems.Include(p => p.SubcategoryNoNavigation).Select(p => new ItemViewModel 
            {
                ItemNo = p.ItemNo,
                SubcategoryName = p.SubcategoryNoNavigation.SubcategoryName,
                ItemName = p.ItemName,
                ItemDescription = p.ItemDescription
            });
            // 從資料庫中獲取最後一個 ItemNo，並加1
            var lastItemNo = _context.ProductItems.OrderByDescending(p => p.ItemNo).FirstOrDefault()?.ItemNo ?? 0;
            var newItemNo = lastItemNo + 1;

            // 將新計算的 ItemNo 和產品分類列表傳遞給視圖             
            ViewBag.ItemNumber = newItemNo;
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryName");
            return View(await eggPlatformContext.ToListAsync());
        }

        // GET: Backstage/ProductItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productItem = await _context.ProductItems
                .Include(p => p.SubcategoryNoNavigation)
                .FirstOrDefaultAsync(m => m.ItemNo == id);
            if (productItem == null)
            {
                return NotFound();
            }

            return View(productItem);
        }

        // GET: Backstage/ProductItems/Create
        public IActionResult Create()
        {
            // 從資料庫中獲取最後一個 ItemNo，並加1
            var lastItemNo = _context.ProductItems.OrderByDescending(p => p.ItemNo).FirstOrDefault()?.ItemNo ?? 0;
            var newItemNo = lastItemNo + 1;

            // 將新計算的 ItemNo 和產品分類列表傳遞給視圖             
            ViewBag.ItemNumber = newItemNo;
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryName");
            return View();
        }

        // POST: Backstage/ProductItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemNo,SubcategoryNo,ItemName,ItemDescription")] ProductItem productItem)
        {            

            if (ModelState.IsValid)
            {
                _context.Add(productItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //這個 SelectList 的建構函數接受四個參數，用來生成下拉選單的選項。
            //第一個 "SubcategoryNo" 是資料來源中用作選項值（value）的屬性名稱，即當使用者選擇一個選項時，這個值會被傳遞回伺服器。
            //第二個 "SubcategoryNo" 是用作選項顯示文本（text）的屬性名稱，即在下拉選單中顯示給使用者看的文字。
            //productItem.SubcategoryNo這是下拉選單中預選擇的值
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryNo", productItem.SubcategoryNo);
            return View(productItem);
        }

        // GET: Backstage/ProductItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productItem = await _context.ProductItems.FindAsync(id);
            if (productItem == null)
            {
                return NotFound();
            }
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryName");
            return View(productItem);
        }

        // POST: Backstage/ProductItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemNo,SubcategoryNo,ItemName,ItemDescription")] ProductItem productItem)
        {
            if (id != productItem.ItemNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductItemExists(productItem.ItemNo))
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
            ViewData["SubcategoryNo"] = new SelectList(_context.ProductSubcategories, "SubcategoryNo", "SubcategoryName");
            return View(productItem);
        }

        // GET: Backstage/ProductItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productItem = await _context.ProductItems
                .Include(p => p.SubcategoryNoNavigation)
                .FirstOrDefaultAsync(m => m.ItemNo == id);
            if (productItem == null)
            {
                return NotFound();
            }

            return View(productItem);
        }

        // POST: Backstage/ProductItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productItem = await _context.ProductItems.FindAsync(id);
            if (productItem != null)
            {
                _context.ProductItems.Remove(productItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //製作一個create的儲存action
        [HttpPost]
        public async Task<IActionResult> CreateSave([FromForm] ProductItem productItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productItem);
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { success = true, message = "資料成功寫入" });
                }
                catch (Exception ex)
                {
                    // 处理错误并返回状态码和错误信息
                    return StatusCode(500, new { success = false, message = "沒有成功寫入資料" });
                }
            }

            return BadRequest(new { success = false, message = "資料驗證失敗" });
        }


        private bool ProductItemExists(int id)
        {
            return _context.ProductItems.Any(e => e.ItemNo == id);
        }
    }
}
