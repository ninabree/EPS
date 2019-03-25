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
                if (ctx.Session.GetString("PF_Name") != null)//if PF not empty
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
            if (actionName == "DMPartial_Dept")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                if (ctx.Session.GetString("DF_Name") != null)//if PF not empty
                {
                    DeptFiltersViewModel DF = new DeptFiltersViewModel
                    {
                        DF_Name = ctx.Session.GetString("DF_Name"),
                        DF_Code = ctx.Session.GetString("DF_Code"),
                        DF_Creator_Name = ctx.Session.GetString("DF_Creator_Name"),
                        DF_Approver_Name = ctx.Session.GetString("DF_Approver_Name"),
                        DF_Status = ctx.Session.GetString("DF_Status")
                    };
                    filters.DF = DF;
                }
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_Check")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                var tmp2 = ctx.Session.GetString("CKF_Input_Date");
                var tmp = DateTime.Parse(tmp2);
                if (ctx.Session.GetString("CKF_Input_Date") != null)//if PF not empty
                {
                    CheckFiltersViewModel CKF = new CheckFiltersViewModel
                    {
                        CKF_Input_Date = DateTime.Parse(ctx.Session.GetString("CKF_Input_Date")),
                        CKF_Series_From = ctx.Session.GetString("CKF_Series_From"),
                        CKF_Series_To = ctx.Session.GetString("CKF_Series_To"),
                        CKF_Name = ctx.Session.GetString("CKF_Name"),
                        CKF_Type = ctx.Session.GetString("CKF_Type"),
                        CKF_Creator_Name = ctx.Session.GetString("CKF_Creator_Name"),
                        CKF_Approver_Name = ctx.Session.GetString("CKF_Approver_Name"),
                        CKF_Status = ctx.Session.GetString("CKF_Status")
                    };
                    filters.CKF = CKF;
                }
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_Acc")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                if (ctx.Session.GetString("AF_Name") != null)//if PF not empty
                {
                    AccFiltersViewModel AF = new AccFiltersViewModel
                    {
                        AF_Name = ctx.Session.GetString("AF_Name"),
                        AF_Code = ctx.Session.GetString("AF_Code"),
                        AF_No = int.Parse(ctx.Session.GetString("AF_No")),
                        AF_Cust = ctx.Session.GetString("AF_Cust"),
                        AF_Div = ctx.Session.GetString("AF_Div"),
                        AF_Fund = ctx.Session.GetString("AF_Fund"),
                        AF_Creator_Name = ctx.Session.GetString("AF_Creator_Name"),
                        AF_Approver_Name = ctx.Session.GetString("AF_Approver_Name"),
                        AF_Status = ctx.Session.GetString("AF_Status")
                    };
                    filters.AF = AF;
                }
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_VAT")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                if (ctx.Session.GetString("VF_Name") != null)//if PF not empty
                {
                    VATFiltersViewModel VF = new VATFiltersViewModel
                    {
                        VF_Name = ctx.Session.GetString("VF_Name"),
                        VF_Rate = ctx.Session.GetString("VF_Rate"),
                        VF_Creator_Name = ctx.Session.GetString("VF_Creator_Name"),
                        VF_Approver_Name = ctx.Session.GetString("VF_Approver_Name"),
                        VF_Status = ctx.Session.GetString("VF_Status")
                    };
                    filters.VF = VF;
                }
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_FBT")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                if (ctx.Session.GetString("FF_Name") != null)//if PF not empty
                {
                    FBTFiltersViewModel FF = new FBTFiltersViewModel
                    {
                        FF_Name = ctx.Session.GetString("FF_Name"),
                        FF_Account = ctx.Session.GetString("FF_Account"),
                        FF_Formula = ctx.Session.GetString("FF_Formula"),
                        FF_Tax_Rate = int.Parse(ctx.Session.GetString("FF_Tax_Rate")),
                        FF_Creator_Name = ctx.Session.GetString("FF_Creator_Name"),
                        FF_Approver_Name = ctx.Session.GetString("FF_Approver_Name"),
                        FF_Status = ctx.Session.GetString("FF_Status")
                    };
                    filters.FF = FF;
                }
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_EWT")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                if (ctx.Session.GetString("EF_Nature") != null)//if PF not empty
                {
                    EWTFiltersViewModel EF = new EWTFiltersViewModel
                    {
                        EF_Nature = ctx.Session.GetString("EF_Nature"),
                        EF_ATC = ctx.Session.GetString("EF_ATC"),
                        EF_Tax_Rate_Desc = ctx.Session.GetString("EF_Tax_Rate_Desc"),
                        EF_Tax_Rate = int.Parse(ctx.Session.GetString("EF_Tax_Rate")),
                        EF_Creator_Name = ctx.Session.GetString("EF_Creator_Name"),
                        EF_Approver_Name = ctx.Session.GetString("EF_Approver_Name"),
                        EF_Status = ctx.Session.GetString("EF_Status")
                    };
                    filters.EF = EF;
                }
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_Curr")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                if (ctx.Session.GetString("CF_Name") != null)//if PF not empty
                {
                    CurrFiltersViewModel CF = new CurrFiltersViewModel
                    {
                        CF_Name = ctx.Session.GetString("CF_Name"),
                        CF_CCY_Code = ctx.Session.GetString("CF_CCY_Code"),
                        CF_Creator_Name = ctx.Session.GetString("CF_Creator_Name"),
                        CF_Approver_Name = ctx.Session.GetString("CF_Approver_Name"),
                        CF_Status = ctx.Session.GetString("CF_Status")
                    };
                    filters.CF = CF;
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
