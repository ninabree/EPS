using System.Linq;
using ExpenseProcessingSystem.Data;
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
            //var tmp = (Request.Headers["x-requested-with"] == "XMLHttpRequest") ? "true" : "false";
            //_session.SetString("isTimeout", tmp); 
            ////{
            ////    //return RedirectToAction("Login", "Account");
            ////}
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

            var acc = _context.Account.Where(x => x.Acc_UserName == model.Acc_UserName).Where(x=> x.Acc_InUse == true).Select(x => x).FirstOrDefault();
            if (acc != null)
            {
                if (CryptoTools.getHashPasswd("PLACEHOLDER", model.Acc_UserName, model.Acc_Password) == acc.Acc_Password)
                {
                    //Set Session Info
                    _session.SetString("UserID", acc.Acc_UserID.ToString());
                    //Set Access Info
                    _session.SetString("isLoggedIn", "true");
                    _session.SetString("accessType", acc.Acc_Role);
                    _session.SetString("isAdmin", acc.Acc_Role == "admin" ? "true" : "false");
                    
                    Log.Information("User Logged In");
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("","Invalid Login Credential");
            return View(model);
        }
        public JsonResult checkSession()
        {
            var tmp = (_session.GetString("UserID") == null) ? true : false;
            return Json(tmp);
        }
    }
}