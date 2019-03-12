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
        public class SortViewModel
        {
            public List<dynamic> list { get; set; }
            public string viewData { get; set; }
            public string viewDataInfo { get; set; }
        }
    }
}
