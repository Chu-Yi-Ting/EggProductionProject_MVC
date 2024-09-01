using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers.Data
{
    [Area("Frontstage")]
    public class DataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
