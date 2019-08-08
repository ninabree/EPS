using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Diagnostics;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.ViewModels;
using System.DirectoryServices.AccountManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;

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
            AccessViewModel accessVM = new AccessViewModel
            {
                isLoggedIn = false,
                accessType = "",
                isAdmin = false
            };
            ViewBag.access = accessVM;
            LoginViewModel lvm = new LoginViewModel();
            return View(lvm);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            AccessViewModel accessVM = new AccessViewModel
            {
                isLoggedIn = false,
                accessType = "",
                isAdmin = false
            };
            ViewBag.access = accessVM;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = ".Net Runtime";
                eventLog.WriteEntry("Express: Express has started", EventLogEntryType.Information, 1000, 1);
            }
            //START OF LDAP LOGIN
            //try
            //{
            //    //string svcUsername = "tuser";
            //    //string domain = "maniladev.mizuho.com";
            //    //string svcPwd = "password@99";
            //    string svcUsername = "gene.delacruz";
            //    string domain = "jpi.local";
            //    string svcPwd = "genecarlod";
                //check if user is existing in Active Directory
                //using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, svcUsername, svcPwd))
                //{
                //    using (UserPrincipal user = UserPrincipal.FindByIdentity(context, model.User_UserName))
                //    {
                //        if (user != null)
                //        {
                //            //Set Session Info
                //            _session.SetString("UserID", model.User_UserName);
                //            //Set User Access Info
                //            _session.SetString("isLoggedIn", "true");
                //            _session.SetString("accessType", "admin");
                //            _session.SetString("isAdmin", "admin" == "admin" ? "true" : "false");
                //            ViewData["message"] = "well shit";
                //            Log.Information("User Logged In");
                //            return RedirectToAction("Index", "Home");
                //        }
                //    }
                //}
                //validate the user's username & password in Active Directory
            //    using (var context = new PrincipalContext(ContextType.Domain, domain, svcUsername, svcPwd))
            //    {
            //        //Username and password for authentication.
            //        bool rslt = context.ValidateCredentials(model.User_UserName, model.User_Password);
            //        if (rslt)
            //        {
            //            //Set Session Info
            //            _session.SetString("UserID", model.User_UserName);
            //            //Set User Access Info
            //            _session.SetString("isLoggedIn", "true");
            //            _session.SetString("accessType", "admin");
            //            _session.SetString("isAdmin", "admin" == "admin" ? "true" : "false");
            //            ViewData["message"] = "got in the fourth loop";
            //            Log.Information("User Logged In");
            //            return RedirectToAction("Index", "Home");
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    using (EventLog eventLog = new EventLog("Application"))
            //    {
            //        eventLog.Source = ".Net Runtime";
            //        eventLog.WriteEntry("Express:" + e.Message, EventLogEntryType.Information, 1000, 1);
            //        eventLog.WriteEntry("Express:" + e.InnerException.Message, EventLogEntryType.Information, 1000, 1);
            //    }
            //}
            //END OF LDAP LOGIN

            var acc = _context.User.Where(x => x.User_UserName == model.User_UserName).Where(x => x.User_InUse == true).Select(x => x).FirstOrDefault();
            if (acc != null)
            {
                if (CryptoTools.getHashPasswd("PLACEHOLDER", model.User_UserName, model.User_Password) == acc.User_Password)
                {
                    //Set Session Info
                    _session.SetString("UserID", acc.User_ID.ToString());
                    //Set User Access Info
                    _session.SetString("isLoggedIn", "true");
                    _session.SetString("accessType", acc.User_Role);
                    _session.SetString("isAdmin", acc.User_Role == "admin" ? "true" : "false");

                    Log.Information("User Logged In");
                    if (acc.User_Role == "admin")
                    {
                        return RedirectToAction("UM", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ViewData["message"] += "There's an error but you can't see me";
            ModelState.AddModelError("", "Invalid Login Credential");
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _session.Clear();

            return RedirectToAction("Login", "Account");
        }

        public JsonResult checkSession()
        {
            var tmp = (_session.GetString("UserID") == null) ? true : false;
            return Json(tmp);
        }
    }
}