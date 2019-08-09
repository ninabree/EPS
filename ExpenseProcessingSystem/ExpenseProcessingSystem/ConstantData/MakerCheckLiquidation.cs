using ExpenseProcessingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ConstantData
{
    public class MakerCheckLiquidation : ActionFilterAttribute
    {
        private readonly EPSDbContext _dbContext;

        public MakerCheckLiquidation(EPSDbContext db)
        {
            _dbContext = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userIDByte = new byte[20];
            bool userOK = context.HttpContext.Session.TryGetValue("UserID", out userIDByte);
            int userID = int.Parse(System.Text.Encoding.UTF8.GetString(userIDByte));

            var roleByte = new byte[20];
            bool roleOK = context.HttpContext.Session.TryGetValue("accessType", out roleByte);
            string role = (System.Text.Encoding.UTF8.GetString(roleByte));

            if (context.ActionArguments.ContainsKey("entryID"))
            {
                var id = context.ActionArguments["entryID"] as Int32?;

                var liqExpense = _dbContext.LiquidationEntryDetails.FirstOrDefault(x => x.ExpenseEntryModel.Expense_ID == id);

                if (liqExpense != null && (userID != liqExpense.Liq_Created_UserID || liqExpense.Liq_Status != GlobalSystemValues.STATUS_PENDING))
                {
                    context.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Home",
                            action = "Liquidation_Main"
                        }));
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
