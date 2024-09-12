using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.ViewModels;
using Azure.Messaging;
using System.Diagnostics.Metrics;
using System.ComponentModel.DataAnnotations;
using NuGet.Protocol;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Hosting;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{

    [Area("Frontstage")]
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class VideoSummariesController : Controller
    {
        private readonly EggPlatformContext _context;

        public VideoSummariesController(EggPlatformContext _context)
        {
            this._context = _context;
        }

        // GET: Frontstage/VideoSummaries
        public async Task<IActionResult> Index()
        {
            //var eggPlatformContext = _context.VideoSummaries
            //    .Include(v => v.CreatorS)
            //    .Include(v => v.NatureS)
            //    .Include(v => v.PublicStatusNoNavigation)
            //    .Include(v => v.ScreenTextS);

            var show = _context.VideoSummaries
                .Include(v => v.CreatorS)
                .Include(v => v.NatureS)
                .Include(v => v.PublicStatusNoNavigation)
                .Include(v => v.ScreenTextS)
                .Include(v => v.CreatorS.MemberS) //UserName是帳號
                .Where(v => v.PublicStatusNoNavigation.PublicStatusNo == 1)
                .Select(x => new VideoSummaryDTO
                {
                    VideoSid = x.VideoSid,
                    CreatorSid = x.CreatorSid,
                    VideoDuration = x.VideoDuration,
                    VideoTitle = x.VideoTitle,
                    MemberName = x.CreatorS.MemberS.Name,
                    TimesWatched = x.TimesWatched,
                    MoviePath = x.MoviePath,
                    InformationColumn = x.InformationColumn,
                    VideoCoverImage = x.VideoCoverImage,
                    UploadDate = x.UploadDate,
                    ViedoNature = x.NatureS.ViedoNature
                });

            return Json(show);
        }

        public async Task<IActionResult> GetMessage(int videoid)
        {
            var Message = _context.Messages
                .Include(M => M.MemberS)
                .Where(M => M.VideoSid == videoid)
                .Select(M => new VideoMessageDTO
                {
                    NumberName = M.MemberS.Name,
                    VideoSid = M.VideoSid,
                    MessageSid = M.MessageSid,
                    MessageContent = M.MessageContent,
                    MessageLikes = M.MessageLikes,
                    MessageDate = M.MessageDate,
                    MessageNumber = M.MessageNumber,
                });

            return Json(Message);
        }

        public async Task<IActionResult> GetOneVideo(int videoid)
        {
            var video = _context.VideoSummaries
                .Where(M => M.VideoSid == videoid)
                .Select(M => new OneVideoDTO
                {
                    VideoSid = M.VideoSid,
                    VideoTitle = M.VideoTitle,
                    TimesWatched = M.TimesWatched,
                    MoviePath = M.MoviePath,
                    InformationColumn = M.InformationColumn,
                    UploadDate = M.UploadDate,
                });

            return Json(video);

        }

        public async Task<IActionResult> TotalVideo(int videoid)
        {
            var TotalVideo = _context.VideoSummaries
                .Include(M => M.CreatorS)
                .Include(M=> M.CreatorS.MemberS)
                .Where(M => M.VideoSid != videoid)
                .Select(M => new TotalVideoDTO
                {
                    VideoSid = M.VideoSid,
                    VideoTitle = M.VideoTitle,
                    TimesWatched = M.TimesWatched,
                    MoviePath = M.MoviePath,
                    UploadDate = M.UploadDate,
                    NumberName = M.CreatorS.MemberS.Name,
                });
            return Json(TotalVideo);
        }

        [HttpPost]
        public async Task<IActionResult> MessageLike([FromForm] int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return NotFound("Message not found");
            }

            message.MessageLikes += 1;

            await _context.SaveChangesAsync();

            // 返回更新後的留言
            return Json(new MessageLikeDTO
            {
                MessageLikes = message.MessageLikes,
                MessageSid = messageId,
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage([FromBody] AddMessageDTO message)
        {

            var newmessage = new EggProductionProject_MVC.Models.Message
            {
                VideoSid = message.VideoSid,
                MemberSid = message.MemberSid,
                MessageContent = message.MessageContent,
                MessageNumber = message.MessageNumber,
                MessageDate = message.MessageDate,
                MessageLikes = message.MessageLikes,
                MessageDelete = false,
            };
            
            _context.Messages.Add(newmessage);
            _context.SaveChanges();

            return Json(newmessage);
        }

        [HttpPost]
        public async Task<IActionResult> EditVideo([FromBody] EditVideoDTO Edit)
        {
            VideoSummary 資料庫資料 = await _context.VideoSummaries.FindAsync(Edit.VideoSid);

            var EditVideo = new EggProductionProject_MVC.Models.VideoSummary
            {

                VideoSid = Edit.VideoSid,
                VideoTitle = Edit.VideoTitle,
                CreatorSid = Edit.CreatorSid,
                TimesWatched = Edit.TimesWatched,
                MoviePath = 資料庫資料.MoviePath,
                InformationColumn = Edit.InformationColumn,
                UploadDate = Edit.UploadDate,
                NatureSid = Edit.NatureSid,
                PublicStatusNo = 1,

        };


            

        }
    }
}