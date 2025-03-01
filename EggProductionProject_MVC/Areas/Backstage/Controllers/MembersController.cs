﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Models.MemberVM;
using Microsoft.AspNetCore.Hosting;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class MembersController : Controller
    {
        private readonly EggPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public MembersController(EggPlatformContext context,IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        //拿圖片
        public async Task<IActionResult> GetPic(int id)
        {
            Member? member = await _context.Members.FindAsync(id);
            string content = member?.ProfilePic;
            return File(content, "image/jpeg");
        }

        //private void ReadUploadImage(MemberVM member)
        //{
        //    using (BinaryReader br = new BinaryReader(
        //        Request.Form.Files["Picture"].OpenReadStream()))
        //    {
        //        member.ProfilePic = br.ReadBytes((int)Request.Form.Files["Picture"].Length);
        //    }
        //}



        //點擊修改後使用者資料要出現在修改表單
        public IActionResult GetMemberDetails(int id)
        {
            var member = _context.Members.FirstOrDefault(m => m.MemberSid == id);
            if (member == null)
            {
                return NotFound();
            }

            return Json(member);
        }




        //點下確認後修改該名會員的資料
        [HttpPost]
        public IActionResult UpdateMember(MemberVM model, IFormFile profilePic)
        {
            if (ModelState.IsValid)
            {
                var member = _context.Members.Find(model.MemberSid);
                if (member == null)
                {
                    return NotFound();
                }

                // 更新其他字段
                member.Name = model.Name;
                member.Email = model.Email;
                member.Phone = model.Phone;
                member.BirthDate = model.BirthDate;
                member.IsChickFarm = model.IsChickFarm;
                member.IsBlocked = model.IsBlocked;

                // 處理上傳的照片
                if (profilePic != null && profilePic.Length > 0)
                {
                    // 確定上傳檔案的目錄
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "memProfilePic");

                    // 檢查資料夾是否存在，不存在則建立
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // 產生唯一的檔案名稱
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profilePic.FileName);

                    // 確定檔案的完整路徑
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // 儲存檔案至伺服器
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                         profilePic.CopyToAsync(fileStream);
                    }

                    // 儲存檔案路徑到資料庫 (儲存相對路徑)
                    member.ProfilePic = "/memProfilePic/" + uniqueFileName;
                }

                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "具体的错误信息" });

        }


        public async Task<IActionResult> CreateSave([FromForm] UserDTO model)
        {
            ModelState.Remove("AspUserId");
            ModelState.Remove("Chickcode");
            if (ModelState.IsValid)
            {
                var member = await _context.Members.FindAsync(model.MemberSid);
                if (member == null)
                {
                    return NotFound(new { success = false, message = "會員未找到" });
                }

                // 更新相关字段
                member.Name = model.Name;
                member.Email = model.Email;
                member.Phone = model.Phone;
                member.BirthDate = model.BirthDate;
                member.IsChickFarm = model.IsChickFarm;
                member.IsBlocked = model.IsBlocked;

                // 如果有上傳檔案，則處理檔案更新
                if (model.ProfilePic != null)
                {
                    // 檔案名稱處理，避免重名
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePic.FileName;
                    // 檔案上傳路徑
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "memProfilePic", model.ProfilePic.FileName);

                    // 將檔案上傳到指定路徑
                    using (var filestream = new FileStream(path, FileMode.Create))
                    {
                        model.ProfilePic.CopyTo(filestream);
                    }

                    // 儲存相對路徑到資料庫
                    member.ProfilePic = "/memProfilePic/" + model.ProfilePic.FileName;

                }

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { success = true, message = "資料成功寫入" });
                }
                catch (Exception ex)
                {
                    // 处理错误并返回状态码和错误信息
                    return StatusCode(500, new { success = false, message = "沒有成功寫入資料", error = ex.Message });
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { success = false, message = "資料驗證失敗", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }





















        //------------------------------------以下都不要動------------------------------------








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
