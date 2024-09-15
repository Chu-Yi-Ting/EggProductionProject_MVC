using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
	[Area("Frontstage")]
	public class StoreCentersController : Controller
	{

		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly EggPlatformContext _context;

		public StoreCentersController(IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, EggPlatformContext context)
		{
			_hostingEnvironment = webHostEnvironment;
			_userManager = userManager;
			_context = context;
		}

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

			return null; // 返回 null 表示检查成功
		}
		public async Task<IActionResult> Index()
		{
			var result = await CheckUserAndMember();
			if (result != null)
			{
				return result;
			}

			ViewData["Title"] = "GOOD EGG 賣家中心-訂單管理";

            return View();
		}

		public async Task<IActionResult> trackSearch()
		{
			var result = await CheckUserAndMember();
			if (result != null)
			{
				return result;
			}
            ViewData["Title"] = "GOOD EGG 賣家中心-物流查詢";
            return View();
		}

        public async Task<IActionResult>  trackControl()
        {
			var result = await CheckUserAndMember();
			if (result != null)
			{
				return result;
			}
            ViewData["Title"] = "GOOD EGG 賣家中心-物流設定";
            return View();
        }
    }
}
