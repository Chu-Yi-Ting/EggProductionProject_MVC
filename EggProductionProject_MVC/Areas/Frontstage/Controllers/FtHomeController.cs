using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
	[Area("Frontstage")]
	public class FtHomeController : Controller
	{
		
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Index2()
        {
            return View();
        }

        public IActionResult Index3()
        {
            return View();
        }

        public IActionResult Index_light()
		{
			return View();
		}

		public IActionResult Index_store()
		{
			return View();
		}
		public IActionResult Index_store2()
		{
			return View();
		}
        public IActionResult Index_store3()
        {
            return View();
        }

        public IActionResult OnlineShop()
        {
            return View();
        }

    }
}
