using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;

namespace ExpenseProcessingSystem.Controllers
{
    public class PartialController : Controller
    {
        private readonly int pageSize = 25;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public PartialController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        [Route("/Partial/DMPartial_Payee/")]
        public IActionResult DMPartial_Payee(string sortOrder, string currentFilter, string searchString, string page)
        {
            int? pg = (page == null) ? 1 : int.Parse(page);
            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PayeeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PayeeTINSortParm"] = sortOrder == "PayeeTINDesc" ? "PayeeTIN" : "PayeeTINDesc";

            if (searchString != null)
            {
                pg = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            List<DMPayeeModel> mList = _context.DMPayee.ToList();
            List<DMPayeeViewModel> vmList = new List<DMPayeeViewModel>();
            foreach(DMPayeeModel m in mList)
            {
                DMPayeeViewModel vm = new DMPayeeViewModel {
                    Payee_ID = m.Payee_ID,
                    Payee_Name = m.Payee_Name,
                    Payee_TIN = m.Payee_TIN,
                    Payee_Address = m.Payee_Address,
                    Payee_Type = m.Payee_Type,
                    Payee_No = m.Payee_No,
                    Payee_Creator_ID = m.Payee_Creator_ID,
                    Payee_Approver_ID = m.Payee_Approver_ID,
                    Payee_Created_Date = m.Payee_Created_Date,
                    Payee_Last_Updated = m.Payee_Last_Updated,
                    Payee_Status = m.Payee_Status
                };
                vmList.Add(vm);
            }

            var payee = from e in vmList.AsQueryable()
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
        public IActionResult DMPartial_Dept(string sortOrder, string currentFilter, string tblName, string colName, string searchString, string page)
        {
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

            List<DMDeptModel> mList = _context.DMDept.ToList();
            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            foreach (DMDeptModel m in mList)
            {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_ID = m.Dept_ID,
                    Dept_Name = m.Dept_Name,
                    Dept_Code = m.Dept_Code,
                    Dept_Creator_ID = m.Dept_Creator_ID,
                    Dept_Approver_ID = m.Dept_Approver_ID,
                    Dept_Created_Date = m.Dept_Created_Date,
                    Dept_Last_Updated = m.Dept_Last_Updated,
                    Dept_Status = m.Dept_Status
                };
                vmList.Add(vm);
            }
            var depts = from e in vmList.AsQueryable()
                        select e;
            //FOR FILTERING
            //if (searchString != null)
            //{
            //    _context.Database.ExecuteSqlCommand($"SELECT * FROM Records AS r WHERE r.{colName} IN @ids", new string[] { "1", "2", "3" });

            //    depts = from e in vmList
            //            where e.Dept_Name.Contains(searchString)
            //            select e;
            //}
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

        //-----
        public List<DMPayeeViewModel> PopulateDMPayee()
        {
            List<DMPayeeViewModel> vmList = new List<DMPayeeViewModel>();
            for (var i = 1; i <= 50; i++)
            {
                DMPayeeViewModel vm = new DMPayeeViewModel
                {
                    Payee_ID = i,
                    Payee_Name = "Payee_" + i,
                    Payee_TIN = "TIN_" + i + 5000,
                    Payee_Address = "Address_" + i + 5000,
                    Payee_Type = "Type_" + i + 5000,
                    Payee_No = i + 5000,
                    Payee_Creator_ID = i + 100,
                    Payee_Approver_ID = i + 200,
                    Payee_Last_Updated = DateTime.Parse("1/12/2017", CultureInfo.GetCultureInfo("en-GB"))
                            .Add(DateTime.Now.TimeOfDay),
                    Payee_Status = "For Approval"
                };
                vmList.Add(vm);
            }
            return vmList;
        }
        public List<DMDeptViewModel> PopulateDMDept()
        {
            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            for (var i = 1; i <= 50; i++)
            {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_ID = i,
                    Dept_Name = "Dept_" + i,
                    Dept_Code = "Code" + i + 5000,
                    Dept_Creator_ID = i + 100,
                    Dept_Approver_ID = i + 200,
                    Dept_Last_Updated = DateTime.Parse("1/12/2017", CultureInfo.GetCultureInfo("en-GB"))
                            .Add(DateTime.Now.TimeOfDay),
                    Dept_Status = "For Approval"
                };
                vmList.Add(vm);
            }
            return vmList;
        }
    }
}