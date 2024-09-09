using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using NuGet.Protocol;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class MembersController : Controller
    {
        private readonly EggPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MembersController(EggPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        //[Route("Members/MemberPage/{userId}")]
        // GET: Frontstage/Members
        public async Task<IActionResult> MemberPage()
        {
            
            string aspuserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string aspuseEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            // 從 Session 取得 memberSid
            var userAspId  = HttpContext.Session.GetString("userId");

            //if (userAspId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            
            var user = _context.Members.Where(x=>x.AspUserId == aspuserId).FirstOrDefault();
           
            // 將 Name 傳遞到視圖
            
           

            //如果==null代表剛註冊好，還不是會員
            if (user == null)
            {
                //新創立一個member並且擁有asp的ID與email
                user =  new Member
                {
                    AspUserId = aspuserId,
                    Email = aspuseEmail,
                    IsBlocked = 0,
                    IsChickFarm = 0,
                };
                ViewBag.UserName = user.Name;

                _context.Members.Add(user);
                await _context.SaveChangesAsync();
                return View(user);
            }
            else
            {
                var member = await _context.Members
               .Include(m => m.AspUser)
               .Include(m => m.ShoppingRankNoNavigation)
               .FirstOrDefaultAsync(m => m.AspUserId == aspuserId);

                ViewBag.UserName = user.Name;
                return View(member);
            }
           
                
        }


        [HttpPost]
        public async Task<IActionResult> MemberbecomeChickFarm([FromBody] UserDTO _user)
        {
            string aspuserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


            // 查詢要更新的 Member 資料
            var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspuserId);
            if (member == null)
            {
                return NotFound("Member not found");
            }


            if (_user.Chickcode != null) 
            {
                // 更新雞農驗證碼，會員變成雞農
                member.Chickcode = _user.Chickcode;
                member.IsChickFarm = 1;
            }
           
            _context.Members.Update(member);
            // 儲存更新後的資料
             _context.SaveChanges();

            // 回傳更新後的檔案儲存的路徑或其他資訊
            return Json(new { success = true, message = "ChickCode updated successfully" });
        }



        [HttpPost]
        public async Task<IActionResult> MemberPageUpdate([FromForm] UserDTO _user)
        {
            
            // 查詢要更新的 Member 資料
            var member = _context.Members.FirstOrDefault(m => m.AspUserId == _user.AspUserId);
            if (member == null)
            {
                return NotFound("Member not found");
            }

            // 更新屬性
            member.Name = _user.Name;
            member.Email = _user.Email;
            member.BirthDate = _user.BirthDate;
            member.Phone = _user.Phone;



			//// 如果有上傳檔案，則處理檔案更新
			//if (_user.ProfilePic != null)
			//{
			//    // 檔案上傳路徑
			//    string path = Path.Combine(_webHostEnvironment.WebRootPath, "memProfilePic", _user.ProfilePic.FileName);

			//    // 將檔案上傳到指定路徑
			//    using (var filestream = new FileStream(path, FileMode.Create))
			//    {
			//        _user.ProfilePic.CopyTo(filestream);
			//    }



			//}


			// 如果有上傳檔案，則處理檔案更新
			if (_user.ProfilePic != null)
			{
				// 檔案名稱處理，避免重名
				string uniqueFileName = Guid.NewGuid().ToString() + "_" + _user.ProfilePic.FileName;
				// 檔案上傳路徑
				string path = Path.Combine(_webHostEnvironment.WebRootPath, "memProfilePic", _user.ProfilePic.FileName);

				// 將檔案上傳到指定路徑
				using (var filestream = new FileStream(path, FileMode.Create))
				{
					_user.ProfilePic.CopyTo(filestream);
				}

				// 儲存相對路徑到資料庫
				member.ProfilePic = "/memProfilePic/" + _user.ProfilePic.FileName;

			}

			_context.Members.Update(member);
            // 儲存更新後的資料
            _context.SaveChanges();

            // 回傳更新後的檔案儲存的路徑或其他資訊
            return Json(new { success = true, message = "Member updated successfully" });
        }
        

        // GET: Frontstage/Members/Edit/5
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
            ViewData["AspUserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", member.AspUserId);
            ViewData["ShoppingRankNo"] = new SelectList(_context.ShoppingRanks, "ShoppingRankNo", "ShoppingRankNo", member.ShoppingRankNo);
            return View(member);
        }




        // POST: Frontstage/Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberSid,Name,Email,Phone,BirthDate,IsChickFarm,ShoppingRankNo,PassWord,UserName,ProfilePic,IsBlocked,Chickcode,AspUserId")] Member member)
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
            ViewData["AspUserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", member.AspUserId);
            ViewData["ShoppingRankNo"] = new SelectList(_context.ShoppingRanks, "ShoppingRankNo", "ShoppingRankNo", member.ShoppingRankNo);
            return View(member);
        }


        // GET: Frontstage/Members/Create
        public IActionResult Create()
        {
            ViewData["AspUserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["ShoppingRankNo"] = new SelectList(_context.ShoppingRanks, "ShoppingRankNo", "ShoppingRankNo");
            return View();
        }

        // POST: Frontstage/Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberSid,Name,Email,Phone,BirthDate,IsChickFarm,ShoppingRankNo,PassWord,UserName,ProfilePic,IsBlocked,Chickcode,AspUserId")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AspUserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", member.AspUserId);
            ViewData["ShoppingRankNo"] = new SelectList(_context.ShoppingRanks, "ShoppingRankNo", "ShoppingRankNo", member.ShoppingRankNo);
            return View(member);
        }


        // GET: Frontstage/Members
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.Members.Include(m => m.AspUser).Include(m => m.ShoppingRankNoNavigation);
            return View(await eggPlatformContext.ToListAsync());
        }

        // GET: Frontstage/Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.AspUser)
                .Include(m => m.ShoppingRankNoNavigation)
                .FirstOrDefaultAsync(m => m.MemberSid == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

       

     

        // GET: Frontstage/Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.AspUser)
                .Include(m => m.ShoppingRankNoNavigation)
                .FirstOrDefaultAsync(m => m.MemberSid == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Frontstage/Members/Delete/5
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
