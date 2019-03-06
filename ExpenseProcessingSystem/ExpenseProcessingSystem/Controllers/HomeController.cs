using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.Services.Excel_Services;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ExpenseProcessingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly int pageSize = 30;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private HomeService _service;
        private ExcelService _excelService;

        public HomeController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _service = new HomeService(_httpContextAccessor, _context, this.ModelState);
            _excelService = new ExcelService(_httpContextAccessor, _context);
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
        [ImportModelState]
        public IActionResult DM(string sortOrder, string currentFilter, string tblName, string colName, string searchString, int? page, string partialName)
        {
            ViewData["sortOrder"] = sortOrder;
            ViewData["currentFilter"] = searchString;
            ViewData["tblName"] = tblName;
            ViewData["colName"] = colName;
            ViewData["searchString"] = searchString;
            ViewData["page"] = page;
            ViewData["partialName"] = partialName ?? "DMPartial_Payee";
            return View();
        }
        public IActionResult Report()
        {
            return View();
        }
        [ImportModelState]
        public IActionResult BM(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
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
        [ImportModelState]
        public IActionResult UM()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<UserViewModel> vmList = new List<UserViewModel>();
            //get all accounts
            var accs = (from a in _context.Account
                        join d in _context.DMDept on a.Acc_DeptID equals d.Dept_ID
                        join b in _context.Account on a.Acc_Creator_ID equals b.Acc_UserID
                        let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                        select new { a.Acc_UserID, a.Acc_UserName, a.Acc_FName, a.Acc_LName, d.Dept_Name, a.Acc_DeptID, a.Acc_Email, a.Acc_Role,
                        a.Acc_Comment, a.Acc_InUse, CreatorName, a.Acc_Created_Date, a.Acc_Status}).ToList();
            //get account approver IDs and dates, not all accounts have this
            var apprv = (from a in _context.Account
                         join c in _context.Account on a.Acc_Approver_ID equals c.Acc_UserID
                         let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                         select new
                         {
                             a.Acc_UserID,
                             ApproverName,
                             a.Acc_Last_Updated
                         }).ToList();

            accs.ForEach(x => {
                var approver = apprv.Where(a => a.Acc_UserID == x.Acc_UserID).Select(a => a.ApproverName).FirstOrDefault();
                var apprvDate = apprv.Where(a => a.Acc_UserID == x.Acc_UserID).Select(a => a.Acc_Last_Updated).FirstOrDefault();
                UserViewModel vm = new UserViewModel
                {
                    Acc_UserID = x.Acc_UserID,
                    Acc_UserName = x.Acc_UserName,
                    Acc_FName = x.Acc_FName,
                    Acc_LName = x.Acc_LName,
                    Acc_Dept_ID = x.Acc_DeptID,
                    Acc_Dept_Name = x.Dept_Name,
                    Acc_Email = x.Acc_Email,
                    Acc_Role = x.Acc_Role,
                    Acc_InUse = x.Acc_InUse,
                    Acc_Comment = x.Acc_Comment,
                    Acc_Creator_Name = x.CreatorName,
                    Acc_Approver_Name = approver ?? "",
                    Acc_Created_Date = x.Acc_Created_Date,
                    Acc_Last_Updated = (apprvDate != null) ? apprvDate : new DateTime(),
                    Acc_Status = x.Acc_Status
                };
                vmList.Add(vm);
            });
            //set static values for Roles
            var list = new SelectList(new[]
            {
                new { ID = "admin", Name = "Admin" },
                new { ID = "maker", Name = "Maker" },
                new { ID = "verifier", Name = "Verifier" },
                new { ID = "approver", Name = "Approver" }
            },
            "ID", "Name", 1);

            ViewData["list"] = list;

            List<DMDeptViewModel> deptList = new List<DMDeptViewModel>();

            DMDeptViewModel optionLbl = new DMDeptViewModel
            {
                Dept_ID = 0,
                Dept_Name = "--Select Department--",
                Dept_Code = "0000"
            };
            deptList.Add(optionLbl);

            _context.DMDept.Where(x=>x.Dept_isDeleted == false).ToList().ForEach(x => {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_ID = x.Dept_ID,
                    Dept_Name = x.Dept_Name,
                    Dept_Code = x.Dept_Code
                };
                deptList.Add(vm);
            });

            UserManagementViewModel mod = new UserManagementViewModel
            {
                NewAcc = new AccountViewModel(),
                AccList = vmList,
                DeptList = deptList
            };
            return View(mod);
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
        //PAYEE
        [HttpPost]
        [ExportModelState]
        public IActionResult AddPayee(NewPayeeListViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addPayee(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Payee" });
        }

        [HttpPost]
        [ExportModelState]
        public IActionResult EditPayee(List<DMPayeeViewModel> model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editPayee(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Payee" });
        }

        [HttpPost]
        [ExportModelState]
        public IActionResult DeletePayee(List<DMPayeeViewModel> model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deletePayee(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Payee" });
        }
        //DEPT
        [HttpPost]
        [ExportModelState]
        public IActionResult AddDept(NewDeptListViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addDept(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }

        [HttpPost]
        [ExportModelState]
        public IActionResult EditDept(List<DMDeptViewModel> model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editDept(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }

        [HttpPost]
        [ExportModelState]
        public IActionResult DeleteDept(List<DMDeptViewModel> model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deleteDept(model, userId);
            }
            
            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        //USER
        [HttpPost]
        [ExportModelState]
        public IActionResult AddEditUser(UserManagementViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addUser(model, userId);
            }
            

            return RedirectToAction("UM", "Home");
        }
        //MISC
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
        public IActionResult Excel()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //List<ExcelViewModel> excelList = new List<ExcelViewModel>();
            ExcelViewModel excelVM = new ExcelViewModel();
            List<Row> rowList = new List<Row>();
            DummyData d = new DummyData(_httpContextAccessor, _context);
            string worksheetName = "Current Payee Information";
            //get column names in DB table
            List<string> colHeadrs = typeof(DMPayeeModel).GetProperties()
                        .Select(property => property.Name)
                        .ToList();

            //Populate Excel VM
            d.GetPayeeData().ForEach(x => {
                Row row = new Row();
                List<string> rowData = new List<string>
                {
                    x.Payee_ID.ToString(),
                    x.Payee_Name,
                    x.Payee_TIN,
                    x.Payee_Address,
                    x.Payee_Type,
                    x.Payee_No.ToString(),
                    x.Payee_Created_Date.ToString("MM/dd/yyyy"),
                    x.Payee_Creator_ID.ToString(),
                    x.Payee_Last_Updated.ToString("MM/dd/yyyy"),
                    x.Payee_Approver_ID.ToString(),
                    x.Payee_Status,
                    x.Payee_isDeleted.ToString()
                };
                row.DataList = rowData;
                rowList.Add(row);
            });

            excelVM.RowList = rowList;
            return File(_excelService.Excel(colHeadrs, excelVM, worksheetName), "application/ms-excel", $"Payee.xlsx");
        }
    }
}