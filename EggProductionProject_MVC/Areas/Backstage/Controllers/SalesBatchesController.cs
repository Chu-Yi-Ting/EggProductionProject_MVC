﻿using System;
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
    public class SalesBatchesController : Controller
    {
        private readonly EggPlatformContext _context;

        public SalesBatchesController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Backstage/SalesBatches
        public async Task<IActionResult> Index()
        {
            return View(await _context.SalesBatches.ToListAsync());
        }

        // GET: Backstage/SalesBatches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesBatch = await _context.SalesBatches
                .FirstOrDefaultAsync(m => m.PeriodSid == id);
            if (salesBatch == null)
            {
                return NotFound();
            }

            return View(salesBatch);
        }

        // GET: Backstage/SalesBatches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Backstage/SalesBatches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PeriodSid,PeriodDate,StartTime,EndTime,RunningStatus")] SalesBatch salesBatch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesBatch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salesBatch);
        }

        // GET: Backstage/SalesBatches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesBatch = await _context.SalesBatches.FindAsync(id);
            if (salesBatch == null)
            {
                return NotFound();
            }
            return View(salesBatch);
        }

        // POST: Backstage/SalesBatches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PeriodSid,PeriodDate,StartTime,EndTime,RunningStatus")] SalesBatch salesBatch)
        {
            if (id != salesBatch.PeriodSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesBatch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesBatchExists(salesBatch.PeriodSid))
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
            return View(salesBatch);
        }

        // GET: Backstage/SalesBatches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesBatch = await _context.SalesBatches
                .FirstOrDefaultAsync(m => m.PeriodSid == id);
            if (salesBatch == null)
            {
                return NotFound();
            }

            return View(salesBatch);
        }

        // POST: Backstage/SalesBatches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesBatch = await _context.SalesBatches.FindAsync(id);
            if (salesBatch != null)
            {
                _context.SalesBatches.Remove(salesBatch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesBatchExists(int id)
        {
            return _context.SalesBatches.Any(e => e.PeriodSid == id);
        }
    }
}
