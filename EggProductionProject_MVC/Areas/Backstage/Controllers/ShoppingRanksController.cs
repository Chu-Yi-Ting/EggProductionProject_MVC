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
    public class ShoppingRanksController : Controller
    {
        private readonly EggPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ShoppingRanksController(EggPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }


        //拿圖片
        public async Task<IActionResult> GetPic(int id)
        {
            ShoppingRank? shoppingRank = await _context.ShoppingRanks.FindAsync(id);
            byte[]? content = shoppingRank?.Eggimage;
            return File(content, "image/jpeg");
        }


        // GET: ShoppingRanks
        public async Task<IActionResult> Index()
        {
            return View(await _context.ShoppingRanks.ToListAsync());
        }

        // GET: ShoppingRanks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingRank = await _context.ShoppingRanks
                .FirstOrDefaultAsync(m => m.ShoppingRankNo == id);
            if (shoppingRank == null)
            {
                return NotFound();
            }

            return View(shoppingRank);
        }

        // GET: ShoppingRanks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShoppingRanks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShoppingRankNo,ShoppingRank1,Eggimage")] ShoppingRank shoppingRank)
        {
            if (ModelState.IsValid)
            {
                if (Request.Form.Files["Eggimage"] != null)
                {
                    ReadUploadImage(shoppingRank);
                }

                _context.Add(shoppingRank);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingRank);
        }

        // GET: ShoppingRanks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingRank = await _context.ShoppingRanks.FindAsync(id);
            if (shoppingRank == null)
            {
                return NotFound();
            }
            return View(shoppingRank);
        }

        // POST: ShoppingRanks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShoppingRankNo,ShoppingRank1,Eggimage")] ShoppingRank shoppingRank)
        {
            if (id != shoppingRank.ShoppingRankNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //寫入圖片
                    ShoppingRank c = await _context.ShoppingRanks.FindAsync(shoppingRank.ShoppingRankNo);
                    if (Request.Form.Files["Eggimage"] != null)
                    {
                        ReadUploadImage(shoppingRank);
                    }
                    else
                    {
                        shoppingRank.Eggimage = c.Eggimage;
                    }
                    _context.Entry(c).State = EntityState.Detached;




                    _context.Update(shoppingRank);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingRankExists(shoppingRank.ShoppingRankNo))
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
            return View(shoppingRank);
        }

        // GET: ShoppingRanks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingRank = await _context.ShoppingRanks
                .FirstOrDefaultAsync(m => m.ShoppingRankNo == id);
            if (shoppingRank == null)
            {
                return NotFound();
            }

            return View(shoppingRank);
        }

        private void ReadUploadImage(ShoppingRank shoppingRank)
        {
            using (BinaryReader br = new BinaryReader(
                Request.Form.Files["Eggimage"].OpenReadStream()))
            {
                shoppingRank.Eggimage = br.ReadBytes((int)Request.Form.Files["Eggimage"].Length);
            }
        }




        // POST: ShoppingRanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingRank = await _context.ShoppingRanks.FindAsync(id);
            if (shoppingRank != null)
            {
                _context.ShoppingRanks.Remove(shoppingRank);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingRankExists(int id)
        {
            return _context.ShoppingRanks.Any(e => e.ShoppingRankNo == id);
        }
    }
}
