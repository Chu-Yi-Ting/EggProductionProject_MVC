using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Models.MemberVM;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class FriendsController : Controller
    {
        private readonly EggPlatformContext _context;

        public FriendsController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Backstage/Friends
        public async Task<IActionResult> Index(int id)
        {
            //var eggPlatformContext = _context.Friends.Include(f => f.MemberS)
            //    .Where(f=>f.MemberSid==id);
            //return View(await eggPlatformContext.ToListAsync());

            var query = from friend in _context.Friends
                        join member1 in _context.Members on friend.MemberSid equals member1.MemberSid
                        join member2 in _context.Members on friend.MemberSid2 equals member2.MemberSid
                        where friend.MemberSid == id
                        select new FriendVM
                        {
                            FriendSid = friend.FriendSid,
                            MemberName1 = member1.Name,
                            MemberName2 = member2.Name,
                            DateAdded = friend.DateAdded
                        };

            var result = await query.ToListAsync();

            return View(result);
        }

        // GET: Backstage/Friends/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friend = await _context.Friends
                .Include(f => f.MemberS)
                .FirstOrDefaultAsync(m => m.FriendSid == id);
            if (friend == null)
            {
                return NotFound();
            }

            return View(friend);
        }

        // GET: Backstage/Friends/Create
        public IActionResult Create()
        {
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid");
            return View();
        }

        // POST: Backstage/Friends/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FriendSid,MemberSid,MemberSid2,DateAdded")] Friend friend)
        {
            if (ModelState.IsValid)
            {
                _context.Add(friend);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", friend.MemberSid);
            return View(friend);
        }

        // GET: Backstage/Friends/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friend = await _context.Friends.FindAsync(id);
            if (friend == null)
            {
                return NotFound();
            }
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", friend.MemberSid);
            return View(friend);
        }

        // POST: Backstage/Friends/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FriendSid,MemberSid,MemberSid2,DateAdded")] Friend friend)
        {
            if (id != friend.FriendSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(friend);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FriendExists(friend.FriendSid))
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
            ViewData["MemberSid"] = new SelectList(_context.Members, "MemberSid", "MemberSid", friend.MemberSid);
            return View(friend);
        }

        // GET: Backstage/Friends/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friend = await _context.Friends
                .Include(f => f.MemberS)
                .FirstOrDefaultAsync(m => m.FriendSid == id);
            if (friend == null)
            {
                return NotFound();
            }

            return View(friend);
        }

        // POST: Backstage/Friends/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var friend = await _context.Friends.FindAsync(id);
            if (friend != null)
            {
                _context.Friends.Remove(friend);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FriendExists(int id)
        {
            return _context.Friends.Any(e => e.FriendSid == id);
        }
    }
}
