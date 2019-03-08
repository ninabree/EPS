using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseProcessingSystem.Controllers
{
    public class PartialController : Controller
    {
        private readonly int pageSize = 25;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private PartialService _service;

        public PartialController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _service = new PartialService(_httpContextAccessor, _context);
        }
        [Route("/Partial/DMPartial_Payee/")]
        public IActionResult DMPartial_Payee(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PayeeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PayeeTINSortParm"] = sortOrder == "PayeeTINDesc" ? "PayeeTIN" : "PayeeTINDesc";

            if (searchString != null){ pg = 1; }
            else{ searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            
            //Sort
            var payee = from e in _service.populatePayee(colName, searchString).AsQueryable()
                        select e;
            switch (sortOrder)
            {
                case "name_desc":
                    payee = payee.OrderByDescending(s => s.Payee_Name);
                    ViewData["glyph-payee"] = "glyphicon-menu-up";
                    break;
                case "PayeeTIN":
                    payee = payee.OrderBy(s => s.Payee_TIN);
                    ViewData["glyph-payee-tin"] = "glyphicon-menu-down";
                    break;
                case "PayeeTINDesc":
                    payee = payee.OrderByDescending(s => s.Payee_TIN);
                    ViewData["glyph-payee-tin"] = "glyphicon-menu-up";
                    break;
                default:
                    payee = payee.OrderBy(s => s.Payee_Name);
                    ViewData["glyph-payee"] = "glyphicon-menu-down";
                    break;
            }
            DMViewModel VM = new DMViewModel()
            {
                Payee = PaginatedList<DMPayeeViewModel>.CreateAsync(payee.AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_Dept/")]
        public IActionResult DMPartial_Dept(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DeptSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DeptCodeSortParm"] = sortOrder == "DeptCodeDesc" ? "DeptCode" : "DeptCodeDesc";

            if (searchString != null)
            {
                pg = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var depts = from e in _service.populateDept(colName, searchString).AsQueryable()
                        select e;
            switch (sortOrder)
            {
                case "name_desc":
                    depts = depts.OrderByDescending(s => s.Dept_Name);
                    ViewData["glyph-dept"] = "glyphicon-menu-up";
                    break;
                case "DeptCode":
                    depts = depts.OrderBy(s => s.Dept_Code);
                    ViewData["glyph-dept-code"] = "glyphicon-menu-down";
                    break;
                case "DeptCodeDesc":
                    depts = depts.OrderByDescending(s => s.Dept_Code);
                    ViewData["glyph-dept-code"] = "glyphicon-menu-up";
                    break;
                default:
                    depts = depts.OrderBy(s => s.Dept_ID);
                    ViewData["glyph-dept"] = "glyphicon-menu-down";
                    break;
            }
            DMViewModel VM = new DMViewModel()
            {
                Dept = PaginatedList<DMDeptViewModel>.CreateAsync(depts.AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }
    }
}