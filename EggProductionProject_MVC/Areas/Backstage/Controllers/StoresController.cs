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
    public class StoresController : Controller
    {
        private readonly EggPlatformContext _context;

        public StoresController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Backstage/Stores
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.Stores.Include(s => s.MemberS).Include(s => s.PublicStatusNoNavigation);
            return View(await eggPlatformContext.ToListAsync());
        }

        //// GET: Backstage/Stores/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var store = await _context.Stores
        //        .Include(s => s.MemberS)
        //        .Include(s => s.PublicStatusNoNavigation)
        //        .FirstOrDefaultAsync(m => m.StoreSid == id);
        //    if (store == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(store);
        //}

        //// GET: Backstage/Stores/Create
        //public IActionResult Create()
        //{
        //    ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid");
        //    ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo");
        //    return View();
        //}

        //// POST: Backstage/Stores/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("StoreSid,MemberSid,Company,EstablishDate,StoreImg,StoreIntroduction,PublicStatusNo")] Store store)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(store);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", store.MemberSid);
        //    ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", store.PublicStatusNo);
        //    return View(store);
        //}

        // GET: Backstage/Stores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", store.MemberSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", store.PublicStatusNo);
            return View(store);
        }

        // POST: Backstage/Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StoreSid,MemberSid,Company,EstablishDate,StoreImg,StoreIntroduction,PublicStatusNo")] Store store)
        {
            if (id != store.StoreSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.StoreSid))
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
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", store.MemberSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", store.PublicStatusNo);
            return View(store);
        }

        //// GET: Backstage/Stores/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var store = await _context.Stores
        //        .Include(s => s.MemberS)
        //        .Include(s => s.PublicStatusNoNavigation)
        //        .FirstOrDefaultAsync(m => m.StoreSid == id);
        //    if (store == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(store);
        //}

        //// POST: Backstage/Stores/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var store = await _context.Stores.FindAsync(id);
        //    if (store != null)
        //    {
        //        _context.Stores.Remove(store);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool StoreExists(int id)
        {
            return _context.Stores.Any(e => e.StoreSid == id);
        }
    }
}
