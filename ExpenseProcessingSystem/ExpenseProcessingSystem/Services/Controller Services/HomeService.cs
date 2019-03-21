using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
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
        //[ PAYEE ]
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
        //---------------------------Currently Not In Use--------------------------------
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
                    Payee_Status = "Is Created"
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
                    Dept_Status = "Is Created"
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
            List<int> intList = model.Select(c => c.Payee_MasterID).ToList();
            List<DMPayeeModel> vmList = _context.DMPayee.Where(x => intList.Contains(x.Payee_MasterID) && x.Payee_isActive == true).ToList();

            vmList.ForEach(dm =>
            {
                model.ForEach(m =>
                {
                    if (m.Payee_MasterID == dm.Payee_ID)
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
            List<int> intList = model.Select(c => c.Payee_MasterID).ToList();
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
