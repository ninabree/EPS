using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Resources;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.Services.Excel_Services;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.NewRecord;
using ExpenseProcessingSystem.ViewModels.Reports;
using ExpenseProcessingSystem.ViewModels.Search_Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExpenseProcessingSystem.ViewModels.Entry;
using System.Xml.Linq;
using ExpenseProcessingSystem.ViewModels.Search_Filters.Home;
using System.Text;
using BIR_Form_Filler.Functions;
using BIR_Form_Filler.Models;
using ExpenseProcessingSystem.Models.Check;
using Microsoft.Extensions.Logging;

namespace ExpenseProcessingSystem.Controllers
{
    [ScreenFltr]
    [ServiceFilter(typeof(HandleExceptionAttribute))]
    public class HomeController : Controller
    {
        private readonly int pageSize = 30;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private readonly GOExpressContext _GOContext;
        private readonly GWriteContext _GwriteContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private HomeService _service;
        private SortService _sortService;
        //to access resources
        private readonly IStringLocalizer<HomeController> _localizer;
        private IHostingEnvironment _env;

        public HomeController(IHttpContextAccessor httpContextAccessor, EPSDbContext context, GOExpressContext gocontext,GWriteContext gwritecontext,IHostingEnvironment hostingEnvironment, IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _GOContext = gocontext;
            _GwriteContext = gwritecontext;
            _service = new HomeService(_httpContextAccessor, _context, _GOContext, _GwriteContext, this.ModelState, hostingEnvironment);
            _sortService = new SortService();
            _env = hostingEnvironment;
        }
        public ReportCommonViewModel GetHeaderInfo()
        {

            ReportCommonViewModel headerVM = new ReportCommonViewModel {
                Header_Name = ReportResource.Branch_Title,
                Header_TIN = ReportResource.Branch_TIN,
                Header_Logo = ReportResource.Branch_Logo,
                Header_Address = ReportResource.Branch_Address,
            };
            return headerVM;
        }

        private string GetUserID()
        {
            return _session.GetString("UserID");
        }

