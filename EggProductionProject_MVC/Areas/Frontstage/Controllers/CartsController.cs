using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class CartsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
