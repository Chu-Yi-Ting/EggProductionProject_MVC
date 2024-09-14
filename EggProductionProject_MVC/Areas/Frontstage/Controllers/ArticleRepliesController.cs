using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using static EggProductionProject_MVC.Areas.Frontstage.Controllers.ArticlesDTO;

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

        //取得文章回覆
        [HttpGet("GetArticleReplies/{id}")]
        public async Task<ActionResult<List<ReplyDto>>> GetArticleReplies(int id)
        {
            // 查询回复
            var replies = await _context.Replies
                .Where(reply => reply.ArticleSid == id && reply.DeleteOrNot == false && reply.PublicStatusNo == 1)
                .Include(reply => reply.ArticleCreaterS)
                .OrderByDescending(reply => reply.ReplyDate)
                .Select(reply => new ReplyDto
                {
                    ReplySid = reply.ReplySid,
                    ReplyInfo = reply.ReplyInfo,
                    ReplyDate = reply.ReplyDate,
                    EditTimes = reply.EditTimes,
                    ArticleCreator = reply.ArticleCreaterS != null
                        ? new MemberDto
                        {
                            MemberSid = reply.ArticleCreaterS.MemberSid,
                            Name = reply.ArticleCreaterS.Name,
                            ProfilePic = "https://localhost:7080/" + reply.ArticleCreaterS.ProfilePic,
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
        //取得編輯文章回復
        [HttpGet("GetReplyById/{id}")]
        public async Task<ActionResult<ReplyDto>> GetReplyById(int id)
        {
            var reply = await _context.Replies
                .Where(reply => reply.ReplySid == id && reply.DeleteOrNot == false && reply.PublicStatusNo == 1)
                .Include(reply => reply.ArticleCreaterS)
                .Select(reply => new ReplyDto
                {
                    ReplySid = reply.ReplySid,
                    ReplyInfo = reply.ReplyInfo,
                    ReplyDate = reply.ReplyDate,
                    EditTimes = reply.EditTimes,
                    ArticleCreator = reply.ArticleCreaterS != null
                        ? new MemberDto
                        {
                            MemberSid = reply.ArticleCreaterS.MemberSid,
                            Name = reply.ArticleCreaterS.Name,
                            ProfilePic = reply.ArticleCreaterS.ProfilePic,
                        }
                        : null,
                    LikeCount = 0,
                    DislikeCount = 0,
                    LikedByUsers = new List<MemberDto>(),
                    DislikedByUsers = new List<MemberDto>(),
                    ArticleSid = reply.ArticleSid ?? 0,
                })
                .FirstOrDefaultAsync();

            if (reply == null)
            {
                return NotFound();
            }

            return Ok(reply);
        }
        // 對回覆按讚
        [HttpPost("LikeReply/{replyId}")]
        public async Task<IActionResult> LikeReply(int replyId)
        {
            var memberNo = HttpContext.Session.GetInt32("userMemberSid"); // 示例中硬编码用户ID，实际情况中需要获取当前登录用户ID

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
                    existingReaction.GorBdate = DateTime.UtcNow;
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
                    GorBdate = DateTime.UtcNow
                };

                _context.GoodorBads.Add(newReaction);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "已成功按讚" });
        }

        // 對回覆按噓
        [HttpPost("DislikeReply/{replyId}")]
        public async Task<IActionResult> DislikeReply(int replyId)
        {
            var memberNo = HttpContext.Session.GetInt32("userMemberSid"); // 示例中硬编码用户ID，实际情况中需要获取当前登录用户ID

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
                    existingReaction.GorBdate = DateTime.UtcNow;
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
                    GorBdate = DateTime.UtcNow
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

        //發表回覆
        [HttpPost("PostReply/{id}")]
        public async Task<ActionResult<Reply>> PostReply([FromForm] ReplyDto replyDto)
        {
            // 輸出 DTO 的內容以確認是否成功接收到資料
            Console.WriteLine("Received ArticleInfo: " + replyDto.ReplyInfo);
            // 將接收的 DTO 資料轉換為資料庫模型
            var newReply = new Reply
            {
                ArticleSid = replyDto.ArticleSid,
                ReplyInfo = replyDto.ReplyInfo,
                ReplyDate = DateTime.UtcNow, // 以當前時間作為創建時間
                ReplyUpdate = DateTime.UtcNow,//當前時間
                ArticleCreaterSid = replyDto.ArticleCreator?.MemberSid, // 確保有會員ID
                PublicStatusNo = replyDto.PublicStatus?.PublicStatusNo, // 公開狀態
                EditTimes = 0,
                DeleteOrNot = false // 預設不刪除
            };
            try
            {
                _context.Replies.Add(newReply);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // 处理异常，返回错误信息
                return StatusCode(500, "An error occurred while saving the reply: " + ex.Message);
            }
            // 將新文章加入資料庫
            return CreatedAtAction("GetReplyDetails", new { id = newReply.ReplySid }, newReply);
        }


        [HttpPut("UpdateReply/{id}")]
        public async Task<IActionResult> UpdateReply(int id, [FromForm] ReplyDto replydto)
        {
            var existingReply = await _context.Replies.FindAsync(id);
            if (existingReply == null)
            {
                return NotFound($"未找到ID為{id}的回復");
            }
            //編輯的紀錄
            var editHistory = new Edit
            {
                ReplySid = existingReply.ReplySid,
                EditBefore = existingReply.ReplyInfo,
                EditAfter = existingReply.ReplyInfo,
                EditTime = DateTime.UtcNow,
            };
            _context.Edits.Add(editHistory);


            existingReply.ReplyInfo = replydto.ReplyInfo;
            existingReply.PublicStatusNo = replydto.PublicStatus?.PublicStatusNo;
            existingReply.ReplyUpdate = DateTime.UtcNow;
            existingReply.EditTimes += 1;
            // 保存變更
            await _context.SaveChangesAsync();

            // 返回204 No Content表示成功，但無需返回內容
            return NoContent();
        }
    }
}
