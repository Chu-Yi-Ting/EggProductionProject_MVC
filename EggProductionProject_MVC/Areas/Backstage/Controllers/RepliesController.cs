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
    public class RepliesController : Controller
    {
        private readonly EggPlatformContext _context;

        public RepliesController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Replies
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var replies = GetFilteredReplies(searchString);

            int totalReplies = await replies.CountAsync();
            ViewData["TotalArticles"] = totalReplies;

            return View(await replies.ToListAsync());
        }

        // GET: Replies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reply = await _context.Replies
                .Include(r => r.ArticleCreaterS)
                .Include(r => r.ArticleS)
                .Include(r => r.PublicStatusNoNavigation)
                .FirstOrDefaultAsync(m => m.ReplySid == id);
            if (reply == null)
            {
                return NotFound();
            }

            return View(reply);
        }
        //後台不會有創造回覆功能
        // GET: Replies/Create 
        //    public IActionResult Create()
        //    {
        //        var statuses = _context.PublicStatuses
        //       .Select(s => new { s.PublicStatusNo, s.StatusDescription })
        //       .ToList();

        //        var artitleTitle = _context.Replies
        //.Select(a => new { a.ArticleSid, a.ArticleS })
        //.ToList();

        //        ViewData["ArtitleTitle"] = artitleTitle;
        //        ViewData["ArticleCreaterSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid");
        //        ViewData["ArticleSid"] = new SelectList(_context.Articles, "ArticleSid", "ArticleSid");
        //        ViewData["PublicStatusNo"] = new SelectList(statuses, "PublicStatusNo", "StatusDescription");
        //        return View();
        //    }

        //    // POST: Replies/Create
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("ReplySid,ArticleSid,ArticleCreaterSid,ReplyInfo,ReplyDate,ReplyUpdate,EditTimes,TagMemberNo,PublicStatusNo,DeleteOrNot")] Reply reply)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(reply);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["ArticleCreaterSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", reply.ArticleCreaterSid);
        //        ViewData["ArticleSid"] = new SelectList(_context.Articles, "ArticleSid", "ArticleSid", reply.ArticleSid);
        //        ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", reply.PublicStatusNo);
        //        return View(reply);
        //    }

        // GET: Replies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reply = await _context.Replies.FindAsync(id);
            if (reply == null)
            {
                return NotFound();
            }

            ViewBag.DeleteOrNot = new SelectList(new[]
{
        new { Value = true, Text = "是" },
        new { Value = false, Text = "否" }
    }, "Value", "Text", reply.DeleteOrNot);

            var articleTitle = await _context.Articles
                .Where(a => a.ArticleSid == reply.ArticleSid)
                .Select(a => a.ArticleTitle)
                .FirstOrDefaultAsync();

            var memberName = await _context.Members
                               .Where(m => m.MemberSid == reply.ArticleCreaterSid)
                               .Select(m => m.Name)
                               .FirstOrDefaultAsync();

            ViewData["ArticleTitle"] = articleTitle;
            ViewData["ArticleCreaterName"] = memberName;
            ViewData["ArticleSid"] = new SelectList(_context.Articles, "ArticleSid", "ArticleSid", reply.ArticleSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription", reply.PublicStatusNo);
            return View(reply);
        }

        // POST: Replies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReplySid,ArticleSid,ArticleCreaterSid,ReplyInfo,ReplyDate,ReplyUpdate,EditTimes,TagMemberNo,PublicStatusNo,DeleteOrNot")] Reply reply)
        {
            if (id != reply.ReplySid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalReply = await _context.Replies.AsNoTracking().FirstOrDefaultAsync(a => a.ReplySid == id);

                    if (originalReply == null)
                    {
                        return NotFound();
                    }

                    // 检查是否有实际更改 並且不檢查刪除跟公開狀態
                    bool hasChanges = originalReply.ReplyInfo != reply.ReplyInfo ||
                                                 originalReply.ReplySid != reply.ReplySid ||
                                                 originalReply.DeleteOrNot != reply.DeleteOrNot ||
                                                 originalReply.PublicStatusNo != reply.PublicStatusNo ||
                                                 originalReply.EditTimes != reply.EditTimes;


                    if (hasChanges)
                    {
                        // 只有在有更改时才保存编辑记录和更新文章数据
                        //var editHistory = new Edit
                        //{
                        //    ReplySid = reply.ReplySid,
                        //    EditBefore = originalReply.ReplyInfo,  // 保存编辑前的信息
                        //    EditAfter = reply.ReplyInfo,  // 保存编辑后的信息
                        //};

                        //_context.Edits.Add(editHistory);  // 保存编辑历史

                        //reply.EditTimes = (reply.EditTimes ?? 0) + 1;

                        //reply.ReplyUpdate = DateTime.Now;
                        _context.Entry(reply).Property(a => a.ReplyDate).IsModified = false;
                        _context.Entry(reply).Property(a => a.ReplyUpdate).IsModified = false;
                        _context.Update(reply);
                        
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReplyExists(reply.ReplySid))
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

            ViewBag.DeleteOrNot = new SelectList(new[]
{
        new { Value = true, Text = "是" },
        new { Value = false, Text = "否" }
    }, "Value", "Text", reply.DeleteOrNot);

            var articleTitle = await _context.Articles
    .Where(a => a.ArticleSid == reply.ArticleSid)
    .Select(a => a.ArticleTitle)
    .FirstOrDefaultAsync();

            var memberName = await _context.Members
                               .Where(m => m.MemberSid == reply.ArticleCreaterSid)
                               .Select(m => m.Name)
                               .FirstOrDefaultAsync();

            ViewData["ArticleTitle"] = articleTitle;
            ViewData["ArticleCreaterName"] = memberName;
            ViewData["ArticleSid"] = new SelectList(_context.Articles, "ArticleSid", "ArticleSid", reply.ArticleSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription", reply.PublicStatusNo);
            return View(reply);
        }

        // GET: Replies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reply = await _context.Replies
                .Include(r => r.ArticleCreaterS)
                .Include(r => r.ArticleS)
                .Include(r => r.PublicStatusNoNavigation)
                .FirstOrDefaultAsync(m => m.ReplySid == id);
            if (reply == null)
            {
                return NotFound();
            }

            return View(reply);
        }

        // POST: Replies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reply = await _context.Replies.FindAsync(id);
            if (reply != null)
            {
                _context.Replies.Remove(reply);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReplyExists(int id)
        {
            return _context.Replies.Any(e => e.ReplySid == id);
        }
        public async Task<IActionResult> SearchPartial(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var replies = GetFilteredReplies(searchString);

            return PartialView("_ReplyPartial", await replies.ToListAsync());
        }
        private IQueryable<Reply> GetFilteredReplies(string searchString)
        {
            var replies = _context.Replies
                                  .Include(r => r.ArticleCreaterS)
                                  .Include(r => r.ArticleS)
                                  .Include(r => r.PublicStatusNoNavigation)
                                  .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                replies = replies.Where(r => r.ArticleS.ArticleTitle.Contains(searchString));
            }

            return replies;
        }
    }
}
