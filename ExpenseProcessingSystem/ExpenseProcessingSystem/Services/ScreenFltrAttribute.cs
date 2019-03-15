using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public class ScreenFltrAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = filterContext.HttpContext;
            var controller = filterContext.Controller as Controller;

            AccessViewModel accessVM = new AccessViewModel
            {
                isLoggedIn = false,
                accessType = "",
                isAdmin = false
            };

            if (ctx.Session.GetString("UserID") == null)
            {
                controller.ViewBag.access = accessVM;
                filterContext.Result = new RedirectToRouteResult(
                 new RouteValueDictionary
                 {
                    { "action", "Login" },
                    { "controller", "Account" }
                 });
                return;
            }
            else
            {
                accessVM = new AccessViewModel {
                    accessType = ctx.Session.GetString("accessType") as String,
                    isAdmin = bool.Parse(ctx.Session.GetString("isAdmin")),
                    isLoggedIn = bool.Parse(ctx.Session.GetString("isLoggedIn"))
                };
                controller.ViewBag.access = accessVM;
            }
        }
    }
}
