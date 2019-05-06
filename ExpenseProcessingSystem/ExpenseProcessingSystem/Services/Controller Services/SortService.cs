﻿using ExpenseProcessingSystem.ViewModels;
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
        public SortViewModel SortData(List<DMVendorViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "vendor_stat":
                    tempList = tempList.OrderBy(x => x.Vendor_Status == "For Approval" || x.Vendor_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "vendor_TIN":
                    tempList = tempList.OrderBy(s => s.Vendor_TIN);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vendor_TIN_desc":
                    tempList = tempList.OrderByDescending(s => s.Vendor_TIN);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "vendor_add":
                    tempList = tempList.OrderBy(s => s.Vendor_Address);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vendor_add_desc":
                    tempList = tempList.OrderByDescending(s => s.Vendor_Address);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "vendor_creatr":
                    tempList = tempList.OrderBy(s => s.Vendor_Creator_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vendor_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Vendor_Creator_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "vendor_approvr":
                    tempList = tempList.OrderBy(s => s.Vendor_Approver_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vendor_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Vendor_Approver_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "name":
                    tempList = tempList.OrderBy(s => s.Vendor_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Vendor_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    //tempList = tempList.OrderBy(s => s.Vendor_Name);
                    tempList = tempList.OrderByDescending(x => x.Vendor_Status == "For Approval" || x.Vendor_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
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
                case "chk_stat":
                    tempList = tempList.OrderBy(x => x.Check_Status == "For Approval" || x.Check_Status == "For Approval (For Deletion)");
                    viewData = "glyph-9";
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
                case "chk_bank":
                    tempList = tempList.OrderBy(s => s.Check_Bank_Info);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_bank_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Bank_Info);
                    viewData = "glyph-4";
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
                case "input_date":
                    tempList = tempList.OrderBy(s => s.Check_Input_Date);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "input_date_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Input_Date);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(s => s.Check_Status == "For Approval" || s.Check_Status == "For Approval (For Deletion)");
                    viewData = "glyph-9";
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
                case "acc_stat":
                    tempList = tempList.OrderBy(x => x.Account_Status == "For Approval" || x.Account_Status == "For Approval (For Deletion)");
                    viewData = "glyph-10";
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
                case "acc_fbt":
                    tempList = tempList.OrderBy(s => s.Account_FBT_MasterID);
                    viewData = "glyph-11";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_fbt_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_FBT_MasterID);
                    viewData = "glyph-11";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "name":
                    tempList = tempList.OrderBy(s => s.Account_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.Account_Status == "For Approval" || x.Account_Status == "For Approval (For Deletion)");
                    viewData = "glyph-10";
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
                    tempList = tempList.OrderByDescending(s => s.User_UserName);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "name":
                    tempList = tempList.OrderBy(s => s.User_LName);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.User_LName);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "dept":
                    tempList = tempList.OrderBy(s => s.User_Dept_ID);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_desc":
                    tempList = tempList.OrderByDescending(s => s.User_Dept_ID);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "role":
                    tempList = tempList.OrderBy(s => s.User_Role);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "role_desc":
                    tempList = tempList.OrderByDescending(s => s.User_Role);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "email":
                    tempList = tempList.OrderBy(s => s.User_Email);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "email_desc":
                    tempList = tempList.OrderByDescending(s => s.User_Email);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "comment":
                    tempList = tempList.OrderBy(s => s.User_Comment);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "comment_desc":
                    tempList = tempList.OrderByDescending(s => s.User_Comment);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "inuse":
                    tempList = tempList.OrderBy(s => s.User_InUse);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "inuse_desc":
                    tempList = tempList.OrderByDescending(s => s.User_InUse);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "creatr":
                    tempList = tempList.OrderBy(s => s.User_Creator_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.User_Creator_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "apprv":
                    tempList = tempList.OrderBy(s => s.User_Approver_Name);
                    viewData = "glyph-9";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "apprv_desc":
                    tempList = tempList.OrderByDescending(s => s.User_Approver_Name);
                    viewData = "glyph-9";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "creatr_date":
                    tempList = tempList.OrderBy(s => s.User_Created_Date);
                    viewData = "glyph-10";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "creatr_date_desc":
                    tempList = tempList.OrderByDescending(s => s.User_Created_Date);
                    viewData = "glyph-10";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "last_updt_date":
                    tempList = tempList.OrderBy(s => s.User_Last_Updated);
                    viewData = "glyph-11";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "last_updt_date_desc":
                    tempList = tempList.OrderByDescending(s => s.User_Last_Updated);
                    viewData = "glyph-11";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "stats":
                    tempList = tempList.OrderBy(s => s.User_Status);
                    viewData = "glyph-12";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "stats_desc":
                    tempList = tempList.OrderByDescending(s => s.User_Status);
                    viewData = "glyph-12";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderBy(s => s.User_UserName);
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
                case "dept_stat":
                    tempList = tempList.OrderBy(x => x.Dept_Status == "For Approval" || x.Dept_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
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
                case "name":
                    tempList = tempList.OrderBy(s => s.Dept_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Dept_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "dept_budget":
                    tempList = tempList.OrderBy(s => s.Dept_Budget_Unit);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_budget_desc":
                    tempList = tempList.OrderByDescending(s => s.Dept_Budget_Unit);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.Dept_Status == "For Approval" || x.Dept_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
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
                case "vat_stat":
                    tempList = tempList.OrderBy(x => x.VAT_Status == "For Approval" || x.VAT_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
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
                case "name":
                    tempList = tempList.OrderBy(s => s.VAT_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.VAT_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.VAT_Status == "For Approval" || x.VAT_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
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
                case "fbt_stat":
                    tempList = tempList.OrderBy(x => x.FBT_Status == "For Approval" || x.FBT_Status == "For Approval (For Deletion)");
                    viewData = "glyph-8";
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
                case "name":
                    tempList = tempList.OrderBy(s => s.FBT_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.FBT_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.FBT_Status == "For Approval" || x.FBT_Status == "For Approval (For Deletion)");
                    viewData = "glyph-8";
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
        public SortViewModel SortData(List<DMTRViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "ewt_stat":
                    tempList = tempList.OrderBy(x => x.TR_Status == "For Approval" || x.TR_Status == "For Approval (For Deletion)");
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_tax":
                    tempList = tempList.OrderBy(s => s.TR_Tax_Rate);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_tax_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Tax_Rate);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_atc":
                    tempList = tempList.OrderBy(s => s.TR_ATC);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_atc_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_ATC);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_tax_descrp":
                    tempList = tempList.OrderBy(s => s.TR_WT_Title);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_tax_descrp_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_WT_Title);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_creatr":
                    tempList = tempList.OrderBy(s => s.TR_Creator_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Creator_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_approvr":
                    tempList = tempList.OrderBy(s => s.TR_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ewt_last_updte":
                    tempList = tempList.OrderBy(s => s.TR_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ewt_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "nature":
                    tempList = tempList.OrderBy(s => s.TR_Nature);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "nature_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Nature);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "nature_inc_pay":
                    tempList = tempList.OrderBy(s => s.TR_Nature_Income_Payment);
                    viewData = "glyph-9";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "nature_inc_pay_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Nature_Income_Payment);
                    viewData = "glyph-9";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.TR_Status == "For Approval" || x.TR_Status == "For Approval (For Deletion)");
                    viewData = "glyph-8";
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
                case "curr_stat":
                    tempList = tempList.OrderBy(x => x.Curr_Status == "For Approval" || x.Curr_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "curr_abbr":
                    tempList = tempList.OrderBy(s => s.Curr_CCY_ABBR);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "curr_abbr_desc":
                    tempList = tempList.OrderByDescending(s => s.Curr_CCY_ABBR);
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
                case "name":
                    tempList = tempList.OrderBy(s => s.Curr_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Curr_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.Curr_Status == "For Approval" || x.Curr_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
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
        public SortViewModel SortData(List<DMEmpViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "regemp_stat":
                    tempList = tempList.OrderBy(x => x.Emp_Status == "For Approval" || x.Emp_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "regemp_no":
                    tempList = tempList.OrderBy(s => s.Emp_Acc_No);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "regemp_no_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Acc_No);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "regemp_creatr":
                    tempList = tempList.OrderBy(s => s.Emp_Creator_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "regemp_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Creator_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "regemp_approvr":
                    tempList = tempList.OrderBy(s => s.Emp_Approver_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "regemp_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Approver_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "regemp_last_updte":
                    tempList = tempList.OrderBy(s => s.Emp_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "name":
                    tempList = tempList.OrderBy(s => s.Emp_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.Emp_Status == "For Approval" || x.Emp_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
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
        public SortViewModel SortData(List<DMEmpViewModel> list, string sortOrder, string type)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "tempemp_stat":
                    tempList = tempList.OrderBy(x => x.Emp_Status == "For Approval" || x.Emp_Status == "For Approval (For Deletion)");
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tempemp_creatr":
                    tempList = tempList.OrderBy(s => s.Emp_Creator_Name);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tempemp_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Creator_Name);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tempemp_approvr":
                    tempList = tempList.OrderBy(s => s.Emp_Approver_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tempemp_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Approver_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tempemp_last_updte":
                    tempList = tempList.OrderBy(s => s.Emp_Last_Updated);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "dept_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Last_Updated);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "name":
                    tempList = tempList.OrderBy(s => s.Emp_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.Emp_Status == "For Approval" || x.Emp_Status == "For Approval (For Deletion)");
                    viewData = "glyph-5";
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
        public SortViewModel SortData(List<DMCustViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "cust_stat":
                    tempList = tempList.OrderBy(x => x.Cust_Status == "For Approval" || x.Cust_Status == "For Approval (For Deletion)");
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "cust_abbr":
                    tempList = tempList.OrderBy(s => s.Cust_Abbr);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "cust_abbr_desc":
                    tempList = tempList.OrderByDescending(s => s.Cust_Abbr);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "cust_no":
                    tempList = tempList.OrderBy(s => s.Cust_No);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "cust_no_desc":
                    tempList = tempList.OrderByDescending(s => s.Cust_No);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "cust_creatr":
                    tempList = tempList.OrderBy(s => s.Cust_Creator_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "cust_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Cust_Creator_Name);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "cust_approvr":
                    tempList = tempList.OrderBy(s => s.Cust_Approver_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "vendor_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Cust_Approver_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "cust_last_updte":
                    tempList = tempList.OrderBy(s => s.Cust_Last_Updated);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "cust_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.Cust_Last_Updated);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "name":
                    tempList = tempList.OrderBy(s => s.Cust_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.Cust_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.Cust_Status == "For Approval" || x.Cust_Status == "For Approval (For Deletion)");
                    viewData = "glyph-6";
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
        public SortViewModel SortData(List<DMBCSViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "bcs_stat":
                    tempList = tempList.OrderBy(x => x.BCS_Status == "For Approval" || x.BCS_Status == "For Approval (For Deletion)");
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "bcs_tin":
                    tempList = tempList.OrderBy(s => s.BCS_TIN);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "bcs_tin_desc":
                    tempList = tempList.OrderByDescending(s => s.BCS_TIN);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "bcs_pos":
                    tempList = tempList.OrderBy(s => s.BCS_Position);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "bcs_pos_desc":
                    tempList = tempList.OrderByDescending(s => s.BCS_Position);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "bcs_sign":
                    tempList = tempList.OrderBy(s => s.BCS_Signatures);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "bcs_sign_desc":
                    tempList = tempList.OrderByDescending(s => s.BCS_Signatures);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "bcs_creatr":
                    tempList = tempList.OrderBy(s => s.BCS_Creator_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "bcs_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.BCS_Creator_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "bcs_approvr":
                    tempList = tempList.OrderBy(s => s.BCS_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "bcs_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.BCS_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "bcs_last_updte":
                    tempList = tempList.OrderBy(s => s.BCS_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "bcs_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.BCS_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "name":
                    tempList = tempList.OrderBy(s => s.BCS_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.BCS_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.BCS_Status == "For Approval" || x.BCS_Status == "For Approval (For Deletion)");
                    viewData = "glyph-8";
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
        public SortViewModel SortData(List<HomeNotifViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "notif_stat":
                    tempList = tempList.OrderBy(x => x.Notif_Type_Status == "For Approval");
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "notif_app_id":
                    tempList = tempList.OrderBy(s => s.Notif_Application_ID);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "notif_app_id_desc":
                    tempList = tempList.OrderByDescending(s => s.Notif_Application_ID);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "notif_message":
                    tempList = tempList.OrderBy(s => s.Notif_Message);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "notif_message_desc":
                    tempList = tempList.OrderByDescending(s => s.Notif_Message);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "notif_approvr":
                    tempList = tempList.OrderBy(s => s.Notif_Verifier_Approver);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "notif_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Notif_Verifier_Approver);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "notif_last_updte":
                    tempList = tempList.OrderBy(s => s.Notif_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "notif_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.Notif_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => x.Notif_Type_Status == "For Approval");
                    viewData = "glyph-6";
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
