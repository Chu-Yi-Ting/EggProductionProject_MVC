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
    public class MembersController : Controller
    {
        private readonly EggPlatformContext _context;

        public MembersController(EggPlatformContext context)
        {
            _context = context;
        }

        //拿圖片
        public async Task<IActionResult> GetPic(int id)
        {
            Member? member = await _context.Members.FindAsync(id);
            byte[]? content = member?.ProfilePic;
            return File(content, "image/jpeg");
        }


       


        public IActionResult GetMemberDetails(int id)
        {
            var member = _context.Members.FirstOrDefault(m => m.MemberSid == id);
            if (member == null)
            {
                return NotFound();
            }

            return Json(member);
        }


        //會員編輯
        [HttpPost]
        public IActionResult UpdateMember(MemberVM model)
        {
            if (ModelState.IsValid)
            {
                var member = _context.Members.Find(model.MemberSid);
                if (member == null)
                {
                    return NotFound();
                }

                member.Name = model.Name;   
                member.Email = model.Email;
                member.Phone = model.Phone;
                member.BirthDate = model.BirthDate;
                member.IsChickFarm = model.IsChickFarm;
                member.IsBlocked = model.IsBlocked;

                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        //查詢與篩選功能
        [HttpGet]
        public IActionResult FilterMembers(string keyword , string isBlocked, string isChickFarm)
        {
            var members = _context.Members.AsQueryable();

			if (!string.IsNullOrEmpty(isBlocked))
			{
                int blocked = int.Parse(isBlocked);
				members = members.Where(m => m.IsBlocked == blocked);
			}

			if (!string.IsNullOrEmpty(isChickFarm))
			{
				int chickFarm = int.Parse(isChickFarm);
				members = members.Where(m => m.IsChickFarm == chickFarm);
			}


			if (!string.IsNullOrEmpty(keyword))
            {
                members = members.Where(m => m.Name.Contains(keyword) || m.Email.Contains(keyword) || m.Phone.Contains(keyword));
            }

            var memberVMs = members.Select(m => new MemberVM
            {
                MemberSid = m.MemberSid,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                BirthDate = m.BirthDate,
                IsChickFarm = m.IsChickFarm,
                ProfilePic = m.ProfilePic,
                IsBlocked = m.IsBlocked
            }).ToList();

            return PartialView("_MemberListPartial", memberVMs);
        }



        // GET: Backstage/Members
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.Members.Include(m => m.ShoppingRankNoNavigation).Select(
                x=>new MemberVM
                {
                    MemberSid = x.MemberSid,
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    BirthDate = x.BirthDate,
                    IsChickFarm = x.IsChickFarm,
                    ShoppingRankNo = x.ShoppingRankNo,
                    ProfilePic = x.ProfilePic,
                    IsBlocked = x.IsBlocked,
                }
                );

            ViewData["IsBlocked"] = new SelectList(_context.Members, "IsBlocked", "IsBlocked");

            return View(await eggPlatformContext.ToListAsync());

            //搜尋關鍵字
            //ViewBag.CurrentFilter = searchString;
            //ViewBag.CurrentMemberId = _memberVM.MemberSid;
            //var query = from Member in _context.Members
            //            where _memberVM.MemberSid == Member.MemberSid
            //            select new MemberVM
            //            {
            //                MemberSid = Member.MemberSid,
            //                Name = Member.Name,
            //                Email = Member.Email,
            //                Phone = Member.Phone,
            //                BirthDate = Member.BirthDate,
            //                IsChickFarm = Member.IsChickFarm,
            //                ShoppingRankNo = Member.ShoppingRankNo,
            //                ProfilePic = Member.ProfilePic,
            //                IsBlocked = Member.IsBlocked,
            //            };

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    query = query.Where(f => f.Name.Contains(searchString) || f.Email.Contains(searchString));
            //}

            //var result = await query.ToListAsync();

            //return View(result);


        }

        // GET: Backstage/Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.ShoppingRankNoNavigation)
                .FirstOrDefaultAsync(m => m.MemberSid == id);
            if (member == null)
            {
                return NotFound();
            }

            var memberVM = new MemberVM
            {
                MemberSid = member.MemberSid,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                BirthDate = member.BirthDate,
                IsChickFarm = member.IsChickFarm,
                ShoppingRankNo = member.ShoppingRankNo,
                ProfilePic = member.ProfilePic,
                IsBlocked = member.IsBlocked,
            };

            return View(memberVM);


            //return View(member);
        }

        // GET: Backstage/Members/Create
        public IActionResult Create()
        {
            ViewData["ShoppingRankNo"] = new SelectList(_context.ShoppingRanks, "ShoppingRankNo", "ShoppingRankNo");
            return View();
        }

        // POST: Backstage/Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberSid,Name,Email,Phone,BirthDate,IsChickFarm,ShoppingRankNo,PassWord,UserName,ProfilePic,IsBlocked")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShoppingRankNo"] = new SelectList(_context.ShoppingRanks, "ShoppingRankNo", "ShoppingRankNo", member.ShoppingRankNo);
            return View(member);
        }

        // GET: Backstage/Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            ViewData["ShoppingRankNo"] = new SelectList(_context.ShoppingRanks, "ShoppingRankNo", "ShoppingRankNo", member.ShoppingRankNo);
            return View(member);
        }

        // POST: Backstage/Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberSid,Name,Email,Phone,BirthDate,IsChickFarm,ShoppingRankNo,PassWord,UserName,ProfilePic,IsBlocked")] Member member)
        {
            if (id != member.MemberSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberSid))
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
            ViewData["ShoppingRankNo"] = new SelectList(_context.ShoppingRanks, "ShoppingRankNo", "ShoppingRankNo", member.ShoppingRankNo);
            return View(member);
        }


        [HttpPost]
        public IActionResult UpdateIsBlocked(int MemberSid, byte IsBlocked)
        {
            // 查找指定的Member
            var member = _context.Members.FirstOrDefault(m => m.MemberSid == MemberSid);

            if (member != null)
            {
                // 更新IsBlocked屬性
                member.IsBlocked = IsBlocked;
                _context.SaveChanges();
            }

            // 重定向到Index視圖
            return RedirectToAction("Index");
        }



        // GET: Backstage/Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.ShoppingRankNoNavigation)
                .FirstOrDefaultAsync(m => m.MemberSid == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Backstage/Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.MemberSid == id);
        }
    }
}
