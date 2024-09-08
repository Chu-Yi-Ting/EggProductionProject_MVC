﻿using System;
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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

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
                .Where(a => a.PublicStatusNoNavigation != null && a.PublicStatusNoNavigation.PublicStatusNo == 1)//只傳公開的文章
                .Where(a =>a.DeleteOrNot !=null && a.DeleteOrNot ==false) //只傳未被刪除的文章
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
                            //讀圖太卡了先不讀 看他有沒有要轉路徑
                            //ProfilePic = article.ArticleCreaterS.ProfilePic
                        }
                        : null,
                    ArticleCategory = article.ArticleCategoriesS != null
                        ? new ArticleCategoryDto
                        {
                            ArticleCategoriesSid = article.ArticleCategoriesS.ArticleCategoriesSid,
                            ArticleCategories = article.ArticleCategoriesS.ArticleCategories
                        }
                        : null,
                    PublicStatus = article.PublicStatusNoNavigation != null
                        ? new PublicStatusDto
                        {
                            PublicStatusNo = article.PublicStatusNoNavigation.PublicStatusNo,
                        }
                        : null
                })
                .ToListAsync();

            return Ok(articles);
        }
        //接收文章寫入資料庫
        // POST: api/Articles/PostArticle
        [HttpPost("PostArticle")]
        public async Task<ActionResult<Article>> PostArticle([FromForm] ArticleDto articleDto)
        {
            // 輸出 DTO 的內容以確認是否成功接收到資料
            Console.WriteLine("Received ArticleTitle: " + articleDto.ArticleTitle);
            Console.WriteLine("Received ArticleInfo: " + articleDto.ArticleInfo);
            // 將接收的 DTO 資料轉換為資料庫模型
            var newArticle = new Article
            {
                ArticleTitle = articleDto.ArticleTitle,
                ArticleInfo = articleDto.ArticleInfo,
                ArticleDate = DateTime.UtcNow, // 以當前時間作為創建時間
                ArticleUpdate = DateTime.UtcNow,//當前時間
                ArticleCategoriesSid = articleDto.ArticleCategory?.ArticleCategoriesSid, // 確保傳遞了分類SID
                ArticleCreaterSid = articleDto.ArticleCreator?.MemberSid, // 確保有會員ID
                PublicStatusNo = articleDto.PublicStatus?.PublicStatusNo, // 公開狀態
                EditCountTimes = 0,
                DeleteOrNot = false // 預設不刪除
            };

            // 將新文章加入資料庫
            _context.Articles.Add(newArticle);
            await _context.SaveChangesAsync(); // 寫入資料庫

            return CreatedAtAction("GetArticleDetails", new { id = newArticle.ArticleSid }, newArticle);
        }

        //刪除文章
        [HttpPost("SoftDeleteArticle/{id}")]
        public async Task<IActionResult> SoftDeleteArticle([FromForm] int articleId)
        {
            Console.WriteLine(articleId);
            // 將接收的 DTO 資料轉換為資料庫模型
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                return NotFound(); // 如果文章不存在，返回404
            }
            article.DeleteOrNot = true;

            // 只更新指定的字段
            _context.Entry(article).Property(a => a.DeleteOrNot).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent(); // 返回无内容
        }

        // GET: api/Articles/5
        [HttpGet("GetArticleDetails/{id}")]
        public async Task<ActionResult<ArticleDto>> GetArticleDetails(int id)
        {
            var article = await _context.Articles
                .Include(a => a.ArticleCreaterS)
                .Include(a => a.ArticleCategoriesS)
                .Include(a => a.PublicStatusNoNavigation)
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
                        //ProfilePic = article.ArticleCreaterS.ProfilePic
                    }
                    : null,
                PublicStatus = article.PublicStatusNoNavigation != null
                    ? new PublicStatusDto
                    {
                        PublicStatusNo = article.PublicStatusNoNavigation.PublicStatusNo,
                        StatusDescription = article.PublicStatusNoNavigation.StatusDescription
                    }
                    : null,
            };
            return Ok(articleDto);
        }
        //這邊處理文章列表按讚跟噓
        [HttpGet("GetArticlesReactionCounts")]
        public async Task<ActionResult<List<ArticleReactionCountDto>>> GetArticlesReactionCounts()
        {
            var reactionCounts = await _context.GoodorBads
                .GroupBy(gb => gb.ArticleSid)
                .Select(g => new ArticleReactionCountDto
                {
                    ArticleSid = g.Key?? 0,
                    LikeCount = g.Count(gb => gb.GorBtype == 1),  // 計算按讚的數量
                    DislikeCount = g.Count(gb => gb.GorBtype == 0)  // 計算不喜歡的數量
                })
                .ToListAsync();

            return Ok(reactionCounts);
        }
        //文章內頁的讚噓
        [HttpGet("GetArticleReactions/{id}")]
        public async Task<ActionResult<List<GoodorBadDto>>> GetArticleReactions(int id)
        {
            // 首先，獲取按讚和點噓的數量
            var reactionCounts = await _context.GoodorBads
                .Where(gb => gb.ArticleSid == id && gb.ReplySid == null)
                .GroupBy(gb => gb.ArticleSid)
                .Select(g => new ArticleReactionCountDto
                {
                    ArticleSid = g.Key ?? 0,
                    LikeCount = g.Count(gb => gb.GorBtype == 1),  // 計算按讚的數量
                    DislikeCount = g.Count(gb => gb.GorBtype == 0),  // 計算點噓的數量
                    Reactions = g.Select(gb => new GoodorBadDto
                    {
                        GorBsid = gb.GorBsid,
                        MemberNo = gb.MemberNo,
                        Member = gb.MemberNoNavigation != null
                    ? new MemberDto
                    {
                        MemberSid = gb.MemberNoNavigation.MemberSid,
                        Name = gb.MemberNoNavigation.Name,
                        //ProfilePic = gb.MemberNoNavigation.ProfilePic
                    }:null,
                        GorBdate = gb.GorBdate,
                        GorBtype = gb.GorBtype,
                        ArticleSid = gb.ArticleSid,
                        ReplySid = gb.ReplySid
                    }).ToList()  // 返回每個用戶的按讚或點噓詳細信息
                })
                .FirstOrDefaultAsync();

            if (reactionCounts == null)
            {
                return NotFound("沒有找到此文章的反應資料");
            }

            return Ok(reactionCounts);
        }
        [HttpGet("GetArticleEdits/{id}")]
        public async Task<ActionResult<List<EditDto>>> GetArticleEdits(int id)
        {
            var edits = await _context.Edits
                .Where(edit => edit.ArticleSid == id)
                .Select(edit => new EditDto
                {
                    EditSid = edit.EditSid,
                    EditDetails = edit.EditBefore,
                    EditTime = edit.EditTime
                })
                .ToListAsync();

            return Ok(edits);
        }
        // 文章分類詳細
        [HttpGet("GetCategories")]
        public async Task<ActionResult<IEnumerable<ArticleCategoryDto>>> GetCategories()
        {
            var categories = await _context.ArticleCategories
                .Select(c => new ArticleCategoryDto
                {
                    ArticleCategoriesSid = c.ArticleCategoriesSid,
                    ArticleCategories = c.ArticleCategories,
                    ArticleCategoriesImg = c.ArticleCategoriesImg != null
                        ? Convert.ToBase64String(c.ArticleCategoriesImg)
                        : null
                })
                .ToListAsync();

            return Ok(categories);
        }

        //測試壓縮圖片 裝了nuget套件SixLabors.ImageSharp
        //沒有效改善 停止使用
        //public async Task<ActionResult<IEnumerable<ArticleCategoryDto>>> GetCategories()
        //{
        //    var categories = await _context.ArticleCategories.ToListAsync();

        //    var categoryDtos = new List<ArticleCategoryDto>();

        //    foreach (var category in categories)
        //    {
        //        var dto = new ArticleCategoryDto
        //        {
        //            ArticleCategoriesSid = category.ArticleCategoriesSid,
        //            ArticleCategories = category.ArticleCategories,
        //            ArticleCategoriesImg = category.ArticleCategoriesImg != null
        //                ? await CompressImageAsync(category.ArticleCategoriesImg)
        //                : null
        //        };
        //        categoryDtos.Add(dto);
        //    }
        //    return Ok(categoryDtos);
        //}
        //private async Task<string> CompressImageAsync(byte[] imageBytes)
        //{
        //    using (var inputStream = new MemoryStream(imageBytes))
        //    using (var image = await Image.LoadAsync(inputStream))
        //    {
        //        image.Mutate(x => x.Resize(new ResizeOptions
        //        {
        //            Size = new Size(400, 0), // 調整寬度，保持比例
        //            Mode = ResizeMode.Max
        //        }));

        //        using (var outputStream = new MemoryStream())
        //        {
        //            var jpegEncoder = new JpegEncoder
        //            {
        //                Quality = 1 // 設定壓縮質量（0-100）
        //            };
        //            await image.SaveAsync(outputStream, jpegEncoder);

        //            return Convert.ToBase64String(outputStream.ToArray());
        //        }
        //    }
        //}
    }
}