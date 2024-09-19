using EggProductionProject_MVC.Areas.Backstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;

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
        public async Task<IActionResult> Get_Member()
        {
            
            var Name = await (from m in _context.Members
                       where m.IsChickFarm == 1
                       select new
                       {
                           value = m.MemberSid,
                           text = m.Name
                       }).ToListAsync();
                        
            return Json(Name);
        }
        [HttpGet]
        public async Task<IActionResult> Get_House(int Sid)
        {
            var House = await((from m in _context.ChickHouses
                        where m.MemberSid == Sid
                        select new
                        {
                            value = m.HouseSid,
                            text = m.HouseName
                        }).Distinct()).ToListAsync();

            return Json(House);
        }
        [HttpGet]
        public IActionResult Export(int Sid, int HouseSid,string Text1,string Text2)
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

                        // 檢查資料表是否有資料
                        if (dt.Rows.Count == 0)
                        {
                            return Json(new { message = "查無資料，無法生成報表。" });
                        }
                    }
                }

                // Load the data from the datatable
                ws.Cells["A1"].LoadFromDataTable(dt, true);

                // 檢查 Excel 工作表是否有加載資料
                if (ws.Dimension == null)
                {
                    return Json(new { message = "無法將資料加載到 Excel 工作表中。" });
                }

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

                if (stream.Length == 0)
                {
                    return Json(new { message = "Excel 文件生成失敗。" });
                }

                stream.Position = 0;
                var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // 返回 Excel 文件
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = Text1 + "-" + Text2 + ".xlsx";
                var filePath = Path.Combine(wwwrootPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
                stream.Position = 0;

                return File(stream,contentType);
            }
            catch (Exception ex) {
                return BadRequest("Operation failed.");
            }
        }

        public async Task<IActionResult> Send(int id,string Text1, string Text2)
        {
            var Email = ((from m in _context.Members
                          where m.MemberSid == id
                          select m.Email
                         ).FirstOrDefaultAsync()).ToString();
            Email = "heartfuldays307@gmail.com";
            string fileName = $"{Text1}-{Text2}.xlsx";
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string filePath = Path.Combine(wwwrootPath, fileName);

            await SendEmailAsync(Email, "您的EXCEL報表",
                        "<h2>您雞舍的EXCEL報表</h2>", filePath
                        );
            return Ok();
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, string attachmentPath)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress("goodeggtworg@gmail.com");
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = htmlMessage;
            mail.IsBodyHtml = true;

            // 添加附件
            if (!string.IsNullOrEmpty(attachmentPath))
            {
                Attachment attachment = new Attachment(attachmentPath);
                mail.Attachments.Add(attachment);
            }

            var client = new SmtpClient("smtp.gmail.com");
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("goodeggtworg@gmail.com", "gzjn benu swzj omaw");
            client.EnableSsl = true;
            client.Send(mail);
        }
    }
}