        //Home Screen Block---------------------------------------------------------------------------------------
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Index(HomeIndexViewModel vm, string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            if (!String.IsNullOrEmpty(GlobalSystemValues.MESSAGE))
            {
                ViewData["MESSAGE"] = GlobalSystemValues.MESSAGE;
                GlobalSystemValues.MESSAGE = "";
            }

            int? pg = (page == null) ? 1 : int.Parse(page);

            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NotifStatSortParm"] = String.IsNullOrEmpty(sortOrder) ? "notif_type_status" : "";
            ViewData["NotifAppTypeSortParm"] = sortOrder == "notif_type_desc" ? "notif_type" : "notif_type_desc";
            ViewData["NotifMessageSortParm"] = sortOrder == "notif_message_desc" ? "notif_message" : "notif_message_desc";
            ViewData["NotifMakerSortParm"] = sortOrder == "notif_mkr_desc" ? "notif_mkr" : "notif_mkr_desc";
            ViewData["NotifDateSortParm"] = sortOrder == "notif_date_desc" ? "notif_date" : "notif_date_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            //FiltersViewModel filters = new FiltersViewModel();
            
            //FiltersViewModel filterVM = new FiltersViewModel();
            if (vm.Filters == null || vm.Filters.NotifFil == null)
            {
                vm.Filters = new FiltersViewModel {
                    NotifFil = new NotifFiltersViewModel {
                        NotifFil_Application_Maker_Name = "",
                        NotifFil_Application_Type_Name = "",
                        NotifFil_Message = "",
                        NotifFil_Status_Name = "",
                        Notif_Date = new DateTime()
                    }
                };
            }
            else{
                vm.Filters = new FiltersViewModel
                {
                    NotifFil = new NotifFiltersViewModel
                    {
                        NotifFil_Application_Maker_Name = vm.Filters.NotifFil.NotifFil_Application_Maker_Name ?? "",
                        NotifFil_Application_Type_Name = vm.Filters.NotifFil.NotifFil_Application_Type_Name ?? "",
                        NotifFil_Message = vm.Filters.NotifFil.NotifFil_Message ?? "",
                        NotifFil_Status_Name = vm.Filters.NotifFil.NotifFil_Status_Name ?? "",
                        Notif_Date = vm.Filters.NotifFil.Notif_Date != new DateTime() ? vm.Filters.NotifFil.Notif_Date : new DateTime()
                    }
                };
            }
            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateNotif(vm.Filters, int.Parse(GetUserID())), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            HomeIndexViewModel VM = new HomeIndexViewModel()
            {
                Filters = vm.Filters,
                NotifList = PaginatedList<HomeNotifViewModel>.CreateAsync(
                        (sortedVals.list).Cast<HomeNotifViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Pending(HomeIndexViewModel vm, string sortOrder, FiltersViewModel currentFilter, int? page)
        {
            var role = _service.getUserRole(GetUserID());
            if (role == "admin")
            {
                return RedirectToAction("UM");
            }

            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PendAppStatSortParm"] = String.IsNullOrEmpty(sortOrder) ? "pend_status" : "";
            ViewData["PendAppIDSortParm"] = sortOrder == "pend_type_desc" ? "pend_type" : "pend_type_desc";
            ViewData["PendAppAmtSortParm"] = sortOrder == "pend_amt_desc" ? "pend_amt" : "pend_amt_desc";
            ViewData["PendAppPaySortParm"] = sortOrder == "pend_payee_desc" ? "pend_payee" : "pend_payee_desc";
            ViewData["PendAppMkrSortParm"] = sortOrder == "pend_maker_desc" ? "pend_maker" : "pend_maker_desc";
            //ViewData["PendAppVfrSortParm"] = sortOrder == "pend_ver_desc" ? "pend_ver" : "pend_ver_desc";
            ViewData["PendDateSortParm"] = sortOrder == "pend_date_created_desc" ? "pend_date_created" : "pend_date_created_desc";
            ViewData["PendUpdateSortParm"] = sortOrder == "pend_last_updte_desc" ? "pend_last_updte" : "pend_last_updte_desc";
            

            FiltersViewModel tempFil = new FiltersViewModel();
            if (vm.Filters == null || vm.Filters.GenPendFil == null)
            {
                tempFil = new FiltersViewModel
                {
                    GenPendFil = new PendingGenFiltersViewModel
                    {
                        Pending_Amount = "",
                        Pending_Created_Date = new DateTime(),
                        Pending_Maker = "",
                        Pending_Status = "",
                        Pending_Updated_Date = new DateTime(),
                        Pending_Payee = "",
                        Pending_Type = ""
                        //Pending_Type_Select = ""
                    }
                };
            }
            else
            {
                tempFil.GenPendFil = new PendingGenFiltersViewModel
                {
                    Pending_Amount = vm.Filters.GenPendFil.Pending_Amount ?? "",
                    Pending_Created_Date = vm.Filters.GenPendFil.Pending_Created_Date != new DateTime() ? vm.Filters.GenPendFil.Pending_Created_Date : new DateTime(),
                    Pending_Maker = vm.Filters.GenPendFil.Pending_Maker ?? "",
                    Pending_Status = vm.Filters.GenPendFil.Pending_Status ?? "",
                    Pending_Updated_Date = vm.Filters.GenPendFil.Pending_Updated_Date != new DateTime() ? vm.Filters.GenPendFil.Pending_Updated_Date : new DateTime(),
                    Pending_Payee = vm.Filters.GenPendFil.Pending_Payee ?? "",
                    Pending_Type = vm.Filters.GenPendFil.Pending_Type ?? ""

                };
            }

            ViewBag.currentFilter = tempFil;
            //populate and sort
            var sortedVals = _sortService.SortData(_service.getPending(int.Parse(GetUserID()), tempFil), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;
            HomeIndexViewModel VM = new HomeIndexViewModel()
            {
                Filters = tempFil,
                GeneralPendingList = PaginatedList<ApplicationsViewModel>.CreateAsync(
                        (sortedVals.list).Cast<ApplicationsViewModel>().AsQueryable().AsNoTracking(), page ?? 1, pageSize)
            };
            ViewBag.printStat = _service.getStatus(GlobalSystemValues.STATUS_FOR_PRINTING);
            ViewBag.loi = HomeReportConstantValue.LOI;
            return View(VM);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult History(HomeIndexViewModel vm, string sortOrder, string currentFilter, string searchString, int? page)
        {
            var role = _service.getUserRole(GetUserID());
            if (role == "admin")
            {
                return RedirectToAction("UM");
            }

            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["HistStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "hist_status" : "";
            ViewData["HistMakerSortParm"] = sortOrder == "hist_maker_desc" ? "hist_maker" : "hist_maker_desc";
            ViewData["HistApproverSortParm"] = sortOrder == "hist_approver_desc" ? "hist_approver" : "hist_approver_desc";
            ViewData["HistDateCreatedSortParm"] = sortOrder == "hist_date_created_desc" ? "hist_date_created" : "hist_date_created_desc";
            ViewData["HistLastUpdatedSortParm"] = sortOrder == "hist_last_updte_desc" ? "hist_last_updte" : "hist_last_updte_desc";
            ViewData["HistVoucherSortParm"] = sortOrder == "hist_voucher_desc" ? "hist_voucher" : "hist_voucher_desc";

            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            FiltersViewModel tempFil = new FiltersViewModel();
            if (vm.Filters == null || vm.Filters.HistoryFil == null)
            {
                tempFil = new FiltersViewModel {
                    HistoryFil = new HistoryFiltersViewModel
                    {
                        Hist_Approver = "",
                        Hist_Created_Date = new DateTime(),
                        Hist_Maker = "",
                        Hist_Status = "",
                        Hist_Updated_Date = new DateTime(),
                        Hist_Voucher_No = "",
                        Hist_Voucher_Type = "",
                        Hist_Voucher_Year = ""
                    }
                };
            }else
            {
                tempFil.HistoryFil = new HistoryFiltersViewModel
                {
                    Hist_Approver = vm.Filters.HistoryFil.Hist_Approver ?? "",
                    Hist_Created_Date = vm.Filters.HistoryFil.Hist_Created_Date != new DateTime() ? vm.Filters.HistoryFil.Hist_Created_Date : new DateTime(),
                    Hist_Maker = vm.Filters.HistoryFil.Hist_Maker ?? "",
                    Hist_Status = vm.Filters.HistoryFil.Hist_Status ?? "",
                    Hist_Updated_Date = vm.Filters.HistoryFil.Hist_Updated_Date != new DateTime() ? vm.Filters.HistoryFil.Hist_Updated_Date : new DateTime(),
                    Hist_Voucher_No = vm.Filters.HistoryFil.Hist_Voucher_No ??  "",
                    Hist_Voucher_Type = vm.Filters.HistoryFil.Hist_Voucher_Type ?? "",
                    Hist_Voucher_Year = vm.Filters.HistoryFil.Hist_Voucher_Year ?? ""
                };
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.getHistory(int.Parse(GetUserID()), tempFil), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;
            HomeIndexViewModel VM = new HomeIndexViewModel()
            {
                Filters = tempFil,
                HistoryList = PaginatedList<AppHistoryViewModel>.CreateAsync(
                        (sortedVals.list).Cast<AppHistoryViewModel>().AsQueryable().AsNoTracking(), page ?? 1, pageSize)
            };
            return View(VM);
        }
        //Home Screen Block End-----------------------------------------------------------------------------------

        public IActionResult Entry()
        {
            return View();
        }
        //-----------Closing Screen-----------------
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Close(string username, string password, string command = "load")
        {
            ClosingViewModel model = new ClosingViewModel();
            bool closeFail = false;
            switch (command)
            {
                case "openBook":
                    if (_service.ClosingCheckStatus())
                    {
                        model = _service.ClosingOpenDailyBook();
                    }
                    else
                    {
                        model.messages.Add("Can't open a new book, status still open.");
                    }
                    break;
                case "CloseRBU":
                    closeFail = _service.ClosingCheckStatus(GlobalSystemValues.BRANCH_RBU);
                    if (!closeFail) {
                        if(!_service.closeTransaction(GlobalSystemValues.BRANCH_RBU, username, password,"close"))
                        {
                            closeFail = true;
                        }
                    }
                    break;
                case "reOpenRBU":
                    if (!_service.closeTransaction(GlobalSystemValues.BRANCH_RBU, username, password, "reopen"))
                    {
                        closeFail = true;
                    }
                    break;
                case "CloseFCDU":
                    closeFail = _service.ClosingCheckStatus(GlobalSystemValues.BRANCH_FCDU);
                    if (!closeFail)
                    {
                        if (!_service.closeTransaction(GlobalSystemValues.BRANCH_FCDU, username, password, "close"))
                        {
                            closeFail = true;
                        }
                    }
                    break;
                case "reOpenFCDU":
                    if (!_service.closeTransaction(GlobalSystemValues.BRANCH_FCDU, username, password, "reopen"))
                    {
                        closeFail = true;
                    }
                    break;
            }

            model = _service.ClosingGetRecords();

            if (closeFail)
                model.messages.Add("Can't close book there are still ongoing transactions!");

            return View(model);
        }

        //----------End Closing Screen--------------
        [OnlineUserCheck]
        [ImportModelState]
        [NonAdminRoleCheck]
        public IActionResult DM(DMViewModel vm, string sortOrder, string currentFilter, string tblName, string colName, string searchString, int? page, string partialName)
        {
            ViewData["sortOrder"] = sortOrder;
            ViewData["currentFilter"] = searchString;
            ViewData["tblName"] = tblName;
            ViewData["colName"] = colName;
            ViewData["searchString"] = searchString;
            ViewData["page"] = page;
            ViewData["partialName"] = partialName ?? "DMPartial_Vendor";

            DMFiltersViewModel filterVM = new DMFiltersViewModel();
            VendorFiltersViewModel payeeFil = new VendorFiltersViewModel();
            if (vm.DMFilters != null)
            {
                if (vm.DMFilters.PF != null)
                {
                    //Vendor
                    _session.SetString("PF_Name", vm.DMFilters.PF.PF_Name ?? "");
                    _session.SetString("PF_TIN", vm.DMFilters.PF.PF_TIN ?? "");
                    _session.SetString("PF_Address", vm.DMFilters.PF.PF_Address ?? "");
                    _session.SetString("PF_Creator_Name", vm.DMFilters.PF.PF_Creator_Name ?? "");
                    _session.SetString("PF_Approver_Name", vm.DMFilters.PF.PF_Approver_Name ?? "");
                    _session.SetString("PF_Status", vm.DMFilters.PF.PF_Status ?? "");
                }
                else if (vm.DMFilters.DF != null)
                {
                    //Dept
                    _session.SetString("DF_Name", vm.DMFilters.DF.DF_Name ?? "");
                    _session.SetString("DF_Code", vm.DMFilters.DF.DF_Code ?? "");
                    _session.SetString("DF_Budget_Unit", vm.DMFilters.DF.DF_Budget_Unit ?? "");
                    _session.SetString("DF_Creator_Name", vm.DMFilters.DF.DF_Creator_Name ?? "");
                    _session.SetString("DF_Approver_Name", vm.DMFilters.DF.DF_Approver_Name ?? "");
                    _session.SetString("DF_Status", vm.DMFilters.DF.DF_Status ?? "");
                }
                else if (vm.DMFilters.CKF != null)
                {
                    //Check
                    _session.SetString("CKF_Series_From", vm.DMFilters.CKF.CKF_Series_From ?? "");
                    _session.SetString("CKF_Series_To", vm.DMFilters.CKF.CKF_Series_To ?? "");
                    _session.SetString("CKF_Name", vm.DMFilters.CKF.CKF_Name ?? "");
                    _session.SetString("CKF_Bank_Info", vm.DMFilters.CKF.CKF_Bank_Info ?? "");
                    _session.SetString("CKF_Creator_Name", vm.DMFilters.CKF.CKF_Creator_Name ?? "");
                    _session.SetString("CKF_Approver_Name", vm.DMFilters.CKF.CKF_Approver_Name ?? "");
                    _session.SetString("CKF_Status", vm.DMFilters.CKF.CKF_Status ?? "");
                }
                else if (vm.DMFilters.AF != null)
                {
                    //Account
                    _session.SetString("AF_Name", vm.DMFilters.AF.AF_Name ?? "");
                    _session.SetString("AF_Code", vm.DMFilters.AF.AF_Code ?? "");
                    _session.SetString("AF_Budget_Code", vm.DMFilters.AF.AF_Budget_Code ?? "");
                    _session.SetString("AF_No", vm.DMFilters.AF.AF_No ?? "");
                    _session.SetString("AF_Cust", vm.DMFilters.AF.AF_Cust ?? "");
                    _session.SetString("AF_Div", vm.DMFilters.AF.AF_Div ?? "");
                    _session.SetString("AF_Group_Name", vm.DMFilters.AF.AF_Group_Name ?? "");
                    _session.SetString("AF_Currency_Name", vm.DMFilters.AF.AF_Currency_Name ?? "");
                    _session.SetString("AF_FBT_Name", vm.DMFilters.AF.AF_FBT_Name ?? "");
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
                    _session.SetString("FF_Formula", vm.DMFilters.FF.FF_Formula ?? "");
                    _session.SetString("FF_Tax_Rate", vm.DMFilters.FF.FF_Tax_Rate ?? "");
                    _session.SetString("FF_Creator_Name", vm.DMFilters.FF.FF_Creator_Name ?? "");
                    _session.SetString("FF_Approver_Name", vm.DMFilters.FF.FF_Approver_Name ?? "");
                    _session.SetString("FF_Status", vm.DMFilters.FF.FF_Status ?? "");
                }
                else if (vm.DMFilters.TF != null)
                {
                    //TR
                    _session.SetString("TR_WT_Title", vm.DMFilters.TF.TR_WT_Title ?? "");
                    _session.SetString("TR_Nature", vm.DMFilters.TF.TR_Nature ?? "");
                    _session.SetString("TR_Nature_Income_Payment", vm.DMFilters.TF.TR_Nature_Income_Payment ?? "");
                    _session.SetString("TR_Tax_Rate", vm.DMFilters.TF.TR_Tax_Rate ?? "");
                    _session.SetString("TR_ATC", vm.DMFilters.TF.TR_ATC ?? "");
                    _session.SetString("TR_Creator_Name", vm.DMFilters.TF.TR_Creator_Name ?? "");
                    _session.SetString("TR_Approver_Name", vm.DMFilters.TF.TR_Approver_Name ?? "");
                    _session.SetString("TR_Status", vm.DMFilters.TF.TR_Status ?? "");
                }
                else if (vm.DMFilters.CF != null)
                {
                    //Currency
                    _session.SetString("CF_Name", vm.DMFilters.CF.CF_Name ?? "");
                    _session.SetString("CF_CCY_ABBR", vm.DMFilters.CF.CF_CCY_ABBR ?? "");
                    _session.SetString("CF_Creator_Name", vm.DMFilters.CF.CF_Creator_Name ?? "");
                    _session.SetString("CF_Approver_Name", vm.DMFilters.CF.CF_Approver_Name ?? "");
                    _session.SetString("CF_Status", vm.DMFilters.CF.CF_Status ?? "");
                }
                else if (vm.DMFilters.EMF != null)
                {
                    //Employee
                    _session.SetString("EMF_Name", vm.DMFilters.EMF.EMF_Name ?? "");
                    _session.SetString("EMF_Acc_No", vm.DMFilters.EMF.EMF_Acc_No ?? "");
                    _session.SetString("EMF_Name", vm.DMFilters.EMF.EMF_Name ?? "");
                    _session.SetString("EMF_Category_Name", vm.DMFilters.EMF.EMF_Category_Name ?? "");
                    _session.SetString("EMF_FBT_Name", vm.DMFilters.EMF.EMF_FBT_Name ?? "");
                    _session.SetString("EMF_Creator_Name", vm.DMFilters.EMF.EMF_Creator_Name ?? "");
                    _session.SetString("EMF_Approver_Name", vm.DMFilters.EMF.EMF_Approver_Name ?? "");
                    _session.SetString("EMF_Status", vm.DMFilters.EMF.EMF_Status ?? "");
                }
                else if (vm.DMFilters.CUF != null)
                {
                    //Customer
                    _session.SetString("CUF_Name", vm.DMFilters.CUF.CUF_Name ?? "");
                    _session.SetString("CUF_Abbr", vm.DMFilters.CUF.CUF_Abbr ?? "");
                    _session.SetString("CUF_No", vm.DMFilters.CUF.CUF_No ?? "");
                    _session.SetString("CUF_Creator_Name", vm.DMFilters.CUF.CUF_Creator_Name ?? "");
                    _session.SetString("CUF_Approver_Name", vm.DMFilters.CUF.CUF_Approver_Name ?? "");
                    _session.SetString("CUF_Status", vm.DMFilters.CUF.CUF_Status ?? "");
                }
                else if (vm.DMFilters.BF != null)
                {
                    //Non Cash Category
                    _session.SetString("BF_Name", vm.DMFilters.BF.BF_Name ?? "");
                    _session.SetString("BF_TIN", vm.DMFilters.BF.BF_TIN ?? "");
                    _session.SetString("BF_Position", vm.DMFilters.BF.BF_Position ?? "");
                    _session.SetString("BF_Signatures", vm.DMFilters.BF.BF_Signatures ?? "");
                    _session.SetString("BF_Creator_Name", vm.DMFilters.BF.BF_Creator_Name ?? "");
                    _session.SetString("BF_Approver_Name", vm.DMFilters.BF.BF_Approver_Name ?? "");
                    _session.SetString("BF_Status", vm.DMFilters.BF.BF_Status ?? "");
                }
                else if (vm.DMFilters.AGF != null)
                {
                    //Account Group
                    _session.SetString("AGF_Name", vm.DMFilters.AGF.AGF_Name ?? "");
                    _session.SetString("AGF_Code", vm.DMFilters.AGF.AGF_Code ?? "");
                    _session.SetString("AGF_Creator_Name", vm.DMFilters.AGF.AGF_Creator_Name ?? "");
                    _session.SetString("AGF_Approver_Name", vm.DMFilters.AGF.AGF_Approver_Name ?? "");
                    _session.SetString("AGF_Status_Name", vm.DMFilters.AGF.AGF_Status_Name ?? "");
                }
            }
            return View();
        }

        //--------------------------[* REPORT *]----------------------------------------
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Report(int reportType)
        {
            //Get list of report types from the constant data file:HomeReportTypesModel.cs
            //uses in Dropdownlist(Report Type)
            IEnumerable<HomeReportTypesModel> ReportTypes = ConstantData.HomeReportConstantValue.GetReportTypeData();
            //Pass list of report type and initial value for report sub type to ViewModel of Report
            var reportViewModel = new HomeReportViewModel
            {
                ReportTypesList = ReportTypes,
                //Initial value of sub type dropdownlist to avoid the nullexception.
                ReportSubTypesList = new HomeReportSubTypesModel[]
                {
                    new HomeReportSubTypesModel
                    {
                    Id = 0,
                    SubTypeName = null,
                    ParentTypeId = 0
                    }
                },
                MonthList = HomeReportConstantValue.GetMonthList(),
                FileFormatList = HomeReportConstantValue.GetFileFormatList(),
                YearList = HomeReportConstantValue.GetYearList(),
                YearSemList = HomeReportConstantValue.GetYearList(),
                SemesterList = HomeReportConstantValue.GetSemesterList(),
                PeriodOptionList = HomeReportConstantValue.GetPeriodOptionList(),
                PeriodFrom = Convert.ToDateTime(HomeReportConstantValue.DateToday),
                PeriodTo = Convert.ToDateTime(HomeReportConstantValue.DateToday),
                TaxRateList = _service.PopulateTaxRaxListIncludeHist(),
                VoucherNoList =  _service.PopulateVoucherNoDDV(),
                VoucherNoListPrepaidAmort = _service.PopulateVoucherNoCV(),
                SignatoryList = _service.PopulateSignatoryList()
            };

            //Return ViewModel
            //ViewBag.reportType = reportViewModel.ReportTypesList.Where(x => x.Id == reportType);
            ViewBag.reportType = reportType;
            return View(reportViewModel);
        }

        //Populate the Report sub-type list to dropdownlist depends on the selected Report Type
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [AcceptVerbs("GET")]
        public JsonResult GetReportSubType(int ReportTypeID)
        {
            List<HomeReportSubTypeAccModel> subtypes = new List<HomeReportSubTypeAccModel>();

            if (ReportTypeID == HomeReportConstantValue.ESAMS)
            {
                var accounts = _service.getAccountList();
                XElement xelem = XElement.Load("wwwroot/xml/GlobalAccounts.xml");
                for(int i = 1; i <=5; i++)
                {
                    var acc = accounts.Where(x => x.Account_MasterID == int.Parse(xelem.Element("D_SS" + i).Value)).FirstOrDefault();
                    if(acc != null)
                    {
                        subtypes.Add(new HomeReportSubTypeAccModel
                        {
                            Id = acc.Account_ID.ToString(),
                            SubTypeName = acc.Account_Name
                        });
                    }
                }
                return Json(subtypes);
            }

            if (ReportTypeID == HomeReportConstantValue.AccSummaryReport)
            {
                var accounts = _service.getAccountList();


                subtypes.Add(new HomeReportSubTypeAccModel
                {
                    Id = "0",
                    SubTypeName = "All"
                });
                foreach (var i in accounts)
                {
                    subtypes.Add(new HomeReportSubTypeAccModel
                    {
                        Id = i.Account_ID.ToString(),
                        SubTypeName = i.Account_Name
                    });
                }
                return Json(subtypes);
            }

            if (ReportTypeID == HomeReportConstantValue.OutstandingAdvances)
            {
                var accounts = _service.getAccountListIncHist();
                
                XElement xelem = XElement.Load("wwwroot/xml/GlobalAccounts.xml");
                for(int i = 1; i <= 5; i++)
                {
                    var acc = accounts.Where(x => x.Account_MasterID == int.Parse(xelem.Element("D_SS" + i).Value)
                                && x.Account_isActive == true && x.Account_isDeleted == false).FirstOrDefault();
                    if (acc != null)
                    {
                        subtypes.Add(new HomeReportSubTypeAccModel
                        {
                            Id = acc.Account_ID.ToString(),
                            SubTypeName = acc.Account_No + " - " + acc.Account_Name
                        });
                    }
                }
                return Json(subtypes);
            }

            if(ReportTypeID == HomeReportConstantValue.WTS)
            {
                var taxList = _service.getAllTaxRateList();
                foreach (var i in taxList)
                {
                    subtypes.Add(new HomeReportSubTypeAccModel
                    {
                        Id = i.TR_MasterID.ToString(),
                        SubTypeName = i.TR_WT_Title
                    });
                }
                return Json(subtypes);
            }
            
            if (ReportTypeID != 0)
            {
                return Json(ConstantData.HomeReportConstantValue.GetReportSubTypeData()
                    .Where(m => m.ParentTypeId == ReportTypeID).ToList());
            }
            return null;
        }

        //[ExportModelState]
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult GenerateFilePreview(HomeReportViewModel model)
        {
            string layoutName = "";
            string fileName = "";
            string returnPeriod = "";
            //string dateNow = DateTime.Now.ToString("MM-dd-yyyy_hhmmsstt"); // ORIGINAL
            string dateNow = DateTime.Now.ToString("MM-dd-yyyy_hhmmss");
            string pdfFooterFormat = HomeReportConstantValue.PdfFooter2;
            var signatory = model.SignatoryID != 0 ? _service.GetSignatoryInfo(model.SignatoryID) : new DMBCSViewModel();

            XElement xelem = XElement.Load("wwwroot/xml/ReportHeader.xml");

            ReportCommonViewModel repComVM = new ReportCommonViewModel
            {
                Header_Logo = xelem.Element("LOGO").Value,
                Header_Name = xelem.Element("NAME").Value,
                Header_TIN = xelem.Element("TIN").Value,
                Header_Address = xelem.Element("ADDRESS").Value,
                Signatory_Name = signatory.BCS_Name,
                Signatory_Position = signatory.BCS_Position
            };

            //Model for data retrieve from Database
            HomeReportDataFilterViewModel data = null;

            //Assign variables and Data to corresponding Report Type
            switch (model.ReportType)
            {
                //For Alphalist of Payees Subject to Withholding Tax (Monthly)
                case HomeReportConstantValue.APSWT_M:

                    fileName = "AlphalistOfPayeesSubjectToWithholdingTax_Monthly_" + dateNow;
                    layoutName = HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;

                    model.MonthName = HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputAPSWT_M = _service.GetAPSWT_MData(model.Month, model.Year),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM
                    };
                    break;
                //For Alphalist of Suppliers by top 10000 corporation
                case HomeReportConstantValue.AST1000:
                    fileName = "AlphalistOfSuppliersByTop10000Corporation_" + dateNow;
                    layoutName = HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;

                    if (!string.IsNullOrEmpty(model.TaxRateArray))
                    {
                        if (model.TaxRateArray.Contains(','))
                        {
                            model.TaxRateList = model.TaxRateArray.Split(',').Select(float.Parse).ToList();
                        }
                        else
                        {
                            model.TaxRateList = new List<float>
                            {
                                float.Parse(model.TaxRateArray)
                            };
                        }
                    }
                    else
                    {
                        model.TaxRateList = new List<float> { 0f };
                    }

                    if(model.PeriodOption == 1)
                    {
                        model.MonthName = HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName + " " + model.Year;
                        model.MonthNameTo = HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.MonthTo).Single().MonthName + " " + model.YearTo;

                    }
                    else
                    {
                        model.MonthName = model.PeriodFrom.ToShortDateString();
                        model.MonthNameTo = model.PeriodTo.ToShortDateString();
                    }
                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputAST1000 = _service.GetAST1000_Data(model),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM
                    };

                    int cnt = 0;
                    foreach (var i in data.HomeReportOutputAST1000)
                    {
                        i.SeqNo = cnt;
                        cnt += 1;
                    }

                    break;

                //For ESAMS Report
                case HomeReportConstantValue.ESAMS:
                    fileName = "ESAMSReport_" + dateNow;
                    layoutName = HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
                    DateTime DateFromLayout = model.PeriodFrom;
                    DateTime DateToLayout = model.PeriodTo;
                    string format = "yyyy-M";
                    if (model.PeriodOption == 1)
                    {
                        DateFromLayout = DateTime.ParseExact(model.Year + "-" + model.Month, format, CultureInfo.InvariantCulture);
                        DateToLayout = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
                        model.PeriodFrom = DateFromLayout;
                        model.PeriodTo = DateToLayout;
                    }
                    var accountno = _service.getAccount(model.ReportSubType);
                    var currency = _service.getCurrencyByMasterID(accountno.Account_Currency_MasterID);
                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputESAMS = _service.GetESAMSData(model),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM,
                        ReportAccountNo = accountno.Account_No,
                        ReportCurrency = currency.Curr_CCY_ABBR,
                        DateFrom = DateFromLayout,
                        DateTo = DateToLayout
                    };
                    
                    break;
                //For Actual Budget Report
                case HomeReportConstantValue.ActualBudgetReport:
                    fileName = "ActualBudgetReport_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
  
                    model.MonthName = HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputActualBudget = _service.GetActualReportData(model.Month, model.Year),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM
                    };
                    break;

                //For BIR Withholding Tax Reporting CSV
                case HomeReportConstantValue.BIRWTCSV:
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;

                    fileName = xelem.Element("WHAgentTIN").Value + xelem.Element("WHAgentBranchCode").Value
                                    + returnPeriod.Replace("/", "") + xelem.Element("FormType").Value.ToLower() + ".csv";

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputBIRWTCSV = _service.GetBIRWTCSVData(model),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM,
                        ReturnPeriod_CSV = returnPeriod
                    };
                    int seq = 1;
                    foreach (var i in data.HomeReportOutputBIRWTCSV)
                    {
                        i.SeqNo = seq;
                        seq = seq + 1;
                    }

                    break;
                //For Transaction List
                case HomeReportConstantValue.TransListReport:
                    fileName = "TransactionList_" + dateNow;
                    layoutName = HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputTransactionList = _service.GetTransactionData(model),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM
                    };
                    break;

                //For Account Summary
                case HomeReportConstantValue.AccSummaryReport:
                    fileName = "AccountSummary_" + dateNow;
                    layoutName = HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputAccountSummary = _service.GetAccountSummaryData(model),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM
                    };

                    break;

                //For Alphalist of Payees Subject to Withholding Tax Summary
                case HomeReportConstantValue.WTS:
                    fileName = "WithholdingTaxSummary_" + dateNow;
                    layoutName = HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
                    data = new HomeReportDataFilterViewModel();
                    //Get the necessary data from Database
                    switch (model.PeriodOption)
                    {
                        case 1:
                            data = new HomeReportDataFilterViewModel
                            {
                                HomeReportOutputWTS = _service.GetWithHoldingSummaryData(model),
                                HomeReportFilter = model,
                                ReportCommonVM = repComVM
                            };
                            model.ReportFrom = HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName
                                                + " " + model.Year;
                            model.ReportTo = HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.MonthTo).Single().MonthName
                                                + " " + model.YearTo;
                            break;
                        case 3:
                            data = new HomeReportDataFilterViewModel
                            {
                                HomeReportOutputWTS = _service.GetWithHoldingSummaryData(model),
                                HomeReportFilter = model,
                                ReportCommonVM = repComVM
                            };
                            model.ReportFrom = model.PeriodFrom.ToShortDateString();
                            model.ReportTo = model.PeriodTo.ToShortDateString();
                            break;
                    }
                    break;

                //For Oustanding Advances
                case HomeReportConstantValue.OutstandingAdvances:
                    fileName = "OutstandingAdvances_" + dateNow;
                    layoutName = HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputAccountSummary = _service.GetAccountSummaryData(model).Where(x => x.Trans_DebitCredit == "D"),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM
                    };

