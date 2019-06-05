using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ConstantData
{
    public class GlobalSystemValues
    {
        //Static values for status values use the for the workflow statuses.
        //i.e: Checking what the current status of an application is 
        //Example: [Application.status == GlobalSystemValues.STATUS_PENDING]
        public static readonly int STATUS_PENDING = 1;
        public static readonly int STATUS_VERIFIED = 2;
        public static readonly int STATUS_APPROVED = 3;
        public static readonly int STATUS_POSTED = 4;
        public static readonly int STATUS_REJECTED = 5;
        public static readonly int STATUS_DELETED = 6;
        public static readonly int STATUS_NEW = 7;
        public static readonly int STATUS_EDIT = 8;
        public static readonly int STATUS_DELETE = 9;

        //Static values for the index of certain lists that are to be stored inside a list object.
        //i.e: Retrieving system values for the dropdown boxes of entry views.
        //Example : vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR] instead of [vendors = listOfSysVals[0]]
        public static readonly int SELECT_LIST_VENDOR = 0;
        public static readonly int SELECT_LIST_DEPARTMENT = 1;
        public static readonly int SELECT_LIST_CURRENCY = 2;
        public static readonly int SELECT_LIST_TAXRATE = 3;
        public static readonly int SELECT_LIST_CATEGORY = 4;

        //Static values for user roles use for checking or adding user roles to users.
        //i.e: Checking user access rights
        //Example: instead of [if (role == "admin")] use [if (role == GlobalSystemValues.ROLE_ADMIN)]
        public static readonly string ROLE_ADMIN = "admin";
        public static readonly string ROLE_MAKER = "maker";
        public static readonly string ROLE_VERIFIER = "verifier";
        public static readonly string ROLE_APPROVER = "approver";

        //Static value for application types used for checking application type
        //i.e: displaying pending applications
        //Example : [Expense.Type = TYPE_CV] instead of [Expense.Type = 1]
        public static readonly int TYPE_DM = 0;
        public static readonly int TYPE_CV = 1;
        public static readonly int TYPE_DDV = 2;
        public static readonly int TYPE_SS = 3;
        public static readonly int TYPE_PC = 4;
        public static readonly int TYPE_NC = 5;

        public static string getApplicationType(int appType)
        {
            switch (appType)
            {
                case 0: return "Data Maintenance";
                case 1: return "Check";
                case 2: return "Direct Deposit";
                case 3: return "Suspense Sundry";
                case 4: return "Petty Cash";
                case 5: return "Non-Cash";
                default: return null;
            }
        }

        //Static values for entry path use for checking if EntityTabsPartial will be shown.
        //Example: instead of [if Context.Request.Path.ToString() == "/Home/Entry"] use (GlobalSystemValues.ENTRY_VALS.Contains(Context.Request.Path.ToString())) ? true : false;
        public static readonly string ENTRY = "/Home/Entry";
        public static readonly string ENTRY_CV = "/Home/Entry_CV";
        public static readonly string ENTRY_DDV = "/Home/Entry_DDV";
        public static readonly string ENTRY_NC = "/Home/Entry_NC";
        public static readonly string ENTRY_PCV = "/Home/Entry_PCV";
        public static readonly string ENTRY_SS = "/Home/Entry_SS";
        public static readonly string ENTRY_NEW_CV = "/Home/AddNewCV";
        public static readonly string ENTRY_NEW_DDV = "/Home/AddNewDDV";

        public static readonly List<string> ENTRY_VALS = new List<string> {
            GlobalSystemValues.ENTRY,
            GlobalSystemValues.ENTRY_CV,
            GlobalSystemValues.ENTRY_DDV,
            GlobalSystemValues.ENTRY_NC,
            GlobalSystemValues.ENTRY_PCV,
            GlobalSystemValues.ENTRY_SS,
            GlobalSystemValues.ENTRY_NEW_CV,
            GlobalSystemValues.ENTRY_NEW_DDV
        };
        //Static values for entry path use for checking if HomeTabsPartial will be shown.
        //Example: instead of [if Context.Request.Path.ToString() == "/Home/Index"] use (GlobalSystemValues.HOME_VALS.Contains(Context.Request.Path.ToString())) ? true : false;
        public static readonly string HOME_INDEX = "/Home/Index";
        public static readonly string HOME_PENDING = "/Home/Pending";
        public static readonly string HOME_HISTORY = "/Home/History";

        public static readonly List<string> HOME_VALS = new List<string> {
            GlobalSystemValues.HOME_INDEX,
            GlobalSystemValues.HOME_PENDING,
            GlobalSystemValues.HOME_HISTORY
        };

        //Static values for Login page
        public static readonly string ACCOUNT_LOGIN = "/Account/Login";


        //Static values for category of entries use for Non Cash.
        public static readonly int NC_LS_PAYROLL = 1;
        public static readonly int NC_TAX_REMITTANCE = 2;
        public static readonly int NC_MONTHLY_ROSS_BILL = 3;
        public static readonly int NC_PSSC = 4;
        public static readonly int NC_PCHC = 5;
        public static readonly int NC_DEPRECIATION = 6;
        public static readonly int NC_PETTY_CASH_REPLENISHMENT = 7;
        public static readonly int NC_JS_PAYROLL = 8;
        public static readonly int NC_RETURN_OF_JS_PAYROLL = 9;
        public static readonly int NC_DOLLAR_PAYMENT = 10;
        public static readonly int NC_ADJUSTMENTS = 11;
        public static readonly int NC_ADVANCE_EXPENSE_HO_REVERSAL = 12;

        public static readonly List<int> NC_CATEGORIES = new List<int> {
            GlobalSystemValues.NC_LS_PAYROLL,
            GlobalSystemValues.NC_TAX_REMITTANCE,
            GlobalSystemValues.NC_MONTHLY_ROSS_BILL,
            GlobalSystemValues.NC_PSSC,
            GlobalSystemValues.NC_PCHC,
            GlobalSystemValues.NC_DEPRECIATION,
            GlobalSystemValues.NC_PETTY_CASH_REPLENISHMENT,
            GlobalSystemValues.NC_JS_PAYROLL,
            GlobalSystemValues.NC_RETURN_OF_JS_PAYROLL,
            GlobalSystemValues.NC_DOLLAR_PAYMENT,
            GlobalSystemValues.NC_ADJUSTMENTS,
            GlobalSystemValues.NC_ADVANCE_EXPENSE_HO_REVERSAL
        };
        public static readonly List<SelectListItem> NC_CATEGORIES_SELECT = new List<SelectListItem> {
                new SelectListItem { Text = "LS PAYROLL", Value = NC_CATEGORIES[0].ToString(), Selected = true },
                new SelectListItem { Text = "TAX REMITTANCE", Value = NC_CATEGORIES[1].ToString(), Selected = false },
                new SelectListItem { Text = "MONTHLY ROSS BILL", Value = NC_CATEGORIES[2].ToString(), Selected = false },
                new SelectListItem { Text = "PSSC", Value = NC_CATEGORIES[3].ToString(), Selected = false },
                new SelectListItem { Text = "PCHC", Value = NC_CATEGORIES[4].ToString(), Selected = false },
                new SelectListItem { Text = "DEPRECIATION", Value = NC_CATEGORIES[5].ToString(), Selected = false },
                new SelectListItem { Text = "PETTY CASH REPLENISHMENT", Value = NC_CATEGORIES[6].ToString(), Selected = false },
                new SelectListItem { Text = "JS PAYROLL.", Value = NC_CATEGORIES[7].ToString(), Selected = false },
                new SelectListItem { Text = "RETURN OF JS PAYROLL", Value = NC_CATEGORIES[8].ToString(), Selected = false },
                new SelectListItem { Text = "DOLLAR PAYMENT", Value = NC_CATEGORIES[9].ToString(), Selected = false },
                new SelectListItem { Text = "ADJUSTMENTS", Value = NC_CATEGORIES[10].ToString(), Selected = false },
                new SelectListItem { Text = "ADVANCE EXPENSE HO REVERSAL", Value = NC_CATEGORIES[11].ToString(), Selected = false }
        };
    }
}
