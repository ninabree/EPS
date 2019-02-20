using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseProcessingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly int pageSize = 30;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public HomeController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            return View();
        }
        public IActionResult Entry()
        {
            return View();
        }
        public IActionResult Close(string sortOrder, string currentFilter, string searchString, int? page)
        {
            return View();
        }
        public IActionResult DM(string sortOrder, string currentFilter, string tblName, string colName, string searchString, int? page, string partialName)
        {
            ViewData["sortOrder"] = sortOrder;
            ViewData["currentFilter"] = searchString;
            ViewData["tblName"] = tblName;
            ViewData["colName"] = colName;
            ViewData["searchString"] = searchString;
            ViewData["page"] = page;
            ViewData["partialName"] = (partialName == null) ? "DMPartial_Payee" : partialName;
            return View();
        }
        public IActionResult Report()
        {
            return View();
        }
        public IActionResult BM(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //CheckScreenRes();

            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AccountSortParm"] = String.IsNullOrEmpty(sortOrder) ? "acc_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type_desc" ? "Type" : "Type_desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var bmmvList = PopulateBM().AsQueryable();
            var bm = from e in bmmvList
                     select e;
            switch (sortOrder)
            {
                case "acc_desc":
                    bm = bm.OrderByDescending(s => s.BM_Account);
                    ViewData["glyph-acc"] = "glyphicon-menu-up";
                    break;
                case "Type":
                    bm = bm.OrderBy(s => s.BM_Type);
                    ViewData["glyph-type"] = "glyphicon-menu-down";
                    break;
                case "Type_desc":
                    bm = bm.OrderByDescending(s => s.BM_Type);
                    ViewData["glyph-type"] = "glyphicon-menu-up";
                    break;
                default:
                    bm = bm.OrderBy(s => s.BM_Account);
                    ViewData["glyph-acc"] = "glyphicon-menu-down";
                    break;
            }

            //pagination
            return View(PaginatedList<BMViewModel>.CreateAsync(bm.AsNoTracking(), page ?? 1, pageSize));
        }
        public IActionResult UM()
        {
            return View();
        }
        public IActionResult Entry_CV()
        {
            return View();
        }
        public IActionResult Entry_DDV()
        {
            return View();
        }
        public IActionResult Entry_PCV()
        {
            return View();
        }
        public IActionResult Entry_SS()
        {
            return View();
        }
        public IActionResult Entry_NC()
        {
            return View();
        }

        [HttpPost]
        //[Route("/RedirectCont/")]
        public IActionResult RedirectCont(string Cont, string Method)
        {
            return RedirectToAction(Method, Cont);
        }
        [HttpPost]
        public IActionResult RedirectCont2(string Cont, string Method, string[] IdsArr)
        {
            return RedirectToAction(Method, Cont, new { IdsArr = IdsArr });
        }

        //------------------------------------------------------------------
        //CRUD
        [HttpPost]
        public IActionResult AddPayee(NewPayeeListViewModel model)
        {
            List<DMPayeeModel> vmList = new List<DMPayeeModel>();
            foreach (NewPayeeViewModel dm in model.NewPayeeVM)
            {
                DMPayeeModel m = new DMPayeeModel
                {
                    Payee_Name = dm.Payee_Name,
                    Payee_TIN = dm.Payee_TIN,
                    Payee_Address = dm.Payee_Address,
                    Payee_Type = dm.Payee_Type,
                    Payee_No = dm.Payee_No,
                    Payee_Creator_ID = int.Parse(_session.GetString("UN")),
                    Payee_Created_Date = DateTime.Now,
                    Payee_Last_Updated = DateTime.Now,
                    Payee_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (ModelState.IsValid)
            {
                _context.DMPayee.AddRange(vmList);
                _context.SaveChanges();
            }

            return RedirectToAction("DM", "Home");
        }
        [HttpPost]
        public IActionResult AddDept(NewDeptListViewModel model)
        {
            List<DMDeptModel> vmList = new List<DMDeptModel>();
            foreach(NewDeptViewModel dm in model.NewDeptVM)
            {
                DMDeptModel m = new DMDeptModel
                {
                    Dept_Name = dm.Dept_Name,
                    Dept_Code = dm.Dept_Code,
                    Dept_Creator_ID = int.Parse(_session.GetString("UN")),
                    Dept_Created_Date = DateTime.Now,
                    Dept_Last_Updated = DateTime.Now,
                    Dept_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (ModelState.IsValid)
            {
                _context.DMDept.AddRange(vmList);
                _context.SaveChanges();
            }

            return RedirectToAction("DM", "Home");
        }

        public void CheckScreenRes()
        {
            //int h = Screen.AllScreens.GetLowerBound.height;
            //int h = Screen.PrimaryScreen.WorkingArea.Height;
            //int height = SystemInformation.PrimaryMonitorSize.Height;
            //int width = SystemInformation.VirtualScreen.Width;
            //string h = _session.GetString("clientScreenHeight");
            //if (int.Parse(h) <= 768)
            //{
            //    pageSize = 20;
            //}
        }
        public List<BMViewModel> PopulateBM()
        {
            List<BMViewModel> bmvmList = new List<BMViewModel>();
            for (var i = 1; i <= 40; i++)
            {
                BMViewModel bmvm = new BMViewModel
                {
                    BM_Id = i,
                    BM_Creator_ID = i + 100,
                    BM_Approver_ID = i + 200,
                    BM_Account = "Account_" + i,
                    BM_Type = "Sample_Type_" + i,
                    BM_Budget = i + 100,
                    BM_Curr_Budget = i + 110,
                    BM_Last_Trans_Date = DateTime.Parse("1/12/2017", CultureInfo.GetCultureInfo("en-GB"))
                            .Add(DateTime.Now.TimeOfDay),
                    BM_Last_Budget_Approval = "Sample"
                };
                bmvmList.Add(bmvm);
            }
            return bmvmList;
        }
    }
}