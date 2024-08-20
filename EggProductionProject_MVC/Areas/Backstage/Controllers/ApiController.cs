using EggProductionProject_MVC.Areas.Backstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Drawing;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    //[Route("Backstage/Api/[controller]")]
    public class ApiController : Controller
    {
        private readonly EggPlatformContext _context;

        public ApiController(EggPlatformContext context) 
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Get_Member()
        {
            var Name = from m in _context.Members
                       where m.IsChickFarm == 1
                       select new
                       {
                           value = m.MemberSid,
                           text = m.Name
                       };
                        
            return Json(Name);
        }
        [HttpGet]
        public IActionResult Get_House(int Sid)
        {
            var House = (from m in _context.ChickHouses
                        where m.MemberSid == Sid
                        select new
                        {
                            value = m.HouseSid,
                            text = m.HouseName
                        }).Distinct();

            return Json(House);
        }

        public IActionResult Export(int Sid, int HouseSid)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage pck = new ExcelPackage();
                DataTable dt = new DataTable();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ChickenFarmData");
                string ConnectionString = @"Data Source=.;Initial Catalog=EggPlatform;Integrated Security=True;TrustServerCertificate=True;";

                string query = @"SELECT Name '會員名字',
                                 HouseName '雞舍名字',
                                 DeathRate as '死亡率(%)', 
                                 EggRate as '產蛋率(%)',
                                 d.UnQRate as '蛋不合格率(%)',
                                 RemainingCount as '剩餘雞隻',
                                 DeathAmount as '死亡數',
                                 EggAmount as '當日產蛋數',
                                 UnQAmount as '當日不合格蛋數',
                                 SubcategoryName as '生產總類',
                                 Weekage as 週齡,
                                 convert(varchar,Date,111) '日期'
                                 FROM DailyChickAmountsRate d
							     where MemberSid = @Sid and HouseSid = @HouseSid
                                 ORDER BY MemberSid,HouseSid,Date ";

                List<SqlParameter> paras = new List<SqlParameter>();
                paras.Add(new SqlParameter("@Sid", Sid.ToString()));
                paras.Add(new SqlParameter("@HouseSid", HouseSid.ToString()));

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (paras != null)
                            cmd.Parameters.AddRange(paras.ToArray());
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                // Load the data from the datatable
                ws.Cells["A1"].LoadFromDataTable(dt, true);

                // Format the header for column
                using (ExcelRange rng = ws.Cells["A1:Z1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                pck.SaveAs(stream);
                stream.Position = 0;

                // 返回 Excel 文件
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "exported_data.xlsx";
                return File(stream, contentType, fileName);

            }catch (Exception ex) {
                return BadRequest("Operation failed.");
            }
            
        }
    }
}
