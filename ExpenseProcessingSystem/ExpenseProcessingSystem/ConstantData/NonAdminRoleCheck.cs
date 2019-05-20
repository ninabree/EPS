using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ExpenseProcessingSystem.ConstantData
{
    public class NonAdminRoleCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionRole = new byte[20];
            bool roleOK = context.HttpContext.Session.TryGetValue("accessType", out sessionRole);

            if (roleOK)
            {
                if (System.Text.Encoding.UTF8.GetString(sessionRole) == GlobalSystemValues.ROLE_ADMIN) {
                    context.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Home",
                            action = "UM"
                        }));
                }
            }
            else
            {
                context.Result =
                    new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "Login"
                    }));
            }
            base.OnActionExecuting(context);
        }
    }
}
