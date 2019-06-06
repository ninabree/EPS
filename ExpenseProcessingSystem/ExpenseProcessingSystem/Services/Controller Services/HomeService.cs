using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ConstantData;
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using ExpenseProcessingSystem.Services.Controller_Services;
using Microsoft.EntityFrameworkCore;

namespace ExpenseProcessingSystem.Services
{
    public class HomeService
    {
        private readonly string defaultPW = "Mizuho2019";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ModalService _modalservice;

        private ModelStateDictionary _modelState;
        public HomeService(IHttpContextAccessor httpContextAccessor, EPSDbContext context, ModelStateDictionary modelState, IHostingEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _modelState = modelState;
            _hostingEnvironment = hostingEnvironment;
            _modalservice = new ModalService(_httpContextAccessor, _context);
        }
        public string getUserRole(string id)
        {
            var data = _context.User.Where(x => x.User_ID == int.Parse(id))
                .Select(x => x.User_Role).FirstOrDefault() ?? "";
            return data;
        }

        //-----------------------------------Populate-------------------------------------//
        //[ Home ]
        //[Notification]
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
        //Pending
        public PaginatedList<ApplicationsViewModel> getPending(int userID)
        {
            List<ApplicationsViewModel> pendingList = new List<ApplicationsViewModel>();

            var dbPending = from p in _context.ExpenseEntry
                            where (p.Expense_Status == GlobalSystemValues.STATUS_PENDING
                            || p.Expense_Status == GlobalSystemValues.STATUS_VERIFIED
                            || p.Expense_Status == GlobalSystemValues.STATUS_NEW
                            || p.Expense_Status == GlobalSystemValues.STATUS_EDIT
                            || p.Expense_Status == GlobalSystemValues.STATUS_DELETE)
                            && p.Expense_Creator_ID != userID
                            select new { p.Expense_ID, p.Expense_Type, p.Expense_Debit_Total, p.Expense_Payee,
                                        p.Expense_Creator_ID, p.Expense_Verifier_1, p.Expense_Verifier_2,
                                        p.Expense_Last_Updated,p.Expense_Date,p.Expense_Status};

            foreach(var item in dbPending)
            {

                string ver1 = item.Expense_Verifier_1 == 0 ? null : getUserName(item.Expense_Verifier_1);
                string ver2 = item.Expense_Verifier_2 == 0 ? null : getUserName(item.Expense_Verifier_2);
                var linktionary = new Dictionary<int, string>
                {
                    {0,"Data Maintenance" },
                    {GlobalSystemValues.TYPE_CV,"View_CV"},
                    {GlobalSystemValues.TYPE_DDV,"View_DDV"},
                    {GlobalSystemValues.TYPE_NC,"View_NC"},
                    {GlobalSystemValues.TYPE_PC,"View_PCV"},
                    {GlobalSystemValues.TYPE_SS,"View_SS"},
                };


                ApplicationsViewModel tempPending = new ApplicationsViewModel
                {
                    App_ID = item.Expense_ID,
                    App_Type = GlobalSystemValues.getApplicationType(item.Expense_Type),
                    App_Amount = item.Expense_Debit_Total,
                    App_Payee = getVendorName(item.Expense_Payee),
                    App_Maker = getUserName(item.Expense_Creator_ID),
                    App_Verifier_ID_List = new List<string> { ver1, ver2 },
                    App_Date = item.Expense_Date,
                    App_Last_Updated = item.Expense_Last_Updated,
                    App_Status = getStatus(item.Expense_Status),
                    App_Link = linktionary[item.Expense_Type]
                };

                pendingList.Add(tempPending);
            }

            PaginatedList<ApplicationsViewModel> pgPendingList = new PaginatedList<ApplicationsViewModel>(pendingList,pendingList.Count,1,10);

            return pgPendingList;
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
                             }).Where(x => intList.Contains(x.Pending_Vendor_MasterID)).Distinct().ToList();
            var allVTVPending = (from pp in _context.DMVendorTRVAT_Pending
                              from pm in _context.DMVendorTRVAT.Where(x => x.VTV_ID == pp.Pending_VTV_ID).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_VTV_ID,
                                  pp.Pending_VTV_Vendor_ID,
                                  pp.Pending_VTV_TR_ID,
                                  pp.Pending_VTV_VAT_ID
                              }).Where(x => intList.Contains(x.Pending_VTV_Vendor_ID)).ToList();

            List<DMVendorModel_Pending> toDelete = _context.DMVendor_Pending.Where(x=> intList.Contains(x.Pending_Vendor_MasterID)).ToList();
            List<DMVendorTRVATModel_Pending> toVTVDelete = _context.DMVendorTRVAT_Pending.Where(x => intList.Contains(x.Pending_VTV_Vendor_ID)).ToList();

            //get all records that currently exists in Master Data
            List<DMVendorModel> vmList = _context.DMVendor.
                Where(x => intList.Contains(x.Vendor_MasterID) && x.Vendor_isActive == true).ToList();
            List<DMVendorTRVATModel> vmVTVList = _context.DMVendorTRVAT.
                Where(x => intList.Contains(x.VTV_Vendor_ID)).ToList();

            //list for formatted records to be added
            List<DMVendorModel> addList = new List<DMVendorModel>();
            List<DMVendorTRVATModel> addVTVList = new List<DMVendorTRVATModel>();

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
                    Vendor_Status_ID = 3,
                    Vendor_isDeleted = pending.Pending_Vendor_IsDeleted,
                    Vendor_isActive = true
                };
                addList.Add(m);
            });
            allVTVPending.ForEach(pending =>
            {
                DMVendorTRVATModel m = new DMVendorTRVATModel
                {
                    VTV_TR_ID = pending.Pending_VTV_TR_ID,
                    VTV_VAT_ID = pending.Pending_VTV_VAT_ID,
                    VTV_Vendor_ID = pending.Pending_VTV_Vendor_ID
                };
                addVTVList.Add(m);
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
                _context.DMVendorTRVAT.RemoveRange(vmVTVList);
                _context.DMVendorTRVAT.AddRange(addVTVList);
                _context.DMVendorTRVAT_Pending.RemoveRange(toVTVDelete);
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
                    Dept_Status_ID = 3,
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
                    Check_Status_ID = 3,
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
                                  pp.Pending_Account_Group_MasterID,
                                  pp.Pending_Account_Currency_MasterID,
                                  pp.Pending_Account_Code,
                                  pp.Pending_Account_Budget_Code,
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
                    Account_Group_MasterID = pending.Pending_Account_Group_MasterID,
                    Account_Currency_MasterID = pending.Pending_Account_Currency_MasterID,
                    Account_Code = pending.Pending_Account_Code,
                    Account_Budget_Code = pending.Pending_Account_Budget_Code,
                    Account_Cust = pending.Pending_Account_Cust,
                    Account_Div = pending.Pending_Account_Div,
                    Account_Fund = pending.Pending_Account_Fund,
                    Account_No = pending.Pending_Account_No,
                    Account_Creator_ID = pending.pmCreatorID == null ? pending.Pending_Account_Creator_ID : int.Parse(pending.pmCreatorID),
                    Account_Approver_ID = int.Parse(_session.GetString("UserID")),
                    Account_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    Account_Last_Updated = DateTime.Now,
                    Account_Status_ID = 3,
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
        //[ ACCOUNT GROUP ]
        public bool approveAccountGroup(List<DMAccountGroupViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.AccountGroup_MasterID).ToList();

            var allPending = (from pp in _context.DMAccountGroup_Pending
                              from pm in _context.DMAccountGroup.Where(x => x.AccountGroup_MasterID == pp.Pending_AccountGroup_MasterID).DefaultIfEmpty()
                              select new
                              {
                                  pp.Pending_AccountGroup_MasterID,
                                  pp.Pending_AccountGroup_Name,
                                  pp.Pending_AccountGroup_Code,
                                  pp.Pending_AccountGroup_isDeleted,
                                  pp.Pending_AccountGroup_Creator_ID,
                                  pmCreatorID = pm.AccountGroup_Creator_ID.ToString(),
                                  pmCreateDate = pm.AccountGroup_Created_Date.ToString()
                              }).Where(x => intList.Contains(x.Pending_AccountGroup_MasterID)).ToList();

            List<DMAccountGroupModel_Pending> toDelete = _context.DMAccountGroup_Pending.Where(x => intList.Contains(x.Pending_AccountGroup_MasterID)).ToList();

            //get all records that currently exists in Master Data
            List<DMAccountGroupModel> vmList = _context.DMAccountGroup.
                Where(x => intList.Contains(x.AccountGroup_MasterID) && x.AccountGroup_isActive == true).ToList();

            //list for formatted records to be added
            List<DMAccountGroupModel> addList = new List<DMAccountGroupModel>();

            //add to master table newly approved records
            allPending.ForEach(pending =>
            {
                DMAccountGroupModel m = new DMAccountGroupModel
                {
                    AccountGroup_Name = pending.Pending_AccountGroup_Name,
                    AccountGroup_MasterID = pending.Pending_AccountGroup_MasterID,
                    AccountGroup_Code = pending.Pending_AccountGroup_Code,
                    AccountGroup_Creator_ID = pending.pmCreatorID == null ? pending.Pending_AccountGroup_Creator_ID : int.Parse(pending.pmCreatorID),
                    AccountGroup_Approver_ID = int.Parse(_session.GetString("UserID")),
                    AccountGroup_Created_Date = pending.pmCreateDate == null ? DateTime.Now : DateTime.Parse(pending.pmCreateDate),
                    AccountGroup_Last_Updated = DateTime.Now,
                    AccountGroup_Status_ID = 3,
                    AccountGroup_isDeleted = pending.Pending_AccountGroup_isDeleted,
                    AccountGroup_isActive = true
                };
                addList.Add(m);
            });

            //update existing records
            vmList.ForEach(dm =>
            {
                dm.AccountGroup_isActive = false;
            });

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup.AddRange(addList);
                _context.DMAccountGroup_Pending.RemoveRange(toDelete);
                _context.SaveChanges();
            }
            return true;
        }
        public bool rejAccountGroup(List<DMAccountGroupViewModel> model, string userId)
        {
            List<int> intList = model.Select(c => c.AccountGroup_MasterID).ToList();
            List<DMAccountGroupModel_Pending> allPending = _context.DMAccountGroup_Pending.Where(x => intList.Contains(x.Pending_AccountGroup_MasterID)).ToList();

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup_Pending.RemoveRange(allPending);
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
                    VAT_Status_ID = 3,
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
                    FBT_Status_ID = 3,
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
                    TR_Status_ID = 3,
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
                    Curr_Status_ID = 3,
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
                    Emp_Status_ID = 3,
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
                    Cust_Status_ID = 3,
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
                    BCS_Status_ID = 3,
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
                    Pending_Vendor_Status_ID = 7
                };
                vmList.Add(m);
            }
            DMVendorTRVATModel_Pending tmp = new DMVendorTRVATModel_Pending();
            List<DMVendorTRVATModel_Pending> tmpList = new List<DMVendorTRVATModel_Pending>();
            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.AddRange(vmList);
                _context.SaveChanges();
                var i = 0;
                vmList.Select(p => p.Pending_Vendor_MasterID)
                    .ToList()
                    .ForEach(x =>
                    {
                        if (model.NewVendorVM[i].Vendor_Tax_Rates_ID != null)
                        {

                            List<string> trIds = model.NewVendorVM[i].Vendor_Tax_Rates_ID.Split(',').ToList();
                            trIds.ForEach(tr =>
                            {
                                if (tr != "")
                                {
                                    tmp = new DMVendorTRVATModel_Pending()
                                    {
                                        Pending_VTV_Vendor_ID = x,
                                        Pending_VTV_TR_ID = int.Parse(tr)
                                    };
                                    tmpList.Add(tmp);
                                }
                            });
                        }
                        if (model.NewVendorVM[i].Vendor_VAT_ID != null)
                        {
                            List<string> vatIds = model.NewVendorVM[i].Vendor_VAT_ID.Split(',').ToList();
                            vatIds.ForEach(vat =>
                            {
                                if (vat != "")
                                {
                                    tmp = new DMVendorTRVATModel_Pending()
                                    {
                                        Pending_VTV_Vendor_ID = x,
                                        Pending_VTV_VAT_ID = int.Parse(vat)
                                    };
                                    tmpList.Add(tmp);
                                }
                            });
                        }
                        i++;
                    });

                _context.DMVendorTRVAT_Pending.AddRange(tmpList);
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
                    Pending_Vendor_Status_ID = 8
                };
                vmList.Add(m);
            }

            DMVendorTRVATModel_Pending tmp = new DMVendorTRVATModel_Pending();
            List<DMVendorTRVATModel_Pending> tmpList = new List<DMVendorTRVATModel_Pending>();
            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.AddRange(vmList);
                _context.SaveChanges();
                var i = 0;
                vmList.Select(p => p.Pending_Vendor_MasterID)
                    .ToList()
                    .ForEach(x =>
                    {
                        if (model[i].Vendor_Tax_Rates_ID != null)
                        {

                            List<string> trIds = model[i].Vendor_Tax_Rates_ID.Split(',').ToList();
                            trIds.ForEach(tr =>
                            {
                                if (tr != "")
                                {
                                    tmp = new DMVendorTRVATModel_Pending()
                                    {
                                        Pending_VTV_Vendor_ID = x,
                                        Pending_VTV_TR_ID = int.Parse(tr)
                                    };
                                    tmpList.Add(tmp);
                                }
                            });
                        }
                        if (model[i].Vendor_VAT_ID != null)
                        {
                            List<string> vatIds = model[i].Vendor_VAT_ID.Split(',').ToList();
                            vatIds.ForEach(vat =>
                            {
                                if (vat != "")
                                {
                                    tmp = new DMVendorTRVATModel_Pending()
                                    {
                                        Pending_VTV_Vendor_ID = x,
                                        Pending_VTV_VAT_ID = int.Parse(vat)
                                    };
                                    tmpList.Add(tmp);
                                }
                            });
                        }
                        i++;
                    });

                _context.DMVendorTRVAT_Pending.AddRange(tmpList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteVendor_Pending(List<DMVendorViewModel> model, string userId)
        {
            List<DMVendorModel_Pending> vmList = new List<DMVendorModel_Pending>();
            List<DMVendorTRVATModel_Pending> vtvList = new List<DMVendorTRVATModel_Pending>();
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
                    Pending_Vendor_Status_ID = 9
                };
                vmList.Add(m);
            }
            DMVendorTRVATModel_Pending tmp = new DMVendorTRVATModel_Pending();
            model.Select(x => x.Vendor_MasterID)
                .ToList()
                .ForEach(x => {
                     _context.DMVendorTRVAT.Where(tr => tr.VTV_Vendor_ID == x).ToList()
                    .ForEach(val => {
                        if (val.VTV_TR_ID != 0)
                        {
                            tmp = new DMVendorTRVATModel_Pending()
                            {
                                Pending_VTV_Vendor_ID = x,
                                Pending_VTV_TR_ID = val.VTV_TR_ID
                            };
                            vtvList.Add(tmp);
                        }
                        if (val.VTV_VAT_ID != 0)
                        {
                            tmp = new DMVendorTRVATModel_Pending()
                            {
                                Pending_VTV_Vendor_ID = x,
                                Pending_VTV_VAT_ID = val.VTV_VAT_ID
                            };
                            vtvList.Add(tmp);
                        }
                    });
                });

            if (_modelState.IsValid)
            {
                _context.DMVendor_Pending.AddRange(vmList);
                _context.DMVendorTRVAT_Pending.AddRange(vtvList);
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
                    Pending_Dept_Status_ID = 7
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
                    Pending_Dept_Status_ID = 8
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
                    Pending_Dept_Status_ID = 9
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
                    Pending_Check_Status_ID = 7
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
                    Pending_Check_Status_ID = 8
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
                    Pending_Check_Status_ID = 9
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
                    Pending_Account_Group_MasterID = dm.Account_Group_MasterID,
                    Pending_Account_Currency_MasterID = dm.Account_Currency_MasterID,
                    Pending_Account_Code = dm.Account_Code,
                    Pending_Account_Budget_Code = dm.Account_Budget_Code,
                    Pending_Account_Cust = dm.Account_Cust,
                    Pending_Account_Div = dm.Account_Div,
                    Pending_Account_Fund = dm.Account_Fund,
                    Pending_Account_No = dm.Account_No,
                    Pending_Account_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Account_Filed_Date = DateTime.Now,
                    Pending_Account_isDeleted = false,
                    Pending_Account_Status_ID = 7
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
                    Pending_Account_Group_MasterID = dm.Account_Group_MasterID,
                    Pending_Account_Currency_MasterID = dm.Account_Currency_MasterID,
                    Pending_Account_Code = dm.Account_Code,
                    Pending_Account_Budget_Code = dm.Account_Budget_Code,
                    Pending_Account_Cust = dm.Account_Cust,
                    Pending_Account_Div = dm.Account_Div,
                    Pending_Account_Fund = dm.Account_Fund,
                    Pending_Account_No = dm.Account_No,
                    Pending_Account_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Account_Filed_Date = DateTime.Now,
                    Pending_Account_isDeleted = false,
                    Pending_Account_Status_ID = 8
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
                    Pending_Account_Group_MasterID = dm.Account_Group_MasterID,
                    Pending_Account_Currency_MasterID = dm.Account_Currency_MasterID,
                    Pending_Account_Code = dm.Account_Code,
                    Pending_Account_Cust = dm.Account_Cust,
                    Pending_Account_Div = dm.Account_Div,
                    Pending_Account_Fund = dm.Account_Fund,
                    Pending_Account_No = dm.Account_No,
                    Pending_Account_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_Account_Filed_Date = DateTime.Now,
                    Pending_Account_isDeleted = true,
                    Pending_Account_Status_ID = 9
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
        //[ ACCOUNT GROUP ]
        public bool addAccountGroup_Pending(NewAccountGroupListViewModel model, string userId)
        {
            List<DMAccountGroupModel_Pending> vmList = new List<DMAccountGroupModel_Pending>();

            var payeeMax = _context.DMAccountGroup.Select(x => x.AccountGroup_MasterID).
                DefaultIfEmpty(0).Max();
            var pendingMax = _context.DMAccountGroup_Pending.Select(x => x.Pending_AccountGroup_MasterID).
                DefaultIfEmpty(0).Max();

            int masterIDMax = payeeMax > pendingMax ? payeeMax : pendingMax;

            foreach (NewAccountGroupViewModel dm in model.NewAccountGroupVM)
            {
                DMAccountGroupModel_Pending m = new DMAccountGroupModel_Pending
                {
                    Pending_AccountGroup_Name = dm.AccountGroup_Name,
                    Pending_AccountGroup_MasterID = ++masterIDMax,
                    Pending_AccountGroup_Code = dm.AccountGroup_Code,
                    Pending_AccountGroup_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_AccountGroup_Filed_Date = DateTime.Now,
                    Pending_AccountGroup_isDeleted = false,
                    Pending_AccountGroup_Status_ID = 7
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool editAccountGroup_Pending(List<DMAccountGroupViewModel> model, string userId)
        {
            List<DMAccountGroupModel_Pending> vmList = new List<DMAccountGroupModel_Pending>();
            foreach (DMAccountGroupViewModel dm in model)
            {
                DMAccountGroupModel_Pending m = new DMAccountGroupModel_Pending
                {
                    Pending_AccountGroup_Name = dm.AccountGroup_Name,
                    Pending_AccountGroup_MasterID = dm.AccountGroup_MasterID,
                    Pending_AccountGroup_Code = dm.AccountGroup_Code,
                    Pending_AccountGroup_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_AccountGroup_Filed_Date = DateTime.Now,
                    Pending_AccountGroup_isDeleted = false,
                    Pending_AccountGroup_Status_ID = 8
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup_Pending.AddRange(vmList);
                _context.SaveChanges();
            }
            return true;
        }
        public bool deleteAccountGroup_Pending(List<DMAccountGroupViewModel> model, string userId)
        {
            List<DMAccountGroupModel_Pending> vmList = new List<DMAccountGroupModel_Pending>();
            foreach (DMAccountGroupViewModel dm in model)
            {
                DMAccountGroupModel_Pending m = new DMAccountGroupModel_Pending
                {
                    Pending_AccountGroup_Name = dm.AccountGroup_Name,
                    Pending_AccountGroup_MasterID = dm.AccountGroup_MasterID,
                    Pending_AccountGroup_Code = dm.AccountGroup_Code,
                    Pending_AccountGroup_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Pending_AccountGroup_Filed_Date = DateTime.Now,
                    Pending_AccountGroup_isDeleted = true,
                    Pending_AccountGroup_Status_ID = 9
                };
                vmList.Add(m);
            }

            if (_modelState.IsValid)
            {
                _context.DMAccountGroup_Pending.AddRange(vmList);
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
                    Pending_VAT_Status_ID = 7
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
                    Pending_VAT_Status_ID = 8
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
                    Pending_VAT_Status_ID = 9
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
                    Pending_FBT_Status_ID = 7
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
                    Pending_FBT_Status_ID = 8
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
                    Pending_FBT_Status_ID = 9
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
                    Pending_TR_Status_ID = 7
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
                    Pending_TR_Status_ID = 8
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
                    Pending_TR_Status_ID = 9
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
                    Pending_Curr_Status_ID = 7
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
                    Pending_Curr_Status_ID = 8
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
                    Pending_Curr_Status_ID = 9
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
                    Pending_Emp_Status_ID = 7
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
                    Pending_Emp_Status_ID = 8
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
                    Pending_Emp_Status_ID = 9
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
                    Pending_Cust_Status_ID = 7
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
                    Pending_Cust_Status_ID = 8
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
                    Pending_Cust_Status_ID = 9
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
                Pending_BCS_Status_ID = 7
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
                Pending_BCS_Status_ID = 8
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
                    Pending_BCS_Status_ID = 9
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

        // [Budget Monitoring]
        public List<BMViewModel> PopulateBM()
        {
            var accList = _context.DMAccount.Where(x => x.Account_isActive == true && x.Account_isDeleted == false
                                                    && x.Account_Fund == true).ToList();

            List<BMViewModel> bmvmList = new List<BMViewModel>();

            var dbBudget = (from bud in _context.Budget
                            join acc in _context.DMAccount on bud.Budget_Account_ID equals acc.Account_ID
                            join accGrp in _context.DMAccountGroup on acc.Account_Group_MasterID equals accGrp.AccountGroup_MasterID
                            where bud.Budget_IsActive == true && bud.Budget_isDeleted == false &&
                            accGrp.AccountGroup_isActive == true && accGrp.AccountGroup_isDeleted == false
                            select new
                            {
                                bud.Budget_ID,
                                acc.Account_ID,
                                acc.Account_MasterID,
                                accGrp.AccountGroup_Name,
                                bud.Budget_Amount,
                                bud.Budget_Date_Registered
                            }).ToList();

            foreach (var i in dbBudget)
            {
                //Get latest account information of saved account ID.
                var accInfo = accList.Where(x => x.Account_MasterID == i.Account_MasterID && x.Account_isActive == true
                                            && x.Account_isDeleted == false).FirstOrDefault();

                bmvmList.Add(new BMViewModel()
                {
                    BM_Budget_ID = i.Budget_ID,
                    BM_Acc_Group_Name = i.AccountGroup_Name,
                    BM_Acc_Name = accInfo.Account_Name,
                    BM_GBase_Code = accInfo.Account_Budget_Code,
                    BM_Acc_Num = accInfo.Account_No,
                    BM_Budget_Amount = i.Budget_Amount,
                    BM_Date_Registered = i.Budget_Date_Registered
                });
            };

            return bmvmList;
        }

        // [Report]
        public IEnumerable<HomeReportOutputAPSWT_MModel> GetAPSWT_MData(int month, int year)
        {
            int[] status = { 3, 4 };

            var dbAPSWT_M = (from vendor in _context.DMVendor
                                join expense in _context.ExpenseEntry on vendor.Vendor_ID equals expense.Expense_Payee
                                join expEntryDetl in _context.ExpenseEntryDetails on  expense.Expense_ID equals expEntryDetl.ExpenseEntryModel.Expense_ID
                                join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                                where status.Contains(expense.Expense_Status)
                                && expense.Expense_Last_Updated.Month == month
                                && expense.Expense_Last_Updated.Year == year
                             orderby vendor.Vendor_Name
            select new HomeReportOutputAPSWT_MModel
                            {
                                Tin = vendor.Vendor_TIN,
                                Payee = vendor.Vendor_Name,
                                ATC = tr.TR_ATC,
                                NOIP =  tr.TR_Nature,
                                AOIP = expEntryDetl.ExpDtl_Credit_Cash,
                                RateOfTax = tr.TR_Tax_Rate,
                                AOTW = expEntryDetl.ExpDtl_Credit_Ewt
                            }).ToList();

            return dbAPSWT_M;
        }

        public IEnumerable<HomeReportOutputAST1000Model> GetAST1000_SData(int yearSem, int semester)
        {
            int[] status = { 3, 4 };
            float[] taxRateConsider = { 0.01f, 0.02f };
            int[] semesterRange = (semester == 1) ? new int[] { 4, 5, 6, 7, 8, 9 } : new int[] { 10, 11, 12, 1, 2, 3 };

            var dbAST1000_S = (from vendor in _context.DMVendor
                             join expense in _context.ExpenseEntry on vendor.Vendor_ID equals expense.Expense_Payee
                             join expEntryDetl in _context.ExpenseEntryDetails on expense.Expense_ID equals expEntryDetl.ExpenseEntryModel.Expense_ID
                             join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                             where status.Contains(expense.Expense_Status)
                             && semesterRange.Contains(expense.Expense_Last_Updated.Month)
                             && expense.Expense_Last_Updated.Year == yearSem
                             && taxRateConsider.Contains(tr.TR_Tax_Rate)
                               orderby vendor.Vendor_Name
                             select new HomeReportOutputAST1000Model
                             {
                                 Tin = vendor.Vendor_TIN,
                                 SupplierName = vendor.Vendor_Name,
                                 ATC = tr.TR_ATC,
                                 NOIP = tr.TR_Nature,
                                 TaxBase = expEntryDetl.ExpDtl_Credit_Cash,
                                 RateOfTax = tr.TR_Tax_Rate,
                                 AOTW = expEntryDetl.ExpDtl_Credit_Ewt
                             }).ToList();

            return dbAST1000_S;
        }

        public IEnumerable<HomeReportOutputAST1000Model> GetAST1000_AData(int year, int month, int yearTo, int monthTo)
        {
            int[] status = { 3, 4 };
            float[] taxRateConsider = { 0.01f, 0.02f };
            string format = "yyyy-M";
            DateTime fromDate = DateTime.ParseExact(year + "-" + month, format, CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(yearTo + "-" + monthTo, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);

            var dbAST1000_A = (from vendor in _context.DMVendor
                            join expense in _context.ExpenseEntry on vendor.Vendor_ID equals expense.Expense_Payee
                            join expEntryDetl in _context.ExpenseEntryDetails on expense.Expense_ID equals expEntryDetl.ExpenseEntryModel.Expense_ID
                            join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                            where status.Contains(expense.Expense_Status)
                            && expense.Expense_Last_Updated.Date >= fromDate.Date
                            && expense.Expense_Last_Updated.Date <= toDate.Date
                            && taxRateConsider.Contains(tr.TR_Tax_Rate)
                               orderby vendor.Vendor_Name
                               select new HomeReportOutputAST1000Model
                               {
                                   Tin = vendor.Vendor_TIN,
                                   SupplierName = vendor.Vendor_Name,
                                   ATC = tr.TR_ATC,
                                   NOIP = tr.TR_Nature,
                                   TaxBase = expEntryDetl.ExpDtl_Credit_Cash,
                                   RateOfTax = tr.TR_Tax_Rate,
                                   AOTW = expEntryDetl.ExpDtl_Credit_Ewt
                               }).ToList();

            return dbAST1000_A;
        }

        public IEnumerable<HomeReportActualBudgetModel> GetActualReportData(int filterMonth, int filterYear)
        {
            List<HomeReportActualBudgetModel> actualBudgetData = new List<HomeReportActualBudgetModel>();

            DateTime startOfTerm = GetSelectedYearMonthOfTerm(filterMonth, filterYear);
            DateTime startDT;
            DateTime endDT;
            int termYear = startOfTerm.Year;
            int termMonth = startOfTerm.Month;
            double budgetBalance;
            double totalExpenseThisTermToPrevMonthend;
            double subTotal;
            string format = "yyyy-M";

            //Get all account group category with budget from DB
            var accountCategory = (from budget in _context.Budget
                                   join acc in _context.DMAccount on budget.Budget_Account_ID equals acc.Account_ID
                                   join accgroup in _context.DMAccountGroup on acc.Account_Group_MasterID equals accgroup.AccountGroup_MasterID
                                   where accgroup.AccountGroup_isActive == true
                                   && accgroup.AccountGroup_isDeleted == false
                                   orderby accgroup.AccountGroup_MasterID
                                   select new
                                   {
                                       startOfTerm,
                                       accgroup.AccountGroup_Name,
                                       accgroup.AccountGroup_MasterID,
                                       Remarks = "Budget Amount - This Term",
                                       budget.Budget_Amount
                                   }).ToList();

            //Get all expenses amount data between start of term date and last day of before filter month, year from DB
            var expOfPrevMonthsList = (from expDtl in _context.ExpenseEntryDetails
                                       join acc in _context.DMAccount on expDtl.ExpDtl_Account equals acc.Account_MasterID
                                       join accgroup in _context.DMAccountGroup on acc.Account_Group_MasterID equals accgroup.AccountGroup_MasterID
                                       join exp in _context.ExpenseEntry on expDtl.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                                       join dept in _context.DMDept on expDtl.ExpDtl_Dept equals dept.Dept_ID
                                       where exp.Expense_Last_Updated.Date >= startOfTerm.Date
                                       && exp.Expense_Last_Updated.Date <= DateTime.ParseExact(filterYear + "-" + filterMonth, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1)
                                       orderby exp.Expense_Last_Updated
                                       select new
                                       {
                                           exp.Expense_Last_Updated,
                                           accgroup.AccountGroup_MasterID,
                                           accgroup.AccountGroup_Name,
                                           expDtl.ExpDtl_Gbase_Remarks,
                                           dept.Dept_Name,
                                           expDtl.ExpDtl_Credit_Cash
                                       }).ToList();

            foreach (var a in accountCategory)
            {
                startDT = DateTime.ParseExact(termYear + "-" + termMonth, format, CultureInfo.InvariantCulture);
                endDT = DateTime.ParseExact(filterYear + "-" + filterMonth, format, CultureInfo.InvariantCulture);
                subTotal = 0.00;
                totalExpenseThisTermToPrevMonthend = 0.00;

                budgetBalance = a.Budget_Amount;

                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    Category = a.AccountGroup_Name,
                    BudgetBalance = budgetBalance,
                    Remarks = a.Remarks,
                    ValueDate = a.startOfTerm
                });

                //Get total expenses from start of term to Prev monthend of selected month, year

                var expensesOfTermMonthToBeforeFilterMonth = expOfPrevMonthsList.Where(c => c.AccountGroup_MasterID == a.AccountGroup_MasterID && c.Expense_Last_Updated.Date >= startOfTerm.Date
                                       && c.Expense_Last_Updated.Date <= endDT.AddDays(-1));

                foreach (var i in expensesOfTermMonthToBeforeFilterMonth)
                {
                    totalExpenseThisTermToPrevMonthend += i.ExpDtl_Credit_Cash;
                    //Debug.WriteLine(i.AccountGroup_Name + " : " + i.ExpDtl_Credit_Cash + " - " + totalExpenseThisTermToPrevMonthend);
                }

                budgetBalance -= totalExpenseThisTermToPrevMonthend;

                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    BudgetBalance = budgetBalance,
                    ExpenseAmount = totalExpenseThisTermToPrevMonthend,
                    Remarks = "Total Expenses - This Term to Prev Monthend",
                    ValueDate = endDT.AddDays(-1)
                });

                //Get all expenses of selected month and year
                var expensesOfFilterYearMonth = expOfPrevMonthsList.Where(c => c.AccountGroup_MasterID == a.AccountGroup_MasterID 
                && c.Expense_Last_Updated.Month == filterMonth && c.Expense_Last_Updated.Year == filterYear);

                foreach (var i in expensesOfFilterYearMonth)
                {
                    budgetBalance -= i.ExpDtl_Credit_Cash;
                    subTotal += i.ExpDtl_Credit_Cash;

                    actualBudgetData.Add(new HomeReportActualBudgetModel()
                    {
                        BudgetBalance = budgetBalance,
                        ExpenseAmount = i.ExpDtl_Credit_Cash,
                        Remarks = i.ExpDtl_Gbase_Remarks,
                        Department = i.Dept_Name,
                        ValueDate = i.Expense_Last_Updated
                    });
                }

                //Add Sub-Total to List
                if (subTotal != 0)
                {
                    actualBudgetData.Add(new HomeReportActualBudgetModel()
                    {
                        BudgetBalance = budgetBalance,
                        ExpenseAmount = subTotal,
                        Remarks = "Sub-total",
                        ValueDate = endDT.AddMonths(1).AddDays(-1)
                    });
                }

                //Insert break or seperation of row
                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    Category = "BREAK"
                });
            }

            return actualBudgetData;
        }

        public DateTime GetSelectedYearMonthOfTerm(int month, int year)
        {
            int[] firstTermMonths = { 4, 5, 6, 7, 8, 9};
            int[] secodnTermNextYearMonths = { 1, 2, 3};
            DateTime startOfTermDate;

            if (firstTermMonths.Contains(month))
            {
                startOfTermDate = DateTime.ParseExact(year + "-04-01", "yyyy-M-dd", CultureInfo.InvariantCulture);
            }
            else
            {
                if (secodnTermNextYearMonths.Contains(month))
                {
                    startOfTermDate = DateTime.ParseExact((year - 1) + "-10-01", "yyyy-M-dd", CultureInfo.InvariantCulture);
                }
                else
                {
                    startOfTermDate = DateTime.ParseExact(year + "-10-01", "yyyy-M-dd", CultureInfo.InvariantCulture);

                }
            }
            return startOfTermDate;
        }
        
        // [Entry Petty Cash Voucher]
        public IEnumerable<DMVendorModel> PopulateVendorList()
        {
            return _context.DMVendor.Where(db => db.Vendor_isActive == true 
                && db.Vendor_isDeleted == false).OrderBy(db => db.Vendor_Name).ToList();
        }

        public IEnumerable<DMAccountModel> PopulateAccountList()
        {
            return _context.DMAccount.Where(db => db.Account_isActive == true
                && db.Account_isDeleted == false).OrderBy(db => db.Account_Name).ToList();
        }

        public IEnumerable<DMDeptModel> PopulateDepartmentList()
        {
            return _context.DMDept.Where(db => db.Dept_isActive == true
                && db.Dept_isDeleted == false).OrderBy(db => db.Dept_Name).ToList();
        }

        public IEnumerable<DMTRModel> PopulateTaxRateList()
        {
            return _context.DMTR.Where(db => db.TR_isActive == true
                && db.TR_isDeleted == false).OrderBy(db => db.TR_Tax_Rate).ToList();
        }

        //MISC

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
        public void SaveToGBase()
        {

        }

        public void SaveToGBaseFBT()
        {

        }
        //============[Retrieve System Values]=============================
        //retrieve vendor list
        public List<SelectList> getEntrySystemVals()
        {
            List<SelectList> listOfLists = new List<SelectList>();

            listOfLists.Add(new SelectList (_context.DMVendor.Where(x => x.Vendor_isActive == true && x.Vendor_isDeleted == false).Select(q => new {q.Vendor_ID, q.Vendor_Name }),
                                                "Vendor_ID", "Vendor_Name"));

            listOfLists.Add(new SelectList(_context.DMDept.Where(x => x.Dept_isActive == true && x.Dept_isDeleted == false).Select(q => new { q.Dept_ID, q.Dept_Name }),
                                                "Dept_ID", "Dept_Name"));

            listOfLists.Add(new SelectList(_context.DMCurrency.Where(x => x.Curr_isActive == true && x.Curr_isDeleted == false).Select(q => new { q.Curr_ID, q.Curr_CCY_ABBR }),
                                    "Curr_ID", "Curr_CCY_ABBR"));

            listOfLists.Add(new SelectList(_context.DMTR.Where(x => x.TR_isActive == true && x.TR_isDeleted == false).Select(q => new { q.TR_ID, q.TR_Tax_Rate }),
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

        public List<accDetails> getAccDetailsEntry(int account)
        {
            List<accDetails> accDetails = new List<accDetails>();

            var accDbDetails = _context.DMAccount.Where(x => x.Account_ID == account).Select(q => new { q.Account_ID, q.Account_Name, q.Account_Code });

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
        //get Status
        public string getStatus(int id)
        {
            var status = _context.StatusList.SingleOrDefault(q => q.Status_ID == id);
            return status.Status_Name;
        }
        //get userName
        public string getUserName(int id)
        {
            var name = _context.User.SingleOrDefault(q => q.User_ID == id);

            if (name == null)
            {
                return null;
            }

            return name.User_UserName;
        }
        //get vat value
        public float getVat()
        {
            var vat = _context.DMVAT.FirstOrDefault(q => q.VAT_isActive == true);

            if (vat == null)
            {
                return 0;
            }

            return vat.VAT_Rate;
        }
        //get Tax Rate list for specific user
        public SelectList getVendorTaxRate(int vendorID)
        {
            var vendorMasterID = _context.DMVendor.Where(x => x.Vendor_ID == vendorID).Select(id => id.Vendor_MasterID).FirstOrDefault();
            var vendorTRIDList = _context.DMVendorTRVAT.Where(x => x.VTV_Vendor_ID == vendorMasterID 
                                                                && x.VTV_TR_ID > 0)
                                                       .Select(q => q.VTV_TR_ID).ToList();

            var select = new SelectList(_context.DMTR.Where(x => vendorTRIDList.Contains(x.TR_MasterID) 
                                                            && x.TR_isActive == true
                                                            && x.TR_isDeleted == false)
                                                     .Select(q => new { q.TR_ID, TR_Tax_Rate = (q.TR_Tax_Rate * 100) }),
                        "TR_ID", "TR_Tax_Rate");

            return select;
        }
        //get Vat list for specific user
        public SelectList getVendorVat(int vendorID)
        {
            var vendorMasterID = _context.DMVendor.Where(x => x.Vendor_ID == vendorID).Select(id => id.Vendor_MasterID).FirstOrDefault();
            var vendorVatIDList = _context.DMVendorTRVAT.Where(x => x.VTV_Vendor_ID == vendorMasterID
                                                                && x.VTV_VAT_ID > 0)
                                                       .Select(q => q.VTV_VAT_ID).ToList();

            var select = new SelectList(_context.DMVAT.Where(x => vendorVatIDList.Contains(x.VAT_ID)
                                                             && x.VAT_isActive == true
                                                             && x.VAT_isDeleted == false)
                                                      .Select(q => new { q.VAT_ID, VAT_Rate = (q.VAT_Rate * 100) }),
                        "VAT_ID", "VAT_Rate");

            return select;
        }
        //get vendor
        public string getVendorName(int vendorID)
        {
            var vendor = _context.DMVendor.SingleOrDefault(x => x.Vendor_ID == vendorID);

            if (vendor == null)
            {
                return null;
            }

            return vendor.Vendor_Name;
        }
        //============[End Retrieve System Values]========================

        //============[Access Entry Tables]===============================
        //save expense details
        public int addExpense_CV(EntryCVViewModelList entryModel,int userId,int expenseType)
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
                    List<ExpenseEntryCashBreakdownModel> expenseCashBreakdown = new List<ExpenseEntryCashBreakdownModel>();
                    List<ExpenseEntryGbaseDtl> expenseGbase = new List<ExpenseEntryGbaseDtl>();

                    if(expenseType == GlobalSystemValues.TYPE_CV)
                    {
                        foreach (var amorSchedule in cv.amtDetails)
                        {
                            ExpenseEntryAmortizationModel amortization = new ExpenseEntryAmortizationModel
                            {
                                Amor_Sched_Date = amorSchedule.amtDate,
                                Amor_Price = amorSchedule.amtAmount
                            };

                            expenseAmor.Add(amortization);
                        }
                    }
                    else if(expenseType == GlobalSystemValues.TYPE_PC)
                    {
                        foreach (var cashbd in cv.cashBreakdown)
                        {
                            expenseCashBreakdown.Add(new ExpenseEntryCashBreakdownModel
                            {
                                CashBreak_Denimination = cashbd.cashDenimination,
                                CashBreak_NoPcs = cashbd.cashNoPC,
                                CashBreak_Amount = cashbd.cashAmount
                            });
                        }
                    }else if(expenseType == GlobalSystemValues.TYPE_SS && cv.ccyAbbrev == "PHP")
                    {
                        foreach (var cashbd in cv.cashBreakdown)
                        {
                            expenseCashBreakdown.Add(new ExpenseEntryCashBreakdownModel
                            {
                                CashBreak_Denimination = cashbd.cashDenimination,
                                CashBreak_NoPcs = cashbd.cashNoPC,
                                CashBreak_Amount = cashbd.cashAmount
                            });
                        }
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
                        ExpDtl_isEwt = cv.chkEwt,
                        ExpDtl_Credit_Ewt = cv.credEwt,
                        ExpDtl_Credit_Cash = cv.credCash,
                        ExpDtl_SS_Payee = cv.dtlSSPayee,
                        ExpDtl_Amor_Month = cv.month,
                        ExpDtl_Amor_Day = cv.day,
                        ExpDtl_Amor_Duration = cv.duration,
                        ExpenseEntryAmortizations = expenseAmor,
                        ExpenseEntryGbaseDtls = expenseGbase,
                        ExpenseEntryCashBreakdowns = expenseCashBreakdown
                    };
                    expenseDtls.Add(expenseDetails);
                }

                ExpenseEntryModel expenseEntry = new ExpenseEntryModel
                {
                    Expense_Type = expenseType,
                    Expense_Date = entryModel.expenseDate,
                    Expense_Payee = entryModel.vendor,
                    Expense_Debit_Total = TotalDebit,
                    Expense_Credit_Total = credEwtTotal + credCashTotal,
                    Expense_Creator_ID = userId,
                    Expense_Created_Date = (entryModel.entryID == 0 ) ? DateTime.Now : entryModel.createdDate,
                    Expense_Last_Updated = DateTime.Now,
                    Expense_isDeleted = false,
                    Expense_Status = 1,
                    ExpenseEntryDetails = expenseDtls
                };

                _context.ExpenseEntry.Add(expenseEntry);
                _context.SaveChanges();
                return expenseEntry.Expense_ID;
            }
            return -1;
        }

        //retrieve expense details
        public EntryCVViewModelList getExpense(int transID)
        {
            List<EntryCVViewModel> cvList = new List<EntryCVViewModel>();

            var EntryDetails = (from e 
                                in _context.ExpenseEntry
                                where e.Expense_ID == transID
                                select new { e,
                                    ExpenseEntryDetails = from d
                                                          in _context.ExpenseEntryDetails
                                                          where d.ExpenseEntryModel.Expense_ID == e.Expense_ID
                                                          select new { d ,
                                                              ExpenseEntryGbaseDtls = from g
                                                                                      in _context.ExpenseEntryGbaseDtls
                                                                                      where g.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                      select g ,
                                                              ExpenseEntryAmortizations = from a
                                                                                          in _context.ExpenseEntryAmortizations
                                                                                          where a.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                          select a,
                                                              ExpenseEntryCashBreakdown = (from c
                                                                                               in _context.ExpenseEntryCashBreakdown
                                                                                               where c.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                               select c).OrderByDescending(db => db.ExpenseEntryDetailModel.ExpDtl_ID).OrderByDescending(db => db.CashBreak_Denimination)
                                                          }
                                }).FirstOrDefault();

            foreach (var dtl in EntryDetails.ExpenseEntryDetails)
            {
                List<amortizationSchedule> amtDetails = new List<amortizationSchedule>();
                List<EntryGbaseRemarksViewModel> remarksDtl = new List<EntryGbaseRemarksViewModel>();
                List<CashBreakdown> cashBreakdown = new List<CashBreakdown>();

                foreach (var amor in dtl.ExpenseEntryAmortizations)
                {
                    amortizationSchedule amorTemp = new amortizationSchedule() {
                        amtDate = amor.Amor_Sched_Date,
                        amtAmount = amor.Amor_Price
                    };

                    amtDetails.Add(amorTemp);
                }

                foreach (var gbase in dtl.ExpenseEntryGbaseDtls)
                {
                    EntryGbaseRemarksViewModel gbaseTemp = new EntryGbaseRemarksViewModel(){
                        amount = gbase.GbaseDtl_Amount,
                        desc = gbase.GbaseDtl_Description,
                        docType = gbase.GbaseDtl_Document_Type,
                        invNo = gbase.GbaseDtl_InvoiceNo
                    };

                    remarksDtl.Add(gbaseTemp);
                }

                foreach (var cashbd in dtl.ExpenseEntryCashBreakdown)
                {
                    CashBreakdown cashbdTemp = new CashBreakdown()
                    {
                        cashDenimination = cashbd.CashBreak_Denimination,
                        cashNoPC = cashbd.CashBreak_NoPcs,
                        cashAmount = cashbd.CashBreak_Amount
                    };

                    cashBreakdown.Add(cashbdTemp);
                }

                EntryCVViewModel cvDtl = new EntryCVViewModel() {
                    GBaseRemarks = dtl.d.ExpDtl_Gbase_Remarks,
                    account = dtl.d.ExpDtl_Account,
                    fbt = dtl.d.ExpDtl_Fbt,
                    dept = dtl.d.ExpDtl_Dept,
                    chkVat = (dtl.d.ExpDtl_Vat <= 0) ? false : true,
                    vat = dtl.d.ExpDtl_Vat,
                    chkEwt = dtl.d.ExpDtl_isEwt,
                    ewt = dtl.d.ExpDtl_Ewt,
                    ccy = dtl.d.ExpDtl_Ccy,
                    debitGross = dtl.d.ExpDtl_Debit,
                    credEwt = dtl.d.ExpDtl_Credit_Ewt,
                    credCash = dtl.d.ExpDtl_Credit_Cash,
                    dtlSSPayee = dtl.d.ExpDtl_SS_Payee,
                    month = dtl.d.ExpDtl_Amor_Month,
                    day = dtl.d.ExpDtl_Amor_Day,
                    duration = dtl.d.ExpDtl_Amor_Duration,
                    amtDetails = amtDetails,
                    gBaseRemarksDetails = remarksDtl,
                    cashBreakdown = cashBreakdown,
                    modalInputFlag = (cashBreakdown == null || cashBreakdown.Count == 0) ? 0 : 1
                };
                cvList.Add(cvDtl);
            }

            EntryCVViewModelList cvModel = new EntryCVViewModelList()
            {
                entryID = EntryDetails.e.Expense_ID,
                expenseDate = EntryDetails.e.Expense_Date,
                vendor = EntryDetails.e.Expense_Payee,
                expenseYear = EntryDetails.e.Expense_Date.Year.ToString(),
                expenseId = EntryDetails.e.Expense_Number,
                checkNo = EntryDetails.e.Expense_CheckNo,
                status = getStatus(EntryDetails.e.Expense_Status),
                statusID = EntryDetails.e.Expense_Status,
                approver = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Approver),
                verifier_1 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_1),
                verifier_2 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_2),
                maker = EntryDetails.e.Expense_Creator_ID,
                createdDate = EntryDetails.e.Expense_Created_Date,
                EntryCV = cvList
            };

            return cvModel;
        }
        // [RETRIEVE DDV EXPENSE DETAILS]
        public EntryDDVViewModelList getExpenseDDV(int transID)
        {
            List<EntryDDVViewModel> ddvList = new List<EntryDDVViewModel>();

            var EntryDetails = (from e
                                in _context.ExpenseEntry
                                where e.Expense_ID == transID
                                select new
                                {
                                    e,
                                    ExpenseEntryDetails = from d
                                                          in _context.ExpenseEntryDetails
                                                          where d.ExpenseEntryModel.Expense_ID == e.Expense_ID
                                                          select new
                                                          {
                                                              d,
                                                              ExpenseEntryGbaseDtls = from g
                                                                                      in _context.ExpenseEntryGbaseDtls
                                                                                      where g.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                      select g,
                                                              ExpenseEntryInterEntity = from a
                                                                                          in _context.ExpenseEntryInterEntity
                                                                                          where a.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                          select a
                                                          }
                                }).FirstOrDefault();

            foreach (var dtl in EntryDetails.ExpenseEntryDetails)
            {
                List<DDVInterEntityViewModel> interDetails = new List<DDVInterEntityViewModel>();
                List<EntryGbaseRemarksViewModel> remarksDtl = new List<EntryGbaseRemarksViewModel>();
                DDVInterEntityViewModel interDetailsVM = new DDVInterEntityViewModel();
                foreach (ExpenseEntryInterEntityModel inter in dtl.ExpenseEntryInterEntity)
                {
                    interDetailsVM = new DDVInterEntityViewModel()
                    {
                        Inter_ID = inter.Inter_ID,
                        Inter_Particular_Title = inter.Inter_Particular_Title ?? "",
                        Inter_Currency1_ABBR_ID = inter.Inter_Currency1_ABBR,
                        Inter_Currency1_ABBR_Name = "",
                        Inter_Currency1_Amount = inter.Inter_Currency1_Amount ?? "0",
                        Inter_Currency2_ABBR_ID = inter.Inter_Currency2_ABBR,
                        Inter_Currency2_ABBR_Name = "",
                        Inter_Currency2_Amount = inter.Inter_Currency2_Amount ?? "0",
                        Inter_Rate = inter.Inter_Rate ?? "1",
                        Inter_Particular1 = _modalservice.PopulateParticular1(inter.Inter_Particular_Title ?? "", inter.Inter_Currency1_ABBR ?? "", inter.Inter_Currency1_Amount ?? "0", inter.Inter_Currency2_Amount ?? "0", double.Parse(inter.Inter_Rate ?? "1")),
                        Inter_Particular2 = _modalservice.PopulateParticular2(inter.Inter_Currency1_ABBR ?? "", inter.Inter_Currency2_ABBR ?? "", inter.Inter_Currency2_Amount ?? "0", double.Parse(inter.Inter_Rate ?? "1")),
                        Inter_Particular3 = _modalservice.PopulateParticular3(inter.Inter_Currency2_ABBR ?? "", inter.Inter_Currency2_Amount ?? "0")
                    };
                    if (interDetailsVM.Inter_Currency1_ABBR_ID != null)
                    {
                        interDetailsVM.Inter_Currency1_ABBR_Name = _context.DMCurrency.Where(x => x.Curr_MasterID == int.Parse(inter.Inter_Currency1_ABBR) &&
                                                       x.Curr_isDeleted == false && x.Curr_isActive == true).Select(x => x.Curr_Name).FirstOrDefault() ?? "";
                    }
                    if (interDetailsVM.Inter_Currency2_ABBR_ID != null)
                    {
                        interDetailsVM.Inter_Currency2_ABBR_Name = _context.DMCurrency.Where(x => x.Curr_MasterID == int.Parse(inter.Inter_Currency2_ABBR) &&
                                                    x.Curr_isDeleted == false && x.Curr_isActive == true).Select(x => x.Curr_Name).FirstOrDefault() ?? "";
                    }
                    interDetails.Add(interDetailsVM);
                }

                foreach (var gbase in dtl.ExpenseEntryGbaseDtls)
                {
                    EntryGbaseRemarksViewModel gbaseTemp = new EntryGbaseRemarksViewModel()
                    {
                        amount = gbase.GbaseDtl_Amount,
                        desc = gbase.GbaseDtl_Description,
                        docType = gbase.GbaseDtl_Document_Type,
                        invNo = gbase.GbaseDtl_InvoiceNo
                    };

                    remarksDtl.Add(gbaseTemp);
                }


                EntryDDVViewModel ddvDtl = new EntryDDVViewModel()
                {
                    GBaseRemarks = dtl.d.ExpDtl_Gbase_Remarks,
                    account = dtl.d.ExpDtl_Account,
                    account_Name = _context.DMAccount.Where(x => x.Account_ID == dtl.d.ExpDtl_Account && x.Account_isActive == true).Select(x => x.Account_Name).FirstOrDefault(),
                    inter_entity = dtl.d.ExpDtl_Inter_Entity,
                    fbt = dtl.d.ExpDtl_Fbt,
                    dept = dtl.d.ExpDtl_Dept,
                    dept_Name = _context.DMDept.Where(x=> x.Dept_ID == dtl.d.ExpDtl_Dept && x.Dept_isActive == true).Select(x=> x.Dept_Name).FirstOrDefault(),
                    chkVat = (dtl.d.ExpDtl_Vat <= 0) ? false : true,
                    vat = dtl.d.ExpDtl_Vat,
                    vat_Name = _context.DMVAT.Where(x => x.VAT_ID == dtl.d.ExpDtl_Vat && x.VAT_isActive == true).Select(x => x.VAT_Name).FirstOrDefault(),
                    chkEwt = dtl.d.ExpDtl_isEwt,
                    ewt = dtl.d.ExpDtl_isEwt ? 0 : dtl.d.ExpDtl_Ewt,
                    ewt_Name = _context.DMTR.Where(x => x.TR_ID == dtl.d.ExpDtl_Ewt).Select(x => x.TR_Tax_Rate.ToString()).FirstOrDefault(),
                    ewt_Payor_Name = _context.DMTR.Where(x => x.TR_ID == int.Parse(dtl.d.ExpDtl_Ewt_Payor_Name)).Select(x => x.TR_Tax_Rate.ToString()).FirstOrDefault(),
                    ccy = dtl.d.ExpDtl_Ccy,
                    ccy_Name = _context.DMCurrency.Where(x => x.Curr_ID == dtl.d.ExpDtl_Ccy && x.Curr_isActive == true).Select(x => x.Curr_CCY_ABBR).FirstOrDefault(),
                    debitGross = dtl.d.ExpDtl_Debit,
                    credEwt = dtl.d.ExpDtl_Credit_Ewt,
                    credCash = dtl.d.ExpDtl_Credit_Cash,
                    ewtPayorName = dtl.d.ExpDtl_Ewt_Payor_Name,
                    interDetails = interDetails,
                    gBaseRemarksDetails = remarksDtl
                };

                ddvList.Add(ddvDtl);
            }

            EntryDDVViewModelList ddvModel = new EntryDDVViewModelList()
            {
                entryID = EntryDetails.e.Expense_ID,
                expenseDate = EntryDetails.e.Expense_Date,
                vendor = EntryDetails.e.Expense_Payee,
                expenseYear = EntryDetails.e.Expense_Date.Year.ToString(),
                expenseId = EntryDetails.e.Expense_Number,
                checkNo = EntryDetails.e.Expense_CheckNo,
                status = getStatus(EntryDetails.e.Expense_Status),
                statusID = EntryDetails.e.Expense_Status,
                approver = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Approver),
                verifier_1 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_1),
                verifier_2 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_2),
                maker = EntryDetails.e.Expense_Creator_ID,
                EntryDDV = ddvList
            };

            return ddvModel;
        }

        //update status of entry
        public bool updateExpenseStatus(int transID, int status, int userid)
        {
            ExpenseEntryModel m = new ExpenseEntryModel
            {
                Expense_ID = transID,
            };

            if (_modelState.IsValid)
            {
                //_context.ExpenseEntry.Attach(m);
                ExpenseEntryModel dbExpenseEntry = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == transID);

                if (status == GlobalSystemValues.STATUS_VERIFIED)
                    dbExpenseEntry.Expense_Verifier_1 = userid;

                if (status == GlobalSystemValues.STATUS_APPROVED)
                { 
                    dbExpenseEntry.Expense_Approver = userid;
                    if (GlobalSystemValues.STATUS_PENDING == GetCurrentEntryStatus(dbExpenseEntry.Expense_ID))
                    {
                        dbExpenseEntry.Expense_Verifier_1 = userid;
                    }
                }
                dbExpenseEntry.Expense_Status = status;
                dbExpenseEntry.Expense_Last_Updated = DateTime.Now;

                //m.Expense_Status = status;
                //m.Expense_Last_Updated = DateTime.Now;

                _context.SaveChanges();
            }
            else { return false; }
            return true;
        }

        //Delete expense entry
        public bool deleteExpenseEntry(int expense_ID)
        {
            var entry = _context.ExpenseEntry.Where(x => x.Expense_ID == expense_ID).First();
            var entryDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID).ToList();
            foreach (var i in entryDtl)
            {
                _context.ExpenseEntryAmortizations.RemoveRange(_context.ExpenseEntryAmortizations
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                _context.ExpenseEntryCashBreakdown.RemoveRange(_context.ExpenseEntryCashBreakdown
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                _context.ExpenseEntryGbaseDtls.RemoveRange(_context.ExpenseEntryGbaseDtls
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
            }

            _context.ExpenseEntryDetails.RemoveRange(_context.ExpenseEntryDetails
                .Where(x => x.ExpenseEntryModel.Expense_ID == entry.Expense_ID));
            _context.ExpenseEntry.RemoveRange(_context.ExpenseEntry.Where(x => x.Expense_ID == expense_ID));
           
            _context.SaveChanges();

            return true;
        }

        public int GetCurrentEntryStatus(int expense_ID)
        {
            return _context.ExpenseEntry.Where(db => db.Expense_ID == expense_ID).SingleOrDefault().Expense_Status;
        }

        public int addExpense_DDV(EntryDDVViewModelList entryModel, int userId, int expenseType)
        {
            float TotalDebit = 0;
            float credEwtTotal = 0;
            float credCashTotal = 0;

            foreach (EntryDDVViewModel cv in entryModel.EntryDDV)
            {
                TotalDebit += cv.debitGross;
                credEwtTotal += cv.credEwt;
                credCashTotal += cv.credCash;
            }

            if (_modelState.IsValid)
            {
                List<ExpenseEntryDetailModel> expenseDtls = new List<ExpenseEntryDetailModel>();

                foreach (EntryDDVViewModel ddv in entryModel.EntryDDV)
                {
                    List<ExpenseEntryInterEntityModel> expenseInter = new List<ExpenseEntryInterEntityModel>();
                    List<ExpenseEntryGbaseDtl> expenseGbase = new List<ExpenseEntryGbaseDtl>();

                    foreach (var interDetails in ddv.interDetails)
                    {
                        ExpenseEntryInterEntityModel interEntity = new ExpenseEntryInterEntityModel
                        {
                            Inter_Particular_Title = interDetails.Inter_Particular_Title,
                            Inter_Currency1_ABBR = interDetails.Inter_Currency1_ABBR_ID,
                            Inter_Currency1_Amount = interDetails.Inter_Currency1_Amount,
                            Inter_Currency2_ABBR = interDetails.Inter_Currency2_ABBR_ID,
                            Inter_Currency2_Amount = interDetails.Inter_Currency2_Amount,
                            Inter_Rate = interDetails.Inter_Rate
                        };

                        expenseInter.Add(interEntity);
                    }

                    foreach (var gbaseRemark in ddv.gBaseRemarksDetails)
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
                        ExpDtl_Gbase_Remarks = ddv.GBaseRemarks,
                        ExpDtl_Account = ddv.account,
                        ExpDtl_Inter_Entity = ddv.inter_entity,
                        ExpDtl_Fbt = ddv.fbt,
                        ExpDtl_Dept = ddv.dept,
                        ExpDtl_Vat = ddv.vat,
                        ExpDtl_Ewt = ddv.ewt,
                        ExpDtl_Ccy = ddv.ccy,
                        ExpDtl_Debit = ddv.debitGross,
                        ExpDtl_Credit_Ewt = ddv.credEwt,
                        ExpDtl_Credit_Cash = ddv.credCash,
                        ExpDtl_Ewt_Payor_Name = ddv.ewtPayorName,
                        ExpenseEntryInterEntity = expenseInter,
                        ExpenseEntryGbaseDtls = expenseGbase
                    };

                    expenseDtls.Add(expenseDetails);
                }

                ExpenseEntryModel expenseEntry = new ExpenseEntryModel
                {
                    Expense_Type = expenseType,
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
                return expenseEntry.Expense_ID;
            }

            return -1;
        }

        ////============[End Access Entry Tables]=========================

    }



}
