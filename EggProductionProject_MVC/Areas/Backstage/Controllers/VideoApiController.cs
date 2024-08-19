using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    public class VideoApiController : Controller
    {
        private readonly EggPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VideoApiController(EggPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        
        public async Task<FileResult> GetVideopath(int id)
        {
                VideoSummary c = await _context.VideoSummaries.FindAsync(id);
                string x = c?.MoviePath;
                return File(x, "Video/mp4");
        }

        //public async Task<FileResult> VideoUpdata(int id)
        //{
        //    VideoSummary video = _context.VideoSummaries.Find(id);
        //    if (video != null)
        //    {
        //        string x = video.MoviePath;
        //    }

        //}
    }
}
