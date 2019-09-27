using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Resources;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.ViewModels.TransFailed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ExpenseProcessingSystem.Controllers
{
    [ScreenFltr]
    [ServiceFilter(typeof(HandleExceptionAttribute))]
    public class TransFailedController : Controller
    {
        private readonly int pageSize = 30;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private readonly GOExpressContext _GOContext;
        private readonly GWriteContext _GwriteContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private TransFailedService _service;
        private SortService _sortService;
        //to access resources
        private readonly IStringLocalizer<TransFailedController> _localizer;
        private IHostingEnvironment _env;
        private ILogger<TransFailedController> _logger;
        XElement xelemTFIntro = XElement.Load("wwwroot/xml/TransFailedIntro.xml");

        public TransFailedController(ILogger<TransFailedController> logger, IHttpContextAccessor httpContextAccessor, EPSDbContext context, GOExpressContext gocontext, GWriteContext gwritecontext, IHostingEnvironment hostingEnvironment, IStringLocalizer<TransFailedController> localizer)
        {
            _logger = logger;
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _GOContext = gocontext;
            _GwriteContext = gwritecontext;
            _service = new TransFailedService(_httpContextAccessor, _context, _GOContext, _GwriteContext, this.ModelState, hostingEnvironment);
            _sortService = new SortService();
            _env = hostingEnvironment;
        }

        private string GetUserID()
        {
            return _session.GetString("UserID");
        }

        [OnlineUserCheck]
        [NonAdminRoleCheck]
        public IActionResult Index()
        {
            //Allow Access only to APPROVER
            if (_session.GetString("accessType") != GlobalSystemValues.ROLE_APPROVER)
            {
                GlobalSystemValues.MESSAGE = GlobalSystemValues.MESSAGE10;
                return RedirectToAction("Index", "Home");
            }

            //Update Transaction list records before loading page.
            //Updates only to below status
            //STATUS_REVERSING = 16;
            //STATUS_REVERSING_ERROR = 17;
            //STATUS_REVERSING_COMPLETE = 18;
            //STATUS_RESENDING = 19;
            //STATUS_RESENDING_COMPLETE = 20;
            _service.UpdateResendingTransactions();
            _service.UpdateReversingTransactions();

            TransFailedViewModel vm = new TransFailedViewModel();

            //Assign XML Introduction to viewmodel
            for (int a = 1; a < 10; a++)
            {
                string msg = xelemTFIntro.Element("TF_MSG" + a).Value;
                if (!String.IsNullOrEmpty(msg))
                {
                    vm.TF_MSGs.Add(msg);
                }
            }

            //Get list of transactions that not accepted by G-BASE SYSTEM.
            vm.TF_TBL_DATA = _service.GetTransFailedList();

            return View(vm);
        }

        [AcceptVerbs("GET")]
        public JsonResult GetTest()
        {
            //_service.TEST();
            return Json("");
        }

        [AcceptVerbs("GET")]
        public JsonResult IsPendingTransactionInGOExpress()
        {
            return Json(_service.IsPendingTransactionInGOExpress());
        }

        [AcceptVerbs("GET")]
        public JsonResult IsSameTransactionStatus(int entryID, bool IsLiq)
        {
            return Json(_service.GetCurrentTransListStatus(entryID, IsLiq));
        }

        [AcceptVerbs("GET")]
        public JsonResult ProcessActionButton(int entryID, int actCmd, bool IsLiq)
        {
            switch (actCmd)
            {
                case GlobalSystemValues.GBaseErrResend:
                    _service.ResendToGOExpress(entryID, IsLiq, int.Parse(GetUserID()));
                    break;
                case GlobalSystemValues.GBaseErrReverse:
                    _service.ReverseToGOExpress(entryID, IsLiq, int.Parse(GetUserID()));
                    break;

                case GlobalSystemValues.GBaseErrReverseResend:
                    _service.ResendReversingErrorToGOExpress(entryID, IsLiq, int.Parse(GetUserID()));
                    break;
            }

            return Json(true);
        }
    }
}