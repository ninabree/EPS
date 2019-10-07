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
    public class MakerCheck : ActionFilterAttribute
    {
        private readonly EPSDbContext _dbContext;

        public MakerCheck(EPSDbContext db)
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

                var expense = _dbContext.ExpenseEntry.FirstOrDefault(x=>x.Expense_ID == id);

                if(userID != expense.Expense_Creator_ID || (expense.Expense_Status != GlobalSystemValues.STATUS_PENDING && 
                                                            expense.Expense_Status != GlobalSystemValues.STATUS_REJECTED &&
                                                            expense.Expense_Status != GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR ))
                {
                    context.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Home",
                            action = "Index"
                        }));
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
