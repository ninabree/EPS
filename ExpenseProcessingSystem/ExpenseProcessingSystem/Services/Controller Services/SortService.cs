using ExpenseProcessingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Controller_Services
{
    public class SortService
    {
        public SortViewModel SortData(List<BMViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "acc_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Account);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "type":
                    tempList = tempList.OrderBy(s => s.BM_Type);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "type_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Type);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "budget":
                    tempList = tempList.OrderBy(s => s.BM_Budget);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "budget_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Budget);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "curr_budget":
                    tempList = tempList.OrderBy(s => s.BM_Curr_Budget);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "curr_budget_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Curr_Budget);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "last_trans_date":
                    tempList = tempList.OrderBy(s => s.BM_Last_Trans_Date);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "last_trans_date_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Last_Trans_Date);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "last_budget_apprvl":
                    tempList = tempList.OrderBy(s => s.BM_Last_Budget_Approval);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "last_budget_apprvl_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Last_Budget_Approval);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.BM_Account);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<DMPayeeViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Payee_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "payee_TIN":
                    tempList = tempList.OrderBy(s => s.Payee_TIN);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "payee_TIN_desc":
                    tempList = tempList.OrderByDescending(s => s.Payee_TIN);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "payee_add":
                    tempList = tempList.OrderBy(s => s.Payee_Address);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "payee_add_desc":
                    tempList = tempList.OrderByDescending(s => s.Payee_Address);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "payee_type":
                    tempList = tempList.OrderBy(s => s.Payee_Type);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "payee_type_desc":
                    tempList = tempList.OrderByDescending(s => s.Payee_Type);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "payee_no":
                    tempList = tempList.OrderBy(s => s.Payee_No);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "payee_no_desc":
                    tempList = tempList.OrderByDescending(s => s.Payee_No);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "payee_creatr":
                    tempList = tempList.OrderBy(s => s.Payee_Creator_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "payee_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Payee_Creator_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "payee_approvr":
                    tempList = tempList.OrderBy(s => s.Payee_Approver_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "payee_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Payee_Approver_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "payee_stat":
                    tempList = tempList.OrderBy(s => s.Payee_Status);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "payee_stat_desc":
                    tempList = tempList.OrderByDescending(s => s.Payee_Status);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.Payee_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<DMCheckViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "input_date_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Input_Date);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "chk_serires_from":
                    tempList = tempList.OrderBy(s => s.Check_Series_From);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_serires_from_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Series_From);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "chk_serires_to":
                    tempList = tempList.OrderBy(s => s.Check_Series_To);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_serires_to_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Series_To);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "chk_type":
                    tempList = tempList.OrderBy(s => s.Check_Type);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_type_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Type);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "chk_name":
                    tempList = tempList.OrderBy(s => s.Check_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_name_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "chk_creatr":
                    tempList = tempList.OrderBy(s => s.Check_Creator_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Creator_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "chk_approvr":
                    tempList = tempList.OrderBy(s => s.Check_Approver_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Approver_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "chk_stat":
                    tempList = tempList.OrderBy(s => s.Check_Status);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_stat_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Status);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.Check_Input_Date);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<DMAccountViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_code":
                    tempList = tempList.OrderBy(s => s.Account_Code);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_code_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Code);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_no":
                    tempList = tempList.OrderBy(s => s.Account_No);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_no_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_No);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_cust":
                    tempList = tempList.OrderBy(s => s.Account_Cust);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_cust_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Cust);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_div":
                    tempList = tempList.OrderBy(s => s.Account_Div);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_div_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Div);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_fund":
                    tempList = tempList.OrderBy(s => s.Account_Fund);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_fund_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Fund);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_creatr":
                    tempList = tempList.OrderBy(s => s.Account_Creator_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Creator_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_approvr":
                    tempList = tempList.OrderBy(s => s.Account_Approver_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Approver_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_last_updte":
                    tempList = tempList.OrderBy(s => s.Account_Status);
                    viewData = "glyph-9";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Status);
                    viewData = "glyph-9";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_stat":
                    tempList = tempList.OrderBy(s => s.Account_Status);
                    viewData = "glyph-10";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_stat_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Status);
                    viewData = "glyph-10";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.Account_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<UserViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "user_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_UserName);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "name":
                    tempList = tempList.OrderBy(s => s.Acc_LName);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_LName);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "dept":
                    tempList = tempList.OrderBy(s => s.Acc_Dept_ID);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_Dept_ID);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "role":
                    tempList = tempList.OrderBy(s => s.Acc_Role);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "role_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_Role);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "email":
                    tempList = tempList.OrderBy(s => s.Acc_Email);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "email_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_Email);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "comment":
                    tempList = tempList.OrderBy(s => s.Acc_Comment);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "comment_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_Comment);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "inuse":
                    tempList = tempList.OrderBy(s => s.Acc_InUse);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "inuse_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_InUse);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "creatr":
                    tempList = tempList.OrderBy(s => s.Acc_Creator_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_Creator_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "apprv":
                    tempList = tempList.OrderBy(s => s.Acc_Approver_Name);
                    viewData = "glyph-9";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "apprv_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_Approver_Name);
                    viewData = "glyph-9";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "creatr_date":
                    tempList = tempList.OrderBy(s => s.Acc_Created_Date);
                    viewData = "glyph-10";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "creatr_date_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_Created_Date);
                    viewData = "glyph-10";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "last_updt_date":
                    tempList = tempList.OrderBy(s => s.Acc_Last_Updated);
                    viewData = "glyph-11";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "last_updt_date_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_Last_Updated);
                    viewData = "glyph-11";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "stats":
                    tempList = tempList.OrderBy(s => s.Acc_Status);
                    viewData = "glyph-12";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "stats_desc":
                    tempList = tempList.OrderByDescending(s => s.Acc_Status);
                    viewData = "glyph-12";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.Acc_UserName);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<DMDeptViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Dept_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "dept_code":
                    tempList = tempList.OrderBy(s => s.Dept_Code);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_code_desc":
                    tempList = tempList.OrderByDescending(s => s.Dept_Code);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "dept_creatr":
                    tempList = tempList.OrderBy(s => s.Dept_Creator_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Dept_Creator_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "dept_approvr":
                    tempList = tempList.OrderBy(s => s.Dept_Approver_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Dept_Approver_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "dept_last_updte":
                    tempList = tempList.OrderBy(s => s.Dept_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.Dept_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "dept_stat":
                    tempList = tempList.OrderBy(s => s.Dept_Status);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_stat_desc":
                    tempList = tempList.OrderByDescending(s => s.Dept_Status);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.Dept_ID);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<DMVATViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.VAT_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "vat_code":
                    tempList = tempList.OrderBy(s => s.VAT_Rate);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vat_code_desc":
                    tempList = tempList.OrderByDescending(s => s.VAT_Rate);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "vat_creatr":
                    tempList = tempList.OrderBy(s => s.VAT_Creator_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vat_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.VAT_Creator_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "vat_approvr":
                    tempList = tempList.OrderBy(s => s.VAT_Approver_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vat_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.VAT_Approver_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "vat_last_updte":
                    tempList = tempList.OrderBy(s => s.VAT_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vat_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.VAT_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "vat_stat":
                    tempList = tempList.OrderBy(s => s.VAT_Status);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vat_stat_desc":
                    tempList = tempList.OrderByDescending(s => s.VAT_Status);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.VAT_ID);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<DMFBTViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.FBT_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "fbt_acc_":
                    tempList = tempList.OrderBy(s => s.FBT_Account);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "fbt_acc_desc":
                    tempList = tempList.OrderByDescending(s => s.FBT_Account);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "fbt_formula":
                    tempList = tempList.OrderBy(s => s.FBT_Formula);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "fbt_formula_desc":
                    tempList = tempList.OrderByDescending(s => s.FBT_Formula);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "fbt_rate":
                    tempList = tempList.OrderBy(s => s.FBT_Tax_Rate);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "fbt_rate_desc":
                    tempList = tempList.OrderByDescending(s => s.FBT_Tax_Rate);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "fbt_creatr":
                    tempList = tempList.OrderBy(s => s.FBT_Creator_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "fbt_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.FBT_Creator_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "fbt_approvr":
                    tempList = tempList.OrderBy(s => s.FBT_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "fbt_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.FBT_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "fbt_last_updte":
                    tempList = tempList.OrderBy(s => s.FBT_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "fbt_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.FBT_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "fbt_stat":
                    tempList = tempList.OrderBy(s => s.FBT_Status);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "fbt_stat_desc":
                    tempList = tempList.OrderByDescending(s => s.FBT_Status);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.FBT_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<dynamic> list, string sortOrder)
        {
            var viewData = "";
            var vdInfo = "";
            if (list.GetType() == typeof(BMViewModel))
            {
                switch (sortOrder)
                {
                    case "acc_desc":
                        list = (dynamic)list.OrderByDescending(s => s.BM_Account);
                        viewData = "glyph-1";
                        vdInfo = "glyphicon-menu-up";
                        break;
                    case "Type":
                        list = (dynamic)list.OrderBy(s => s.BM_Type);
                        viewData = "glyph-2";
                        vdInfo = "glyphicon-menu-down";
                        break;
                    case "Type_desc":
                        list = (dynamic)list.OrderByDescending(s => s.BM_Type);
                        viewData = "glyph-2";
                        vdInfo = "glyphicon-menu-up";
                        break;
                    default:
                        list = (dynamic)list.OrderBy(s => s.BM_Account);
                        viewData = "glyph-1";
                        vdInfo = "glyphicon-menu-down";
                        break;
                }
            }
            SortViewModel vm = new SortViewModel
            {
                list = list,
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<DMEWTViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "nature_desc":
                    tempList = tempList.OrderByDescending(s => s.EWT_Nature);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_tax":
                    tempList = tempList.OrderBy(s => s.EWT_Tax_Rate);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_tax_desc":
                    tempList = tempList.OrderByDescending(s => s.EWT_Tax_Rate);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_atc":
                    tempList = tempList.OrderBy(s => s.EWT_ATC);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_atc_desc":
                    tempList = tempList.OrderByDescending(s => s.EWT_ATC);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_tax_descrp":
                    tempList = tempList.OrderBy(s => s.EWT_Tax_Rate_Desc);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_tax_descrp_desc":
                    tempList = tempList.OrderByDescending(s => s.EWT_Tax_Rate_Desc);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_creatr":
                    tempList = tempList.OrderBy(s => s.EWT_Creator_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.EWT_Creator_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_approvr":
                    tempList = tempList.OrderBy(s => s.EWT_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.EWT_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_last_updte":
                    tempList = tempList.OrderBy(s => s.EWT_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.EWT_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_stat":
                    tempList = tempList.OrderBy(s => s.EWT_Status);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "payee_stat_desc":
                    tempList = tempList.OrderByDescending(s => s.EWT_Status);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.EWT_Nature);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }
        public SortViewModel SortData(List<DMCurrencyViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Curr_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "curr_code":
                    tempList = tempList.OrderBy(s => s.Curr_CCY_Code);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "curr_code_desc":
                    tempList = tempList.OrderByDescending(s => s.Curr_CCY_Code);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "curr_creatr":
                    tempList = tempList.OrderBy(s => s.Curr_Creator_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "curr_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Curr_Creator_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "curr_approvr":
                    tempList = tempList.OrderBy(s => s.Curr_Approver_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "curr_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Curr_Approver_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "curr_last_updte":
                    tempList = tempList.OrderBy(s => s.Curr_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "curr_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.Curr_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "curr_stat":
                    tempList = tempList.OrderBy(s => s.Curr_Status);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "curr_stat_desc":
                    tempList = tempList.OrderByDescending(s => s.Curr_Status);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.Curr_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
            }
            SortViewModel vm = new SortViewModel
            {
                list = tempList.Cast<dynamic>().ToList(),
                viewData = viewData,
                viewDataInfo = vdInfo
            };
            return vm;
        }

        public class SortViewModel
        {
            public List<dynamic> list { get; set; }
            public string viewData { get; set; }
            public string viewDataInfo { get; set; }
        }
    }
}
