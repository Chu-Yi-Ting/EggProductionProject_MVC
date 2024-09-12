using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using EggProductionProject_MVC.HTTPModels;

namespace EggProductionProject_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebScrapingService _scrapingService;

        public HomeController(ILogger<HomeController> logger, WebScrapingService scrapingService)
        {
            _logger = logger;
            _scrapingService = scrapingService;
        }

        public async Task<IActionResult> Index()
        {
            string url = "http://www.foodchina.com.tw/model/marketing/ListNew.aspx?PID=6";
            var tableData = await _scrapingService.FetchTableDataAsync(url);
            return View(tableData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
