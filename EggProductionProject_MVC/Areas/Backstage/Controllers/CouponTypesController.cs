using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using EggProductionProject_MVC.Areas.Backstage.ViewModels;

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


        //[HttpPost]
        //public IActionResult CreateOrEdit(CouponType model)
        //{
        //    if (model.CouponTypeNo == 0) // Assuming Id = 0 means it's a new item
        //    {
        //        // 执行创建逻辑
        //        _context.CouponTypes.Add(model);
        //        _context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        // 执行编辑逻辑
        //        var existingModel = _context.CouponTypes.Find(model.CouponTypeNo);
        //        if (existingModel == null)
        //        {
        //            return NotFound();
        //        }

        //        existingModel.Name = model.Name;
        //        existingModel.Minimum = model.Minimum;
        //        _context.Update(existingModel);
        //        _context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //}

        // GET: Backstage/CouponTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponType = await _context.CouponTypes
                .Include(c => c.PublicStatusNoNavigation)
                .FirstOrDefaultAsync(m => m.CouponTypeNo == id);
            if (couponType == null)
            {
                return NotFound();
            }

            return View(couponType);
        }

        // GET: Backstage/CouponTypes/Create
        public IActionResult Create()
        {
            ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo");
            return View();
        }

        // POST: Backstage/CouponTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Backstage/CouponTypes/Edit/5

        public async Task<IActionResult> GetEditForm(int id)
        {
            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType == null)
            {
                return NotFound();
            }

            return PartialView("_EditForm", couponType);
        }



        //[HttpPost]
        //public async Task<IActionResult> Edit(CouponType model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // 如果模型无效，返回部分视图以显示错误c
        //        return PartialView("_EditForm", model);
        //    }

        //    var couponType = await _context.CouponTypes.FindAsync(model.CouponTypeNo);
        //    if (couponType == null)
        //    {
        //        return NotFound();
        //    }

        //    // 更新 couponType 的属性
        //    couponType.Name = model.Name;
        //    couponType.PublicStatusNo = model.PublicStatusNo;

        //    // 保存更改
        //    await _context.SaveChangesAsync();

        //    // 返回成功的响应（可以是 JSON，也可以是简单的消息）
        //    return Json(new { success = true });
        //}
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    //if (id == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //var couponType = await _context.CouponTypes.FindAsync(id);
        //    //if (couponType == null)
        //    //{
        //    //    return NotFound();
        //    //}
        //    //ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
        //    return View();
        //}

        // POST: Backstage/CouponTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CouponTypeNo,Name,Price,Minimum,PublicStatusNo,StartTime,EndTime,UseAlone,EmployeeSid")] CouponType couponType)
        //{
        //    if (id != couponType.CouponTypeNo)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(couponType);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            //if (!CouponTypeExists(couponType.CouponTypeNo))
        //            //{
        //            //    return NotFound();
        //            //}
        //            //else
        //            //{
        //            //    throw;
        //            //}
        //        }
        //        //return RedirectToAction(nameof(Index));
        //        return PartialView("_EditForm", couponType);
        //    }
        //    ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
        //    return View(couponType);
        //}

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType == null)
            {
                return NotFound();
            }


            return PartialView("_EditForm", couponType); // 返回部分视图
            //return View(couponType);
        }

        // POST: /CouponTypes/Edit
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CouponTypeNo,Name,Price,Minimum,PublicStatusNo,StartTime,EndTime,UseAlone,EmployeeSid")] CouponType couponType)
        //{
        //    if (id != couponType.CouponTypeNo)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //    {
        //        _context.Update(couponType);
        //        await _context.SaveChangesAsync();
        //        return Json(new { success = true });
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return Json(new { success = false, message = "Update failed." });
        //    }
        //    }

        //    ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
        //    return Json(new { success = false, message = "Update failed." });
        //}


        //成功
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit([Bind("CouponTypeNo,Name,Price,Minimum,PublicStatusNo,StartTime,EndTime,UseAlone,EmployeeSid")] CouponType couponType)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(couponType);
        //            await _context.SaveChangesAsync();
        //            return PartialView("_EditForm", couponType);  // 返回 JSON 响应以指示成功
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            return PartialView("_EditForm", couponType);  // 返回 JSON 响应以指示失败
        //        }
        //    }
        //    return PartialView("_EditForm", couponType); // 返回 JSON 响应以指示模型状态无效
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CouponTypeNo,Name,Price,Minimum,PublicStatusNo,StartTime,EndTime,UseAlone,EmployeeSid")] CouponType couponType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(couponType);
                    await _context.SaveChangesAsync();
                    //return Json(new { success = true }); // 返回 JSON 响应以表示成功
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { success = false, message = "Update failed." });
                }
            }
            //return Json(new { success = false, message = "Model state is invalid." });
            return View(couponType);
        }









        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CouponTypeNo,Name,Price,Minimum,PublicStatusNo,StartTime,EndTime,UseAlone,EmployeeSid")] CouponType couponType)
        //{
        //    if (id != couponType.CouponTypeNo)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(couponType);
        //            await _context.SaveChangesAsync();
        //            return Json(new { success = true }); // 返回 JSON 响应以指示成功
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            return Json(new { success = false, message = "Update failed." }); // 返回 JSON 响应以指示失败
        //        }
        //    }

        //    ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
        //    return PartialView("_EditForm", couponType); // 返回部分视图
        //}

        //[HttpGet]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var couponType = await _context.CouponTypes.FindAsync(id);
        //    if (couponType == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
        //    return PartialView("_EditForm", couponType); // 返回部分视图
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CouponTypeNo,Name,Price,Minimum,PublicStatusNo,StartTime,EndTime,UseAlone,EmployeeSid")] CouponType couponType)
        //{
        //    if (id != couponType.CouponTypeNo)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(couponType);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CouponTypeExists(couponType.CouponTypeNo))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PublicStatusNo"] = new SelectList(_context.PublicStatuses, "PublicStatusNo", "PublicStatusNo", couponType.PublicStatusNo);
        //    return PartialView("_EditForm", couponType);
        //}

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponType = await _context.CouponTypes
                .Include(c => c.PublicStatusNoNavigation)
                .FirstOrDefaultAsync(m => m.CouponTypeNo == id);
            if (couponType == null)
            {
                return NotFound();
            }

            return View(couponType);

        }

        // POST: Backstage/CouponTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType != null)
            {
                _context.CouponTypes.Remove(couponType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CouponTypeExists(int id)
        {
            return _context.CouponTypes.Any(e => e.CouponTypeNo == id);
        }
    }
}
