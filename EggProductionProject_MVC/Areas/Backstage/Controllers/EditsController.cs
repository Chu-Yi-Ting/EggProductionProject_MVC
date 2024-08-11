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
    public class EditsController : Controller
    {
        private readonly EggPlatformContext _context;

        public EditsController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Edits
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.Edits.Include(e => e.ArticleS).Include(e => e.ReplyS);
            return View(await eggPlatformContext.ToListAsync());
        }

        // GET: Edits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var edit = await _context.Edits
                .Include(e => e.ArticleS)
                .Include(e => e.ReplyS)
                .FirstOrDefaultAsync(m => m.EditSid == id);
            if (edit == null)
            {
                return NotFound();
            }

            return View(edit);
        }

        //編輯紀錄只給讀
        //// GET: Edits/Create
        //public IActionResult Create()
        //{
        //    ViewData["ArticleSid"] = new SelectList(_context.Articles, "ArticleSid", "ArticleSid");
        //    ViewData["ReplySid"] = new SelectList(_context.Replies, "ReplySid", "ReplySid");
        //    return View();
        //}

        //// POST: Edits/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("EditSid,ArticleSid,ReplySid,EditBefore,EditAfter,EditTime")] Edit edit)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(edit);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ArticleSid"] = new SelectList(_context.Articles, "ArticleSid", "ArticleSid", edit.ArticleSid);
        //    ViewData["ReplySid"] = new SelectList(_context.Replies, "ReplySid", "ReplySid", edit.ReplySid);
        //    return View(edit);
        //}

        //// GET: Edits/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var edit = await _context.Edits.FindAsync(id);
        //    if (edit == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ArticleSid"] = new SelectList(_context.Articles, "ArticleSid", "ArticleSid", edit.ArticleSid);
        //    ViewData["ReplySid"] = new SelectList(_context.Replies, "ReplySid", "ReplySid", edit.ReplySid);
        //    return View(edit);
        //}

        //// POST: Edits/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("EditSid,ArticleSid,ReplySid,EditBefore,EditAfter,EditTime")] Edit edit)
        //{
        //    if (id != edit.EditSid)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(edit);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!EditExists(edit.EditSid))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ArticleSid"] = new SelectList(_context.Articles, "ArticleSid", "ArticleSid", edit.ArticleSid);
        //    ViewData["ReplySid"] = new SelectList(_context.Replies, "ReplySid", "ReplySid", edit.ReplySid);
        //    return View(edit);
        //}

        //// GET: Edits/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var edit = await _context.Edits
        //        .Include(e => e.ArticleS)
        //        .Include(e => e.ReplyS)
        //        .FirstOrDefaultAsync(m => m.EditSid == id);
        //    if (edit == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(edit);
        //}

        //// POST: Edits/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var edit = await _context.Edits.FindAsync(id);
        //    if (edit != null)
        //    {
        //        _context.Edits.Remove(edit);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool EditExists(int id)
        {
            return _context.Edits.Any(e => e.EditSid == id);
        }
    }
}
