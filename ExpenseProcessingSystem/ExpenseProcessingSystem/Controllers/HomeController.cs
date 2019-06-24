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
using System.Text;
using System.Xml.Linq;

namespace ExpenseProcessingSystem.Controllers
{
    [ScreenFltr]
    public class HomeController : Controller
    {
        private readonly int pageSize = 30;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private readonly GOExpressContext _GOContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private HomeService _service;
        private SortService _sortService;
        //to access resources
        private readonly IStringLocalizer<HomeController> _localizer;
        private IHostingEnvironment _env;

        public HomeController(IHttpContextAccessor httpContextAccessor, EPSDbContext context, GOExpressContext gocontext, IHostingEnvironment hostingEnvironment, IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _GOContext = gocontext;
            _service = new HomeService(_httpContextAccessor, _context, _GOContext, this.ModelState, hostingEnvironment);
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
                    _session.SetString("AF_Budget_Code", vm.DMFilters.AF.AF_Budget_Code ?? "");
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

        //--------------------------[* REPORT *]----------------------------------------
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
                PeriodTo = Convert.ToDateTime(ConstantData.HomeReportConstantValue.DateToday),
                TaxRateList = _service.PopulateTaxRaxListIncludeHist(),
                SignatoryList = _service.PopulateSignatoryList()
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
                return Json(ConstantData.HomeReportConstantValue.GetReportSubTypeData()
                    .Where(m => m.ParentTypeId == Convert.ToInt32(ReportTypeID)).ToList());
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
            string pdfFooterFormat = HomeReportConstantValue.PdfFooter2;
            var signatory = _service.GetSignatoryInfo(model.SignatoryID);

            XElement xelem = XElement.Load("wwwroot/xml/ChangeableData.xml");

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
                case ConstantData.HomeReportConstantValue.APSWT_M:

                    fileName = "AlphalistOfPayeesSubjectToWithholdingTax_Monthly_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;

                    model.MonthName = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputAPSWT_M = _service.GetAPSWT_MData(model.Month, model.Year),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM
                    };
                    break;

                //For Alphalist of Suppliers by top 10000 corporation
                case ConstantData.HomeReportConstantValue.AST1000:
                    fileName = "AlphalistOfSuppliersByTop10000Corporation_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
                    model.MonthName = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName;
                    model.MonthNameTo = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.MonthTo).Single().MonthName;

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

                //For Actual Budget Report
                case ConstantData.HomeReportConstantValue.ActualBudgetReport:
                    fileName = "ActualBudgetReport_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
  
