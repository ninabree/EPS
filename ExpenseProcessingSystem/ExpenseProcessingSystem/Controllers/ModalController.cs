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
        //DM Approve Payee
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
        //DM Add Payee
        public IActionResult DMAddPayee_Pending()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addPayee());
        }
        //APPROVED
        //DM Add Payee
        public IActionResult DMAddPayee()
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_service.addPayee());
        }
        //DM Edit Payee
        public IActionResult DMEditPayee(string[] IdsArr)
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
        public IActionResult DMDeletePayee(string[] IdsArr)
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
        //DM Add Dept
        public IActionResult DMAddDept()
        {
            return View();
        }
        //DM Edit Payee
        public IActionResult DMEditDept(string[] IdsArr)
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

        public IActionResult DMDeleteDept(string[] IdsArr)
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