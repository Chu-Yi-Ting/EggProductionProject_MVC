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
            var replies = await _context.Replies
                .Where(reply => reply.ArticleSid == id)
                .Include(reply => reply.ArticleCreaterS)
                .Select(reply => new ReplyDto
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
                })
                .ToListAsync();

            return Ok(replies);
        }
    }
}
