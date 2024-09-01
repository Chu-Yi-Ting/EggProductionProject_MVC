using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Data;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers.Data
{
    [Area("Frontstage")]
    public class DataApiController : Controller
    {
        private readonly EggPlatformContext _context;
        private string _connectionString = @"Data Source=.;Initial Catalog=EggPlatform;Integrated Security=True;TrustServerCertificate=True;";

        public DataApiController(EggPlatformContext context)
        {
            _context = context;
        }

        /*public IActionResult Get_House()
        {
            var House = (from m in _context.ChickHouses
                         where m.MemberSid == 1 //需抓瀏覽器session的id
                         select new
                         {
                             value = m.HouseSid,
                             text = m.HouseName
                         }).Distinct();

            return Json(House);
        }

        [HttpGet]
        public IActionResult Export_Chart()
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
							     where MemberSid = 1  
                                       and HouseSid = 1
                                 ORDER BY MemberSid,HouseSid,Date ";

                List<SqlParameter> paras = new List<SqlParameter>();

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

            }
            catch (Exception ex)
            {
                return BadRequest("Operation failed.");
            }

        }*/


        [HttpPost]
        public async Task<IActionResult> InsertCalendar([FromBody] Models.Calendar Calendar)
        {
            if(!ModelState.IsValid)
                return BadRequest(new { success = false, message = "提供的資料無效。" });
            try{

                Calendar.MemberSid = 1;
                Calendar.InsertDate = DateOnly.FromDateTime(DateTime.Now);
                Calendar.Finished = 0;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertCalendar", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MemberSid", Calendar.MemberSid);
                        cmd.Parameters.AddWithValue("@Title", Calendar.Title);
                        cmd.Parameters.AddWithValue("@TodoList", Calendar.TodoList);
                        cmd.Parameters.AddWithValue("@StartDate", Calendar.StartDate);
                        cmd.Parameters.AddWithValue("@InsertDate", Calendar.InsertDate);
                        cmd.Parameters.AddWithValue("@Finished", Calendar.Finished);

                        await conn.OpenAsync();
                        var result = await cmd.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new { success = true, message = "事件已成功建立！" });
            }
            catch (Exception ex) { 
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "提供的資料無效。" });

            try
            {
                var result = await ((from c in _context.Calendars
                             where c.MemberSid == 1 //需抓瀏覽器session的id
                             select new
                             {
                                 id = c.CalendarSid,
                                 title = c.Title,
                                 start = c.StartDate,
                                 description = c.TodoList,
                                 finished = c.Finished,
                                 allDay = true
                             }).ToListAsync());

                return Ok(result);
            }
            catch (Exception ex){
                return StatusCode(500,ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCalendar(int id, [FromBody] Models.Calendar updatedEvent)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "提供的數據無效。" });

            var existingEvent = await _context.Calendars.FindAsync(id);

            if (existingEvent == null)
                return NotFound(new { success = false, message = "找不到該事件。" });

            // 更新現有事件的屬性
            existingEvent.Title = updatedEvent.Title;
            existingEvent.TodoList = updatedEvent.TodoList;
            existingEvent.Finished = updatedEvent.Finished;
            existingEvent.InsertDate = DateOnly.FromDateTime(DateTime.Now); ; // 更新時間

            try{
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "事件已成功更新！" });
            }
            catch (Exception ex){
                return StatusCode(500, ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCalendar(int id)
        {
            var existingEvent = await _context.Calendars.FindAsync(id);
            if (existingEvent == null)
                return NotFound(new { success = false, message = "找不到該事件。" });

            _context.Calendars.Remove(existingEvent);

            try{
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "事件已成功刪除！" });
            }
            catch (Exception ex){
                return StatusCode(500, ex);
            }
        }

    }
}
