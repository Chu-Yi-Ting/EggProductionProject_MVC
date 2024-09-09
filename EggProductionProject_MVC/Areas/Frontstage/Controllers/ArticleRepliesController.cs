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
                .Where(reply => reply.ArticleSid == id && reply.DeleteOrNot ==false && reply.PublicStatusNo==1)
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
                            Name = reply.ArticleCreatorS.Name,
                            ProfilePic = reply.ArticleCreatorS.ProfilePic,
                        }
                        : null,
                    LikeCount = 0, // 初始化，稍后将填充
                    DislikeCount = 0, // 初始化，稍后将填充
                    LikedByUsers = new List<MemberDto>(), // 初始化点赞用户
                    DislikedByUsers = new List<MemberDto>() // 初始化点噓用户
                })
                .ToListAsync();

            // 查询每个回复的按赞和点噓数以及点赞、点噓的用户信息
            var reactions = await _context.GoodorBads
                .Where(g => g.ReplySid.HasValue && replies.Select(r => r.ReplySid).Contains(g.ReplySid.Value))
                .GroupBy(g => g.ReplySid)
                .Select(g => new
                {
                    ReplySid = g.Key.Value,
                    LikeCount = g.Count(gb => gb.GorBtype == 1),
                    DislikeCount = g.Count(gb => gb.GorBtype == 0),
                    LikedByUsers = g
                        .Where(gb => gb.GorBtype == 1)
                        .Select(gb => new MemberDto
                        {
                            MemberSid = gb.MemberNoNavigation.MemberSid,
                            Name = gb.MemberNoNavigation.Name
                        }).ToList(),
                    DislikedByUsers = g
                        .Where(gb => gb.GorBtype == 0)
                        .Select(gb => new MemberDto
                        {
                            MemberSid = gb.MemberNoNavigation.MemberSid,
                            Name = gb.MemberNoNavigation.Name
                        }).ToList()
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
                    reply.LikedByUsers = reaction.LikedByUsers;
                    reply.DislikedByUsers = reaction.DislikedByUsers;
                }
            }

            return Ok(replies);
        }
        // 按讚回复
        [HttpPost("LikeReply/{replyId}")]
        public async Task<IActionResult> LikeReply(int replyId)
        {
            var memberNo = 1; // 示例中硬编码用户ID，实际情况中需要获取当前登录用户ID

            if (memberNo == null)
            {
                return Unauthorized("用戶未登入");
            }

            var existingReaction = await _context.GoodorBads
                .FirstOrDefaultAsync(g => g.ReplySid == replyId && g.MemberNo == memberNo);

            if (existingReaction != null)
            {
                if (existingReaction.GorBtype == 1)
                {
                    return BadRequest("您已經對這則回覆按過讚");
                }
                else
                {
                    // 如果之前点了噓，现在切换为按赞
                    existingReaction.GorBtype = 1; // 更新为按讚
                    existingReaction.GorBdate = DateTime.Now;
                }
            }
            else
            {
                // 新增按赞
                var newReaction = new GoodorBad
                {
                    MemberNo = memberNo,
                    ReplySid = replyId,
                    GorBtype = 1, // 1 代表按讚
                    GorBdate = DateTime.Now
                };

                _context.GoodorBads.Add(newReaction);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "已成功按讚" });
        }

        // 點噓回复
        [HttpPost("DislikeReply/{replyId}")]
        public async Task<IActionResult> DislikeReply(int replyId)
        {
            var memberNo = 1; // 示例中硬编码用户ID，实际情况中需要获取当前登录用户ID

            if (memberNo == null)
            {
                return Unauthorized("用戶未登入");
            }

            var existingReaction = await _context.GoodorBads
                .FirstOrDefaultAsync(g => g.ReplySid == replyId && g.MemberNo == memberNo);

            if (existingReaction != null)
            {
                if (existingReaction.GorBtype == 0)
                {
                    return BadRequest("您已經對這則回覆點過噓");
                }
                else
                {
                    // 如果之前点了赞，现在切换为点噓
                    existingReaction.GorBtype = 0; // 更新为点噓
                    existingReaction.GorBdate = DateTime.Now;
                }
            }
            else
            {
                // 新增点噓
                var newReaction = new GoodorBad
                {
                    MemberNo = memberNo,
                    ReplySid = replyId,
                    GorBtype = 0, // 0 代表点噓
                    GorBdate = DateTime.Now
                };

                _context.GoodorBads.Add(newReaction);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "已成功點噓" });
        }

        //刪除回復
        [HttpPost("SoftDeleteReply/{id}")]
        public async Task<IActionResult> SoftDeleteReply([FromRoute] int id)
        {
            Console.WriteLine(id);
            // 將接收的 DTO 資料轉換為資料庫模型
            var reply = await _context.Replies.FindAsync(id);
            if (reply == null)
            {
                return NotFound(); // 如果文章不存在，返回404
            }
            reply.DeleteOrNot = true;

            // 只更新指定的字段
            _context.Entry(reply).Property(a => a.DeleteOrNot).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent(); // 返回无内容
        }
    }
}
