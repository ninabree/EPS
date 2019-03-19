using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.Services.Excel_Services;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Search_Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ExpenseProcessingSystem.Controllers
{
    [ScreenFltr]
    public class HomeController : Controller
    {
        private readonly int pageSize = 30;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private HomeService _service;
        private SortService _sortService;
        private ExcelData _excelData;

        public HomeController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _service = new HomeService(_httpContextAccessor, _context, this.ModelState);
            _sortService = new SortService();
            _excelData = new ExcelData(_httpContextAccessor, _context);
        }

        private string GetUserID()
        {
            return _session.GetString("UserID");
        }

        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var role = _service.getUserRole(_session.GetString("UserID"));
            if (role == "admin")
            {
                return RedirectToAction("UM");
            }
            HomeIndexViewModel vm = new HomeIndexViewModel
            {

            };
            return View(vm);
        }
        public IActionResult Entry()
        {
            return View();
        }
        public IActionResult Close(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [ImportModelState]
        public IActionResult DM(DMViewModel vm, string sortOrder, string currentFilter, string tblName, string colName, string searchString, int? page, string partialName)
        {
            if (GetUserID() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewData["sortOrder"] = sortOrder;
            ViewData["currentFilter"] = searchString;
            ViewData["tblName"] = tblName;
            ViewData["colName"] = colName;
            ViewData["searchString"] = searchString;
            ViewData["page"] = page;
            ViewData["partialName"] = partialName ?? "DMPartial_Payee";

            DMFiltersViewModel filterVM = new DMFiltersViewModel();
            PayeeFiltersViewModel payeeFil = new PayeeFiltersViewModel();
            if (vm.DMFilters != null)
            {
                _session.SetString("PF_Name", vm.DMFilters.PF.PF_Name ?? "");
                _session.SetString("PF_TIN", vm.DMFilters.PF.PF_TIN.ToString() ?? "0");
                _session.SetString("PF_Address", vm.DMFilters.PF.PF_Address ?? "");
                _session.SetString("PF_Type", vm.DMFilters.PF.PF_Type ?? "");
                _session.SetString("PF_No", vm.DMFilters.PF.PF_No.ToString() ?? "0");
                _session.SetString("PF_Creator_Name", vm.DMFilters.PF.PF_Creator_Name ?? "");
                _session.SetString("PF_Approver_Name", vm.DMFilters.PF.PF_Approver_Name ?? "");
                _session.SetString("PF_Status", vm.DMFilters.PF.PF_Status ?? "");
            }
            else
            {
                _session.SetString("PF_Name", "");
                //_session.SetString("PF_Name", filters.PF.PF_Name);
                _session.SetString("PF_TIN", "0");
                _session.SetString("PF_Address", "");
                _session.SetString("PF_Type", "");
                _session.SetString("PF_No", "0");
                _session.SetString("PF_Creator_Name", "");
                _session.SetString("PF_Approver_Name", "");
                _session.SetString("PF_Status", "");
            }
            return View();
        }
        public IActionResult Report()
        {
            return View();
        }
        [ImportModelState]
        public IActionResult BM(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AccountSortParm"] = String.IsNullOrEmpty(sortOrder) ? "acc_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "type_desc" ? "type" : "type_desc";
            ViewData["BudgetSortParm"] = sortOrder == "budget_desc" ? "budget" : "budget_desc";
            ViewData["CurrBudgetSortParm"] = sortOrder == "curr_budget_desc" ? "curr_budget" : "curr_budget_desc";
            ViewData["LastTransDateSortParm"] = sortOrder == "last_trans_date_desc" ? "last_trans_date" : "last_trans_date_desc";
            ViewData["LastBudgetApprvlSortParm"] = sortOrder == "last_budget_apprvl_desc" ? "last_budget_apprvl" : "last_budget_apprvl_desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            //populate and sort
            var sortedVals = _sortService.SortData(PopulateBM(), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            return View(PaginatedList<BMViewModel>.CreateAsync(
                (sortedVals.list).Cast<BMViewModel>().AsQueryable().AsNoTracking(), page ?? 1, pageSize));
        }
        [ImportModelState]
        public IActionResult UM(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["UserSortParm"] = String.IsNullOrEmpty(sortOrder) ? "user_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";
            ViewData["DeptSortParm"] = sortOrder == "dept_desc" ? "dept" : "dept_desc";
            ViewData["RoleSortParm"] = sortOrder == "role_desc" ? "role" : "role_desc";
            ViewData["EmailSortParm"] = sortOrder == "email_desc" ? "email" : "email_desc";
            ViewData["CommentSortParm"] = sortOrder == "comment_desc" ? "comment" : "comment_desc";
            ViewData["InUseSortParm"] = sortOrder == "inuse_desc" ? "inuse" : "inuse_desc";
            ViewData["CreatrSortParm"] = sortOrder == "creatr_desc" ? "creatr" : "creatr_desc";
            ViewData["ApprvrSortParm"] = sortOrder == "apprv_desc" ? "apprv" : "apprv_desc";
            ViewData["CreatedDateSortParm"] = sortOrder == "creatr_date_desc" ? "creatr_date" : "creatr_date_desc";
            ViewData["LastUpdateDateSortParm"] = sortOrder == "last_updt_date_desc" ? "last_updt_date" : "last_updt_date_desc";
            ViewData["StatusSortParm"] = sortOrder == "stats_desc" ? "stats" : "stats";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var data = _service.populateUM();
            //populate and sort
            var sortedVals = _sortService.SortData(data.AccList, sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            UserManagementViewModel mod = new UserManagementViewModel
            {
                AccList = PaginatedList<UserViewModel>.CreateAsync(
                (sortedVals.list).Cast<UserViewModel>().AsQueryable().AsNoTracking(), page ?? 1, pageSize),
                DeptList = data.DeptList,
                NewAcc = data.NewAcc,
                RoleList = data.RoleList
            };
                
            //pagination
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
        //[* PAYEE *]
        [HttpPost]
        [ExportModelState]
        public IActionResult ApprovePayee(List<DMPayeeViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.approvePayee(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Payee" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult RejPayee(List<DMPayeeViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.rejPayee(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Payee" });
        }
        //__________________________________________________________________
        [HttpPost]
        [ExportModelState]
        public IActionResult AddPayee_Pending(NewPayeeListViewModel model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addPayee_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Payee" });
        }

        //--------------------------Not Yet In use----------------------------------------
        [HttpPost]
        [ExportModelState]
        public IActionResult AddPayee(NewPayeeListViewModel model)
        {
            var userId = GetUserID();
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
            var userId = GetUserID();
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
            var userId = GetUserID();
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

        //[* DEPT *]
        [HttpPost]
        [ExportModelState]
        public IActionResult AddDept(NewDeptListViewModel model)
        {
            var userId = GetUserID();
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
            string userId = GetUserID();
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
            var userId = GetUserID();
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

        //[* USER *]
        [HttpPost]
        [ExportModelState]
        public IActionResult AddEditUser(UserManagementViewModel model)
        {
            var userId = GetUserID();
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

        //[* EXCEL *]
        public IActionResult Excel()
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return File(_excelData.GetDeptExcelData(), "application/ms-excel", $"Department.xlsx");
        }
        //public void PreviewExcel()
        //{
        //    var userId = HttpContext.Session.GetString("UserID");
        //    if (userId == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    return File(_excelData.GetPayeeExcelData(), "application/ms-excel", $"Payee.xlsx");
        //}

        //[* MISC *]
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