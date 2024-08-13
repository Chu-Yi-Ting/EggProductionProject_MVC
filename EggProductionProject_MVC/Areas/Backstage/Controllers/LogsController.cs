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
    public class LogsController : Controller
    {
        private readonly EggPlatformContext _context;

        public LogsController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Backstage/Logs
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.Logs.Include(l => l.MemberS);
            return View(await eggPlatformContext.ToListAsync());
        }

        // GET: Backstage/Logs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .Include(l => l.MemberS)
                .FirstOrDefaultAsync(m => m.LogSid == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        // GET: Backstage/Logs/Create
        public IActionResult Create()
        {
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid");
            return View();
        }

        // POST: Backstage/Logs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LogSid,MemberSid,IsLogSuccess,LogTime")] Log log)
        {
            if (ModelState.IsValid)
            {
                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", log.MemberSid);
            return View(log);
        }

        // GET: Backstage/Logs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", log.MemberSid);
            return View(log);
        }

        // POST: Backstage/Logs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LogSid,MemberSid,IsLogSuccess,LogTime")] Log log)
        {
            if (id != log.LogSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(log);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogExists(log.LogSid))
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
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", log.MemberSid);
            return View(log);
        }

        // GET: Backstage/Logs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .Include(l => l.MemberS)
                .FirstOrDefaultAsync(m => m.LogSid == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        // POST: Backstage/Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                _context.Logs.Remove(log);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogExists(int id)
        {
            return _context.Logs.Any(e => e.LogSid == id);
        }
    }
}
