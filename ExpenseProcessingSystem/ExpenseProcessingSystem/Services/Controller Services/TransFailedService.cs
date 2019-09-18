using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using ModelStateDictionary = Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

namespace ExpenseProcessingSystem.Services.Controller_Services
{
    public class TransFailedService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private readonly GOExpressContext _GOContext;
        private readonly GWriteContext _gWriteContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ModalService _modalservice;
        private FilterQueryService _filterservice;
        XElement xelemAcc = XElement.Load("wwwroot/xml/GlobalAccounts.xml");
        XElement xelemLiq = XElement.Load("wwwroot/xml/LiquidationValue.xml");
        XElement xelemReport = XElement.Load("wwwroot/xml/ReportHeader.xml");
        private ModelStateDictionary _modelState;
        private NumberToText _class;

        public TransFailedService(IHttpContextAccessor httpContextAccessor, EPSDbContext context, GOExpressContext goContext, GWriteContext gWriteContext, ModelStateDictionary modelState, IHostingEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _GOContext = goContext;
            _gWriteContext = gWriteContext;
            _modelState = modelState;
            _hostingEnvironment = hostingEnvironment;
            _modalservice = new ModalService(_httpContextAccessor, _context, _gWriteContext);
            _filterservice = new FilterQueryService();
            _class = new NumberToText();
        }

        public string getUserRole(string id)
        {
            var data = _context.User.Where(x => x.User_ID == int.Parse(id))
                .Select(x => x.User_Role).FirstOrDefault() ?? "";
            return data;
        }
        private int[] status = { GlobalSystemValues.STATUS_POSTED, GlobalSystemValues.STATUS_FOR_CLOSING,
                            GlobalSystemValues.STATUS_FOR_PRINTING };
        private Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState;
        private IHostingEnvironment hostingEnvironment;

        public void TEST()
        {
            var test = _GOContext.TblCm10.ToList();
            foreach (var i in test)
            {
                Console.WriteLine("###########" + i.Datestamp );
            }
        }
    }
}
