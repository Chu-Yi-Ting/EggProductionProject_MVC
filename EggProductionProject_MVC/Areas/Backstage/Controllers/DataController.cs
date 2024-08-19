using EggProductionProject_MVC.Models;
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

        /*public IActionResult Export()
        {
            ExcelPackage pck = new ExcelPackage();
            var worksheet = pck.Workbook.Worksheets.Add("Sheet1");


            var f = from c in _context.DailyChickAmountsRates
                    select new
                    {

                    };


            var stream = new MemoryStream();
            pck.SaveAs(stream);
            stream.Position = 0;

            // 返回 Excel 文件
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "exported_data.xlsx";

            return File(stream, contentType, fileName);
        }*/
    }
}
