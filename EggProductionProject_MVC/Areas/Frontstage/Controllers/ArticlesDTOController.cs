using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class ArticlesDTOController : Controller
    {
        private readonly EggPlatformContext _context;

        public ArticlesDTOController(EggPlatformContext context)
        {
            _context = context;
        }
        public class ArticleDto
        {
            public int ArticleSid { get; set; }
            public string? ArticleTitle { get; set; }
            public DateTime? ArticleDate { get; set; }
            public string ArticleInfo { get; set; } = string.Empty;

            public ArticleCategoryDto? ArticleCategory { get; set; }
            public MemberDto? ArticleCreator { get; set; }
            public PublicStatusDto? PublicStatus { get; set; }
            public List<EditDto> Edits { get; set; } = new();
            public List<GoodorBadDto> GoodorBads { get; set; } = new();
            public List<ReplyDto> Replies { get; set; } = new();
        }
        public class ArticleDetailDto
        {
            public int ArticleSid { get; set; }
            public string? ArticleTitle { get; set; }
            public string ArticleInfo { get; set; }
            public DateTime? ArticleDate { get; set; }
            public ArticleCategoryDto? ArticleCategory { get; set; }
            public MemberDto? ArticleCreator { get; set; }
            public PublicStatusDto? PublicStatus { get; set; }
            public List<EditDto> Edits { get; set; }
            public List<GoodorBadDto> GoodorBads { get; set; }
            public List<ReplyDto> Replies { get; set; }
        }
        public class ArticleCategoryDto
        {
            public int ArticleCategoriesSid { get; set; }
            public string? ArticleCategories { get; set; }
            public string? ArticleCategoriesImg { get; set; }
        }

        public class MemberDto
        {
            public int MemberSid { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public DateOnly? BirthDate { get; set; }
            public string? UserName { get; set; }
            public byte[]? ProfilePic { get; set; }
        }

        public class PublicStatusDto
        {
            public int PublicStatusNo { get; set; }
            public string? StatusDescription { get; set; }
        }

        public class EditDto
        {
            public int EditSid { get; set; }
            public string? EditDetails { get; set; }
            public DateTime? EditTime { get; set; }
        }

        public class GoodorBadDto
        {
            public int GorBsid { get; set; }
            public int? MemberNo { get; set; }
            public MemberDto? Member { get; set; } // 添加MemberDto以显示用户信息
            public int? ArticleSid { get; set; }  // 新增文章 ID
            public int? ReplySid { get; set; }    // 新增回覆 ID
            public DateOnly? GorBdate { get; set; }
            public int GorBtype { get; set; }
        }
        public class ArticleReactionCountDto
        {
            public int ArticleSid { get; set; }
            public int LikeCount { get; set; }
            public int DislikeCount { get; set; }
            public List<GoodorBadDto> Reactions { get; set; }
        }
        public class ReplyDto
        {
            public int ReplySid { get; set; }
            public string ReplyInfo { get; set; } = null!;
            public DateTime? ReplyDate { get; set; }
            public int? EditTimes { get; set; }
            public MemberDto? ArticleCreator { get; set; }
            public int GorBtype { get; set; }
            public int LikeCount { get; set; }    // 新增按讚數
            public int DislikeCount { get; set; } // 新增點噓數
            public List<MemberDto> LikedByUsers { get; set; } // 点赞的用户
            public List<MemberDto> DislikedByUsers { get; set; } // 点噓的用户
        }
        public class ReplyReactionCountDto
        {
            public int ReplySid { get; set; }
            public int LikeCount { get; set; }
            public int DislikeCount { get; set; }        
        }
    }
}
