using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using System.Security.Claims;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class ArticlesController : Controller
    {
        private readonly EggPlatformContext _context;

        public ArticlesController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Articles
        public async Task<IActionResult> Index(string searchString)
        {
            //搜尋後保留搜尋的關鍵字
            ViewData["CurrentFilter"] = searchString;

            var articles = from a in _context.Articles
                           .Include(a => a.ArticleCategoriesS)
                           .Include(a => a.ArticleCreaterS)
                           .Include(a => a.PublicStatusNoNavigation)
                           select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(s => s.ArticleTitle.Contains(searchString));
            }
            int totalArticles = await articles.CountAsync();
            ViewData["TotalArticles"] = totalArticles;
            return View(await articles.ToListAsync());
        }
        //後臺不需要
        // GET: Articles/Details/5 
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var article = await _context.Articles
        //        .Include(a => a.ArticleCategoriesS)
        //        .Include(a => a.ArticleCreaterS)
        //        .Include(a => a.PublicStatusNoNavigation)
        //        .FirstOrDefaultAsync(m => m.ArticleSid == id);
        //    if (article == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(article);
        //}

        // GET: Articles/Create
        public IActionResult Create()
        {
            var categories = _context.ArticleCategories
                                     .Where(c => c.ArticleCategoriesSid == 1)
                                     .ToList();

            var statuses = _context.PublicStatuses
                       .Select(s => new { s.PublicStatusNo, s.StatusDescription })
                       .ToList();




            ViewData["ArticleCategoriesSid"] = new SelectList(categories, "ArticleCategoriesSid", "ArticleCategories");
            ViewData["ArticleCreaterSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid");
            // 创建下拉列表，显示 StatusDescription 而非 PublicStatusNo
            ViewData["PublicStatusNo"] = new SelectList(statuses, "PublicStatusNo", "StatusDescription");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleSid,ArticleCreaterSid,ArticleTitle,ArticleInfo,ArticleCategoriesSid,ArticleDate,ArticleUpdate,EditCountTimes,TagMemberNo,PublicStatusNo,DeleteOrNot")] Article article)
        {
            // 检查用户是否登录
            if (User.Identity.IsAuthenticated)
            {
                // 获取当前登录用户的 ID，并转换为整数类型
                string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                {
                    // 如果转换失败，返回错误视图
                    ModelState.AddModelError(string.Empty, "无法获取有效的用户 ID。");
                    return View(article);
                }
                article.ArticleCreaterSid = userId;
            }
            else
            {
                // 如果用户未登录，将 ArticleCreaterSid 设置为 1
                article.ArticleCreaterSid = 1;
            }

            if (ModelState.IsValid)
            {
                // 设置当前时间
                var currentTime = DateTime.Now;
                article.ArticleDate = currentTime;
                article.ArticleUpdate = currentTime;

                // 创建设定 EditCountTimes 为 0
                article.EditCountTimes = 0;
                article.DeleteOrNot = false;
                // 保存数据
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // 重新加载下拉列表的内容
            ViewData["ArticleCategoriesSid"] = new SelectList(_context.ArticleCategories, "ArticleCategoriesSid", "ArticleCategoriesName", article.ArticleCategoriesSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription", article.PublicStatusNo);

            return View(article);
        }


        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            //取得文章作者的名字
            var articleCreaterName = await _context.Members
            .Where(m => m.MemberSid == article.ArticleCreaterSid)
            .Select(m => m.Name)
            .FirstOrDefaultAsync();

            var categories = await _context.ArticleCategories
                .Where(c => c.ArticleCategoriesSid == article.ArticleCategoriesSid)
                .Select(c => c.ArticleCategories)
                .FirstOrDefaultAsync();
            //將刪除文章做成下拉式
            ViewBag.DeleteOrNot = new SelectList(new[]
    {
        new { Value = true, Text = "是" },
        new { Value = false, Text = "否" }
    }, "Value", "Text", article.DeleteOrNot);

            ViewData["ArticleCreaterName"] = articleCreaterName;
            ViewData["ArticleCategories"] = categories;
            //ViewData["ArticleCategoriesSid"] = new SelectList(_context.ArticleCategories, "ArticleCategoriesSid", "ArticleCategories", article.ArticleCategoriesSid);
            //ViewData["ArticleCreaterSid"] = new SelectList(_context.Members, "MemberSid", "Name", article.ArticleCreaterSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription", article.PublicStatusNo);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleSid,ArticleCreaterSid,ArticleTitle,ArticleInfo,ArticleCategoriesSid,ArticleDate,ArticleUpdate,EditCountTimes,TagMemberNo,PublicStatusNo,DeleteOrNot")] Article article)
        {
            if (id != article.ArticleSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 获取数据库中原始的文章数据
                    var originalArticle = await _context.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.ArticleSid == id);

                    if (originalArticle == null)
                    {
                        return NotFound();
                    }

                    // 检查是否有实际更改 並且不檢查刪除跟公開狀態
                    bool hasChanges = originalArticle.ArticleTitle != article.ArticleTitle ||
                                                 originalArticle.ArticleInfo != article.ArticleInfo ||
                                                 originalArticle.ArticleCategoriesSid != article.ArticleCategoriesSid ||
                                                 originalArticle.TagMemberNo != article.TagMemberNo;

                    if (hasChanges)
                    {
                        // 只有在有更改时才保存编辑记录和更新文章数据
                        var editHistory = new Edit
                        {
                            ArticleSid = article.ArticleSid,
                            EditBefore = originalArticle.ArticleInfo,  // 保存编辑前的信息
                            EditAfter = article.ArticleInfo,  // 保存编辑后的信息
                            EditTime = DateOnly.FromDateTime(DateTime.Now).ToDateTime(new TimeOnly(0, 0))  // 只保存日期
                        };

                        _context.Edits.Add(editHistory);  // 保存编辑历史

                        // 更新为编辑后的时间
                        article.ArticleUpdate = DateTime.Now;

                        // 增加编辑次数
                        article.EditCountTimes = (article.EditCountTimes ?? 0) + 1;


                        //不要更改創建文章時間
                        _context.Entry(article).Property(a => a.ArticleDate).IsModified = false;

                        _context.Update(article);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleSid))
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

            ViewData["ArticleCategoriesSid"] = new SelectList(_context.ArticleCategories, "ArticleCategoriesSid", "ArticleCategories", article.ArticleCategoriesSid);
            ViewData["ArticleCreaterSid"] = new SelectList(_context.Members, "MemberSid", "Name", article.ArticleCreaterSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription", article.PublicStatusNo);

            return View(article);
        }

        //後台不需要
        // GET: Articles/Delete/5
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var article = await _context.Articles
        //            .Include(a => a.ArticleCategoriesS)
        //            .Include(a => a.ArticleCreaterS)
        //            .Include(a => a.PublicStatusNoNavigation)
        //            .FirstOrDefaultAsync(m => m.ArticleSid == id);
        //        if (article == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(article);
        //    }

        //    // POST: Articles/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        var article = await _context.Articles.FindAsync(id);
        //        if (article != null)
        //        {
        //            _context.Articles.Remove(article);
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleSid == id);
        }
    }
}

