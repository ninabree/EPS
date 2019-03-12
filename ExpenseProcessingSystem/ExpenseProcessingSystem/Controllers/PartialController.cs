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
        private SortService _sortService;

        public PartialController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _service = new PartialService(_httpContextAccessor, _context);
            _sortService = new SortService();
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
            ViewData["PayeeTINSortParm"] = sortOrder == "payee_TIN_desc" ? "payee_TIN" : "payee_TIN_desc";
            ViewData["PayeeAddSortParm"] = sortOrder == "payee_add_desc" ? "payee_add" : "payee_add_desc";
            ViewData["PayeeTypeSortParm"] = sortOrder == "payee_type_desc" ? "payee_type" : "payee_type_desc";
            ViewData["PayeeNoSortParm"] = sortOrder == "payee_no_desc" ? "payee_no" : "payee_no_desc";
            ViewData["PayeeCreatorSortParm"] = sortOrder == "payee_creatr_desc" ? "payee_creatr" : "payee_creatr_desc";
            ViewData["PayeeApproverSortParm"] = sortOrder == "payee_approvr_desc" ? "payee_approvr" : "payee_approvr_desc";
            ViewData["PayeeStatusSortParm"] = sortOrder == "payee_stat_desc" ? "payee_stat" : "payee_stat_desc";

            if (searchString != null){ pg = 1; }
            else{ searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            
            //populate and sort
            var sortedVals = _sortService.SortData(_service.populatePayee(colName, searchString), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                Payee = PaginatedList<DMPayeeViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMPayeeViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
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
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DeptSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DeptCodeSortParm"] = sortOrder == "dept_code_desc" ? "dept_code" : "dept_code_desc";
            ViewData["DeptCreatorSortParm"] = sortOrder == "dept_creatr_desc" ? "dept_creatr" : "dept_creatr_desc";
            ViewData["DeptApproverSortParm"] = sortOrder == "dept_approvr_desc" ? "dept_approvr" : "dept_approvr_desc";
            ViewData["DeptLastUpdatedSortParm"] = sortOrder == "dept_last_updte_desc" ? "dept_last_updte" : "dept_last_updte_desc";
            ViewData["DeptStatusSortParm"] = sortOrder == "dept_stat_desc" ? "dept_stat" : "dept_stat_desc";

            if (searchString != null)
            {
                pg = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateDept(colName, searchString), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                Dept = PaginatedList<DMDeptViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMDeptViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }
    }
}