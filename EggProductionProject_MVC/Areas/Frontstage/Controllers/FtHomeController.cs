using Microsoft.AspNetCore.Mvc;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
	public class FtHomeController : Controller
	{
		[Area("Frontstage")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
