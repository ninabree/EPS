using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.ViewModels.Entry;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.Models;

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

        //Static value for the view path of voucher layout
        public static readonly string VOUCHER_LAYOUT = "EntryReports/_Voucherlayout";

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
        public static readonly string ENTRY_Liquidation = "/Home/Entry_Liquidation";
        public static readonly string ENTRY_NEW_CV = "/Home/AddNewCV";
        public static readonly string ENTRY_NEW_DDV = "/Home/AddNewDDV";

        public static readonly List<string> ENTRY_VALS = new List<string> {
            GlobalSystemValues.ENTRY,
            GlobalSystemValues.ENTRY_CV,
            GlobalSystemValues.ENTRY_DDV,
            GlobalSystemValues.ENTRY_NC,
            GlobalSystemValues.ENTRY_PCV,
            GlobalSystemValues.ENTRY_SS,
            GlobalSystemValues.ENTRY_Liquidation,
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
        //Static values for account types of entries use for Non Cash.
        public static readonly int NC_DEBIT = 1;
        public static readonly int NC_CREDIT = 2;
    }
    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_LSPAYROLL
    {
        public static EntryNCViewModel Populate_LSPAYROLL(DMCurrencyModel currDetails)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 1,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "T-L/S PAY:",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 458,
                                ExpNCDtlAcc_Acc_Name = "H99-767-111123 - P.E. Salary-Local Staff",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "HDMF ER Cont.",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 459,
                                ExpNCDtlAcc_Acc_Name = "H99-767-111220 - P.E. Welfare-Local Staff",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "WHT L/S PAY",
                        ExpNCDtl_Period_Duration = "Dec16-31'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 461,
                                ExpNCDtlAcc_Acc_Name = "H79-767-151242 - WHT on Local Staff Salary",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "HDMF LOAN:",
                        ExpNCDtl_Period_Duration = "Dec16-31'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 465,
                                ExpNCDtlAcc_Acc_Name = "H79-767-151381 - HDMF Loan Repayment",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "NET BDO -L/S PAY:",
                        ExpNCDtl_Period_Duration = "Dec16-31'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 466,
                                ExpNCDtlAcc_Acc_Name = "H60-767-801042 - Current Account with BDO MNL",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "CTBC LOAN:",
                        ExpNCDtl_Period_Duration = "Dec16-31'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 467,
                                ExpNCDtlAcc_Acc_Name = "H79-767-151365 - Bank's Multipurpose Loan Payable",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "HSBC LOAN:",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 467,
                                ExpNCDtlAcc_Acc_Name =  "H79-767-151365 - Bank's Multipurpose Loan Payable",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "HDMF ER-EE CONT:",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 468,
                                ExpNCDtlAcc_Acc_Name = "H79-767-151357 - HDMF Contribution Payable",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "SSS LOAN:",
                        ExpNCDtl_Period_Duration = "Dec16-31,'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 469,
                                ExpNCDtlAcc_Acc_Name = "H79-767-151365 - SSS Salary Loan Repayment",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "NT - Cloth, Medical, Rice:",
                        ExpNCDtl_Period_Duration = "",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 459,
                                ExpNCDtlAcc_Acc_Name = "H99-767-111220 - P.E. Welfare-Local Staff",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "NT - Proportionate Bonus:",
                        ExpNCDtl_Period_Duration = "2017",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 460,
                                ExpNCDtlAcc_Acc_Name = "H99-767-111212 - P.E. Bonus-Local Staff",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 454,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111 - Computer Suspense",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    }
                }
            };
        }        
    }
    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_TAXREMITTANCE
    {
        public static EntryNCViewModel Populate_TAXREMITTANCE(DMCurrencyModel currDetails)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 2,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "REMIT RTGS PHILPASS",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 470,
                                ExpNCDtlAcc_Acc_Name = "H79-767-151292 - 2% WTX PHILPASS TRANS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "BPI PAYROLL FACILITY",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 472,
                                ExpNCDtlAcc_Acc_Name = "H79-767-152060 - BPI PAYROLL WHTAX",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "REMIT WHT CONTRACTORS",
                        ExpNCDtl_Period_Duration = "Dec16-31'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 473,
                                ExpNCDtlAcc_Acc_Name = "H79-767-151226 - W/H TAX INCOME PAYMENT",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "REMIT WHT MONEY MARKET",
                        ExpNCDtl_Period_Duration = "Dec16-31'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 474,
                                ExpNCDtlAcc_Acc_Name = "B79-767-151195 - W/H TAX MONEY MARKETS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "REMIT WHT J/S SALARY",
                        ExpNCDtl_Period_Duration = "Dec16-31'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 475,
                                ExpNCDtlAcc_Acc_Name = "B79-767-151234 - W/H TAX SALARIES & ALLOW (JAP)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "REMIT WHT L/S SALARY",
                        ExpNCDtl_Period_Duration = "Dec16-31'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 476,
                                ExpNCDtlAcc_Acc_Name = "B79-767-151242 - W/H TAX SALARIES & ALLOW (LOC)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "REMIT WHT L/S BONUS",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 477,
                                ExpNCDtlAcc_Acc_Name = "B79-767-151268 - W/T TAX BONUS (LOCAL STAFF)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "REMIT VAT",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 478,
                                ExpNCDtlAcc_Acc_Name = "B79-767-151399 - TAXES WITHHELD OUTPUT VAT OPI",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "FRINGE BENEFIT TAX",
                        ExpNCDtl_Period_Duration = "Dec16-31,'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 479,
                                ExpNCDtlAcc_Acc_Name = "B79-767-151307 - W/H TAX PAY FRINGE BENEFITS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    }
                }
            };
        }
    }
    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_MONTHLYROSSBILL
    {
        public static EntryNCViewModel Populate_MONTHLYROSSBILL(DMCurrencyModel currDetails)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 3,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "MONTHLY ROSS BILL",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 433,
                                ExpNCDtlAcc_Acc_Name = "H90-767-121781 - SUNDRY OTHERS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    }
                }
            };
        }
    }
    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_PSSC
    {
        public static EntryNCViewModel Populate_PSSC(DMCurrencyModel currDetails)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 4,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "PSSC",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 433,
                                ExpNCDtlAcc_Acc_Name = "H90-767-121781 - SUNDRY OTHERS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    }
                }
            };
        }
    }
    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_PCHC
    {
        public static EntryNCViewModel Populate_PCHC(DMCurrencyModel currDetails)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 5,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "PASS Transactions",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 433,
                                ExpNCDtlAcc_Acc_Name = "H90-767-121781 - SUNDRY OTHERS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "Processing Fees",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 433,
                                ExpNCDtlAcc_Acc_Name = "H90-767-121781 - SUNDRY OTHERS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "Tokens",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 433,
                                ExpNCDtlAcc_Acc_Name = "H90-767-121781 - SUNDRY OTHERS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 471,
                                ExpNCDtlAcc_Acc_Name = "H79-767-801000 - BANGKO SENTRAL NG PILIPINAS",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    }
                }
            };
        }
    }
    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_DEPRECIATION
    {
        public static EntryNCViewModel Populate_DEPRECIATION(DMCurrencyModel currDetails)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 6,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "BANK PREMISES",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 480,
                                ExpNCDtlAcc_Acc_Name = "H99-767-31115-51205 - (Depr on Bk Premises-DEPRECIATION)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 481,
                                ExpNCDtlAcc_Acc_Name = "H79-767-1600013-16205 - Bk Premises Accum Deprec-LEASEHOLD IMPRV",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "FURNITURES & FIXTURES",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 482,
                                ExpNCDtlAcc_Acc_Name = "H99-767-131123-51205 - (Depr on Bk Fixtures-DEPRECIATION)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 483,
                                ExpNCDtlAcc_Acc_Name = "H79-767-170018-16409 - (Accum Depre Fur Fix & Equi-Furniture&Fix)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "Software",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 484,
                                ExpNCDtlAcc_Acc_Name = "B99-767-111115-54122 - (Depre. Softwre)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 485,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111-17146 (Def AST SYS-Dev)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    }
                }
            };
        }
    }
    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_PETTYCASHREPLENISHMENT
    {
        public static EntryNCViewModel Populate_PETTYCASHREPLENISHMENT(DMCurrencyModel currDetails)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 7,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "PETTY CASH REPLENISHMENT",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 486,
                                ExpNCDtlAcc_Acc_Name = "B60-767-151129-00204 - (Petty Cash)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 487,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111-14017 - (Computer Suspense)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    }
                }
            };
        }
        public static EntryNCViewModel Populate_CDD_Instruc_Sheet(DMCurrencyModel currDetails)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 7,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "PETTY CASH REPLENISHMENT",
                        ExpNCDtl_Period_Duration = "Dec'17",
                        ExpNCDtl_Remarks_Period_From = DateTime.Now,
                        ExpNCDtl_Remarks_Period_To = DateTime.Now,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 487,
                                ExpNCDtlAcc_Acc_Name = "B79-767-111111-14017 - (Computer Suspense)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = 487,
                                ExpNCDtlAcc_Acc_Name = "xxxxxxxxxxxxxxxxxxxx (Computer Suspense)",
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    }
                }
            };
        }
    }
}
