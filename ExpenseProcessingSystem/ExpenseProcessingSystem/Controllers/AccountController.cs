using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

            var acc = _context.User.Where(x => x.User_UserName == model.User_UserName).Where(x=> x.User_InUse == true).Select(x => x).FirstOrDefault();
            if (acc != null)
            {
                if (CryptoTools.getHashPasswd("PLACEHOLDER", model.User_UserName, model.User_Password) == acc.User_Password)
                {
                    //Set Session Info
                    _session.SetString("UserID", acc.User_ID.ToString());
                    //Set Useress Info
                    _session.SetString("isLoggedIn", "true");
                    _session.SetString("accessType", acc.User_Role);
                    _session.SetString("isAdmin", acc.User_Role == "admin" ? "true" : "false");
                    
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