﻿using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Data;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using EggProductionProject_MVC.ViewModels;
using EggProductionProject_MVC.Areas.Frontstage.ViewModels;

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
        
//-----------------------------------行事曆------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> InsertCalendar([FromBody] EggProductionProject_MVC.Models.Calendar Calendar)
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
        public async Task<IActionResult> UpdateCalendar(int id, [FromBody] EggProductionProject_MVC.Models.Calendar updatedEvent)
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

//---------------------------------圖表--------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Get_House(int memberSid)
        {
            var House = await ((from m in _context.ChickHouses
                                where m.MemberSid == memberSid //需抓瀏覽器session的id
                                select new
                                {
                                    value = m.HouseSid,
                                    text = m.HouseName
                                }).Distinct().ToListAsync());

            return Json(House);
        }

        [HttpGet]
        public async Task<IActionResult> Update_Chart(int memberSid,int Judge,int HouseSid,string StartDate, string EndDate)
        {
            var result = new EggProductionReport();
            try {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetDailyData", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Judge", Judge);
                        cmd.Parameters.AddWithValue("@MemberSid", memberSid);
                        cmd.Parameters.AddWithValue("@HouseSid", HouseSid);
                        cmd.Parameters.AddWithValue("@StartDate", !string.IsNullOrWhiteSpace(StartDate) ? StartDate : DBNull.Value);
                        cmd.Parameters.AddWithValue("@EndDate", !string.IsNullOrWhiteSpace(EndDate) ? EndDate : DBNull.Value);
                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                // 設置標題（生產總類）
                                result.Title = reader["生產總類"].ToString();

                                // 判斷根據 Judge 參數返回的結果集類型，並添加對應的數據
                                if (Judge == 1)
                                {
                                    result.Rate.Add(new
                                    {
                                        date = reader["Date"].ToString(),
                                        actualValue = reader["ActualEggRate"],
                                        normalizedValue = reader["NormalizedEggRate"]
                                    });
                                }
                                else if (Judge == 2)
                                {
                                    result.Rate.Add(new
                                    {
                                        date = reader["日期"].ToString(),
                                        value = reader["UnQRate"]
                                    });
                                }
                                else
                                {
                                    result.Rate.Add(new
                                    {
                                        date = reader["Date"].ToString(),
                                        actualValue = reader["ActualDeathRate"],
                                        normalizedValue = reader["NormalizedDeathRate"]
                                    });
                                }
                            }
                        }
                    }
                }
                return Json(result);
            }catch (Exception ex) {
                return StatusCode(500, ex);
            }

        }
//------------------------------------------雞農新增資料---------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Get_Area(int memberSid)
        {
            var Area = await ((from m in _context.MemberAreas
                                where m.MemberSid == memberSid //需抓瀏覽器session的id
                                select new
                                {
                                    value = m.AreaSid,
                                    text = m.MemberAddress
                                }).Distinct().ToListAsync());

            return Json(Area);
        }

        [HttpPost]
        public async Task<IActionResult> Insert_Record1([FromBody] CombinedRecord request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "提供的資料無效。" });

            var subCategoryNo = await (from m in _context.ChickHouses
                                       where m.HouseSid == request.HouseSid && m.MemberSid == request.MemberSid
                                       select m.SubcategoryNo).FirstOrDefaultAsync();

            bool dailyEggRateExists = await _context.DailyEggRes
                .AnyAsync(e => e.HouseSid == request.HouseSid && e.Date == DateOnly.FromDateTime(request.Date));

            bool chickDeathExists = await _context.ChickDeaths
                .AnyAsync(d => d.HouseSid == request.HouseSid && d.Date == DateOnly.FromDateTime(request.Date));

            if (dailyEggRateExists || chickDeathExists)
                return BadRequest(new { success = false, message = "該日該雞舍已存在記錄，無法新增。" });

            try
            {
                // 插入到 DailyEggRate 表
                var dailyEggRate = new DailyEggRe
                {
                    HouseSid = request.HouseSid,
                    MemberSid = request.MemberSid,
                    EggAmount = request.EggAmount,
                    UnQamount = request.UnQAmount,
                    SubcategoryNo = (int)subCategoryNo,
                    Date = DateOnly.FromDateTime(request.Date)
                };

                _context.DailyEggRes.Add(dailyEggRate);

                    // 插入到 ChickDeath 表
                var chickDeath = new ChickDeath
                {
                    HouseSid = request.HouseSid,
                    DeathAmount = request.DeathAmount,
                    Date = DateOnly.FromDateTime(request.Date)
                };

                _context.ChickDeaths.Add(chickDeath);

                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "記錄已成功建立！" });
            }catch (Exception ex){
                return StatusCode(500, new { success = false, message = "伺服器錯誤", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Insert_Record2([FromBody] CombinedRecord2 request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "提供的資料無效。" });

            try
            {
                // 插入到 DailyEggRate 表
                var areaFeed = new AreaFeed
                {
                    AreaSid = request.AreaSid,
                    MemberSid = request.MemberSid,
                    LotNo = request.LotNo,
                    Cost = request.Cost,
                    Weight = request.Weight,
                    Date = DateOnly.FromDateTime(request.Date)
                };

                _context.AreaFeeds.Add(areaFeed);


                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "記錄已成功建立！" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "伺服器錯誤", error = ex.Message });
            }
        }

    }
}