                    model.MonthName = ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == model.Month).Single().MonthName;

                    //Get the necessary data from Database
                    data = new HomeReportDataFilterViewModel
                    {
                        HomeReportOutputActualBudget = _service.GetActualReportData(model.Month, model.Year),
                        HomeReportFilter = model,
                        ReportCommonVM = repComVM
                    };
                    break;

                case ConstantData.HomeReportConstantValue.WTS:
                    fileName = "WithholdingTaxSummary_" + dateNow;
                    layoutName = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
                    data = new HomeReportDataFilterViewModel();
                    //Get the necessary data from Database
                    switch (model.PeriodOption)
                    {
                        case 1:
                            data = new HomeReportDataFilterViewModel
                            {
                                HomeReportOutputWTS = ConstantData.TEMP_HomeReportWTSDummyData.GetTEMP_HomeReportWTSOutputModelData_Month(model.Year, model.Month,
                                    ConstantData.TEMP_HomeReportWTSDummyData.GetTEMP_HomeReportWTSOutputModelData(), model.ReportSubType),
                                HomeReportFilter = model,
                                ReportCommonVM = repComVM
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
                                HomeReportFilter = model,
                                ReportCommonVM = repComVM
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

        //public IActionResult MizuhoLogo2Image()
        //{
        //    var dir = _env.WebRootPath;
        //    var file = System.IO.Path.Combine(dir, "\\images\\mizuho-logo-2.png");
        //    return base.File(file, "image/png");
        //}

        //-----------------------[* BUDGET MONITORING *]-------------------------------------------
        [OnlineUserCheck]
        [ImportModelState]
        public IActionResult BM(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AccountMappingSortParm"] = sortOrder == "acc_mapping_desc" ? "acc_mapping" : "acc_mapping_desc";
            ViewData["AccountNameSortParm"] = sortOrder == "acc_name_desc" ? "acc_name" : "acc_name_desc";
            ViewData["GBaseBudgetCodeSortParm"] = sortOrder == "gbase_budget_code_desc" ? "gbase_budget_code" : "gbase_budget_code_desc";
            ViewData["AccountNumberSortParm"] = sortOrder == "acc_num_desc" ? "acc_num" : "acc_num_desc";
            ViewData["BudgetSortParm"] = sortOrder == "budget_desc" ? "budget" : "budget_desc";
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
        public IActionResult Entry_CV()
        {
            var userId = GetUserID();

            EntryCVViewModelList viewModel = new EntryCVViewModelList();
            viewModel = PopulateEntry((EntryCVViewModelList)viewModel);
            //viewModel.vendor = 2;
            viewModel.EntryCV.Add(new EntryCVViewModel());
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
            int id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_CV);
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
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED, int.Parse(GetUserID())))
                    {
                        _service.postCV(entryID);
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_CV_ReadOnly";
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
                    viewLink = "Entry_CV_ReadOnly";
                    break;
                case "Reject": break;
                default:
                    break;
            }

            ModelState.Clear();

            cvList = _service.getExpense(entryID);

            cvList = PopulateEntry((EntryCVViewModelList)cvList);

            foreach (var acc in cvList.EntryCV)
            {
                cvList.systemValues.acc.AddRange(_service.getAccDetailsEntry(acc.account));
            }

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
        [ImportModelState]
        public IActionResult Entry_DDV(EntryDDVViewModelList viewModel)
        {
            var userId = GetUserID();
            if (viewModel.EntryDDV.Count <= 0)
            {
               viewModel = new EntryDDVViewModelList();
               viewModel.EntryDDV.Add(new EntryDDVViewModel { interDetails = new List<DDVInterEntityViewModel> { new DDVInterEntityViewModel() } });
            }
            viewModel = PopulateEntry((EntryDDVViewModelList)viewModel);
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
            return RedirectToAction("View_DDV", "Home", new { entryID = id });
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult View_DDV(int entryID)
        {
            var userId = GetUserID();

            EntryDDVViewModelList ddvList = _service.getExpenseDDV(entryID);
            ddvList = PopulateEntry((EntryDDVViewModelList)ddvList);

            return View("Entry_DDV_ReadOnly", ddvList);
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult VerAppModDDV(int entryID, string command)
        {
            var userId = GetUserID();

            string viewLink = "Entry_DDV";
            EntryDDVViewModelList ddvList;

            switch (command)
            {
                case "Modify":
                    viewLink = "Entry_DDV";
                    break;
                case "approver":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED, int.Parse(GetUserID())))
                    {
                        //_service.SaveToGBase();
                        var expDtls = _context.ExpenseEntry.Where(x => x.Expense_ID == entryID).Select(x => x.ExpenseEntryDetails).FirstOrDefault();
                        var isFbt = expDtls.Select(x => x.ExpDtl_Fbt).FirstOrDefault() == true;
                        if (isFbt)
                        {
                            //_service.SaveToGBaseFBT();
                        }
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_DDV_ReadOnly";
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
                    viewLink = "Entry_DDV_ReadOnly";
                    break;
                case "Reject": break;
                default:
                    break;
            }

            ModelState.Clear();

            ddvList = _service.getExpenseDDV(entryID);

            ddvList = PopulateEntry((EntryDDVViewModelList)ddvList);

            foreach (var acc in ddvList.EntryDDV)
            {
                ddvList.systemValues.acc.AddRange(_service.getAccDetailsEntry(acc.account));
            }

            return View(viewLink, ddvList);
        }

        //-----------------[* Entry Petty Cash *]-----------------------------
        [OnlineUserCheck]
        [ImportModelState]
        public IActionResult Entry_PCV()
        {
            var userId = GetUserID();

            EntryCVViewModelList viewModel = new EntryCVViewModelList();
            viewModel = PopulateEntry((EntryCVViewModelList)viewModel);

            viewModel.EntryCV.Add(new EntryCVViewModel { screenCode = "PCV"});

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
                return View("Entry_PCV", PopulateEntry((EntryCVViewModelList)EntryCVViewModelList));
            }

            EntryCVViewModelList pcvList = new EntryCVViewModelList();

            int id = 0;
            if (EntryCVViewModelList.entryID == 0)
            {
                id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_PC);
            }
            else
            {
                if (_service.deleteExpenseEntry(EntryCVViewModelList.entryID))
                {
                    id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_PC);
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
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_APPROVED, int.Parse(GetUserID())))
                    {
                        _service.postCV(entryID);
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_PCV_ReadOnly";
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
                    viewLink = "Entry_PCV_ReadOnly";
                    break;
                case "Delete":
                    if (_service.deleteExpenseEntry(entryID))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    return RedirectToAction("Index", "Home");

                case "Reject":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REJECTED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_PCV_ReadOnly";
                    break;

                default:
                    break;
            }

            pcvList = _service.getExpense(entryID);

            pcvList = PopulateEntry((EntryCVViewModelList)pcvList);
            foreach (var i in pcvList.EntryCV)
            {
                pcvList.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));
                i.screenCode = "PCV";
            }

            return View(viewLink, pcvList);
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
            int firstId = int.Parse(listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR].First().Value);

            pcvList.systemValues.ewt = _service.getVendorTaxRate(firstId);
            pcvList.systemValues.vat = _service.getVendorVat(firstId);
            pcvList.systemValues.acc = _service.getAccDetailsEntry();
            foreach (var i in pcvList.EntryCV)
            {
                i.screenCode = "PCV";
            }

            return View("Entry_PCV_ReadOnly", pcvList);
        }
        //[* Entry Petty Cash *]
        //------------------------------------------------------------------

        //-------------[* Entry Cash Advance(SS) *]--------------------------
        [OnlineUserCheck]
        [ImportModelState]
        public IActionResult Entry_SS()
        {
            var userId = GetUserID();

            EntryCVViewModelList viewModel = new EntryCVViewModelList();
            viewModel = PopulateEntry((EntryCVViewModelList)viewModel);

            viewModel.EntryCV.Add(new EntryCVViewModel { screenCode = "SS" });

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
                return View("Entry_SS", PopulateEntry((EntryCVViewModelList)EntryCVViewModelList));
            }

            //EntryCVViewModelList ssList = new EntryCVViewModelList();

            int id = 0;
            if (EntryCVViewModelList.entryID == 0)
            {
                id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_SS);
            }
            else
            {
                if (_service.deleteExpenseEntry(EntryCVViewModelList.entryID))
                {
                    id = _service.addExpense_CV(EntryCVViewModelList, int.Parse(GetUserID()), GlobalSystemValues.TYPE_SS);
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

            string viewLink = "Entry_SS";
            EntryCVViewModelList ssList;

            switch (command)
            {
                case "Modify":
                    viewLink = "Entry_SS";
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
                    viewLink = "Entry_SS_ReadOnly";
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
                    viewLink = "Entry_SS_ReadOnly";
                    break;
                case "Delete":
                    if (_service.deleteExpenseEntry(entryID))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    return RedirectToAction("Index", "Home");

                case "Reject":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REJECTED, int.Parse(GetUserID())))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_SS_ReadOnly";
                    break;

                default:
                    break;
            }

            ssList = _service.getExpense(entryID);
            ssList = PopulateEntry((EntryCVViewModelList)ssList);

            foreach (var i in ssList.EntryCV)
            {
                ssList.systemValues.acc.AddRange(_service.getAccDetailsEntry(i.account));
                i.screenCode = "SS";
            }

            return View(viewLink, ssList);
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
            int firstId = int.Parse(listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR].First().Value);

            ssList.systemValues.ewt = _service.getVendorTaxRate(firstId);
            ssList.systemValues.vat = _service.getVendorVat(firstId);
            ssList.systemValues.acc = _service.getAccDetailsEntry();
            foreach(var i in ssList.EntryCV)
            {
                i.screenCode = "SS";
            }

            return View("Entry_SS_ReadOnly", ssList);
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult CDD_IS_SS(int entryID)
        {
            string newFileName = "CDD_IS" + DateTime.Now.ToString("MM-dd-yyyy_hhmmss") + ".xlsx";
            ExcelGenerateService excelGenerate = new ExcelGenerateService();

            return File(excelGenerate.ExcelCDDIS(new CDDISValuesVIewModel
            {
                VALUE_DATE = DateTime.Parse("2019/01/01"),
                REFERENCE_NO = "CHK767123456",
                COMMENT = "CD",
                SECTION = "09",
                REMARKS = "THIS IS CDD Instruction sheet generate TEST",
                SCHEME_NO = "123456789012",
                MEMO = "Y",
                DEBIT_CREDIT_1 = "D",
                CCY_1 = "JPY",
                AMOUNT_1 = 98223,
                CUSTOMER_ABBR_1 = "900",
                ACCOUNT_CODE_1 = "14017",
                ACCOUNT_NO_1 = "B79789111111",
                EXCHANGE_RATE_1 = 0.4545,
                CONTRA_CCY_1 = "USD",
                FUND_1 = "O",
                CHECK_NO_1 = "2019062122",
                AVAILABLE_DATE_1 = DateTime.Parse("2019/02/01"),
                ADVICE_1 = "Y",
                DETAILS_1 = "This is Details 1 Test",
                ENTITY_1 = "010",
                DIVISION_1 = "11",
                INTER_AMOUNT_1 = 915.25,
                INTER_RATE_1 = 0.0093,
                DEBIT_CREDIT_2 = "C",
                CCY_2 = "JPY",
                AMOUNT_2 = 98223.25,
                CUSTOMER_ABBR_2 = "911",
                ACCOUNT_CODE_2 = "00204",
                ACCOUNT_NO_2 = "F79789171137",
                EXCHANGE_RATE_2 = 0.4545,
                CONTRA_CCY_2 = "SGD",
                FUND_2 = "O",
                CHECK_NO_2 = "2019062123",
                AVAILABLE_DATE_2 = DateTime.Parse("2019/02/22"),
                ADVICE_2 = "Y",
                DETAILS_2 = "This is Details 2 Test",
                ENTITY_2 = "011",
                DIVISION_2 = "12",
                INTER_AMOUNT_2 = 1915.25,
                INTER_RATE_2 = 0.1193
            }, newFileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", newFileName);

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
            return RedirectToAction("View_NC", "Home", new { entryID = id });
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult View_NC(int entryID)
        {
            var userId = GetUserID();

            EntryNCViewModelList ncList = _service.getExpenseNC(entryID);
            ncList = PopulateEntryNC(ncList);

            return View("Entry_NC_ReadOnly", ncList);
        }
        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult VerAppModNC(int entryID, string command)
        {
            var userId = GetUserID();

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
                        _service.postNC(entryID);
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_NC_ReadOnly";
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
                    viewLink = "Entry_NC_ReadOnly";
                    break;
                case "Reject":
                    if (_service.updateExpenseStatus(entryID, GlobalSystemValues.STATUS_REJECTED, int.Parse(GetUserID())))
                    {
                        _service.postNC(entryID);
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_NC_ReadOnly";
                    break;
                case "Delete":
                    if (_service.deleteExpenseEntry(entryID, GlobalSystemValues.TYPE_NC))
                    {
                        ViewBag.Success = 1;
                    }
                    else
                    {
                        ViewBag.Success = 0;
                    }
                    viewLink = "Entry_NC";
                    return RedirectToAction("Entry_NC", new EntryNCViewModelList());
                default:
                    break;
            }

            ModelState.Clear();
            ncList = _service.getExpenseNC(entryID);
            ncList = PopulateEntryNC(ncList);

            //foreach (var dtls in ncList.EntryNC.ExpenseEntryNCDtls)
            //{
            //    foreach (var acc in dtls.ExpenseEntryNCDtlAccs)
            //    {
            //        ncList.systemValues.acc.AddRange(_service.getAccDetailsEntry(acc.ExpNCDtlAcc_Acc_ID));
            //    }
            //}
            ViewData["partialName"] = ncList.EntryNC.NC_Category_ID.ToString();
            return View(viewLink, ncList);
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
        public IActionResult Liquidation_SS(int entryID)
        {
            var userId = GetUserID();

            LiquidationViewModel ssList = _service.getExpenseToLiqudate(entryID);

            foreach (var i in ssList.LiquidationDetails)
            {
                i.screenCode = "Liquidation_SS";
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
                return View("Liquidation_SS", vm);
            }
            int id = _service.addLiquidationDetail(vm, int.Parse(GetUserID()));

            return RedirectToAction("Liquidation_Main");
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
            DateTime fromDate = DateTime.ParseExact(model.Year + "-" + model.Month, format, CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);


            if (model.ReportType != 0)
            {
                switch (model.ReportType)
                {
                    case ConstantData.HomeReportConstantValue.AST1000:
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
        public JsonResult getVendorTRList(int vendorID)
        {
            var trList = _service.getVendorTaxRate(vendorID);

            return Json(trList.ToList());
        }

        [HttpPost]
        public JsonResult getVendorVatList(int vendorID)
        {
            var vatList = _service.getVendorVat(vendorID);

            return Json(vatList.ToList());
        }

        public IActionResult GenerateVoucher(VoucherViewModelList model)
        {
            //string dateNow = DateTime.Now.ToString("MM-dd-yyyy_hhmmsstt"); // ORIGINAL
            model.date = DateTime.Now.ToString("MM-dd-yyyy");
            ReportCommonViewModel headerVM = new ReportCommonViewModel();

            model.headvm.Header_Logo = "";
            model.headvm.Header_Name = "Mizuho Bank Ltd., Manila Branch";
            model.headvm.Header_TIN = "004-669-467-000";
            model.headvm.Header_Address = "25th Floor, The Zuellig Building, Makati Avenue corner Paseo de Roxas, Makati City";

            model.maker = GetUserID();

            //Return Preview
            return View(GlobalSystemValues.VOUCHER_LAYOUT, model);
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
    }
}