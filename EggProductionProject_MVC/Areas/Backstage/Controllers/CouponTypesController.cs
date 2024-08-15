using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Areas.Backstage.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Humanizer;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class CouponTypesController : Controller
    {
        private readonly EggPlatformContext _context;

        

        public CouponTypesController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Backstage/CouponTypes
        public IActionResult  Index()
        {
            var eggPlatformContext = _context.CouponTypes.Include(c => c.PublicStatusNoNavigation);
            ViewData["TimeOptions"] = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Value = "1", Text = "近三月" },
                        new SelectListItem { Value = "2", Text = "本月"},
                        new SelectListItem { Value = "3", Text = "本週"}
                    }, "Value", "Text");

            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "StatusDescription");
            ViewData["TypeOptions"] = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Value = "1", Text = "常駐" },
                        new SelectListItem { Value = "2", Text = "限時" }
                        
                    }, "Value", "Text");
            ViewData["NameOptions"] = new SelectList(_context.CouponTypes, "CouponTypeNo", "Name");
            return View();
        }

        public IActionResult Orders(string id)
        {
            //ViewBag.Orders = new SelectList(_context.Orders.Where(o => o.CustomerId == id), "OrderId", "OrderId");

            if (id == "1")
            {
                ViewData["NameOptions"] = new SelectList(_context.CouponTypes.Where(o => !o.StartTime.HasValue), "CouponTypeNo", "Name");
            }
            else if (id == "2") {
                ViewData["NameOptions"] = new SelectList(_context.CouponTypes.Where(o => o.StartTime.HasValue), "CouponTypeNo", "Name");
            }
            else
            {
                ViewData["NameOptions"] = new SelectList(_context.CouponTypes, "CouponTypeNo", "Name");
            }

           
            return PartialView("_SelectNamePartialcs");
        }



        public JsonResult IndexJson()
        {
            var publicStatuses = _context.PublicStatuses.ToList();
            var couponTypes = _context.CouponTypes.ToList();
            var employees = _context.Employees.ToList();

            var CouponTypesViewModels = couponTypes.Select(p => new CouponTypeViewModels
            {
                CouponTypeNo = p.CouponTypeNo,
                Name = p.Name,
                Price = p.Price,
                Minimum = p.Minimum,
                PublicStatusNo = p.PublicStatusNo,
                StatusDescription = publicStatuses.FirstOrDefault(s => s.PublicStatusNo == p.PublicStatusNo)?.StatusDescription,
                StartTime = p.StartTime,
                EndTime = p.EndTime,
                UseAlone = p.UseAlone,
                EmpName = employees.FirstOrDefault(s => s.EmployeeSid == p.EmployeeSid)?.EmpName,
            });

            return Json(CouponTypesViewModels);
        }


        public class CouponSec
        {
            public string? TimeSec { get; set; }
            public string? TypeSec { get; set; }
            public string? NameSec { get; set; }
            public string? StatusSec { get; set; }
        }

   
        internal class CoupontypeDto
        {
            public int CouponTypeNo { get; set; }
            public DateTime? StartTime { get; set; }
            public string? Name { get; set; }
        }

        [HttpPost]
        public JsonResult CouponList1([FromBody] CouponSec sec)
        {
            var couponStatuses = _context.CouponStatuses.Select(c => new couponStatusDto
            {
                CouponStatusNo = c.CouponStatusNo,
                Status = c.Status
            }).ToList();
            var members = _context.Members.Select(c => new memberDto
            {
                MemberSid = c.MemberSid,
                Name = c.Name
            }).ToList();




            var ori_ct = _context.CouponTypes
               .Select(c => new CoupontypeDto
               {
                   CouponTypeNo = c.CouponTypeNo,
                   StartTime = c.StartTime,
                   Name = c.Name
               })
               .ToList();
            var one_ct = ori_ct;

            var ori_co = _context.Coupons.ToList();
            var end_co = ori_co;


            if (sec.NameSec != "all" && sec.NameSec != null && sec.NameSec != "" && sec.NameSec != "0")
            {
                if (!int.TryParse(sec.NameSec, out int nameSecInt))
                {
                    return new JsonResult(new { Success = false, Message = "Invalid CouponTypeNo" });
                }
         
                ori_ct = _context.CouponTypes
               .Select(c => new CoupontypeDto
               {
                   CouponTypeNo = c.CouponTypeNo,
                   StartTime = c.StartTime,
                   Name = c.Name
               })
                .Where(c => c.CouponTypeNo == nameSecInt)
                .ToList();
            }
            

            switch (sec.TypeSec)
            {
                case "1":
                    one_ct = ori_ct
                     .Where(c => !c.StartTime.HasValue)
                     .ToList();
                    break;

                case "2":
                    one_ct = ori_ct
                    .Where(c => c.StartTime.HasValue)
                    .ToList();
                    break;

                default:
                    one_ct = ori_ct
                   .ToList();
                    break;
            }

            //type結束回到 coupo,拿到被塞選的結果
            var filteredCouponTypeNos = one_ct.Select(c => c.CouponTypeNo).ToList();



            if (sec.StatusSec != "all" && sec.StatusSec != null && sec.StatusSec != "" && sec.StatusSec != "0")
            {
                if (!int.TryParse(sec.StatusSec, out int statusSecInt))
                {
                    return new JsonResult(new { Success = false, Message = "Invalid CouponTypeNo" });
                }
                ori_co = _context.Coupons
                .Where(c => c.CouponStatusNo == statusSecInt)
                .ToList();
            }
            else
            {
                ori_co = _context.Coupons
                .ToList();
            }




            switch (sec.TimeSec)
            {
                case "1":

                    DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);

                    end_co = ori_co
                    .Where(c => filteredCouponTypeNos.Contains((int)c.CouponTypeNo) &&
                    c.CollectionTime >= threeMonthsAgo)
                    .ToList();
                    break;

                case "2":
                    DateTime oneMonthsAgo = DateTime.Now.AddMonths(-1);
                    end_co = _context.Coupons
                      .Where(c => filteredCouponTypeNos.Contains((int)c.CouponTypeNo) &&
                    c.CollectionTime >= oneMonthsAgo)
                    .ToList();
                    break;

                case "3":
                    DateTime now = DateTime.Now;
                    DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                    DateTime endOfWeek = startOfWeek.AddDays(6);

                    end_co = ori_co
                    .Where(c => filteredCouponTypeNos.Contains((int)c.CouponTypeNo) &&
                    c.CollectionTime >= startOfWeek &&
                    c.CollectionTime <= endOfWeek)
                    .ToList();
                    break;

                default:
                    end_co = ori_co
                     .Where(c => filteredCouponTypeNos.Contains((int)c.CouponTypeNo))
                     .ToList();
                    break;
            }

            var CouponViewModels = end_co.Select(p => new CouponViewModel
            {
                CouponSid = p.CouponSid,
                CouponTypeNo = p.CouponTypeNo,
                Name = ori_ct.FirstOrDefault(s => s.CouponTypeNo == p.CouponTypeNo)?.Name,
                CouponStatusNo = p.CouponStatusNo,
                Status = couponStatuses.FirstOrDefault(s => s.CouponStatusNo == p.CouponStatusNo)?.Status,
                CollectionTime = p.CollectionTime,
                MemberName = members.FirstOrDefault(s => s.MemberSid == p.MemberSid)?.Name
            });



            return Json(CouponViewModels);
        }

        public JsonResult CouponList()
        {
            var couponStatuses = _context.CouponStatuses.ToList();


            var coupons = _context.Coupons.ToList();
            var couponTypes = _context.CouponTypes.ToList();
            var members = _context.Members.ToList();

            var CouponViewModels = coupons.Select(p => new CouponViewModel
            {
                CouponSid = p.CouponSid,
                CouponTypeNo = p.CouponTypeNo,
                Name = couponTypes.FirstOrDefault(s => s.CouponTypeNo == p.CouponTypeNo)?.Name,
                CouponStatusNo = p.CouponStatusNo,
                Status = couponStatuses.FirstOrDefault(s => s.CouponStatusNo == p.CouponStatusNo)?.Status,
                CollectionTime = p.CollectionTime,
                MemberName = members.FirstOrDefault(s => s.MemberSid == p.MemberSid)?.Name
            });

            return Json(CouponViewModels);
        }

        // GET: CouponTypes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id < 0)
            {
                return BadRequest("无效的ID");
            }
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses,"PublicStatusNo","StatusDescription");

            ViewData["UseAloneOptions"] = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Value = "1", Text = "是" },
                        new SelectListItem { Value = "0", Text = "否" }
                    },"Value", "Text");

            //ViewData["TypeOptions"] = new SelectList(
            //        new List<SelectListItem>
            //        {
            //            new SelectListItem { Value = "1", Text = "限時" },
            //            new SelectListItem { Value = "0", Text = "常駐" }
            //        }, "Value", "Text");

            CouponType couponType;

            if (id == 0)
            {
                couponType = new CouponType
                {
                    CouponTypeNo = 0 
                };
            }
            else
            {
                couponType = await _context.CouponTypes.FindAsync(id);
                if (couponType == null)
                {
                    return NotFound();
                }
            }

            return PartialView("_EditForm", couponType);
        }

        // POST: CouponTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(CouponType couponType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    

                    if (couponType.CouponTypeNo == 0) // 新建
                    {
                        couponType.EmployeeSid = 1;
                        _context.Add(couponType);
                    }
                    else // 更新
                    {
                        couponType.EmployeeSid = 1;
                        _context.Update(couponType);
                    }
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            // 如果模型状态无效，返回错误信息
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
            return Json(new { success = false, message = string.Join(", ", errors) });
        }

    
    }

    internal class memberDto
    {
        public int MemberSid { get; set; }
        public string Name { get; set; }
    }

    internal class couponStatusDto
    {
        public int CouponStatusNo { get; set; }
        public string Status { get; set; }
    }
}
