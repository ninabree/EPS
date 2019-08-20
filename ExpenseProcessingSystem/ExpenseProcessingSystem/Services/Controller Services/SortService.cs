using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Controller_Services
{
    public class SortService
    {
        int[] statusIDs = { 7, 8, 9 };
        public SortViewModel SortData(IEnumerable<BMViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "acc_mapping":
                    tempList = tempList.OrderBy(s => s.BM_Acc_Group_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_mapping_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Acc_Group_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_name":
                    tempList = tempList.OrderBy(s => s.BM_Acc_Name);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_name_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Acc_Name);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "gbase_budget_code":
                    tempList = tempList.OrderBy(s => s.BM_GBase_Code);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "gbase_budget_code_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_GBase_Code);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_num":
                    tempList = tempList.OrderBy(s => s.BM_Acc_Num);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_num_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Acc_Num);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "budget":
                    tempList = tempList.OrderBy(s => s.BM_Budget_Current);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "budget_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Budget_Current);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "budget_new":
                    tempList = tempList.OrderBy(s => s.BM_Budget_Amount);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "budget_new_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Budget_Amount);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "gwrite_status":
                    tempList = tempList.OrderBy(s => s.BM_GWrite_Status);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "gwrite_status_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_GWrite_Status);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "date_registered":
                    tempList = tempList.OrderBy(s => s.BM_Date_Registered);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "date_registered_desc":
                    tempList = tempList.OrderByDescending(s => s.BM_Date_Registered);
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
        public SortViewModel SortData(List<DMVendorViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "vendor_stat":
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.Vendor_Status_ID));
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
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.Vendor_Status_ID));
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
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.Check_Status_ID));
                    viewData = "glyph-8";
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
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Creator_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "chk_approvr":
                    tempList = tempList.OrderBy(s => s.Check_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Approver_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "chk_last_updte":
                    tempList = tempList.OrderBy(s => s.Check_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "chk_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.Check_Last_Updated);
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
                    tempList = tempList.OrderByDescending(x=> statusIDs.Contains(x.Check_Status_ID));
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
        public SortViewModel SortData(List<DMAccountViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "acc_stat":
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.Account_Status_ID));
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
                case "acc_budget_code":
                    tempList = tempList.OrderBy(s => s.Account_Budget_Code);
                    viewData = "glyph-14";
                    vdInfo = "glyphicon-menu-down";
                    break; 
                case "acc_budget_code_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Budget_Code);
                    viewData = "glyph-14";
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
                case "acc_grp":
                    tempList = tempList.OrderBy(s => s.Account_Group_Name);
                    viewData = "glyph-12";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_grp_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Group_Name);
                    viewData = "glyph-12";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "acc_curr":
                    tempList = tempList.OrderBy(s => s.Account_Currency_Name);
                    viewData = "glyph-13";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "acc_curr_desc":
                    tempList = tempList.OrderByDescending(s => s.Account_Currency_Name);
                    viewData = "glyph-13";
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
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.Account_Status_ID));
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
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.Dept_Status_ID));
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
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.Dept_Status_ID));
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
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.VAT_Status_ID));
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
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.VAT_Status_ID));
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
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.FBT_Status_ID));
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
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.FBT_Status_ID));
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
                case "tr_stat":
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.TR_Status_ID));
                    viewData = "glyph-9";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tr_tax":
                    tempList = tempList.OrderBy(s => s.TR_Tax_Rate);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tr_tax_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Tax_Rate);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tr_atc":
                    tempList = tempList.OrderBy(s => s.TR_ATC);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tr_atc_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_ATC);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tr_income":
                    tempList = tempList.OrderBy(s => s.TR_Nature_Income_Payment);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tr_income_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Nature_Income_Payment);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "nature":
                    tempList = tempList.OrderBy(s => s.TR_Nature);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "nature_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Nature);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tr_creatr":
                    tempList = tempList.OrderBy(s => s.TR_Creator_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tr_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Creator_Name);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tr_approvr":
                    tempList = tempList.OrderBy(s => s.TR_Approver_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tr_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Approver_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tr_last_updte":
                    tempList = tempList.OrderBy(s => s.TR_Last_Updated);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tr_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_Last_Updated);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tr_title":
                    tempList = tempList.OrderBy(s => s.TR_WT_Title);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tr_title_desc":
                    tempList = tempList.OrderByDescending(s => s.TR_WT_Title);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.TR_Status_ID));
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
        public SortViewModel SortData(List<DMCurrencyViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "curr_stat":
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.Curr_Status_ID));
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
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.Curr_Status_ID));
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
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.Emp_Status_ID));
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
                case "regemp_last_updte_desc":
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
                case "regemp_cat":
                    tempList = tempList.OrderBy(s => s.Emp_Category_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "regemp_cat_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Category_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "regemp_fbt":
                    tempList = tempList.OrderBy(s => s.Emp_FBT_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "regemp_fbt_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_FBT_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.Emp_Status_ID));
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
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.Emp_Status_ID));
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
                case "tempemp_last_updte_desc":
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
                case "tempemp_cat":
                    tempList = tempList.OrderBy(s => s.Emp_Category_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tempemp_cat_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_Category_Name);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "tempemp_fbt":
                    tempList = tempList.OrderBy(s => s.Emp_FBT_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "tempemp_fbt_desc":
                    tempList = tempList.OrderByDescending(s => s.Emp_FBT_Name);
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.Emp_Status_ID));
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
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.Cust_Status_ID));
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
                case "cust_approvr_desc":
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
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.Cust_Status_ID));
                    viewData = "glyph-7";
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
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.BCS_Status_ID));
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
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.BCS_Status_ID));
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
                case "notif_type_status":
                    tempList = tempList.OrderByDescending(x => x.Notif_Status_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "notif_type":
                    tempList = tempList.OrderByDescending(s => s.Notif_Application_Type_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "notif_type_desc":
                    tempList = tempList.OrderBy(s => s.Notif_Application_Type_Name);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "notif_message":
                    tempList = tempList.OrderByDescending(s => s.Notif_Message);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "notif_message_desc":
                    tempList = tempList.OrderBy(s => s.Notif_Message);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "notif_mkr":
                    tempList = tempList.OrderBy(s => s.Notif_Application_Maker_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "notif_mkr_desc":
                    tempList = tempList.OrderByDescending(s => s.Notif_Application_Maker_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "notif_date":
                    tempList = tempList.OrderBy(s => s.Notif_Date);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "notif_type_status_desc":
                    tempList = tempList.OrderBy(x => x.Notif_Status_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                default:
                    tempList = tempList.OrderByDescending(s => s.Notif_Date);
                    viewData = "glyph-4";
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
        public SortViewModel SortData(List<DMAccountGroupViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "ag_stat":
                    tempList = tempList.OrderBy(x => statusIDs.Contains(x.AccountGroup_Status_ID));
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ag_code":
                    tempList = tempList.OrderBy(s => s.AccountGroup_Code);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ag_code_desc":
                    tempList = tempList.OrderByDescending(s => s.AccountGroup_Code);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ag_creatr":
                    tempList = tempList.OrderBy(s => s.AccountGroup_Creator_Name);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ag_creatr_desc":
                    tempList = tempList.OrderByDescending(s => s.AccountGroup_Creator_Name);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ag_approvr":
                    tempList = tempList.OrderBy(s => s.AccountGroup_Approver_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ag_approvr_desc":
                    tempList = tempList.OrderByDescending(s => s.AccountGroup_Approver_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "ag_last_updte":
                    tempList = tempList.OrderBy(s => s.AccountGroup_Last_Updated);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "ag_last_updte_desc":
                    tempList = tempList.OrderByDescending(s => s.AccountGroup_Last_Updated);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "name":
                    tempList = tempList.OrderBy(s => s.AccountGroup_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "name_desc":
                    tempList = tempList.OrderByDescending(s => s.AccountGroup_Name);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(x => statusIDs.Contains(x.AccountGroup_Status_ID));
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

        public SortViewModel SortData(List<AppHistoryViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "hist_voucher":
                    tempList = tempList.OrderBy(x => x.App_Voucher_No);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "hist_maker":
                    tempList = tempList.OrderBy(s => s.App_Maker_Name);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "hist_maker_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Maker_Name);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "hist_approver":
                    tempList = tempList.OrderBy(s => s.App_Approver_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "hist_approver_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Approver_Name);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "hist_date_created":
                    tempList = tempList.OrderBy(s => s.App_Date);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "hist_date_created_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Date);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "hist_last_updte":
                    tempList = tempList.OrderBy(s => s.App_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "hist_status":
                    tempList = tempList.OrderBy(s => s.App_Status);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "hist_status_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Status);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "hist_voucher_desc":
                    tempList = tempList.OrderByDescending(x => x.App_Voucher_No);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(s => s.App_Last_Updated);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
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
        public SortViewModel SortData(List<ApplicationsViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "pend_status":
                    tempList = tempList.OrderBy(x => x.App_Status == "PENDING");
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "pend_type":
                    tempList = tempList.OrderBy(s => s.App_Type);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "pend_type_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Type);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "pend_amt":
                    tempList = tempList.OrderBy(s => s.App_Amount);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "pend_amt_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Amount);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "pend_payee":
                    tempList = tempList.OrderBy(s => s.App_Payee);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "pend_payee_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Payee);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "pend_maker":
                    tempList = tempList.OrderBy(s => s.App_Maker);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "pend_maker_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Maker);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "pend_date_created":
                    tempList = tempList.OrderBy(s => s.App_Date);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "pend_date_created_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Date);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "pend_last_updte":
                    tempList = tempList.OrderBy(s => s.App_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "pend_status_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Status == "PENDING");
                    viewData = "glyph-8";
                    vdInfo = "glyphicon-menu-up";
                    break;
                default:
                    tempList = tempList.OrderByDescending(s => s.App_Last_Updated);
                    viewData = "glyph-7";
                    vdInfo = "glyphicon-menu-up";
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
        public SortViewModel SortData(IEnumerable<LiquidationMainListViewModel> list, string sortOrder)
        {
            var tempList = list.AsQueryable();
            var viewData = "";
            var vdInfo = "";
            switch (sortOrder)
            {
                case "app_type":
                    tempList = tempList.OrderBy(s => s.App_Type);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "app_type_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Type);
                    viewData = "glyph-1";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "amount":
                    tempList = tempList.OrderBy(s => s.App_Amount);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "amount_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Amount);
                    viewData = "glyph-2";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "maker":
                    tempList = tempList.OrderBy(s => s.App_Maker);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "maker_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Maker);
                    viewData = "glyph-3";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "approver":
                    tempList = tempList.OrderBy(s => s.App_Approver);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "approver_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Approver);
                    viewData = "glyph-4";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "date_submitted":
                    tempList = tempList.OrderBy(s => s.App_Date);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "date_submitted_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Date);
                    viewData = "glyph-5";
                    vdInfo = "glyphicon-menu-down";
                    break;
                case "last_updated":
                    tempList = tempList.OrderBy(s => s.App_Last_Updated);
                    viewData = "glyph-6";
                    vdInfo = "glyphicon-menu-up";
                    break;
                case "last_updated_desc":
                    tempList = tempList.OrderByDescending(s => s.App_Last_Updated);
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
