using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Models.MemberVM;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class MemLoginController : Controller
    {
        private readonly EggPlatformContext _context;
        public IActionResult Index()
        {
            return View();
        }

        public MemLoginController(EggPlatformContext context)
        {
            _context = context;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MemberLogVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Members
                                   .FirstOrDefault(u => u.Email == model.Email && u.PassWord == model.Password);
                //// 保存Session
                HttpContext.Session.SetInt32("MemberSid", user.MemberSid);

                //if (user != null)
                //{
                //    var claims = new List<Claim>
                //    {
                //         new Claim(ClaimTypes.Name, user.Name),  // 將使用者的名稱存入 Claim
                //            new Claim(ClaimTypes.Email, user.Email) // 您仍然可以將電子郵件作為另一個 Claim 存儲
                //    };

                //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //    var authProperties = new AuthenticationProperties
                //    {
                //        // 在這裡可以自訂選項，例如是否持久化 Cookie
                //    };

                //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                //        new ClaimsPrincipal(claimsIdentity),
                //        authProperties);



                //    // 保存Session
                //    HttpContext.Session.SetInt32("MemberSid", user.MemberSid);
                //    return RedirectToAction("Index", "FtHome", new { area = "Frontstage" });
                //}
                //else
                //{

                //    ModelState.AddModelError(string.Empty, "無效的登入嘗試。");
                //}
            }

            return RedirectToAction("Index", "FtHome", new { area = "Frontstage" });
            return View(model);
        }
    }
}
