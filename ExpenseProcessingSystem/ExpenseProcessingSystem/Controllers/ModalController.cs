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
        //DM Approve Payee
        public IActionResult DMApprovePayee(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMPayeeViewModel> vmList = new List<DMPayeeViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approvePayee(IdsArr);
            }
            return View(vmList);
        }
        //DM Reject Payee
        public IActionResult DMRejPayee(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMPayeeViewModel> vmList = new List<DMPayeeViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectPayee(IdsArr);
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
            List<DMCheckViewModel> vmList = new List<DMCheckViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectCheck(IdsArr);
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
        //DM EWT
        public IActionResult DMApproveEWT(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEWTViewModel> vmList = new List<DMEWTViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.approveEWT(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMRejEWT(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEWTViewModel> vmList = new List<DMEWTViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.rejectEWT(IdsArr);
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
        //___________________Pending Functions___________________________
        //DM Payee
        public IActionResult DMAddPayee_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addPayee());
        }
        public IActionResult DMEditPayee_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMPayeeViewModel> vmList = new List<DMPayeeViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeletePayee(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeletePayee_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMPayeeViewModel> vmList = new List<DMPayeeViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeletePayee(IdsArr);
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
        //DM EWT
        public IActionResult DMAddEWT_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addEWT());
        }
        public IActionResult DMEditEWT_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEWTViewModel> vmList = new List<DMEWTViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteEWT(IdsArr);
            }
            return View(vmList);
        }
        public IActionResult DMDeleteEWT_Pending(string[] IdsArr)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMEWTViewModel> vmList = new List<DMEWTViewModel>();
            if (ModelState.IsValid)
            {
                vmList = _service.editDeleteEWT(IdsArr);
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
    }
}