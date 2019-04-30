using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Search_Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseProcessingSystem.Controllers
{
    [ScreenFltr]
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

        [Route("/Partial/DMPartial_Vendor/")]
        public IActionResult DMPartial_Vendor(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["VendorStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "vendor_stat" : "";
            ViewData["VendorTINSortParm"] = sortOrder == "vendor_TIN_desc" ? "vendor_TIN" : "vendor_TIN_desc";
            ViewData["VendorAddSortParm"] = sortOrder == "vendor_add_desc" ? "vendor_add" : "vendor_add_desc";
            ViewData["VendorTypeSortParm"] = sortOrder == "vendor_type_desc" ? "vendor_type" : "vendor_type_desc";
            ViewData["VendorNoSortParm"] = sortOrder == "vendor_no_desc" ? "vendor_no" : "vendor_no_desc";
            ViewData["VendorCreatorSortParm"] = sortOrder == "vendor_creatr_desc" ? "vendor_creatr" : "vendor_creatr_desc";
            ViewData["VendorApproverSortParm"] = sortOrder == "vendor_approvr_desc" ? "vendor_approvr" : "vendor_approvr_desc";
            ViewData["VendorSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null){ pg = 1; }
            else{ searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateVendor(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                //pagination
                Vendor = PaginatedList<DMVendorViewModel>.CreateAsync(
                        (sortedVals.list).Cast<DMVendorViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
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
            ViewData["DeptStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "dept_stat" : "";
            ViewData["DeptCodeSortParm"] = sortOrder == "dept_code_desc" ? "dept_code" : "dept_code_desc";
            ViewData["DeptCreatorSortParm"] = sortOrder == "dept_creatr_desc" ? "dept_creatr" : "dept_creatr_desc";
            ViewData["DeptApproverSortParm"] = sortOrder == "dept_approvr_desc" ? "dept_approvr" : "dept_approvr_desc";
            ViewData["DeptLastUpdatedSortParm"] = sortOrder == "dept_last_updte_desc" ? "dept_last_updte" : "dept_last_updte_desc";
            ViewData["DeptSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateDept(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                Dept = PaginatedList<DMDeptViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMDeptViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_Check/")]
        public IActionResult DMPartial_Check(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CheckStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "chk_stat" : "";
            ViewData["CheckSeriesFromSortParm"] = sortOrder == "chk_serires_from_desc" ? "chk_serires_from" : "chk_serires_from_desc";
            ViewData["CheckSeriresToSortParm"] = sortOrder == "chk_serires_to_desc" ? "chk_serires_to" : "chk_serires_to_desc";
            ViewData["CheckTypeSortParm"] = sortOrder == "chk_type_desc" ? "chk_type" : "chk_type_desc";
            ViewData["CheckNameSortParm"] = sortOrder == "chk_name_desc" ? "chk_name" : "chk_name_desc";
            ViewData["CheckCreatorSortParm"] = sortOrder == "chk_creatr_desc" ? "chk_creatr" : "chk_creatr_desc";
            ViewData["CheckApproverSortParm"] = sortOrder == "chk_approvr_desc" ? "chk_approvr" : "chk_approvr_desc";
            ViewData["CheckLastUpdatedSortParm"] = sortOrder == "chk_last_updte_desc" ? "chk_last_updte" : "chk_last_updte_desc";
            ViewData["CheckSortParm"] = sortOrder == "input_date_desc" ? "input_date" : "input_date_desc";
            
            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateCheck(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                Check = PaginatedList<DMCheckViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMCheckViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_Acc/")]
        public IActionResult DMPartial_Acc(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AccountStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "acc_stat" : "";
            ViewData["AccountCodeSortParm"] = sortOrder == "acc_code_desc" ? "acc_code" : "acc_code_desc";
            ViewData["AccountNumberSortParm"] = sortOrder == "acc_no_desc" ? "acc_no" : "acc_no_desc";
            ViewData["AccountCustSortParm"] = sortOrder == "acc_cust_desc" ? "acc_cust" : "acc_cust_desc";
            ViewData["AccountDivSortParm"] = sortOrder == "acc_div_desc" ? "acc_div" : "acc_div_desc";
            ViewData["AccountFundSortParm"] = sortOrder == "acc_fund_desc" ? "acc_fund" : "acc_fund_desc";
            ViewData["AccountCreatorSortParm"] = sortOrder == "acc_creatr_desc" ? "acc_creatr" : "acc_creatr_desc";
            ViewData["AccountApproverSortParm"] = sortOrder == "acc_approvr_desc" ? "acc_approvr" : "acc_approvr_desc";
            ViewData["AccountLastUpdatedSortParm"] = sortOrder == "acc_last_updte_desc" ? "acc_last_updte" : "acc_last_updte_desc";
            ViewData["AccountSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateAccount(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                Account = PaginatedList<DMAccountViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMAccountViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_VAT/")]
        public IActionResult DMPartial_VAT(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["VATStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "vat_stat" : "";
            ViewData["VATCodeSortParm"] = sortOrder == "vat_code_desc" ? "vat_code" : "vat_code_desc";
            ViewData["VATCreatorSortParm"] = sortOrder == "vat_creatr_desc" ? "vat_creatr" : "vat_creatr_desc";
            ViewData["VATApproverSortParm"] = sortOrder == "vat_approvr_desc" ? "vat_approvr" : "vat_approvr_desc";
            ViewData["VATLastUpdatedSortParm"] = sortOrder == "vat_last_updte_desc" ? "vat_last_updte" : "vat_last_updte_desc";
            ViewData["VATSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateVAT(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                VAT = PaginatedList<DMVATViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMVATViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_FBT/")]
        public IActionResult DMPartial_FBT(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["FBTStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "fbt_stat" : "";
            ViewData["FBTAccountSortParm"] = sortOrder == "fbt_acc_desc" ? "fbt_acc" : "fbt_acc_desc";
            ViewData["FBTFormulaSortParm"] = sortOrder == "fbt_formula_desc" ? "fbt_formula" : "fbt_formula_desc";
            ViewData["FBTRateSortParm"] = sortOrder == "fbt_rate_desc" ? "fbt_rate" : "fbt_rate_desc";
            ViewData["FBTCreatorSortParm"] = sortOrder == "fbt_creatr_desc" ? "fbt_creatr" : "fbt_creatr_desc";
            ViewData["FBTApproverSortParm"] = sortOrder == "fbt_approvr_desc" ? "fbt_approvr" : "fbt_approvr_desc";
            ViewData["FBTLastUpdatedSortParm"] = sortOrder == "fbt_last_updte_desc" ? "fbt_last_updte" : "fbt_last_updte_desc";
            ViewData["FBTSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateFBT(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                FBT = PaginatedList<DMFBTViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMFBTViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_TR/")]
        public IActionResult DMPartial_TR(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TRStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "ewt_stat" : "";
            ViewData["TRTaxRateSortParm"] = sortOrder == "ewt_tax_desc" ? "ewt_tax" : "ewt_tax_desc";
            ViewData["TRATCSortParm"] = sortOrder == "ewt_atc_desc" ? "ewt_atc" : "ewt_atc_desc";
            ViewData["TRTaxRateDescSortParm"] = sortOrder == "ewt_tax_descrp_desc" ? "ewt_tax_descrp" : "ewt_tax_descrp_desc";
            ViewData["TRCreatorSortParm"] = sortOrder == "ewt_creatr_desc" ? "ewt_creatr" : "ewt_creatr_desc";
            ViewData["TRApproverSortParm"] = sortOrder == "ewt_approvr_desc" ? "ewt_approvr" : "ewt_approvr_desc";
            ViewData["TRLastUpdatedSortParm"] = sortOrder == "ewt_last_updte_desc" ? "ewt_last_updte" : "ewt_last_updte_desc";
            ViewData["TRSortParm"] = sortOrder == "nature_desc" ? "nature" : "nature_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            ////populate and sort
            var sortedVals = _sortService.SortData(_service.populateTR(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                TR = PaginatedList<DMTRViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMTRViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_Curr/")]
        public IActionResult DMPartial_Curr(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "curr_stat" : "";
            ViewData["CurrCodeSortParm"] = sortOrder == "curr_code_desc" ? "curr_code" : "curr_code_desc";
            ViewData["CurrCreatorSortParm"] = sortOrder == "curr_creatr_desc" ? "curr_creatr" : "curr_creatr_desc";
            ViewData["CurrApproverSortParm"] = sortOrder == "curr_approvr_desc" ? "curr_approvr" : "curr_approvr_desc";
            ViewData["CurrLastUpdatedSortParm"] = sortOrder == "curr_last_updte_desc" ? "curr_last_updte" : "curr_last_updte_desc";
            ViewData["CurrSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateCurr(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                Curr = PaginatedList<DMCurrencyViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMCurrencyViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_RegEmp/")]
        public IActionResult DMPartial_RegEmp(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["RegEmpStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "regemp_stat" : "";
            ViewData["RegEmpNoSortParm"] = sortOrder == "regemp_no_desc" ? "regemp_no" : "regemp_no_desc";
            ViewData["RegEmpCreatorSortParm"] = sortOrder == "regemp_creatr_desc" ? "regemp_creatr" : "regemp_creatr_desc";
            ViewData["RegEmpApproverSortParm"] = sortOrder == "regemp_approvr_desc" ? "regemp_approvr" : "regemp_approvr_desc";
            ViewData["RegEmpLastUpdatedSortParm"] = sortOrder == "regemp_last_updte_desc" ? "regemp_last_updte" : "regemp_last_updte_desc";
            ViewData["RegEmpSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateRegEmp(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                Emp = PaginatedList<DMEmpViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMEmpViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_TempEmp/")]
        public IActionResult DMPartial_TempEmp(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TempEmpStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "tempemp_stat" : "";
            //ViewData["TempEmpNoSortParm"] = sortOrder == "tempemp_no_desc" ? "tempemp_no" : "tempemp_no_desc";
            ViewData["TempEmpCreatorSortParm"] = sortOrder == "tempemp_creatr_desc" ? "tempemp_creatr" : "tempemp_creatr_desc";
            ViewData["TempEmpApproverSortParm"] = sortOrder == "tempemp_approvr_desc" ? "tempemp_approvr" : "tempemp_approvr_desc";
            ViewData["TempEmpLastUpdatedSortParm"] = sortOrder == "tempemp_last_updte_desc" ? "tempemp_last_updte" : "tempemp_last_updte_desc";
            ViewData["TempEmpSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateTempEmp(filters), sortOrder, "TEMP");
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                Emp = PaginatedList<DMEmpViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMEmpViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_Cust/")]
        public IActionResult DMPartial_Cust(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CustStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "cust_stat" : "";
            ViewData["CustAbbrSortParm"] = sortOrder == "cust_abbr_desc" ? "cust_abbr" : "cust_abbr_desc";
            ViewData["CustNoSortParm"] = sortOrder == "cust_no_desc" ? "cust_no" : "cust_no_desc";
            ViewData["CustCreatorSortParm"] = sortOrder == "cust_creatr_desc" ? "cust_creatr" : "cust_creatr_desc";
            ViewData["CustApproverSortParm"] = sortOrder == "cust_approvr_desc" ? "cust_approvr" : "cust_approvr_desc";
            ViewData["CustLastUpdatedSortParm"] = sortOrder == "cust_last_updte_desc" ? "cust_last_updte" : "cust_last_updte_desc";
            ViewData["CustSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateCust(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                Cust = PaginatedList<DMCustViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMCustViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_NCC/")]
        public IActionResult DMPartial_NCC(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NCCStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "ncc_stat" : "";
            ViewData["NCCCodeSortParm"] = sortOrder == "ncc_code_desc" ? "ncc_code" : "ncc_code_desc";
            ViewData["NCCCreatorSortParm"] = sortOrder == "ncc_creatr_desc" ? "ncc_creatr" : "ncc_creatr_desc";
            ViewData["NCCApproverSortParm"] = sortOrder == "ncc_approvr_desc" ? "ncc_approvr" : "ncc_approvr_desc";
            ViewData["NCCLastUpdatedSortParm"] = sortOrder == "ncc_last_updte_desc" ? "ncc_last_updte" : "ncc_last_updte_desc";
            ViewData["NCCSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateNCC(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                NCC = PaginatedList<DMNCCViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMNCCViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_BCS/")]
        public IActionResult DMPartial_BCS(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["BCSStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "bcs_stat" : "";
            ViewData["BCSTINSortParm"] = sortOrder == "bcs_tin_desc" ? "bcs_tin" : "bcs_tin_desc";
            ViewData["BCSPosSortParm"] = sortOrder == "bcs_pos_desc" ? "bcs_pos" : "bcs_pos_desc";
            ViewData["BCSSignSortParm"] = sortOrder == "bcs_sign_desc" ? "bcs_sign" : "bcs_sign_desc";
            ViewData["BCSCreatorSortParm"] = sortOrder == "bcs_creatr_desc" ? "bcs_creatr" : "bcs_creatr_desc";
            ViewData["BCSApproverSortParm"] = sortOrder == "bcs_approvr_desc" ? "bcs_approvr" : "bcs_approvr_desc";
            ViewData["BCSLastUpdatedSortParm"] = sortOrder == "bcs_last_updte_desc" ? "bcs_last_updte" : "bcs_last_updte_desc";
            ViewData["BCSSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateBCS(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                BCS = PaginatedList<DMBCSViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMBCSViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

    }
}