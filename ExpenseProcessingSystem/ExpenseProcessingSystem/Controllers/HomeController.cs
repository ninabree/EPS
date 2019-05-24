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
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExpenseProcessingSystem.ViewModels.Entry;
using System.Diagnostics;

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
        //to access resources
        private readonly IStringLocalizer<HomeController> _localizer;


        public HomeController(IHttpContextAccessor httpContextAccessor, EPSDbContext context, IHostingEnvironment hostingEnvironment, IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _service = new HomeService(_httpContextAccessor, _context, this.ModelState, hostingEnvironment);
            _sortService = new SortService();
        }
        public ReportHeaderViewModel GetHeaderInfo()
        {
            ReportHeaderViewModel headerVM = new ReportHeaderViewModel {
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
        public IActionResult Index(HomeIndexViewModel vm, string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            int? pg = (page == null) ? 1 : int.Parse(page);

            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NotifTypeStatSortParm"] = String.IsNullOrEmpty(sortOrder) ? "notif_type_status" : "";
            ViewData["NotifAppIDSortParm"] = sortOrder == "notif_app_id_desc" ? "notif_app_id" : "notif_app_id_desc";
            ViewData["NotifMessageSortParm"] = sortOrder == "notif_message_desc" ? "notif_message" : "notif_message_desc";
            ViewData["NotifApproverSortParm"] = sortOrder == "notif_approvr_desc" ? "notif_approvr" : "notif_approvr_desc";
            ViewData["NotifLastUpdatedSortParm"] = sortOrder == "notif_last_updte_desc" ? "notif_last_updte" : "notif_last_updte_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            FiltersViewModel filters = new FiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (FiltersViewModel)TempData["filters"];
            }
            FiltersViewModel filterVM = new FiltersViewModel();
            if (vm.Filters != null)
            {
                if (vm.Filters.NotifFil != null)
                {
                    //Notifications
                    _session.SetString("Notif_Last_Updated", vm.Filters.NotifFil.Notif_Last_Updated.ToShortDateString() ?? "");
                    _session.SetString("NotifFil_Message", vm.Filters.NotifFil.NotifFil_Message ?? "");
                    _session.SetString("NotifFil_Status", vm.Filters.NotifFil.NotifFil_Status ?? "");
                    _session.SetString("NotifFil_Verifier_Approver_Name", vm.Filters.NotifFil.NotifFil_Verifier_Approver_Name ?? "");
                }
            }
            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateNotif(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            HomeIndexViewModel VM = new HomeIndexViewModel()
            {
                Filters = filters,
                NotifList = PaginatedList<HomeNotifViewModel>.CreateAsync(
                        (sortedVals.list).Cast<HomeNotifViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Pending(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var userId = GetUserID();

            HomeIndexViewModel vm = new HomeIndexViewModel();

            vm.GeneralPendingList = _service.getPending(int.Parse(userId));

            return View(vm);
        }
        [OnlineUserCheck]
        public IActionResult History(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var role = _service.getUserRole(GetUserID());
            if (role == "admin")
            {
                return RedirectToAction("UM");
            }
            HomeIndexViewModel vm = new HomeIndexViewModel();
            return View(vm);
        }
        //Home Screen Block End-----------------------------------------------------------------------------------

        public IActionResult Entry()
        {
            return View();
        }
        [OnlineUserCheck]
        public IActionResult Close(string sortOrder, string currentFilter, string searchString, int? page)
        {
            return View();
        }
        [OnlineUserCheck]
        [ImportModelState]
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
                    _session.SetString("PF_TIN", vm.DMFilters.PF.PF_TIN.ToString() ?? "0");
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
                    _session.SetString("CKF_Input_Date", vm.DMFilters.CKF.CKF_Input_Date.ToString() ?? new DateTime().ToString());
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
                    _session.SetString("AF_No", vm.DMFilters.AF.AF_No.ToString() ?? "0");
                    _session.SetString("AF_Cust", vm.DMFilters.AF.AF_Cust ?? "");
                    _session.SetString("AF_Div", vm.DMFilters.AF.AF_Div ?? "");
                    _session.SetString("AF_Group", vm.DMFilters.AF.AF_Group ?? "");
                    _session.SetString("AF_FBT", vm.DMFilters.AF.AF_FBT ?? "0");
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
                    _session.SetString("FF_Tax_Rate", vm.DMFilters.FF.FF_Tax_Rate.ToString() ?? "0");
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
                    _session.SetString("TR_Tax_Rate", vm.DMFilters.TF.TR_Tax_Rate.ToString() ?? "0");
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
                    _session.SetString("EMF_Type", vm.DMFilters.EMF.EMF_Type ?? "");
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
                    _session.SetString("AGF_Status", vm.DMFilters.AGF.AGF_Status ?? "");
                }
            }
            return View();
        }

        //------------------------------------------------------------------
        //[* REPORT *]
        [OnlineUserCheck]
        public IActionResult Report()
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
                MonthList = ConstantData.HomeReportConstantValue.GetMonthList(),
                FileFormatList = ConstantData.HomeReportConstantValue.GetFileFormatList(),
                YearList = ConstantData.HomeReportConstantValue.GetYearList(),
                YearSemList = ConstantData.HomeReportConstantValue.GetYearList(),
                SemesterList = ConstantData.HomeReportConstantValue.GetSemesterList(),
                PeriodOptionList = ConstantData.HomeReportConstantValue.GetPeriodOptionList(),
                PeriodFrom = Convert.ToDateTime(ConstantData.HomeReportConstantValue.DateToday),
                PeriodTo = Convert.ToDateTime(ConstantData.HomeReportConstantValue.DateToday)
            };

            //Return ViewModel
            return View(reportViewModel);
        }

        //Populate the Report sub-type list to dropdownlist depends on the selected Report Type
        [AcceptVerbs("GET")]
        public JsonResult GetReportSubType(string ReportTypeID)
        {
            if (!string.IsNullOrWhiteSpace(ReportTypeID))
            {
                var ReportSubTypes = ConstantData.HomeReportConstantValue.GetReportSubTypeData().Where(m => m.ParentTypeId == Convert.ToInt32(ReportTypeID)).ToList();

                return Json(ReportSubTypes);
            }
            return null;
        }

        //[ExportModelState]
        public IActionResult GenerateFilePreview(HomeReportViewModel model)
        {
            string layoutName = "";
            string fileName = "";
            //string dateNow = DateTime.Now.ToString("MM-dd-yyyy_hhmmsstt"); // ORIGINAL
            string dateNow = DateTime.Now.ToString("MM-dd-yyyy_hhmmss");
            string pdfFooterFormat = "";
            ReportHeaderViewModel headerVM = new ReportHeaderViewModel();

            headerVM.Header_Logo = "";
            headerVM.Header_Name = "Mizuho Bank Ltd., Manila Branch";
            headerVM.Header_TIN = "004-669-467-000";
            headerVM.Header_Address = "25th Floor, The Zuellig Building, Makati Avenue corner Paseo de Roxas, Makati City";

            //Model for data retrieve from Database
            HomeReportDataFilterViewModel data = null;

            //Assign variables and Data to corresponding Report Type
            switch (model.ReportType)
            {
                //For Alphalist of Payees Subject to Withholding Tax (Monthly)
                case ConstantData.HomeReportConstantValue.APSWT_M:

                    fileName = "AlphalistOfPayeesSubjectToWithholdingTax_Monthly_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
                    pdfFooterFormat = ConstantData.HomeReportConstantValue.PdfFooter1;

                    model.MonthName = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputAPSWT_M = _service.GetAPSWT_MData(model.Month, model.Year),
                        HomeReportFilter = model,
                        ReportHeaderVM = headerVM
                    };
                    break;

                //For Alphalist of Suppliers by top 10000 corporation (Semestral)
                case ConstantData.HomeReportConstantValue.AST1000_S:
                    fileName = "AlphalistOfSuppliersByTop10000Corporation_Semestral_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
                    pdfFooterFormat = ConstantData.HomeReportConstantValue.PdfFooter2;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputAST1000 = _service.GetAST1000_SData(model.YearSem, model.Semester),
                        HomeReportFilter = model,
                    };
                    break;

                //For Alphalist of Suppliers by top 10000 corporation (Annual)
                case ConstantData.HomeReportConstantValue.AST1000_A:
                    fileName = "AlphalistOfSuppliersByTop10000Corporation_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
                    pdfFooterFormat = ConstantData.HomeReportConstantValue.PdfFooter2;
                    model.MonthName = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName;
                    model.MonthNameTo = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputAST1000 = _service.GetAST1000_AData(model.Year, model.Month, model.YearTo, model.MonthTo),
                        HomeReportFilter = model,
                    };
                    break;

                //For Actual Budget Report
                case ConstantData.HomeReportConstantValue.ActualBudgetReport:
                    fileName = "ActualBudgetReport_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
                    pdfFooterFormat = String.Empty;

                    model.MonthName = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName;

                    IEnumerable<HomeReportActualBudgetModel> testtest = _service.GetActualReportData(model.Month, model.Year);

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputActualBudget = _service.GetActualReportData(model.Month, model.Year),
                        HomeReportFilter = model
                    };
                    break;

                case ConstantData.HomeReportConstantValue.WTS:
                    fileName = "WithholdingTaxSummary_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
                    pdfFooterFormat = ConstantData.HomeReportConstantValue.PdfFooter2;
                    data = new HomeReportDataFilterViewModel();
                    //Get the necessary data from Database
                    switch (model.PeriodOption)
                    {
                        case 1:
                            data = new HomeReportDataFilterViewModel
                            {
                                HomeReportOutputWTS = ConstantData.TEMP_HomeReportWTSDummyData.GetTEMP_HomeReportWTSOutputModelData_Month(model.Year, model.Month,
                                    ConstantData.TEMP_HomeReportWTSDummyData.GetTEMP_HomeReportWTSOutputModelData(), model.ReportSubType),
                                HomeReportFilter = model
                            };
                            model.ReportFrom = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName
                                                + " " + model.Year;
                            model.ReportTo = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.MonthTo).Single().MonthName
                                                + " " + model.YearTo;
                            break;
                        case 3:
                            data = new HomeReportDataFilterViewModel
                            {
                                HomeReportOutputWTS = ConstantData.TEMP_HomeReportWTSDummyData.GetTEMP_HomeReportWTSOutputModelData_Period(model.PeriodFrom, model.PeriodTo,
                                    ConstantData.TEMP_HomeReportWTSDummyData.GetTEMP_HomeReportWTSOutputModelData(), model.ReportSubType),
                                HomeReportFilter = model
                            };
                            model.ReportFrom = model.PeriodFrom.ToShortDateString();
                            model.ReportTo = model.PeriodTo.ToShortDateString();
                            break;
                    }
                    break;
            }

            if (model.FileFormat == ConstantData.HomeReportConstantValue.EXCELID)
            {
                ExcelGenerateService excelGenerate = new ExcelGenerateService();
                fileName = fileName + ".xlsx";

                //Return Excel file
                return File(excelGenerate.ExcelGenerateData(layoutName, fileName, data), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else if (model.FileFormat == ConstantData.HomeReportConstantValue.PDFID)
            {
                //Return PDF file
                return OutputPDF(ConstantData.HomeReportConstantValue.ReportPdfPrevLayoutPath, layoutName, data, fileName, pdfFooterFormat);
            }
            else if (model.FileFormat == ConstantData.HomeReportConstantValue.PreviewID)
            {
                string pdfLayoutFilePath = ConstantData.HomeReportConstantValue.ReportPdfPrevLayoutPath + layoutName;

                //Return Preview
                return View(pdfLayoutFilePath, data);
            }

            //Temporary return
            return View("Report");
        }

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

        //[* REPORT *]
        //------------------------------------------------------------------

        //------------------------------------------------------------------
        //[* BUDGET MONITORING *]
        [OnlineUserCheck]
        [ImportModelState]
        public IActionResult BM(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AccountCodeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "acc_code" : "";
            ViewData["AccountGroupSortParm"] = sortOrder == "acc_group_desc" ? "acc_group" : "acc_group_desc";
            ViewData["GBaseAccountCodeSortParm"] = sortOrder == "gbase_acc_desc" ? "gbase_acc" : "gbase_acc_desc";
            ViewData["BudgetSortParm"] = sortOrder == "budget_desc" ? "budget" : "budget_desc";
            ViewData["CurrentBudgetSortParm"] = sortOrder == "curr_budget_desc" ? "curr_budget" : "curr_budget_desc";
            ViewData["ApproverIDSortParm"] = sortOrder == "approval_id_desc" ? "approval_id" : "approval_id_desc";
            ViewData["LastBudgetApprovalSortParm"] = sortOrder == "last_budget_approval_desc" ? "last_budget_approval" : "last_budget_approval_desc";

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

        //[* BUDGET MONITORING *]
        //------------------------------------------------------------------

        [OnlineUserCheck]
        [ImportModelState]
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
        public IActionResult Entry_CV()
        {
            var userId = GetUserID();

            EntryCVViewModelList viewModel = new EntryCVViewModelList();
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            viewModel.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            viewModel.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            viewModel.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
            viewModel.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            viewModel.systemValues.acc = _service.getAccDetailsEntry();

            viewModel.expenseYear = DateTime.Today.Year.ToString();
            viewModel.expenseDate = DateTime.Today;
            //viewModel.vendor = 2;
            viewModel.EntryCV.Add(new EntryCVViewModel());
            return View(viewModel);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult AddNewCV(EntryCVViewModelList EntryCVViewModelList)
        {
            var userId = GetUserID();

            EntryCVViewModelList cvList = new EntryCVViewModelList();
            int id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()),GlobalSystemValues.TYPE_CV);
            ModelState.Clear();
            if (id > -1) {
                cvList = _service.getExpense(id);
                List<SelectList> listOfSysVals = _service.getEntrySystemVals();
                cvList.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
                cvList.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
                cvList.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
                cvList.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
                cvList.systemValues.acc = _service.getAccDetailsEntry();
                ViewBag.Status = cvList.status;
            }

            return View("Entry_CV_ReadOnly", cvList);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult VerAppModCV(int entryID, string command)
        {
            var userId = GetUserID();

            string viewLink = "Entry_CV";
            EntryCVViewModelList cvList;

            switch (command)
            {
                case "Modify":
                    viewLink = "Entry_CV";
                    break;
                case "approver":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_CV_ReadOnly";
                    break;
                case "verifier":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_VERIFIED))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_CV_ReadOnly";
                    break;
                case "Reject": break;
                default:
                    break;
            }

            cvList = _service.getExpense(entryID);

            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            cvList.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            cvList.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            cvList.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
            cvList.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            cvList.systemValues.acc = _service.getAccDetailsEntry();

            return View(viewLink, cvList);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult View_CV(int entryID)
        {
            var userId = GetUserID();

            EntryCVViewModelList cvList;
            cvList = _service.getExpense(entryID);
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            cvList.systemValues.vendors = listOfSysVals[0];
            cvList.systemValues.dept = listOfSysVals[1];
            cvList.systemValues.currency = listOfSysVals[2];
            cvList.systemValues.ewt = listOfSysVals[3];
            cvList.systemValues.acc = _service.getAccDetailsEntry();

            return View("Entry_CV_ReadOnly", cvList);
        }

        //Expense Entry Check Voucher Block End=========================================================================
        //------------------------------------------------------------------
        //[* Entry Direct Deposit *]
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Entry_DDV()
        {
            var userId = GetUserID();

            EntryDDVViewModelList viewModel = new EntryDDVViewModelList();
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();
            viewModel.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            viewModel.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            viewModel.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
            viewModel.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            viewModel.systemValues.acc = _service.getAccDetailsEntry();

            viewModel.expenseYear = DateTime.Today.Year.ToString();
            viewModel.expenseDate = DateTime.Today;
            //viewModel.vendor = 2;
            viewModel.EntryDDV.Add(new EntryDDVViewModel { interDetails = new List<DDVInterEntityViewModel> { new DDVInterEntityViewModel()} });
            return View(viewModel);
        }
        public IActionResult AddNewDDV(EntryDDVViewModelList EntryDDVViewModelList)
        {
            var userId = GetUserID();

            EntryDDVViewModelList ddvList = new EntryDDVViewModelList();
            int id = _service.addExpense_DDV(EntryDDVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_CV);
            ModelState.Clear();
            if (id > -1)
            {
                ddvList = _service.getExpenseDDV(id);
                List<SelectList> listOfSysVals = _service.getCheckEntrySystemVals();
                ddvList.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
                ddvList.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
                ddvList.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
                ddvList.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
                ddvList.systemValues.acc = _service.getAccDetailsEntry();
                ViewBag.Status = ddvList.status;
            }

            return View("Entry_DDV_ReadOnly", ddvList);
        }
        //------------------------------------------------------------------
        //[* Entry Petty Cash *]
        [OnlineUserCheck]
        [ImportModelState]
        public IActionResult Entry_PCV()
        {
            var userId = GetUserID();

            EntryCVViewModelList viewModel = new EntryCVViewModelList();
            List<SelectList> listOfSysVals = _service.getCheckEntrySystemVals();
            viewModel.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            viewModel.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            viewModel.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            viewModel.systemValues.acc = _service.getAccDetailsEntry();

            viewModel.expenseYear = DateTime.Today.Year.ToString();
            viewModel.expenseDate = DateTime.Today;

            viewModel.EntryCV.Add(new EntryCVViewModel());

            return View(viewModel);
        }

        public EntryCVViewModelList PopulateEntryCV(EntryCVViewModelList viewModel)
        {
            List<SelectList> listOfSysVals = _service.getCheckEntrySystemVals();
            viewModel.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            viewModel.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            viewModel.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
            viewModel.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            viewModel.systemValues.acc = _service.getAccDetailsEntry();

            viewModel.expenseYear = DateTime.Today.Year.ToString();
            viewModel.expenseDate = DateTime.Today;

            return viewModel;
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        [ExportModelState]
        public IActionResult AddNewPCV(EntryCVViewModelList EntryCVViewModelList)
        {
            var userId = GetUserID();

            if (!ModelState.IsValid)
            {
                return View("Entry_PCV", PopulateEntryCV(EntryCVViewModelList));
            }

            EntryCVViewModelList pcList = new EntryCVViewModelList();
            int id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_PC);
            ModelState.Clear();
            if (id > -1)
            {
                pcList = _service.getExpense(id);
                List<SelectList> listOfSysVals = _service.getCheckEntrySystemVals();
                pcList.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
                pcList.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
                pcList.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
                pcList.systemValues.acc = _service.getAccDetailsEntry();
                ViewBag.Status = pcList.status;
            }

            return View("Entry_PCV_ReadOnly", pcList);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult VerAppModPCV(int entryID, string command)
        {
            var userId = GetUserID();

            string viewLink = "Entry_PCV";
            EntryCVViewModelList pcvList;

            switch (command)
            {
                case "Modify":
                    viewLink = "Entry_PCV";
                    break;
                case "approver":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_PCV_ReadOnly";
                    break;
                case "verifier":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_VERIFIED))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_PCV_ReadOnly";
                    break;
                case "Reject": break;
                default:
                    break;
            }

            pcvList = _service.getExpense(entryID);

            List<SelectList> listOfSysVals = _service.getCheckEntrySystemVals();
            pcvList.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            pcvList.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            pcvList.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
            pcvList.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            pcvList.systemValues.acc = _service.getAccDetailsEntry();

            return View(viewLink, pcvList);
        }
        //[* Entry Petty Cash *]
        //------------------------------------------------------------------

        //------------------------------------------------------------------
        //[* Entry Cash Advance(SS) *]
        [OnlineUserCheck]
        public IActionResult Entry_SS()
        {
            var role = _service.getUserRole(_session.GetString("UserID"));
            if (role == GlobalSystemValues.ROLE_ADMIN)
            {
                return RedirectToAction("UM");
            }

            EntryCVViewModelList viewModel = new EntryCVViewModelList();
            List<SelectList> listOfSysVals = _service.getEntrySystemVals();

            viewModel.systemValues.vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR];
            viewModel.systemValues.dept = listOfSysVals[GlobalSystemValues.SELECT_LIST_DEPARTMENT];
            viewModel.systemValues.ewt = listOfSysVals[GlobalSystemValues.SELECT_LIST_TAXRATE];
            viewModel.systemValues.currency = listOfSysVals[GlobalSystemValues.SELECT_LIST_CURRENCY];
            viewModel.systemValues.acc = _service.getAccDetailsEntry();

            viewModel.expenseYear = DateTime.Today.Year.ToString();
            viewModel.expenseDate = DateTime.Today;

            viewModel.EntryCV.Add(new EntryCVViewModel());
            return View(viewModel);
        }
        //[* Entry Cash Advance(SS) *]
        //------------------------------------------------------------------

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
        //[ExportModelState]
        public IActionResult HomeReportValidation(HomeReportViewModel model)
        {
            List<String> errors = new List<String>();
            string format = "yyyy-M";
            DateTime fromDate = DateTime.ParseExact(model.Year + "-" + model.Month, format, CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);


            if (model.ReportType != 0)
            {
                switch (model.ReportType)
                {
                    case ConstantData.HomeReportConstantValue.AST1000_S:
                        if (fromDate > toDate)
                        {
                            errors.Add("Year,Month(TO) must be later than Year,Month(FROM)");
                        }
                        break;
                    case ConstantData.HomeReportConstantValue.AST1000_A:
                        if (fromDate > toDate)
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

        public IEnumerable<HomeReportActualBudgetModel> GetDummyActualBudgetData()
        {
            List<HomeReportActualBudgetModel> actualBudgetData = new List<HomeReportActualBudgetModel>();

            int filterYear = 2019;  //Filter value from screen
            int filterMonth = 6;    //Filter value from screen
            int termYear = 2019;    //Create logic to get term year
            int termMonth = 4;      //Create logic to get term month
            double budgetBalance;
            double totalExpenseThisTermToPrevMonthend;
            double subTotal;
            string format = "yyyy-M";
            var accountCategory = ConstantData.TEMP_HomeReportActualBudgetReportDummyData.GetAcountCategory().OrderBy(c => c.AccountID);

            DateTime startDT;
            DateTime endDT;

            foreach(var a in accountCategory)
            {
                startDT = DateTime.ParseExact(termYear + "-" + termMonth, format, CultureInfo.InvariantCulture);
                endDT = DateTime.ParseExact(filterYear + "-" + filterMonth, format, CultureInfo.InvariantCulture);
                subTotal = 0.00;
                totalExpenseThisTermToPrevMonthend = 0.00;

                //Get current Budget of selected term
                var budgetMonitoringData = ConstantData.TEMP_HomeReportActualBudgetReportDummyData.GetBudgetMonitoringData().Where(c => c.AccountID == a.AccountID && c.TermOfBudget.Year == termYear && c.TermOfBudget.Month == termMonth).SingleOrDefault();
                budgetBalance = (budgetMonitoringData == null) ? 0.00 : budgetMonitoringData.BudgetAmount;
                    
                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    Category = a.Account_Category,
                    BudgetBalance = budgetBalance,
                    Remarks = "Budget Amount - This Term",
                    ValueDate = DateTime.Parse(termYear + "/" + termMonth)
                });

                //Get total expense of selected term to Prev monthend
                while (startDT < endDT)
                {
                    var expensesOfTermMonthToBeforeFilterMonth = ConstantData.TEMP_HomeReportActualBudgetReportDummyData.GetExpenseData().Where(c => c.AccountID == a.AccountID && c.DateReflected.Year == startDT.Year && c.DateReflected.Month == startDT.Month);

                    foreach (var i in expensesOfTermMonthToBeforeFilterMonth)
                    {
                        totalExpenseThisTermToPrevMonthend += i.ExpenseAmount;
                    }
                    startDT = startDT.AddMonths(1);
                }
                budgetBalance -= totalExpenseThisTermToPrevMonthend;

                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    BudgetBalance = budgetBalance,
                    ExpenseAmount = totalExpenseThisTermToPrevMonthend,
                    Remarks = "Total Expenses - This Term to Prev Monthend",
                    ValueDate = endDT.AddDays(-1)
                });

                //Get all expenses of selected month and year
                var expensesOfFilterYearMonth = ConstantData.TEMP_HomeReportActualBudgetReportDummyData.GetExpenseData().Where(c => c.AccountID == a.AccountID && c.DateReflected.Year == filterYear && c.DateReflected.Month == filterMonth).OrderBy(c => c.DateReflected);

                foreach(var i in expensesOfFilterYearMonth)
                {
                    budgetBalance -= i.ExpenseAmount;
                    subTotal += i.ExpenseAmount;

                    actualBudgetData.Add(new HomeReportActualBudgetModel()
                    {
                        BudgetBalance = budgetBalance,
                        ExpenseAmount = i.ExpenseAmount,
                        Remarks = i.Remarks,
                        Department = i.Department,
                        ValueDate = i.DateReflected
                    });
                }

                //Add Sub-Total to List
                if(subTotal != 0)
                {
                    actualBudgetData.Add(new HomeReportActualBudgetModel()
                    {
                        BudgetBalance = budgetBalance,
                        ExpenseAmount = subTotal,
                        Remarks = "Sub-total",
                        ValueDate = endDT.AddMonths(1).AddDays(-1)
                    });
                }

                //Insert break or seperation row
                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    Category = "BREAK"
                });
            }

            return actualBudgetData;
        }
    }
}