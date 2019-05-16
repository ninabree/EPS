using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Search_Filters;
using ExpenseProcessingSystem.ViewModels.Search_Filters.Home;
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
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];

            //FOR Home
            if (actionName == "Index" && controllerName == "Home")
            {
                FiltersViewModel filters = new FiltersViewModel();
                NotifFiltersViewModel NotifFil = new NotifFiltersViewModel();
                NotifFil = new NotifFiltersViewModel
                {
                    Notif_Last_Updated = DateTime.Parse(ctx.Session.GetString("Notif_Last_Updated") ?? DateTime.Now.ToString()),
                    NotifFil_Message = ctx.Session.GetString("NotifFil_Message") ?? "",
                    NotifFil_Status = ctx.Session.GetString("NotifFil_Status") ?? "",
                    NotifFil_Verifier_Approver_Name = ctx.Session.GetString("NotifFil_Verifier_Approver_Name") ?? ""
                };
                filters.NotifFil = NotifFil;
                controller.TempData["filters"] = filters;
            }
            //FOR DM

            if (actionName == "DMPartial_Vendor")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                VendorFiltersViewModel PF = new VendorFiltersViewModel();
                PF = new VendorFiltersViewModel
                {
                    PF_Name = ctx.Session.GetString("PF_Name") ?? "",
                    PF_TIN = int.Parse(ctx.Session.GetString("PF_TIN") ?? "0"),
                    PF_Address = ctx.Session.GetString("PF_Address") ?? "",
                    PF_Creator_Name = ctx.Session.GetString("PF_Creator_Name") ?? "",
                    PF_Approver_Name = ctx.Session.GetString("PF_Approver_Name") ?? "",
                    PF_Status = ctx.Session.GetString("PF_Status") ?? ""
                };
                filters.PF = PF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_Dept")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                DeptFiltersViewModel DF = new DeptFiltersViewModel
                {
                    DF_Name = ctx.Session.GetString("DF_Name") ?? "",
                    DF_Code = ctx.Session.GetString("DF_Code") ?? "",
                    DF_Budget_Unit = ctx.Session.GetString("DF_Budget_Unit") ?? "",
                    DF_Creator_Name = ctx.Session.GetString("DF_Creator_Name") ?? "",
                    DF_Approver_Name = ctx.Session.GetString("DF_Approver_Name") ?? "",
                    DF_Status = ctx.Session.GetString("DF_Status") ?? ""
                };
                filters.DF = DF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_Check")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                //var tmp2 = ctx.Session.GetString("CKF_Input_Date");
                //var tmp = DateTime.Parse(tmp2);
                CheckFiltersViewModel CKF = new CheckFiltersViewModel
                {
                    CKF_Input_Date = DateTime.Parse(ctx.Session.GetString("CKF_Input_Date") ?? new DateTime().ToString()),
                    CKF_Series_From = ctx.Session.GetString("CKF_Series_From") ?? "",
                    CKF_Series_To = ctx.Session.GetString("CKF_Series_To") ?? "",
                    CKF_Name = ctx.Session.GetString("CKF_Name") ?? "",
                    CKF_Bank_Info = ctx.Session.GetString("CKF_Bank_Info") ?? "",
                    CKF_Creator_Name = ctx.Session.GetString("CKF_Creator_Name") ?? "",
                    CKF_Approver_Name = ctx.Session.GetString("CKF_Approver_Name") ?? "",
                    CKF_Status = ctx.Session.GetString("CKF_Status") ?? ""
                };
                filters.CKF = CKF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_Acc")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                AccFiltersViewModel AF = new AccFiltersViewModel
                {
                    AF_Name = ctx.Session.GetString("AF_Name") ?? "",
                    AF_Code = ctx.Session.GetString("AF_Code") ?? "",
                    AF_No = int.Parse(ctx.Session.GetString("AF_No") ?? "0"),
                    AF_Cust = ctx.Session.GetString("AF_Cust") ?? "",
                    AF_Div = ctx.Session.GetString("AF_Div") ?? "",
                    AF_Group = ctx.Session.GetString("AF_Group") ?? "",
                    AF_FBT = ctx.Session.GetString("AF_FBT") ?? "",
                    AF_Creator_Name = ctx.Session.GetString("AF_Creator_Name") ?? "",
                    AF_Approver_Name = ctx.Session.GetString("AF_Approver_Name") ?? "",
                    AF_Status = ctx.Session.GetString("AF_Status") ?? ""
                };
                filters.AF = AF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_VAT")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                VATFiltersViewModel VF = new VATFiltersViewModel
                {
                    VF_Name = ctx.Session.GetString("VF_Name") ?? "",
                    VF_Rate = ctx.Session.GetString("VF_Rate") ?? "",
                    VF_Creator_Name = ctx.Session.GetString("VF_Creator_Name") ?? "",
                    VF_Approver_Name = ctx.Session.GetString("VF_Approver_Name") ?? "",
                    VF_Status = ctx.Session.GetString("VF_Status") ?? ""
                };
                filters.VF = VF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_FBT")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                FBTFiltersViewModel FF = new FBTFiltersViewModel
                {
                    FF_Name = ctx.Session.GetString("FF_Name") ?? "",
                    FF_Formula = ctx.Session.GetString("FF_Formula") ?? "",
                    FF_Tax_Rate = int.Parse(ctx.Session.GetString("FF_Tax_Rate")?? "0"),
                    FF_Creator_Name = ctx.Session.GetString("FF_Creator_Name") ?? "",
                    FF_Approver_Name = ctx.Session.GetString("FF_Approver_Name") ?? "",
                    FF_Status = ctx.Session.GetString("FF_Status") ?? ""
                };
                filters.FF = FF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_TR")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                TRFiltersViewModel TF = new TRFiltersViewModel
                {
                    TR_WT_Title = ctx.Session.GetString("TR_WT_Title") ?? "",
                    TR_Nature = ctx.Session.GetString("TR_Nature") ?? "",
                    TR_Nature_Income_Payment = ctx.Session.GetString("TR_Nature_Income_Payment") ?? "",
                    TR_ATC = ctx.Session.GetString("TR_ATC") ?? "",
                    TR_Tax_Rate = int.Parse(ctx.Session.GetString("TR_Tax_Rate") ?? "0"),
                    TR_Creator_Name = ctx.Session.GetString("TR_Creator_Name") ?? "",
                    TR_Approver_Name = ctx.Session.GetString("TR_Approver_Name") ?? "",
                    TR_Status = ctx.Session.GetString("TR_Status") ?? ""
                };
                filters.TF = TF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_Curr")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                CurrFiltersViewModel CF = new CurrFiltersViewModel
                {
                    CF_Name = ctx.Session.GetString("CF_Name") ?? "",
                    CF_CCY_ABBR = ctx.Session.GetString("CF_CCY_ABBR") ?? "",
                    CF_Creator_Name = ctx.Session.GetString("CF_Creator_Name") ?? "",
                    CF_Approver_Name = ctx.Session.GetString("CF_Approver_Name") ?? "",
                    CF_Status = ctx.Session.GetString("CF_Status") ?? ""
                };
                filters.CF = CF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_RegEmp" || actionName == "DMPartial_TempEmp")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                EmpFiltersViewModel EMF = new EmpFiltersViewModel
                {
                    EMF_Name = ctx.Session.GetString("EMF_Name") ?? "",
                    EMF_Acc_No = ctx.Session.GetString("EMF_Acc_No") ?? "",
                    EMF_Type = ctx.Session.GetString("EMF_Type") ?? "",
                    EMF_Creator_Name = ctx.Session.GetString("EMF_Creator_Name") ?? "",
                    EMF_Approver_Name = ctx.Session.GetString("EMF_Approver_Name") ?? "",
                    EMF_Status = ctx.Session.GetString("EMF_Status") ?? ""
                };
                filters.EMF = EMF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_Cust")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                CustFiltersViewModel CUF = new CustFiltersViewModel
                {
                    CUF_Name = ctx.Session.GetString("CUF_Name") ?? "",
                    CUF_Abbr = ctx.Session.GetString("CUF_Abbr") ?? "",
                    CUF_No = ctx.Session.GetString("CUF_No") ?? "",
                    CUF_Creator_Name = ctx.Session.GetString("CUF_Creator_Name") ?? "",
                    CUF_Approver_Name = ctx.Session.GetString("CUF_Approver_Name") ?? "",
                    CUF_Status = ctx.Session.GetString("CUF_Status") ?? ""
                };
                filters.CUF = CUF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_BCS")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                BCSFiltersViewModel BF = new BCSFiltersViewModel
                {
                    BF_Name = ctx.Session.GetString("BF_Name") ?? "",
                    BF_TIN = ctx.Session.GetString("BF_TIN") ?? "",
                    BF_Position = ctx.Session.GetString("BF_Position") ?? "",
                    BF_Status = ctx.Session.GetString("BF_Status") ?? "",
                    BF_Creator_Name = ctx.Session.GetString("BF_Creator_Name") ?? "",
                    BF_Approver_Name = ctx.Session.GetString("BF_Approver_Name") ?? "",
                    BF_Signatures = ctx.Session.GetString("BF_Signatures") ?? ""
                };
                filters.BF = BF;
                controller.TempData["filters"] = filters;
            }
            if (actionName == "DMPartial_AccGroup")
            {
                DMFiltersViewModel filters = new DMFiltersViewModel();
                AccGroupFiltersViewModel AGF = new AccGroupFiltersViewModel
                {
                    AGF_Name = ctx.Session.GetString("AGF_Name") ?? "",
                    AGF_Code = ctx.Session.GetString("AGF_Code") ?? "",
                    AGF_Creator_Name = ctx.Session.GetString("AGF_Creator_Name") ?? "",
                    AGF_Approver_Name = ctx.Session.GetString("AGF_Approver_Name") ?? "",
                    AGF_Status = ctx.Session.GetString("AGF_Status") ?? ""
                };
                filters.AGF = AGF;
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
