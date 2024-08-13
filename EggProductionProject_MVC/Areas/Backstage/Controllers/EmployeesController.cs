using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class EmployeesController : Controller
    {
        private readonly EggPlatformContext _context;

        public EmployeesController(EggPlatformContext context)
        {
            _context = context;
        }

        // GET: Backstage/Employees
        public async Task<IActionResult> Index()
        {
            var eggPlatformContext = _context.Employees.Include(e => e.EmpDepartmentS);
            return View(await eggPlatformContext.ToListAsync());
        }

        // GET: Backstage/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.EmpDepartmentS)
                .FirstOrDefaultAsync(m => m.EmployeeSid == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Backstage/Employees/Create
        public IActionResult Create()
        {
            ViewData["EmpDepartmentSid"] = new SelectList(_context.EmpDepartments, "EmpDepartmentSid", "EmpDepartmentSid");
            return View();
        }

        // POST: Backstage/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeSid,EmpName,EmpDepartmentSid,EmpPhone,ReportTo,Password,Account")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpDepartmentSid"] = new SelectList(_context.EmpDepartments, "EmpDepartmentSid", "EmpDepartmentSid", employee.EmpDepartmentSid);
            return View(employee);
        }

        // GET: Backstage/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["EmpDepartmentSid"] = new SelectList(_context.EmpDepartments, "EmpDepartmentSid", "EmpDepartmentSid", employee.EmpDepartmentSid);
            return View(employee);
        }

        // POST: Backstage/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeSid,EmpName,EmpDepartmentSid,EmpPhone,ReportTo,Password,Account")] Employee employee)
        {
            if (id != employee.EmployeeSid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeSid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpDepartmentSid"] = new SelectList(_context.EmpDepartments, "EmpDepartmentSid", "EmpDepartmentSid", employee.EmpDepartmentSid);
            return View(employee);
        }

        // GET: Backstage/Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.EmpDepartmentS)
                .FirstOrDefaultAsync(m => m.EmployeeSid == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Backstage/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeSid == id);
        }
    }
}
