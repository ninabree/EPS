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
using ExpenseProcessingSystem.ViewModels.NewRecord;
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
            HomeIndexViewModel vm = new HomeIndexViewModel();
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
                if (vm.DMFilters.PF != null)
                {
                    //Payee
                    _session.SetString("PF_Name", vm.DMFilters.PF.PF_Name ?? "");
                    _session.SetString("PF_TIN", vm.DMFilters.PF.PF_TIN.ToString() ?? "0");
                    _session.SetString("PF_Address", vm.DMFilters.PF.PF_Address ?? "");
                    _session.SetString("PF_Type", vm.DMFilters.PF.PF_Type ?? "");
                    _session.SetString("PF_No", vm.DMFilters.PF.PF_No.ToString() ?? "0");
                    _session.SetString("PF_Creator_Name", vm.DMFilters.PF.PF_Creator_Name ?? "");
                    _session.SetString("PF_Approver_Name", vm.DMFilters.PF.PF_Approver_Name ?? "");
                    _session.SetString("PF_Status", vm.DMFilters.PF.PF_Status ?? "");
                }
                else if (vm.DMFilters.DF != null)
                {
                    //Dept
                    _session.SetString("DF_Name", vm.DMFilters.DF.DF_Name ?? "");
                    _session.SetString("DF_Code", vm.DMFilters.DF.DF_Code ?? "");
                    _session.SetString("DF_Creator_Name", vm.DMFilters.DF.DF_Creator_Name ?? "");
                    _session.SetString("DF_Approver_Name", vm.DMFilters.DF.DF_Approver_Name ?? "");
                    _session.SetString("DF_Status", vm.DMFilters.DF.DF_Status ?? "");
                }
                else if (vm.DMFilters.CKF != null)
                {
                    //Check
                    _session.SetString("CKF_Input_Date", vm.DMFilters.CKF.CKF_Input_Date.ToString() ?? new DateTime().ToString());
                    _session.SetString("CKF_Series_From", vm.DMFilters.CKF.CKF_Series_From ?? "");
                    _session.SetString("CKF_Series_To", vm.DMFilters.CKF.CKF_Series_To ?? "");
                    _session.SetString("CKF_Name", vm.DMFilters.CKF.CKF_Name ?? "");
                    _session.SetString("CKF_Type", vm.DMFilters.CKF.CKF_Type ?? "");
                    _session.SetString("CKF_Creator_Name", vm.DMFilters.CKF.CKF_Creator_Name ?? "");
                    _session.SetString("CKF_Approver_Name", vm.DMFilters.CKF.CKF_Approver_Name ?? "");
                    _session.SetString("CKF_Status", vm.DMFilters.CKF.CKF_Status ?? "");
                }
                else if (vm.DMFilters.AF != null)
                {
                    //Account
                    _session.SetString("AF_Name", vm.DMFilters.AF.AF_Name ?? "");
                    _session.SetString("AF_Code", vm.DMFilters.AF.AF_Code ?? "");
                    _session.SetString("AF_No", vm.DMFilters.AF.AF_No.ToString() ?? "0");
                    _session.SetString("AF_Cust", vm.DMFilters.AF.AF_Cust ?? "");
                    _session.SetString("AF_Div", vm.DMFilters.AF.AF_Div ?? "");
                    _session.SetString("AF_Fund", vm.DMFilters.AF.AF_Fund ?? "");
                    _session.SetString("AF_Creator_Name", vm.DMFilters.AF.AF_Creator_Name ?? "");
                    _session.SetString("AF_Approver_Name", vm.DMFilters.AF.AF_Approver_Name ?? "");
                    _session.SetString("AF_Status", vm.DMFilters.AF.AF_Status ?? "");
                }
                else if (vm.DMFilters.VF != null)
                {
                    //Account
                    _session.SetString("VF_Name", vm.DMFilters.VF.VF_Name ?? "");
                    _session.SetString("VF_Rate", vm.DMFilters.VF.VF_Rate ?? "");
                    _session.SetString("VF_Creator_Name", vm.DMFilters.VF.VF_Creator_Name ?? "");
                    _session.SetString("VF_Approver_Name", vm.DMFilters.VF.VF_Approver_Name ?? "");
                    _session.SetString("VF_Status", vm.DMFilters.VF.VF_Status ?? "");
                }
                else if (vm.DMFilters.FF != null)
                {
                    //FBT
                    _session.SetString("FF_Name", vm.DMFilters.FF.FF_Name ?? "");
                    _session.SetString("FF_Account", vm.DMFilters.FF.FF_Account ?? "");
                    _session.SetString("FF_Formula", vm.DMFilters.FF.FF_Formula ?? "");
                    _session.SetString("FF_Tax_Rate", vm.DMFilters.FF.FF_Tax_Rate.ToString() ?? "0");
                    _session.SetString("FF_Creator_Name", vm.DMFilters.FF.FF_Creator_Name ?? "");
                    _session.SetString("FF_Approver_Name", vm.DMFilters.FF.FF_Approver_Name ?? "");
                    _session.SetString("FF_Status", vm.DMFilters.FF.FF_Status ?? "");
                }
                else if (vm.DMFilters.EF != null)
                {
                    //EWT
                    _session.SetString("EF_Nature", vm.DMFilters.EF.EF_Nature ?? "");
                    _session.SetString("EF_Tax_Rate", vm.DMFilters.EF.EF_Tax_Rate.ToString() ?? "0");
                    _session.SetString("EF_ATC", vm.DMFilters.EF.EF_ATC ?? "");
                    _session.SetString("EF_Tax_Rate_Desc", vm.DMFilters.EF.EF_Tax_Rate_Desc ?? "");
                    _session.SetString("EF_Creator_Name", vm.DMFilters.EF.EF_Creator_Name ?? "");
                    _session.SetString("EF_Approver_Name", vm.DMFilters.EF.EF_Approver_Name ?? "");
                    _session.SetString("EF_Status", vm.DMFilters.EF.EF_Status ?? "");
                }
                else if (vm.DMFilters.CF != null)
                {
                    //Currency
                    _session.SetString("CF_Name", vm.DMFilters.CF.CF_Name ?? "");
                    _session.SetString("CF_CCY_Code", vm.DMFilters.CF.CF_CCY_Code ?? "");
                    _session.SetString("CF_Creator_Name", vm.DMFilters.CF.CF_Creator_Name ?? "");
                    _session.SetString("CF_Approver_Name", vm.DMFilters.CF.CF_Approver_Name ?? "");
                    _session.SetString("CF_Status", vm.DMFilters.CF.CF_Status ?? "");
                }
            }
            else
            {
                //Payee
                _session.SetString("PF_Name", "");
                _session.SetString("PF_TIN", "0");
                _session.SetString("PF_Address", "");
                _session.SetString("PF_Type", "");
                _session.SetString("PF_No", "0");
                _session.SetString("PF_Creator_Name", "");
                _session.SetString("PF_Approver_Name", "");
                _session.SetString("PF_Status", "");
                //Dept
                _session.SetString("DF_Name", "");
                _session.SetString("DF_Code", "");
                _session.SetString("DF_Creator_Name", "");
                _session.SetString("DF_Approver_Name", "");
                _session.SetString("DF_Status", "");
                //Check
                _session.SetString("CKF_Input_Date", new DateTime().ToString());
                _session.SetString("CKF_Series_From", "");
                _session.SetString("CKF_Series_To", "");
                _session.SetString("CKF_Name", "");
                _session.SetString("CKF_Type", "");
                _session.SetString("CKF_Creator_Name", "");
                _session.SetString("CKF_Approver_Name", "");
                _session.SetString("CKF_Status", "");
                //Account
                _session.SetString("AF_Name", "");
                _session.SetString("AF_Code", "");
                _session.SetString("AF_No", "0");
                _session.SetString("AF_Cust", "");
                _session.SetString("AF_Div", "");
                _session.SetString("AF_Fund", "");
                _session.SetString("AF_Creator_Name", "");
                _session.SetString("AF_Approver_Name", "");
                _session.SetString("AF_Status", "");
                //VAT
                _session.SetString("VF_Name", "");
                _session.SetString("VF_Rate", "");
                _session.SetString("VF_Creator_Name", "");
                _session.SetString("VF_Approver_Name", "");
                _session.SetString("VF_Status", "");
                //FBT
                _session.SetString("FF_Name", "");
                _session.SetString("FF_Account", "");
                _session.SetString("FF_Formula", "");
                _session.SetString("FF_Tax_Rate", "0");
                _session.SetString("FF_Creator_Name", "");
                _session.SetString("FF_Approver_Name", "");
                _session.SetString("FF_Status", "");
                //EWT
                _session.SetString("EF_Nature", "");
                _session.SetString("EF_Tax_Rate", "0");
                _session.SetString("EF_ATC", "");
                _session.SetString("EF_Tax_Rate_Desc", "");
                _session.SetString("EF_Creator_Name", "");
                _session.SetString("EF_Approver_Name", "");
                _session.SetString("EF_Status",  "");
                //Currency
                _session.SetString("CF_Name", "");
                _session.SetString("CF_CCY_Code", "");
                _session.SetString("CF_Creator_Name", "");
                _session.SetString("CF_Approver_Name", "");
                _session.SetString("CF_Status", "");
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
        //[* ACCOUNT *]
        [HttpPost]
        [ExportModelState]
        public IActionResult SendEmail(ForgotPWViewModel model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.sendEmail(model);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Payee" });
        }

        //------------------------------DM-------------------------------
        //[* ADMIN *]
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
        //[* DEPARTMENT *]
        [HttpPost]
        [ExportModelState]
        public IActionResult ApproveDept(List<DMDeptViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.approveDept(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult RejDept(List<DMDeptViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.rejDept(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        //[* CHECK *]
        [HttpPost]
        [ExportModelState]
        public IActionResult ApproveCheck(List<DMCheckViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.approveCheck(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult RejCheck(List<DMCheckViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.rejCheck(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        //[* ACCOUNT *]
        [HttpPost]
        [ExportModelState]
        public IActionResult ApproveAccount(List<DMAccountViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.approveAccount(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult RejAccount(List<DMAccountViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.rejAccount(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        //[* VAT *]
        [HttpPost]
        [ExportModelState]
        public IActionResult ApproveVAT(List<DMVATViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.approveVAT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult RejVAT(List<DMVATViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.rejVAT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        //[* FBT *]
        [HttpPost]
        [ExportModelState]
        public IActionResult ApproveFBT(List<DMFBTViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.approveFBT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult RejFBT(List<DMFBTViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.rejFBT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        //[* EWT *]
        [HttpPost]
        [ExportModelState]
        public IActionResult ApproveEWT(List<DMEWTViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.approveEWT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_EWT" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult RejEWT(List<DMEWTViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.rejEWT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_EWT" });
        }
        //[* Currency *]
        [HttpPost]
        [ExportModelState]
        public IActionResult ApproveCurr(List<DMCurrencyViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.approveCurr(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult RejCurr(List<DMCurrencyViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.rejCurr(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
        }
        //--------------------------------PENDING--------------------------------
        // [PAYEE]
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
        [HttpPost]
        [ExportModelState]
        public IActionResult EditPayee_Pending(List<DMPayeeViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editPayee_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Payee" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult DeletePayee_Pending(List<DMPayeeViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deletePayee_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Payee" });
        }
        // [DEPARTMENT]
        [HttpPost]
        [ExportModelState]
        public IActionResult AddDept_Pending(NewDeptListViewModel model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addDept_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult EditDept_Pending(List<DMDeptViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editDept_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult DeleteDept_Pending(List<DMDeptViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deleteDept_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        // [CHECK]
        [HttpPost]
        [ExportModelState]
        public IActionResult AddCheck_Pending(NewCheckListViewModel model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addCheck_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult EditCheck_Pending(List<DMCheckViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editCheck_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult DeleteCheck_Pending(List<DMCheckViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deleteCheck_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        // [ACCOUNT]
        [HttpPost]
        [ExportModelState]
        public IActionResult AddAccount_Pending(NewAccountListViewModel model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addAccount_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult EditAccount_Pending(List<DMAccountViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editAccount_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult DeleteAccount_Pending(List<DMAccountViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deleteAccount_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        // [VAT]
        [HttpPost]
        [ExportModelState]
        public IActionResult AddVAT_Pending(NewVATListViewModel model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addVAT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult EditVAT_Pending(List<DMVATViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editVAT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult DeleteVAT_Pending(List<DMVATViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deleteVAT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        // [FBT]
        [HttpPost]
        [ExportModelState]
        public IActionResult AddFBT_Pending(NewFBTListViewModel model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addFBT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult EditFBT_Pending(List<DMFBTViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editFBT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult DeleteFBT_Pending(List<DMFBTViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deleteFBT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        // [EWT]
        [HttpPost]
        [ExportModelState]
        public IActionResult AddEWT_Pending(NewEWTListViewModel model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addEWT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_EWT" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult EditEWT_Pending(List<DMEWTViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editEWT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_EWT" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult DeleteEWT_Pending(List<DMEWTViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deleteEWT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_EWT" });
        }
        // [Curr]
        [HttpPost]
        [ExportModelState]
        public IActionResult AddCurr_Pending(NewCurrListViewModel model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addCurr_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult EditCurr_Pending(List<DMCurrencyViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.editCurr_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
        }
        [HttpPost]
        [ExportModelState]
        public IActionResult DeleteCurr_Pending(List<DMCurrencyViewModel> model)
        {
            var userId = GetUserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.deleteCurr_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
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