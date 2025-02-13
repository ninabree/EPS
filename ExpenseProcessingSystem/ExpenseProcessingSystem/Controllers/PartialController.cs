﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Xml;
using System.Xml.Linq;
using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using ExpenseProcessingSystem.ViewModels.Search_Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        XElement xelemLiq = XElement.Load("wwwroot/xml/LiquidationValue.xml");

        public PartialController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _service = new PartialService(_httpContextAccessor, _context);
            _sortService = new SortService();
        }
        

        // -------------------------------------- [[ NON CASH ]] --------------------------------------
        // [[ Local Payroll ]]
        [OnlineUserCheck]
        //[ImportModelState]
        [Route("//Partial/NC_Partial_GetPreview/")]
        public IActionResult NC_Partial_GetPreview (string categoryID, string entryID)
        {
            var userId = _session.GetString("UserID");
            EntryNCViewModelList viewModel = new EntryNCViewModelList();
            var accs = _service.getNCAccsForFilter();
            viewModel.accList = accs;

            ViewData["USDmstr"] = _service.getXMLCurrency("USD").currMasterID;
            ViewData["JPYmstr"] = _service.getXMLCurrency("YEN").currMasterID;
            ViewData["PHPmstr"] = _service.getXMLCurrency("PHP").currMasterID;

            viewModel = PopulateEntryNC(viewModel);
            DMCurrencyModel currDtl = _service.getCurrencyByMasterID(int.Parse(xelemLiq.Element("CURRENCY_PHP").Value));
            DMCurrencyModel currDtlUSD = _service.getCurrencyByMasterID(int.Parse(xelemLiq.Element("CURRENCY_US").Value));


            if ((entryID != "0") && (entryID != null) && (categoryID != null))
            {
                viewModel = _service.getExpenseNC(int.Parse(entryID));
                viewModel.EntryNC.NC_Category_ID = int.Parse(categoryID);

                viewModel.accList = accs;
                viewModel = PopulateEntryNC(viewModel);
                if (categoryID == GlobalSystemValues.NC_PETTY_CASH_REPLENISHMENT.ToString())
                {
                    viewModel.EntryNC.ExpenseEntryNCDtls_CDD = CONSTANT_NC_PETTYCASHREPLENISHMENT.Populate_CDD_Instruc_Sheet(currDtl);
                    viewModel.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpNCDtl_Remarks_Period = viewModel.EntryNC.NC_CS_Period;
                }
                else if (categoryID == GlobalSystemValues.NC_RETURN_OF_JS_PAYROLL.ToString())
                {
                    viewModel.EntryNC.ExpenseEntryNCDtls_CDD = CONSTANT_NC_RETURN_OF_JSPAYROLL.Populate_CDD_Instruc_Sheet(currDtl, currDtlUSD, _service.getNCAccs("/NONCASHACCOUNTS/RETURNOFJSPAYROLLCDD/ACCOUNT"));
                    viewModel.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpNCDtl_Remarks_Period = viewModel.EntryNC.NC_CS_Period;
                }
            } else
            {
                viewModel.entryID = 0;
                viewModel.EntryNC.NC_Category_ID = int.Parse(categoryID);

                if (categoryID == GlobalSystemValues.NC_LS_PAYROLL.ToString())
                {
                    viewModel.EntryNC = CONSTANT_NC_LSPAYROLL.Populate_LSPAYROLL(currDtl, _service.getNCAccs("/NONCASHACCOUNTS/LSPAYROLL/ACCOUNT"));
                }
                else if (categoryID == GlobalSystemValues.NC_TAX_REMITTANCE.ToString())
                {
                    viewModel.EntryNC = CONSTANT_NC_TAXREMITTANCE.Populate_TAXREMITTANCE(currDtl, _service.getNCAccs("/NONCASHACCOUNTS/TAXREMITTANCE/ACCOUNT"));
                }
                else if (categoryID == GlobalSystemValues.NC_MONTHLY_ROSS_BILL.ToString())
                {
                    viewModel.EntryNC = CONSTANT_NC_MONTHLYROSSBILL.Populate_MONTHLYROSSBILL(currDtl, _service.getNCAccs("/NONCASHACCOUNTS/MONTHLYROSS/ACCOUNT"));
                }
                else if (categoryID == GlobalSystemValues.NC_PSSC.ToString())
                {
                    viewModel.EntryNC = CONSTANT_NC_PSSC.Populate_PSSC(currDtl, _service.getNCAccs("/NONCASHACCOUNTS/PSSC/ACCOUNT"));
                }
                else if (categoryID == GlobalSystemValues.NC_PCHC.ToString())
                {
                    viewModel.EntryNC = CONSTANT_NC_PCHC.Populate_PCHC(currDtl, _service.getNCAccs("/NONCASHACCOUNTS/PCHC/ACCOUNT"));
                }
                else if (categoryID == GlobalSystemValues.NC_DEPRECIATION.ToString())
                {
                    viewModel.EntryNC = CONSTANT_NC_DEPRECIATION.Populate_DEPRECIATION(currDtl, _service.getNCAccs("/NONCASHACCOUNTS/DEPRECIATION/ACCOUNT"));
                }
                else if (categoryID == GlobalSystemValues.NC_PETTY_CASH_REPLENISHMENT.ToString())
                {
                    viewModel.EntryNC = CONSTANT_NC_PETTYCASHREPLENISHMENT.Populate_PETTYCASHREPLENISHMENT(currDtl, _service.getNCAccs("/NONCASHACCOUNTS/PCR/ACCOUNT"));
                    viewModel.EntryNC.ExpenseEntryNCDtls_CDD = CONSTANT_NC_PETTYCASHREPLENISHMENT.Populate_CDD_Instruc_Sheet(currDtl);
                }
                else if (categoryID == GlobalSystemValues.NC_JS_PAYROLL.ToString())
                {
                    viewModel.EntryNC = CONSTANT_NC_JSPAYROLL.Populate_JSPAYROLL(currDtl, currDtlUSD, _service.getNCAccs("/NONCASHACCOUNTS/JSPAYROLL/ACCOUNT"));
                }
                else if (categoryID == GlobalSystemValues.NC_RETURN_OF_JS_PAYROLL.ToString())
                {
                    viewModel.EntryNC = CONSTANT_NC_RETURN_OF_JSPAYROLL.Populate_RETURN_OF_JSPAYROLL(currDtl, currDtlUSD, _service.getNCAccs("/NONCASHACCOUNTS/RETURNOFJSPAYROLL/ACCOUNT"));
                    viewModel.EntryNC.ExpenseEntryNCDtls_CDD = CONSTANT_NC_RETURN_OF_JSPAYROLL.Populate_CDD_Instruc_Sheet(currDtl, currDtlUSD, _service.getNCAccs("/NONCASHACCOUNTS/RETURNOFJSPAYROLLCDD/ACCOUNT"));
                }
                else if (categoryID == GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES.ToString())
                {
                    if (TempData["amortizationModel"] != null)
                    {
                        string jsonModel = TempData["amortizationModel"].ToString();
                        var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<EntryNCViewModelList>(jsonModel);

                        viewModel.EntryNC = temp.EntryNC;
                        viewModel.amortizationID = temp.amortizationID;
                    }
                    else {
                        viewModel.EntryNC = CONSTANT_NC_MISC_ENTRIES.Populate_MISC_ENTRIES(currDtl, new DMAccountModel { Account_ID = int.Parse(viewModel.accountList[0].Value.Length < 1 ? "0" : viewModel.accountList[0].Value), Account_Name = viewModel.accountList[0].Text });
                    }
                }
            }
            ViewBag.phpid = _service.getCurrencyByMasterID(int.Parse(xelemLiq.Element("CURRENCY_PHP").Value)).Curr_ID;
            ViewBag.usdid = _service.getCurrencyByMasterID(int.Parse(xelemLiq.Element("CURRENCY_US").Value)).Curr_ID;
            ViewBag.yenid = _service.getCurrencyByMasterID(int.Parse(xelemLiq.Element("CURRENCY_Yen").Value)).Curr_ID;
            viewModel.taxaccID = viewModel.accList.FirstOrDefault(x => x.accName == "ET").accID;
            viewModel.usdCashAccID = viewModel.accList.FirstOrDefault(x => x.accName == "USD").accID;
            viewModel.yenCashAccID = viewModel.accList.FirstOrDefault(x => x.accName == "YEN").accID;
            return View("NCPartial", viewModel);
        }

        public EntryNCViewModelList PopulateEntryNC(EntryNCViewModelList viewModel)
        { 
            viewModel.category_of_entry = GlobalSystemValues.NC_CATEGORIES_SELECT;
            viewModel.expenseDate = DateTime.Now;
            viewModel.accountList = _service.getAccountSelectList();
            viewModel.currList = _service.getCurrencySelectList();
            viewModel.vendorList = _service.getVendorSelectList();
            viewModel.trList = _service.getTaxRateSelectList();
            return viewModel;
        }
        
        // -------------------------------------- [[ DATA MAINTENANCE ]] --------------------------------------
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
            var pendingList = (sortedVals.list).Cast<DMVendorViewModel>().Where(x => x.Vendor_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x=> x.Vendor_MasterID).ToList() : new List<int>(),
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
            ViewData["DeptBudgetSortParm"] = sortOrder == "dept_budget_desc" ? "dept_budget" : "dept_budget_desc";
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
            var pendingList = (sortedVals.list).Cast<DMDeptViewModel>().Where(x => x.Dept_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.Dept_MasterID).ToList() : new List<int>(),
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
            ViewData["CheckSeriesToSortParm"] = sortOrder == "chk_serires_to_desc" ? "chk_serires_to" : "chk_serires_to_desc";
            ViewData["CheckBankSortParm"] = sortOrder == "chk_bank_desc" ? "chk_bank" : "chk_bank_desc";
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
            var pendingList = (sortedVals.list).Cast<DMCheckViewModel>().Where(x => x.Check_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();
            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.Check_MasterID).ToList() : new List<int>(),
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
            ViewData["AccountBudgetCodeSortParm"] = sortOrder == "acc_budget_code_desc" ? "acc_budget_code" : "acc_budget_code_desc";
            ViewData["AccountNumberSortParm"] = sortOrder == "acc_no_desc" ? "acc_no" : "acc_no_desc";
            ViewData["AccountCustSortParm"] = sortOrder == "acc_cust_desc" ? "acc_cust" : "acc_cust_desc";
            ViewData["AccountDivSortParm"] = sortOrder == "acc_div_desc" ? "acc_div" : "acc_div_desc";
            ViewData["AccountFundSortParm"] = sortOrder == "acc_fund_desc" ? "acc_fund" : "acc_fund_desc";
            ViewData["AccountFBTSortParm"] = sortOrder == "acc_fbt_desc" ? "acc_fbt" : "acc_fbt_desc";
            ViewData["AccountCreatorSortParm"] = sortOrder == "acc_creatr_desc" ? "acc_creatr" : "acc_creatr_desc";
            ViewData["AccountApproverSortParm"] = sortOrder == "acc_approvr_desc" ? "acc_approvr" : "acc_approvr_desc";
            ViewData["AccountLastUpdatedSortParm"] = sortOrder == "acc_last_updte_desc" ? "acc_last_updte" : "acc_last_updte_desc";
            ViewData["AccountGrpSortParm"] = sortOrder == "acc_grp_desc" ? "acc_grp" : "acc_grp_desc";
            ViewData["AccountCurrencySortParm"] = sortOrder == "acc_curr_desc" ? "acc_curr" : "acc_curr_desc";
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
            var pendingList = (sortedVals.list).Cast<DMAccountViewModel>().Where(x => x.Account_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.Account_MasterID).ToList() : new List<int>(),
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
            var pendingList = (sortedVals.list).Cast<DMVATViewModel>().Where(x => x.VAT_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.VAT_MasterID).ToList() : new List<int>(),
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
            var pendingList = (sortedVals.list).Cast<DMFBTViewModel>().Where(x => x.FBT_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.FBT_MasterID).ToList() : new List<int>(),
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
            ViewData["TRStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "tr_stat" : "";
            ViewData["TRTaxRateSortParm"] = sortOrder == "tr_tax_desc" ? "tr_tax" : "tr_tax_desc";
            ViewData["TRIncomeSortParm"] = sortOrder == "tr_income_desc" ? "tr_income" : "tr_income_desc";
            ViewData["TRATCSortParm"] = sortOrder == "tr_atc_desc" ? "tr_atc" : "tr_atc_desc";
            ViewData["TRWTTileSortParm"] = sortOrder == "tr_title_desc" ? "tr_title" : "tr_title_desc";
            ViewData["TRCreatorSortParm"] = sortOrder == "tr_creatr_desc" ? "tr_creatr" : "tr_creatr_desc";
            ViewData["TRApproverSortParm"] = sortOrder == "tr_approvr_desc" ? "tr_approvr" : "tr_approvr_desc";
            ViewData["TRLastUpdteSortParm"] = sortOrder == "tr_last_updte_desc" ? "tr_last_updte" : "tr_last_updte_desc";
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
            var pendingList = (sortedVals.list).Cast<DMTRViewModel>().Where(x => x.TR_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.TR_MasterID).ToList() : new List<int>(),
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
            var pendingList = (sortedVals.list).Cast<DMCurrencyViewModel>().Where(x => x.Curr_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.Curr_MasterID).ToList() : new List<int>(),
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
            ViewData["RegEmpCodeSortParm"] = sortOrder == "regemp_no_desc" ? "regemp_no" : "regemp_no_desc";
            ViewData["RegEmpCatSortParm"] = sortOrder == "regemp_cat_desc" ? "regemp_cat" : "regemp_cat_desc";
            ViewData["RegEmpFbtSortParm"] = sortOrder == "regemp_fbt_desc" ? "regemp_fbt" : "regemp_fbt_desc";
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
            var pendingList = (sortedVals.list).Cast<DMEmpViewModel>().Where(x => x.Emp_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.Emp_MasterID).ToList() : new List<int>(),
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
            ViewData["TempEmpCatSortParm"] = sortOrder == "tempemp_cat_desc" ? "tempemp_cat" : "tempemp_cat_desc";
            ViewData["TempEmpFbtSortParm"] = sortOrder == "tempemp_fbt_desc" ? "tempemp_fbt" : "tempemp_fbt_desc";
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
            var pendingList = (sortedVals.list).Cast<DMEmpViewModel>().Where(x => x.Emp_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.Emp_MasterID).ToList() : new List<int>(),
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
            var pendingList = (sortedVals.list).Cast<DMCustViewModel>().Where(x => x.Cust_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.Cust_MasterID).ToList() : new List<int>(),
                Cust = PaginatedList<DMCustViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMCustViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
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
            var pendingList = (sortedVals.list).Cast<DMBCSViewModel>().Where(x => x.BCS_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.BCS_MasterID).ToList() : new List<int>(),
                BCS = PaginatedList<DMBCSViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMBCSViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

        [Route("/Partial/DMPartial_AccGroup/")]
        public IActionResult DMPartial_AccGroup(string sortOrder, string currentFilter, string colName, string searchString, string page)
        {
            var userId = _session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int? pg = (page == null) ? 1 : int.Parse(page);
            //set sort vals
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AGStatusSortParm"] = String.IsNullOrEmpty(sortOrder) ? "ag_stat" : "";
            ViewData["AGCodeSortParm"] = sortOrder == "ag_code_desc" ? "ag_code" : "ag_code_desc";
            ViewData["AGCreatorSortParm"] = sortOrder == "ag_creatr_desc" ? "ag_creatr" : "ag_creatr_desc";
            ViewData["AGApproverSortParm"] = sortOrder == "ag_approvr_desc" ? "ag_approvr" : "ag_approvr_desc";
            ViewData["AGLastUpdatedSortParm"] = sortOrder == "ag_last_updte_desc" ? "ag_last_updte" : "ag_last_updte_desc";
            ViewData["AGSortParm"] = sortOrder == "name_desc" ? "name" : "name_desc";

            if (searchString != null) { pg = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            DMFiltersViewModel filters = new DMFiltersViewModel();
            if (TempData.ContainsKey("filters"))
            {
                filters = (DMFiltersViewModel)TempData["filters"];
            }

            //populate and sort
            var sortedVals = _sortService.SortData(_service.populateAG(filters), sortOrder);
            ViewData[sortedVals.viewData] = sortedVals.viewDataInfo;
            var pendingList = (sortedVals.list).Cast<DMAccountGroupViewModel>().Where(x => x.AccountGroup_Status_ID != GlobalSystemValues.STATUS_APPROVED).ToList();

            //pagination
            DMViewModel VM = new DMViewModel()
            {
                DMFilters = filters,
                PendingMasterIDList = pendingList.Count > 0 ? pendingList.Select(x => x.AccountGroup_MasterID).ToList() : new List<int>(),
                AccountGroup = PaginatedList<DMAccountGroupViewModel>.CreateAsync(
                    (sortedVals.list).Cast<DMAccountGroupViewModel>().AsQueryable().AsNoTracking(), pg ?? 1, pageSize)
            };
            return View(VM);
        }

    }
}