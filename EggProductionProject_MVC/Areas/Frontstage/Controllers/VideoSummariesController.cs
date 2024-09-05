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

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Route("/api/[controller]/[Action]")]
    [Area("Frontstage")]
    public class VideoSummariesController : Controller
    {
        private readonly EggPlatformContext _context;

        public VideoSummariesController(EggPlatformContext context)
        {
            _context = context;
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
                .Where(v => v.PublicStatusNoNavigation.PublicStatusNo == 1)
                .Select(x => new VideoSummaryDTO
                {
                    VideoSid = x.VideoSid,
                    CreatorSid = x.CreatorSid,
                    VideoDuration = x.VideoDuration,
                    VideoTitle = x.VideoTitle,
                    MemberName = x.CreatorS.MemberName,
                    TimesWatched = x.TimesWatched,
                    MoviePath = x.MoviePath,
                    InformationColumn = x.InformationColumn,
                    VideoCoverImage = x.VideoCoverImage,
                    UploadDate = x.UploadDate,
                    ViedoNature= x.NatureS.ViedoNature
                });

            return Json(show);
        }

        public async Task<IActionResult> GetMessage(int videoid)
        {
            var Message = _context.Messages
                .Include(M =>M.MemberS)
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
                .Where(M => M.VideoSid != videoid)
                .Select(M => new TotalVideoDTO
                {

                    VideoTitle = M.VideoTitle,
                    TimesWatched = M.TimesWatched,
                    MoviePath = M.MoviePath,
                    UploadDate = M.UploadDate,
                    NumberName = M.CreatorS.MemberName,
                });
            return Json(TotalVideo);
        }

        // GET: Frontstage/VideoSummaries/Create
        public IActionResult Create()
        {
            ViewData["CreatorSid"] = new SelectList(_context.Creators, "CreatorSid", "CreatorSid");
            ViewData["NatureSid"] = new SelectList(_context.Natures, "NatureSid", "NatureSid");
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo");
            ViewData["ScreenTextSid"] = new SelectList(_context.ScreenSummaries, "ScreenTextSid", "ScreenTextSid");
            return View();
        }

        // POST: Frontstage/VideoSummaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VideoSid,CreatorSid,VideoDuration,VideoTitle,NatureSid,InformationColumn,UploadDate,TimesWatched,ScreenTextSid,MoviePath,AdSource,Advertised,PublicStatusNo,VideoCoverImage")] VideoSummary videoSummary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(videoSummary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorSid"] = new SelectList(_context.Creators, "CreatorSid", "CreatorSid", videoSummary.CreatorSid);
            ViewData["NatureSid"] = new SelectList(_context.Natures, "NatureSid", "NatureSid", videoSummary.NatureSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", videoSummary.PublicStatusNo);
            ViewData["ScreenTextSid"] = new SelectList(_context.ScreenSummaries, "ScreenTextSid", "ScreenTextSid", videoSummary.ScreenTextSid);
            return View(videoSummary);
        }

        // GET: Frontstage/VideoSummaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoSummary = await _context.VideoSummaries.FindAsync(id);
            if (videoSummary == null)
            {
                return NotFound();
            }
            ViewData["CreatorSid"] = new SelectList(_context.Creators, "CreatorSid", "CreatorSid", videoSummary.CreatorSid);
            ViewData["NatureSid"] = new SelectList(_context.Natures, "NatureSid", "NatureSid", videoSummary.NatureSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", videoSummary.PublicStatusNo);
            ViewData["ScreenTextSid"] = new SelectList(_context.ScreenSummaries, "ScreenTextSid", "ScreenTextSid", videoSummary.ScreenTextSid);
            return View(videoSummary);
        }

        // POST: Frontstage/VideoSummaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VideoSid,CreatorSid,VideoDuration,VideoTitle,NatureSid,InformationColumn,UploadDate,TimesWatched,ScreenTextSid,MoviePath,AdSource,Advertised,PublicStatusNo,VideoCoverImage")] VideoSummary videoSummary)
        {
            if (id != videoSummary.VideoSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(videoSummary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoSummaryExists(videoSummary.VideoSid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorSid"] = new SelectList(_context.Creators, "CreatorSid", "CreatorSid", videoSummary.CreatorSid);
            ViewData["NatureSid"] = new SelectList(_context.Natures, "NatureSid", "NatureSid", videoSummary.NatureSid);
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", videoSummary.PublicStatusNo);
            ViewData["ScreenTextSid"] = new SelectList(_context.ScreenSummaries, "ScreenTextSid", "ScreenTextSid", videoSummary.ScreenTextSid);
            return View(videoSummary);
        }

        // GET: Frontstage/VideoSummaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoSummary = await _context.VideoSummaries
                .Include(v => v.CreatorS)
                .Include(v => v.NatureS)
                .Include(v => v.PublicStatusNoNavigation)
                .Include(v => v.ScreenTextS)
                .FirstOrDefaultAsync(m => m.VideoSid == id);
            if (videoSummary == null)
            {
                return NotFound();
            }

            return View(videoSummary);
        }

        // POST: Frontstage/VideoSummaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var videoSummary = await _context.VideoSummaries.FindAsync(id);
            if (videoSummary != null)
            {
                _context.VideoSummaries.Remove(videoSummary);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VideoSummaryExists(int id)
        {
            return _context.VideoSummaries.Any(e => e.VideoSid == id);
        }
    }
}