                    break;
                //For Prepaid Amortization Schedule Report
                case HomeReportConstantValue.PrepaidAmortReport:
                    fileName = "PrepaidAmortSched_" + dateNow;
                    layoutName = HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        ReportAmort = _service.GetPrepaidAmortSchedule(model),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM
                    };

                    break;
                //LOI
                case HomeReportConstantValue.LOI:
                    fileName = "LOI_" + dateNow;
                    layoutName = HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        ReportLOI = _service.GetLOIData(model),
                        HomeReportFilter = model,
                    };
                    break;
            }

            if (model.FileFormat == HomeReportConstantValue.EXCELID)
            {
                ExcelGenerateService excelGenerate = new ExcelGenerateService();
                fileName = fileName + ".xlsx";
                //TEMP
                if(model.ReportType == HomeReportConstantValue.LOI)
                {
                    foreach (var accs in data.ReportLOI.Rep_LOIEntryIDList)
                    {
                        _service.updateExpenseStatus(accs, GlobalSystemValues.STATUS_POSTED, int.Parse(GetUserID()));
                    }
                }
                //Return Excel file
                return File(excelGenerate.ExcelGenerateData(layoutName, fileName, data), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else if (model.FileFormat == HomeReportConstantValue.PDFID)
            {
                //TEMP
                if (model.ReportType == HomeReportConstantValue.LOI)
                {
                    foreach (var accs in data.ReportLOI.Rep_LOIEntryIDList)
                    {
                        _service.updateExpenseStatus(accs, GlobalSystemValues.STATUS_POSTED, int.Parse(GetUserID()));
                    }
                }
                //Return PDF file
                return OutputPDF(ConstantData.HomeReportConstantValue.ReportPdfPrevLayoutPath, layoutName, data, fileName, pdfFooterFormat);
            }
            else if (model.FileFormat == HomeReportConstantValue.PreviewID)
            {
                string pdfLayoutFilePath = HomeReportConstantValue.ReportPdfPrevLayoutPath + layoutName;

                //Return Preview
                return View(pdfLayoutFilePath, data);
            }
            else if (model.FileFormat == HomeReportConstantValue.CSVID)
            {
                ExcelGenerateService excelGenerate = new ExcelGenerateService();

                byte[] csvData = excelGenerate.CSVOutput(data);
                Console.WriteLine(Encoding.Default.GetString(csvData));

                //Return CSV file
                return File(csvData, "text/csv", fileName);
            }

            //Temporary return
            return View("Report");
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult OutputPDF(string layoutPath, string layoutName, HomeReportDataFilterViewModel data, string fileName, string footerFormat)
        {
            string pdfLayoutFilePath = layoutPath + layoutName;
            fileName = fileName + ".pdf";

            return new ViewAsPdf(pdfLayoutFilePath, data)
            {
                FileName = fileName,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = footerFormat,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Generate2307File(int _vendor, int _ewt, int _tax, double _amount, DateTime date, string approver,int expID)
        {
            string path = "";
            XElement xelem = XElement.Load("wwwroot/xml/ReportHeader.xml");

            var Header_Name = xelem.Element("NAME").Value;
            var Header_TIN = xelem.Element("TIN").Value;
            var Header_Address = xelem.Element("ADDRESS").Value;

            try
            {
                BIRExcelFiller exlFiller = new BIRExcelFiller();
                FirstPartBIRForm fp = new FirstPartBIRForm();

                var vendor = _service.getVendor(_vendor);
                var ewt = _service.GetEWT(_ewt);
                float vat = _service.getVat(_tax);

                var payItem = new PaymentInfo();
                double amount;

                if (vat > 0)
                    amount = _amount / (1 + vat);
                else
                    amount = _amount;
                if (new List<int> { 1, 4, 7, 10 }.Contains(date.Month))
                {
                    payItem.M1Quarter = amount;
                }
                else if (new List<int> { 2, 5, 8, 11 }.Contains(date.Month))
                {
                    payItem.M2Quarter = amount;
                }
                else if (new List<int> { 3, 6, 9, 12 }.Contains(date.Month))
                {
                    payItem.M3Quarter = amount;
                }

                //payitem
                payItem.Atc = ewt.TR_ATC;
                payItem.Payments = ewt.TR_Nature_Income_Payment;
                payItem.TaxWithheld = amount * ewt.TR_Tax_Rate;

                fp.IncomePay.Add(payItem);

                //payor
                fp.OrRegAddress = Header_Address;
                fp.OrTin = Header_TIN;
                fp.PayorName = Header_Name;

                //payee
                fp.EeRegAddress = vendor.Vendor_Address;
                fp.EeTin = vendor.Vendor_TIN;
                fp.PayeeName = vendor.Vendor_Name;
                fp.From_Date = DateTime.Now;
                fp.To_Date = DateTime.Now;

                //signatory
                fp.PayorSig = new Signatories {
                    Name = approver
                };
                fp.PayeeSig = new Signatories();

                var entry = _service.getExpense(expID);

                fp.VoucherNo = _service.getVoucherNo(entry.expenseType,entry.expenseDate,int.Parse(entry.expenseId),false);

                path = exlFiller.FillBirForm(fp);
            }
            catch (Exception e) {
                //redirect to Error Screen
                Console.WriteLine(e.Message);
            };

            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", path.Substring(26));
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult GenerateCheckFile(int ExpenseID)
        {
            ChequeData cd = new ChequeData();
            string filepath = "";

            var expModel = _service.getExpense(ExpenseID);

            cd.Date = DateTime.Now;
            cd.Payee = _service.getVendor(expModel.vendor).Vendor_Name;

            foreach(var item in expModel.EntryCV)
            {
                cd.Amount += item.credCash;
            }

            cd.Signatory1 = _service.getUserFullName(expModel.approver_id);
            cd.Signatory2 = "";
            cd.Voucher = _service.getVoucherNo(1, expModel.expenseDate, int.Parse(expModel.expenseId));

            Services.Check.GenerateCheck gc = new Services.Check.GenerateCheck();

            filepath = gc.Generate(cd);

            return File(filepath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filepath.Substring(26));
        }
        //public IActionResult MizuhoLogo2Image()
        //{
        //    var dir = _env.WebRootPath;
        //    var file = System.IO.Path.Combine(dir, "\\images\\mizuho-logo-2.png");
        //    return base.File(file, "image/png");
        //}

        //-----------------------[* BUDGET MONITORING *]-------------------------------------------

        [OnlineUserCheck]
        [ImportModelState]
        [NonAdminRoleCheck]
        public IActionResult BM(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AccountMappingSortParm"] = sortOrder == "acc_mapping_desc" ? "acc_mapping" : "acc_mapping_desc";
            ViewData["AccountNameSortParm"] = sortOrder == "acc_name_desc" ? "acc_name" : "acc_name_desc";
            ViewData["GBaseBudgetCodeSortParm"] = sortOrder == "gbase_budget_code_desc" ? "gbase_budget_code" : "gbase_budget_code_desc";
            ViewData["AccountNumberSortParm"] = sortOrder == "acc_num_desc" ? "acc_num" : "acc_num_desc";
            ViewData["BudgetSortParm"] = sortOrder == "budget_desc" ? "budget" : "budget_desc";
            ViewData["BudgetNewSortParm"] = sortOrder == "budget_new_desc" ? "budget_new" : "budget_new_desc";
            ViewData["GWriteStatusParm"] = sortOrder == "gwrite_status_desc" ? "gwrite_status" : "gwrite_status_desc";
            ViewData["DateRegisteredSortParm"] = sortOrder == "date_registered_desc" ? "date_registered" : "date_registered_desc";

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
            var sortedVals = _sortService.SortData(_service.PopulateBM(), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            return View(PaginatedList<BMViewModel>.CreateAsync(
                (sortedVals.list).Cast<BMViewModel>().AsQueryable().AsNoTracking(), page ?? 1, pageSize));
        }

        [OnlineUserCheck]
        [ImportModelState]
        [NonAdminRoleCheck]
        public IActionResult BM_PrintList()
        {
            return new ViewAsPdf("BM_PrintPDF", _service.PopulateBM())
            {
                FileName = "Budget_List_" + DateTime.Now + ".pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = HomeReportConstantValue.PdfFooter2,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        [OnlineUserCheck]
        [ImportModelState]
        [NonAdminRoleCheck]
        public IActionResult UM(string sortOrder, string currentFilter, string searchString, int? page)
        {
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

        //Expense Entry Block---------------------------------------------------------------------------------------

        //Expense Entry Check Voucher Block=========================================================================
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ImportModelState]
        [ServiceFilter(typeof(MakerCheck))]
        public IActionResult Entry_CV(int entryID = 0)
        {
            var userId = GetUserID();
            EntryCVViewModelList viewModel = new EntryCVViewModelList();

            if (entryID > 0)
            {
                viewModel = _service.getExpense(entryID);
                viewModel = PopulateEntry((EntryCVViewModelList)viewModel);
                viewModel.systemValues.payee_type_sel = new SelectList(GlobalSystemValues.PAYEETYPE_SELECT_CV, "Value", "Text", GlobalSystemValues.PAYEETYPE_SELECT_CV.First());

                foreach (var i in viewModel.EntryCV)
                {
                    viewModel.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));
                    i.screenCode = "CV";

                    var vend = _service.getVendor(i.dtl_Ewt_Payor_Name_ID);
                    if (vend != null)
                    {
                        i.vendTRList = _service.getVendorTaxList(vend.Vendor_MasterID).ToList();
                        i.vendVATList = _service.getVendorVatList(vend.Vendor_MasterID).ToList();
                        if (i.vendTRList == null || i.vendTRList.Count() == 0)
                        {
                            i.vendTRList.Add(new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 });
                        }
                        if (i.vendVATList == null || i.vendVATList.Count() == 0)
                        {
                            i.vendVATList.Add(new DMVATModel { VAT_ID = 0, VAT_Rate = 0 });
                        }
                    }
                    else
                    {
                        i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                        i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                    }
                }
            }
            else
            {
                viewModel = PopulateEntry((EntryCVViewModelList)viewModel);
                //viewModel.vendor = 2;
                viewModel.EntryCV.Add(new EntryCVViewModel());
            }

            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));
            viewModel.yenCurrID = ccyYEN.Curr_ID;
            viewModel.yenCurrMasterID = ccyYEN.Curr_MasterID;
            viewModel.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            return View(viewModel);
        }

        public dynamic PopulateEntry(dynamic viewModel)
        {
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            viewModel.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            viewModel.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            viewModel.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];

            int firstId = int.Parse(listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR].First().Value);

            viewModel.systemValues.ewt = _service.getVendorTaxRate(firstId);
            viewModel.systemValues.vat = _service.getVendorVat(firstId);
            viewModel.systemValues.acc = _service.getAccDetailsEntry();
            //TEMP for CV
            viewModel.systemValues.payee_type_sel = new SelectList(GlobalSystemValues.PAYEETYPE_SELECT_CV, "Value", "Text", GlobalSystemValues.PAYEETYPE_SELECT_CV.First());
            viewModel.systemValues.employees = listOfSysVals[GlobalSystemValues.SELECT_LIST_REGEMPLOYEE]; 
            viewModel.systemValues.employeesAll = listOfSysVals[GlobalSystemValues.SELECT_LIST_ALLEMPLOYEE];
            //for NC
            if (viewModel.GetType() != typeof(EntryNCViewModelList) && viewModel.expenseYear == null)
            {
                viewModel.expenseYear = DateTime.Today.Year.ToString();
                viewModel.expenseDate = DateTime.Today;
            }

            return viewModel;
        }

        public dynamic PopulateEntry(dynamic viewModel, int screenCode)
        {
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            viewModel.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            viewModel.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            viewModel.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];

            int firstId = int.Parse(listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR].First().Value);

            viewModel.systemValues.ewt = _service.getVendorTaxRate(firstId);
            viewModel.systemValues.vat = _service.getVendorVat(firstId);
            viewModel.systemValues.employees = listOfSysVals[GlobalSystemValues.SELECT_LIST_REGEMPLOYEE];

            if (screenCode == GlobalSystemValues.TYPE_SS)
            {
                DMAccountModel acc = new DMAccountModel();
                List<accDetails> acclist = new List<accDetails>();
                XElement xelem = XElement.Load("wwwroot/xml/GlobalAccounts.xml");
                for(int i = 1; i <= 5; i++)
                {
                    acc = _service.getAccountByMasterID(int.Parse(xelem.Element("D_SS" + i).Value));
                    acclist.Add(new accDetails
                    {
                        accId = acc.Account_ID,
                        accName = acc.Account_Name,
                        accCode = acc.Account_Code
                    });
                }
                viewModel.systemValues.acc = acclist;
            }
            else
            {
                viewModel.systemValues.acc = _service.getAccDetailsEntry();
            }

            //for NC

            if (viewModel.GetType() != typeof(EntryNCViewModelList))
            {
                viewModel.expenseYear = DateTime.Today.Year.ToString();
                viewModel.expenseDate = DateTime.Today;
            }

            return viewModel;
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ExportModelState]
        public IActionResult AddNewCV(EntryCVViewModelList EntryCVViewModelList)
        {
            var userId = GetUserID();
            if (!ModelState.IsValid)
            {
                return View("Entry_CV", PopulateEntry((EntryCVViewModelList)EntryCVViewModelList));
            }

            EntryCVViewModelList cvList = new EntryCVViewModelList();

            int id = 0;
            if (EntryCVViewModelList.entryID == 0)
            {
                id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_CV);
            }
            else
            {
                List<int> EditableStatus = new List<int>{
                    GlobalSystemValues.STATUS_PENDING,
                    GlobalSystemValues.STATUS_REJECTED
                };
                var currentStat = _service.GetCurrentEntryStatus(EntryCVViewModelList.entryID);
                if (EditableStatus.Contains(currentStat))
                {
                    if (_service.deleteExpenseEntry(EntryCVViewModelList.entryID))
                    {
                        id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_CV);
                        var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == id).Expense_Creator_ID;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_CV, GlobalSystemValues.STATUS_EDIT, makerId);
                        //----------------------------- NOTIF----------------------------------
                    }
                }
                else
                {
                    id = EntryCVViewModelList.entryID;
                    GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE1;
                }
            }

            ModelState.Clear();

            return RedirectToAction("View_CV", "Home", new { entryID = id });
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult VerAppModCV(int entryID, string command)
        {
            var userId = GetUserID();
            var intUser = int.Parse(userId);
            string viewLink = "Entry_CV";
            EntryCVViewModelList cvList = _service.getExpense(entryID);

            switch (command)
            {
                case "Modify":
                    viewLink = "Entry_CV";
                    break;
                case "approver":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED, int.Parse(GetUserID())))
                    {
                        _service.postCV(entryID);
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_CV";
                    break;
                case "verifier":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_VERIFIED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_CV";
                    break;
                case "Reject":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REJECTED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_CV, GlobalSystemValues.STATUS_REJECTED, cvList.maker);
                        //----------------------------- NOTIF----------------------------------
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_CV";
                    break;
                case "Delete":
                    int expStatus = _service.GetCurrentEntryStatus(entryID);
                    if (expStatus == GlobalSystemValues.STATUS_PENDING || expStatus == GlobalSystemValues.STATUS_REJECTED)
                    {
                        if (_service.deleteExpenseEntry(entryID))
                        {
                            //----------------------------- NOTIF----------------------------------
                            _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_CV, GlobalSystemValues.STATUS_DELETE, 0);
                            //----------------------------- NOTIF----------------------------------
                        }
                        else
                        {
                            ViewBag.Success = 0;
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE2;
                        ViewBag.Success = 0;
                    }
                    
                    viewLink = "View_CV";
                    break;
                default:
                    break;
            }

            //Entry was Edited, Deleted by user before clicking the approve/verify button by user, 
            //return error message then redirect to Home Index screen.
            if (GlobalSystemValues.MESSAGE == GlobalSystemValues.MESSAGE3)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.Clear();

            return RedirectToAction(viewLink, "Home", new { entryID = entryID });
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult View_CV(int entryID)
        {
            var userId = GetUserID();

            if (entryID == 0 && TempData["entryIDAddtoView"] != null)
            {
                entryID = (int)TempData["entryIDAddtoView"];
                TempData.Keep();
            }
            else
            {
                TempData.Remove("entryIDAddtoView");
            }

            EntryCVViewModelList cvList;
            cvList = _service.getExpense(entryID);
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            cvList.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            cvList.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            cvList.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
            cvList.systemValues.ewt =_service.getVendorTaxRate(cvList.vendor);
            cvList.systemValues.vat = _service.getVendorVat(cvList.vendor);
            cvList.systemValues.acc = _service.getAccDetailsEntry();

            List<cvBirForm> birForms = new List<cvBirForm>();
            foreach (var item in cvList.EntryCV)
            {
                if (birForms.Any(x => x.ewt == item.ewt))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ewt);
                    birForms[index].amount += item.debitGross;
                }
                else
                {
                    cvBirForm temp = new cvBirForm {
                        amount = item.debitGross,
                        ewt = item.ewt,
                        vat = item.vat,
                        vendor = cvList.vendor,
                        approver = cvList.approver,
                        date = cvList.createdDate
                    };

                    birForms.Add(temp);
                }
            }
            cvList.birForms.AddRange(birForms);

            if (!String.IsNullOrEmpty(GlobalSystemValues.MESSAGE))
            {
                ViewData["MESSAGE"] = GlobalSystemValues.MESSAGE;
                GlobalSystemValues.MESSAGE = "";
            }

            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));
            cvList.yenCurrID = ccyYEN.Curr_ID;
            cvList.yenCurrMasterID = ccyYEN.Curr_MasterID;
            cvList.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            return View("Entry_CV_ReadOnly", cvList);
        }

        //Expense Entry Check Voucher Block End=========================================================================
        //------------------------------------------------------------------
        //[* Entry Direct Deposit *]
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ImportModelState]
        [ServiceFilter(typeof(MakerCheck))]
        public IActionResult Entry_DDV(int entryID)
        {
            EntryDDVViewModelList viewModel;
            var userId = GetUserID();
            if(entryID > 0)
            {
                viewModel = _service.getExpenseDDV(entryID);
                viewModel = PopulateEntry((EntryDDVViewModelList)viewModel);

                foreach (var i in viewModel.EntryDDV)
                {
                    viewModel.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));

                    var vend = _service.getVendor(i.ewt_Payor_Name_ID);
                    if (vend != null)
                    {
                        i.vendTRList = _service.getVendorTaxList(vend.Vendor_MasterID).ToList();
                        i.vendVATList = _service.getVendorVatList(vend.Vendor_MasterID).ToList();
                        if (i.vendTRList == null || i.vendTRList.Count() == 0)
                        {
                            i.vendTRList.Add(new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 });
                        }
                        if (i.vendVATList == null || i.vendVATList.Count() == 0)
                        {
                            i.vendVATList.Add(new DMVATModel { VAT_ID = 0, VAT_Rate = 0 });
                        }
                    }
                    else
                    {
                        i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                        i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                    }
                }
            }
            else
            {
                viewModel = new EntryDDVViewModelList();
                viewModel = PopulateEntry((EntryDDVViewModelList)viewModel);
                viewModel.EntryDDV.Add(new EntryDDVViewModel {
                    interDetails = new DDVInterEntityViewModel {
                        interPartList = new List<ExpenseEntryInterEntityParticularViewModel>{
                            new ExpenseEntryInterEntityParticularViewModel{
                                ExpenseEntryInterEntityAccs = new List<ExpenseEntryInterEntityAccsViewModel>{
                                    new ExpenseEntryInterEntityAccsViewModel(),
                                    new ExpenseEntryInterEntityAccsViewModel(),
                                    new ExpenseEntryInterEntityAccsViewModel()
                                }
                            },
                            new ExpenseEntryInterEntityParticularViewModel{
                                ExpenseEntryInterEntityAccs = new List<ExpenseEntryInterEntityAccsViewModel>{
                                    new ExpenseEntryInterEntityAccsViewModel(),
                                    new ExpenseEntryInterEntityAccsViewModel()
                                }
                            },
                            new ExpenseEntryInterEntityParticularViewModel{
                                ExpenseEntryInterEntityAccs = new List<ExpenseEntryInterEntityAccsViewModel>{
                                    new ExpenseEntryInterEntityAccsViewModel(),
                                    new ExpenseEntryInterEntityAccsViewModel()
                                }
                            }
                        }
                    },
                    vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } },
                    vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } },
                    ccy = _service.getCurrencyByMasterID(_service.getAccount(viewModel.systemValues.acc[0].accId).Account_Currency_MasterID).Curr_ID
                });
            }
            viewModel.payee_type = GlobalSystemValues.PAYEETYPE_REGEMP;
            viewModel.systemValues.ewt = new SelectList("0", "0");
            viewModel.systemValues.vat = new SelectList("0", "0");
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));
            viewModel.yenCurrID = ccyYEN.Curr_ID;
            viewModel.yenCurrMasterID = ccyYEN.Curr_MasterID;
            viewModel.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            return View(viewModel);
        }
        [ExportModelState]
        public IActionResult AddNewDDV(EntryDDVViewModelList EntryDDVViewModelList)
        {
            var userId = GetUserID();
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Entry_DDV", EntryDDVViewModelList);
            }

            EntryDDVViewModelList ddvList = new EntryDDVViewModelList();
            int id = _service.addExpense_DDV(EntryDDVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_DDV);
            ModelState.Clear();
            //return RedirectToAction("View_DDV", "Home", new { entryID = id });
            TempData["entryIDAddtoView"] = id;

            return RedirectToAction("View_DDV", "Home");
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult View_DDV(int entryID)
        {
            var userId = GetUserID();

            if (entryID == 0 && TempData["entryIDAddtoView"] != null)
            {
                entryID = (int)TempData["entryIDAddtoView"];
                TempData.Keep();
            }
            else
            {
                TempData.Remove("entryIDAddtoView");
            }

            EntryDDVViewModelList ddvList = _service.getExpenseDDV(entryID);
            ddvList = PopulateEntry((EntryDDVViewModelList)ddvList);

            List<cvBirForm> birForms = new List<cvBirForm>();
            foreach (var item in ddvList.EntryDDV)
            {
                if (birForms.Any(x => x.ewt == item.ewt && x.vendor == item.ewt_Payor_Name_ID))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ewt);
                    birForms[index].amount += item.debitGross;
                }
                else
                {
                    cvBirForm temp = new cvBirForm
                    {
                        amount = item.debitGross,
                        ewt = item.ewt,
                        vat = item.vat,
                        vendor = item.ewt_Payor_Name_ID,
                        approver = ddvList.approver,
                        date = ddvList.expenseDate
                    };

                    birForms.Add(temp);
                }
            }
            ddvList.birForms.AddRange(birForms);

            if (!String.IsNullOrEmpty(GlobalSystemValues.MESSAGE))
            {
                ViewData["MESSAGE"] = GlobalSystemValues.MESSAGE;
                GlobalSystemValues.MESSAGE = "";
            }

            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));
            ddvList.yenCurrID = ccyYEN.Curr_ID;
            ddvList.yenCurrMasterID = ccyYEN.Curr_MasterID;
            ddvList.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            return View("Entry_DDV_ReadOnly", ddvList);
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult VerAppModDDV(int entryID, string command)
        {
            var userId = GetUserID();
            var intUser = int.Parse(userId);
            string viewLink = "Entry_DDV";
            EntryDDVViewModelList ddvList;

            switch (command)
            {
                case "Modify":
                    return RedirectToAction("Entry_DDV", "Home", new { entryID = entryID });
                case "approver":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED, intUser))
                    {
                        _service.postDDV(entryID, "P", int.Parse(GetUserID()));
                        ViewBag.Success = 1;
                        _service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_FOR_PRINTING, int.Parse(GetUserID()));

                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_DDV_ReadOnly";
                    break;
                case "verifier":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_VERIFIED, intUser))
                    {
                        ViewBag.Success = 1;
                        var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_DDV, GlobalSystemValues.STATUS_VERIFIED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------                    
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_DDV_ReadOnly";
                    break;
                case "Reject":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REJECTED, intUser))
                    {
                        ViewBag.Success = 1;
                        var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_DDV, GlobalSystemValues.STATUS_REJECTED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_DDV_ReadOnly";
                    break;
                case "Delete":
                    int expStatus = _service.GetCurrentEntryStatus(entryID);

                    if (expStatus == GlobalSystemValues.STATUS_PENDING || expStatus == GlobalSystemValues.STATUS_REJECTED)
                    {
                        if (_service.deleteExpenseEntry(entryID, GlobalSystemValues.TYPE_DDV))
                        {
                            ViewBag.Success = 1;
                            //----------------------------- NOTIF----------------------------------
                            _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_DDV, GlobalSystemValues.STATUS_DELETE, 0);
                            //----------------------------- NOTIF----------------------------------
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.Success = 0;
                        }
                        viewLink = "Entry_DDV_ReadOnly";
                        break;
                    }
                    else
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE2;
                        ViewBag.Success = 0;
                    }
                    break;
                case "Reversal":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REVERSED, int.Parse(GetUserID())))
                    {
                        _service.postDDV(entryID, "R", int.Parse(GetUserID()));
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_DDV_ReadOnly";
                    break;
                default:
                    break;
            }

            //Entry was Edited, Deleted by user before clicking the approve/verify button by user, 
            //return error message then redirect to Home Index screen.
            if (GlobalSystemValues.MESSAGE == GlobalSystemValues.MESSAGE3)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.Clear();

            ddvList = _service.getExpenseDDV(entryID);

            ddvList = PopulateEntry((EntryDDVViewModelList)ddvList);
            ddvList.payee_type = GlobalSystemValues.PAYEETYPE_REGEMP;
            ddvList.systemValues.vat = _service.getAllVat();
            ddvList.systemValues.ewt = _service.getAllTaxRate();

            foreach (var acc in ddvList.EntryDDV)
            {
                ddvList.systemValues.acc.AddRange(_service.getAccDetailsEntry(acc.account));
            }

            return View(viewLink, ddvList);
        }

        //-----------------[* Entry Petty Cash *]-----------------------------
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ImportModelState]
        [ServiceFilter(typeof(MakerCheck))]
        public IActionResult Entry_PCV(int entryID)
        {
            var userId = GetUserID();

            EntryCVViewModelList viewModel = new EntryCVViewModelList();
            if (entryID > 0) //If Modify
            {
                viewModel = _service.getExpense(entryID);
                viewModel = PopulateEntry((EntryCVViewModelList)viewModel);

                foreach (var i in viewModel.EntryCV)
                {
                    viewModel.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));
                    i.screenCode = "PCV";

                    var vend = _service.getVendor(i.dtl_Ewt_Payor_Name_ID);
                    if (vend != null)
                    {
                        i.vendTRList = _service.getVendorTaxList(vend.Vendor_MasterID).ToList();
                        i.vendVATList = _service.getVendorVatList(vend.Vendor_MasterID).ToList();
                        if (i.vendTRList == null || i.vendTRList.Count() == 0)
                        {
                            i.vendTRList.Add(new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 });
                        }
                        if (i.vendVATList == null || i.vendVATList.Count() == 0)
                        {
                            i.vendVATList.Add(new DMVATModel { VAT_ID = 0, VAT_Rate = 0 });
                        }
                    }
                    else
                    {
                        i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                        i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                    }
                }
                viewModel.systemValues.ewt = new SelectList("0", "0");
                viewModel.systemValues.vat = new SelectList("0", "0");
            }
            else //If new entry
            {
                viewModel = PopulateEntry((EntryCVViewModelList)viewModel);

                viewModel.EntryCV.Add(new EntryCVViewModel
                {
                    screenCode = "PCV",
                    vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } },
                    vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } }
                });
                //select values for reg employee payee
                viewModel.payee_type = GlobalSystemValues.PAYEETYPE_EMP_ALL;
                viewModel.systemValues.ewt = new SelectList("0", "0");
                viewModel.systemValues.vat = new SelectList("0", "0");
            }

            return View(viewModel);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ExportModelState]
        public IActionResult AddNewPCV(EntryCVViewModelList EntryCVViewModelList)
        {
            var userId = GetUserID();

            if (!ModelState.IsValid)
            {
                EntryCVViewModelList = PopulateEntry((EntryCVViewModelList)EntryCVViewModelList);
                foreach (var i in EntryCVViewModelList.EntryCV)
                {
                    EntryCVViewModelList.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));
                    i.screenCode = "PCV";

                    var vend = _service.getVendor(i.dtl_Ewt_Payor_Name_ID);
                    if (vend != null)
                    {
                        i.vendTRList = _service.getVendorTaxList(vend.Vendor_MasterID).ToList();
                        i.vendVATList = _service.getVendorVatList(vend.Vendor_MasterID).ToList();
                        if (i.vendTRList == null || i.vendTRList.Count() == 0)
                        {
                            i.vendTRList.Add(new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 });
                        }
                        if (i.vendVATList == null || i.vendVATList.Count() == 0)
                        {
                            i.vendVATList.Add(new DMVATModel { VAT_ID = 0, VAT_Rate = 0 });
                        }
                    }
                    else
                    {
                        i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                        i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                    }
                }
                EntryCVViewModelList.systemValues.ewt = new SelectList("0", "0");
                EntryCVViewModelList.systemValues.vat = new SelectList("0", "0");
                return View("Entry_PCV", EntryCVViewModelList);
            }
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            var phpID = _service.getAccountByMasterID(int.Parse(xelem.Element("CURRENCY_PHP").Value)).Account_ID;
            foreach (var i in EntryCVViewModelList.EntryCV)
            {
                i.ccy = phpID;
            }
            EntryCVViewModelList pcvList = new EntryCVViewModelList();

            int id = 0;
            if (EntryCVViewModelList.entryID == 0)
            {
                id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_PC);
                //----------------------------- NOTIF----------------------------------
                _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_PC, GlobalSystemValues.STATUS_NEW, 0);
                //----------------------------- NOTIF----------------------------------
            }
            else
            {
                List<int> EditableStatus = new List<int>{
                    GlobalSystemValues.STATUS_PENDING,
                    GlobalSystemValues.STATUS_REJECTED
                };
                var currentStat = _service.GetCurrentEntryStatus(EntryCVViewModelList.entryID);
                if (EditableStatus.Contains(currentStat))
                {
                    if (_service.deleteExpenseEntry(EntryCVViewModelList.entryID))
                    {
                        id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_PC);
                        var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == id).Expense_Creator_ID;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_PC, GlobalSystemValues.STATUS_EDIT, makerId);
                        //----------------------------- NOTIF----------------------------------
                    }
                }
                else
                {
                    id = EntryCVViewModelList.entryID;
                    GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE1;
                }
            }

            ModelState.Clear();

            //if (id > -1)
            //{
            //    pcvList = _service.getExpense(id);
            //    List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            //    pcvList.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            //    pcvList.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            //    pcvList.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            //    pcvList.systemValues.acc = _service.getAccDetailsEntry();
            //    ViewBag.Status = pcvList.status;
            //}

            //return View("Entry_PCV_ReadOnly", pcvList);

            TempData["entryIDAddtoView"] = id;

            return RedirectToAction("View_PCV", "Home");
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [NonAdminRoleCheck]
        public IActionResult VerAppModPCV(int entryID, string command)
        {
            var userId = GetUserID();
            var intUser = int.Parse(userId);
            string viewLink = "Entry_PCV";
            EntryCVViewModelList pcvList;
            var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);

            switch (command)
            {
                case "Modify":
                    viewLink = "Entry_PCV";
                    break;
                case "approver":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED, int.Parse(GetUserID())))
                    {
                        _service.postCV(entryID, "P", int.Parse(GetUserID()));
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_PC, GlobalSystemValues.STATUS_APPROVED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------                    
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_PCV";
                    break;
                case "verifier":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_VERIFIED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_PC, GlobalSystemValues.STATUS_VERIFIED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------                    
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_PCV";
                    break;
                case "Delete":
                    int expStatus = _service.GetCurrentEntryStatus(entryID);
                    if (expStatus == GlobalSystemValues.STATUS_PENDING || expStatus == GlobalSystemValues.STATUS_REJECTED)
                    {
                        if (_service.deleteExpenseEntry(entryID))
                        {
                            ViewBag.Success = 1;
                            //----------------------------- NOTIF----------------------------------
                            _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_PC, GlobalSystemValues.STATUS_DELETE, 0);
                            //----------------------------- NOTIF----------------------------------
                        }
                        else
                        {
                            ViewBag.Success = 0;
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE2;
                        ViewBag.Success = 0;
                    }

                    viewLink = "View_PCV";
                    break;
                case "Reject":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REJECTED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_PC, GlobalSystemValues.STATUS_REJECTED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------                    
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_PCV";
                    break;
                case "Reversal":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REVERSED, int.Parse(GetUserID())))
                    {
                        _service.postCV(entryID, "R", int.Parse(GetUserID()));
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_PC, GlobalSystemValues.STATUS_REVERSED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_PCV";
                    break;
                default:
                    break;
            }

            //Entry was Edited, Deleted by user before clicking the approve/verify button by user, 
            //return error message then redirect to Home Index screen.
            if (GlobalSystemValues.MESSAGE == GlobalSystemValues.MESSAGE3)
            {
                return RedirectToAction("Index", "Home");
            }

            pcvList = _service.getExpense(entryID);
            pcvList = PopulateEntry((EntryCVViewModelList)pcvList);

            foreach (var i in pcvList.EntryCV)
            {
                pcvList.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));
                i.screenCode = "PCV";

                var vend = _service.getVendor(i.dtl_Ewt_Payor_Name_ID);
                if (vend != null)
                {
                    i.vendTRList = _service.getVendorTaxList(vend.Vendor_MasterID).ToList();
                    i.vendVATList = _service.getVendorVatList(vend.Vendor_MasterID).ToList();
                    if (i.vendTRList == null || i.vendTRList.Count() == 0)
                    {
                        i.vendTRList.Add(new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 });
                    }
                    if (i.vendVATList == null || i.vendVATList.Count() == 0)
                    {
                        i.vendVATList.Add(new DMVATModel { VAT_ID = 0, VAT_Rate = 0 });
                    }
                }
                else
                {
                    i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                    i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                } 
            }
            pcvList.systemValues.ewt = new SelectList("0", "0");
            pcvList.systemValues.vat = new SelectList("0", "0");

            List<cvBirForm> birForms = new List<cvBirForm>();
            foreach (var item in pcvList.EntryCV)
            {
                if (birForms.Any(x => x.ewt == item.ewt && x.vendor == item.dtl_Ewt_Payor_Name_ID))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ewt);
                    birForms[index].amount += item.debitGross;
                }
                else
                {
                    cvBirForm temp = new cvBirForm
                    {
                        amount = item.debitGross,
                        ewt = item.ewt,
                        vat = item.vat,
                        vendor = item.dtl_Ewt_Payor_Name_ID,
                        approver = pcvList.approver,
                        date = pcvList.createdDate
                    };
                    if (item.ewt > 0)
                    {
                        birForms.Add(temp);
                    }
                }
            }
            pcvList.birForms.AddRange(birForms);

            //return View(viewLink, pcvList);
            return RedirectToAction(viewLink, "Home", new { entryID = entryID });
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult View_PCV(int entryID)
        {
            var userId = GetUserID();

            if (entryID == 0 && TempData["entryIDAddtoView"] != null)
            { 
                entryID = (int)TempData["entryIDAddtoView"];
                TempData.Keep();
            }
            else
            {
                TempData.Remove("entryIDAddtoView");
            }

            EntryCVViewModelList pcvList = _service.getExpense(entryID);
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            pcvList.systemValues.vendors = listOfSysVals[0];
            pcvList.systemValues.dept = listOfSysVals[1];
            pcvList.systemValues.currency = listOfSysVals[2];
            pcvList.systemValues.employeesAll = listOfSysVals[5];
            //int firstId = int.Parse(listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR].First().Value);
            
            pcvList.systemValues.acc = _service.getAccDetailsEntry();

            foreach (var i in pcvList.EntryCV)
            {
                i.screenCode = "PCV";
                if (i.dtl_Ewt_Payor_Name_ID > 0)
                {
                    i.vendTRList = _service.getAllTRList();
                    i.vendVATList = _service.getAllVATList();
                }
                else
                {
                    i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                    i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                }
            }

            List<cvBirForm> birForms = new List<cvBirForm>();
            foreach (var item in pcvList.EntryCV)
            {
                if (birForms.Any(x => x.ewt == item.ewt && x.vendor == item.dtl_Ewt_Payor_Name_ID))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ewt);
                    birForms[index].amount += item.debitGross;
                }
                else
                {
                    cvBirForm temp = new cvBirForm
                    {
                        amount = item.debitGross,
                        ewt = item.ewt,
                        vat = item.vat,
                        vendor = item.dtl_Ewt_Payor_Name_ID,
                        approver = pcvList.approver,
                        date = pcvList.createdDate
                    };
                    if(item.ewt > 0)
                    {
                        birForms.Add(temp);
                    }
                }
            }
            pcvList.birForms.AddRange(birForms);

            if (!String.IsNullOrEmpty(GlobalSystemValues.MESSAGE))
            {
                ViewData["MESSAGE"] = GlobalSystemValues.MESSAGE;
                GlobalSystemValues.MESSAGE = "";
            }

            return View("Entry_PCV_ReadOnly", pcvList);
        }
        //[* Entry Petty Cash *]
        //------------------------------------------------------------------

        //-------------[* Entry Cash Advance(SS) *]--------------------------
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ImportModelState]
        [ServiceFilter(typeof(MakerCheck))]
        public IActionResult Entry_SS(int entryID)
        {
            var userId = GetUserID();
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            var ccyPHP = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_PHP").Value));
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));

            EntryCVViewModelList viewModel = new EntryCVViewModelList();

            if (entryID > 0)
            {
                viewModel = _service.getExpense(entryID);
                viewModel = PopulateEntry((EntryCVViewModelList)viewModel, GlobalSystemValues.TYPE_SS);

                foreach (var i in viewModel.EntryCV)
                {
                    viewModel.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));
                    i.screenCode = "SS";

                    var vend = _service.getVendor(i.dtl_Ewt_Payor_Name_ID);
                    if (vend != null)
                    {
                        i.vendTRList = _service.getVendorTaxList(vend.Vendor_MasterID).ToList();
                        i.vendVATList = _service.getVendorVatList(vend.Vendor_MasterID).ToList();
                        if (i.vendTRList == null || i.vendTRList.Count() == 0)
                        {
                            i.vendTRList.Add(new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 });
                        }
                        if (i.vendVATList == null || i.vendVATList.Count() == 0)
                        {
                            i.vendVATList.Add(new DMVATModel { VAT_ID = 0, VAT_Rate = 0 });
                        }
                    }
                    else
                    {
                        i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                        i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                    }
                }
            }
            else
            {
                viewModel = PopulateEntry((EntryCVViewModelList)viewModel, GlobalSystemValues.TYPE_SS);

                viewModel.EntryCV.Add(new EntryCVViewModel {
                    screenCode = "SS",
                    vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } },
                    vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } },
                    ccy = ccyPHP.Curr_ID
                });
            }

            //select values for reg employee payee
            viewModel.payee_type = GlobalSystemValues.PAYEETYPE_REGEMP;
            viewModel.systemValues.ewt = new SelectList("0", "0");
            viewModel.systemValues.vat = new SelectList("0", "0");
            viewModel.phpCurrID = ccyPHP.Curr_ID;
            viewModel.phpCurrMasterID = ccyPHP.Curr_MasterID;
            viewModel.phpAbbrev = ccyPHP.Curr_CCY_ABBR;
            viewModel.yenCurrID = ccyYEN.Curr_ID;
            viewModel.yenCurrMasterID = ccyYEN.Curr_MasterID;
            viewModel.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            return View(viewModel);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ExportModelState]
        public IActionResult AddNewSS(EntryCVViewModelList EntryCVViewModelList)
        {
            var userId = GetUserID();

            if (!ModelState.IsValid)
            {
                EntryCVViewModelList = PopulateEntry((EntryCVViewModelList)EntryCVViewModelList, GlobalSystemValues.TYPE_SS);

                foreach (var i in EntryCVViewModelList.EntryCV)
                {
                    EntryCVViewModelList.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));
                    i.screenCode = "SS";

                    var vend = _service.getVendor(i.dtl_Ewt_Payor_Name_ID);
                    if (vend != null)
                    {
                        i.vendTRList = _service.getVendorTaxList(vend.Vendor_MasterID).ToList();
                        i.vendVATList = _service.getVendorVatList(vend.Vendor_MasterID).ToList();
                        if (i.vendTRList == null || i.vendTRList.Count() == 0)
                        {
                            i.vendTRList.Add(new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 });
                        }
                        if (i.vendVATList == null || i.vendVATList.Count() == 0)
                        {
                            i.vendVATList.Add(new DMVATModel { VAT_ID = 0, VAT_Rate = 0 });
                        }
                    }
                    else
                    {
                        i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                        i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                    }
                }
                EntryCVViewModelList.systemValues.ewt = new SelectList("0", "0");
                EntryCVViewModelList.systemValues.vat = new SelectList("0", "0");

                XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
                var ccyPHP = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_PHP").Value));
                EntryCVViewModelList.phpCurrID = ccyPHP.Curr_ID;
                EntryCVViewModelList.phpCurrMasterID = ccyPHP.Curr_MasterID;
                EntryCVViewModelList.phpAbbrev = ccyPHP.Curr_CCY_ABBR;
                var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));
                EntryCVViewModelList.yenCurrID = ccyYEN.Curr_ID;
                EntryCVViewModelList.yenCurrMasterID = ccyYEN.Curr_MasterID;
                EntryCVViewModelList.yenAbbrev = ccyYEN.Curr_CCY_ABBR;
                return View("Entry_SS", PopulateEntry((EntryCVViewModelList)EntryCVViewModelList, GlobalSystemValues.TYPE_SS));
            }

            //EntryCVViewModelList ssList = new EntryCVViewModelList();

            int id = 0;
            if (EntryCVViewModelList.entryID == 0)
            {
                id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_SS);
                //----------------------------- NOTIF----------------------------------
                _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_SS, GlobalSystemValues.STATUS_NEW, 0);
                //----------------------------- NOTIF----------------------------------
            }
            else
            {
                List<int> EditableStatus = new List<int>{
                    GlobalSystemValues.STATUS_PENDING,
                    GlobalSystemValues.STATUS_REJECTED
                };
                var currentStat = _service.GetCurrentEntryStatus(EntryCVViewModelList.entryID);
                if (EditableStatus.Contains(currentStat))
                {
                    if (_service.deleteExpenseEntry(EntryCVViewModelList.entryID))
                    {
                        id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_SS);
                        var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == id).Expense_Creator_ID;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_SS, GlobalSystemValues.STATUS_EDIT, makerId);
                        //----------------------------- NOTIF----------------------------------
                    }
                }
                else
                {
                    id = EntryCVViewModelList.entryID;
                    GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE1;
                }
            }

            ModelState.Clear();

            //if (id > -1)
            //{
            //    pcvList = _service.getExpense(id);
            //    List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            //    pcvList.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            //    pcvList.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            //    pcvList.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            //    pcvList.systemValues.acc = _service.getAccDetailsEntry();
            //    ViewBag.Status = pcvList.status;
            //}

            //return View("Entry_PCV_ReadOnly", pcvList);

            TempData["entryIDAddtoView"] = id;

            return RedirectToAction("View_SS", "Home");
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult VerAppModSS(int entryID, string command)
        {
            var userId = GetUserID();
            var intUser = int.Parse(userId);
            string viewLink = "Entry_SS";
            EntryCVViewModelList ssList;

            var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);
            switch (command)
            {
                case "Modify":
                    viewLink = "Entry_SS";
                    break;
                case "approver":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED, int.Parse(GetUserID())))
                    {
                        _service.postCV(entryID, "P", int.Parse(GetUserID()));
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_SS, GlobalSystemValues.STATUS_APPROVED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_SS";
                    break;
                case "verifier":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_VERIFIED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_SS, GlobalSystemValues.STATUS_VERIFIED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_SS";
                    break;
                case "Delete":
                    int expStatus = _service.GetCurrentEntryStatus(entryID);
                    if (expStatus == GlobalSystemValues.STATUS_PENDING || expStatus == GlobalSystemValues.STATUS_REJECTED)
                    {
                        if (_service.deleteExpenseEntry(entryID))
                        {
                            ViewBag.Success = 1;
                            //----------------------------- NOTIF----------------------------------
                            _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_SS, GlobalSystemValues.STATUS_DELETE, 0);
                            //----------------------------- NOTIF----------------------------------                    
                        }
                        else
                        {
                            ViewBag.Success = 0;
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE2;
                        ViewBag.Success = 0;
                    }

                    viewLink = "View_SS";
                    break;
                case "Reject":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REJECTED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_SS, GlobalSystemValues.STATUS_REJECTED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------                    
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_SS";
                    break;
                case "Reversal":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REVERSED, int.Parse(GetUserID())))
                    {
                        _service.postCV(entryID, "R", int.Parse(GetUserID()));
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_SS, GlobalSystemValues.STATUS_REVERSED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------                    
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_SS";
                    break;
                default:
                    break;
            }

            //Entry was Edited, Deleted by user before clicking the approve/verify button by user, 
            //return error message then redirect to Home Index screen.
            if (GlobalSystemValues.MESSAGE == GlobalSystemValues.MESSAGE3)
            {
                return RedirectToAction("Index", "Home");
            }

            ssList = _service.getExpense(entryID);
            ssList = PopulateEntry((EntryCVViewModelList)ssList, GlobalSystemValues.TYPE_SS);

            foreach (var i in ssList.EntryCV)
            {
                ssList.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));
                i.screenCode = "SS";

                var vend = _service.getVendor(i.dtl_Ewt_Payor_Name_ID);
                if(vend != null)
                {
                    i.vendTRList = _service.getVendorTaxList(vend.Vendor_MasterID).ToList();
                    i.vendVATList = _service.getVendorVatList(vend.Vendor_MasterID).ToList();
                    if (i.vendTRList == null || i.vendTRList.Count() == 0)
                    {
                        i.vendTRList.Add(new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 });
                    }
                    if (i.vendVATList == null || i.vendVATList.Count() == 0)
                    {
                        i.vendVATList.Add(new DMVATModel { VAT_ID = 0, VAT_Rate = 0 });
                    }
                }
                else
                {
                    i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                    i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                }
            }
            ssList.systemValues.ewt = new SelectList("0", "0");
            ssList.systemValues.vat = new SelectList("0", "0");

            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            var ccyPHP = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_PHP").Value));
            ssList.phpCurrID = ccyPHP.Curr_ID;
            ssList.phpCurrMasterID = ccyPHP.Curr_MasterID;
            ssList.phpAbbrev = ccyPHP.Curr_CCY_ABBR;
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));
            ssList.yenCurrID = ccyYEN.Curr_ID;
            ssList.yenCurrMasterID = ccyYEN.Curr_MasterID;
            ssList.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            List<cvBirForm> birForms = new List<cvBirForm>();
            foreach (var item in ssList.EntryCV)
            {
                double grossOrig = item.gBaseRemarksDetails.Sum(x => x.amount);

                if (birForms.Any(x => x.ewt == item.ewt && x.vendor == item.dtl_Ewt_Payor_Name_ID))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ewt);
                    birForms[index].amount += grossOrig;
                }
                else
                {
                    cvBirForm temp = new cvBirForm
                    {
                        amount = grossOrig,
                        ewt = item.ewt,
                        vat = item.vat,
                        vendor = item.dtl_Ewt_Payor_Name_ID,
                        approver = ssList.approver,
                        date = ssList.createdDate
                    };
                    if (item.ewt > 0)
                    {
                        birForms.Add(temp);
                    }
                }
            }
            ssList.birForms.AddRange(birForms);

            return RedirectToAction(viewLink, "Home", new { entryID = entryID });
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult View_SS(int entryID)
        {
            var userId = GetUserID();

            if (entryID == 0 && TempData["entryIDAddtoView"] != null)
            {
                entryID = (int)TempData["entryIDAddtoView"];
                TempData.Keep();
            }
            else
            {
                TempData.Remove("entryIDAddtoView");
            }

            EntryCVViewModelList ssList = _service.getExpense(entryID);
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            ssList.systemValues.vendors = listOfSysVals[0];
            ssList.systemValues.dept = listOfSysVals[1];
            ssList.systemValues.currency = listOfSysVals[2];
            ssList.systemValues.employees = listOfSysVals[4];
            //int firstId = int.Parse(listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR].First().Value);

            XElement xelemliq = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            var ccyPHP = _service.getCurrencyByMasterID(int.Parse(xelemliq.Element("CURRENCY_PHP").Value));
            ssList.phpCurrID = ccyPHP.Curr_ID;
            ssList.phpCurrMasterID = ccyPHP.Curr_MasterID;
            ssList.phpAbbrev = ccyPHP.Curr_CCY_ABBR;
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelemliq.Element("CURRENCY_Yen").Value));
            ssList.yenCurrID = ccyYEN.Curr_ID;
            ssList.yenCurrMasterID = ccyYEN.Curr_MasterID;
            ssList.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            DMAccountModel acc = new DMAccountModel();
            List<accDetails> acclist = new List<accDetails>();
            XElement xelem = XElement.Load("wwwroot/xml/GlobalAccounts.xml");
            for (int i = 1; i <= 5; i++)
            {
                acc = _service.getAccountByMasterID(int.Parse(xelem.Element("D_SS" + i).Value));
                if(acc != null)
                {
                    acclist.Add(new accDetails
                    {
                        accId = acc.Account_ID,
                        accName = acc.Account_Name,
                        accCode = acc.Account_Code
                    });
                }
            }
            ssList.systemValues.acc = acclist;

            foreach (var i in ssList.EntryCV)
            {
                i.screenCode = "SS";
                if(i.dtl_Ewt_Payor_Name_ID > 0)
                {
                    i.vendTRList = _service.getAllTRList();
                    i.vendVATList = _service.getAllVATList();
                }
                else
                {
                    i.vendTRList = new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } };
                    i.vendVATList = new List<DMVATModel> { new DMVATModel { VAT_ID = 0, VAT_Rate = 0 } };
                }
            }

            List<cvBirForm> birForms = new List<cvBirForm>();
            foreach (var item in ssList.EntryCV)
            {
                double grossOrig = item.gBaseRemarksDetails.Sum(x => x.amount);

                if (birForms.Any(x => x.ewt == item.ewt && x.vendor == item.dtl_Ewt_Payor_Name_ID))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ewt);
                    birForms[index].amount += grossOrig;
                }
                else
                {
                    cvBirForm temp = new cvBirForm
                    {
                        amount = grossOrig,
                        ewt = item.ewt,
                        vat = item.vat,
                        vendor = item.dtl_Ewt_Payor_Name_ID,
                        approver = ssList.approver,
                        date = ssList.createdDate
                    };
                    if (item.ewt > 0)
                    {
                        birForms.Add(temp);
                    }
                }
            }
            ssList.birForms.AddRange(birForms);

            return View("Entry_SS_ReadOnly", ssList);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult CDD_IS_SS(int entryID, int entryDtlID, string ccyAbbr)
        {
            string newFileName = "CDD_IS_CashAdvance_" + ccyAbbr + "_" + DateTime.Now.ToString("MM-dd-yyyy_hhmmss") + ".xlsx";
            var expense = _service.getExpenseDetail(entryID);
            var expenseDtl = expense.ExpenseEntryDetails.Where(x => x.ExpDtl_ID == entryDtlID).FirstOrDefault();

            ExcelGenerateService excelGenerate = new ExcelGenerateService();
            CDDISValuesVIewModel viewModel = new CDDISValuesVIewModel
            {
                VALUE_DATE = DateTime.Parse(expense.Expense_Date.ToLongDateString()),
                REMARKS = expenseDtl.ExpDtl_Gbase_Remarks,
                CURRENCY = _service.getCurrency(expenseDtl.ExpDtl_Ccy).Curr_MasterID
            };

            List<CDDISValueContentsViewModel> cddContents = new List<CDDISValueContentsViewModel>
            {
                new CDDISValueContentsViewModel
                {
                    AMOUNT = expenseDtl.ExpDtl_Debit,
                },
                new CDDISValueContentsViewModel
                {
                    AMOUNT = expenseDtl.ExpDtl_Debit,
                },
            };
            viewModel.CDDContents = cddContents;
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            string excelTemplateName = (viewModel.CURRENCY == int.Parse(xelem.Element("CURRENCY_Yen").Value)) ? "CDDIS_Yen.xlsx" : "CDDIS_USD.xlsx";

            bool result = _service.UpdateCDDPrintingStatus(entryID, entryDtlID, GlobalSystemValues.TYPE_SS);

            return File(excelGenerate.ExcelCDDIS(viewModel, newFileName, excelTemplateName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", newFileName);

        }

        //------------------------------------------------------------------
        //[* Entry Non Cash *]

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ImportModelState]
        public IActionResult Entry_NC(EntryNCViewModelList viewModel, string partialName)
        {
            var userId = GetUserID();
            if(viewModel.EntryNC == null)
            {
                viewModel = new EntryNCViewModelList();
            }
            ViewData["partialName"] = partialName ?? (viewModel.EntryNC.NC_Category_ID.ToString() != "0" ? viewModel.EntryNC.NC_Category_ID.ToString() : GlobalSystemValues.NC_LS_PAYROLL.ToString());
            viewModel.category_of_entry = GlobalSystemValues.NC_CATEGORIES_SELECT;
            viewModel.expenseDate = DateTime.Now;
            viewModel.entryID = 0;
            return View(viewModel);
        }
        [ExportModelState]
        public IActionResult AddNewNC(EntryNCViewModelList EntryNCViewModelList)
        {
            var userId = GetUserID();
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Entry_NC", EntryNCViewModelList);
            }

            //EntryNCViewModelList ncList = new EntryNCViewModelList();
            int id = _service.addExpense_NC(EntryNCViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_NC);
            ModelState.Clear();
            //return RedirectToAction("View_NC", "Home", new { entryID = id });
            TempData["entryIDAddtoView"] = id;

            return RedirectToAction("View_NC", "Home");
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult View_NC(int entryID)
        {
            var userId = GetUserID();

            if (entryID == 0 && TempData["entryIDAddtoView"] != null)
            {
                entryID = (int)TempData["entryIDAddtoView"];
                TempData.Keep();
            }
            else
            {
                TempData.Remove("entryIDAddtoView");
            }

            EntryNCViewModelList ncList = _service.getExpenseNC(entryID);
            ncList = PopulateEntryNC(ncList);
            DMCurrencyModel currDtl = _context.DMCurrency.Where(x => x.Curr_MasterID == 1 && x.Curr_isActive == true && x.Curr_isDeleted == false).FirstOrDefault();
            DMCurrencyModel currDtlUSD = _context.DMCurrency.Where(x => x.Curr_MasterID == 2 && x.Curr_isActive == true && x.Curr_isDeleted == false).FirstOrDefault();

            if (ncList.EntryNC.NC_Category_ID == GlobalSystemValues.NC_PETTY_CASH_REPLENISHMENT)
            {
                ncList.EntryNC.ExpenseEntryNCDtls_CDD = CONSTANT_NC_PETTYCASHREPLENISHMENT.Populate_CDD_Instruc_Sheet(currDtl);
                ncList.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Amount = ncList.EntryNC.NC_CS_DebitAmt;
                ncList.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Amount = ncList.EntryNC.NC_CS_CredAmt;
            }else if (ncList.EntryNC.NC_Category_ID == GlobalSystemValues.NC_RETURN_OF_JS_PAYROLL)
            {
                ncList.EntryNC.ExpenseEntryNCDtls_CDD = CONSTANT_NC_RETURN_OF_JSPAYROLL.Populate_CDD_Instruc_Sheet(currDtl, currDtlUSD);
                var currDtls = ncList.EntryNC.ExpenseEntryNCDtls[0].ExpenseEntryNCDtlAccs[0];
                ncList.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Curr_ID = currDtls.ExpNCDtlAcc_Curr_ID;
                ncList.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Curr_ID = currDtls.ExpNCDtlAcc_Curr_ID;
                ncList.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Curr_Name = currDtls.ExpNCDtlAcc_Curr_Name;
                ncList.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Curr_Name = currDtls.ExpNCDtlAcc_Curr_Name;
                ncList.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Amount = ncList.EntryNC.NC_CS_DebitAmt;
                ncList.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Amount = ncList.EntryNC.NC_CS_CredAmt;
            }
            ViewData["USDmstr"] = _service.getXMLCurrency("USD").FirstOrDefault().currMasterID;
            ViewData["JPYmstr"] = _service.getXMLCurrency("YEN").FirstOrDefault().currMasterID;
            ViewData["PHPmstr"] = _service.getXMLCurrency("PHP").FirstOrDefault().currMasterID;


            List<cvBirForm> birForms = new List<cvBirForm>();
            foreach (var item in ncList.EntryNC.ExpenseEntryNCDtls)
            {
                if (birForms.Any(x => x.ewt == item.ExpNCDtl_TR_ID && x.vendor == item.ExpNCDtl_Vendor_ID))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ExpNCDtl_TR_ID);
                    foreach (var a in item.ExpenseEntryNCDtlAccs)
                    {
                        if (a.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT)
                        {
                            birForms[index].amount += a.ExpNCDtlAcc_Amount;
                        }
                    }
                }
                else
                {
                    double amt = 0;
                    foreach (var a in item.ExpenseEntryNCDtlAccs)
                    {
                        if (a.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT)
                        {
                            amt += a.ExpNCDtlAcc_Amount;
                        }
                    }
                    cvBirForm temp = new cvBirForm
                    {
                        amount = amt,
                        ewt = item.ExpNCDtl_TR_ID,
                        //vat = item.vat,
                        vendor = item.ExpNCDtl_Vendor_ID,
                        approver = ncList.approver,
                        date = ncList.expenseDate
                    };

                    birForms.Add(temp);
                }
            }
            ncList.birForms.AddRange(birForms);

            if (!String.IsNullOrEmpty(GlobalSystemValues.MESSAGE))
            {
                ViewData["MESSAGE"] = GlobalSystemValues.MESSAGE;
                GlobalSystemValues.MESSAGE = "";
            }

            return View("Entry_NC_ReadOnly", ncList);
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult VerAppModNC(int entryID, string command)
        {
            var userId = GetUserID();
            var intUser = int.Parse(userId);
            string viewLink = "Entry_NC";
            EntryNCViewModelList ncList;

            switch (command)
            {
                case "Modify":
                    viewLink = "Entry_NC";
                    break;
                case "approver":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED, int.Parse(GetUserID())))
                    {
                        //"P" for Normal Posting, "R" for Reversal
                        _service.postNC(entryID,"P", int.Parse(GetUserID()));
                        ViewBag.Success = 1;
                        var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_NC, GlobalSystemValues.STATUS_APPROVED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------                    
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    return RedirectToAction("View_NC", "Home", new { entryID = entryID });
                case "verifier":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_VERIFIED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                        var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_NC, GlobalSystemValues.STATUS_VERIFIED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------                    
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    return RedirectToAction("View_NC", "Home", new { entryID = entryID });

                case "Reject":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REJECTED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                        var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_NC, GlobalSystemValues.STATUS_REJECTED, makerId);
                        //----------------------------- NOTIF----------------------------------  
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    return RedirectToAction("View_NC", "Home", new { entryID = entryID });

                case "Delete":
                    int expStatus = _service.GetCurrentEntryStatus(entryID);
                    if (expStatus == GlobalSystemValues.STATUS_PENDING || expStatus == GlobalSystemValues.STATUS_REJECTED)
                    {
                        if (_service.deleteExpenseEntry(entryID, GlobalSystemValues.TYPE_NC))
                        {
                            ViewBag.Success = 1;
                            //----------------------------- NOTIF----------------------------------
                            _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_NC, GlobalSystemValues.STATUS_DELETE, 0);
                            //----------------------------- NOTIF----------------------------------                    
                        }
                        else
                        {
                            ViewBag.Success = 0;
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE2;
                        ViewBag.Success = 0;
                    }

                    return RedirectToAction("View_NC", "Home", new { entryID = entryID });

                case "PrintCDD":
                    return RedirectToAction("CDD_IS_NC_PCR", new { entryID = entryID });
                case "PrintCDD_RET":
                    return RedirectToAction("CDD_IS_NC_RET", new { entryID = entryID }); 
                case "Reversal":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REVERSED, int.Parse(GetUserID())))
                    {
                        _service.postNC(entryID, "R", int.Parse(GetUserID()));
                        ViewBag.Success = 1;
                        var makerId = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == entryID);
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_NC, GlobalSystemValues.STATUS_REVERSED, makerId.Expense_Creator_ID);
                        //----------------------------- NOTIF----------------------------------                    
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    return RedirectToAction("View_NC", "Home", new { entryID = entryID });

                default:
                    break;
            }

            ModelState.Clear();
            ncList = _service.getExpenseNC(entryID);
            ncList = PopulateEntryNC(ncList);

            var accs = _service.getNCAccsForFilter();
            ncList.accList = accs;
            ViewData["partialName"] = ncList.EntryNC.NC_Category_ID.ToString();
            return View(viewLink, ncList);
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult CDD_IS_NC_PCR(int entryID)
        {
            var entryVals = _service.getExpenseNC(entryID);
            string newFileName = "CDD_IS_NC_PCR_" + DateTime.Now.ToString("MM-dd-yyyy_hhmmss") + ".xlsx";
            ExcelGenerateService excelGenerate = new ExcelGenerateService();
            CDDISValuesVIewModel viewModel = new CDDISValuesVIewModel {
                VALUE_DATE = DateTime.Parse("2019/01/01"), //TEMP VALUE
                COMMENT = "  ",
                SECTION = "09",
                REMARKS = "AD: PETTY CASH REPLENISHEMENT",
                SCHEME_NO = "  ",
                MEMO = " "
            };
            List<CDDISValueContentsViewModel> cddContents = new List<CDDISValueContentsViewModel>();
            foreach (var dtl in entryVals.EntryNC.ExpenseEntryNCDtls){
                foreach (var acc in dtl.ExpenseEntryNCDtlAccs)
                {
                    var acct = _context.DMAccount.Where(x => x.Account_ID == acc.ExpNCDtlAcc_Acc_ID).FirstOrDefault();
                    CDDISValueContentsViewModel vm = new CDDISValueContentsViewModel
                    {
                        DEBIT_CREDIT = (acc.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT) ? "D" : "C",
                        CCY = _context.DMCurrency.Where(x=> x.Curr_ID == acc.ExpNCDtlAcc_Curr_ID).Select(x => x.Curr_CCY_ABBR).FirstOrDefault(),
                        AMOUNT = acc.ExpNCDtlAcc_Amount,
                        CUSTOMER_ABBR = "900",
                        ACCOUNT_CODE = "147017"/*acct.Account_Code*/,
                        ACCOUNT_NO = acct.Account_No,
                        //EXCHANGE_RATE = ,
                        CONTRA_CCY = "   ",
                        FUND = (acct.Account_Fund) ? "O" : " ",
                        CHECK_NO = " ",
                        AVAILABLE_DATE = DateTime.Parse("2019/02/01"),
                        ADVICE = " ",
                        DETAILS = " ",
                        ENTITY = "010",
                        DIVISION = "11",
                        INTER_AMOUNT = 0,
                        INTER_RATE = 0,
                    };
                    cddContents.Add(vm);
                }
            }
            viewModel.CDDContents = cddContents;
            return File(excelGenerate.ExcelCDDIS_PRC(viewModel, newFileName, "CDDIS_NC_PCR.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", newFileName);

        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult CDD_IS_NC_RET(int entryID)
        {
            var entryVals = _service.getExpenseNC(entryID);
            List<CONSTANT_CCY_VALS> currUSD = _service.getXMLCurrency("USD");
            List<CONSTANT_CCY_VALS> currJPY = _service.getXMLCurrency("YEN");
            var masterid = _service.getCurrencyMasterID(entryVals.EntryNC.ExpenseEntryNCDtls[0].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Curr_ID);
            DMCurrencyModel currModel = _service.getCurrency(entryVals.EntryNC.ExpenseEntryNCDtls[0].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Curr_ID);

            string newFileName = "CDD_IS_NC_RET_" + DateTime.Now.ToString("MM-dd-yyyy_hhmmss") + ".xlsx";
            ExcelGenerateService excelGenerate = new ExcelGenerateService();
            CDDISValuesVIewModel viewModel = new CDDISValuesVIewModel
            {
                VALUE_DATE = DateTime.Now, //TEMP VALUE
                COMMENT = "  ",
                SECTION = "09",
                REMARKS = "",
                SCHEME_NO = "  ",
                MEMO = " "
            };
            List<CDDISValueContentsViewModel> cddContents = new List<CDDISValueContentsViewModel>();
            float totalDeb = 0;
            float totalCred = 0;
            entryVals.EntryNC.ExpenseEntryNCDtls.ForEach(x =>
            {
                x.ExpenseEntryNCDtlAccs.ForEach(a =>
                {
                    //check if acc is Computer Suspense or Computer Suspense(USD)
                    var filterAccs = _service.getNCAccsForFilter().ToList();
                   if(filterAccs.Where(f=> f.accName.ToLower().Contains("comp")).Select(n => n.accID).Contains(a.ExpNCDtlAcc_Acc_ID))
                    {
                        if(a.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT)
                        {
                            totalDeb += a.ExpNCDtlAcc_Amount;
                        }
                        else
                        {
                            totalCred += a.ExpNCDtlAcc_Amount;
                        }
                    }
                });
            });

            CDDISValueContentsViewModel vm = new CDDISValueContentsViewModel
            {
                DEBIT_CREDIT = "D",
                CCY = currModel.Curr_CCY_ABBR,
                AMOUNT = totalDeb
            };
            CDDISValueContentsViewModel vm2 = new CDDISValueContentsViewModel
            {
                DEBIT_CREDIT = "C",
                CCY = currModel.Curr_CCY_ABBR,
                AMOUNT = totalCred
            };
            cddContents.Add(vm);
            cddContents.Add(vm2);
            viewModel.CDDContents = cddContents;
            var xlFileName = masterid == currUSD.FirstOrDefault().currMasterID
                ? "CDDIS_NC_RET_USD.xlsx" : masterid == currJPY.FirstOrDefault().currMasterID ? "CDDIS_NC_RET_YEN.xlsx" : "CDDIS_NC_PCR.xlsx";
            return File(excelGenerate.ExcelCDDIS_RET(viewModel, newFileName, xlFileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", newFileName);

        }

        //[* Entry Non Cash *]
        //------------------------------------------------------------------

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

        public EntryCVViewModelList PopulateEntryCV(EntryCVViewModelList viewModel)
        {
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            viewModel.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            viewModel.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            viewModel.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
            viewModel.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            viewModel.systemValues.acc = _service.getAccDetailsEntry();

            viewModel.expenseYear = DateTime.Today.Year.ToString();
            viewModel.expenseDate = DateTime.Today;

            return viewModel;
        }
        public EntryNCViewModelList PopulateEntryNC(EntryNCViewModelList viewModel)
        {
            viewModel.category_of_entry = GlobalSystemValues.NC_CATEGORIES_SELECT;
            viewModel.EntryNC.NC_Category_Name = GlobalSystemValues.NC_CATEGORIES_SELECT.Where(x => x.Value == viewModel.EntryNC.NC_Category_ID + "")
                                            .Select(x => x.Text).FirstOrDefault();
            viewModel.expenseDate = DateTime.Now;
            return viewModel;
        }

        //-------------[* Liquidation *]--------------------------
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Liquidation_Main(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var userId = GetUserID();
            InterEntityValues interEntityValues = new InterEntityValues();

            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ApplicationTypeSortParm"] = sortOrder == "app_type_desc" ? "app_type" : "app_type_desc";
            ViewData["AmountSortParm"] = sortOrder == "amount_desc" ? "amount" : "amount_desc";
            ViewData["MakerSortParm"] = sortOrder == "maker_desc" ? "maker" : "maker_desc";
            ViewData["ApproverSortParm"] = sortOrder == "approver_desc" ? "approver" : "approver_desc";
            ViewData["DateSubmittedSortParm"] = sortOrder == "date_submitted_desc" ? "date_submitted" : "date_submitted_desc";
            ViewData["LastUpdatedDateSortParm"] = sortOrder == "last_updated_desc" ? "last_updated" : "last_updated_desc";

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
            var sortedVals = _sortService.SortData(_service.populateLiquidationList(int.Parse(userId)), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            return View(PaginatedList<LiquidationMainListViewModel>.CreateAsync(
                (sortedVals.list).Cast<LiquidationMainListViewModel>().AsQueryable().AsNoTracking(), page ?? 1, pageSize));
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ServiceFilter(typeof(MakerCheckLiquidation))]
        public IActionResult Liquidation_SS(int entryID)
        {
            var userId = GetUserID();
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");

            LiquidationViewModel ssList = _service.getExpenseToLiqudate(entryID);
            ssList.accList = _service.getAccountList();
            ssList.accAllList = _service.getAccountListIncHist();
            ssList.vendorList = _service.getVendorList().OrderBy(x => x.Vendor_Name).ToList();
            if(ssList.LiqEntryDetails.Liq_Created_UserID == 0)
            {
                ssList.LiqEntryDetails.Liq_Created_Date = DateTime.Now.Date;
            }

            foreach (var i in ssList.accAllList)
            {
                i.Account_Name = i.Account_No + " - " + i.Account_Name;
            }
            foreach (var i in ssList.LiquidationDetails)
            {
                i.screenCode = "Liquidation_SS";
            }

            var ccyPHP = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_PHP").Value));
            ssList.phpCurrID = ccyPHP.Curr_ID;
            ssList.phpCurrMasterID = ccyPHP.Curr_MasterID;
            ssList.phpAbbrev = ccyPHP.Curr_CCY_ABBR;
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));
            ssList.yenCurrID = ccyYEN.Curr_ID;
            ssList.yenCurrMasterID = ccyYEN.Curr_MasterID;
            ssList.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            List<cvBirForm> birForms = new List<cvBirForm>();
            ssList.birForms = new List<cvBirForm>();
            foreach (var item in ssList.LiquidationDetails)
            {
                double grossOrig = item.gBaseRemarksDetails.Sum(x => x.amount);

                if (birForms.Any(x => x.ewt == item.ewtID && x.vendor == item.dtl_Ewt_Payor_Name_ID))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ewtID);
                    birForms[index].amount += grossOrig;
                }
                else
                {
                    cvBirForm temp = new cvBirForm
                    {
                        amount = grossOrig,
                        ewt = item.ewtID,
                        vat = item.vatID,
                        vendor = item.dtl_Ewt_Payor_Name_ID,
                        approver = ssList.approver,
                        date = ssList.createdDate
                    };
                    if (item.ewtID > 0)
                    {
                        birForms.Add(temp);
                    }
                }
            }
            if (birForms.Count() > 0)
            {
                ssList.birForms.AddRange(birForms);
            }

            return View("Liquidation_SS", ssList);
        }

        [ExportModelState]
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Liquidation_AddNewSS(LiquidationViewModel vm)
        {
            var userId = GetUserID();
            if (!ModelState.IsValid)
            {
                vm.accList = _service.getAccountList();
                vm.accAllList = _service.getAccountListIncHist();
                vm.vendorList = _service.getVendorList().OrderBy(x => x.Vendor_Name).ToList();
                vm.taxRateList = _service.getVendorTaxList(vm.vendorList[0].Vendor_MasterID);
                foreach (var i in vm.taxRateList)
                {
                    i.TR_WT_Title = (i.TR_Tax_Rate * 100) + "% " + i.TR_WT_Title;
                }
                foreach (var i in vm.accAllList)
                {
                    i.Account_Name = i.Account_No + " - " + i.Account_Name;
                }
                return View("Liquidation_SS", vm);
            }

            int id = 0;
            int exist = _service.getLiquidationExistence(vm.entryID);

            if (exist == 0)
            {
                id = _service.addLiquidationDetail(vm, int.Parse(GetUserID()), exist);
                //----------------------------- NOTIF----------------------------------
                _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_LIQ, GlobalSystemValues.STATUS_NEW, 0);
                //----------------------------- NOTIF----------------------------------
            }
            else
            {
                List<int> EditableStatus = new List<int>{
                    GlobalSystemValues.STATUS_PENDING,
                    GlobalSystemValues.STATUS_REJECTED
                };
                var currentStat = _service.getCurrentLiquidationStatus(vm.entryID);
                if (EditableStatus.Contains(currentStat))
                {
                    if (_service.deleteLiquidationEntry(vm.entryID))
                    {
                        id = _service.addLiquidationDetail(vm, int.Parse(GetUserID()), exist);
                        var makerId = _context.LiquidationEntryDetails.FirstOrDefault(x => x.ExpenseEntryModel.Expense_ID == id).Liq_Created_UserID;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(int.Parse(userId), GlobalSystemValues.TYPE_SS, GlobalSystemValues.STATUS_EDIT, makerId);
                        //----------------------------- NOTIF----------------------------------
                    }
                }
                else
                {
                    id = vm.entryID;
                    GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE1;
                }
            }

            ModelState.Clear();

            TempData["entryIDAddtoView"] = id;

            return RedirectToAction("View_Liquidation_SS", "Home");
        }

        [ExportModelState]
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult View_Liquidation_SS(int entryID)
        {
            var userId = GetUserID();
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");

            if (entryID == 0 && TempData["entryIDAddtoView"] != null)
            {
                entryID = (int)TempData["entryIDAddtoView"];
                TempData.Keep();
            }
            else
            {
                TempData.Remove("entryIDAddtoView");
            }

            LiquidationViewModel ssList = _service.getExpenseToLiqudate(entryID);
            ssList.accList = _service.getAccountList();
            ssList.accAllList = _service.getAccountListIncHist();
            ssList.vendorList = _service.getAllVendorList();

            foreach (var i in ssList.accAllList)
            {
                i.Account_Name = i.Account_No + " - " + i.Account_Name;
            }
            foreach (var i in ssList.LiquidationDetails)
            {
                i.screenCode = "Liquidation_SS";
            }

            var ccyPHP = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_PHP").Value));
            ssList.phpCurrID = ccyPHP.Curr_ID;
            ssList.phpCurrMasterID = ccyPHP.Curr_MasterID;
            ssList.phpAbbrev = ccyPHP.Curr_CCY_ABBR;
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));
            ssList.yenCurrID = ccyYEN.Curr_ID;
            ssList.yenCurrMasterID = ccyYEN.Curr_MasterID;
            ssList.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            List<cvBirForm> birForms = new List<cvBirForm>();
            ssList.birForms = new List<cvBirForm>();
            foreach (var item in ssList.LiquidationDetails)
            {
                double grossOrig = item.gBaseRemarksDetails.Sum(x => x.amount);

                if (birForms.Any(x => x.ewt == item.ewtID && x.vendor == item.dtl_Ewt_Payor_Name_ID))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ewtID);
                    birForms[index].amount += grossOrig;
                }
                else
                {
                    cvBirForm temp = new cvBirForm
                    {
                        amount = grossOrig,
                        ewt = item.ewtID,
                        vat = item.vatID,
                        vendor = item.dtl_Ewt_Payor_Name_ID,
                        approver = ssList.approver,
                        date = ssList.createdDate
                    };
                    if (item.ewtID > 0)
                    {
                        birForms.Add(temp);
                    }
                }
            }
            if(birForms.Count() > 0)
            {
                ssList.birForms.AddRange(birForms);
            }

            if (!String.IsNullOrEmpty(GlobalSystemValues.MESSAGE))
            {
                ViewData["MESSAGE"] = GlobalSystemValues.MESSAGE;
                GlobalSystemValues.MESSAGE = "";
            }

            return View("Liquidation_SS_ReadOnly", ssList);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Liquidation_VerAppModSS(int entryID, string command)
        {
            var userId = GetUserID();
            var intUser = int.Parse(userId);

            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");

            string viewLink = "Liquidation_SS";
            LiquidationViewModel ssList;

            var makerId = _context.LiquidationEntryDetails.FirstOrDefault(x => x.ExpenseEntryModel.Expense_ID == entryID);
            switch (command)
            {
                case "Modify":
                    viewLink = "Liquidation_SS";
                    break;
                case "approver":
                    if (_service.updateLiquidateStatus(entryID, GlobalSystemValues.STATUS_APPROVED, int.Parse(GetUserID())))
                    {
                        _service.postLiq_SS(entryID, command, int.Parse(GetUserID()));
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_LIQ, GlobalSystemValues.STATUS_APPROVED, makerId.Liq_Created_UserID);
                        //----------------------------- NOTIF----------------------------------
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_Liquidation_SS";
                    break;
                case "verifier":
                    if (_service.updateLiquidateStatus(entryID, GlobalSystemValues.STATUS_VERIFIED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_LIQ, GlobalSystemValues.STATUS_VERIFIED, makerId.Liq_Created_UserID);
                        //----------------------------- NOTIF----------------------------------
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_Liquidation_SS";
                    break;
                case "Delete":
                    int expStatus = _service.GetCurrentEntryStatus(entryID);
                    if (expStatus == GlobalSystemValues.STATUS_PENDING || expStatus == GlobalSystemValues.STATUS_REJECTED)
                    {
                        if (_service.deleteLiquidationEntry(entryID))
                        {
                            ViewBag.Success = 1;
                            //----------------------------- NOTIF----------------------------------
                            _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_LIQ, GlobalSystemValues.STATUS_DELETE, 0);
                            //----------------------------- NOTIF----------------------------------
                        }
                        else
                        {
                            ViewBag.Success = 0;
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE2;
                        ViewBag.Success = 0;
                    }

                    viewLink = "View_Liquidation_SS";
                    break;
                case "Reject":
                    if (_service.updateLiquidateStatus(entryID, GlobalSystemValues.STATUS_REJECTED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_LIQ, GlobalSystemValues.STATUS_REJECTED, makerId.Liq_Created_UserID);
                        //----------------------------- NOTIF----------------------------------
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_Liquidation_SS";
                    break;
                case "Reversal":
                    if (_service.updateLiquidateStatus(entryID, GlobalSystemValues.STATUS_REVERSED, int.Parse(GetUserID())))
                    {
                        _service.postLiq_SS(entryID, "R", int.Parse(GetUserID()));
                        ViewBag.Success = 1;
                        //----------------------------- NOTIF----------------------------------
                        _service.insertIntoNotif(intUser, GlobalSystemValues.TYPE_LIQ, GlobalSystemValues.STATUS_REVERSED, makerId.Liq_Created_UserID);
                        //----------------------------- NOTIF----------------------------------
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "View_Liquidation_SS";
                    break;
                default:
                    break;
            }

            //Entry was Edited, Deleted by user before clicking the approve/verify button by user, 
            //return error message then redirect to Home Index screen.
            if (GlobalSystemValues.MESSAGE == GlobalSystemValues.MESSAGE3)
            {
                return RedirectToAction("Index", "Home");
            }

            ssList = _service.getExpenseToLiqudate(entryID);
            ssList.accList = _service.getAccountList();
            ssList.accAllList = _service.getAccountListIncHist();
            if (command == "Modify")
            {
                ssList.vendorList = _service.getVendorList().OrderBy(x => x.Vendor_Name).ToList();
            }
            else
            {
                ssList.vendorList = _service.getAllVendorList();
            }
            
            foreach (var i in ssList.accAllList)
            {
                i.Account_Name = i.Account_No + " - " + i.Account_Name;
            }

            foreach (var i in ssList.LiquidationDetails)
            {
                i.screenCode = "Liquidation_SS";
            }

            var ccyPHP = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_PHP").Value));
            ssList.phpCurrID = ccyPHP.Curr_ID;
            ssList.phpCurrMasterID = ccyPHP.Curr_MasterID;
            ssList.phpAbbrev = ccyPHP.Curr_CCY_ABBR;
            var ccyYEN = _service.getCurrencyByMasterID(int.Parse(xelem.Element("CURRENCY_Yen").Value));
            ssList.yenCurrID = ccyYEN.Curr_ID;
            ssList.yenCurrMasterID = ccyYEN.Curr_MasterID;
            ssList.yenAbbrev = ccyYEN.Curr_CCY_ABBR;

            List<cvBirForm> birForms = new List<cvBirForm>();
            ssList.birForms = new List<cvBirForm>();
            foreach (var item in ssList.LiquidationDetails)
            {
                double grossOrig = item.gBaseRemarksDetails.Sum(x => x.amount);

                if (birForms.Any(x => x.ewt == item.ewtID && x.vendor == item.dtl_Ewt_Payor_Name_ID))
                {
                    int index = birForms.FindIndex(x => x.ewt == item.ewtID);
                    birForms[index].amount += grossOrig;
                }
                else
                {
                    cvBirForm temp = new cvBirForm
                    {
                        amount = grossOrig,
                        ewt = item.ewtID,
                        vat = item.vatID,
                        vendor = item.dtl_Ewt_Payor_Name_ID,
                        approver = ssList.approver,
                        date = ssList.createdDate
                    };
                    if (item.ewtID > 0)
                    {
                        birForms.Add(temp);
                    }
                }
            }
            if (birForms.Count() > 0)
            {
                ssList.birForms.AddRange(birForms);
            }

            //return View(viewLink, ssList);
            return RedirectToAction(viewLink, "Home", new { entryID = entryID });
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult CDD_IS_Liquidation(int entryID, int entryDtlID, string ccyAbbr)
        {
            string newFileName = "CDD_IS_Liqudation_" + ccyAbbr + "_" + DateTime.Now.ToString("MM-dd-yyyy_hhmmss") + ".xlsx";
            var expense = _service.getExpenseToLiqudate(entryID);
            var expenseDtl = expense.LiquidationDetails.Where(x => x.EntryDetailsID == entryDtlID).FirstOrDefault();

            ExcelGenerateService excelGenerate = new ExcelGenerateService();
            CDDISValuesVIewModel viewModel = new CDDISValuesVIewModel
            {
                VALUE_DATE = DateTime.Parse(expense.LiqEntryDetails.Liq_Created_Date.ToLongDateString()),
                REMARKS = "S" + expenseDtl.GBaseRemarks,
                CURRENCY = _service.getCurrency(expenseDtl.ccyID).Curr_MasterID
            };


            List<CDDISValueContentsViewModel> cddContents = new List<CDDISValueContentsViewModel>
            {
                new CDDISValueContentsViewModel
                {
                    AMOUNT = expenseDtl.liqInterEntity[2].Liq_Amount_1_2,
                },
                new CDDISValueContentsViewModel
                {
                    AMOUNT = expenseDtl.liqInterEntity[2].Liq_Amount_1_2,
                },
            };
            viewModel.CDDContents = cddContents;

            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            string excelTemplateName = (viewModel.CURRENCY == int.Parse(xelem.Element("CURRENCY_Yen").Value)) ? "CDDIS_Liq_Yen.xlsx" : "CDDIS_Liq_USD.xlsx";

            bool result = _service.UpdateCDDPrintingStatus(entryID, entryDtlID, GlobalSystemValues.TYPE_LIQ);

            return File(excelGenerate.ExcelCDDIS(viewModel, newFileName, excelTemplateName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", newFileName);

        }

        //------------------------------------------------------------------
        //[* ACCOUNT *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult SendEmail(ForgotPWViewModel model)
        {
            if (ModelState.IsValid)
            {
                _service.sendEmail(model);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Vendor" });
        }

        //------------------------------DM-------------------------------
        //[* ADMIN *]
        //[* PAYEE *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveVendor(List<DMVendorViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveVendor(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Vendor" });
        }
        [OnlineUserCheck]
        [HttpPost]
        [ExportModelState]
        public IActionResult RejVendor(List<DMVendorViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejVendor(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Vendor" });
        }
        //[* DEPARTMENT *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveDept(List<DMDeptViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveDept(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejDept(List<DMDeptViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejDept(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        //[* CHECK *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveCheck(List<DMCheckViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveCheck(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejCheck(List<DMCheckViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejCheck(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        //[* ACCOUNT *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveAccount(List<DMAccountViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveAccount(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejAccount(List<DMAccountViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejAccount(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        //[* VAT *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveVAT(List<DMVATViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveVAT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejVAT(List<DMVATViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejVAT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        //[* FBT *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveFBT(List<DMFBTViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveFBT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejFBT(List<DMFBTViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejFBT(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        //[* TR *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveTR(List<DMTRViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveTR(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_TR" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejTR(List<DMTRViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejTR(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_TR" });
        }
        //[* Currency *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveCurr(List<DMCurrencyViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveCurr(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejCurr(List<DMCurrencyViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejCurr(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
        }
        //[* Employee *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveEmp(List<DMEmpViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveEmp(model, userId);
            }
            var partialName = model[0].Emp_Acc_No == null ? "DMPartial_TempEmp" : "DMPartial_RegEmp";
            return RedirectToAction("DM", "Home", new { partialName = partialName });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejEmp(List<DMEmpViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejEmp(model, userId);
            }
            var partialName = model[0].Emp_Acc_No == null ? "DMPartial_TempEmp" : "DMPartial_RegEmp";
            return RedirectToAction("DM", "Home", new { partialName = partialName });
        }
        //[* CUSTOMER *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveCust(List<DMCustViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveCust(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Cust" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejCust(List<DMCustViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejCust(model, userId);
            }
            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Cust" });
        }
        //[* BIR CERT SIGNATORY*]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult ApproveBCS(List<DMBCSViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.approveBCS(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_BCS" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult RejBCS(List<DMBCSViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.rejBCS(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_BCS" });
        }
        //--------------------------------PENDING--------------------------------
        // [PAYEE]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddVendor_Pending(NewVendorListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addVendor_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Vendor" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditVendor_Pending(List<DMVendorViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editVendor_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Vendor" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteVendor_Pending(List<DMVendorViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteVendor_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Vendor" });
        }
        // [DEPARTMENT]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddDept_Pending(NewDeptListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addDept_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditDept_Pending(List<DMDeptViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editDept_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteDept_Pending(List<DMDeptViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteDept_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Dept" });
        }
        // [CHECK]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddCheck_Pending(NewCheckListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addCheck_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditCheck_Pending(List<DMCheckViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editCheck_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteCheck_Pending(List<DMCheckViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteCheck_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Check" });
        }
        // [ACCOUNT]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddAccount_Pending(NewAccountListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addAccount_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditAccount_Pending(List<DMAccountViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editAccount_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteAccount_Pending(List<DMAccountViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteAccount_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Acc" });
        }
        // [VAT]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddVAT_Pending(NewVATListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addVAT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditVAT_Pending(List<DMVATViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editVAT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteVAT_Pending(List<DMVATViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteVAT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_VAT" });
        }
        // [FBT]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddFBT_Pending(NewFBTListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addFBT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditFBT_Pending(List<DMFBTViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editFBT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteFBT_Pending(List<DMFBTViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteFBT_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_FBT" });
        }
        // [TR]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddTR_Pending(NewTRListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addTR_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_TR" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditTR_Pending(List<DMTRViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editTR_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_TR" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteTR_Pending(List<DMTRViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteTR_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_TR" });
        }
        // [Curr]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddCurr_Pending(NewCurrListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addCurr_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditCurr_Pending(List<DMCurrencyViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editCurr_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteCurr_Pending(List<DMCurrencyViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteCurr_Pending(model, userId);
            }

            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Curr" });
        }
        // [EMPLOYEE]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddEmp_Pending(NewEmpListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addEmp_Pending(model, userId);
            }

            var partialName = model.NewEmpVM[0].Emp_Acc_No == null ? "DMPartial_TempEmp" : "DMPartial_RegEmp";
            return RedirectToAction("DM", "Home", new { partialName = partialName });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditEmp_Pending(List<DMEmpViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editEmp_Pending(model, userId);
            }

            var partialName = model[0].Emp_Acc_No == null ? "DMPartial_TempEmp" : "DMPartial_RegEmp";
            return RedirectToAction("DM", "Home", new { partialName = partialName });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteEmp_Pending(List<DMEmpViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteEmp_Pending(model, userId);
            }

            var partialName = model[0].Emp_Acc_No == null ? "DMPartial_TempEmp" : "DMPartial_RegEmp";
            return RedirectToAction("DM", "Home", new { partialName = partialName });
        }
        // [CUSTOMER]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddCust_Pending(NewCustListViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addCust_Pending(model, userId);
            }
            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Cust" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditCust_Pending(List<DMCustViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editCust_Pending(model, userId);
            }
            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Cust" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteCust_Pending(List<DMCustViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteCust_Pending(model, userId);
            }
            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_Cust" });
        }
        // [BIR CERT SIGNATORY]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddBCS_Pending(NewBCSViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addBCS_Pending(model, userId);
            }
            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_BCS" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult EditBCS_Pending(DMBCS2ViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.editBCS_Pending(model, userId);
            }
            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_BCS" });
        }
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult DeleteBCS_Pending(List<DMBCSViewModel> model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.deleteBCS_Pending(model, userId);
            }
            return RedirectToAction("DM", "Home", new { partialName = "DMPartial_BCS" });
        }

        //[* USER *]
        [HttpPost]
        [ExportModelState]
        [OnlineUserCheck]
        public IActionResult AddEditUser(UserManagementViewModel model)
        {
            var userId = GetUserID();
            if (ModelState.IsValid)
            {
                _service.addUser(model, userId);
            }


            return RedirectToAction("UM", "Home");
        }

        //[* MISC *]

        //public void addNCC()
        //{
        //    FileService fs = new FileService();
        //    //fs.CreateFileAndFolder();
        //    fs.CopyFileToLocation("00283_martin.nina.png");
        //}

        [HttpPost]
        public IActionResult HomeReportValidation(HomeReportViewModel model)
        {
            List<String> errors = new List<String>();
            string format = "yyyy-M";


            if (model.ReportType != 0)
            {
                switch (model.ReportType)
                {
                    case ConstantData.HomeReportConstantValue.AST1000:
                        DateTime fromDate = DateTime.ParseExact(model.Year + "-" + model.Month, format, CultureInfo.InvariantCulture);
                        DateTime toDate = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
                        if (fromDate.Date > toDate.Date)
                        {
                            errors.Add("Year,Month(TO) must be later than Year,Month(FROM)");
                        }
                        break;
                }

                switch (model.PeriodOption)
                {
                    case 1:

                        break;

                    case 2:

                        break;
                    case 3:
                        if (model.PeriodFrom == DateTime.MinValue)
                        {
                            errors.Add("Period From input is required");
                        }
                        if (model.PeriodTo == DateTime.MinValue)
                        {
                            errors.Add("Period To input is required");
                        }
                        break;
                }
            }

            string errorFlag = "Valid";

            if(errors.Count > 0)
            {
                errorFlag = "Invalid";
            }

            var data = new JsonDataResult { Message = errorFlag, Items = errors };
            return Json(data);
        }

        public class JsonDataResult
        {
            public string Message { get; set; }
            public List<String> Items = new List<String>();
        }

        [HttpPost]
        [AcceptVerbs("GET")]
        public JsonResult getVendorTRList(int vendorID)
        {
            var vend = _service.getVendor(vendorID);
            var trList = _service.getVendorTaxList(vend.Vendor_MasterID);

            return Json(trList.ToList());
        }
        [HttpPost]
        [AcceptVerbs("GET")]
        public JsonResult getVendorTaxRate(int vendorID)
        {
            var trList = _service.getVendorTaxList(_service.getVendor(vendorID).Vendor_MasterID);
            foreach (var i in trList)
            {
                i.TR_WT_Title = i.TR_Tax_Rate + "%";
            }
            return Json(trList.ToList());
        }
        [HttpPost]
        [AcceptVerbs("GET")]
        public JsonResult getAllTRList()
        {
            var trList = _service.getAllTaxRate();

            return Json(trList.ToList());
        }
        [HttpPost]
        [AcceptVerbs("GET")]
        public JsonResult getVendorVatList(int vendorID)
        {
            var vend = _service.getVendor(vendorID);
            var vatList = _service.getVendorVatList(vend.Vendor_MasterID);

            return Json(vatList.ToList());
        }
        [HttpPost]
        [AcceptVerbs("GET")]
        public JsonResult getAllVatList()
        {
            var vatList = _service.getAllVat();

            return Json(vatList.ToList());
        }
        [AcceptVerbs("GET")]
        public JsonResult getAccount(int masterID)
        {
            var acc = _service.getAccountByMasterID(masterID);

            return Json(acc);
        }
        [AcceptVerbs("GET")]
        public JsonResult getCurrency(int masterID)
        {
            var acc = _service.getCurrencyByMasterID(masterID);

            return Json(acc);
        }
        [AcceptVerbs("GET")]
        public JsonResult getAllAccount()
        {
            var acc = _service.getAccountList();

            return Json(acc);
        }
        [AcceptVerbs("GET")]
        public JsonResult UpdateCDDPrintingStatus(int entryID)
        {
            bool result = _service.UpdateCDDPrintingStatus(entryID);

            if (result)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        [AcceptVerbs("GET")]
        public JsonResult GetInitCurrencyOfAccount(int acc_id)
        {
            return Json(_service.getCurrencyByMasterID(_service.getAccount(acc_id).Account_Currency_MasterID).Curr_ID);
        }
        /// <summary>
        /// Generate Voucher for CV Module
        /// </summary>
        /// <param name="model"></param>
        /// <returns> View</returns>
        [AcceptVerbs("GET")]
        [HttpPost]
        public IActionResult VoucherCV(EntryCVViewModelList model)
        {
            VoucherViewModelList vvm = GenerateVoucherViewModelCV(model);

            return View(GlobalSystemValues.VOUCHER_LAYOUT, vvm);
        }
        [AcceptVerbs("GET")]
        public IActionResult VoucherViewCV(int ExpenseID)
        {
            EntryCVViewModelList model = _service.getExpense(ExpenseID);
            VoucherViewModelList vvm = GenerateVoucherViewModelCV(model);

            return View(GlobalSystemValues.VOUCHER_LAYOUT, vvm);
        }
        private VoucherViewModelList GenerateVoucherViewModelCV(EntryCVViewModelList model) {
            VoucherViewModelList vvm = new VoucherViewModelList();

            XElement accXML = XElement.Load("wwwroot/xml/GlobalAccounts.xml");
            XElement xelem = XElement.Load("wwwroot/xml/ReportHeader.xml");

            int officeID = int.Parse(accXML.Element("HOUSE_RENT").Value);
            //string dateNow = DateTime.Now.ToString("MM-dd-yyyy_hhmmsstt"); // ORIGINAL
            vvm.date = DateTime.Now.ToString("MM-dd-yyyy");

            vvm.headvm.Header_Logo = "";
            vvm.headvm.Header_Name = xelem.Element("NAME").Value;
            vvm.headvm.Header_TIN = xelem.Element("TIN").Value;
            vvm.headvm.Header_Address = xelem.Element("ADDRESS").Value;

            vvm.maker = GetUserID();
            if(model.expenseId != null)
                vvm.voucherNo = _service.getVoucherNo(1,model.expenseDate,int.Parse(model.expenseId));
            vvm.payee = _service.getVendorName(model.vendor, model.payee_type);
            vvm.checkNo = model.checkNo;
            vvm.approver = model.approver;
            vvm.verifier_1 = model.verifier_1;
            vvm.verifier_2 = model.verifier_2;
            vvm.isFbt = false;

            List<ewtAmtList> _ewtList = new List<ewtAmtList>();

            vvm.accountCredit.Add(new accountList
            {
                account = "BDO MNL"
            });

            double tax_vat = 0.00;
            double tax_gross = 0.00;
            double amountGross = 0.00;
            double amountCredit = 0.00;

            foreach (var inputItem in model.EntryCV)
            {
                foreach (var particular in inputItem.gBaseRemarksDetails)
                {
                    particulars temp = new particulars();

                    if (vvm.particulars.Any(x => x.documentType.Trim() == particular.docType
                                              && x.invoiceNo.Trim() == particular.invNo))
                    {
                        int index = vvm.particulars.FindIndex(x => x.documentType.Trim() == particular.docType.Trim()
                            && x.invoiceNo.Trim() == particular.invNo.Trim());

                        vvm.particulars[index].amount += particular.amount;
                    }
                    else
                    {
                        temp.documentType = particular.docType;
                        temp.invoiceNo = particular.invNo;
                        temp.description = particular.desc;
                        temp.amount = particular.amount;

                        vvm.particulars.Add(temp);
                    }
                }

                vvm.accountsDebit.Add(new accountList
                {
                    account = _service.GetAccountName(inputItem.account),
                    amount = inputItem.debitGross,
                    accountid = inputItem.account
                });

                amountGross += inputItem.debitGross;
                amountCredit += inputItem.debitGross;

                double _vat = 0;

                if (inputItem.chkEwt || inputItem.chkVat)
                    tax_gross += inputItem.debitGross;

                if (inputItem.chkVat)
                {
                    _vat = _service.getVat(inputItem.vat);
                    tax_vat += Mizuho.round((inputItem.debitGross / (1 + _vat)) * _vat, 2);
                }

                if (inputItem.chkEwt)
                {
                    double _ewt = Mizuho.round(_service.GetEWTValue(inputItem.ewt),4);
                    double _ewtAmount = Mizuho.round((inputItem.debitGross / (1 + _vat)) * _ewt, 2);
                    if (_ewtList.Any(x => x.ewt == _ewt))
                    {
                        int index = _ewtList.FindIndex(x => x.ewt == _ewt);
                        _ewtList[index].ewtAmt += _ewtAmount;
                    }
                    else
                    {
                        _ewtList.Add(new ewtAmtList
                        {
                            ewt = _ewt * 100.00,
                            ewtAmt = _ewtAmount
                        }
                        );
                    }
                    amountCredit -= _ewtAmount;
                    vvm.accountCredit[0].amount += inputItem.debitGross - _ewtAmount;
                    vvm.accountCredit.Add(new accountList {
                        account = (_ewt * 100).ToString(),
                        amount = _ewtAmount
                    });
                }

                if (inputItem.fbt)
                {
                    vvm.isFbt = true;

                    if(_service.getAccount(inputItem.account).Account_MasterID == officeID)
                    {
                        vvm.fbtAmount += Mizuho.round((((inputItem.debitGross * .50) /.65)*.35), 2);
                    }
                    else
                    {
                        vvm.fbtAmount += Mizuho.round(((inputItem.debitGross/.65)*.35), 2);
                    }
                }
            }

            foreach (var item in _ewtList)
                vvm.taxWithheld += item.ewtAmt;

            vvm.amountString = ConvertToWord.ToWord(amountCredit);

            vvm.vatAmtList.AddRange(_ewtList);
            vvm.taxInfo_vat = tax_vat;
            vvm.taxInfo_gross = tax_gross;
            vvm.taxInfo_taxBase = tax_gross - tax_vat;
            vvm.amountCredit = amountCredit;
            vvm.amountGross = amountGross;

            return vvm;
        }
        [AcceptVerbs("GET")]
        [HttpPost]
        public IActionResult VoucherDDV(EntryDDVViewModelList model)
        {
            VoucherViewModelList vvm = GenerateVoucherViewModelDDV(model);

            return View(GlobalSystemValues.VOUCHER_LAYOUT, vvm);
        }
        [AcceptVerbs("GET")]
        public IActionResult VoucherViewDDV(int ExpenseID)
        {
            EntryDDVViewModelList model = _service.getExpenseDDV(ExpenseID);
            VoucherViewModelList vvm = GenerateVoucherViewModelDDV(model);

            return View(GlobalSystemValues.VOUCHER_LAYOUT, vvm);
        }
        private VoucherViewModelList GenerateVoucherViewModelDDV(EntryDDVViewModelList model)
        {
            VoucherViewModelList vvm = new VoucherViewModelList();

            XElement accXML = XElement.Load("wwwroot/xml/GlobalAccounts.xml");
            XElement xelem = XElement.Load("wwwroot/xml/ReportHeader.xml");

            int officeID = int.Parse(accXML.Element("HOUSE_RENT").Value);
            //string dateNow = DateTime.Now.ToString("MM-dd-yyyy_hhmmsstt"); // ORIGINAL
            vvm.date = DateTime.Now.ToString("MM-dd-yyyy");

            vvm.headvm.Header_Logo = "";
            vvm.headvm.Header_Name = xelem.Element("NAME").Value;
            vvm.headvm.Header_TIN = xelem.Element("TIN").Value;
            vvm.headvm.Header_Address = xelem.Element("ADDRESS").Value;

            vvm.maker = GetUserID();
            if (model.expenseId != null)
                vvm.voucherNo = _service.getVoucherNo(1, model.expenseDate, int.Parse(model.expenseId));
            vvm.payee = _service.getVendorName(model.vendor, model.payee_type);
            vvm.checkNo = model.checkNo;
            vvm.approver = model.approver;
            vvm.verifier_1 = model.verifier_1;
            vvm.verifier_2 = model.verifier_2;
            vvm.isFbt = false;

            List<ewtAmtList> _ewtList = new List<ewtAmtList>();

            vvm.accountCredit.Add(new accountList
            {
                account = "BDO MNL"
            });

            double tax_vat = 0.00;
            double tax_gross = 0.00;
            double amountGross = 0.00;
            double amountCredit = 0.00;

            foreach (var inputItem in model.EntryDDV)
            {
                foreach (var particular in inputItem.gBaseRemarksDetails)
                {
                    particulars temp = new particulars();

                    if (vvm.particulars.Any(x => x.documentType.Trim() == particular.docType
                                              && x.invoiceNo.Trim() == particular.invNo))
                    {
                        int index = vvm.particulars.FindIndex(x => x.documentType.Trim() == particular.docType.Trim()
                            && x.invoiceNo.Trim() == particular.invNo.Trim());

                        vvm.particulars[index].amount += particular.amount;
                    }
                    else
                    {
                        temp.documentType = particular.docType;
                        temp.invoiceNo = particular.invNo;
                        temp.description = particular.desc;
                        temp.amount = particular.amount;

                        vvm.particulars.Add(temp);
                    }
                }

                vvm.accountsDebit.Add(new accountList
                {
                    account = _service.GetAccountName(inputItem.account),
                    amount = inputItem.debitGross,
                    accountid = inputItem.account
                });

                amountGross += inputItem.debitGross;
                amountCredit += inputItem.debitGross;

                double _vat = 0;

                if (inputItem.chkEwt || inputItem.chkVat)
                    tax_gross += inputItem.debitGross;

                if (inputItem.chkVat)
                {
                    _vat = _service.getVat(inputItem.vat);
                    tax_vat += Mizuho.round((inputItem.debitGross / (1 + _vat)) * _vat, 2);
                }

                if (inputItem.chkEwt)
                {
                    double _ewt = Mizuho.round(_service.GetEWTValue(inputItem.ewt), 4);
                    double _ewtAmount = Mizuho.round((inputItem.debitGross / (1 + _vat)) * _ewt, 2);
                    if (_ewtList.Any(x => x.ewt == _ewt))
                    {
                        int index = _ewtList.FindIndex(x => x.ewt == _ewt);
                        _ewtList[index].ewtAmt += _ewtAmount;
                    }
                    else
                    {
                        _ewtList.Add(new ewtAmtList
                        {
                            ewt = _ewt * 100,
                            ewtAmt = _ewtAmount
                        }
                        );
                    }
                    amountCredit -= _ewtAmount;
                    vvm.accountCredit[0].amount += inputItem.debitGross - _ewtAmount;
                    vvm.accountCredit.Add(new accountList
                    {
                        account = (_ewt * 100).ToString(),
                        amount = _ewtAmount
                    });
                }

                if (inputItem.fbt)
                {
                    vvm.isFbt = true;

                    if (_service.getAccount(inputItem.account).Account_MasterID == officeID)
                    {
                        vvm.fbtAmount += Mizuho.round((((inputItem.debitGross * .50) / .65) * .35), 2);
                    }
                    else
                    {
                        vvm.fbtAmount += Mizuho.round(((inputItem.debitGross / .65) * .35), 2);
                    }
                }
            }

            foreach (var item in _ewtList)
                vvm.taxWithheld += item.ewtAmt;

            vvm.amountString = ConvertToWord.ToWord(amountCredit);

            vvm.vatAmtList.AddRange(_ewtList);
            vvm.taxInfo_vat = tax_vat;
            vvm.taxInfo_gross = tax_gross;
            vvm.taxInfo_taxBase = tax_gross - tax_vat;
            vvm.amountCredit = amountCredit;
            vvm.amountGross = amountGross;

            return vvm;
        }
    }
}