using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            var acc = _context.Account.Where(x => x.Acc_UserName == model.Acc_UserName).Select(x => x).FirstOrDefault();
            if (acc != null){
                if (CryptoTools.getHashPasswd("PLACEHOLDER", model.Acc_UserName, model.Acc_Password) == acc.Acc_Password)
                {
                    Log.Information("User Logged In");
                    //SetSession Info
                    _session.SetString("UserID", acc.Acc_UserID.ToString());
                    return RedirectToAction("Index", "Home");
                }
                return View(model);
            }
            else
            {
                ModelState.TryAddModelError(string.Empty,"Invalid Login Credentials");
                return View(model);
            }
        }
    }
}