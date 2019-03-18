using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Search_Filters;
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
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            HttpContext ctx = filterContext.HttpContext;
            var controller = filterContext.Controller as Controller;
        }
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
            //FOR DM
            var actionName = (string)filterContext.RouteData.Values["action"];
            if (actionName == "DMPartial_Payee")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                if (true)//if PF not empty
                {
                    PayeeFiltersViewModel PF = new PayeeFiltersViewModel
                    {
                        PF_Name = ctx.Session.GetString("PF_Name"),
                        PF_TIN = int.Parse(ctx.Session.GetString("PF_TIN")),
                        PF_Address = ctx.Session.GetString("PF_Address"),
                        PF_Type = ctx.Session.GetString("PF_Type"),
                        PF_No = int.Parse(ctx.Session.GetString("PF_No")),
                        PF_Creator_Name = ctx.Session.GetString("PF_Creator_Name"),
                        PF_Approver_Name = ctx.Session.GetString("PF_Approver_Name"),
                        PF_Status = ctx.Session.GetString("PF_Status")
                    };
                    filters.PF = PF;
                }
                controller.TempData["filters"] = filters;
            }

            //check session vals
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
                //set access vals
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
