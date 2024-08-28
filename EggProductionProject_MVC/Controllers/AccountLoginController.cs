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
        public IActionResult LoginIsRight(EmpLog model)
        {

            var user = _context.Employees
                               .FirstOrDefault(u => u.Account == model.Account && u.Password == model.Password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("EmployeeSid", user.EmployeeSid);
                return Json(user);
            }
            else
            {
               
                return NotFound();
            }

        }




    }
}
