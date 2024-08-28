using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using System.Security.Claims;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly EggPlatformContext _context;

        public ArticlesController(EggPlatformContext context)
        {
            _context = context;
        }
        //不使用DTO因此將各資料傳送分割成不同API
        // 返回文章列表JSON
        [HttpGet("GetArticles")]
        public async Task<IActionResult> GetArticles()
        {
            var articles = await _context.Articles
                .Select(a => new
                {
                    a.ArticleSid,
                    a.ArticleTitle,
                    a.ArticleInfo,
                    a.ArticleCreaterSid,
                    a.ArticleCategoriesSid,
                    a.ArticleDate
                })
                .ToListAsync();

            return Ok(articles);
        }
        //返回
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.ArticleCategories
                .Select(c => new
                {
                    c.ArticleCategoriesSid,
                    c.ArticleCategories,
                    ArticleCategoriesImg = c.ArticleCategoriesImg != null ? Convert.ToBase64String(c.ArticleCategoriesImg) : null
                })
                .ToListAsync();

            return Ok(categories);
        }
        //文章內頁
        [HttpGet("GetArticleJson/{id}")]
        public async Task<IActionResult> GetArticleJson(int id)
        {
            var article = await _context.Articles
                .Include(a => a.ArticleCategoriesS)
                .Include(a => a.ArticleCreaterS)
                .Include(a => a.PublicStatusNoNavigation)
                .Where(a => a.ArticleSid == id)
                .Select(a => new
                {
                    a.ArticleSid,
                    a.ArticleTitle,
                    a.ArticleInfo,
                    a.ArticleCreaterSid,
                    ArticleCreaterName = a.ArticleCreaterS.Name,
                    a.ArticleCategoriesSid,
                    CategoryName = a.ArticleCategoriesS.ArticleCategories,
                    a.ArticleDate,
                    a.PublicStatusNoNavigation.StatusDescription
                })
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return NotFound();
            }

            return Ok(article);
        }

        // 文章分類詳細
        [HttpGet("GetCategoryJson/{id}")]
        public async Task<IActionResult> GetCategoryJson(int id)
        {
            var category = await _context.ArticleCategories
                .Where(c => c.ArticleCategoriesSid == id)
                .Select(c => new
                {
                    c.ArticleCategoriesSid,
                    c.ArticleCategories,
                    ArticleCategoriesImg = c.ArticleCategoriesImg != null ? Convert.ToBase64String(c.ArticleCategoriesImg) : null
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
    }
}