using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class ProductImagesController : Controller
    {
        private readonly EggPlatformContext _context;

        public ProductImagesController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Frontstage/ProductImages
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.ProductImages.Include(p => p.ProductS);
            return View(await eggPlatformContext.ToListAsync());
        }

        // GET: Frontstage/ProductImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .Include(p => p.ProductS)
                .FirstOrDefaultAsync(m => m.ProductImageSid == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // GET: Frontstage/ProductImages/Create
        public IActionResult Create()
        {
            ViewData["ProductSid"] = new SelectList(_context.Products, "ProductSid", "ProductSid");
            return View();
        }

        // POST: Frontstage/ProductImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductImageSid,ProductSid,Image,ImageDescription,UploadTime")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductSid"] = new SelectList(_context.Products, "ProductSid", "ProductSid", productImage.ProductSid);
            return View(productImage);
        }

        // GET: Frontstage/ProductImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }
            ViewData["ProductSid"] = new SelectList(_context.Products, "ProductSid", "ProductSid", productImage.ProductSid);
            return View(productImage);
        }

        // POST: Frontstage/ProductImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductImageSid,ProductSid,Image,ImageDescription,UploadTime")] ProductImage productImage)
        {
            if (id != productImage.ProductImageSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.ProductImageSid))
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
            ViewData["ProductSid"] = new SelectList(_context.Products, "ProductSid", "ProductSid", productImage.ProductSid);
            return View(productImage);
        }

        // GET: Frontstage/ProductImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .Include(p => p.ProductS)
                .FirstOrDefaultAsync(m => m.ProductImageSid == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: Frontstage/ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage != null)
            {
                _context.ProductImages.Remove(productImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductImageExists(int id)
        {
            return _context.ProductImages.Any(e => e.ProductImageSid == id);
        }
    }
}
