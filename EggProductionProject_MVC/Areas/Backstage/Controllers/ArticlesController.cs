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
            // 搜尋後保留搜尋的關鍵字
            ViewData["CurrentFilter"] = searchString;

            var articles = await GetFilteredArticlesAsync(searchString);
            int totalArticles = articles.Count();
            ViewData["TotalArticles"] = totalArticles;
            return View(articles);
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
        public IActionResult CreatePartial()
        {
            var categories = _context.ArticleCategories
                                     .Where(c => c.ArticleCategoriesSid == 1)
                                     .ToList();

            var statuses = _context.PublicStatuses
                       .Select(s => new { s.PublicStatusNo, s.StatusDescription })
                       .ToList();

            ViewData["ArticleCategoriesSid"] = new SelectList(categories, "ArticleCategoriesSid", "ArticleCategories");
            ViewData["ArticleCreaterSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid");
            // 建立下拉列表，顯示StatusDescription
            ViewData["PublicStatusNo"] = new SelectList(statuses, "PublicStatusNo", "StatusDescription");
            return PartialView("_CreateArticlePartial", new Article());
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial([Bind("ArticleSid,ArticleCreaterSid,ArticleTitle,ArticleInfo,ArticleCategoriesSid,ArticleDate,ArticleUpdate,EditCountTimes,TagMemberNo,PublicStatusNo,DeleteOrNot")] Article article)
        {
            // 檢查使用者是否登陸
            if (User.Identity.IsAuthenticated)
            {
                // 獲取用戶ID轉成INT
                string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                {
                    // 轉換失敗的話
                    ModelState.AddModelError(string.Empty, "並非有效使用者ID");
                    return PartialView("_CreateArticlePartial", article);
                }
                article.ArticleCreaterSid = userId;
            }
            else
            {
                //未登錄時使用使用者編號1 後台測試用 測試搞定後這邊要改成錯誤訊息
                article.ArticleCreaterSid = 1;
            }

            if (ModelState.IsValid)
            {
                // 設成當前時間
                var currentTime = DateTime.Now;
                article.ArticleDate = currentTime;
                article.ArticleUpdate = currentTime;

                // 創建時編輯次數為0
                article.EditCountTimes = 0;
                article.DeleteOrNot = false;
                // 保存
                _context.Add(article);
                await _context.SaveChangesAsync();
                // 返回成功訊息，以便在前端處理成功後的邏輯
                return Json(new { success = true });
            }

            // 重加載下拉列表
            ViewData["ArticleCategoriesSid"] = new SelectList(_context.ArticleCategories, "ArticleCategoriesSid", "ArticleCategories", article.ArticleCategoriesSid);
            ViewData["ArticleCreaterSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", article.ArticleCreaterSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription", article.PublicStatusNo);

            return PartialView("_CreateArticlePartial", article);
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
        public async Task<IActionResult> Edit(int id, [Bind("ArticleSid,ArticleCreaterSid,ArticleTitle,ArticleInfo,ArticleCategoriesSid,ArticleDate,ArticleUpdate,EditCountTimes,TagMemberNo,PublicStatusNo,DeleteOrNot,EditTime")] Article article)
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
                                                 originalArticle.TagMemberNo != article.TagMemberNo||
                                                 originalArticle.PublicStatusNo != article.PublicStatusNo || // 加上这行
                                                 originalArticle.DeleteOrNot != article.DeleteOrNot;

                    //給前台再用 編輯後更新進編輯資料庫
                    //if (hasChanges)
                    //{
                    //    // 只有在有更改时才保存编辑记录和更新文章数据
                    //    var editHistory = new Edit
                    //    {
                    //        ArticleSid = article.ArticleSid,
                    //        EditBefore = originalArticle.ArticleInfo,  // 保存编辑前的信息
                    //        EditAfter = article.ArticleInfo,  // 保存编辑后的信息
                    //        //改資料庫的類別後後換這個
                    //        EditTime = DateTime.Now
                    //        //EditTime = DateOnly.FromDateTime(DateTime.Now)  // 只保存日期
                    //    };

                    //    _context.Edits.Add(editHistory);  // 保存编辑历史

                    //    // 更新为编辑后的时间
                    //    article.ArticleUpdate = DateTime.Now;

                    //    // 增加编辑次数
                    //    //article.EditCountTimes = (article.EditCountTimes ?? 0) + 1;


                    //    //不要更改創建文章時間
                    _context.Entry(article).Property(a => a.ArticleDate).IsModified = false;
                    _context.Entry(article).Property(a => a.ArticleUpdate).IsModified = false;
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                    //}
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
        //回傳文章的局部頁面 局部更新用
        public async Task<IActionResult> SearchPartial(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var articles = await GetFilteredArticlesAsync(searchString);
            int totalArticles = articles.Count();

            ViewData["TotalArticles"] = totalArticles;
            return PartialView("_ArticlesPartial", articles);
        }
        //搜尋後獲取查詢列表的方法
        private async Task<List<Article>> GetFilteredArticlesAsync(string searchString)
        {
            var articles = from a in _context.Articles
                           .Include(a => a.ArticleCategoriesS)
                           .Include(a => a.ArticleCreaterS)
                           .Include(a => a.PublicStatusNoNavigation)
                           select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(s => s.ArticleTitle.Contains(searchString));
            }

            return await articles.ToListAsync();
        }

    }
}

