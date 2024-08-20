using System.Security.Claims;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Models.MemberVM;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;


namespace EggProductionProject_MVC.Controllers
{
    public class AccountLoginController : Controller
    {
        private readonly EggPlatformContext _context;
        public AccountLoginController(EggPlatformContext context)
        {
            _context = context;
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
                    new Claim(ClaimTypes.Name, user.Email),
                    // 可以在這裡添加更多的聲明（Claims），如角色
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        // 在這裡可以自訂選項，例如是否持久化 Cookie
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Members",  new { area = "Backstage" });
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
