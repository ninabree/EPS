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
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace ExpenseProcessingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger,IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _logger = logger;
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

            ////START OF LDAP LOGIN
            try
            {
                XElement xelem = XElement.Load("wwwroot/xml/ActiveDirectory.xml");

                string svcUsername = xelem.Element("svcUsername").Value;
                string domain = xelem.Element("domain").Value;
                string svcPwd = EncrytionTool.DecryptString(xelem.Element("svcPwd").Value, "eXpreSS");

                //validate the user's username & password in Active Directory
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, svcUsername, svcPwd))
                {
                    //Username and password for authentication.
                    bool rslt = context.ValidateCredentials(model.User_UserName, model.User_Password);
                    if (rslt)
                    {
                        var acc = _context.User.Where(x => x.User_UserName == model.User_UserName).Where(x => x.User_InUse == true).Select(x => x).FirstOrDefault();
                        if (acc != null)
                        {
                            //Set Session Info
                            _session.SetString("UserID", acc.User_ID.ToString());
                            _session.SetString("UserName", acc.User_FName + " " + acc.User_LName);
                            //Set User Access Info
                            _session.SetString("isLoggedIn", "true");
                            _session.SetString("accessType", acc.User_Role);
                            _session.SetString("isAdmin", acc.User_Role == "admin" ? "true" : "false");
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User [" + model.User_UserName + "] has encountered a system error at [" + DateTime.Now + "].");
                return StatusCode(500);
            }

            //_session.SetString("UserID", "1");
            //_session.SetString("UserName", "test user");
            ////Set User Access Info
            //_session.SetString("isLoggedIn", "true");
            //_session.SetString("accessType", "approver");
            //_session.SetString("isAdmin", "false");
            //return RedirectToAction("Index", "Home");
            ////END OF LDAP LOGIN

            //var acc = _context.User.Where(x => x.User_UserName == model.User_UserName).Where(x => x.User_InUse == true).Select(x => x).FirstOrDefault();
            //if (acc != null)
            //{
            //    //if (CryptoTools.getHashPasswd("PLACEHOLDER", model.User_UserName, model.User_Password) == acc.User_Password)
            //    //{
            //        //Set Session Info
            //        _session.SetString("UserID", acc.User_ID.ToString());
            //        _session.SetString("UserName", acc.User_FName + " " + acc.User_LName);
            //        //Set User Access Info
            //        _session.SetString("isLoggedIn", "true");
            //        _session.SetString("accessType", acc.User_Role);
            //        _session.SetString("isAdmin", acc.User_Role == "admin" ? "true" : "false");

            //        if (acc.User_Role == "admin")
            //        {
            //            return RedirectToAction("UM", "Home");
            //        }
            //        else
            //        {
            //            return RedirectToAction("Index", "Home");
            //        }
            //    //}
            //}
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