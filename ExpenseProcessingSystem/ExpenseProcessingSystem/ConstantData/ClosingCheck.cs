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
    public class ClosingCheck : ActionFilterAttribute
    {
        private readonly EPSDbContext _dbContext;

        public ClosingCheck(EPSDbContext db)
        {
            _dbContext = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var closingRBU = _dbContext.Closing.Where(x => x.Close_Type == "767").OrderByDescending(x => x.Close_ID).FirstOrDefault();
            var closingFCDU = _dbContext.Closing.Where(x => x.Close_Type == "789").OrderByDescending(x => x.Close_ID).FirstOrDefault();


            if(closingFCDU.Close_Status == GlobalSystemValues.STATUS_CLOSED || closingRBU.Close_Status == GlobalSystemValues.STATUS_CLOSED)
            {
                context.Result =
                        new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Home",
                            action = "Index"
                        }));

                GlobalSystemValues.MESSAGE = "Daily book is currently closed, cannot make any transactions.";
            }

            base.OnActionExecuting(context);
        }
    }
}
