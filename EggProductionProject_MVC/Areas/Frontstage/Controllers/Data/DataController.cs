using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers.Data
{
    [Area("Frontstage")]
    public class DataController : Controller
    {
        private EggPlatformContext _context;
        public DataController(EggPlatformContext context) 
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var MemberSid = HttpContext.Session.GetInt32("userMemberSid");
            var user = _context.Members.Where(x => x.MemberSid == MemberSid).FirstOrDefault();
            @ViewData["Title"] = "GOOD EGG 生產分析";
            return View(user);
        }
    }
}
