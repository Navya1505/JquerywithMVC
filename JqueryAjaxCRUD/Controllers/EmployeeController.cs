using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JqueryAjaxCRUD.Models;
using NetTopologySuite.Operation.Valid;

namespace JqueryAjaxCRUD.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeDBContext _context;

        public EmployeeController(EmployeeDBContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> GetAllEmployees()
        {
            return View(await _context.Employees.ToListAsync());
        }

        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new EmployeeModel());
            else
            {
                var EmpModel = await _context.Employees.FindAsync(id);
                if (EmpModel == null)
                {
                    return NotFound();
                }
                return View(EmpModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("EmpId,Name,ProfileImage,Gender,Department,Salary,StartDate,Notes ")] EmployeeModel employee)
        {
            if (ModelState.IsValid)
            {

                //Insert
                if (id == 0)
                {

                    _context.Add(employee);
                    await _context.SaveChangesAsync();

                }
                //Update
                else
                {
                    try
                    {
                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmpModelExists(employee.EmpId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Employees.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", employee) });
        }




        private bool EmpModelExists(int id)
        {
            return _context.Employees.Any(e => e.EmpId == id);
        }

        //for delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empModel = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(empModel);
            await _context.SaveChangesAsync();
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Employees.ToList()) });
        }
    }
}

