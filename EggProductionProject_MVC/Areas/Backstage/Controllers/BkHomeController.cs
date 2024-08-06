using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    public class BkHomeController : Controller
    {
        [Area("Backstage")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
