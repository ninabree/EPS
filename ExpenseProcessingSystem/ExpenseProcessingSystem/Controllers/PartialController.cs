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
            ViewData["PayeeStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "payee_stat" : "";
            ViewData["PayeeTINSortParm"] = sortOrder == "payee_TIN_desc" ? "payee_TIN" : "payee_TIN_desc";
            ViewData["PayeeAddSortParm"] = sortOrder == "payee_add_desc" ? "payee_add" : "payee_add_desc";
            ViewData["PayeeTypeSortParm"] = sortOrder == "payee_type_desc" ? "payee_type" : "payee_type_desc";
            ViewData["PayeeNoSortParm"] = sortOrder == "payee_no_desc" ? "payee_no" : "payee_no_desc";
            ViewData["PayeeCreatorSortParm"] = sortOrder == "payee_creatr_desc" ? "payee_creatr" : "payee_creatr_desc";
            ViewData["PayeeApproverSortParm"] = sortOrder == "payee_approvr_desc" ? "payee_approvr" : "payee_approvr_desc";
            ViewData["PayeeSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null){ pg = 1; }
            else{ searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populatePayee(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                //pagination
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
            ViewData["AccountSortParm"] = sortOrder == "name_desc" ? "name_desc" : "name_desc";

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
            ViewData["VATCodeSortParm"] = sortOrder == "vat_code_desc" ? "code_code" : "vat_code_desc";
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

        [Route("/Partial/DMPartial_EWT/")]
        public IActionResult DMPartial_EWT(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["EWTStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "ewt_stat" : "";
            ViewData["EWTTaxRateSortParm"] = sortOrder == "ewt_tax_desc" ? "ewt_tax" : "ewt_tax_desc";
            ViewData["EWTATCSortParm"] = sortOrder == "ewt_atc_desc" ? "ewt_atc" : "ewt_atc_desc";
            ViewData["EWTTaxRateDescSortParm"] = sortOrder == "ewt_tax_descrp_desc" ? "ewt_tax_descrp" : "ewt_tax_descrp_desc";
            ViewData["EWTCreatorSortParm"] = sortOrder == "ewt_creatr_desc" ? "ewt_creatr" : "ewt_creatr_desc";
            ViewData["EWTApproverSortParm"] = sortOrder == "ewt_approvr_desc" ? "ewt_approvr" : "ewt_approvr_desc";
            ViewData["EWTLastUpdatedSortParm"] = sortOrder == "ewt_last_updte_desc" ? "ewt_last_updte" : "ewt_last_updte_desc";
            ViewData["EWTSortParm"] = sortOrder == "nature_desc" ? "nature" : "nature_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            ////populate and sort
            var sortedVals = _sortService.SortData(_service.populateEWT(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                EWT = PaginatedList<DMEWTViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMEWTViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
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
    }
}