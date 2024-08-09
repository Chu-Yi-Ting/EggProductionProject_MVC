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
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo");
            return View();
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


        // GET: CouponTypes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType == null)
            {
                return NotFound();
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
                        _context.Add(couponType);
                    }
                    else // 更新
                    {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CouponTypeNo,Name,Price,Minimum,PublicStatusNo,StartTime,EndTime,UseAlone,EmployeeSid")] CouponType couponType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(couponType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
            return View(couponType);
        }

       
      

        private bool CouponTypeExists(int id)
        {
            return _context.CouponTypes.Any(e => e.CouponTypeNo == id);
        }
    }
}
