using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services
{
    public class HomeService
    {
        private readonly string defaultPW = "Mizuho2019";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        private ModelStateDictionary _modelState;
        public HomeService(IHttpContextAccessor httpContextAccessor, EPSDbContext context, ModelStateDictionary modelState)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _modelState = modelState;
        }

        public bool addUser(UserManagementViewModel model, string userId)
        {
            AccountModel mod = _context.Account.Where(x => model.NewAcc.Acc_UserID == x.Acc_UserID).FirstOrDefault();
            if (mod == null)
            {
                mod = new AccountModel
                {
                    Acc_UserName = model.NewAcc.Acc_UserName,
                    Acc_FName = model.NewAcc.Acc_FName,
                    Acc_LName = model.NewAcc.Acc_LName,
                    Acc_DeptID = model.NewAcc.Acc_DeptID,
                    Acc_Email = model.NewAcc.Acc_Email,
                    Acc_Role = model.NewAcc.Acc_Role,
                    Acc_Password = (CryptoTools.getHashPasswd("PLACEHOLDER", model.NewAcc.Acc_UserName, model.NewAcc.Acc_Password ?? defaultPW)),
                    Acc_Comment = model.NewAcc.Acc_Comment,
                    Acc_InUse = model.NewAcc.Acc_InUse,
                    Acc_Creator_ID = int.Parse(userId),
                    Acc_Created_Date = DateTime.Now,
                    Acc_Status = "Is Created"
                };
                if (_modelState.IsValid)
                {
                    _context.Account.Add(mod);
                    _context.SaveChanges();
                }
            }
            else
            {
                if (model.NewAcc.Acc_UserID == mod.Acc_UserID)
                {
                    mod.Acc_UserID = model.NewAcc.Acc_UserID;
                    mod.Acc_UserName = model.NewAcc.Acc_UserName;
                    mod.Acc_FName = model.NewAcc.Acc_FName;
                    mod.Acc_LName = model.NewAcc.Acc_LName;
                    mod.Acc_DeptID = model.NewAcc.Acc_DeptID;
                    mod.Acc_Email = model.NewAcc.Acc_Email;
                    mod.Acc_Role = model.NewAcc.Acc_Role;
                    mod.Acc_Password = model.NewAcc.Acc_Password != null ? (CryptoTools.getHashPasswd("PLACEHOLDER", model.NewAcc.Acc_UserName, model.NewAcc.Acc_Password)) :
                        model.NewAcc.Acc_UserName != null ? (CryptoTools.getHashPasswd("PLACEHOLDER", model.NewAcc.Acc_UserName, defaultPW)) : mod.Acc_Password;
                    mod.Acc_Comment = model.NewAcc.Acc_Comment;
                    mod.Acc_InUse = model.NewAcc.Acc_InUse;
                    mod.Acc_Approver_ID = int.Parse(userId);
                    mod.Acc_Last_Updated = DateTime.Now;
                    mod.Acc_Status = "Is Updated";
                    if (_modelState.IsValid)
                    {
                        _context.SaveChanges();
                    }
                }
            }
            return true;
        }
        //[DM]
        //Add
        public bool addPayee(NewPayeeListViewModel model, string userId)
        {
            List<DMPayeeModel> vmList = new List<DMPayeeModel>();
            foreach (NewPayeeViewModel dm in model.NewPayeeVM)
            {
                DMPayeeModel m = new DMPayeeModel
                {
                    Payee_Name = dm.Payee_Name,
                    Payee_TIN = dm.Payee_TIN,
                    Payee_Address = dm.Payee_Address,
                    Payee_Type = dm.Payee_Type,
                    Payee_No = dm.Payee_No,
                    Payee_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Payee_Created_Date = DateTime.Now,
                    Payee_Last_Updated = DateTime.Now,
                    Payee_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMPayee.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool addDept(NewDeptListViewModel model, string userId)
        {
            List<DMDeptModel> vmList = new List<DMDeptModel>();
            foreach (NewDeptViewModel dm in model.NewDeptVM)
            {
                DMDeptModel m = new DMDeptModel
                {
                    Dept_Name = dm.Dept_Name,
                    Dept_Code = dm.Dept_Code,
                    Dept_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Dept_Created_Date = DateTime.Now,
                    Dept_Last_Updated = DateTime.Now,
                    Dept_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMDept.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //Edit
        public bool editPayee(List<DMPayeeViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Payee_ID).ToList();
            List<DMPayeeModel> vmList = _context.DMPayee.Where(x => intList.Contains(x.Payee_ID)).ToList();

            vmList.ForEach(dm =>
            {
                model.ForEach(m =>
                {
                    if (m.Payee_ID == dm.Payee_ID)
                    {
                        dm.Payee_Name = m.Payee_Name;
                        dm.Payee_TIN = m.Payee_TIN;
                        dm.Payee_Address = m.Payee_Address;
                        dm.Payee_Type = m.Payee_Type;
                        dm.Payee_No = m.Payee_No;
                        dm.Payee_Approver_ID = int.Parse(_session.GetString("UserID"));
                        dm.Payee_Last_Updated = DateTime.Now;
                        dm.Payee_Status = "Is Updated";
                        dm.Payee_isDeleted = false;
                    }
                });
            });

            if (_modelState.IsValid)
            {
                _context.SaveChanges();
            }
            return true;
        }
        public bool editDept(List<DMDeptViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Dept_ID).ToList();
            List<DMDeptModel> vmList = _context.DMDept.Where(x => intList.Contains(x.Dept_ID)).ToList();

            vmList.ForEach(dm =>
            {
                model.ForEach(m =>
                {
                    if (m.Dept_ID == dm.Dept_ID)
                    {
                        dm.Dept_Name = m.Dept_Name;
                        dm.Dept_Code = m.Dept_Code;
                        dm.Dept_Approver_ID = int.Parse(_session.GetString("UserID"));
                        dm.Dept_Last_Updated = DateTime.Now;
                        dm.Dept_Status = "Is Updated";
                        dm.Dept_isDeleted = false;
                    }
                });
            });

            if (_modelState.IsValid)
            {
                _context.SaveChanges();
            }
            return true;
        }
        //Delete
        public bool deletePayee(List<DMPayeeViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Payee_ID).ToList();
            List<DMPayeeModel> vmList = _context.DMPayee.Where(x => intList.Contains(x.Payee_ID)).ToList();

            vmList.ForEach(dm =>
            {
                dm.Payee_Approver_ID = int.Parse(_session.GetString("UserID"));
                dm.Payee_Last_Updated = DateTime.Now;
                dm.Payee_Status = "Is Deleted";
                dm.Payee_isDeleted = true;
            });

            if (_modelState.IsValid)
            {
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteDept(List<DMDeptViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Dept_ID).ToList();
            List<DMDeptModel> vmList = _context.DMDept.Where(x => intList.Contains(x.Dept_ID)).ToList();

            vmList.ForEach(dm =>
            {
                dm.Dept_Approver_ID = int.Parse(_session.GetString("UserID"));
                dm.Dept_Last_Updated = DateTime.Now;
                dm.Dept_Status = "Is Deleted";
                dm.Dept_isDeleted = true;
            });

            if (_modelState.IsValid)
            {
                _context.SaveChanges();
            }
            return true;
        }
    }
}
