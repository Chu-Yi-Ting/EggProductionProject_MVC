using System.Security.Claims;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Models.MemberVM;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Microsoft.Identity.Client;


namespace EggProductionProject_MVC.Controllers
{
    public class AccountLoginController : Controller
    {
        private readonly EggPlatformContext _context;
		private readonly ILogger<AccountLoginController> _logger; // 注入 ILogger



		public AccountLoginController(EggPlatformContext context, ILogger<AccountLoginController> logger)
        {
            _context = context;
			_logger = logger; // 赋值 logger
		}

        public IActionResult Index()
        {
            return View();
        }




        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MemberLogVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Members
                                   .FirstOrDefault(u => u.Email == model.Email && u.PassWord == model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
                {
                     new Claim(ClaimTypes.Name, user.Name),  // 將使用者的名稱存入 Claim
                        new Claim(ClaimTypes.Email, user.Email) // 您仍然可以將電子郵件作為另一個 Claim 存儲
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true // 設置 Cookie 持久化


                        ,ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // 設置過期時間
                    };


                    try {  await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties); }
                    catch (Exception ex)
{
						_logger.LogError(ex, "SignInAsync 執行過程中發生異常");
					}



                    // 檢查身份驗證是否成功

                    // 登錄成功
                    return RedirectToAction("LoginSuccessPage", "Members", new { area = "Backstage" });
                    


                }
                else
                {
                    
                    ModelState.AddModelError(string.Empty, "無效的登入嘗試。");
                }
            }
            
            
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
