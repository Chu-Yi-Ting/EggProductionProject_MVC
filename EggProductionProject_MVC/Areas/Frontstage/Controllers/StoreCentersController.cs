using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
	[Area("Frontstage")]
	public class StoreCentersController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult trackSearch()
		{
			return View();
		}

        public IActionResult trackControl()
        {
            return View();
        }
    }
}
