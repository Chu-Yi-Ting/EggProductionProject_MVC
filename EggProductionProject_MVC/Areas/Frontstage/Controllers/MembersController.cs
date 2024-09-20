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
using Microsoft.AspNetCore.Identity;
using EggProductionProject_MVC.Models.MemberVM;


namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class MembersController : Controller
    {
        private readonly EggPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public MembersController(EggPlatformContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }



        [HttpGet]
        // GET: Frontstage/Members
        public async Task<IActionResult> MemberPage()
        {
            
            string aspuserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string aspuseEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            // 從 Session 取得 memberAspID，不知道有沒有掛掉
            var userAspId  = HttpContext.Session.GetString("userId");



            var member =  _context.Members.Where(x => x.AspUser.Id == aspuserId).FirstOrDefault();

            var memberPageVM = new MemberPageVM
            {
                // 將 member 的屬性對應到 MemberPageVM 的屬性
                MemberSid = member.MemberSid,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                BirthDate = member.BirthDate,
                IsChickFarm = member.IsChickFarm,
                IsBlocked = member.IsBlocked,
                Chickcode = member.Chickcode,
                AspUserId = member.AspUserId,
                ProfilePic = member.ProfilePic,
                MemberAreas = member.MemberAreas,
                Certifications = member.Certifications,
            };


            //原本的寫法，不知道為什麼掛了
            // var member = await _context.Members
            //.Include(m => m.AspUser)
            //.Include(m => m.ShoppingRankNoNavigation)
            //.FirstOrDefaultAsync(m => m.AspUserId == aspuserId);


            ViewData["Title"] = "GOOD EGG 會員頁面"; // 設定首頁的標題

            //ViewBag.UserName = user.Name;
            return View(memberPageVM);
            
           
                
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

            //儲存當前使用者的大頭貼與姓名session
            if (member.Name != null) { HttpContext.Session.SetString("userName", member.Name); }
            if (member.ProfilePic != null) { HttpContext.Session.SetString("userProfilePic", member.ProfilePic); }
       

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












        //----------------------------------------------


        private async Task<IActionResult> CheckUserAndMember()
        {
            var aspUser = await _userManager.GetUserAsync(User);

            if (aspUser == null)
            {
                return Redirect("https://localhost:7080/Identity/Account/Login");
            }

            var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspUser.Id);

            if (member == null)
            {
                return Redirect("https://localhost:7080/Identity/Account/Login");
            }

            ViewBag.MemberSid = member.MemberSid;

            return null; 
        }

        public async Task<IActionResult> MemberCenter()
        {

            var result = await CheckUserAndMember();
            if (result != null)
            {
                return result;
            }

			string aspuserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			string aspuseEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
			var userAspId = HttpContext.Session.GetString("userId");

			var member = _context.Members.Where(x => x.AspUser.Id == aspuserId).FirstOrDefault();
			var memberPageVM = new MemberPageVM
			{
				// 將 member 的屬性對應到 MemberPageVM 的屬性
				MemberSid = member.MemberSid,
				Name = member.Name,
				Email = member.Email,
				Phone = member.Phone,
				BirthDate = member.BirthDate,
				IsChickFarm = member.IsChickFarm,
				IsBlocked = member.IsBlocked,
				Chickcode = member.Chickcode,
				AspUserId = member.AspUserId,
				ProfilePic = member.ProfilePic,
				MemberAreas = member.MemberAreas,
				Certifications = member.Certifications,
			};

			ViewData["Title"] = "GOOD EGG 會員中心";

            return View(memberPageVM);

        }


		[HttpPost]
		public IActionResult GetUpdatedProfile()
		{
			string aspuserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


			// 查詢要更新的 Member 資料
			var member = _context.Members.FirstOrDefault(m => m.AspUserId == aspuserId);
			// 從資料庫或其他存儲中獲取更新的圖片路徑和使用者姓名
			var imagePath = member.ProfilePic;  // 假設這是更新後的圖片路徑
			var name = member.Name;  // 假設這是更新後的使用者姓名

			// 返回圖片路徑和姓名
			return Json(new { imageUrl = imagePath, userName = name });
		}





	}
}
