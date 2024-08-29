using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using System.Security.Claims;
using EggProductionProject_MVC.Areas.Frontstage.Controllers;
using static EggProductionProject_MVC.Areas.Frontstage.Controllers.ArticlesDTOController;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    [Route("api/[controller]")]
    //[ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly EggPlatformContext _context;

        public ArticlesController(EggPlatformContext context)
        {
            _context = context;
        }

        [HttpGet("GetArticles")]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles()
        {
            var articles = await _context.Articles
                .Include(a => a.ArticleCreaterS)
                .Include(a => a.ArticleCategoriesS)
                .Include(a => a.PublicStatusNoNavigation)
                .Select(article => new ArticleDto
                {
                    ArticleSid = article.ArticleSid,
                    ArticleTitle = article.ArticleTitle,
                    ArticleDate = article.ArticleDate,
                    ArticleInfo = article.ArticleInfo,
                    ArticleCreator = article.ArticleCreaterS != null
                        ? new MemberDto
                        {
                            MemberSid = article.ArticleCreaterS.MemberSid,
                            Name = article.ArticleCreaterS.Name,
                            Email = article.ArticleCreaterS.Email,
                            Phone = article.ArticleCreaterS.Phone,
                            BirthDate = article.ArticleCreaterS.BirthDate,
                            UserName = article.ArticleCreaterS.UserName,
                            ProfilePic = article.ArticleCreaterS.ProfilePic
                        }
                        : null,
                    ArticleCategory = article.ArticleCategoriesS != null
                        ? new ArticleCategoryDto
                        {
                            ArticleCategoriesSid = article.ArticleCategoriesS.ArticleCategoriesSid,
                            ArticleCategories = article.ArticleCategoriesS.ArticleCategories,
                            ArticleCategoriesImg = article.ArticleCategoriesS.ArticleCategoriesImg != null
                                ? Convert.ToBase64String(article.ArticleCategoriesS.ArticleCategoriesImg)
                                : null
                        }
                        : null,
                    PublicStatus = article.PublicStatusNoNavigation != null
                        ? new PublicStatusDto
                        {
                            PublicStatusNo = article.PublicStatusNoNavigation.PublicStatusNo,
                            StatusDescription = article.PublicStatusNoNavigation.StatusDescription
                        }
                        : null
                })
                .ToListAsync();

            return Ok(articles);
        }

        // GET: api/Articles/5
        [HttpGet("GetArticleDetails/{id}")]
        public async Task<ActionResult<ArticleDto>> GetArticleDetails(int id)
        {
            var article = await _context.Articles
                .Include(a => a.ArticleCreaterS)
                .Include(a => a.ArticleCategoriesS)
                .Include(a => a.PublicStatusNoNavigation)
                .Include(a => a.Edits)
                .Include(a => a.GoodorBads)
                .Include(a => a.Replies)
                .ThenInclude(r => r.ArticleCreaterS)
                .FirstOrDefaultAsync(a => a.ArticleSid == id);

            if (article == null)
            {
                return NotFound();
            }

            var articleDto = new ArticleDto
            {
                ArticleSid = article.ArticleSid,
                ArticleTitle = article.ArticleTitle,
                ArticleDate = article.ArticleDate,
                ArticleInfo = article.ArticleInfo,
                ArticleCategory = article.ArticleCategoriesS != null
                    ? new ArticleCategoryDto
                    {
                        ArticleCategoriesSid = article.ArticleCategoriesS.ArticleCategoriesSid,
                        ArticleCategories = article.ArticleCategoriesS.ArticleCategories
                    }
                    : null,
                ArticleCreator = article.ArticleCreaterS != null
                    ? new MemberDto
                    {
                        MemberSid = article.ArticleCreaterS.MemberSid,
                        Name = article.ArticleCreaterS.Name,
                        Email = article.ArticleCreaterS.Email,
                        Phone = article.ArticleCreaterS.Phone,
                        BirthDate = article.ArticleCreaterS.BirthDate,
                        UserName = article.ArticleCreaterS.UserName,
                        ProfilePic = article.ArticleCreaterS.ProfilePic
                    }
                    : null,
                PublicStatus = article.PublicStatusNoNavigation != null
                    ? new PublicStatusDto
                    {
                        PublicStatusNo = article.PublicStatusNoNavigation.PublicStatusNo,
                        StatusDescription = article.PublicStatusNoNavigation.StatusDescription
                    }
                    : null,
                Edits = article.Edits.Select(edit => new EditDto
                {
                    EditSid = edit.EditSid,
                    EditDetails = edit.EditBefore,
                    EditTime = edit.EditTime
                }).ToList(),
                GoodorBads = article.GoodorBads.Select(gb => new GoodorBadDto
                {
                    GorBsid = gb.GorBsid,
                    GorBdate = gb.GorBdate,
                    GorBtype = gb.GorBtype
                }).ToList(),
                Replies = article.Replies.Select(reply => new ReplyDto
                {
                    ReplySid = reply.ReplySid,
                    ReplyInfo = reply.ReplyInfo,
                    ReplyDate = reply.ReplyDate,
                    EditTimes = reply.EditTimes,
                    ArticleCreater = reply.ArticleCreaterS != null
                        ? new MemberDto
                        {
                            MemberSid = reply.ArticleCreaterS.MemberSid,
                            Name = reply.ArticleCreaterS.Name
                        }
                        : null
                }).ToList()
            };

            return Ok(articleDto);
        }


       

        //public async Task<IActionResult> GetArticles()
        //{
        //    var articles = await _context.Articles
        //        .Select(a => new
        //        {
        //            a.ArticleSid,
        //            a.ArticleTitle,
        //            a.ArticleInfo,
        //            a.ArticleCreaterSid,
        //            a.ArticleCategoriesSid,
        //            a.ArticleDate
        //        })
        //        .ToListAsync();

        //    return Ok(articles);
        //}
        ////返回
        //[HttpGet("GetCategories")]
        //public async Task<IActionResult> GetCategories()
        //{
        //    var categories = await _context.ArticleCategories
        //        .Select(c => new
        //        {
        //            c.ArticleCategoriesSid,
        //            c.ArticleCategories,
        //            ArticleCategoriesImg = c.ArticleCategoriesImg != null ? Convert.ToBase64String(c.ArticleCategoriesImg) : null
        //        })
        //        .ToListAsync();

        //    return Ok(categories);
        //}
        ////文章內頁
        //[HttpGet("GetArticleJson/{id}")]
        //public async Task<IActionResult> GetArticleJson(int id)
        //{
        //    var article = await _context.Articles
        //        .Include(a => a.ArticleCategoriesS)
        //        .Include(a => a.ArticleCreaterS)
        //        .Include(a => a.PublicStatusNoNavigation)
        //        .Where(a => a.ArticleSid == id)
        //        .Select(a => new
        //        {
        //            a.ArticleSid,
        //            a.ArticleTitle,
        //            a.ArticleInfo,
        //            a.ArticleCreaterSid,
        //            ArticleCreaterName = a.ArticleCreaterS.Name,
        //            a.ArticleCategoriesSid,
        //            CategoryName = a.ArticleCategoriesS.ArticleCategories,
        //            a.ArticleDate,
        //            a.PublicStatusNoNavigation.StatusDescription
        //        })
        //        .FirstOrDefaultAsync();

        //    if (article == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(article);
        //}

        // 文章分類詳細
        //[HttpGet("GetCategoryJson/{id}")]
        //public async Task<IActionResult> GetCategoryJson(int id)
        //{
        //    var category = await _context.ArticleCategories
        //        .Where(c => c.ArticleCategoriesSid == id)
        //        .Select(c => new
        //        {
        //            c.ArticleCategoriesSid,
        //            c.ArticleCategories,
        //            ArticleCategoriesImg = c.ArticleCategoriesImg != null ? Convert.ToBase64String(c.ArticleCategoriesImg) : null
        //        })
        //        .FirstOrDefaultAsync();

        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(category);
        //}
    }
}