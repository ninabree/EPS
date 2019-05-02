using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseProcessingSystem.Controllers
{
    public class ModalController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private ModalService _service;

        public ModalController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _service = new ModalService(_httpContextAccessor, _context);
        }

        //Entry_DDV
        public IActionResult ReversalEntryModal()
        {
            return View();
        }
        //BM
        public IActionResult BudgetAdjustmentModal()
        {
            return View();
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
        //DM Non Cash Category
        public IActionResult DMApproveNCC(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMNCCViewModel> vmList = new List<DMNCCViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveNCC(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejNCC(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMNCCViewModel> vmList = new List<DMNCCViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectNCC(IdsArr);
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
        //DM Non Cash Category
        public IActionResult DMAddNCC_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addNCC());
        }
        public IActionResult DMEditNCC_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            DMNCC2ViewModel vmList = new DMNCC2ViewModel();
            if (ModelState.IsValid)
            {
                vmList = _service.editNCC(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeleteNCC_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMNCCViewModel> vmList = new List<DMNCCViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.deleteNCC(IdsArr);
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

        public IActionResult EntryGbaseRemarks(string[] IdsArr)
        {
            ViewBag.parentID = IdsArr[0];
            return PartialView();
        }
    }
}