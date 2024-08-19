using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.ViewModels;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class VideoSummariesController : Controller
    {
        private readonly EggPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VideoSummariesController(EggPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Backstage/VideoSummaries
        public async Task<IActionResult> Index()
        {
            var show = _context.VideoSummaries
                  .Include(v => v.CreatorS)
                  .Include(v => v.NatureS)
                  .Include(v => v.PublicStatusNoNavigation)
                  .Include(v => v.ScreenTextS)
                  .Include(v => v.Advertisments)
                  .Include(v => v.Messages)

                  .Select(x => new ShowViedoSummary

                  {  CreatorSid = x.CreatorSid,
                      ScreenTextCategory = x.ScreenTextS.ScreenTextCategory,
                      VideoSid = x.VideoSid,
                      Advertised = x.Advertised,
                      VideoDuration = x.VideoDuration,
                      VideoTitle = x.VideoTitle,
                      MemberName = x.CreatorS.MemberName,
                      TimesWatched = x.TimesWatched,
                      MoviePath = x.MoviePath,
                      InformationColumn = x.InformationColumn,
                      VideoCoverImage = x.VideoCoverImage,
                      UploadDate = x.UploadDate,
                      ViedoNature = x.NatureS.ViedoNature,
                      StatusDescription = x.PublicStatusNoNavigation.StatusDescription
                  });

            return View(show);
        }

        // GET: Backstage/VideoSummaries/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Backstage/VideoSummaries/Create
        public IActionResult Create()
        {
            ViewData["CreatorSid"] = new SelectList(_context.Creators, "CreatorSid", "CreatorSid");
            ViewData["NatureSid"] = new SelectList(_context.Natures, "NatureSid", "NatureSid");
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo");
    
            ViewData["ScreenTextSid"] = new SelectList(_context.ScreenSummaries, "ScreenTextSid", "ScreenTextSid");
            return View();
        }

        // POST: Backstage/VideoSummaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VideoSid,CreatorSid,VideoDuration,VideoTitle,NatureSid,InformationColumn,UploadDate,TimesWatched,MemberName,ScreenTextSid,MoviePath,Advertise,PublicStatusNo,VideoCoverImage")] VideoSummary videoSummary)
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

        // GET: Backstage/VideoSummaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            
            
            var videoSummary = await _context.VideoSummaries.FindAsync(id);
            if (id == null)
            {
                return NotFound();
            }

            string VideoLong = Path.Combine(_webHostEnvironment.WebRootPath,
                "Video", _context.Creators.ToQueryString() + id);
            //var editViewmodels = _context.VideoSummaries.Where(x=>x.VideoSid == id )
            //    .Select(x=> new ShowViedoSummary
            //    {
            //        ScreenTextCategory = x.ScreenTextS.ScreenTextCategory,
            //        VideoSid = x.VideoSid,
            //        Advertise = x.Advertised,
            //        VideoDuration = x.VideoDuration,
            //        VideoTitle = x.VideoTitle,
            //        MemberName = x.CreatorS.MemberName,
            //        TimesWatched = x.TimesWatched,
            //        MoviePath = x.MoviePath,
            //        InformationColumn = x.InformationColumn,
            //        VideoCoverImage = x.VideoCoverImage,
            //        UploadDate = x.UploadDate,
            //        ViedoNature = x.NatureS.ViedoNature,
            //        StatusDescription = x.PublicStatusNoNavigation.StatusDescription
            //    });

            //var x = await _context.VideoSummaries.FindAsync(id)  ;
            if (videoSummary == null)
            {
                return NotFound();
            }
            else
            {
                //ShowViedoSummary x = editViewmodels.ToList()[0];
                ViewData["CreatorSid"] = new SelectList(_context.Creators, "CreatorSid", "MemberName");
                ViewBag.NatureSid = new SelectList(_context.Natures, "NatureSid", "ViedoNature");
                ViewData["ScreenTextSid"] = new SelectList(_context.ScreenSummaries, "ScreenTextSid", "ScreenTextCategory", videoSummary.ScreenTextS);
                ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription");
                ViewBag.Advertised = new SelectList(_context.VideoSummaries,"VideoSid","Advertised");


                return View(videoSummary);
            }
            

        }

        // POST: Backstage/VideoSummaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VideoSid,CreatorSid,VideoDuration,VideoTitle,NatureSid,InformationColumn,UploadDate,TimesWatched,ScreenTextSid,MoviePath,Advertise,PublicStatusNo,VideoCoverImage")] VideoSummary videoSummary)
        {
            if (id != videoSummary.VideoSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    VideoSummary 上傳時間 = _context.VideoSummaries.Find(videoSummary.UploadDate);



                    _context.Entry(上傳時間).State = EntityState.Detached;
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

        // GET: Backstage/VideoSummaries/Delete/5
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

        // POST: Backstage/VideoSummaries/Delete/5
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
