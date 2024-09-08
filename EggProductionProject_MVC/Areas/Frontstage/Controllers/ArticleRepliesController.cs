using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using static EggProductionProject_MVC.Areas.Frontstage.Controllers.ArticlesDTOController;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    [Route("api/[controller]")]
    public class ArticleRepliesController : ControllerBase
    {
        private readonly EggPlatformContext _context;

        public ArticleRepliesController(EggPlatformContext context)
        {
            _context = context;
        }
        [HttpGet("GetArticleReplies/{id}")]
        public async Task<ActionResult<List<ReplyDto>>> GetArticleReplies(int id)
        {
            // 查询回复
            var replies = await _context.Replies
                .Where(reply => reply.ArticleSid == id)
                .Include(reply => reply.ArticleCreatorS)
                .OrderByDescending(reply => reply.ReplyDate)
                .Select(reply => new ReplyDto
                {
                    ReplySid = reply.ReplySid,
                    ReplyInfo = reply.ReplyInfo,
                    ReplyDate = reply.ReplyDate,
                    EditTimes = reply.EditTimes,
                    ArticleCreator = reply.ArticleCreatorS != null
                        ? new MemberDto
                        {
                            MemberSid = reply.ArticleCreatorS.MemberSid,
                            Name = reply.ArticleCreatorS.Name
                        }
                        : null
                })
                .ToListAsync();

            // 查询每个回复的按赞和点噓数
            var reactions = await _context.GoodorBads
                .Where(g => g.ReplySid.HasValue && replies.Select(r => r.ReplySid).Contains(g.ReplySid.Value))
                .GroupBy(g => g.ReplySid)
                .Select(g => new ReplyReactionCountDto
                {
                    ReplySid = g.Key.Value,  // 确保 ReplySid 非空
                    LikeCount = g.Count(gb => gb.GorBtype == 1),
                    DislikeCount = g.Count(gb => gb.GorBtype == 0)
                })
                .ToListAsync();

            // 将 reactions 信息合并到 replies 中
            foreach (var reply in replies)
            {
                var reaction = reactions.FirstOrDefault(r => r.ReplySid == reply.ReplySid);
                if (reaction != null)
                {
                    reply.LikeCount = reaction.LikeCount;
                    reply.DislikeCount = reaction.DislikeCount;
                }
            }

            return Ok(replies);
        }
    }
}
