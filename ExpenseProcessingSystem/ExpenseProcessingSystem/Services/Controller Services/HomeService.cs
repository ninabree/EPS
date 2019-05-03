using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Models.Pending;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using ExpenseProcessingSystem.ViewModels.NewRecord;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace ExpenseProcessingSystem.Services
{
    public class HomeService
    {
        private readonly string defaultPW = "Mizuho2019";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IHostingEnvironment _hostingEnvironment;

        private ModelStateDictionary _modelState;
        public HomeService(IHttpContextAccessor httpContextAccessor, EPSDbContext context, ModelStateDictionary modelState, IHostingEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _modelState = modelState;
            _hostingEnvironment = hostingEnvironment;
        }
        public string getUserRole(string id)
        {
            var data = _context.User.Where(x => x.User_ID == int.Parse(id))
                .Select(x => x.User_Role).FirstOrDefault() ?? "";
            return data;
        }

        //-----------------------------------Populate-------------------------------------//
        //[ Home ]
        public List<HomeNotifViewModel> populateNotif(FiltersViewModel filters)
        {
            IQueryable<HomeNotifModel> mList = _context.HomeNotif.ToList().AsQueryable();
            PropertyInfo[] properties = filters.NotifFil.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string subStr = propertyName.Substring(propertyName.IndexOf('_') + 1);
                var toStr = property.GetValue(filters.NotifFil).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(subStr)) // IF INT VAL
                        {
                            mList = mList.Where("Notif_" + subStr + " = @0 AND  Notif_isDeleted == @1", property.GetValue(filters.NotifFil), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Last_Updated")
                        {
                            mList = mList
                               .Where(x => (x.Notif_Last_Updated.ToShortDateString() == property.GetValue(filters.NotifFil).ToString()))
                              .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Notif_" + subStr + ".Contains(@0) AND  Notif_isDeleted == @1", property.GetValue(filters.NotifFil).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }

            var creatorList = (from a in mList
                               join b in _context.User on a.Notif_Verifr_Apprvr_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new
                               { a.Notif_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.Notif_Verifr_Apprvr_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new
                              { a.Notif_ID, ApproverName }).ToList();

            //assign values
            List<HomeNotifModel> mList2 = mList.ToList();
            List<HomeNotifViewModel> vmList = new List<HomeNotifViewModel>();
            foreach (HomeNotifModel m in mList2)
            {
                var creator = creatorList.Where(a => a.Notif_ID == m.Notif_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Notif_ID == m.Notif_ID).Select(a => a.ApproverName).FirstOrDefault();
                HomeNotifViewModel vm = new HomeNotifViewModel
                {
                    Notif_Application_ID = m.Notif_Application_ID,
                    Notif_Message = m.Notif_Message,
                    Notif_Verifier_Approver = approver ?? "",
                    Notif_Type_Status = m.Notif_Type_Status,
                    Notif_Last_Updated = m.Notif_Last_Updated,
                    Notif_Status = m.Notif_Status.ToString()
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        //[ User Maintenance ]
        public UserManagementViewModel2 populateUM()
        {
            List<UserViewModel> vmList = new List<UserViewModel>();
            //get all accounts
            var accs = (from a in _context.User
                        join d in _context.DMDept on a.User_DeptID equals d.Dept_ID
                        select new
                        {
                            a.User_ID, a.User_UserName, a.User_FName, a.User_LName, d.Dept_Name, a.User_DeptID, a.User_Email, a.User_Role,
                            a.User_Comment, a.User_InUse, a.User_Creator_ID, a.User_Created_Date,a.User_Approver_ID, a.User_Last_Updated, a.User_Status
                        }).ToList();
            //get account creator/approver IDs and dates, not all accounts have this
            var creatr = (from a in accs
                         join c in _context.User on a.User_Creator_ID equals c.User_ID
                         let CreatorName = c.User_LName + ", " + c.User_FName
                         select new
                         { a.User_ID, CreatorName }).ToList();
            var apprv = (from a in accs
                         join c in _context.User on a.User_Approver_ID equals c.User_ID
                         let ApproverName = c.User_LName + ", " + c.User_FName
                         select new
                         { a.User_ID, ApproverName }).ToList();

            accs.ForEach(x => {
                var creator = creatr.Where(a => a.User_ID == x.User_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprv.Where(a => a.User_ID == x.User_ID).Select(a => a.ApproverName).FirstOrDefault();
                UserViewModel vm = new UserViewModel
                {
                    User_ID = x.User_ID,
                    User_UserName = x.User_UserName,
                    User_FName = x.User_FName,
                    User_LName = x.User_LName,
                    User_Dept_ID = x.User_DeptID,
                    User_Dept_Name = x.Dept_Name,
                    User_Email = x.User_Email,
                    User_Role = x.User_Role,
                    User_InUse = x.User_InUse,
                    User_Comment = x.User_Comment,
                    User_Creator_Name = creator ?? "N/A",
                    User_Approver_Name = approver ?? "",
                    User_Created_Date = x.User_Created_Date,
                    User_Last_Updated =x.User_Last_Updated,
                    User_Status = x.User_Status
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
            UserModel mod = _context.User.Where(x => model.NewAcc.User_ID == x.User_ID).FirstOrDefault();
            if (mod == null)
            {
                mod = new UserModel
                {
                    User_UserName = model.NewAcc.User_UserName,
                    User_FName = model.NewAcc.User_FName,
                    User_LName = model.NewAcc.User_LName,
                    User_DeptID = model.NewAcc.User_DeptID,
                    User_Email = model.NewAcc.User_Email,
                    User_Role = model.NewAcc.User_Role,
                    User_Password = (CryptoTools.getHashPasswd("PLACEHOLDER", model.NewAcc.User_UserName, model.NewAcc.User_Password ?? defaultPW)),
                    User_Comment = model.NewAcc.User_Comment,
                    User_InUse = model.NewAcc.User_InUse,
                    User_Creator_ID = int.Parse(userId),
                    User_Created_Date = DateTime.Now,
                    User_Status = "Is Created"
                };
                if (_modelState.IsValid)
                {
                    _context.User.Add(mod);
                    _context.SaveChanges();
                }
            }
            else
            {
                if (model.NewAcc.User_ID == mod.User_ID)
                {
                    mod.User_FName = model.NewAcc.User_FName;
                    mod.User_LName = model.NewAcc.User_LName;
                    mod.User_DeptID = model.NewAcc.User_DeptID;
                    mod.User_Email = model.NewAcc.User_Email;
                    mod.User_Role = model.NewAcc.User_Role;
                    mod.User_Password = model.NewAcc.User_Password != null ? (CryptoTools.getHashPasswd("PLACEHOLDER", mod.User_UserName, model.NewAcc.User_Password)) : mod.User_Password;
                    mod.User_Comment = model.NewAcc.User_Comment;
                    mod.User_InUse = model.NewAcc.User_InUse;
                    mod.User_Approver_ID = int.Parse(userId);
                    mod.User_Last_Updated = DateTime.Now;
                    mod.User_Status = "Is Updated";
                    if (_modelState.IsValid)
                    {
                        _context.SaveChanges();
                    }
                }
            }
            return true;
        }
         
        //---------------------------DM - ADMIN---------------------------
        //[ PAYEE ]
        public bool approveVendor(List<DMVendorViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Vendor_MasterID).ToList();

            var allPending = (from pp in _context.DMVendor_Pending
                             from pm in _context.DMVendor.Where(x => x.Vendor_MasterID == pp.Pending_Vendor_MasterID).DefaultIfEmpty()
                             select new {pp.Pending_Vendor_MasterID,
                                         pp.Pending_Vendor_Name,
                                         pp.Pending_Vendor_TIN,
                                         pp.Pending_Vendor_Address,
                                         pp.Pending_Vendor_IsDeleted,
                                         pp.Pending_Vendor_Creator_ID,
                                         pmCreatorID = pm.Vendor_Creator_ID.ToString(),
                                         pmCreateDate = pm.Vendor_Created_Date.ToString()
                             }).Where(x => intList.Contains(x.Pending_Vendor_MasterID)).ToList();

            List<DMVendorModel_Pending> toDelete = _context.DMVendor_Pending.Where(x=> intList.Contains(x.Pending_Vendor_MasterID)).ToList();
            
            //get all records that currently exists in Master Data
            List<DMVendorModel> vmList = _context.DMVendor.
                Where(x => intList.Contains(x.Vendor_MasterID) && x.Vendor_isActive == true).ToList();
            
            //list for formatted records to be added
            List<DMVendorModel> addList = new List<DMVendorModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMVendorModel m = new DMVendorModel
                {
                    Vendor_Name = pending.Pending_Vendor_Name,
                    Vendor_MasterID = pending.Pending_Vendor_MasterID,
                    Vendor_TIN = pending.Pending_Vendor_TIN,
                    Vendor_Address = pending.Pending_Vendor_Address,
                    Vendor_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Vendor_Creator_ID : int.Parse(pending.pmCreatorID),
                    Vendor_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Vendor_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Vendor_Last_Updated = DateTime.Now,
                    Vendor_Status = "Approved",
                    Vendor_isDeleted = pending.Pending_Vendor_IsDeleted,
                    Vendor_isActive = true
                };
                addList.Add(m);
            });
            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Vendor_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMVendor.AddRange(addList);
                _context.DMVendor_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejVendor(List<DMVendorViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Vendor_MasterID).ToList();
            List<DMVendorModel_Pending> allPending = _context.DMVendor_Pending.Where(x => intList.Contains(x.Pending_Vendor_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.RemoveRange(allPending);
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
                                   pp.Pending_Dept_Budget_Unit,
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
                    Dept_Budget_Unit = pending.Pending_Dept_Budget_Unit,
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
                                   pp.Pending_Check_Bank_Info,
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
                    Check_Bank_Info = pending.Pending_Check_Bank_Info,
                    Check_Series_From = pending.Pending_Check_Series_From,
                    Check_Series_To = pending.Pending_Check_Series_To,
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
                                  pp.Pending_Account_FBT_MasterID,
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
                    Account_FBT_MasterID = pending.Pending_Account_FBT_MasterID,
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
        //[ TR ]
        public bool approveTR(List<DMTRViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.TR_MasterID).ToList();

            var allPending = (from pp in _context.DMTR_Pending
                              from pm in _context.DMTR.Where(x => x.TR_MasterID == pp.Pending_TR_MasterID).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_TR_WT_Title,
                                  pp.Pending_TR_MasterID,
                                  pp.Pending_TR_Nature,
                                  pp.Pending_TR_Tax_Rate,
                                  pp.Pending_TR_ATC,
                                  pp.Pending_TR_Nature_Income_Payment,
                                  pp.Pending_TR_isDeleted,
                                  pp.Pending_TR_Creator_ID,
                                  pmCreatorID = pm.TR_Creator_ID.ToString(),
                                  pmCreateDate = pm.TR_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_TR_MasterID)).Distinct().ToList();

            List<DMTRModel_Pending> toDelete = _context.DMTR_Pending.Where(x => intList.Contains(x.Pending_TR_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMTRModel> vmList = _context.DMTR.
                Where(x => intList.Contains(x.TR_MasterID) && x.TR_isActive == true).ToList();

            //list for formatted records to be added
            List<DMTRModel> addList = new List<DMTRModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMTRModel m = new DMTRModel
                {
                    TR_WT_Title = pending.Pending_TR_WT_Title,
                    TR_Nature = pending.Pending_TR_Nature,
                    TR_MasterID = pending.Pending_TR_MasterID,
                    TR_Tax_Rate = pending.Pending_TR_Tax_Rate,
                    TR_ATC = pending.Pending_TR_ATC,
                    TR_Nature_Income_Payment = pending.Pending_TR_Nature_Income_Payment,
                    TR_Creator_ID = pending.pmCreatorID == null ? pending.Pending_TR_Creator_ID : int.Parse(pending.pmCreatorID),
                    TR_Approver_ID = int.Parse(_session.GetString("UserID")),
                    TR_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    TR_Last_Updated = DateTime.Now,
                    TR_Status = "Approved",
                    TR_isDeleted = pending.Pending_TR_isDeleted,
                    TR_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.TR_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMTR.AddRange(addList);
                _context.DMTR_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejTR(List<DMTRViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.TR_MasterID).ToList();
            List<DMTRModel_Pending> allPending = _context.DMTR_Pending.Where(x => intList.Contains(x.Pending_TR_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMTR_Pending.RemoveRange(allPending);
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
                                  pp.Pending_Curr_CCY_ABBR,
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
                    Curr_CCY_ABBR = pending.Pending_Curr_CCY_ABBR,
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
        //[ Employee ]
        public bool approveEmp(List<DMEmpViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Emp_MasterID).ToList();

            var allPending = ((from pp in _context.DMEmp_Pending
                               from pm in _context.DMEmp.Where(x => x.Emp_MasterID == pp.Pending_Emp_MasterID).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_Emp_MasterID,
                                   pp.Pending_Emp_Name,
                                   pp.Pending_Emp_Acc_No,
                                   pp.Pending_Emp_Type,
                                   pp.Pending_Emp_isDeleted,
                                   pp.Pending_Emp_Creator_ID,
                                   pmCreatorID = pm.Emp_Creator_ID.ToString(),
                                   pmCreateDate = pm.Emp_Created_Date.ToString(),
                                   pp.Pending_Emp_isActive
                               })).Where(x => intList.Contains(x.Pending_Emp_MasterID)).Distinct().ToList();

            List<DMEmpModel_Pending> toDelete = _context.DMEmp_Pending.Where(x => intList.Contains(x.Pending_Emp_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMEmpModel> vmList = _context.DMEmp.
                Where(x => intList.Contains(x.Emp_MasterID) && x.Emp_isActive == true).ToList();

            //list for formatted records to be added
            List<DMEmpModel> addList = new List<DMEmpModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMEmpModel m = new DMEmpModel
                {
                    Emp_Name = pending.Pending_Emp_Name,
                    Emp_MasterID = pending.Pending_Emp_MasterID,
                    Emp_Acc_No = pending.Pending_Emp_Acc_No,
                    Emp_Type = pending.Pending_Emp_Acc_No.Length <= 0 ? "Temporary" : "Regular",
                    Emp_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Emp_Creator_ID : int.Parse(pending.pmCreatorID),
                    Emp_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Emp_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Emp_Last_Updated = DateTime.Now,
                    Emp_Status = "Approved",
                    Emp_isDeleted = pending.Pending_Emp_isDeleted,
                    Emp_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Emp_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMEmp.AddRange(addList);
                _context.DMEmp_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejEmp(List<DMEmpViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Emp_MasterID).ToList();
            List<DMEmpModel_Pending> allPending = _context.DMEmp_Pending.Where(x => intList.Contains(x.Pending_Emp_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMEmp_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Customer ]
        public bool approveCust(List<DMCustViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Cust_MasterID).ToList();

            var allPending = ((from pp in _context.DMCust_Pending
                               from pm in _context.DMCust.Where(x => x.Cust_MasterID == pp.Pending_Cust_MasterID).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_Cust_MasterID,
                                   pp.Pending_Cust_Name,
                                   pp.Pending_Cust_Abbr,
                                   pp.Pending_Cust_No,
                                   pp.Pending_Cust_isDeleted,
                                   pp.Pending_Cust_Creator_ID,
                                   pmCreatorID = pm.Cust_Creator_ID.ToString(),
                                   pmCreateDate = pm.Cust_Created_Date.ToString(),
                                   pp.Pending_Cust_isActive
                               })).Where(x => intList.Contains(x.Pending_Cust_MasterID)).Distinct().ToList();

            List<DMCustModel_Pending> toDelete = _context.DMCust_Pending.Where(x => intList.Contains(x.Pending_Cust_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMCustModel> vmList = _context.DMCust.
                Where(x => intList.Contains(x.Cust_MasterID) && x.Cust_isActive == true).ToList();

            //list for formatted records to be added
            List<DMCustModel> addList = new List<DMCustModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMCustModel m = new DMCustModel
                {
                    Cust_Name = pending.Pending_Cust_Name,
                    Cust_MasterID = pending.Pending_Cust_MasterID,
                    Cust_Abbr = pending.Pending_Cust_Abbr,
                    Cust_No = pending.Pending_Cust_No,
                    Cust_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Cust_Creator_ID : int.Parse(pending.pmCreatorID),
                    Cust_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Cust_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Cust_Last_Updated = DateTime.Now,
                    Cust_Status = "Approved",
                    Cust_isDeleted = pending.Pending_Cust_isDeleted,
                    Cust_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.Cust_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMCust.AddRange(addList);
                _context.DMCust_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejCust(List<DMCustViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.Cust_MasterID).ToList();
            List<DMCustModel_Pending> allPending = _context.DMCust_Pending.Where(x => intList.Contains(x.Pending_Cust_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMCust_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //[ BIR Cert Signatory ]
        public bool approveBCS(List<DMBCSViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.BCS_MasterID).ToList();

            var allPending = ((from pp in _context.DMBCS_Pending
                               from pm in _context.DMBCS.Where(x => x.BCS_MasterID == pp.Pending_BCS_MasterID).DefaultIfEmpty()
                               select new
                               {
                                   pp.Pending_BCS_MasterID,
                                   pp.Pending_BCS_Name,
                                   pp.Pending_BCS_TIN,
                                   pp.Pending_BCS_Position,
                                   pp.Pending_BCS_Signatures,
                                   pp.Pending_BCS_isDeleted,
                                   pp.Pending_BCS_Creator_ID,
                                   pmCreatorID = pm.BCS_Creator_ID.ToString(),
                                   pmCreateDate = pm.BCS_Created_Date.ToString(),
                                   pp.Pending_BCS_isActive
                               })).Where(x => intList.Contains(x.Pending_BCS_MasterID)).Distinct().ToList();

            List<DMBIRCertSignModel_Pending> toDelete = _context.DMBCS_Pending.Where(x => intList.Contains(x.Pending_BCS_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMBIRCertSignModel> vmList = _context.DMBCS.
                Where(x => intList.Contains(x.BCS_MasterID) && x.BCS_isActive == true).ToList();

            //list for formatted records to be added
            List<DMBIRCertSignModel> addList = new List<DMBIRCertSignModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMBIRCertSignModel m = new DMBIRCertSignModel
                {
                    BCS_Name = pending.Pending_BCS_Name,
                    BCS_MasterID = pending.Pending_BCS_MasterID,
                    BCS_TIN = pending.Pending_BCS_TIN,
                    BCS_Position = pending.Pending_BCS_Position,
                    BCS_Signatures = pending.Pending_BCS_Signatures,
                    BCS_Creator_ID = pending.pmCreatorID == null ? pending.Pending_BCS_Creator_ID : int.Parse(pending.pmCreatorID),
                    BCS_Approver_ID = int.Parse(_session.GetString("UserID")),
                    BCS_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    BCS_Last_Updated = DateTime.Now,
                    BCS_Status = "Approved",
                    BCS_isDeleted = pending.Pending_BCS_isDeleted,
                    BCS_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.BCS_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMBCS.AddRange(addList);
                _context.DMBCS_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejBCS(List<DMBCSViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.BCS_MasterID).ToList();
            List<DMBIRCertSignModel_Pending> allPending = _context.DMBCS_Pending.Where(x => intList.Contains(x.Pending_BCS_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMBCS_Pending.RemoveRange(allPending);
                _context.SaveChanges();
            }
            return true;
        }
        //------------------------------For Approval------------------------------
        //[ PAYEE ]
        public bool addVendor_Pending(NewVendorListViewModel model, string userId)
        {
            List<DMVendorModel_Pending> vmList = new List<DMVendorModel_Pending>();

            var payeeMax = _context.DMVendor.Select(x => x.Vendor_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMVendor_Pending.Select(x => x.Pending_Vendor_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewVendorViewModel dm in model.NewVendorVM)
            {
                DMVendorModel_Pending m = new DMVendorModel_Pending
                {
                    Pending_Vendor_Name = dm.Vendor_Name,
                    Pending_Vendor_MasterID = ++masterIDMax,
                    Pending_Vendor_TIN = dm.Vendor_TIN,
                    Pending_Vendor_Address = dm.Vendor_Address,
                    Pending_Vendor_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Vendor_Filed_Date = DateTime.Now,
                    Pending_Vendor_IsDeleted = false,
                    Pending_Vendor_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editVendor_Pending(List<DMVendorViewModel> model, string userId)
        {
            List<DMVendorModel_Pending> vmList = new List<DMVendorModel_Pending>();
            foreach (DMVendorViewModel dm in model)
            {
                DMVendorModel_Pending m = new DMVendorModel_Pending
                {
                    Pending_Vendor_Name = dm.Vendor_Name,
                    Pending_Vendor_MasterID = dm.Vendor_MasterID,
                    Pending_Vendor_TIN = dm.Vendor_TIN,
                    Pending_Vendor_Address = dm.Vendor_Address,
                    Pending_Vendor_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Vendor_Filed_Date = DateTime.Now,
                    Pending_Vendor_IsDeleted = false,
                    Pending_Vendor_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteVendor_Pending(List<DMVendorViewModel> model, string userId)
        {
            List<DMVendorModel_Pending> vmList = new List<DMVendorModel_Pending>();
            foreach (DMVendorViewModel dm in model)
            {
                DMVendorModel_Pending m = new DMVendorModel_Pending
                {
                    Pending_Vendor_Name = dm.Vendor_Name,
                    Pending_Vendor_MasterID = dm.Vendor_MasterID,
                    Pending_Vendor_TIN = dm.Vendor_TIN,
                    Pending_Vendor_Address = dm.Vendor_Address,
                    Pending_Vendor_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Vendor_Filed_Date = DateTime.Now,
                    Pending_Vendor_IsDeleted = true,
                    Pending_Vendor_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.AddRange(vmList);
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
                    Pending_Dept_Budget_Unit = dm.Dept_Budget_Unit,
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
                    Pending_Dept_Budget_Unit = dm.Dept_Budget_Unit,
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
                    Pending_Dept_Budget_Unit = dm.Dept_Budget_Unit,
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
                    Pending_Check_Bank_Info = dm.Check_Bank_Info,
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
                    Pending_Check_Bank_Info = dm.Check_Bank_Info,
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
                    Pending_Check_Bank_Info = dm.Check_Bank_Info,
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
                    Pending_Account_FBT_MasterID = dm.Account_FBT_MasterID,
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
                    Pending_Account_FBT_MasterID = dm.Account_FBT_MasterID,
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
                    Pending_Account_FBT_MasterID = dm.Account_FBT_MasterID,
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
        //[ TR ]
        public bool addTR_Pending(NewTRListViewModel model, string userId)
        {
            List<DMTRModel_Pending> vmList = new List<DMTRModel_Pending>();

            var payeeMax = _context.DMTR.Select(x => x.TR_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMTR_Pending.Select(x => x.Pending_TR_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewTRViewModel dm in model.NewTRVM)
            {
                DMTRModel_Pending m = new DMTRModel_Pending
                {
                    Pending_TR_Nature = dm.TR_Nature,
                    Pending_TR_MasterID = ++masterIDMax,
                    Pending_TR_Tax_Rate = dm.TR_Tax_Rate,
                    Pending_TR_ATC = dm.TR_ATC,
                    Pending_TR_WT_Title = dm.TR_WT_Title,
                    Pending_TR_Nature_Income_Payment = dm.TR_Nature_Income_Payment,
                    Pending_TR_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_TR_Filed_Date = DateTime.Now,
                    Pending_TR_isDeleted = false,
                    Pending_TR_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMTR_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editTR_Pending(List<DMTRViewModel> model, string userId)
        {
            List<DMTRModel_Pending> vmList = new List<DMTRModel_Pending>();
            foreach (DMTRViewModel dm in model)
            {
                DMTRModel_Pending m = new DMTRModel_Pending
                {
                    Pending_TR_WT_Title = dm.TR_WT_Title,
                    Pending_TR_Nature = dm.TR_Nature,
                    Pending_TR_MasterID = dm.TR_MasterID,
                    Pending_TR_Tax_Rate = dm.TR_Tax_Rate,
                    Pending_TR_ATC = dm.TR_ATC,
                    Pending_TR_Nature_Income_Payment = dm.TR_Nature_Income_Payment,
                    Pending_TR_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_TR_Filed_Date = DateTime.Now,
                    Pending_TR_isDeleted = false,
                    Pending_TR_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMTR_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteTR_Pending(List<DMTRViewModel> model, string userId)
        {
            List<DMTRModel_Pending> vmList = new List<DMTRModel_Pending>();
            foreach (DMTRViewModel dm in model)
            {
                DMTRModel_Pending m = new DMTRModel_Pending
                {
                    Pending_TR_WT_Title = dm.TR_WT_Title,
                    Pending_TR_Nature = dm.TR_Nature,
                    Pending_TR_MasterID = dm.TR_MasterID,
                    Pending_TR_Tax_Rate = dm.TR_Tax_Rate,
                    Pending_TR_ATC = dm.TR_ATC,
                    Pending_TR_Nature_Income_Payment = dm.TR_Nature_Income_Payment,
                    Pending_TR_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_TR_Filed_Date = DateTime.Now,
                    Pending_TR_isDeleted = true,
                    Pending_TR_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMTR_Pending.AddRange(vmList);
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
                    Pending_Curr_CCY_ABBR = dm.Curr_CCY_ABBR,
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
                    Pending_Curr_CCY_ABBR = dm.Curr_CCY_ABBR,
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
                    Pending_Curr_CCY_ABBR = dm.Curr_CCY_ABBR,
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
        //[ Employee ]
        public bool addEmp_Pending(NewEmpListViewModel model, string userId)
        {
            List<DMEmpModel_Pending> vmList = new List<DMEmpModel_Pending>();
            var deptMax = _context.DMEmp.Select(x => x.Emp_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMEmp_Pending.Select(x => x.Pending_Emp_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;

            foreach (NewEmpViewModel dm in model.NewEmpVM)
            {
                var tempAccNo = dm.Emp_Acc_No ?? "";
                DMEmpModel_Pending m = new DMEmpModel_Pending
                {
                    Pending_Emp_Name = dm.Emp_Name,
                    Pending_Emp_MasterID = ++masterIDMax,
                    Pending_Emp_Acc_No = tempAccNo,
                    Pending_Emp_Type = tempAccNo.Length <= 0 ? "Temporary" : "Regular",
                    Pending_Emp_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Emp_Filed_Date = DateTime.Now,
                    Pending_Emp_isDeleted = false,
                    Pending_Emp_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMEmp_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editEmp_Pending(List<DMEmpViewModel> model, string userId)
        {
            List<DMEmpModel_Pending> vmList = new List<DMEmpModel_Pending>();
            var deptMax = _context.DMEmp.Select(x => x.Emp_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMEmp_Pending.Select(x => x.Pending_Emp_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMEmpViewModel dm in model)
            {
                var tempAccNo = dm.Emp_Acc_No ?? "";
                DMEmpModel_Pending m = new DMEmpModel_Pending
                {
                    Pending_Emp_Name = dm.Emp_Name,
                    Pending_Emp_MasterID = dm.Emp_MasterID,
                    Pending_Emp_Acc_No = tempAccNo,
                    Pending_Emp_Type = tempAccNo.Length <= 0 ? "Temporary" : "Regular",
                    Pending_Emp_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Emp_Filed_Date = DateTime.Now,
                    Pending_Emp_isDeleted = false,
                    Pending_Emp_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMEmp_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteEmp_Pending(List<DMEmpViewModel> model, string userId)
        {
            List<DMEmpModel_Pending> vmList = new List<DMEmpModel_Pending>();
            var deptMax = _context.DMEmp.Select(x => x.Emp_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMEmp_Pending.Select(x => x.Pending_Emp_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMEmpViewModel dm in model)
            {
                var tempAccNo = dm.Emp_Acc_No ?? "";
                DMEmpModel_Pending m = new DMEmpModel_Pending
                {
                    Pending_Emp_Name = dm.Emp_Name,
                    Pending_Emp_MasterID = dm.Emp_MasterID,
                    Pending_Emp_Acc_No = tempAccNo,
                    Pending_Emp_Type = tempAccNo.Length <= 0 ? "Temporary" : "Regular",
                    Pending_Emp_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Emp_Filed_Date = DateTime.Now,
                    Pending_Emp_isDeleted = true,
                    Pending_Emp_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMEmp_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ Customer ]
        public bool addCust_Pending(NewCustListViewModel model, string userId)
        {
            List<DMCustModel_Pending> vmList = new List<DMCustModel_Pending>();

            var deptMax = _context.DMCust.Select(x => x.Cust_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCust_Pending.Select(x => x.Pending_Cust_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;

            foreach (NewCustViewModel dm in model.NewCustVM)
            {
                DMCustModel_Pending m = new DMCustModel_Pending
                {
                    Pending_Cust_Name = dm.Cust_Name,
                    Pending_Cust_MasterID = ++masterIDMax,
                    Pending_Cust_Abbr = dm.Cust_Abbr,
                    Pending_Cust_No = dm.Cust_No,
                    Pending_Cust_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Cust_Filed_Date = DateTime.Now,
                    Pending_Cust_isDeleted = false,
                    Pending_Cust_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCust_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editCust_Pending(List<DMCustViewModel> model, string userId)
        {
            List<DMCustModel_Pending> vmList = new List<DMCustModel_Pending>();
            var deptMax = _context.DMCust.Select(x => x.Cust_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCust_Pending.Select(x => x.Pending_Cust_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMCustViewModel dm in model)
            {
                DMCustModel_Pending m = new DMCustModel_Pending
                {
                    Pending_Cust_Name = dm.Cust_Name,
                    Pending_Cust_MasterID = dm.Cust_MasterID,
                    Pending_Cust_Abbr = dm.Cust_Abbr,
                    Pending_Cust_No = dm.Cust_No,
                    Pending_Cust_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Cust_Filed_Date = DateTime.Now,
                    Pending_Cust_isDeleted = false,
                    Pending_Cust_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCust_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteCust_Pending(List<DMCustViewModel> model, string userId)
        {
            List<DMCustModel_Pending> vmList = new List<DMCustModel_Pending>();
            var deptMax = _context.DMCust.Select(x => x.Cust_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMCust_Pending.Select(x => x.Pending_Cust_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMCustViewModel dm in model)
            {
                DMCustModel_Pending m = new DMCustModel_Pending
                {
                    Pending_Cust_Name = dm.Cust_Name,
                    Pending_Cust_MasterID = dm.Cust_MasterID,
                    Pending_Cust_Abbr = dm.Cust_Abbr,
                    Pending_Cust_No = dm.Cust_No,
                    Pending_Cust_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Cust_Filed_Date = DateTime.Now,
                    Pending_Cust_isDeleted = true,
                    Pending_Cust_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMCust_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        //[ BIR Cert Signatory ]
        public bool addBCS_Pending(NewBCSViewModel model, string userId)
        {
            List<DMBIRCertSignModel_Pending> vmList = new List<DMBIRCertSignModel_Pending>();

            var deptMax = _context.DMBCS.Select(x => x.BCS_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMBCS_Pending.Select(x => x.Pending_BCS_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, _context.FileLocation.Where(x => x.FL_Type == "BCS").Select(x => x.FL_Location).FirstOrDefault());
            //save file to uploads folder
            FileService objFile = new FileService();
            string strFilePath = objFile.SaveFile(model.BCS_Signatures, uploads, model.BCS_Name);

            DMBIRCertSignModel_Pending m = new DMBIRCertSignModel_Pending
            {
                Pending_BCS_Name = model.BCS_Name,
                Pending_BCS_MasterID = ++masterIDMax,
                Pending_BCS_TIN = model.BCS_TIN,
                Pending_BCS_Position = model.BCS_Position,
                Pending_BCS_Signatures = strFilePath,
                Pending_BCS_Creator_ID = int.Parse(_session.GetString("UserID")),
                Pending_BCS_Filed_Date = DateTime.Now,
                Pending_BCS_isDeleted = false,
                Pending_BCS_Status = "For Approval"
            };
            vmList.Add(m);

            if (_modelState.IsValid)
            {
                _context.DMBCS_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editBCS_Pending(DMBCS2ViewModel model, string userId)
        {
            List<DMBIRCertSignModel_Pending> vmList = new List<DMBIRCertSignModel_Pending>();
            var deptMax = _context.DMBCS.Select(x => x.BCS_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMBCS_Pending.Select(x => x.Pending_BCS_MasterID).
                DefaultIfEmpty(0).Max();
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, _context.FileLocation.Where(x => x.FL_Type == "BCS").Select(x => x.FL_Location).FirstOrDefault());
            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            string strFilePath = _context.DMBCS.Where(x => x.BCS_MasterID == model.BCS_MasterID).Select(x => x.BCS_Signatures).FirstOrDefault();
            //if there is image uploaded
            if (model.BCS_Signatures != null)
            {

                FileService objFile = new FileService();
                strFilePath = objFile.SaveFile(model.BCS_Signatures, uploads, model.BCS_Name);
            }
            DMBIRCertSignModel_Pending m = new DMBIRCertSignModel_Pending
            {
                Pending_BCS_Name = model.BCS_Name,
                Pending_BCS_TIN = model.BCS_TIN,
                Pending_BCS_Position = model.BCS_Position,
                Pending_BCS_MasterID = model.BCS_MasterID,
                Pending_BCS_Signatures = strFilePath,
                Pending_BCS_Creator_ID = int.Parse(_session.GetString("UserID")),
                Pending_BCS_Filed_Date = DateTime.Now,
                Pending_BCS_isDeleted = false,
                Pending_BCS_Status = "For Approval"
            };
            vmList.Add(m);

            if (_modelState.IsValid)
            {
                _context.DMBCS_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteBCS_Pending(List<DMBCSViewModel> model, string userId)
        {
            List<DMBIRCertSignModel_Pending> vmList = new List<DMBIRCertSignModel_Pending>();
            var deptMax = _context.DMBCS.Select(x => x.BCS_MasterID).
               DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMBCS_Pending.Select(x => x.Pending_BCS_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = deptMax > pendingMax ? deptMax : pendingMax;
            foreach (DMBCSViewModel dm in model)
            {
                DMBIRCertSignModel_Pending m = new DMBIRCertSignModel_Pending
                {
                    Pending_BCS_Name = dm.BCS_Name,
                    Pending_BCS_TIN = dm.BCS_TIN,
                    Pending_BCS_Position = dm.BCS_Position,
                    Pending_BCS_MasterID = dm.BCS_MasterID,
                    Pending_BCS_Signatures = dm.BCS_Signatures,
                    Pending_BCS_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_BCS_Filed_Date = DateTime.Now,
                    Pending_BCS_isDeleted = true,
                    Pending_BCS_Status = "For Approval (For Deletion)"
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMBCS_Pending.AddRange(vmList);
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

        //--------------------Expense Entries--------------------------------
        //retrieve vendor list
        public List<SelectList> getCheckEntrySystemVals()
        {
            List<SelectList> listOfLists = new List<SelectList>();

            listOfLists.Add(new SelectList (_context.DMVendor.Where(x => x.Vendor_isActive == true).Select(q => new {q.Vendor_ID, q.Vendor_Name }),
                                                "Vendor_ID", "Vendor_Name"));

            listOfLists.Add(new SelectList(_context.DMDept.Where(x => x.Dept_isActive == true).Select(q => new { q.Dept_ID, q.Dept_Name }),
                                                "Dept_ID", "Dept_Name"));

            listOfLists.Add(new SelectList(_context.DMCurrency.Where(x => x.Curr_isActive == true).Select(q => new { q.Curr_ID, q.Curr_Name }),
                                    "Curr_ID", "Curr_Name"));

            listOfLists.Add(new SelectList(_context.DMTR.Where(x => x.TR_isActive == true).Select(q => new { q.TR_ID, q.TR_Tax_Rate }),
                        "TR_ID", "TR_Tax_Rate"));

            return listOfLists;
        }
        //retrieve account details
        public List<accDetails> getAccDetailsEntry()
        {
            List<accDetails> accDetails = new List<accDetails>();

            var accDbDetails = _context.DMAccount.Where(x => x.Account_isActive == true).Select(q => new { q.Account_ID, q.Account_Name, q.Account_Code });

            foreach (var detail in accDbDetails)
            {
                accDetails temp = new accDetails();
                temp.accId = detail.Account_ID;
                temp.accName = detail.Account_Name;
                temp.accCode = detail.Account_Code;

                accDetails.Add(temp);
            }

            return accDetails;
        }
        //save expense details
        public bool addExpense_CV(EntryCVViewModelList entryModel,int userId)
        {
            float TotalDebit = 0;
            float credEwtTotal = 0;
            float credCashTotal = 0;

            foreach(EntryCVViewModel cv in entryModel.EntryCV)
            {
                TotalDebit += cv.debitGross;
                credEwtTotal += cv.credEwt;
                credCashTotal += cv.credCash;
            }

            if (_modelState.IsValid)
            {
                List<ExpenseEntryDetailModel> expenseDtls = new List<ExpenseEntryDetailModel>();

                foreach (EntryCVViewModel cv in entryModel.EntryCV)
                {
                    List<ExpenseEntryAmortizationModel> expenseAmor = new List<ExpenseEntryAmortizationModel>();
                    List<ExpenseEntryGbaseDtl> expenseGbase = new List<ExpenseEntryGbaseDtl>();

                    foreach (var amorSchedule in cv.amtDetails)
                    {
                        ExpenseEntryAmortizationModel amortization = new ExpenseEntryAmortizationModel
                        {
                            Amor_Sched_Date = amorSchedule.amtDate,
                            Amor_Price = amorSchedule.amtAmount
                        };

                        expenseAmor.Add(amortization);
                    }

                    foreach (var gbaseRemark in cv.gBaseRemarksDetails)
                    {
                        ExpenseEntryGbaseDtl remarks = new ExpenseEntryGbaseDtl
                        {
                            GbaseDtl_Document_Type = gbaseRemark.docType,
                            GbaseDtl_InvoiceNo = gbaseRemark.invNo,
                            GbaseDtl_Description = gbaseRemark.desc,
                            GbaseDtl_Amount = gbaseRemark.amount
                        };

                        expenseGbase.Add(remarks);
                    }

                    ExpenseEntryDetailModel expenseDetails = new ExpenseEntryDetailModel
                    {
                        ExpDtl_Gbase_Remarks = cv.GBaseRemarks,
                        ExpDtl_Account = cv.account,
                        ExpDtl_Fbt = cv.fbt,
                        ExpDtl_Dept = cv.dept,
                        ExpDtl_Vat = cv.vat,
                        ExpDtl_Ewt = cv.ewt,
                        ExpDtl_Ccy = cv.ccy,
                        ExpDtl_Debit = cv.debitGross,
                        ExpDtl_Credit_Ewt = cv.credEwt,
                        ExpDtl_Credit_Cash = cv.credCash,
                        ExpDtl_Amor_Month = cv.month,
                        ExpDtl_Amor_Day = cv.day,
                        ExpDtl_Amor_Duration = cv.duration,
                        ExpenseEntryAmortizations = expenseAmor,
                        ExpenseEntryGbaseDtls = expenseGbase
                    };

                    expenseDtls.Add(expenseDetails);
                }

                ExpenseEntryModel expenseEntry = new ExpenseEntryModel
                {
                    Expense_Date = entryModel.expenseDate,
                    Expense_Payee = entryModel.vendor,
                    Expense_Debit_Total = TotalDebit,
                    Expense_Credit_Total = credEwtTotal + credCashTotal,
                    Expense_Creator_ID = userId,
                    Expense_Created_Date = DateTime.Now,
                    Expense_Last_Updated = DateTime.Now,
                    Expense_isDeleted = false,
                    Expense_Status = 1,
                    ExpenseEntryDetails = expenseDtls
                };

                _context.ExpenseEntry.Add(expenseEntry);
                _context.SaveChanges();
                return true;
            }

            return false;
        }
    }

}
