using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{

    [Area("Backstage")]
    public class DataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
