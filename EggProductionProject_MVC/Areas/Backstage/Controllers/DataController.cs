using EggProductionProject_MVC.Areas.Backstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Security.Policy;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{

    [Area("Backstage")]
    public class DataController : Controller
    {
        private EggPlatformContext _context;

        public DataController(EggPlatformContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Excel_Export()
        {
            return View();
        }
        
    }
}
