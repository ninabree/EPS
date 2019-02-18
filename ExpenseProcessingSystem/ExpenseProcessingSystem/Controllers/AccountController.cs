using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpenseProcessingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public AccountController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        [HttpGet]
        public ActionResult Login()
        {
            //sample error log
            try
            {
                //some codes
            }catch(Exception ex)
            {
                Log.Error(ex, "User: {user}, StackTrace : {trace}, Error Message: {message}", "UserID", ex.StackTrace, ex.Message);
                return RedirectToAction("Index", "Error");
            }
            finally
            {
                //required to trigger the write log to email
                Log.CloseAndFlush();
            }

            LoginViewModel lvm = new LoginViewModel();
            return View(lvm);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (CryptoTools.getHashPasswd("PLACEHOLDER", model.Acc_UserName, model.Acc_Password) == "468C042885E02E767E2EE567EC21A860F88C08BF537A8EEF9BB042D5A4E638E0F3DEBFB33F86D895F0DDE41218AC2DEA10E8C2AFA011E9BA98755781538B5232")
            {
                Log.Information("User Logged In");
                //SetSession Info
                _session.SetString("UN", model.Acc_UserName);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }
    }
}