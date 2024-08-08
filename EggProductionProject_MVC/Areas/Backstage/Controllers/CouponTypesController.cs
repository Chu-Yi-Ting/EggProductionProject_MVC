using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Areas.Backstage.ViewModels;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class CouponTypesController : Controller
    {
        private readonly EggPlatformContext _context;

        public CouponTypesController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Backstage/CouponTypes
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.CouponTypes.Include(c => c.PublicStatusNoNavigation);
            return View(await eggPlatformContext.ToListAsync());
        }

        public JsonResult IndexJson()
        {
            var publicStatuses = _context.PublicStatuses.ToList();
            var couponTypes = _context.CouponTypes.ToList();
            var employees = _context.Employees.ToList();

            var CouponTypesViewModels = couponTypes.Select(p => new CouponTypeViewModels
            {
                CouponTypeNo = p.CouponTypeNo,
                Name = p.Name,
                Price = p.Price,
                Minimum = p.Minimum,
                PublicStatusNo = p.PublicStatusNo,
                StatusDescription = publicStatuses.FirstOrDefault(s => s.PublicStatusNo == p.PublicStatusNo)?.StatusDescription,
                StartTime = p.StartTime,
                EndTime = p.EndTime,
                UseAlone = p.UseAlone,
                EmpName = employees.FirstOrDefault(s => s.EmployeeSid == p.EmployeeSid)?.EmpName,
            });

            return Json(CouponTypesViewModels);
        }

        // GET: Backstage/CouponTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponType = await _context.CouponTypes
                .Include(c => c.PublicStatusNoNavigation)
                .FirstOrDefaultAsync(m => m.CouponTypeNo == id);
            if (couponType == null)
            {
                return NotFound();
            }

            return View(couponType);
        }

        // GET: Backstage/CouponTypes/Create
        public IActionResult Create()
        {
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo");
            return View();
        }

        // POST: Backstage/CouponTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CouponTypeNo,Name,Price,Minimum,PublicStatusNo,StartTime,EndTime,UseAlone,EmployeeSid")] CouponType couponType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(couponType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
            return View(couponType);
        }

        // GET: Backstage/CouponTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType == null)
            {
                return NotFound();
            }
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
            return View(couponType);
        }

        // POST: Backstage/CouponTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CouponTypeNo,Name,Price,Minimum,PublicStatusNo,StartTime,EndTime,UseAlone,EmployeeSid")] CouponType couponType)
        {
            if (id != couponType.CouponTypeNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(couponType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponTypeExists(couponType.CouponTypeNo))
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
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
            return View(couponType);
        }

        // GET: Backstage/CouponTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponType = await _context.CouponTypes
                .Include(c => c.PublicStatusNoNavigation)
                .FirstOrDefaultAsync(m => m.CouponTypeNo == id);
            if (couponType == null)
            {
                return NotFound();
            }

            return View(couponType);
        }

        // POST: Backstage/CouponTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType != null)
            {
                _context.CouponTypes.Remove(couponType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CouponTypeExists(int id)
        {
            return _context.CouponTypes.Any(e => e.CouponTypeNo == id);
        }
    }
}
