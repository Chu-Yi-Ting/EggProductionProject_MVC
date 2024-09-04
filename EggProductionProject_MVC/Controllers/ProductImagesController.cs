using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;

namespace EggProductionProject_MVC.Controllers
{
    public class ProductImagesController : Controller
    {
        private readonly EggPlatformContext _context;

        public ProductImagesController(EggPlatformContext context)
        {
            _context = context;
        }

        //GET: ProductImages/GetPicture/1
        [HttpGet]
        //public async Task<FileResult> GetPicture(int id)
        //{
        //    ProductImage c = await _context.ProductImages.FindAsync(id);
        //    byte[] ImageContent = c?.Image; //?的意思是當c有值的時候，再去取值，c沒有值的時候，就填空值，所以這個時候，查超過資料id時，報錯是顯示null
        //    return File(ImageContent, "image/jpeg");
        //}

        // GET: ProductImages
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.ProductImages.Include(p => p.ProductS);
            return View(await eggPlatformContext.ToListAsync());
        }

        // GET: ProductImages/AllData
        public async Task<IActionResult> AllData()
        {
            var eggPlatformContext = _context.ProductImages.Include(p => p.ProductS);
            return Json(eggPlatformContext);
        }

        // GET: ProductImages/Details/5
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

        // GET: ProductImages/Create
        public IActionResult Create()
        {
            ViewData["ProductSid"] = new SelectList(_context.Products, "ProductSid", "ProductName");
            return View();
        }

        // POST: ProductImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductImageSid,ProductSid,Image,ImageDescription,UploadTime")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                if (Request.Form.Files["Image"] != null)
                {
                    using (BinaryReader reader = new BinaryReader(Request.Form.Files["Image"].OpenReadStream()))
                    {
                        //productImage.Image = reader.ReadBytes((int)Request.Form.Files["Image"].Length);
                    }
                }
                _context.Add(productImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductSid"] = new SelectList(_context.Products, "ProductSid", "ProductSid", productImage.ProductSid);
            return View(productImage);
        }

        // GET: ProductImages/Edit/5
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
            //ViewData["ProductSid"] = new SelectList(_context.Products, "ProductSid", "ProductSid", productImage.ProductSid);
            ViewData["ProductSid"] = new SelectList(_context.Products, "ProductSid", "ProductName");
            return View(productImage);
        }

        // POST: ProductImages/Edit/5
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
                    ProductImage p = await _context.ProductImages.FindAsync(productImage.ProductImageSid);
                    if (Request.Form.Files["Image"] != null)
                    {
                        using (BinaryReader reader = new BinaryReader(Request.Form.Files["Image"].OpenReadStream()))
                        {
                            //productImage.Image = reader.ReadBytes((int)Request.Form.Files["Image"].Length);
                        }

                    }
                    else
                    {
                        //productImage.Image = p.Image;
                    }
                    _context.Entry(p).State = EntityState.Detached;
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

        // GET: ProductImages/Delete/5
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

        // POST: ProductImages/Delete/5
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
