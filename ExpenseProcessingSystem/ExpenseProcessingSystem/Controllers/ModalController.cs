using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseProcessingSystem.Controllers
{
    public class ModalController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private readonly GWriteContext _gWriteContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private ModalService _service;

        public ModalController(IHttpContextAccessor httpContextAccessor, EPSDbContext context, GWriteContext gWriteContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _gWriteContext = gWriteContext;
            _service = new ModalService(_httpContextAccessor, _context, _gWriteContext);
        }

        //Entry_DDV
        public IActionResult ReversalEntryModal()
        {
            return View();
        }

        //[ Budget Monitoring ]
        //Open Budget registration screen
        public IActionResult BudgetRegistrationModal()
        {
            var accountList = _service.GetAccountListForBudgetMonitoring();
            var budgetList = _service.GetAllCurrentBudget();
            List<BMViewModel> vm = new List<BMViewModel>();

            foreach(var i in accountList)
            {

                var bud = budgetList.Where(x => x.Budget_Account_MasterID == i.Account_MasterID).FirstOrDefault();

                vm.Add(new BMViewModel {
                    BM_Account_ID = i.Account_ID,
                    BM_Account_MasterID = i.Account_MasterID,
                    BM_Acc_Name = i.Account_Name,
                    BM_Acc_Num = i.Account_No,
                    BM_GBase_Code = i.Account_Budget_Code,
                    BM_Budget_Current = (bud != null) ? bud.Budget_Amount : 0,
                    BM_Budget_Amount = (bud != null) ? bud.Budget_New_Amount : 0,
                    BM_GWrite_StatusID = (bud != null) ? bud.Budget_GWrite_Status : GlobalSystemValues.STATUS_APPROVED
                });
            }

            return View(vm);
        }

        //Open history of Budget Registration screen
        public IActionResult BudgetRegHistModal(int? year)
        {
            if (year.HasValue)
            {
                return View(new BMRegHistViewModel
                {
                    BMVM = _service.PopulateBudgetRegHist(year),
                    Year = year.GetValueOrDefault(),
                    YearList = ConstantData.HomeReportConstantValue.GetYearList()
                });
            }
            else
            {
                return View(new BMRegHistViewModel
                {
                    BMVM = _service.PopulateBudgetRegHist(DateTime.Now.Year),
                    YearList = ConstantData.HomeReportConstantValue.GetYearList()
                });
            }
        }

        //Register new budget from Budget Monitoring screen
        public IActionResult RegisterNewBudget(List<BMViewModel> vmList, string username, string password, string BudgetCommand)
        {
            var userId = GetUserID();

            if(BudgetCommand == "NEW")
            {
                _service.AddNewBudget(vmList, int.Parse(GetUserID()), username, password);
            }
            else
            {
                _service.ReSendNewBudget(int.Parse(GetUserID()), username, password);
            }
            
            return RedirectToAction("BM", "Home");
        }

        // [FOR APPROVAL]
        //DM Vendor
        public IActionResult DMApproveVendor(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMVendorViewModel> vmList = new List<DMVendorViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveVendor(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejVendor(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMVendorViewModel> vmList = new List<DMVendorViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectVendor(IdsArr);
            }
            return View(vmList);
        }
        //DM Department
        public IActionResult DMApproveDept(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveDept(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejDept(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectDept(IdsArr);
            }
            return View(vmList);
        }
        //DM Check
        public IActionResult DMApproveCheck(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCheckViewModel> vmList = new List<DMCheckViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveCheck(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejCheck(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCheckViewModel> vmList = new List<DMCheckViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectCheck(IdsArr);
            }
            return View(vmList);
        }
        //DM Account
        public IActionResult DMApproveAccount(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMAccountViewModel> vmList = new List<DMAccountViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveAccount(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejAccount(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMAccountViewModel> vmList = new List<DMAccountViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectAccount(IdsArr);
            }
            return View(vmList);
        }
        //DM Account Group
        public IActionResult DMApproveAccountGroup(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMAccountGroupViewModel> vmList = new List<DMAccountGroupViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveAccountGroup(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejAccountGroup(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMAccountGroupViewModel> vmList = new List<DMAccountGroupViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectAccountGroup(IdsArr);
            }
            return View(vmList);
        }
        //DM VAT
        public IActionResult DMApproveVAT(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMVATViewModel> vmList = new List<DMVATViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveVAT(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejVAT(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMVATViewModel> vmList = new List<DMVATViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectVAT(IdsArr);
            }
            return View(vmList);
        }
        //DM FBT
        public IActionResult DMApproveFBT(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMFBTViewModel> vmList = new List<DMFBTViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveFBT(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejFBT(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMFBTViewModel> vmList = new List<DMFBTViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectFBT(IdsArr);
            }
            return View(vmList);
        }
        //DM TR
        public IActionResult DMApproveTR(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMTRViewModel> vmList = new List<DMTRViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveTR(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejTR(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMTRViewModel> vmList = new List<DMTRViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectTR(IdsArr);
            }
            return View(vmList);
        }
        //DM Curr
        public IActionResult DMApproveCurr(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCurrencyViewModel> vmList = new List<DMCurrencyViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveCurr(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejCurr(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCurrencyViewModel> vmList = new List<DMCurrencyViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectCurr(IdsArr);
            }
            return View(vmList);
        }
        //DM Regular Employee
        public IActionResult DMApproveRegEmp(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveEmp(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejRegEmp(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectEmp(IdsArr);
            }
            return View(vmList);
        }
        //DM Temporary Employee
        public IActionResult DMApproveTempEmp(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveEmp(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejTempEmp(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectEmp(IdsArr);
            }
            return View(vmList);
        }
        //DM Customer
        public IActionResult DMApproveCust(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCustViewModel> vmList = new List<DMCustViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveCust(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejCust(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCustViewModel> vmList = new List<DMCustViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectCust(IdsArr);
            }
            return View(vmList);
        }
        //DM BIR Cert Signatory
        public IActionResult DMApproveBCS(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMBCSViewModel> vmList = new List<DMBCSViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveBCS(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejBCS(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMBCSViewModel> vmList = new List<DMBCSViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectBCS(IdsArr);
            }
            return View(vmList);
        }
        //___________________Pending Functions___________________________
        //DM Vendor
        public IActionResult DMAddVendor_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addVendor());
        }
        public IActionResult DMEditVendor_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMVendorViewModel> vmList = new List<DMVendorViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteVendor(IdsArr);
                ViewBag.trList = _service.getTRList();
                ViewBag.vatList = _service.getVATList();
            }
            return View(vmList);
        }
        public IActionResult DMDeleteVendor_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMVendorViewModel> vmList = new List<DMVendorViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteVendor(IdsArr);
            }
            return View(vmList);
        }
        // DM Department
        public IActionResult DMAddDept_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addDept());
        }
        public IActionResult DMEditDept_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteDept(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeleteDept_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteDept(IdsArr);
            }
            return View(vmList);
        }
        //DM Check
        public IActionResult DMAddCheck_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addCheck());
        }
        public IActionResult DMEditCheck_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCheckViewModel> vmList = new List<DMCheckViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteCheck(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeleteCheck_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCheckViewModel> vmList = new List<DMCheckViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteCheck(IdsArr);
            }
            return View(vmList);
        }
        //DM Account
        public IActionResult DMAddAccount_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addAccount());
        }
        public IActionResult DMEditAccount_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMAccountViewModel> vmList = new List<DMAccountViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteAccount(IdsArr);
                ViewBag.fbtList = _service.getFbtSelectList();
                ViewBag.grpList = _service.getAccGroupSelectList();
                ViewBag.currList = _service.getCurrencySelectList();
            }
            return View(vmList);
        }
        public IActionResult DMDeleteAccount_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMAccountViewModel> vmList = new List<DMAccountViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteAccount(IdsArr);
            }
            return View(vmList);
        }
        //DM Account Group
        public IActionResult DMAddAccountGroup_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addAccountGroup());
        }
        public IActionResult DMEditAccountGroup_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMAccountGroupViewModel> vmList = new List<DMAccountGroupViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteAccountGroup(IdsArr);
                ViewBag.fbtList = _service.getFbtSelectList();
            }
            return View(vmList);
        }
        public IActionResult DMDeleteAccountGroup_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMAccountGroupViewModel> vmList = new List<DMAccountGroupViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteAccountGroup(IdsArr);
            }
            return View(vmList);
        }
        //DM VAT
        public IActionResult DMAddVAT_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "VAT");
            }
            return View(_service.addVAT());
        }
        public IActionResult DMEditVAT_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "VAT");
            }
            List<DMVATViewModel> vmList = new List<DMVATViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteVAT(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeleteVAT_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "VAT");
            }
            List<DMVATViewModel> vmList = new List<DMVATViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteVAT(IdsArr);
            }
            return View(vmList);
        }
        //DM FBT
        public IActionResult DMAddFBT_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addFBT());
        }
        public IActionResult DMEditFBT_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMFBTViewModel> vmList = new List<DMFBTViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteFBT(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeleteFBT_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMFBTViewModel> vmList = new List<DMFBTViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteFBT(IdsArr);
            }
            return View(vmList);
        }
        //DM TR
        public IActionResult DMAddTR_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addTR());
        }
        public IActionResult DMEditTR_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMTRViewModel> vmList = new List<DMTRViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteTR(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeleteTR_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMTRViewModel> vmList = new List<DMTRViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteTR(IdsArr);
            }
            return View(vmList);
        }
        //DM Curr
        public IActionResult DMAddCurr_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addCurr());
        }
        public IActionResult DMEditCurr_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCurrencyViewModel> vmList = new List<DMCurrencyViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteCurr(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeleteCurr_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCurrencyViewModel> vmList = new List<DMCurrencyViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteCurr(IdsArr);
            }
            return View(vmList);
        }
        //DM Regular Employee
        public IActionResult DMAddRegEmp_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addEmp());
        }
        public IActionResult DMEditRegEmp_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteEmp(IdsArr);
                ViewBag.fbtList = _service.getFbtSelectList();
                ViewBag.catList = _service.getEmpCategorySelectList();
            }
            return View(vmList);
        }
        public IActionResult DMDeleteRegEmp_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteEmp(IdsArr);
            }
            return View(vmList);
        }
        //DM Temporary Employee
        public IActionResult DMAddTempEmp_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addEmp());
        }
        public IActionResult DMEditTempEmp_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteEmp(IdsArr);
                ViewBag.fbtList = _service.getFbtSelectList();
                ViewBag.catList = _service.getEmpCategorySelectList();
            }
            return View(vmList);
        }
        public IActionResult DMDeleteTempEmp_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteEmp(IdsArr);
            }
            return View(vmList);
        }
        //DM Customer
        public IActionResult DMAddCust_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addCust());
        }
        public IActionResult DMEditCust_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCustViewModel> vmList = new List<DMCustViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteCust(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeleteCust_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMCustViewModel> vmList = new List<DMCustViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteCust(IdsArr);
            }
            return View(vmList);
        }
        //DM BIR Cert Signatory
        public IActionResult DMAddBCS_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.empList = _service.getEmpSelectList();
            return View(_service.addBCS());
        }
        public IActionResult DMEditBCS_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            DMBCS2ViewModel vmList = new DMBCS2ViewModel();
            if (ModelState.IsValid)
            {
                vmList = _service.editBCS(IdsArr);
                ViewBag.empList = _service.getEmpSelectList();
            }
            return View(vmList);
        }
        public IActionResult DMDeleteBCS_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMBCSViewModel> vmList = new List<DMBCSViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.deleteBCS(IdsArr);
            }
            return View(vmList);
        }
        //___________________________//[ACCOUNT]//_______________________________
        //Forgot Login Credentials
        public IActionResult ForgotPW()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ForgotPWViewModel vmList = new ForgotPWViewModel();
            return View(vmList);
        }
        //_________________________//[Check Expense]//_____________________________
        //Expense Amortization
        public IActionResult EntryExpenseAmortization(string id, string vendor, string account, int month, int day, int duration)
        {
            CVAmortizationViewModel model = new CVAmortizationViewModel();

            model.id = id;
            model.vendor = vendor;
            model.account = account;
            model.month = month;
            model.day = day;
            model.duration = duration;

            return PartialView(model);
        }
        public IActionResult EntryExpenseInterEntity(string id, string interRate, string account, string remarksTitle, string Curr1AbbrID, string Curr1AbbrName, string Curr1Amt,
                                                    string Curr2AbbrID, string Curr2AbbrName, string Curr2Amt, string Chk1, string Conv1Amt, string Chk2, string Conv2Amt, 
                                                    string accID)
        {
            InterEntityParticular par = new InterEntityParticular();
            float InterRate = interRate != null ? float.Parse(interRate) : 1;
            string remarks = remarksTitle + " " + ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == DateTime.Now.Month).Single().MonthName + " " + DateTime.Now.Year;

            List<SelectListItem> currList = _service.getCurrencyIDSelectList(int.Parse(Curr1AbbrID));
            Curr2AbbrID = Curr2AbbrID == "0" ? currList.Select(x => x.Value).FirstOrDefault() : Curr2AbbrID;
            List<InterEntityParticular> parList1 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular1(account, Curr1AbbrName, decimal.Parse(Curr1Amt), decimal.Parse(Curr2Amt), InterRate, int.Parse(accID), int.Parse(Curr1AbbrID), _service.getInterEntityAccs("/INTERENTITYACCOUNTS/ACCOUNT[@class='entry1']"));
            List<InterEntityParticular> parList2 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular2(Curr1AbbrName, Curr2AbbrName, decimal.Parse(Curr2Amt), InterRate, int.Parse(Curr1AbbrID), int.Parse(Curr2AbbrID), _service.getInterEntityAccs("/INTERENTITYACCOUNTS/ACCOUNT[@class='entry2']"));
            List<InterEntityParticular> parList3 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular3(Curr2AbbrName, decimal.Parse(Curr2Amt), int.Parse(Curr2AbbrID), _service.getInterEntityAccs("/INTERENTITYACCOUNTS/ACCOUNT[@class='entry3']"));
            
            DDVInterEntityViewModel model = new DDVInterEntityViewModel
            {
                Inter_ID = int.Parse(id.Substring(5)),
                Inter_Currency1_ID = int.Parse(Curr1AbbrID),
                Inter_Currency1_ABBR = Curr1AbbrName,
                Inter_Currency1_Amount = decimal.Parse(Curr1Amt),
                Inter_Currency2_ID = Curr2AbbrID == "0" ? int.Parse(currList.Select(x => x.Value).FirstOrDefault()) : int.Parse(Curr2AbbrID),
                Inter_Currency2_ABBR = Curr2AbbrID == "0" ? currList.Select(x => x.Text).FirstOrDefault() : Curr2AbbrName,
                Inter_Currency2_Amount = decimal.Parse(Curr2Amt),
                Inter_Rate = InterRate,
                Inter_Check1 = Chk1 == "True" ? true : Chk1 == "true" ? true : false,
                Inter_Check2 = Chk2 == "True" ? true : Chk2 == "true" ? true : false,
                Inter_Convert1_Amount = decimal.Parse(Conv1Amt),
                Inter_Convert2_Amount = decimal.Parse(Conv2Amt),
                interPartList = new List<ExpenseEntryInterEntityParticularViewModel>(),
                CurrencyList = currList
            };
            ExpenseEntryInterEntityParticularViewModel particular = new ExpenseEntryInterEntityParticularViewModel {
                InterPart_Particular_Title = remarks,
                Inter_Particular1 = parList1,
                Inter_Particular2 = parList2,
                Inter_Particular3 = parList3
            };
            model.interPartList.Add(particular);
            return PartialView(model);
        }
        public IActionResult EntryExpenseInterEntity_READONLY(string id, string interRate, string account, string remarksTitle, string Curr1AbbrID, string Curr1AbbrName, string Curr1Amt,
                                                    string Curr2AbbrID, string Curr2AbbrName, string Curr2Amt, string Chk1, string Conv1Amt, string Chk2, string Conv2Amt,
                                                    string accID)
        {
            InterEntityParticular par = new InterEntityParticular();
            float InterRate = interRate != null ? float.Parse(interRate) : 1;
            string remarks = remarksTitle + " " + ConstantData.HomeReportConstantValue.GetMonthList().Where(c => c.MonthID == DateTime.Now.Month).Single().MonthName + " " + DateTime.Now.Year;

            List<InterEntityParticular> parList1 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular1(account, Curr1AbbrName, decimal.Parse(Curr1Amt), decimal.Parse(Curr2Amt), InterRate, int.Parse(accID), int.Parse(Curr1AbbrID), _service.getInterEntityAccs("/INTERENTITYACCOUNTS/ACCOUNT[@class='entry1']"));
            List<InterEntityParticular> parList2 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular2(Curr1AbbrName, Curr2AbbrName, decimal.Parse(Curr2Amt), InterRate, int.Parse(Curr1AbbrID), int.Parse(Curr2AbbrID), _service.getInterEntityAccs("/INTERENTITYACCOUNTS/ACCOUNT[@class='entry2']"));
            List<InterEntityParticular> parList3 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular3(Curr2AbbrName, decimal.Parse(Curr2Amt), int.Parse(Curr2AbbrID), _service.getInterEntityAccs("/INTERENTITYACCOUNTS/ACCOUNT[@class='entry3']"));
            List<SelectListItem> currList = _service.getCurrencySelectList();

            DDVInterEntityViewModel model = new DDVInterEntityViewModel
            {
                Inter_ID = int.Parse(id.Substring(5)),
                Inter_Currency1_ID = int.Parse(Curr1AbbrID),
                Inter_Currency1_ABBR = Curr1AbbrName,
                Inter_Currency1_Amount = decimal.Parse(Curr1Amt),
                Inter_Currency2_ID = Curr2AbbrID == "NAN" ? 2 : int.Parse(Curr2AbbrID),
                Inter_Currency2_ABBR = Curr2AbbrID == "NAN" ? _context.DMCurrency.Where(x => x.Curr_MasterID == 2 && x.Curr_isActive == true && x.Curr_isDeleted == false).Select(x => x.Curr_CCY_ABBR).FirstOrDefault() : Curr2AbbrName,
                Inter_Currency2_Amount = decimal.Parse(Curr2Amt),
                Inter_Rate = InterRate,
                Inter_Check1 = Chk1 == "True" ? true : Chk1 == "true" ? true : false,
                Inter_Check2 = Chk2 == "True" ? true : Chk2 == "true" ? true : false,
                Inter_Convert1_Amount = decimal.Parse(Conv1Amt),
                Inter_Convert2_Amount = decimal.Parse(Conv2Amt),
                interPartList = new List<ExpenseEntryInterEntityParticularViewModel>(),
                CurrencyList = currList
            };
            ExpenseEntryInterEntityParticularViewModel particular = new ExpenseEntryInterEntityParticularViewModel
            {
                InterPart_Particular_Title = remarks,
                Inter_Particular1 = parList1,
                Inter_Particular2 = parList2,
                Inter_Particular3 = parList3
            };
            model.interPartList.Add(particular);
            return PartialView(model);
        }
        //_________________________//[Petty Cash/Cash Advance(Suspense sundry) Expense]//_____________________________
        //Expense Cash Breakdown
        public IActionResult EntryExpenseCashBreakdown(string id, string vendor, string account, decimal amount, string screencode)
        {
            PCVCashBreakdownViewModel model = new PCVCashBreakdownViewModel();

            model.id = id;
            model.vendor = vendor;
            model.accountName = account;
            model.amount = amount;
            model.screencode = screencode;
            model.cashBreakdown = new List<ExpenseEntryCashBreakdownModel>();


            foreach (var i in DenominationValues.GetDenominationList())
            {
                model.cashBreakdown.Add(
                    new ExpenseEntryCashBreakdownModel
                    {
                        CashBreak_Denomination = i.CashBreak_Denomination
                    });
            }

            return PartialView(model);
        }

        public IActionResult EntryGbaseRemarks(string[] IdsArr)
        {
            ViewBag.parentID = IdsArr[0];
            return PartialView();
        }

        public IActionResult EntryExpenseEWT(string id, string taxpayor)
        {
            var venList = _service.getVendorSelectList();
            DDVEWTViewModel model = new DDVEWTViewModel {
                table_ID = id,
                tax_payor = taxpayor ?? venList.Select(x => x.Value).FirstOrDefault(),
                vendor_list = venList
            };
            return PartialView(model);
        }

        public IActionResult ClosePettyCash(string command)
        {
            ClosingBrkDwnViewModel model = new ClosingBrkDwnViewModel();
            var pc = _service.getPC(command);
            switch (command)
            {
                case "StartPettyCash":
                    model.CBD_displayMode = true;
                    if (_service.lastPCEntry())
                        model.enableBtn = true;
                    else
                        model.enableBtn = false;
                    break;
                case "ClosePettyCash":
                    if(pc.PC_Status == GlobalSystemValues.STATUS_CLOSED)
                        model.enableBtn = false;
                    else
                        model.enableBtn = true;

                    model.CBD_displayMode = false;
                    model.CBD_ConfirmClose = true;
                    break;
            }
            if(pc != null)
            {
                model.CBD_Date = pc.PC_OpenDate;
                model.CBD_opeBalance = pc.PC_StartBal;
                model.CBD_recieve = pc.PC_Recieved;
                model.CBD_disburse = pc.PC_Disbursed;
                model.CBD_closeBalance = pc.PC_EndBal;

                model.CBD_oneK = pc.PCB_OneThousand;
                model.CBD_oneKAmount = pc.PCB_OneThousand * 1000;
                model.CBD_fiveH = pc.PCB_FiveHundred;
                model.CBD_fiveHAmount = pc.PCB_FiveHundred * 500;
                model.CBD_twoH = pc.PCB_TwoHundred;
                model.CBD_twoHAmount = pc.PCB_TwoHundred * 200;
                model.CBD_oneH = pc.PCB_OneHundred;
                model.CBD_oneHAmount = pc.PCB_OneHundred * 100;
                model.CBD_fifty = pc.PCB_Fifty;
                model.CBD_fiftyAmount = pc.PCB_Fifty * 50;
                model.CBD_twenty = pc.PCB_Twenty;
                model.CBD_twentyAmount = pc.PCB_Twenty * 20;
                model.CBD_ten = pc.PCB_Ten;
                model.CBD_tenAmount = pc.PCB_Ten * 10;
                model.CBD_five = pc.PCB_Five;
                model.CBD_fiveAmount = pc.PCB_Five * 5;
                model.CBD_one = pc.PCB_One;
                model.CBD_oneAmount = pc.PCB_One * 1;
                model.CBD_c25 = pc.PCB_TwentyFiveCents;
                model.CBD_c25Amount = (decimal)(pc.PCB_TwentyFiveCents * .25);
                model.CBD_c10 = pc.PCB_TenCents;
                model.CBD_c10Amount = (decimal)(pc.PCB_TenCents * .10);
                model.CBD_c5 = pc.PCB_FiveCents;
                model.CBD_c5Amount = (decimal)(pc.PCB_FiveCents * .05);
                model.CBD_c1 = pc.PCB_OneCents;
                model.CBD_c1Amount = (decimal)(pc.PCB_OneCents * .01);
            }
            else
            {
                model.message =  "There are no Pettycash records yet.";
            }
            
            return PartialView(model);
        }

        public IActionResult CloseConfirmPettyCash()
        {
            if (_service.confirmPC(int.Parse(GetUserID())))
            {
                TempData["closeMessage"] = "Petty cash confirmed, new petty cash day opened.";
                return RedirectToAction("Close", "Home");
            }
            TempData["closeMessage"] = "Failed to confirm petty cash.";
            return RedirectToAction("Close", "Home");
        }

        public JsonResult closeSveBrkDown(ClosingBrkDwnViewModel model)
        {
            if (_service.saveBrkDwnPC(model))
                return Json(true);
            else
                return Json(false);
        }

        private string GetUserID()
        {
            return _session.GetString("UserID");
        }
    }
}