using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Models.Pending;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.NewRecord;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public string getUserRole(string id)
        {
            var data = _context.Account.Where(x => x.Acc_UserID == int.Parse(id))
                .Select(x => x.Acc_Role).FirstOrDefault() ?? "";
            return data;
        }

        //Populate
        public UserManagementViewModel2 populateUM()
        {
            List<UserViewModel> vmList = new List<UserViewModel>();
            //get all accounts
            var accs = (from a in _context.Account
                        join d in _context.DMDept on a.Acc_DeptID equals d.Dept_ID
                        select new
                        {
                            a.Acc_UserID, a.Acc_UserName, a.Acc_FName, a.Acc_LName, d.Dept_Name, a.Acc_DeptID, a.Acc_Email, a.Acc_Role,
                            a.Acc_Comment, a.Acc_InUse, a.Acc_Creator_ID, a.Acc_Created_Date,a.Acc_Approver_ID, a.Acc_Last_Updated, a.Acc_Status
                        }).ToList();
            //get account creator/approver IDs and dates, not all accounts have this
            var creatr = (from a in accs
                         join c in _context.Account on a.Acc_Creator_ID equals c.Acc_UserID
                         let CreatorName = c.Acc_LName + ", " + c.Acc_FName
                         select new
                         { a.Acc_UserID, CreatorName }).ToList();
            var apprv = (from a in accs
                         join c in _context.Account on a.Acc_Approver_ID equals c.Acc_UserID
                         let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                         select new
                         { a.Acc_UserID, ApproverName }).ToList();

            accs.ForEach(x => {
                var creator = creatr.Where(a => a.Acc_UserID == x.Acc_UserID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprv.Where(a => a.Acc_UserID == x.Acc_UserID).Select(a => a.ApproverName).FirstOrDefault();
                UserViewModel vm = new UserViewModel
                {
                    Acc_UserID = x.Acc_UserID,
                    Acc_UserName = x.Acc_UserName,
                    Acc_FName = x.Acc_FName,
                    Acc_LName = x.Acc_LName,
                    Acc_Dept_ID = x.Acc_DeptID,
                    Acc_Dept_Name = x.Dept_Name,
                    Acc_Email = x.Acc_Email,
                    Acc_Role = x.Acc_Role,
                    Acc_InUse = x.Acc_InUse,
                    Acc_Comment = x.Acc_Comment,
                    Acc_Creator_Name = creator ?? "N/A",
                    Acc_Approver_Name = approver ?? "",
                    Acc_Created_Date = x.Acc_Created_Date,
                    Acc_Last_Updated =x.Acc_Last_Updated,
                    Acc_Status = x.Acc_Status
                };
                vmList.Add(vm);
            });
            List<DMDeptViewModel> deptList = new List<DMDeptViewModel>();

            DMDeptViewModel optionLbl = new DMDeptViewModel
            {
                Dept_ID = 0,
                Dept_Name = "--Select Department--",
                Dept_Code = "0000"
            };
            deptList.Add(optionLbl);

            _context.DMDept.Where(x => x.Dept_isDeleted == false).ToList().ForEach(x => {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_ID = x.Dept_ID,
                    Dept_Name = x.Dept_Name,
                    Dept_Code = x.Dept_Code
                };
                deptList.Add(vm);
            });

            UserManagementViewModel2 mod = new UserManagementViewModel2
            {
                NewAcc = new User2ViewModel(),
                AccList = vmList,
                DeptList = deptList
            };
            return mod;
        }
        //Add / Edit User
        public bool addUser(UserManagementViewModel model, string userId)
        {
            UserModel mod = _context.Account.Where(x => model.NewAcc.Acc_UserID == x.Acc_UserID).FirstOrDefault();
            if (mod == null)
            {
                mod = new UserModel
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
                    mod.Acc_FName = model.NewAcc.Acc_FName;
                    mod.Acc_LName = model.NewAcc.Acc_LName;
                    mod.Acc_DeptID = model.NewAcc.Acc_DeptID;
                    mod.Acc_Email = model.NewAcc.Acc_Email;
                    mod.Acc_Role = model.NewAcc.Acc_Role;
                    mod.Acc_Password = model.NewAcc.Acc_Password != null ? (CryptoTools.getHashPasswd("PLACEHOLDER", mod.Acc_UserName, model.NewAcc.Acc_Password)) : mod.Acc_Password;
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
        //---------------------------ADMIN---------------------------
        //[ PAYEE ]
        public bool approvePayee(List<DMPayeeViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Payee_MasterID).ToList();

            var allPending = (from pp in _context.DMPayee_Pending
                             from pm in _context.DMPayee.Where(x => x.Payee_MasterID == pp.Pending_Payee_MasterID).DefaultIfEmpty()
                             select new {pp.Pending_Payee_MasterID,
                                         pp.Pending_Payee_Name,
                                         pp.Pending_Payee_TIN,
                                         pp.Pending_Payee_Address,
                                         pp.Pending_Payee_Type,
                                         pp.Pending_Payee_No,
                                         pp.Pending_Payee_IsDeleted,
                                         pp.Pending_Payee_Creator_ID,
                                         pmCreatorID = pm.Payee_Creator_ID.ToString(),
                                         pmCreateDate = pm.Payee_Created_Date.ToString()
                             }).Where(x => intList.Contains(x.Pending_Payee_MasterID)).ToList();

            List<DMPayeeModel_Pending> toDelete = _context.DMPayee_Pending.Where(x=> intList.Contains(x.Pending_Payee_MasterID)).ToList();
            
            //get all records that currently exists in Master Data
            List<DMPayeeModel> vmList = _context.DMPayee.
                Where(x => intList.Contains(x.Payee_MasterID) && x.Payee_isActive == true).ToList();
            
            //list for formatted records to be added
            List<DMPayeeModel> addList = new List<DMPayeeModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMPayeeModel m = new DMPayeeModel
                {
                    Payee_Name = pending.Pending_Payee_Name,
                    Payee_MasterID = pending.Pending_Payee_MasterID,
                    Payee_TIN = pending.Pending_Payee_TIN,
                    Payee_Address = pending.Pending_Payee_Address,
                    Payee_Type = pending.Pending_Payee_Type,
                    Payee_No = pending.Pending_Payee_No,
                    Payee_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Payee_Creator_ID : int.Parse(pending.pmCreatorID),
                    Payee_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Payee_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Payee_Last_Updated = DateTime.Now,
                    Payee_Status = "Approved",
                    Payee_isDeleted = pending.Pending_Payee_IsDeleted,
                    Payee_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Payee_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMPayee.AddRange(addList);
                _context.DMPayee_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejPayee(List<DMPayeeViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Payee_MasterID).ToList();
            List<DMPayeeModel_Pending> allPending = _context.DMPayee_Pending.Where(x => intList.Contains(x.Pending_Payee_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMPayee_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ DEPARTMENT ]
        public bool approveDept(List<DMDeptViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Dept_MasterID).ToList();

            var allPending = ((from pp in _context.DMDept_Pending
                               from pm in _context.DMDept.Where(x => x.Dept_MasterID == pp.Pending_Dept_MasterID).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_Dept_MasterID,
                                   pp.Pending_Dept_Name,
                                   pp.Pending_Dept_Code,
                                   pp.Pending_Dept_isDeleted,
                                   pp.Pending_Dept_Creator_ID,
                                   pmCreatorID = pm.Dept_Creator_ID.ToString(),
                                   pmCreateDate = pm.Dept_Created_Date.ToString(),
                                   pp.Pending_Dept_isActive
                               })).Where(x=> intList.Contains(x.Pending_Dept_MasterID)).Distinct().ToList();

            List<DMDeptModel_Pending> toDelete = _context.DMDept_Pending.Where(x => intList.Contains(x.Pending_Dept_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMDeptModel> vmList = _context.DMDept.
                Where(x => intList.Contains(x.Dept_MasterID) && x.Dept_isActive == true).ToList();

            //list for formatted records to be added
            List<DMDeptModel> addList = new List<DMDeptModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMDeptModel m = new DMDeptModel
                {
                    Dept_Name = pending.Pending_Dept_Name,
                    Dept_MasterID = pending.Pending_Dept_MasterID,
                    Dept_Code = pending.Pending_Dept_Code,
                    Dept_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Dept_Creator_ID : int.Parse(pending.pmCreatorID),
                    Dept_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Dept_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Dept_Last_Updated = DateTime.Now,
                    Dept_Status = "Approved",
                    Dept_isDeleted = pending.Pending_Dept_isDeleted,
                    Dept_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Dept_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMDept.AddRange(addList);
                _context.DMDept_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejDept(List<DMDeptViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Dept_MasterID).ToList();
            List<DMDeptModel_Pending> allPending = _context.DMDept_Pending.Where(x => intList.Contains(x.Pending_Dept_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMDept_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ CHECK ]
        public bool approveCheck(List<DMCheckViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Check_MasterID).ToList();

            var allPending = ((from pp in _context.DMCheck_Pending
                               from pm in _context.DMCheck.Where(x => x.Check_MasterID == pp.Pending_Check_MasterID).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_Check_MasterID,
                                   pp.Pending_Check_Input_Date,
                                   pp.Pending_Check_Series_From,
                                   pp.Pending_Check_Series_To,
                                   pp.Pending_Check_Name,
                                   pp.Pending_Check_Type,
                                   pp.Pending_Check_isDeleted,
                                   pp.Pending_Check_Creator_ID,
                                   pmCreatorID = pm.Check_Creator_ID.ToString(),
                                   pmCreateDate = pm.Check_Created_Date.ToString(),
                                   pp.Pending_Check_isActive
                               })).Where(x => intList.Contains(x.Pending_Check_MasterID)).Distinct().ToList();

            List<DMCheckModel_Pending> toDelete = _context.DMCheck_Pending.Where(x => intList.Contains(x.Pending_Check_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMCheckModel> vmList = _context.DMCheck.
                Where(x => intList.Contains(x.Check_MasterID) && x.Check_isActive == true).ToList();

            //list for formatted records to be added
            List<DMCheckModel> addList = new List<DMCheckModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMCheckModel m = new DMCheckModel
                {
                    Check_Input_Date = pending.Pending_Check_Input_Date,
                    Check_MasterID = pending.Pending_Check_MasterID,
                    Check_Name = pending.Pending_Check_Name,
                    Check_Series_From = pending.Pending_Check_Series_From,
                    Check_Series_To = pending.Pending_Check_Series_To,
                    Check_Type = pending.Pending_Check_Type,
                    Check_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Check_Creator_ID : int.Parse(pending.pmCreatorID),
                    Check_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Check_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Check_Last_Updated = DateTime.Now,
                    Check_Status = "Approved",
                    Check_isDeleted = pending.Pending_Check_isDeleted,
                    Check_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Check_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMCheck.AddRange(addList);
                _context.DMCheck_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejCheck(List<DMCheckViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Check_MasterID).ToList();
            List<DMCheckModel_Pending> allPending = _context.DMCheck_Pending.Where(x => intList.Contains(x.Pending_Check_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMCheck_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ ACCOUNT ]
        public bool approveAccount(List<DMAccountViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Account_MasterID).ToList();

            var allPending = (from pp in _context.DMAccount_Pending
                              from pm in _context.DMAccount.Where(x => x.Account_MasterID == pp.Pending_Account_MasterID).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_Account_MasterID,
                                  pp.Pending_Account_Name,
                                  pp.Pending_Account_Code,
                                  pp.Pending_Account_No,
                                  pp.Pending_Account_Cust,
                                  pp.Pending_Account_Div,
                                  pp.Pending_Account_Fund,
                                  pp.Pending_Account_isDeleted,
                                  pp.Pending_Account_Creator_ID,
                                  pmCreatorID = pm.Account_Creator_ID.ToString(),
                                  pmCreateDate = pm.Account_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_Account_MasterID)).ToList();

            List<DMAccountModel_Pending> toDelete = _context.DMAccount_Pending.Where(x => intList.Contains(x.Pending_Account_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMAccountModel> vmList = _context.DMAccount.
                Where(x => intList.Contains(x.Account_MasterID) && x.Account_isActive == true).ToList();

            //list for formatted records to be added
            List<DMAccountModel> addList = new List<DMAccountModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMAccountModel m = new DMAccountModel
                {
                    Account_Name = pending.Pending_Account_Name,
                    Account_MasterID = pending.Pending_Account_MasterID,
                    Account_Code = pending.Pending_Account_Code,
                    Account_Cust = pending.Pending_Account_Cust,
                    Account_Div = pending.Pending_Account_Div,
                    Account_Fund = pending.Pending_Account_Fund,
                    Account_No = pending.Pending_Account_No,
                    Account_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Account_Creator_ID : int.Parse(pending.pmCreatorID),
                    Account_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Account_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Account_Last_Updated = DateTime.Now,
                    Account_Status = "Approved",
                    Account_isDeleted = pending.Pending_Account_isDeleted,
                    Account_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Account_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMAccount.AddRange(addList);
                _context.DMAccount_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejAccount(List<DMAccountViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Account_MasterID).ToList();
            List<DMAccountModel_Pending> allPending = _context.DMAccount_Pending.Where(x => intList.Contains(x.Pending_Account_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMAccount_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ VAT ]
        public bool approveVAT(List<DMVATViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.VAT_MasterID).ToList();

            var allPending = (from pp in _context.DMVAT_Pending
                              from pm in _context.DMVAT.Where(x => x.VAT_MasterID == pp.Pending_VAT_MasterID).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_VAT_MasterID,
                                  pp.Pending_VAT_Name,
                                  pp.Pending_VAT_Rate,
                                  pp.Pending_VAT_isDeleted,
                                  pp.Pending_VAT_Creator_ID,
                                  pmCreatorID = pm.VAT_Creator_ID.ToString(),
                                  pmCreateDate = pm.VAT_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_VAT_MasterID)).ToList();

            List<DMVATModel_Pending> toDelete = _context.DMVAT_Pending.Where(x => intList.Contains(x.Pending_VAT_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMVATModel> vmList = _context.DMVAT.
                Where(x => intList.Contains(x.VAT_MasterID) && x.VAT_isActive == true).ToList();

            //list for formatted records to be added
            List<DMVATModel> addList = new List<DMVATModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMVATModel m = new DMVATModel
                {
                    VAT_Name = pending.Pending_VAT_Name,
                    VAT_MasterID = pending.Pending_VAT_MasterID,
                    VAT_Rate = pending.Pending_VAT_Rate,
                    VAT_Creator_ID = pending.pmCreatorID == null ? pending.Pending_VAT_Creator_ID : int.Parse(pending.pmCreatorID),
                    VAT_Approver_ID = int.Parse(_session.GetString("UserID")),
                    VAT_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    VAT_Last_Updated = DateTime.Now,
                    VAT_Status = "Approved",
                    VAT_isDeleted = pending.Pending_VAT_isDeleted,
                    VAT_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.VAT_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMVAT.AddRange(addList);
                _context.DMVAT_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejVAT(List<DMVATViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.VAT_MasterID).ToList();
            List<DMVATModel_Pending> allPending = _context.DMVAT_Pending.Where(x => intList.Contains(x.Pending_VAT_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMVAT_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ FBT ]
        public bool approveFBT(List<DMFBTViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.FBT_MasterID).ToList();

            var allPending = (from pp in _context.DMFBT_Pending
                              from pm in _context.DMFBT.Where(x => x.FBT_MasterID == pp.Pending_FBT_MasterID).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_FBT_MasterID,
                                  pp.Pending_FBT_Name,
                                  pp.Pending_FBT_Account,
                                  pp.Pending_FBT_Formula,
                                  pp.Pending_FBT_Tax_Rate,
                                  pp.Pending_FBT_isDeleted,
                                  pp.Pending_FBT_Creator_ID,
                                  pmCreatorID = pm.FBT_Creator_ID.ToString(),
                                  pmCreateDate = pm.FBT_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_FBT_MasterID)).Distinct().ToList();

            List<DMFBTModel_Pending> toDelete = _context.DMFBT_Pending.Where(x => intList.Contains(x.Pending_FBT_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMFBTModel> vmList = _context.DMFBT.
                Where(x => intList.Contains(x.FBT_MasterID) && x.FBT_isActive == true).ToList();

            //list for formatted records to be added
            List<DMFBTModel> addList = new List<DMFBTModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMFBTModel m = new DMFBTModel
                {
                    FBT_Name = pending.Pending_FBT_Name,
                    FBT_MasterID = pending.Pending_FBT_MasterID,
                    FBT_Account = pending.Pending_FBT_Account,
                    FBT_Formula = pending.Pending_FBT_Formula,
                    FBT_Tax_Rate = pending.Pending_FBT_Tax_Rate,
                    FBT_Creator_ID = pending.pmCreatorID == null ? pending.Pending_FBT_Creator_ID : int.Parse(pending.pmCreatorID),
                    FBT_Approver_ID = int.Parse(_session.GetString("UserID")),
                    FBT_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    FBT_Last_Updated = DateTime.Now,
                    FBT_Status = "Approved",
                    FBT_isDeleted = pending.Pending_FBT_isDeleted,
                    FBT_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.FBT_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMFBT.AddRange(addList);
                _context.DMFBT_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejFBT(List<DMFBTViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.FBT_MasterID).ToList();
            List<DMFBTModel_Pending> allPending = _context.DMFBT_Pending.Where(x => intList.Contains(x.Pending_FBT_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMFBT_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ EWT ]
        public bool approveEWT(List<DMEWTViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.EWT_MasterID).ToList();

            var allPending = (from pp in _context.DMEWT_Pending
                              from pm in _context.DMEWT.Where(x => x.EWT_MasterID == pp.Pending_EWT_MasterID).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_EWT_MasterID,
                                  pp.Pending_EWT_Nature,
                                  pp.Pending_EWT_Tax_Rate,
                                  pp.Pending_EWT_ATC,
                                  pp.Pending_EWT_Tax_Rate_Desc,
                                  pp.Pending_EWT_isDeleted,
                                  pp.Pending_EWT_Creator_ID,
                                  pmCreatorID = pm.EWT_Creator_ID.ToString(),
                                  pmCreateDate = pm.EWT_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_EWT_MasterID)).Distinct().ToList();

            List<DMEWTModel_Pending> toDelete = _context.DMEWT_Pending.Where(x => intList.Contains(x.Pending_EWT_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMEWTModel> vmList = _context.DMEWT.
                Where(x => intList.Contains(x.EWT_MasterID) && x.EWT_isActive == true).ToList();

            //list for formatted records to be added
            List<DMEWTModel> addList = new List<DMEWTModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMEWTModel m = new DMEWTModel
                {
                    EWT_Nature = pending.Pending_EWT_Nature,
                    EWT_MasterID = pending.Pending_EWT_MasterID,
                    EWT_Tax_Rate = pending.Pending_EWT_Tax_Rate,
                    EWT_ATC = pending.Pending_EWT_ATC,
                    EWT_Tax_Rate_Desc = pending.Pending_EWT_Tax_Rate_Desc,
                    EWT_Creator_ID = pending.pmCreatorID == null ? pending.Pending_EWT_Creator_ID : int.Parse(pending.pmCreatorID),
                    EWT_Approver_ID = int.Parse(_session.GetString("UserID")),
                    EWT_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    EWT_Last_Updated = DateTime.Now,
                    EWT_Status = "Approved",
                    EWT_isDeleted = pending.Pending_EWT_isDeleted,
                    EWT_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.EWT_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMEWT.AddRange(addList);
                _context.DMEWT_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejEWT(List<DMEWTViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.EWT_MasterID).ToList();
            List<DMEWTModel_Pending> allPending = _context.DMEWT_Pending.Where(x => intList.Contains(x.Pending_EWT_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMEWT_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Curr ]
        public bool approveCurr(List<DMCurrencyViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Curr_MasterID).ToList();

            var allPending = (from pp in _context.DMCurrency_Pending
                              from pm in _context.DMCurrency.Where(x => x.Curr_MasterID == pp.Pending_Curr_MasterID).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_Curr_MasterID,
                                  pp.Pending_Curr_Name,
                                  pp.Pending_Curr_CCY_Code,
                                  pp.Pending_Curr_isDeleted,
                                  pp.Pending_Curr_Creator_ID,
                                  pmCreatorID = pm.Curr_Creator_ID.ToString(),
                                  pmCreateDate = pm.Curr_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_Curr_MasterID)).Distinct().ToList();

            List<DMCurrencyModel_Pending> toDelete = _context.DMCurrency_Pending.Where(x => intList.Contains(x.Pending_Curr_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMCurrencyModel> vmList = _context.DMCurrency.
                Where(x => intList.Contains(x.Curr_MasterID) && x.Curr_isActive == true).ToList();

            //list for formatted records to be added
            List<DMCurrencyModel> addList = new List<DMCurrencyModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMCurrencyModel m = new DMCurrencyModel
                {
                    Curr_Name = pending.Pending_Curr_Name,
                    Curr_MasterID = pending.Pending_Curr_MasterID,
                    Curr_CCY_Code = pending.Pending_Curr_CCY_Code,
                    Curr_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Curr_Creator_ID : int.Parse(pending.pmCreatorID),
                    Curr_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Curr_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Curr_Last_Updated = DateTime.Now,
                    Curr_Status = "Approved",
                    Curr_isDeleted = pending.Pending_Curr_isDeleted,
                    Curr_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Curr_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMCurrency.AddRange(addList);
                _context.DMCurrency_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejCurr(List<DMCurrencyViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Curr_MasterID).ToList();
            List<DMCurrencyModel_Pending> allPending = _context.DMCurrency_Pending.Where(x => intList.Contains(x.Pending_Curr_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMCurrency_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //------------------------------For Approval------------------------------
        //[ PAYEE ]
        public bool addPayee_Pending(NewPayeeListViewModel model, string userId)
        {
            List<DMPayeeModel_Pending> vmList = new List<DMPayeeModel_Pending>();

            var payeeMax = _context.DMPayee.Select(x => x.Payee_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMPayee_Pending.Select(x => x.Pending_Payee_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewPayeeViewModel dm in model.NewPayeeVM)
            {
                DMPayeeModel_Pending m = new DMPayeeModel_Pending
                {
                    Pending_Payee_Name = dm.Payee_Name,
                    Pending_Payee_MasterID = ++masterIDMax,
                    Pending_Payee_TIN = dm.Payee_TIN,
                    Pending_Payee_Address = dm.Payee_Address,
                    Pending_Payee_Type = dm.Payee_Type,
                    Pending_Payee_No = dm.Payee_No,
                    Pending_Payee_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Payee_Filed_Date = DateTime.Now,
                    Pending_Payee_IsDeleted = false,
                    Pending_Payee_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMPayee_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editPayee_Pending(List<DMPayeeViewModel> model, string userId)
        {
            List<DMPayeeModel_Pending> vmList = new List<DMPayeeModel_Pending>();
            foreach (DMPayeeViewModel dm in model)
            {
                DMPayeeModel_Pending m = new DMPayeeModel_Pending
                {
                    Pending_Payee_Name = dm.Payee_Name,
                    Pending_Payee_MasterID = dm.Payee_MasterID,
                    Pending_Payee_TIN = dm.Payee_TIN,
                    Pending_Payee_Address = dm.Payee_Address,
                    Pending_Payee_Type = dm.Payee_Type,
                    Pending_Payee_No = dm.Payee_No,
                    Pending_Payee_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Payee_Filed_Date = DateTime.Now,
                    Pending_Payee_IsDeleted = false,
                    Pending_Payee_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMPayee_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deletePayee_Pending(List<DMPayeeViewModel> model, string userId)
        {
            List<DMPayeeModel_Pending> vmList = new List<DMPayeeModel_Pending>();
            foreach (DMPayeeViewModel dm in model)
            {
                DMPayeeModel_Pending m = new DMPayeeModel_Pending
                {
                    Pending_Payee_Name = dm.Payee_Name,
                    Pending_Payee_MasterID = dm.Payee_MasterID,
                    Pending_Payee_TIN = dm.Payee_TIN,
                    Pending_Payee_Address = dm.Payee_Address,
                    Pending_Payee_Type = dm.Payee_Type,
                    Pending_Payee_No = dm.Payee_No,
                    Pending_Payee_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Payee_Filed_Date = DateTime.Now,
                    Pending_Payee_IsDeleted = true,
                    Pending_Payee_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMPayee_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ DEPARTMENT ]
        public bool addDept_Pending(NewDeptListViewModel model, string userId)
        {
            List<DMDeptModel_Pending> vmList = new List<DMDeptModel_Pending>();

            var deptMax = _context.DMDept.Select(x => x.Dept_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMDept_Pending.Select(x => x.Pending_Dept_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;

            foreach (NewDeptViewModel dm in model.NewDeptVM)
            {
                DMDeptModel_Pending m = new DMDeptModel_Pending
                {
                    Pending_Dept_Name = dm.Dept_Name,
                    Pending_Dept_MasterID = ++masterIDMax,
                    Pending_Dept_Code = dm.Dept_Code,
                    Pending_Dept_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Dept_Filed_Date = DateTime.Now,
                    Pending_Dept_isDeleted = false,
                    Pending_Dept_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMDept_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editDept_Pending(List<DMDeptViewModel> model, string userId)
        {
            List<DMDeptModel_Pending> vmList = new List<DMDeptModel_Pending>();
            var deptMax = _context.DMDept.Select(x => x.Dept_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMDept_Pending.Select(x => x.Pending_Dept_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMDeptViewModel dm in model)
            {
                DMDeptModel_Pending m = new DMDeptModel_Pending
                {
                    Pending_Dept_Name = dm.Dept_Name,
                    Pending_Dept_MasterID = dm.Dept_MasterID,
                    Pending_Dept_Code = dm.Dept_Code,
                    Pending_Dept_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Dept_Filed_Date = DateTime.Now,
                    Pending_Dept_isDeleted = false,
                    Pending_Dept_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMDept_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteDept_Pending(List<DMDeptViewModel> model, string userId)
        {
            List<DMDeptModel_Pending> vmList = new List<DMDeptModel_Pending>();
            var deptMax = _context.DMDept.Select(x => x.Dept_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMDept_Pending.Select(x => x.Pending_Dept_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMDeptViewModel dm in model)
            {
                DMDeptModel_Pending m = new DMDeptModel_Pending
                {
                    Pending_Dept_Name = dm.Dept_Name,
                    Pending_Dept_MasterID = dm.Dept_MasterID,
                    Pending_Dept_Code = dm.Dept_Code,
                    Pending_Dept_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Dept_Filed_Date = DateTime.Now,
                    Pending_Dept_isDeleted = true,
                    Pending_Dept_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMDept_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Check ]
        public bool addCheck_Pending(NewCheckListViewModel model, string userId)
        {
            List<DMCheckModel_Pending> vmList = new List<DMCheckModel_Pending>();

            var checkMax = _context.DMCheck.Select(x => x.Check_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCheck_Pending.Select(x => x.Pending_Check_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = checkMax > pendingMax ? checkMax : pendingMax;

            foreach (NewCheckViewModel dm in model.NewCheckVM)
            {
                DMCheckModel_Pending m = new DMCheckModel_Pending
                {
                    Pending_Check_Input_Date = dm.Check_Input_Date,
                    Pending_Check_MasterID = ++masterIDMax,
                    Pending_Check_Series_From = dm.Check_Series_From,
                    Pending_Check_Series_To = dm.Check_Series_To,
                    Pending_Check_Name = dm.Check_Name,
                    Pending_Check_Type = dm.Check_Type,
                    Pending_Check_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Check_Filed_Date = DateTime.Now,
                    Pending_Check_isDeleted = false,
                    Pending_Check_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCheck_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editCheck_Pending(List<DMCheckViewModel> model, string userId)
        {
            List<DMCheckModel_Pending> vmList = new List<DMCheckModel_Pending>();
            var checkMax = _context.DMCheck.Select(x => x.Check_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCheck_Pending.Select(x => x.Pending_Check_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = checkMax > pendingMax ? checkMax : pendingMax;
            foreach (DMCheckViewModel dm in model)
            {
                DMCheckModel_Pending m = new DMCheckModel_Pending
                {
                    Pending_Check_Input_Date = dm.Check_Input_Date,
                    Pending_Check_MasterID = dm.Check_MasterID,
                    Pending_Check_Series_From = dm.Check_Series_From,
                    Pending_Check_Series_To = dm.Check_Series_To,
                    Pending_Check_Name = dm.Check_Name,
                    Pending_Check_Type = dm.Check_Type,
                    Pending_Check_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Check_Filed_Date = DateTime.Now,
                    Pending_Check_isDeleted = false,
                    Pending_Check_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCheck_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteCheck_Pending(List<DMCheckViewModel> model, string userId)
        {
            List<DMCheckModel_Pending> vmList = new List<DMCheckModel_Pending>();
            var checkMax = _context.DMCheck.Select(x => x.Check_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCheck_Pending.Select(x => x.Pending_Check_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = checkMax > pendingMax ? checkMax : pendingMax;
            foreach (DMCheckViewModel dm in model)
            {
                DMCheckModel_Pending m = new DMCheckModel_Pending
                {
                    Pending_Check_Input_Date = dm.Check_Input_Date,
                    Pending_Check_MasterID = dm.Check_MasterID,
                    Pending_Check_Series_From = dm.Check_Series_From,
                    Pending_Check_Series_To = dm.Check_Series_To,
                    Pending_Check_Name = dm.Check_Name,
                    Pending_Check_Type = dm.Check_Type,
                    Pending_Check_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Check_Filed_Date = DateTime.Now,
                    Pending_Check_isDeleted = true,
                    Pending_Check_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCheck_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ ACCOUNT ]
        public bool addAccount_Pending(NewAccountListViewModel model, string userId)
        {
            List<DMAccountModel_Pending> vmList = new List<DMAccountModel_Pending>();

            var payeeMax = _context.DMAccount.Select(x => x.Account_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMAccount_Pending.Select(x => x.Pending_Account_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewAccountViewModel dm in model.NewAccountVM)
            {
                DMAccountModel_Pending m = new DMAccountModel_Pending
                {
                    Pending_Account_Name = dm.Account_Name,
                    Pending_Account_MasterID = ++masterIDMax,
                    Pending_Account_Code = dm.Account_Code,
                    Pending_Account_Cust = dm.Account_Cust,
                    Pending_Account_Div = dm.Account_Div,
                    Pending_Account_Fund = dm.Account_Fund,
                    Pending_Account_No = dm.Account_No,
                    Pending_Account_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Account_Filed_Date = DateTime.Now,
                    Pending_Account_isDeleted = false,
                    Pending_Account_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccount_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editAccount_Pending(List<DMAccountViewModel> model, string userId)
        {
            List<DMAccountModel_Pending> vmList = new List<DMAccountModel_Pending>();
            foreach (DMAccountViewModel dm in model)
            {
                DMAccountModel_Pending m = new DMAccountModel_Pending
                {
                    Pending_Account_Name = dm.Account_Name,
                    Pending_Account_MasterID = dm.Account_MasterID,
                    Pending_Account_Code = dm.Account_Code,
                    Pending_Account_Cust = dm.Account_Cust,
                    Pending_Account_Div = dm.Account_Div,
                    Pending_Account_Fund = dm.Account_Fund,
                    Pending_Account_No = dm.Account_No,
                    Pending_Account_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Account_Filed_Date = DateTime.Now,
                    Pending_Account_isDeleted = false,
                    Pending_Account_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccount_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteAccount_Pending(List<DMAccountViewModel> model, string userId)
        {
            List<DMAccountModel_Pending> vmList = new List<DMAccountModel_Pending>();
            foreach (DMAccountViewModel dm in model)
            {
                DMAccountModel_Pending m = new DMAccountModel_Pending
                {
                    Pending_Account_Name = dm.Account_Name,
                    Pending_Account_MasterID = dm.Account_MasterID,
                    Pending_Account_Code = dm.Account_Code,
                    Pending_Account_Cust = dm.Account_Cust,
                    Pending_Account_Div = dm.Account_Div,
                    Pending_Account_Fund = dm.Account_Fund,
                    Pending_Account_No = dm.Account_No,
                    Pending_Account_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Account_Filed_Date = DateTime.Now,
                    Pending_Account_isDeleted = true,
                    Pending_Account_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccount_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ VAT ]
        public bool addVAT_Pending(NewVATListViewModel model, string userId)
        {
            List<DMVATModel_Pending> vmList = new List<DMVATModel_Pending>();

            var VATMax = _context.DMVAT.Select(x => x.VAT_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMVAT_Pending.Select(x => x.Pending_VAT_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = VATMax > pendingMax ? VATMax : pendingMax;

            foreach (NewVATViewModel dm in model.NewVATVM)
            {
                DMVATModel_Pending m = new DMVATModel_Pending
                {
                    Pending_VAT_Name = dm.VAT_Name,
                    Pending_VAT_MasterID = ++masterIDMax,
                    Pending_VAT_Rate = dm.VAT_Rate,
                    Pending_VAT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_VAT_Filed_Date = DateTime.Now,
                    Pending_VAT_isDeleted = false,
                    Pending_VAT_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMVAT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editVAT_Pending(List<DMVATViewModel> model, string userId)
        {
            List<DMVATModel_Pending> vmList = new List<DMVATModel_Pending>();
            foreach (DMVATViewModel dm in model)
            {
                DMVATModel_Pending m = new DMVATModel_Pending
                {
                    Pending_VAT_Name = dm.VAT_Name,
                    Pending_VAT_MasterID = dm.VAT_MasterID,
                    Pending_VAT_Rate = dm.VAT_Rate,
                    Pending_VAT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_VAT_Filed_Date = DateTime.Now,
                    Pending_VAT_isDeleted = false,
                    Pending_VAT_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMVAT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteVAT_Pending(List<DMVATViewModel> model, string userId)
        {
            List<DMVATModel_Pending> vmList = new List<DMVATModel_Pending>();
            foreach (DMVATViewModel dm in model)
            {
                DMVATModel_Pending m = new DMVATModel_Pending
                {
                    Pending_VAT_Name = dm.VAT_Name,
                    Pending_VAT_MasterID = dm.VAT_MasterID,
                    Pending_VAT_Rate = dm.VAT_Rate,
                    Pending_VAT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_VAT_Filed_Date = DateTime.Now,
                    Pending_VAT_isDeleted = true,
                    Pending_VAT_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMVAT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ FBT ]
        public bool addFBT_Pending(NewFBTListViewModel model, string userId)
        {
            List<DMFBTModel_Pending> vmList = new List<DMFBTModel_Pending>();

            var payeeMax = _context.DMFBT.Select(x => x.FBT_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMFBT_Pending.Select(x => x.Pending_FBT_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewFBTViewModel dm in model.NewFBTVM)
            {
                DMFBTModel_Pending m = new DMFBTModel_Pending
                {
                    Pending_FBT_Name = dm.FBT_Name,
                    Pending_FBT_MasterID = ++masterIDMax,
                    Pending_FBT_Account = dm.FBT_Account,
                    Pending_FBT_Formula = dm.FBT_Formula,
                    Pending_FBT_Tax_Rate = dm.FBT_Tax_Rate,
                    Pending_FBT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_FBT_Filed_Date = DateTime.Now,
                    Pending_FBT_isDeleted = false,
                    Pending_FBT_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMFBT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editFBT_Pending(List<DMFBTViewModel> model, string userId)
        {
            List<DMFBTModel_Pending> vmList = new List<DMFBTModel_Pending>();
            foreach (DMFBTViewModel dm in model)
            {
                DMFBTModel_Pending m = new DMFBTModel_Pending
                {
                    Pending_FBT_Name = dm.FBT_Name,
                    Pending_FBT_MasterID = dm.FBT_MasterID,
                    Pending_FBT_Account = dm.FBT_Account,
                    Pending_FBT_Formula = dm.FBT_Formula,
                    Pending_FBT_Tax_Rate = dm.FBT_Tax_Rate,
                    Pending_FBT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_FBT_Filed_Date = DateTime.Now,
                    Pending_FBT_isDeleted = false,
                    Pending_FBT_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMFBT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteFBT_Pending(List<DMFBTViewModel> model, string userId)
        {
            List<DMFBTModel_Pending> vmList = new List<DMFBTModel_Pending>();
            foreach (DMFBTViewModel dm in model)
            {
                DMFBTModel_Pending m = new DMFBTModel_Pending
                {
                    Pending_FBT_Name = dm.FBT_Name,
                    Pending_FBT_MasterID = dm.FBT_MasterID,
                    Pending_FBT_Account = dm.FBT_Account,
                    Pending_FBT_Formula = dm.FBT_Formula,
                    Pending_FBT_Tax_Rate = dm.FBT_Tax_Rate,
                    Pending_FBT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_FBT_Filed_Date = DateTime.Now,
                    Pending_FBT_isDeleted = true,
                    Pending_FBT_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMFBT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ EWT ]
        public bool addEWT_Pending(NewEWTListViewModel model, string userId)
        {
            List<DMEWTModel_Pending> vmList = new List<DMEWTModel_Pending>();

            var payeeMax = _context.DMEWT.Select(x => x.EWT_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMEWT_Pending.Select(x => x.Pending_EWT_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewEWTViewModel dm in model.NewEWTVM)
            {
                DMEWTModel_Pending m = new DMEWTModel_Pending
                {
                    Pending_EWT_Nature = dm.EWT_Nature,
                    Pending_EWT_MasterID = ++masterIDMax,
                    Pending_EWT_Tax_Rate = dm.EWT_Tax_Rate,
                    Pending_EWT_ATC = dm.EWT_ATC,
                    Pending_EWT_Tax_Rate_Desc = dm.EWT_Tax_Rate_Desc,
                    Pending_EWT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_EWT_Filed_Date = DateTime.Now,
                    Pending_EWT_isDeleted = false,
                    Pending_EWT_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMEWT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editEWT_Pending(List<DMEWTViewModel> model, string userId)
        {
            List<DMEWTModel_Pending> vmList = new List<DMEWTModel_Pending>();
            foreach (DMEWTViewModel dm in model)
            {
                DMEWTModel_Pending m = new DMEWTModel_Pending
                {
                    Pending_EWT_Nature = dm.EWT_Nature,
                    Pending_EWT_MasterID = dm.EWT_MasterID,
                    Pending_EWT_Tax_Rate = dm.EWT_Tax_Rate,
                    Pending_EWT_ATC = dm.EWT_ATC,
                    Pending_EWT_Tax_Rate_Desc = dm.EWT_Tax_Rate_Desc,
                    Pending_EWT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_EWT_Filed_Date = DateTime.Now,
                    Pending_EWT_isDeleted = false,
                    Pending_EWT_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMEWT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteEWT_Pending(List<DMEWTViewModel> model, string userId)
        {
            List<DMEWTModel_Pending> vmList = new List<DMEWTModel_Pending>();
            foreach (DMEWTViewModel dm in model)
            {
                DMEWTModel_Pending m = new DMEWTModel_Pending
                {
                    Pending_EWT_Nature = dm.EWT_Nature,
                    Pending_EWT_MasterID = dm.EWT_MasterID,
                    Pending_EWT_Tax_Rate = dm.EWT_Tax_Rate,
                    Pending_EWT_ATC = dm.EWT_ATC,
                    Pending_EWT_Tax_Rate_Desc = dm.EWT_Tax_Rate_Desc,
                    Pending_EWT_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_EWT_Filed_Date = DateTime.Now,
                    Pending_EWT_isDeleted = true,
                    Pending_EWT_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMEWT_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Curr ]
        public bool addCurr_Pending(NewCurrListViewModel model, string userId)
        {
            List<DMCurrencyModel_Pending> vmList = new List<DMCurrencyModel_Pending>();

            var payeeMax = _context.DMCurrency.Select(x => x.Curr_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCurrency_Pending.Select(x => x.Pending_Curr_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewCurrViewModel dm in model.NewCurrVM)
            {
                DMCurrencyModel_Pending m = new DMCurrencyModel_Pending
                {
                    Pending_Curr_Name = dm.Curr_Name,
                    Pending_Curr_MasterID = ++masterIDMax,
                    Pending_Curr_CCY_Code = dm.Curr_CCY_Code,
                    Pending_Curr_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Curr_Filed_Date = DateTime.Now,
                    Pending_Curr_isDeleted = false,
                    Pending_Curr_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCurrency_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editCurr_Pending(List<DMCurrencyViewModel> model, string userId)
        {
            List<DMCurrencyModel_Pending> vmList = new List<DMCurrencyModel_Pending>();
            foreach (DMCurrencyViewModel dm in model)
            {
                DMCurrencyModel_Pending m = new DMCurrencyModel_Pending
                {
                    Pending_Curr_Name = dm.Curr_Name,
                    Pending_Curr_MasterID = dm.Curr_MasterID,
                    Pending_Curr_CCY_Code = dm.Curr_CCY_Code,
                    Pending_Curr_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Curr_Filed_Date = DateTime.Now,
                    Pending_Curr_isDeleted = false,
                    Pending_Curr_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCurrency_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteCurr_Pending(List<DMCurrencyViewModel> model, string userId)
        {
            List<DMCurrencyModel_Pending> vmList = new List<DMCurrencyModel_Pending>();
            foreach (DMCurrencyViewModel dm in model)
            {
                DMCurrencyModel_Pending m = new DMCurrencyModel_Pending
                {
                    Pending_Curr_Name = dm.Curr_Name,
                    Pending_Curr_MasterID = dm.Curr_MasterID,
                    Pending_Curr_CCY_Code = dm.Curr_CCY_Code,
                    Pending_Curr_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Curr_Filed_Date = DateTime.Now,
                    Pending_Curr_isDeleted = true,
                    Pending_Curr_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCurrency_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }

        //MISC
        public List<BMViewModel> PopulateBM()
        {
            List<BMViewModel> bmvmList = new List<BMViewModel>();
            for (var i = 1; i <= 40; i++)
            {
                BMViewModel bmvm = new BMViewModel
                {
                    BM_Id = i,
                    BM_Creator_ID = i + 100,
                    BM_Approver_ID = i + 200,
                    BM_Account = "Account_" + i,
                    BM_Type = "Sample_Type_" + i,
                    BM_Budget = i + 100,
                    BM_Curr_Budget = i + 110,
                    BM_Last_Trans_Date = DateTime.Parse("1/12/2017", CultureInfo.GetCultureInfo("en-GB"))
                            .Add(DateTime.Now.TimeOfDay),
                    BM_Last_Budget_Approval = "Sample"
                };
                bmvmList.Add(bmvm);
            }
            return bmvmList;
        }
        //--------------------TEMP LOCATION-->MOVE TO ACCOUNT SERVICE-----------------------
        public bool sendEmail(ForgotPWViewModel model)
        {
            EmailService _email = new EmailService();
            var email = "monina.martinn@gmail.com";
            var subject = "EPS New PW";
            var hmtlMessage = "You will recieve new password. Choutto matte.";
            _email.SendEmailAsync(email, subject, hmtlMessage);
            return true;
        }
    }
}
