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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VideoSummariesController(EggPlatformContext _context, IWebHostEnvironment webHostEnvironment)
        {
            this._context = _context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult GetLoginUser()
        {
            var GetLoin = new LoginUserDTO
            {
                userMemberSid = HttpContext.Session.GetInt32("userMemberSid"),
                UserName = HttpContext.Session.GetString("userName")

        };
            
            return Json(GetLoin);
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
                    MemberImage = M.MemberS.ProfilePic,
                });

            return Json(Message);
        }

        public async Task<IActionResult> GetOneVideo(int videoid)
        {
            var video = _context.VideoSummaries
                .Include(M => M.CreatorS.MemberS)
                .Include(M=>M.NatureS)
                .Where(M => M.VideoSid == videoid)
                .Select(M => new OneVideoDTO
                {
                    VideoCoverImage = M.VideoCoverImage,
                    NatureSid = M.NatureSid,
                    MemberName = M.CreatorS.MemberS.Name,
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
                .Where(M => M.VideoSid != videoid && M.PublicStatusNo == 1)
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
        public async Task<IActionResult> EditVideo([FromForm]EditVideoDTO Edit,IFormFile? image)
        {
            VideoSummary 資料庫資料 = await _context.VideoSummaries.FindAsync(Edit.VideoSid);

            if (image != null && image.Length > 0)
            {

                string FileImg = Path.GetExtension(image.FileName);
                string FileNameImg = Path.GetFileName("創作者編號_" +
                    Edit.CreatorSid + "-" + Edit.VideoSid + FileImg);

                string ImagePath = Path.Combine(_webHostEnvironment
                    .WebRootPath, "VideoImage", FileNameImg);

                using (var filesteam = new FileStream(ImagePath, FileMode.Create))
                {
                    image.CopyTo(filesteam);
                }
                Edit.VideoCoverImage = $"/VideoImage/{FileNameImg}";
            }
            else
            {
                Edit.VideoCoverImage = 資料庫資料.VideoCoverImage;
            }

            var EditVideo = new EggProductionProject_MVC.Models.VideoSummary
            {
                VideoDuration = Edit.VideoDuration,
                VideoCoverImage = Edit.VideoCoverImage,
                VideoSid = Edit.VideoSid,
                VideoTitle = Edit.VideoTitle,
                CreatorSid = Edit.CreatorSid,
                TimesWatched = 資料庫資料.TimesWatched,
                MoviePath = 資料庫資料.MoviePath,
                InformationColumn = Edit.InformationColumn,
                UploadDate = 資料庫資料.UploadDate,
                NatureSid = Edit.NatureSid,
                PublicStatusNo = 1,

            };
            _context.Entry(資料庫資料).State = EntityState.Detached;

            _context.Update(EditVideo);
            _context.SaveChanges();

            return Json(new { Message = "修改成功" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVideo([FromForm] DeleteVideoDTO delete, IFormFile? image)
        {
            VideoSummary 資料庫資料 = await _context.VideoSummaries.FindAsync(delete.VideoSid);

            var DeleteVideo = new EggProductionProject_MVC.Models.VideoSummary
            {

                VideoSid = delete.VideoSid,
                VideoCoverImage = 資料庫資料.VideoCoverImage,
                VideoTitle = 資料庫資料.VideoTitle,
                CreatorSid = 資料庫資料.CreatorSid,
                TimesWatched = 資料庫資料.TimesWatched,
                MoviePath = 資料庫資料.MoviePath,
                InformationColumn = 資料庫資料.InformationColumn,
                UploadDate = 資料庫資料.UploadDate,
                NatureSid = 資料庫資料.NatureSid,
                VideoDuration = 資料庫資料.VideoDuration,
                PublicStatusNo = 3,
            };
            _context.Entry(資料庫資料).State = EntityState.Detached;

            _context.Update(DeleteVideo);
            _context.SaveChanges();

            return Json(new { Message = "刪除成功" });
        }

        [HttpPost]
        public IActionResult CreatVideo([FromBody] CreatVideoDTO Creat)
        {
            
            // 儲存影片的基本資訊
            var CreatVideo = new EggProductionProject_MVC.Models.VideoSummary
            {
                VideoDuration = Creat.VideoDuration,
                VideoTitle = Creat.VideoTitle,
                CreatorSid = Creat.CreatorSid,
                TimesWatched = 0,
                MoviePath = null, // 初始設為 null，等檔案上傳後再更新
                InformationColumn = Creat.InformationColumn,
                UploadDate = Creat.UploadDate,
                NatureSid = Creat.NatureSid,
                PublicStatusNo = 1,
            };

            _context.VideoSummaries.Add(CreatVideo);
            _context.SaveChanges();  // 這會生成影片Sid

            // 返回影片Sid，讓前端繼續處理檔案上傳
            return Json(new { videoSid = CreatVideo.VideoSid });
        }
        
        [HttpPost]
        public async Task<IActionResult> UploadVideoFiles([FromForm] VideoAddDTO AddVideo, IFormFile? image, IFormFile video)
        {
            VideoSummary 資料庫資料 = await _context.VideoSummaries.FindAsync(AddVideo.VideoSid);

            if (image != null && image.Length > 0)
            {

                string FileImg = Path.GetExtension(image.FileName);
                string FileNameImg = Path.GetFileName("創作者編號_" +
                    AddVideo.CreatorSid + "-" + AddVideo.VideoSid+ FileImg);

                string ImagePath = Path.Combine(_webHostEnvironment
                    .WebRootPath, "VideoImage", FileNameImg);

                using (var filesteam = new FileStream(ImagePath, FileMode.Create))
                {
                    image.CopyTo(filesteam);
                }
                AddVideo.VideoCoverImage = $"/VideoImage/{FileNameImg}";
            }

            if (video != null && video.Length > 0)
            {
                string File副檔名 = Path.GetExtension(video.FileName);
                string FileName = Path.GetFileName("創作者編號_" + AddVideo
                    .CreatorSid + "-" + AddVideo.VideoSid + File副檔名);
                string VideoPath = Path.Combine(_webHostEnvironment.
                WebRootPath, "Video", FileName);
                using (var filesteam = new FileStream(VideoPath, FileMode.Create))
                {
                    video.CopyTo(filesteam);
                }
                AddVideo.MoviePath = $"/Video/{FileName}";
            }


            var CreatVideo = new EggProductionProject_MVC.Models.VideoSummary
            {
                
                VideoCoverImage = AddVideo.VideoCoverImage,
                VideoSid = AddVideo.VideoSid,
                MoviePath = AddVideo.MoviePath,
                VideoTitle = 資料庫資料.VideoTitle,
                CreatorSid = 資料庫資料.CreatorSid,
                TimesWatched = 資料庫資料.TimesWatched,
                InformationColumn = 資料庫資料.InformationColumn,
                UploadDate = 資料庫資料.UploadDate,
                VideoDuration = 資料庫資料.VideoDuration,
                NatureSid = 資料庫資料.NatureSid,
                PublicStatusNo = 資料庫資料.PublicStatusNo,
            };

            _context.Entry(資料庫資料).State = EntityState.Detached;
            _context.Update(CreatVideo);
            _context.SaveChanges();

            return Json(new { success = true });
        }


    }
}







