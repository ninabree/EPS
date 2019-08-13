using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;

namespace ExpenseProcessingSystem.Resources
{
    public class HandleExceptionAttribute : ExceptionFilterAttribute
    {
        public ILogger<HandleExceptionAttribute> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public HandleExceptionAttribute()
        {
        }

        public HandleExceptionAttribute(IHttpContextAccessor httpContextAccessor, ILogger<HandleExceptionAttribute> logger)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

        }

        public override void OnException(ExceptionContext filterContext)
        {
            Log(filterContext.Exception);
            base.OnException(filterContext);
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/ErrorPage.cshtml"
            };
            filterContext.ExceptionHandled = true;
        }

        private void Log(Exception exception)
        {
            //Log here
            _logger.LogError(exception, "User [" + _session.GetString("UserID") + " : " + _session.GetString("UserName") + "] has encountered a system error at [" + DateTime.Now + "].");
        }
    }
}
