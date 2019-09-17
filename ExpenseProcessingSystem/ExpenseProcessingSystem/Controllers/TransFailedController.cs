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
        private HomeService _service;
        private SortService _sortService;
        //to access resources
        private readonly IStringLocalizer<HomeController> _localizer;
        private IHostingEnvironment _env;
        private ILogger<HomeController> _logger;
        XElement xelemTFIntro = XElement.Load("wwwroot/xml/TransFailedIntro.xml");

        public TransFailedController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, EPSDbContext context, GOExpressContext gocontext, GWriteContext gwritecontext, IHostingEnvironment hostingEnvironment, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _GOContext = gocontext;
            _GwriteContext = gwritecontext;
            _service = new HomeService(_httpContextAccessor, _context, _GOContext, _GwriteContext, this.ModelState, hostingEnvironment);
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

            return View(vm);
        }
    }
}