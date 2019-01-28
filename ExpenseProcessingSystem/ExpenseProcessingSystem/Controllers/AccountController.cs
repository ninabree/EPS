﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExpenseProcessingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly DbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public AccountController(IHttpContextAccessor httpContextAccessor/*, DbContext context*/)
        {
            _httpContextAccessor = httpContextAccessor;
            //_context = context;
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
            if (CryptoTools.getHashPasswd("PLACEHOLDER", model.Acc_UserName, model.Acc_Password) == "4C54DE81642F796E39C11F4AD92187707427A6780C65B9BB92403B962C99E6A702E9D829439B41094D645CDC043836EA0B46E8D60B474C4E35DC439623B13F98")
            {
                Log.Information("Valid Login Cred");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }
    }
}