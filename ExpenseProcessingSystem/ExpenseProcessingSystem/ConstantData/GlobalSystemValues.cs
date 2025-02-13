﻿using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels.Entry;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

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
        public static readonly int STATUS_REVERSED = 10;
        public static readonly int STATUS_CLOSED = 11;
        public static readonly int STATUS_OPEN = 12;
        public static readonly int STATUS_FOR_PRINTING = 13;
        public static readonly int STATUS_FOR_CLOSING = 14;
        public static readonly int STATUS_ERROR = 15;
        public static readonly int STATUS_REVERSING = 16;
        public static readonly int STATUS_REVERSING_ERROR = 17;
        public static readonly int STATUS_REVERSING_COMPLETE = 18;
        public static readonly int STATUS_RESENDING = 19;
        public static readonly int STATUS_RESENDING_COMPLETE = 20;
        public static readonly int STATUS_REVERSED_GBASE_ERROR = 21;

        //Static values for the index of certain lists that are to be stored inside a list object.
        //i.e: Retrieving system values for the dropdown boxes of entry views.
        //Example : vendors = listOfSysVals[GlobalSystemValues.SELECT_LIST_VENDOR] instead of [vendors = listOfSysVals[0]]
        public static readonly int SELECT_LIST_VENDOR = 0;
        public static readonly int SELECT_LIST_DEPARTMENT = 1;
        public static readonly int SELECT_LIST_CURRENCY = 2;
        public static readonly int SELECT_LIST_TAXRATE = 3;
        //public static readonly int SELECT_LIST_CATEGORY = 4;
        public static readonly int SELECT_LIST_REGEMPLOYEE = 4;
        public static readonly int SELECT_LIST_ALLEMPLOYEE = 5;

        //Static values for payee type use for checking or adding payee type to expense entries.
        //i.e: Checking payee type
        //Example: instead of [if (type == "vendor")] use [if (type == GlobalSystemValues.PAYEETYPE_VENDOR)]
        public static readonly int PAYEETYPE_VENDOR = 1;
        public static readonly int PAYEETYPE_REGEMP = 2;
        public static readonly int PAYEETYPE_TEMPEMP = 3;
        public static readonly int PAYEETYPE_CUST = 4;
        public static readonly int PAYEETYPE_EMP_ALL = 5;

        public static readonly List<SelectListItem> PAYEETYPE_SELECT_ALL = new List<SelectListItem> {
                new SelectListItem { Text = "ALL EMPLOYEE", Value = PAYEETYPE_EMP_ALL.ToString(), Selected = true },
                new SelectListItem { Text = "CUSTOMER", Value = PAYEETYPE_REGEMP.ToString(), Selected = false },
                new SelectListItem { Text = "REGULAR EMPLOYEE", Value = PAYEETYPE_REGEMP.ToString(), Selected = false },
                new SelectListItem { Text = "TEMPORARY EMPLOYEE", Value = PAYEETYPE_REGEMP.ToString(), Selected = false },
                new SelectListItem { Text = "VENDOR", Value = PAYEETYPE_VENDOR.ToString(), Selected = false },
        };
        public static readonly List<SelectListItem> PAYEETYPE_SELECT_CV = new List<SelectListItem> {
                new SelectListItem { Text = "REGULAR EMPLOYEE", Value = PAYEETYPE_REGEMP.ToString(), Selected = true },
                new SelectListItem { Text = "VENDOR", Value = PAYEETYPE_VENDOR.ToString(), Selected = false }
        };
        //Static values for payee type use for checking or adding employee category.
        //i.e: Checking category type
        //Example: instead of [if (type == "local")] use [if (type == GlobalSystemValues.EMPCAT_LOCAL)]
        public const int EMPCAT_LOCAL = 1;
        public const int EMPCAT_EXPAT = 2;

        public static readonly List<SelectListItem> EMPCATEGORY_SELECT = new List<SelectListItem> {
                new SelectListItem { Text = "EXPAT", Value = EMPCAT_EXPAT.ToString(), Selected = true },
                new SelectListItem { Text = "LOCAL", Value = EMPCAT_LOCAL.ToString(), Selected = false }
        };

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
        public const int TYPE_DM = 0;
        public const int TYPE_CV = 1;
        public const int TYPE_DDV = 2;
        public const int TYPE_SS = 3;
        public const int TYPE_PC = 4;
        public const int TYPE_NC = 5;
        public const int TYPE_LIQ = 6;
        public const int TYPE_BUDGET = 7;
        public const int TYPE_CLOSING = 8;

        //Static value for application types used for checking DM application type
        //i.e: displaying pending applications
        //Example : [Expense.Type = TYPE_DM_VEN] instead of [Expense.Type = 1]
        public const int TYPE_DM_VEN = 1;
        public const int TYPE_DM_CHK = 2;
        public const int TYPE_DM_ACC = 3;
        public const int TYPE_DM_ACC_GRP = 4;
        public const int TYPE_DM_DEPT = 5;
        public const int TYPE_DM_VAT = 6;
        public const int TYPE_DM_FBT = 7;
        public const int TYPE_DM_TR = 8;
        public const int TYPE_DM_CURR = 9;
        public const int TYPE_DM_REG_EMP = 10;
        public const int TYPE_DM_TEMP_EMP = 11;
        public const int TYPE_DM_BCS = 12;
        public const int TYPE_DM_CUST= 13;

        //Static value for branch Type/Name/Code
        //i.e: determining what is the branch code for the transaction
        //Example : RBU = 767, FCDU = 789
        public static readonly string BRANCH_RBU = "767";
        public static readonly string BRANCH_FCDU = "789";

        //Static value for application codes used for Expense Transactio No.
        //Example : [CV]-2019-100001
        private static readonly Dictionary<int, string> codeDictio = new Dictionary<int, string>
            {
                {0,"DM" },
                {1,"CV" },
                {2,"DDV"},
                {3,"SSV"},
                {4,"PCV"},
                {5,"NCV"},
                {6,"LIQ"}
            };

        //Static value for the view path of voucher layout
        public static readonly string VOUCHER_LAYOUT = "EntryReports/_Voucherlayout";

        public static string getStatus(int statusCode)
        {
            switch (statusCode)
            {
                case 1: return "PENDING";
                case 2: return "VERIFIED";
                case 3: return "APPROVED";
                case 4: return "POSTED";
                case 5: return "REJECTED";
                case 6: return "DELETED";
                case 7: return "NEW";
                case 8: return "EDIT";
                case 9: return "DELETE";
                case 10: return "REVERSED";
                case 11: return "CLOSED";
                case 12: return "OPEN";
                case 13: return "FOR PRINTING";
                case 14: return "POSTED";
                case 15: return "ERROR";
                case 16: return "REVERSING";
                case 17: return "REVERSING ERROR";
                case 18: return "REVERSING COMPLETED";
                case 19: return "RE-SENDING";
                case 20: return "RE-SENDING COMPLETED";
                case 21: return "REVERSED DUE TO G-BASE ERROR";
            };

            return "";
    }

        public static string getApplicationCode(int appType)
        {
            return codeDictio[appType];
        }

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
                case 6: return "Suspense Sundry (Liquidation)";
                case 7: return "Non-Cash (Liquidation)";
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
        public static readonly string ENTRY_NEW_PCV = "/Home/AddNewPCV";
        public static readonly string ENTRY_NEW_SS = "/Home/AddNewSS";
        public static readonly string ENTRY_NEW_NC = "/Home/AddNewNC";
        public static readonly string ENTRY_VIEW_CV = "/Home/View_CV";
        public static readonly string ENTRY_VIEW_DDV = "/Home/View_DDV";
        public static readonly string ENTRY_VIEW_PCV = "/Home/View_PCV";
        public static readonly string ENTRY_VIEW_SS = "/Home/View_SS";
        public static readonly string ENTRY_VIEW_NC = "/Home/View_NC";
        public static readonly string ENTRY_MOD_CV = "/Home/VerAppModCV";
        public static readonly string ENTRY_MOD_DDV = "/Home/VerAppModDDV";
        public static readonly string ENTRY_MOD_PCV = "/Home/VerAppModPCV";
        public static readonly string ENTRY_MOD_SS = "/Home/VerAppModSS";
        public static readonly string ENTRY_MOD_NC = "/Home/VerAppModNC";
        public static readonly string LIQ_MAIN = "/Home/Liquidation_Main";
        public static readonly string AMOR = "/Home/Amortization";
        public static readonly string LIQ_SS = "/Home/Liquidation_SS";
        public static readonly string LIQ_New_SS = "/Home/Liquidation_AddNewSS";
        public static readonly string LIQ_View_SS = "/Home/View_Liquidation_SS";
        public static readonly string LIQ_MOD_SS = "/Home/Liquidation_VerAppModSS";

        public static readonly List<string> ENTRY_VALS = new List<string> {
            GlobalSystemValues.ENTRY,
            GlobalSystemValues.ENTRY_CV,
            GlobalSystemValues.ENTRY_DDV,
            GlobalSystemValues.ENTRY_NC,
            GlobalSystemValues.ENTRY_PCV,
            GlobalSystemValues.ENTRY_SS,
            GlobalSystemValues.ENTRY_Liquidation,
            GlobalSystemValues.ENTRY_NEW_CV,
            GlobalSystemValues.ENTRY_NEW_DDV,
            GlobalSystemValues.ENTRY_NEW_PCV,
            GlobalSystemValues.ENTRY_NEW_SS,
            GlobalSystemValues.ENTRY_NEW_NC,
            GlobalSystemValues.ENTRY_VIEW_CV,
            GlobalSystemValues.ENTRY_VIEW_DDV,
            GlobalSystemValues.ENTRY_VIEW_PCV,
            GlobalSystemValues.ENTRY_VIEW_SS,
            GlobalSystemValues.ENTRY_VIEW_NC,
            GlobalSystemValues.ENTRY_MOD_CV,
            GlobalSystemValues.ENTRY_MOD_DDV,
            GlobalSystemValues.ENTRY_MOD_PCV,
            GlobalSystemValues.ENTRY_MOD_SS,
            GlobalSystemValues.ENTRY_MOD_NC,
            GlobalSystemValues.LIQ_MAIN,
            GlobalSystemValues.AMOR,
            GlobalSystemValues.LIQ_SS,
            GlobalSystemValues.LIQ_New_SS,
            GlobalSystemValues.LIQ_View_SS,
            GlobalSystemValues.LIQ_MOD_SS
        };
        //Static values for entry path use for checking if HomeTabsPartial will be shown.
        //Example: instead of [if Context.Request.Path.ToString() == "/Home/Index"] use (GlobalSystemValues.HOME_VALS.Contains(Context.Request.Path.ToString())) ? true : false;
        public static readonly string HOME_INDEX = "/Home/Index";
        public static readonly string HOME_PENDING = "/Home/Pending";
        public static readonly string HOME_HISTORY = "/Home/History";
        public static readonly string TRANSFAILED_INDEX = "/TransFailed/Index";


        public static readonly string HOME_DM = "/Home/DM";

        public static readonly List<string> HOME_VALS = new List<string> {
            GlobalSystemValues.HOME_INDEX,
            GlobalSystemValues.HOME_PENDING,
            GlobalSystemValues.HOME_HISTORY,
            GlobalSystemValues.TRANSFAILED_INDEX
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
        public static readonly int NC_MISCELLANEOUS_ENTRIES = 11;

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
            GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES
        };

        //Static values for Document Type to be printed in all Expense Transactions
        public static readonly int PS_LOI = 1;
        public static readonly int PS_BIR2307 = 2;
        public static readonly int PS_CDD = 3;
        public static readonly int PS_Check = 4;
        public static readonly int PS_Voucher = 5;

        public static readonly List<int> PS_DocType = new List<int> {
            GlobalSystemValues.PS_LOI,
            GlobalSystemValues.PS_BIR2307,
            GlobalSystemValues.PS_CDD,
            GlobalSystemValues.PS_Check,
            GlobalSystemValues.PS_Voucher
        };

        public static readonly List<SelectListItem> NC_CATEGORIES_SELECT = new List<SelectListItem> {
                new SelectListItem { Text = "LS PAYROLL", Value = NC_CATEGORIES[0].ToString(), Selected = true },
                new SelectListItem { Text = "TAX REMITTANCE", Value = NC_CATEGORIES[1].ToString(), Selected = false },
                new SelectListItem { Text = "MONTHLY ROSS BILL", Value = NC_CATEGORIES[2].ToString(), Selected = false },
                new SelectListItem { Text = "PSSC", Value = NC_CATEGORIES[3].ToString(), Selected = false },
                new SelectListItem { Text = "PCHC", Value = NC_CATEGORIES[4].ToString(), Selected = false },
                new SelectListItem { Text = "DEPRECIATION", Value = NC_CATEGORIES[5].ToString(), Selected = false },
                new SelectListItem { Text = "PETTY CASH REPLENISHMENT", Value = NC_CATEGORIES[6].ToString(), Selected = false },
                new SelectListItem { Text = "JS PAYROLL", Value = NC_CATEGORIES[7].ToString(), Selected = false },
                new SelectListItem { Text = "RETURN OF JS PAYROLL", Value = NC_CATEGORIES[8].ToString(), Selected = false },
                new SelectListItem { Text = "MISCELLANEOUS ENTRIES", Value = NC_CATEGORIES[9].ToString(), Selected = false }
        };
        //Static values for account types of entries use for Non Cash.
        public static readonly int NC_DEBIT = 1;
        public static readonly int NC_CREDIT = 2;
        public static readonly int NC_EWT = 3;

        //Static values for Notification.
        public static readonly int UF_ALL = 0; // For General Messages; Notif for someone creating changes in

        //G-Base Error handling action value
        public const int GBaseErrResend = 1;
        public const int GBaseErrReverse = 2;
        public const int GBaseErrReverseResend = 3;
        public const int GBaseErrReject = 4;

        //System message
        public static string MESSAGE = "";
        public static readonly string MESSAGE1 = "You can only edit the entry in Pending or Rejected status.";
        public static readonly string MESSAGE2 = "You can only delete the entry in Pending or Rejected status.";
        public static readonly string MESSAGE3 = "This entry was edited/deleted by the maker. Please check the entry again.";
        public static readonly string MESSAGE4 = "This entry was already approved by Approver. Cannot proceed with your request.";
        public static readonly string MESSAGE5 = "This entry was already in For Printing status. Cannot proceed with your request.";
        public static readonly string MESSAGE6 = "This entry was already in For Closing status. Cannot proceed with your request.";
        public static readonly string MESSAGE7 = "This entry was already in Rejected status. Cannot proceed with your request.";
        public static readonly string MESSAGE8 = "This entry was already verified by other verifier/s. Cannot proceed with your request.";
        public static readonly string MESSAGE9 = "There are is no check series registered. Check the Data Maintenance and try again.";
        public static readonly string MESSAGE10 = "Only approver can access to G-Base transaction failure list page.";

        //Static values for DEBIT/CREDIT transaction type.
        public static readonly int TRANS_DEBIT = 1;
        public static readonly int TRANS_CREDIT = 2;
    }
    public class CONSTANT_NC_VALS
    {
        public int accID { get; set; }
        public string accNo { get; set; }
        public string accName { get; set; }
    }
    public class CONSTANT_CCY_VALS
    {
        public int currID { get; set; }
        public int currMasterID { get; set; }
        public string currABBR { get; set; }
    }


    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_LSPAYROLL
    {
        public static EntryNCViewModel Populate_LSPAYROLL(DMCurrencyModel currDetails, List<CONSTANT_NC_VALS> accList)
        {
           
                return new EntryNCViewModel
            {
                NC_Category_ID = 1,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "T-L/S PAY:",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[0].accID,
                                ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[1].accID,
                                ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[2].accID,
                                ExpNCDtlAcc_Acc_Name = accList[2].accNo + " - " + accList[2].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[3].accID,
                                ExpNCDtlAcc_Acc_Name = accList[3].accNo + " - " + accList[3].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[4].accID,
                                ExpNCDtlAcc_Acc_Name = accList[4].accNo + " - " + accList[4].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[5].accID,
                                ExpNCDtlAcc_Acc_Name = accList[5].accNo + " - " + accList[5].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[6].accID,
                                ExpNCDtlAcc_Acc_Name = accList[6].accNo + " - " + accList[6].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[7].accID,
                                ExpNCDtlAcc_Acc_Name = accList[7].accNo + " - " + accList[7].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[8].accID,
                                ExpNCDtlAcc_Acc_Name = accList[8].accNo + " - " + accList[8].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[9].accID,
                                ExpNCDtlAcc_Acc_Name = accList[9].accNo + " - " + accList[9].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[10].accID,
                                ExpNCDtlAcc_Acc_Name = accList[10].accNo + " - " + accList[10].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[11].accID,
                                ExpNCDtlAcc_Acc_Name = accList[11].accNo + " - " + accList[11].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[12].accID,
                                ExpNCDtlAcc_Acc_Name = accList[12].accNo + " - " + accList[12].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[13].accID,
                                ExpNCDtlAcc_Acc_Name = accList[13].accNo + " - " + accList[13].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[14].accID,
                                ExpNCDtlAcc_Acc_Name = accList[14].accNo + " - " + accList[14].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[15].accID,
                                ExpNCDtlAcc_Acc_Name = accList[15].accNo + " - " + accList[15].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[16].accID,
                                ExpNCDtlAcc_Acc_Name = accList[16].accNo + " - " + accList[16].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[17].accID,
                                ExpNCDtlAcc_Acc_Name = accList[17].accNo + " - " + accList[17].accName,
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
                        ExpNCDtl_Remarks_Period = " ",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[18].accID,
                                ExpNCDtlAcc_Acc_Name = accList[18].accNo + " - " + accList[18].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[19].accID,
                                ExpNCDtlAcc_Acc_Name = accList[19].accNo + " - " + accList[19].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[20].accID,
                                ExpNCDtlAcc_Acc_Name = accList[20].accNo + " - " + accList[20].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[21].accID,
                                ExpNCDtlAcc_Acc_Name = accList[21].accNo + " - " + accList[21].accName,
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
        public static EntryNCViewModel Populate_TAXREMITTANCE(DMCurrencyModel currDetails, List<CONSTANT_NC_VALS> accList)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 2,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "REMIT RTGS PHILPASS",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[0].accID,
                                ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[1].accID,
                                ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[2].accID,
                                ExpNCDtlAcc_Acc_Name = accList[2].accNo + " - " + accList[2].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[3].accID,
                                ExpNCDtlAcc_Acc_Name = accList[3].accNo + " - " + accList[3].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[4].accID,
                                ExpNCDtlAcc_Acc_Name = accList[4].accNo + " - " + accList[4].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[5].accID,
                                ExpNCDtlAcc_Acc_Name = accList[5].accNo + " - " + accList[5].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[6].accID,
                                ExpNCDtlAcc_Acc_Name = accList[6].accNo + " - " + accList[6].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[7].accID,
                                ExpNCDtlAcc_Acc_Name = accList[7].accNo + " - " + accList[7].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[8].accID,
                                ExpNCDtlAcc_Acc_Name = accList[8].accNo + " - " + accList[8].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[9].accID,
                                ExpNCDtlAcc_Acc_Name = accList[9].accNo + " - " + accList[9].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[10].accID,
                                ExpNCDtlAcc_Acc_Name = accList[10].accNo + " - " + accList[10].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[11].accID,
                                ExpNCDtlAcc_Acc_Name = accList[11].accNo + " - " + accList[11].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[12].accID,
                                ExpNCDtlAcc_Acc_Name = accList[12].accNo + " - " + accList[12].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[13].accID,
                                ExpNCDtlAcc_Acc_Name = accList[13].accNo + " - " + accList[13].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[14].accID,
                                ExpNCDtlAcc_Acc_Name = accList[14].accNo + " - " + accList[14].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[15].accID,
                                ExpNCDtlAcc_Acc_Name = accList[15].accNo + " - " + accList[15].accName,
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
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[16].accID,
                                ExpNCDtlAcc_Acc_Name = accList[16].accNo + " - " + accList[16].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[17].accID,
                                ExpNCDtlAcc_Acc_Name = accList[17].accNo + " - " + accList[17].accName,
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
        public static EntryNCViewModel Populate_MONTHLYROSSBILL(DMCurrencyModel currDetails, List<CONSTANT_NC_VALS> accList)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 3,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "BSP:MONTHLY ROSS BILL",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[0].accID,
                                ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[1].accID,
                                ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
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
        public static EntryNCViewModel Populate_PSSC(DMCurrencyModel currDetails, List<CONSTANT_NC_VALS> accList)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 4,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "PSSC:2% PDDTS CHARGES",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[0].accID,
                                ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_DEBIT
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[1].accID,
                                ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_EWT
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[2].accID,
                                ExpNCDtlAcc_Acc_Name = accList[2].accNo + " - " + accList[2].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_CREDIT
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
        public static EntryNCViewModel Populate_PCHC(DMCurrencyModel currDetails, List<CONSTANT_NC_VALS> accList)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 5,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "PCHC:2% PROC FEES",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[0].accID,
                                ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_DEBIT
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[1].accID,
                                ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_EWT
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[2].accID,
                                ExpNCDtlAcc_Acc_Name = accList[2].accNo + " - " + accList[2].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_CREDIT
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "2% PAS 5",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[3].accID,
                                ExpNCDtlAcc_Acc_Name = accList[3].accNo + " - " + accList[3].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_DEBIT
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[4].accID,
                                ExpNCDtlAcc_Acc_Name = accList[4].accNo + " - " + accList[4].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_EWT
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[5].accID,
                                ExpNCDtlAcc_Acc_Name = accList[5].accNo + " - " + accList[5].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_CREDIT
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
        public static EntryNCViewModel Populate_DEPRECIATION(DMCurrencyModel currDetails, List<CONSTANT_NC_VALS> accList)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 6,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "DEPRECIATION:BANK PREMISES",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[0].accID,
                                ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[1].accID,
                                ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
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
                        ExpNCDtl_Remarks_Desc = "DEPRECIATION:BK FIR FIX EQUIP",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[2].accID,
                                ExpNCDtlAcc_Acc_Name = accList[2].accNo + " - " + accList[2].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[3].accID,
                                ExpNCDtlAcc_Acc_Name = accList[3].accNo + " - " + accList[3].accName,
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
                        ExpNCDtl_Remarks_Desc = "DEPRECIATION: SOFTWARE",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[4].accID,
                                ExpNCDtlAcc_Acc_Name = accList[4].accNo + " - " + accList[4].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[5].accID,
                                ExpNCDtlAcc_Acc_Name = accList[5].accNo + " - " + accList[5].accName,
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
        public static EntryNCViewModel Populate_PETTYCASHREPLENISHMENT(DMCurrencyModel currDetails, List<CONSTANT_NC_VALS> accList)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 7,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "PETTY CASH REPLENISHMENT",
                        ExpNCDtl_Remarks_Period = " ",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[0].accID,
                                ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[1].accID,
                                ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
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
        public static List<ExpenseEntryNCDtlViewModel> Populate_CDD_Instruc_Sheet(DMCurrencyModel currDetails)
        {
            return new List<ExpenseEntryNCDtlViewModel>()
            {
                new ExpenseEntryNCDtlViewModel
                {
                    ExpNCDtl_Remarks_Desc = "PETTY CASH REPLENISHMENT",
                        ExpNCDtl_Remarks_Period = " ",
                    ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                    {
                        new ExpenseEntryNCDtlAccViewModel{
                            ExpNCDtlAcc_Acc_ID = 487,
                            ExpNCDtlAcc_Acc_Name = "B79-767-111111 (14017) - COMPUTER SUSPENSE",
                            ExpNCDtlAcc_Amount = 0,
                            ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                            ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                            ExpNCDtlAcc_Inter_Rate = 0,
                            ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_DEBIT
                        },
                        new ExpenseEntryNCDtlAccViewModel{
                            ExpNCDtlAcc_Acc_ID = 489,
                            ExpNCDtlAcc_Acc_Name = "H79-767-151111 (00204)",
                            ExpNCDtlAcc_Amount = 0,
                            ExpNCDtlAcc_Curr_ID = currDetails.Curr_ID,
                            ExpNCDtlAcc_Curr_Name = currDetails.Curr_CCY_ABBR,
                            ExpNCDtlAcc_Inter_Rate = 0,
                            ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_CREDIT
                        }
                    }
                }
            };
        }
    }
    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_JSPAYROLL
    {
        public static EntryNCViewModel Populate_JSPAYROLL(DMCurrencyModel currDetailsPHP, DMCurrencyModel currDetailsUSD, List<CONSTANT_NC_VALS> accList)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 8,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "J/S PAYROLL: ",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[0].accID,
                                ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[1].accID,
                                ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "WHT J/S PAYROLL:",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[2].accID,
                                ExpNCDtlAcc_Acc_Name = accList[2].accNo + " - " + accList[2].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[3].accID,
                                ExpNCDtlAcc_Acc_Name = accList[3].accNo + " - " + accList[3].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "J/S PAYROLL: ",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[4].accID,
                                ExpNCDtlAcc_Acc_Name = accList[4].accNo + " - " + accList[4].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsUSD.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsUSD.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[5].accID,
                                ExpNCDtlAcc_Acc_Name = accList[5].accNo + " - " + accList[5].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsUSD.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsUSD.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "J/S PAYROLL: ",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[6].accID,
                                ExpNCDtlAcc_Acc_Name = accList[6].accNo + " - " + accList[6].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[7].accID,
                                ExpNCDtlAcc_Acc_Name = accList[7].accNo + " - " + accList[7].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsUSD.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsUSD.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "JS PAYROLL: CAR ADJUST",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[8].accID,
                                ExpNCDtlAcc_Acc_Name = accList[8].accNo + " - " + accList[8].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[9].accID,
                                ExpNCDtlAcc_Acc_Name = accList[9].accNo + " - " + accList[9].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
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
    public class CONSTANT_NC_RETURN_OF_JSPAYROLL
    {
        public static EntryNCViewModel Populate_RETURN_OF_JSPAYROLL(DMCurrencyModel currDetailsPHP, DMCurrencyModel currDetailsUSD, List<CONSTANT_NC_VALS> accList)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 9,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[0].accID,
                                ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsUSD.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsUSD.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[1].accID,
                                ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsUSD.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsUSD.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[2].accID,
                                ExpNCDtlAcc_Acc_Name = accList[2].accNo + " - " + accList[2].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsUSD.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsUSD.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[3].accID,
                                ExpNCDtlAcc_Acc_Name = accList[3].accNo + " - " + accList[3].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    },
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[4].accID,
                                ExpNCDtlAcc_Acc_Name = accList[4].accNo + " - " + accList[4].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 1
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = accList[5].accID,
                                ExpNCDtlAcc_Acc_Name = accList[5].accNo + " - " + accList[5].accName,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = 2
                            }
                        }
                    }
                }
            };
        }
        public static List<ExpenseEntryNCDtlViewModel> Populate_CDD_Instruc_Sheet(DMCurrencyModel currDetailsPHP, DMCurrencyModel currDetailsUSD, List<CONSTANT_NC_VALS> accList)
        {
            return new List<ExpenseEntryNCDtlViewModel>()
            {
                new ExpenseEntryNCDtlViewModel
                {
                    ExpNCDtl_Remarks_Desc = "",
                        ExpNCDtl_Remarks_Period = "",
                    ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                    {
                        new ExpenseEntryNCDtlAccViewModel{
                            ExpNCDtlAcc_Acc_ID = accList[0].accID,
                            ExpNCDtlAcc_Acc_Name = accList[0].accNo + " - " + accList[0].accName,
                            ExpNCDtlAcc_Amount = 0,
                            ExpNCDtlAcc_Curr_ID = currDetailsUSD.Curr_ID,
                            ExpNCDtlAcc_Curr_Name = currDetailsUSD.Curr_CCY_ABBR,
                            ExpNCDtlAcc_Inter_Rate = 0,
                            ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_DEBIT
                        },
                        new ExpenseEntryNCDtlAccViewModel{
                            ExpNCDtlAcc_Acc_ID = accList[1].accID,
                            ExpNCDtlAcc_Acc_Name = accList[1].accNo + " - " + accList[1].accName,
                            ExpNCDtlAcc_Amount = 0,
                            ExpNCDtlAcc_Curr_ID = currDetailsUSD.Curr_ID,
                            ExpNCDtlAcc_Curr_Name = currDetailsUSD.Curr_CCY_ABBR,
                            ExpNCDtlAcc_Inter_Rate = 0,
                            ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_CREDIT
                        }
                    }
                }
            };
        }
    }
    //NON CASH CONSTANT VALUES
    public class CONSTANT_NC_MISC_ENTRIES
    {
        public static EntryNCViewModel Populate_MISC_ENTRIES(DMCurrencyModel currDetailsPHP, DMAccountModel initialAcc)
        {
            return new EntryNCViewModel
            {
                NC_Category_ID = 11,
                ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>()
                {
                    new ExpenseEntryNCDtlViewModel
                    {
                        ExpNCDtl_Remarks_Desc = "",
                        ExpNCDtl_Remarks_Period = "",
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>
                        {
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = initialAcc.Account_ID,
                                ExpNCDtlAcc_Acc_Name = initialAcc.Account_Name,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_DEBIT
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = initialAcc.Account_ID,
                                ExpNCDtlAcc_Acc_Name = initialAcc.Account_Name,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_CREDIT
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = initialAcc.Account_ID,
                                ExpNCDtlAcc_Acc_Name = initialAcc.Account_Name,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_CREDIT
                            },
                            new ExpenseEntryNCDtlAccViewModel{
                                ExpNCDtlAcc_Acc_ID = initialAcc.Account_ID,
                                ExpNCDtlAcc_Acc_Name = initialAcc.Account_Name,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Curr_ID = currDetailsPHP.Curr_ID,
                                ExpNCDtlAcc_Curr_Name = currDetailsPHP.Curr_CCY_ABBR,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_CREDIT
                            }                        }
                    }
                }
            };
        }
    }
    //Populate DDV Inter-Entity Particulars
    public class CONSTANT_DDV_INTER_PARTICULARS
    {
        public static List<InterEntityParticular> PopulateParticular1(string accName, string Curr1Abbr, decimal Curr1Amt, decimal Curr2Amt, float InterRate, int accID, int curr1ID, List<CONSTANT_NC_VALS> accList)
        {
            return new List<InterEntityParticular> {
                new InterEntityParticular {
                    Particular_Acc_ID = accID,
                    Particular_Account_Name = accName,
                    Particular_DebCurr_ID = curr1ID,
                    Particular_Debit_Curr = Curr1Abbr,
                    Particular_Debit_Amount = Curr1Amt + (Curr2Amt * (decimal)InterRate),
                    Particular_CredCurr_ID= 0,
                    Particular_Credit_Curr = "",
                    Particular_Credit_Amount = 0,
                    Particular_Type_ID = GlobalSystemValues.NC_DEBIT
                },
                new InterEntityParticular {
                    Particular_Acc_ID = accList[0].accID,
                    Particular_Account_Name = accList[0].accNo + " - " + accList[0].accName,
                    Particular_DebCurr_ID = 0,
                    Particular_CredCurr_ID= curr1ID,
                    Particular_Debit_Curr = "",
                    Particular_Debit_Amount = 0,
                    Particular_Credit_Curr = Curr1Abbr,
                    Particular_Credit_Amount = Curr2Amt * (decimal)InterRate,
                    Particular_Type_ID = GlobalSystemValues.NC_CREDIT
                },
                new InterEntityParticular {
                    Particular_Acc_ID = accList[1].accID,
                    Particular_Account_Name = accList[1].accNo + " - " + accList[1].accName,
                    Particular_DebCurr_ID = 0,
                    Particular_CredCurr_ID= curr1ID,
                    Particular_Debit_Curr = "",
                    Particular_Debit_Amount = 0,
                    Particular_Credit_Curr = Curr1Abbr,
                    Particular_Credit_Amount = Curr1Amt,
                    Particular_Type_ID = GlobalSystemValues.NC_CREDIT
                },
            };
        }
        public static List<InterEntityParticular> PopulateParticular2(string Curr1Abbr, string Curr2Abbr, decimal Curr2Amt, float InterRate, int curr1ID, int curr2ID, List<CONSTANT_NC_VALS> accList)
        {
            return new List<InterEntityParticular> {
                new InterEntityParticular {
                    Particular_Acc_ID = accList[0].accID,
                    Particular_Account_Name = accList[0].accNo + " - " + accList[0].accName,
                    Particular_DebCurr_ID = curr1ID,
                    Particular_CredCurr_ID= 0,
                    Particular_Debit_Curr = Curr1Abbr,
                    Particular_Debit_Amount = Curr2Amt * (decimal)InterRate,
                    Particular_Credit_Curr = "",
                    Particular_Credit_Amount = 0,
                    Particular_Type_ID = GlobalSystemValues.NC_DEBIT
                },
                new InterEntityParticular {
                    Particular_Acc_ID = accList[1].accID,
                    Particular_Account_Name = accList[1].accNo + " - " + accList[1].accName,
                    Particular_DebCurr_ID = 0,
                    Particular_CredCurr_ID= curr2ID,
                    Particular_Debit_Curr = "",
                    Particular_Debit_Amount = 0,
                    Particular_Credit_Curr = Curr2Abbr,
                    Particular_Credit_Rate = InterRate,
                    Particular_Credit_Amount = Curr2Amt,
                    Particular_Type_ID = GlobalSystemValues.NC_CREDIT
                }
            };
        }
        public static List<InterEntityParticular> PopulateParticular3(string Curr2Abbr, decimal Curr2Amt, int curr2ID, List<CONSTANT_NC_VALS> accList)
        {
            return new List<InterEntityParticular> {
                new InterEntityParticular {
                    Particular_Acc_ID = accList[0].accID,
                    Particular_Account_Name = accList[0].accNo + " - " + accList[0].accName,
                    Particular_DebCurr_ID = 0,
                    Particular_CredCurr_ID= curr2ID,
                    Particular_Debit_Curr = Curr2Abbr,
                    Particular_Debit_Amount = Curr2Amt,
                    Particular_Credit_Curr = "",
                    Particular_Credit_Amount = 0,
                    Particular_Type_ID = GlobalSystemValues.NC_DEBIT
                },
                new InterEntityParticular {
                    Particular_Acc_ID = accList[1].accID,
                    Particular_Account_Name = accList[1].accNo + " - " + accList[1].accName,
                    Particular_DebCurr_ID = 0,
                    Particular_CredCurr_ID= curr2ID,
                    Particular_Debit_Curr = "",
                    Particular_Debit_Amount = 0,
                    Particular_Credit_Curr = Curr2Abbr,
                    Particular_Credit_Amount = Curr2Amt,
                    Particular_Type_ID = GlobalSystemValues.NC_CREDIT
                }
            };
        }
    }

    //Populate DDV Inter-Entity Particulars READONLY
    public class CONSTANT_DDV_INTER_PARTICULARS_READONLY
    {
        public static List<InterEntityParticular> PopulateParticular1(string accName, string Curr1Abbr, decimal DebitAmt, decimal CreditAmt, decimal Curr1Amt, decimal Curr2Amt, float InterRate, int accID, int curr1ID, List<CONSTANT_NC_VALS> accList)
        {
            return new List<InterEntityParticular> {
                    new InterEntityParticular {
                        Particular_Acc_ID = accID,
                        Particular_Account_Name = accName,
                        Particular_DebCurr_ID = curr1ID,
                        Particular_Debit_Curr = Curr1Abbr,
                        Particular_Debit_Amount = DebitAmt,
                        Particular_CredCurr_ID= 0,
                        Particular_Credit_Curr = "",
                        Particular_Credit_Amount = 0,
                        Particular_Type_ID = GlobalSystemValues.NC_DEBIT
                    },
                    new InterEntityParticular {
                        Particular_Acc_ID = accList[0].accID,
                        Particular_Account_Name = accList[0].accNo + " - " + accList[0].accName,
                        Particular_DebCurr_ID = 0,
                        Particular_CredCurr_ID= curr1ID,
                        Particular_Debit_Curr = "",
                        Particular_Debit_Amount = 0,
                        Particular_Credit_Curr = Curr1Abbr,
                        Particular_Credit_Amount = CreditAmt,
                        Particular_Type_ID = GlobalSystemValues.NC_CREDIT
                    },
                    new InterEntityParticular {
                        Particular_Acc_ID = accList[1].accID,
                        Particular_Account_Name = accList[1].accNo + " - " + accList[1].accName,
                        Particular_DebCurr_ID = 0,
                        Particular_CredCurr_ID= curr1ID,
                        Particular_Debit_Curr = "",
                        Particular_Debit_Amount = 0,
                        Particular_Credit_Curr = Curr1Abbr,
                        Particular_Credit_Amount = Curr1Amt,
                        Particular_Type_ID = GlobalSystemValues.NC_CREDIT
                    },
                };
        }
        public static List<InterEntityParticular> PopulateParticular2(string Curr1Abbr, string Curr2Abbr, decimal DebitAmt, decimal Curr2Amt, float InterRate, int curr1ID, int curr2ID, List<CONSTANT_NC_VALS> accList)
        {
            return new List<InterEntityParticular> {
                    new InterEntityParticular {
                        Particular_Acc_ID = accList[0].accID,
                        Particular_Account_Name = accList[0].accNo + " - " + accList[0].accName,
                        Particular_DebCurr_ID = curr1ID,
                        Particular_CredCurr_ID= 0,
                        Particular_Debit_Curr = Curr1Abbr,
                        Particular_Debit_Amount = DebitAmt,
                        Particular_Credit_Curr = "",
                        Particular_Credit_Amount = 0,
                        Particular_Type_ID = GlobalSystemValues.NC_DEBIT
                    },
                    new InterEntityParticular {
                        Particular_Acc_ID = accList[1].accID,
                        Particular_Account_Name = accList[1].accNo + " - " + accList[1].accName,
                        Particular_DebCurr_ID = 0,
                        Particular_CredCurr_ID= curr2ID,
                        Particular_Debit_Curr = "",
                        Particular_Debit_Amount = 0,
                        Particular_Credit_Curr = Curr2Abbr,
                        Particular_Credit_Rate = InterRate,
                        Particular_Credit_Amount = Curr2Amt,
                        Particular_Type_ID = GlobalSystemValues.NC_CREDIT
                    }
                };
        }
        public static List<InterEntityParticular> PopulateParticular3(string Curr2Abbr, decimal Curr2Amt, int curr2ID, List<CONSTANT_NC_VALS> accList)
        {
            return new List<InterEntityParticular> {
                    new InterEntityParticular {
                        Particular_Acc_ID = accList[0].accID,
                        Particular_Account_Name = accList[0].accNo + " - " + accList[0].accName,
                        Particular_DebCurr_ID = 0,
                        Particular_CredCurr_ID= curr2ID,
                        Particular_Debit_Curr = Curr2Abbr,
                        Particular_Debit_Amount = Curr2Amt,
                        Particular_Credit_Curr = "",
                        Particular_Credit_Amount = 0,
                        Particular_Type_ID = GlobalSystemValues.NC_DEBIT
                    },
                    new InterEntityParticular {
                        Particular_Acc_ID = accList[1].accID,
                        Particular_Account_Name = accList[1].accNo + " - " + accList[1].accName,
                        Particular_DebCurr_ID = 0,
                        Particular_CredCurr_ID= curr2ID,
                        Particular_Debit_Curr = "",
                        Particular_Debit_Amount = 0,
                        Particular_Credit_Curr = Curr2Abbr,
                        Particular_Credit_Amount = Curr2Amt,
                        Particular_Type_ID = GlobalSystemValues.NC_CREDIT
                    }
                };
        }
    }
}
