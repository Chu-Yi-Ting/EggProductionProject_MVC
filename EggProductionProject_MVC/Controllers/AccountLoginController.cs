﻿using System.Security.Claims;
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
        public async Task<IActionResult> Login(EmpLog model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Employees
                                   .FirstOrDefault(u => u.Account == model.Account && u.Password == model.Password);
                // 保存Session
                HttpContext.Session.SetInt32("EmployeeSid", user.EmployeeSid);

                //    if (user != null)
                //    {
                //        var claims = new List<Claim>
                //    {
                //         new Claim(ClaimTypes.Name, user.Name),  // 將使用者的名稱存入 Claim
                //            new Claim(ClaimTypes.Email, user.Email) // 您仍然可以將電子郵件作為另一個 Claim 存儲
                //    };

                //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //        var authProperties = new AuthenticationProperties
                //        {
                //            // 在這裡可以自訂選項，例如是否持久化 Cookie
                //        };

                //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                //            new ClaimsPrincipal(claimsIdentity),
                //            authProperties);



                //        // 保存Session
                //        HttpContext.Session.SetInt32("EmployeeSid", user.MemberSid);
                //        return RedirectToAction("Index", "Members",  new { area = "Backstage" });
                //    }
                //    else
                //    {

                //        ModelState.AddModelError(string.Empty, "無效的登入嘗試。");
                //    }
            }

            return RedirectToAction("Index", "Members", new { area = "Backstage" });
            return View(model);
        }

    }
}
