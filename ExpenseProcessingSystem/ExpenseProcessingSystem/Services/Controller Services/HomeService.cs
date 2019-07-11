using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Models.Gbase;
using ExpenseProcessingSystem.Models.Pending;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using ExpenseProcessingSystem.ViewModels.NewRecord;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace ExpenseProcessingSystem.Services
{
    public class HomeService
    {
        private readonly string defaultPW = "Mizuho2019";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private readonly GOExpressContext _GOContext;
        private readonly GWriteContext _gWriteContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ModalService _modalservice;

        private ModelStateDictionary _modelState;
        public HomeService(IHttpContextAccessor httpContextAccessor, EPSDbContext context, GOExpressContext goContext, GWriteContext gWriteContext ,ModelStateDictionary modelState, IHostingEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _GOContext = goContext;
            _gWriteContext = gWriteContext;
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
                            join l in _context.LiquidationEntryDetails on p.Expense_ID equals l.ExpenseEntryModel.Expense_ID into gj
                            from l in gj.DefaultIfEmpty()
                            where (
                            (p.Expense_Status == GlobalSystemValues.STATUS_PENDING
                            || p.Expense_Status == GlobalSystemValues.STATUS_VERIFIED
                            || p.Expense_Status == GlobalSystemValues.STATUS_NEW
                            || p.Expense_Status == GlobalSystemValues.STATUS_EDIT
                            || p.Expense_Status == GlobalSystemValues.STATUS_DELETE)
                            && p.Expense_Creator_ID != userID
                            && p.Expense_Verifier_1 != userID 
                            && p.Expense_Verifier_2 != userID
                            )
                            ||
                            (
                            p.Expense_Status == GlobalSystemValues.STATUS_POSTED
                            && p.Expense_Type == GlobalSystemValues.TYPE_SS
                            && l.Liq_Created_UserID != userID
                            && l.Liq_Verifier1 != userID 
                            && l.Liq_Verifier2 != userID
                            && (l.Liq_Status == GlobalSystemValues.STATUS_PENDING
                            || l.Liq_Status == GlobalSystemValues.STATUS_VERIFIED)
                            )
                            select new
                            {
                                p.Expense_ID,
                                p.Expense_Type,
                                p.Expense_Debit_Total,
                                p.Expense_Payee,
                                p.Expense_Creator_ID,
                                p.Expense_Verifier_1,
                                p.Expense_Verifier_2,
                                p.Expense_Last_Updated,
                                p.Expense_Date,
                                p.Expense_Status,
                                Liq_Status = l == null ? 0 : l.Liq_Status,
                                Liq_Created_UserID = l == null ? 0 : l.Liq_Created_UserID,
                                Liq_Created_Date = l == null ? DateTime.Now : l.Liq_Created_Date,
                                Liq_Verifier1 = l == null ? 0 : l.Liq_Verifier1,
                                Liq_Verifier2 = l == null ? 0 : l.Liq_Verifier2,
                                Liq_LastUpdated_Date = l == null ? DateTime.Now : l.Liq_LastUpdated_Date
                            };

            foreach (var item in dbPending)
            {
                string ver1 = "";
                string ver2 = "";
                var linktionary = new Dictionary<int, string>();

                if (item.Liq_Status == 0)
                {
                    ver1 = item.Expense_Verifier_1 == 0 ? null : getUserName(item.Expense_Verifier_1);
                    ver2 = item.Expense_Verifier_2 == 0 ? null : getUserName(item.Expense_Verifier_2);

                    linktionary = new Dictionary<int, string>
                    {
                        {0,"Data Maintenance" },
                        {GlobalSystemValues.TYPE_CV,"View_CV"},
                        {GlobalSystemValues.TYPE_DDV,"View_DDV"},
                        {GlobalSystemValues.TYPE_NC,"View_NC"},
                        {GlobalSystemValues.TYPE_PC,"View_PCV"},
                        {GlobalSystemValues.TYPE_SS,"View_SS"},
                    };
                }
                else
                {
                    ver1 = item.Liq_Verifier1 == 0 ? null : getUserName(item.Liq_Verifier1);
                    ver2 = item.Liq_Verifier2 == 0 ? null : getUserName(item.Liq_Verifier2);

                    linktionary = new Dictionary<int, string>
                    {
                        {0,"Data Maintenance" },
                        {GlobalSystemValues.TYPE_CV,"View_CV"},
                        {GlobalSystemValues.TYPE_DDV,"View_DDV"},
                        {GlobalSystemValues.TYPE_NC,"View_Liquidation_NC"},
                        {GlobalSystemValues.TYPE_PC,"View_PCV"},
                        {GlobalSystemValues.TYPE_SS,"View_Liquidation_SS"},
                    };
                }

                ApplicationsViewModel tempPending = new ApplicationsViewModel
                {
                    App_ID = item.Expense_ID,
                    App_Type = (item.Liq_Status == 0) ? GlobalSystemValues.getApplicationType(item.Expense_Type) : GlobalSystemValues.getApplicationType(item.Expense_Type) + " (Liquidation)",
                    App_Amount = item.Expense_Debit_Total,
                    App_Payee = getVendorName(item.Expense_Payee),
                    App_Maker = (item.Liq_Status == 0) ? getUserName(item.Expense_Creator_ID) : getUserName(item.Liq_Created_UserID),
                    App_Verifier_ID_List = new List<string> { ver1, ver2 },
                    App_Date = (item.Liq_Status == 0) ? item.Expense_Date : item.Liq_Created_Date,
                    App_Last_Updated = (item.Liq_Status == 0) ? item.Expense_Last_Updated : item.Liq_LastUpdated_Date,
                    App_Status = (item.Liq_Status == 0) ? getStatus(item.Expense_Status) : getStatus(item.Liq_Status),
                    App_Link = linktionary[item.Expense_Type]
                };
                
                pendingList.Add(tempPending);
            }

            PaginatedList<ApplicationsViewModel> pgPendingList = new PaginatedList<ApplicationsViewModel>(pendingList, pendingList.Count, 1, 10);

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
                            a.User_Comment, a.User_InUse, a.User_Creator_ID, a.User_Created_Date, a.User_Approver_ID, a.User_Last_Updated, a.User_Status
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
                    User_Last_Updated = x.User_Last_Updated,
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
                              select new { pp.Pending_Vendor_MasterID,
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

            List<DMVendorModel_Pending> toDelete = _context.DMVendor_Pending.Where(x => intList.Contains(x.Pending_Vendor_MasterID)).ToList();
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
                               })).Where(x => intList.Contains(x.Pending_Dept_MasterID)).Distinct().ToList();

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
            int[] status = { GlobalSystemValues.STATUS_APPROVED, GlobalSystemValues.STATUS_POSTED };
            var vendList = _context.DMVendor.ToList();
            var dbAPSWT_M = (from expEntryDetl in _context.ExpenseEntryDetails
                             join expense in _context.ExpenseEntry on expEntryDetl.ExpenseEntryModel.Expense_ID equals expense.Expense_ID
                             join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                             where status.Contains(expense.Expense_Status)
                             && expense.Expense_Last_Updated.Month == month
                             && expense.Expense_Last_Updated.Year == year
                             orderby expense.Expense_Last_Updated
                             select new HomeReportOutputAPSWT_MModel
                             {
                                 Payee_ID = expense.Expense_Payee,
                                 Payee_SS_ID = expEntryDetl.ExpDtl_SS_Payee,
                                 ATC = tr.TR_ATC,
                                 NOIP = tr.TR_Nature,
                                 AOIP = expEntryDetl.ExpDtl_Credit_Cash,
                                 RateOfTax = tr.TR_Tax_Rate,
                                 AOTW = expEntryDetl.ExpDtl_Credit_Ewt
                             }).ToList();

            foreach (var i in dbAPSWT_M)
            {
                var vendorRecord = vendList.Where(x => x.Vendor_ID == i.Payee_ID || x.Vendor_ID == i.Payee_SS_ID).FirstOrDefault();

                i.Tin = vendorRecord.Vendor_TIN;
                i.Payee = vendorRecord.Vendor_Name;
            }

            return dbAPSWT_M;
        }

        public IEnumerable<HomeReportOutputAST1000Model> GetAST1000_Data(HomeReportViewModel model)
        {
            int[] status = { GlobalSystemValues.STATUS_APPROVED, GlobalSystemValues.STATUS_POSTED };
            string format = "yyyy-M";
            DateTime startDT = DateTime.ParseExact(model.Year + "-" + model.Month, format, CultureInfo.InvariantCulture);
            DateTime endDT = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
            var vendList = _context.DMVendor.ToList();
            var dbAST1000 = (from expEntryDetl in _context.ExpenseEntryDetails
                             join expense in _context.ExpenseEntry on expEntryDetl.ExpenseEntryModel.Expense_ID equals expense.Expense_ID
                             join tr in _context.DMTR on expEntryDetl.ExpDtl_Ewt equals tr.TR_ID
                             where status.Contains(expense.Expense_Status)
                             && model.TaxRateList.Contains(tr.TR_Tax_Rate)
                             && startDT.Date <= expense.Expense_Last_Updated
                             && expense.Expense_Last_Updated <= endDT.Date
                             orderby expense.Expense_Last_Updated
                             select new HomeReportOutputAST1000Model
                             {
                                 Payee_ID = expense.Expense_Payee,
                                 Payee_SS_ID = expEntryDetl.ExpDtl_SS_Payee,
                                 ATC = tr.TR_ATC,
                                 NOIP = tr.TR_Nature,
                                 TaxBase = expEntryDetl.ExpDtl_Credit_Cash,
                                 RateOfTax = tr.TR_Tax_Rate,
                                 AOTW = expEntryDetl.ExpDtl_Credit_Ewt
                             }).ToList();

            foreach (var i in dbAST1000)
            {
                var vendorRecord = vendList.Where(x => x.Vendor_ID == i.Payee_ID || x.Vendor_ID == i.Payee_SS_ID).FirstOrDefault();

                i.Tin = vendorRecord.Vendor_TIN;
                i.SupplierName = vendorRecord.Vendor_Name;
            }

            return dbAST1000;
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
            List<AccGroupBudgetModel> accountCategory = new List<AccGroupBudgetModel>();
            DateTime startOfTerm = GetSelectedYearMonthOfTerm(filterMonth, filterYear);
            DateTime startDT;
            DateTime endDT;
            int termYear = startOfTerm.Year;
            int termMonth = startOfTerm.Month;
            double budgetBalance;
            double totalExpenseThisTermToPrevMonthend;
            double subTotal;
            string format = "yyyy-M";

            endDT = DateTime.ParseExact(filterYear + "-" + filterMonth, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);

            //Get latest Budget that until end of the selected month of each account
            var accountList = _context.DMAccount.Where(x => x.Account_isActive == true && x.Account_isDeleted == false
                                                        && x.Account_Fund == true).OrderBy(x => x.Account_Group_MasterID);
            var accountGrpList = _context.DMAccountGroup.Where(x => x.AccountGroup_isActive == true && x.AccountGroup_isDeleted == false);
            var budgetList = _context.Budget.Where(x => x.Budget_Date_Registered.Date <= endDT.Date)
                                                    .OrderByDescending(x => x.Budget_Date_Registered);
            int currGroup = accountList.First().Account_Group_MasterID;
            double budgetAmount = 0.0;

            if (budgetList.Count() == 0)
            {
                actualBudgetData.Add(new HomeReportActualBudgetModel {
                    BudgetBalance = 0.0,
                    ExpenseAmount = 0.0,
                    Remarks = "NO_RECORD",
                    ValueDate = DateTime.Parse("1991/01/01 12:00:00")
                });
                return actualBudgetData;
            }

            foreach (var i in accountList)
            {
                var budget = budgetList.Where(x => x.Budget_Account_MasterID == i.Account_MasterID)
                    .DefaultIfEmpty(new BudgetModel {
                        Budget_Account_MasterID = i.Account_MasterID,
                        Budget_Amount = 0.0
                    }).OrderByDescending(x => x.Budget_Date_Registered).First();

                if (currGroup == i.Account_Group_MasterID)
                {
                    budgetAmount += budget.Budget_Amount;
                }
                else
                {
                    accountCategory.Add(new AccGroupBudgetModel
                    {
                        StartOfTerm = startOfTerm,
                        AccountGroupName = accountGrpList.Where(x => x.AccountGroup_MasterID == currGroup).FirstOrDefault().AccountGroup_Name,
                        AccountGroupMasterID = currGroup,
                        Remarks = "Budget Amount - This Term",
                        Budget = budgetAmount
                    });

                    budgetAmount = budget.Budget_Amount;
                    currGroup = i.Account_Group_MasterID;

                }
            }

            //Add the last account group info and budget to List.
            accountCategory.Add(new AccGroupBudgetModel
            {
                StartOfTerm = startOfTerm,
                AccountGroupName = accountGrpList.Where(x => x.AccountGroup_MasterID == currGroup).FirstOrDefault().AccountGroup_Name,
                AccountGroupMasterID = currGroup,
                Remarks = "Budget Amount - This Term",
                Budget = budgetAmount
            });

            ////Get all expenses amount data between start of term date and last day of before filter month, year from DB
            var expOfPrevMonthsList = (from expDtl in _context.ExpenseEntryDetails
                                       join acc in _context.DMAccount on expDtl.ExpDtl_Account equals acc.Account_ID
                                       join accgroup in _context.DMAccountGroup on acc.Account_Group_MasterID equals accgroup.AccountGroup_MasterID
                                       join exp in _context.ExpenseEntry on expDtl.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                                       join dept in _context.DMDept on expDtl.ExpDtl_Dept equals dept.Dept_ID
                                       where exp.Expense_Last_Updated.Date >= startOfTerm.Date
                                       && exp.Expense_Last_Updated.Date <= DateTime.ParseExact(filterYear + "-" + filterMonth, format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1).Date
                                       && accgroup.AccountGroup_isActive == true && accgroup.AccountGroup_isDeleted == false
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

                budgetBalance = a.Budget;

                actualBudgetData.Add(new HomeReportActualBudgetModel()
                {
                    Category = a.AccountGroupName,
                    BudgetBalance = budgetBalance,
                    Remarks = a.Remarks,
                    ValueDate = a.StartOfTerm
                });

                //Get total expenses from start of term to Prev monthend of selected month, year

                var expensesOfTermMonthToBeforeFilterMonth = expOfPrevMonthsList.Where(c => c.AccountGroup_MasterID == a.AccountGroupMasterID && c.Expense_Last_Updated.Date >= startOfTerm.Date
                                       && c.Expense_Last_Updated.Date <= endDT.AddDays(-1).Date);

                foreach (var i in expensesOfTermMonthToBeforeFilterMonth)
                {
                    totalExpenseThisTermToPrevMonthend += i.ExpDtl_Credit_Cash;
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
                var expensesOfFilterYearMonth = expOfPrevMonthsList.Where(c => c.AccountGroup_MasterID == a.AccountGroupMasterID
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

        public IEnumerable<GOExpressHistModel> GetTransactionData(HomeReportViewModel model)
        {
            string whereQuery = "";
            DateTime startDT = DateTime.ParseExact(model.Year + "-" + model.Month, "yyyy-M", CultureInfo.InvariantCulture);
            DateTime endDT = DateTime.ParseExact(model.YearTo + "-" + model.MonthTo, "yyyy-M", CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);

            if (model.ReportSubType != 0)
                whereQuery = "Expense_Type = @0";


            if (model.PeriodOption == 1)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "@1 <= Expense_Last_Updated.Date && Expense_Last_Updated.Date <= @2";
            }
            else
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "@3 <= Expense_Last_Updated.Date && Expense_Last_Updated.Date <= @4";

            }

            if (model.CheckNoFrom > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "@5 <= int.Parse(Expense_CheckNo)";
            }

            if (model.CheckNoTo > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "int.Parse(Expense_CheckNo) <= @6";
            }

            if (model.VoucherNoFrom > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "@7 <= Expense_Number";
            }

            if (model.VoucherNoTo > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "Expense_Number <= @8";
            }

            if (model.TransNoFrom > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "@9 <= TL_TransID";
            }

            if (model.TransNoTo > 0)
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "TL_TransID <= @10";
            }

            if (model.SubjName != "")
            {
                if (whereQuery != "")
                    whereQuery += " && ";

                whereQuery += "GOExpHist_Remarks.Contains(@11)";
            }

            if (model.ReportSubType == GlobalSystemValues.TYPE_CV || model.ReportSubType == GlobalSystemValues.TYPE_PC ||
                model.ReportSubType == GlobalSystemValues.TYPE_DDV || model.ReportSubType == GlobalSystemValues.TYPE_SS)
            {
                var dbTransData = (from hist in _context.GOExpressHist
                                   join exp in _context.ExpenseEntry on hist.ExpenseEntryID equals exp.Expense_ID
                                   join trans in _context.ExpenseTransLists on hist.GOExpHist_Id equals trans.TL_GoExpHist_ID
                                   select new 
                                   {
                                       exp.Expense_ID,
                                       exp.Expense_Type,
                                       exp.Expense_Last_Updated,
                                       exp.Expense_Date,
                                       exp.Expense_Number,
                                       exp.Expense_CheckNo,
                                       hist.ExpenseEntryID,
                                       hist.ExpenseDetailID,
                                       hist.GOExpHist_Id,
                                       hist.GOExpHist_SystemName,
                                       hist.GOExpHist_Groupcode,
                                       hist.GOExpHist_Branchno,
                                       hist.GOExpHist_OpeKind,
                                       hist.GOExpHist_AutoApproved,
                                       hist.GOExpHist_WarningOverride,
                                       hist.GOExpHist_CcyFormat,
                                       hist.GOExpHist_OpeBranch,
                                       hist.GOExpHist_ValueDate,
                                       hist.GOExpHist_ReferenceType,
                                       hist.GOExpHist_ReferenceNo,
                                       hist.GOExpHist_Comment,
                                       hist.GOExpHist_Section,
                                       hist.GOExpHist_Remarks,
                                       hist.GOExpHist_Memo,
                                       hist.GOExpHist_SchemeNo,
                                       hist.GOExpHist_Entry11Type,
                                       hist.GOExpHist_Entry11IbfCode,
                                       hist.GOExpHist_Entry11Ccy,
                                       hist.GOExpHist_Entry11Amt,
                                       hist.GOExpHist_Entry11Cust,
                                       hist.GOExpHist_Entry11Actcde,
                                       hist.GOExpHist_Entry11ActType,
                                       hist.GOExpHist_Entry11ActNo,
                                       hist.GOExpHist_Entry11ExchRate,
                                       hist.GOExpHist_Entry11ExchCcy,
                                       hist.GOExpHist_Entry11Fund,
                                       hist.GOExpHist_Entry11CheckNo,
                                       hist.GOExpHist_Entry11Available,
                                       hist.GOExpHist_Entry11AdvcPrnt,
                                       hist.GOExpHist_Entry11Details,
                                       hist.GOExpHist_Entry11Entity,
                                       hist.GOExpHist_Entry11Division,
                                       hist.GOExpHist_Entry11InterAmt,
                                       hist.GOExpHist_Entry11InterRate,
                                       hist.GOExpHist_Entry12Type,
                                       hist.GOExpHist_Entry12IbfCode,
                                       hist.GOExpHist_Entry12Ccy,
                                       hist.GOExpHist_Entry12Amt,
                                       hist.GOExpHist_Entry12Cust,
                                       hist.GOExpHist_Entry12Actcde,
                                       hist.GOExpHist_Entry12ActType,
                                       hist.GOExpHist_Entry12ActNo,
                                       hist.GOExpHist_Entry12ExchRate,
                                       hist.GOExpHist_Entry12ExchCcy,
                                       hist.GOExpHist_Entry12Fund,
                                       hist.GOExpHist_Entry12CheckNo,
                                       hist.GOExpHist_Entry12Available,
                                       hist.GOExpHist_Entry12AdvcPrnt,
                                       hist.GOExpHist_Entry12Details,
                                       hist.GOExpHist_Entry12Entity,
                                       hist.GOExpHist_Entry12Division,
                                       hist.GOExpHist_Entry12InterAmt,
                                       hist.GOExpHist_Entry12InterRate,
                                       hist.GOExpHist_Entry21Type,
                                       hist.GOExpHist_Entry21IbfCode,
                                       hist.GOExpHist_Entry21Ccy,
                                       hist.GOExpHist_Entry21Amt,
                                       hist.GOExpHist_Entry21Cust,
                                       hist.GOExpHist_Entry21Actcde,
                                       hist.GOExpHist_Entry21ActType,
                                       hist.GOExpHist_Entry21ActNo,
                                       hist.GOExpHist_Entry21ExchRate,
                                       hist.GOExpHist_Entry21ExchCcy,
                                       hist.GOExpHist_Entry21Fund,
                                       hist.GOExpHist_Entry21CheckNo,
                                       hist.GOExpHist_Entry21Available,
                                       hist.GOExpHist_Entry21AdvcPrnt,
                                       hist.GOExpHist_Entry21Details,
                                       hist.GOExpHist_Entry21Entity,
                                       hist.GOExpHist_Entry21Division,
                                       hist.GOExpHist_Entry21InterAmt,
                                       hist.GOExpHist_Entry21InterRate,
                                       hist.GOExpHist_Entry22Type,
                                       hist.GOExpHist_Entry22IbfCode,
                                       hist.GOExpHist_Entry22Ccy,
                                       hist.GOExpHist_Entry22Amt,
                                       hist.GOExpHist_Entry22Cust,
                                       hist.GOExpHist_Entry22Actcde,
                                       hist.GOExpHist_Entry22ActType,
                                       hist.GOExpHist_Entry22ActNo,
                                       hist.GOExpHist_Entry22ExchRate,
                                       hist.GOExpHist_Entry22ExchCcy,
                                       hist.GOExpHist_Entry22Fund,
                                       hist.GOExpHist_Entry22CheckNo,
                                       hist.GOExpHist_Entry22Available,
                                       hist.GOExpHist_Entry22AdvcPrnt,
                                       hist.GOExpHist_Entry22Details,
                                       hist.GOExpHist_Entry22Entity,
                                       hist.GOExpHist_Entry22Division,
                                       hist.GOExpHist_Entry22InterAmt,
                                       hist.GOExpHist_Entry22InterRate,
                                       hist.GOExpHist_Entry31Type,
                                       hist.GOExpHist_Entry31IbfCode,
                                       hist.GOExpHist_Entry31Ccy,
                                       hist.GOExpHist_Entry31Amt,
                                       hist.GOExpHist_Entry31Cust,
                                       hist.GOExpHist_Entry31Actcde,
                                       hist.GOExpHist_Entry31ActType,
                                       hist.GOExpHist_Entry31ActNo,
                                       hist.GOExpHist_Entry31ExchRate,
                                       hist.GOExpHist_Entry31ExchCcy,
                                       hist.GOExpHist_Entry31Fund,
                                       hist.GOExpHist_Entry31CheckNo,
                                       hist.GOExpHist_Entry31Available,
                                       hist.GOExpHist_Entry31AdvcPrnt,
                                       hist.GOExpHist_Entry31Details,
                                       hist.GOExpHist_Entry31Entity,
                                       hist.GOExpHist_Entry31Division,
                                       hist.GOExpHist_Entry31InterAmt,
                                       hist.GOExpHist_Entry31InterRate,
                                       hist.GOExpHist_Entry32Type,
                                       hist.GOExpHist_Entry32IbfCode,
                                       hist.GOExpHist_Entry32Ccy,
                                       hist.GOExpHist_Entry32Amt,
                                       hist.GOExpHist_Entry32Cust,
                                       hist.GOExpHist_Entry32Actcde,
                                       hist.GOExpHist_Entry32ActType,
                                       hist.GOExpHist_Entry32ActNo,
                                       hist.GOExpHist_Entry32ExchRate,
                                       hist.GOExpHist_Entry32ExchCcy,
                                       hist.GOExpHist_Entry32Fund,
                                       hist.GOExpHist_Entry32CheckNo,
                                       hist.GOExpHist_Entry32Available,
                                       hist.GOExpHist_Entry32AdvcPrnt,
                                       hist.GOExpHist_Entry32Details,
                                       hist.GOExpHist_Entry32Entity,
                                       hist.GOExpHist_Entry32Division,
                                       hist.GOExpHist_Entry32InterAmt,
                                       hist.GOExpHist_Entry32InterRate,
                                       hist.GOExpHist_Entry41Type,
                                       hist.GOExpHist_Entry41IbfCode,
                                       hist.GOExpHist_Entry41Ccy,
                                       hist.GOExpHist_Entry41Amt,
                                       hist.GOExpHist_Entry41Cust,
                                       hist.GOExpHist_Entry41Actcde,
                                       hist.GOExpHist_Entry41ActType,
                                       hist.GOExpHist_Entry41ActNo,
                                       hist.GOExpHist_Entry41ExchRate,
                                       hist.GOExpHist_Entry41ExchCcy,
                                       hist.GOExpHist_Entry41Fund,
                                       hist.GOExpHist_Entry41CheckNo,
                                       hist.GOExpHist_Entry41Available,
                                       hist.GOExpHist_Entry41AdvcPrnt,
                                       hist.GOExpHist_Entry41Details,
                                       hist.GOExpHist_Entry41Entity,
                                       hist.GOExpHist_Entry41Division,
                                       hist.GOExpHist_Entry41InterAmt,
                                       hist.GOExpHist_Entry41InterRate,
                                       hist.GOExpHist_Entry42Type,
                                       hist.GOExpHist_Entry42IbfCode,
                                       hist.GOExpHist_Entry42Ccy,
                                       hist.GOExpHist_Entry42Amt,
                                       hist.GOExpHist_Entry42Cust,
                                       hist.GOExpHist_Entry42Actcde,
                                       hist.GOExpHist_Entry42ActType,
                                       hist.GOExpHist_Entry42ActNo,
                                       hist.GOExpHist_Entry42ExchRate,
                                       hist.GOExpHist_Entry42ExchCcy,
                                       hist.GOExpHist_Entry42Fund,
                                       hist.GOExpHist_Entry42CheckNo,
                                       hist.GOExpHist_Entry42Available,
                                       hist.GOExpHist_Entry42AdvcPrnt,
                                       hist.GOExpHist_Entry42Details,
                                       hist.GOExpHist_Entry42Entity,
                                       hist.GOExpHist_Entry42Division,
                                       hist.GOExpHist_Entry42InterAmt,
                                       hist.GOExpHist_Entry42InterRate,
                                       trans.TL_ID,
                                       trans.TL_GoExpress_ID,
                                       trans.TL_TransID
                                   }).Where(whereQuery, model.ReportSubType, startDT.Date, endDT.Date, model.PeriodFrom.Date, 
                                   model.PeriodTo.Date, model.CheckNoFrom, model.CheckNoTo, model.VoucherNoFrom, model.VoucherNoTo, 
                                   model.TransNoFrom, model.TransNoTo, model.SubjName).ToList();

                //Console.WriteLine("#############" + model.ReportSubType + startDT.Date + endDT.Date + model.PeriodFrom.Date + model.PeriodTo.Date + model.CheckNoFrom + model.CheckNoTo + model.VoucherNoFrom + model.VoucherNoTo + model.TransNoFrom + model.TransNoTo + model.SubjName);
                Console.WriteLine("########################");
                Console.WriteLine(whereQuery, model.ReportSubType, startDT.Date, endDT.Date, model.PeriodFrom.Date, model.PeriodTo.Date, model.CheckNoFrom, model.CheckNoTo, model.VoucherNoFrom, model.VoucherNoTo, model.TransNoFrom, model.TransNoTo, model.SubjName);
                foreach (var i in dbTransData)
                {
                    Console.WriteLine("#############" + i.Expense_ID);
                }
            }
            else
            {

            }

            return null;
        }

        public DateTime GetSelectedYearMonthOfTerm(int month, int year)
        {
            int[] firstTermMonths = { 4, 5, 6, 7, 8, 9 };
            int[] secodnTermNextYearMonths = { 1, 2, 3 };
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

        public List<float> PopulateTaxRaxListIncludeHist()
        {
            var taxRate = _context.DMTR.OrderBy(x => x.TR_Tax_Rate).Select(x => x.TR_Tax_Rate).ToList().Distinct();
            List<float> taxRateList = new List<float>();
            foreach (var i in taxRate)
            {
                taxRateList.Add(i);
            }

            return taxRateList;
        }

        public IEnumerable<DMBIRCertSignModel> PopulateSignatoryList()
        {
            return _context.DMBCS.Where(x => x.BCS_isActive == true && x.BCS_isDeleted == false).OrderBy(x => x.BCS_Name).ToList();
        }

        public DMBIRCertSignModel GetSignatoryInfo(int id)
        {
            return _context.DMBCS.Where(x => x.BCS_ID == id).FirstOrDefault();
        }

        // [Entry Petty Cash Voucher]
        public IEnumerable<DMVendorModel> PopulateVendorList()
        {
            return _context.DMVendor.Where(x => x.Vendor_isActive == true
                && x.Vendor_isDeleted == false).OrderBy(x => x.Vendor_Name).ToList();
        }

        public IEnumerable<DMAccountModel> PopulateAccountList()
        {
            return _context.DMAccount.Where(x => x.Account_isActive == true
                && x.Account_isDeleted == false).OrderBy(x => x.Account_Name).ToList();
        }

        public IEnumerable<DMDeptModel> PopulateDepartmentList()
        {
            return _context.DMDept.Where(x => x.Dept_isActive == true
                && x.Dept_isDeleted == false).OrderBy(x => x.Dept_Name).ToList();
        }

        public IEnumerable<DMTRModel> PopulateTaxRateList()
        {
            return _context.DMTR.Where(x => x.TR_isActive == true
                && x.TR_isDeleted == false).OrderBy(x => x.TR_Tax_Rate).ToList();
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

        ////============[Access Entry Tables]===============================
        //save expense details
        public int addExpense_CV(EntryCVViewModelList entryModel, int userId, int expenseType)
        {
            float TotalDebit = 0;
            float credEwtTotal = 0;
            float credCashTotal = 0;

            foreach (EntryCVViewModel cv in entryModel.EntryCV)
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

                    if (expenseType == GlobalSystemValues.TYPE_CV)
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
                    else if (expenseType == GlobalSystemValues.TYPE_PC)
                    {
                        foreach (var cashbd in cv.cashBreakdown)
                        {
                            expenseCashBreakdown.Add(new ExpenseEntryCashBreakdownModel
                            {
                                CashBreak_Denomination = cashbd.cashDenomination,
                                CashBreak_NoPcs = cashbd.cashNoPC,
                                CashBreak_Amount = cashbd.cashAmount
                            });
                        }
                    } else if (expenseType == GlobalSystemValues.TYPE_SS && cv.ccyAbbrev == "PHP")
                    {
                        foreach (var cashbd in cv.cashBreakdown)
                        {
                            expenseCashBreakdown.Add(new ExpenseEntryCashBreakdownModel
                            {
                                CashBreak_Denomination = cashbd.cashDenomination,
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
                        ExpDtl_FbtID = (cv.fbt) ? getFbt(getAccount(cv.account).Account_FBT_MasterID) : 0,
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
                    Expense_Created_Date = (entryModel.entryID == 0) ? DateTime.Now : entryModel.createdDate,
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
                                                          select new { d,
                                                              ExpenseEntryGbaseDtls = from g
                                                                                      in _context.ExpenseEntryGbaseDtls
                                                                                      where g.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                      select g,
                                                              ExpenseEntryAmortizations = from a
                                                                                          in _context.ExpenseEntryAmortizations
                                                                                          where a.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                          select a,
                                                              ExpenseEntryCashBreakdown = (from c
                                                                                               in _context.ExpenseEntryCashBreakdown
                                                                                           where c.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                           select c).OrderByDescending(db => db.ExpenseEntryDetailModel.ExpDtl_ID).OrderByDescending(db => db.CashBreak_Denomination)
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
                    EntryGbaseRemarksViewModel gbaseTemp = new EntryGbaseRemarksViewModel() {
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
                        cashDenomination = cashbd.CashBreak_Denomination,
                        cashNoPC = cashbd.CashBreak_NoPcs,
                        cashAmount = cashbd.CashBreak_Amount
                    };

                    cashBreakdown.Add(cashbdTemp);
                }

                EntryCVViewModel cvDtl = new EntryCVViewModel() {
                    expenseDtlID = dtl.d.ExpDtl_ID,
                    GBaseRemarks = dtl.d.ExpDtl_Gbase_Remarks,
                    account = dtl.d.ExpDtl_Account,
                    fbt = dtl.d.ExpDtl_Fbt,
                    fbtID = dtl.d.ExpDtl_FbtID,
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
                expenseId = EntryDetails.e.Expense_Number.ToString().PadLeft(5,'0'),
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
                                                                                        select new
                                                                                        {
                                                                                            a,
                                                                                            ExpenseEntryInterEntityParticular = from p
                                                                                                                                in _context.ExpenseEntryInterEntityParticular
                                                                                                                                where p.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == a.ExpDtl_DDVInter_ID
                                                                                                                                select new
                                                                                                                                {
                                                                                                                                    p,
                                                                                                                                    ExpenseEntryEntityAccounts = from acc
                                                                                                                                                                 in _context.ExpenseEntryInterEntityAccs
                                                                                                                                                                 where acc.ExpenseEntryInterEntityParticular.InterPart_ID == p.InterPart_ID
                                                                                                                                                                 select acc
                                                                                                                                }
                                                                                        }
                                                          }
                                }).FirstOrDefault();
            foreach (var dtl in EntryDetails.ExpenseEntryDetails)
            {
                DDVInterEntityViewModel interDetail = new DDVInterEntityViewModel();
                List<EntryGbaseRemarksViewModel> remarksDtl = new List<EntryGbaseRemarksViewModel>();
                ExpenseEntryInterEntityAccsViewModel interDetailAccs = new ExpenseEntryInterEntityAccsViewModel();
                foreach (var inter in dtl.ExpenseEntryInterEntity)
                {
                    interDetail = new DDVInterEntityViewModel
                    {
                        Inter_Check1 = inter.a.ExpDtl_DDVInter_Check1,
                        Inter_Check2 = inter.a.ExpDtl_DDVInter_Check2,
                        Inter_Convert1_Amount = inter.a.ExpDtl_DDVInter_Conv_Amount1,
                        Inter_Convert2_Amount = inter.a.ExpDtl_DDVInter_Conv_Amount2,
                        Inter_Currency1_ID = inter.a.ExpDtl_DDVInter_Curr1_ID,
                        Inter_Currency1_ABBR =  "",
                        Inter_Currency1_Amount = inter.a.ExpDtl_DDVInter_Amount1,
                        Inter_Currency2_ID = inter.a.ExpDtl_DDVInter_Curr2_ID,
                        Inter_Currency2_ABBR = "",
                        Inter_Currency2_Amount = inter.a.ExpDtl_DDVInter_Amount2,
                        Inter_Rate = (inter.a.ExpDtl_DDVInter_Rate > 0) ? inter.a.ExpDtl_DDVInter_Rate : 1,
                        interPartList = new List<ExpenseEntryInterEntityParticularViewModel>()

                    };

                    if (interDetail.Inter_Currency1_ID > 0)
                    {
                        interDetail.Inter_Currency1_ABBR = _context.DMCurrency.Where(x => x.Curr_ID == inter.a.ExpDtl_DDVInter_Curr1_ID &&
                                                       x.Curr_isDeleted == false && x.Curr_isActive == true).Select(x => x.Curr_CCY_ABBR).FirstOrDefault() ?? "";
                    }
                    if (interDetail.Inter_Currency2_ID > 0)
                    {
                        interDetail.Inter_Currency2_ABBR = _context.DMCurrency.Where(x => x.Curr_ID == inter.a.ExpDtl_DDVInter_Curr2_ID &&
                                                    x.Curr_isDeleted == false && x.Curr_isActive == true).Select(x => x.Curr_CCY_ABBR).FirstOrDefault() ?? "";
                    }
                    //ERROR HERE                                                                  VVVVVV
                    foreach (var interPart in inter.ExpenseEntryInterEntityParticular)
                    {
                        var acc = _context.DMAccount.Where(x => x.Account_ID == dtl.d.ExpDtl_Account).FirstOrDefault();
                        ExpenseEntryInterEntityParticularViewModel interParticular = new ExpenseEntryInterEntityParticularViewModel
                        {
                            InterPart_ID = interPart.p.InterPart_ID,
                            InterPart_Particular_Title = interPart.p.InterPart_Particular_Title,
                            Inter_Particular1 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular1(acc.Account_Name ?? "", interDetail.Inter_Currency1_ABBR ?? "", interDetail.Inter_Currency1_Amount, interDetail.Inter_Currency2_Amount, interDetail.Inter_Rate, acc.Account_ID, interDetail.Inter_Currency1_ID),
                            Inter_Particular2 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular2(interDetail.Inter_Currency1_ABBR, interDetail.Inter_Currency2_ABBR, interDetail.Inter_Currency2_Amount, interDetail.Inter_Rate, interDetail.Inter_Currency1_ID, interDetail.Inter_Currency2_ID),
                            Inter_Particular3 = CONSTANT_DDV_INTER_PARTICULARS.PopulateParticular3(interDetail.Inter_Currency2_ABBR, interDetail.Inter_Currency2_Amount, interDetail.Inter_Currency2_ID),
                            ExpenseEntryInterEntityAccs = new List<ExpenseEntryInterEntityAccsViewModel>()
                        };
                        foreach (var interAcc in interPart.ExpenseEntryEntityAccounts)
                        {
                            ExpenseEntryInterEntityAccsViewModel interDetailAcc = new ExpenseEntryInterEntityAccsViewModel()
                            {
                                Inter_Acc_ID = interAcc.InterAcc_Acc_ID,
                                Inter_Amount = interAcc.InterAcc_Amount,
                                Inter_Curr_ID = interAcc.InterAcc_Curr_ID,
                                Inter_Rate = interAcc.InterAcc_Rate,
                                Inter_Type_ID = interAcc.InterAcc_Type_ID
                            };
                            interParticular.ExpenseEntryInterEntityAccs.Add(interDetailAcc);
                        }
                        interDetail.interPartList.Add(interParticular);
                    }
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
                    dtlID = dtl.d.ExpDtl_ID,
                    GBaseRemarks = dtl.d.ExpDtl_Gbase_Remarks,
                    account = dtl.d.ExpDtl_Account,
                    account_Name = _context.DMAccount.Where(x => x.Account_ID == dtl.d.ExpDtl_Account && x.Account_isActive == true).Select(x => x.Account_Name).FirstOrDefault(),
                    inter_entity = dtl.d.ExpDtl_Inter_Entity,
                    fbt = dtl.d.ExpDtl_Fbt,
                    dept = dtl.d.ExpDtl_Dept,
                    dept_Name = _context.DMDept.Where(x => x.Dept_ID == dtl.d.ExpDtl_Dept && x.Dept_isActive == true).Select(x => x.Dept_Name).FirstOrDefault(),
                    chkVat = (dtl.d.ExpDtl_Vat <= 0) ? false : true,
                    vat = dtl.d.ExpDtl_Vat,
                    //vat_Name is actually the rate
                    vat_Name = _context.DMVAT.Where(x => x.VAT_ID == dtl.d.ExpDtl_Vat && x.VAT_isActive == true).Select(x => x.VAT_Rate.ToString()).FirstOrDefault(),
                    chkEwt = (dtl.d.ExpDtl_Ewt <= 0) ? false : true,
                    ewt = dtl.d.ExpDtl_isEwt ? 0 : dtl.d.ExpDtl_Ewt,
                    ewt_Name = _context.DMTR.Where(x => x.TR_ID == dtl.d.ExpDtl_Ewt).Select(x => x.TR_Tax_Rate.ToString()).FirstOrDefault(),
                    ewt_Payor_Name = (dtl.d.ExpDtl_Ewt_Payor_Name_ID >= 0) ? _context.DMVendor.Where(x => x.Vendor_ID == dtl.d.ExpDtl_Ewt_Payor_Name_ID).Select(x => x.Vendor_Name).FirstOrDefault() : "",
                    ccy = dtl.d.ExpDtl_Ccy,
                    ccy_Name = _context.DMCurrency.Where(x => x.Curr_ID == dtl.d.ExpDtl_Ccy && x.Curr_isActive == true).Select(x => x.Curr_CCY_ABBR).FirstOrDefault(),
                    debitGross = dtl.d.ExpDtl_Debit,
                    credEwt = dtl.d.ExpDtl_Credit_Ewt,
                    credCash = dtl.d.ExpDtl_Credit_Cash,
                    ewt_Payor_Name_ID = (dtl.d.ExpDtl_Ewt_Payor_Name_ID >= 0) ? dtl.d.ExpDtl_Ewt_Payor_Name_ID : 0,
                    interDetails = interDetail,
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
                expenseId = EntryDetails.e.Expense_Number.ToString().PadLeft(5,'0'),
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
        // [RETRIEVE NC EXPENSE DETAILS]
        public EntryNCViewModelList getExpenseNC(int transID)
        {
            List<EntryNCViewModel> ncList = new List<EntryNCViewModel>();

            var EntryDetails = (from e
                                in _context.ExpenseEntry
                                where e.Expense_ID == transID
                                select new
                                {
                                    e,
                                    ExpenseEntryNC = from d
                                                          in _context.ExpenseEntryNonCash
                                                     where d.ExpenseEntryModel.Expense_ID == e.Expense_ID
                                                     select new
                                                     {
                                                         d,
                                                         ExpenseEntryNCDtls = from g
                                                                                 in _context.ExpenseEntryNonCashDetails
                                                                              where g.ExpenseEntryNCModel.ExpNC_ID == d.ExpNC_ID
                                                                              select new
                                                                              {
                                                                                  g,
                                                                                  ExpenseEntryNCDtlAccs = from a
                                                                                                           in _context.ExpenseEntryNonCashDetailAccounts
                                                                                                          where a.ExpenseEntryNCDtlModel.ExpNCDtl_ID == g.ExpNCDtl_ID
                                                                                                          orderby a.ExpNCDtlAcc_Type_ID
                                                                                                          select a
                                                                              }

                                                     }
                                }).FirstOrDefault();
            EntryNCViewModel ncDtlVM = new EntryNCViewModel();
            foreach (var dtl in EntryDetails.ExpenseEntryNC)
            {
                List<ExpenseEntryNCDtlViewModel> ncDtls = new List<ExpenseEntryNCDtlViewModel>();
                ExpenseEntryNCDtlViewModel entryNCDtl;
                foreach (var ncDtl in dtl.ExpenseEntryNCDtls)
                {
                    List<ExpenseEntryNCDtlAccViewModel> ncDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>();
                    ExpenseEntryNCDtlAccViewModel entryNCDtlAcc;
                    foreach (var ncDtlAcc in ncDtl.ExpenseEntryNCDtlAccs)
                    {
                        entryNCDtlAcc = new ExpenseEntryNCDtlAccViewModel()
                        {
                            ExpNCDtlAcc_Acc_ID = ncDtlAcc.ExpNCDtlAcc_Acc_ID,
                            ExpNCDtlAcc_Acc_Name = ncDtlAcc.ExpNCDtlAcc_Acc_Name ?? "",
                            ExpNCDtlAcc_Curr_ID = ncDtlAcc.ExpNCDtlAcc_Curr_ID,
                            ExpNCDtlAcc_Curr_Name = "",
                            ExpNCDtlAcc_Inter_Rate = ncDtlAcc.ExpNCDtlAcc_Inter_Rate,
                            ExpNCDtlAcc_Amount = ncDtlAcc.ExpNCDtlAcc_Amount,
                            ExpNCDtlAcc_Type_ID = ncDtlAcc.ExpNCDtlAcc_Type_ID
                        };
                        if (entryNCDtlAcc.ExpNCDtlAcc_Acc_ID != 0)
                        {
                            entryNCDtlAcc.ExpNCDtlAcc_Acc_Name = _context.DMAccount.Where(x => x.Account_ID == entryNCDtlAcc.ExpNCDtlAcc_Acc_ID).Select(x => x.Account_Name).FirstOrDefault() ?? "";
                        }
                        if (entryNCDtlAcc.ExpNCDtlAcc_Curr_ID != 0)
                        {
                            entryNCDtlAcc.ExpNCDtlAcc_Curr_Name = _context.DMCurrency.Where(x => x.Curr_ID == entryNCDtlAcc.ExpNCDtlAcc_Curr_ID).Select(x => x.Curr_CCY_ABBR).FirstOrDefault() ?? "";
                        }
                        ncDtlAccs.Add(entryNCDtlAcc);
                    }
                    entryNCDtl = new ExpenseEntryNCDtlViewModel()
                    {
                        ExpNCDtl_Remarks_Desc = ncDtl.g.ExpNCDtl_Remarks_Desc,
                        ExpNCDtl_Remarks_Period = ncDtl.g.ExpNCDtl_Remarks_Period,
                        ExpenseEntryNCDtlAccs = ncDtlAccs
                    };
                    ncDtls.Add(entryNCDtl);
                }
                ncDtlVM = new EntryNCViewModel()
                {
                    NC_ID = dtl.d.ExpNC_ID,
                    NC_Category_ID = dtl.d.ExpNC_Category_ID,
                    NC_CredAmt = dtl.d.ExpNC_CredAmt,
                    NC_DebitAmt = dtl.d.ExpNC_DebitAmt,
                    NC_CS_CredAmt = dtl.d.ExpNC_CS_CredAmt,
                    NC_CS_DebitAmt = dtl.d.ExpNC_CS_DebitAmt,
                    NC_IE_CredAmt = dtl.d.ExpNC_IE_CredAmt,
                    NC_IE_DebitAmt = dtl.d.ExpNC_IE_DebitAmt,
                    NC_TotalAmt = dtl.d.ExpNC_CredAmt + dtl.d.ExpNC_DebitAmt,
                    NC_CS_TotalAmt = dtl.d.ExpNC_CS_CredAmt + dtl.d.ExpNC_CS_DebitAmt,
                    NC_IE_TotalAmt = dtl.d.ExpNC_IE_CredAmt + dtl.d.ExpNC_IE_DebitAmt,
                    ExpenseEntryNCDtls = ncDtls
                };
            }
            EntryNCViewModelList ncModel = new EntryNCViewModelList()
            {
                entryID = EntryDetails.e.Expense_ID,
                expenseDate = EntryDetails.e.Expense_Date,
                status = getStatus(EntryDetails.e.Expense_Status),
                statusID = EntryDetails.e.Expense_Status,
                approver = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Approver),
                verifier_1 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_1),
                verifier_2 = (EntryDetails.e.Expense_Status == 1) ? "" : getUserName(EntryDetails.e.Expense_Verifier_2),
                maker = EntryDetails.e.Expense_Creator_ID,
                EntryNC = ncDtlVM
            };

            return ncModel;
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
                {
                    if (dbExpenseEntry.Expense_Verifier_1 == 0)
                    {
                        dbExpenseEntry.Expense_Verifier_1 = userid;
                    }
                    else
                    {
                        if (dbExpenseEntry.Expense_Verifier_2 == 0)
                        {
                            dbExpenseEntry.Expense_Verifier_2 = userid;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                if (status == GlobalSystemValues.STATUS_APPROVED || status == GlobalSystemValues.STATUS_REJECTED)
                {
                    dbExpenseEntry.Expense_Approver = userid;
                    if (GlobalSystemValues.STATUS_PENDING == GetCurrentEntryStatus(dbExpenseEntry.Expense_ID))
                    {
                        dbExpenseEntry.Expense_Verifier_1 = userid;
                    }
                }
                dbExpenseEntry.Expense_Number = getExpTransNo(dbExpenseEntry.Expense_Type);
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
        //Delete expense entry NC
        public bool deleteExpenseEntry(int expense_ID, int expenseType)
        {
            var entry = _context.ExpenseEntry.Where(x => x.Expense_ID == expense_ID).First();
            if(expenseType == GlobalSystemValues.TYPE_NC)
            {
                var entryDtlNC = _context.ExpenseEntryNonCash.Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID).ToList();
                foreach (var nc in entryDtlNC)
                {
                    var entryDtlNCDtl = _context.ExpenseEntryNonCashDetails.Where(x => x.ExpenseEntryNCModel.ExpNC_ID == nc.ExpNC_ID).ToList();
                    foreach (var dtl in entryDtlNCDtl)
                    {
                        _context.ExpenseEntryNonCashDetailAccounts.RemoveRange(_context.ExpenseEntryNonCashDetailAccounts
                            .Where(x => x.ExpenseEntryNCDtlModel.ExpNCDtl_ID == dtl.ExpNCDtl_ID));

                    }
                    _context.ExpenseEntryNonCashDetails.RemoveRange(_context.ExpenseEntryNonCashDetails
                        .Where(x => x.ExpenseEntryNCModel.ExpNC_ID == nc.ExpNC_ID));
                }
                _context.ExpenseEntryNonCash.RemoveRange(_context.ExpenseEntryNonCash
                    .Where(x => x.ExpenseEntryModel.Expense_ID == entry.Expense_ID));
            }
            else if (expenseType == GlobalSystemValues.TYPE_DDV)
            {
                var entryDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expense_ID).ToList();
                foreach (var i in entryDtl)
                {
                    var interList = _context.ExpenseEntryInterEntity.Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID).ToList();
                    foreach (var inter in interList)
                    {
                        var partList = _context.ExpenseEntryInterEntityParticular.Where(x => x.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == inter.ExpDtl_DDVInter_ID).ToList();
                        foreach (var part in partList)
                        {
                            var accList = _context.ExpenseEntryInterEntityAccs.Where(x => x.ExpenseEntryInterEntityParticular.InterPart_ID == part.InterPart_ID).ToList();
                            foreach (var accs in accList)
                            {
                                _context.ExpenseEntryInterEntityAccs.RemoveRange(_context.ExpenseEntryInterEntityAccs
                                    .Where(x => x.ExpenseEntryInterEntityParticular.InterPart_ID == part.InterPart_ID));
                            }
                            _context.ExpenseEntryInterEntityParticular.RemoveRange(_context.ExpenseEntryInterEntityParticular
                                .Where(x => x.ExpenseEntryInterEntityModel.ExpDtl_DDVInter_ID == inter.ExpDtl_DDVInter_ID));
                        }
                        _context.ExpenseEntryInterEntity.RemoveRange(_context.ExpenseEntryInterEntity
                            .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
                    }
                }
                _context.ExpenseEntryDetails.RemoveRange(_context.ExpenseEntryDetails
                    .Where(x => x.ExpenseEntryModel.Expense_ID == entry.Expense_ID));
            }
            else
            {
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
            }
            

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

                    ExpenseEntryInterEntityModel interDetail = new ExpenseEntryInterEntityModel();
                    var inter = ddv.interDetails;

                    interDetail = new ExpenseEntryInterEntityModel
                    {
                        ExpDtl_DDVInter_Check1 = inter.Inter_Check1,
                        ExpDtl_DDVInter_Check2 = inter.Inter_Check2,
                        ExpDtl_DDVInter_Conv_Amount1 = inter.Inter_Convert1_Amount,
                        ExpDtl_DDVInter_Conv_Amount2 = inter.Inter_Convert2_Amount,
                        ExpDtl_DDVInter_Curr1_ID = inter.Inter_Currency1_ID,
                        ExpDtl_DDVInter_Amount1 = inter.Inter_Currency1_Amount,
                        ExpDtl_DDVInter_Curr2_ID = inter.Inter_Currency2_ID,
                        ExpDtl_DDVInter_Amount2 = inter.Inter_Currency2_Amount,
                        ExpDtl_DDVInter_Rate = (inter.Inter_Rate > 0) ? inter.Inter_Rate : 1,
                        ExpenseEntryInterEntityParticular = new List<ExpenseEntryInterEntityParticularModel>()

                    };

                    foreach (ExpenseEntryInterEntityParticularViewModel interPart in inter.interPartList)
                    {
                        var accName = _context.DMAccount.Where(x => x.Account_ID == ddv.account).Select(x => x.Account_Name).FirstOrDefault();
                        ExpenseEntryInterEntityParticularModel interParticular = new ExpenseEntryInterEntityParticularModel
                        {
                            InterPart_ID = interPart.InterPart_ID,
                            InterPart_Particular_Title = interPart.InterPart_Particular_Title
                        };
                        List<ExpenseEntryInterEntityAccsModel> interAccsList = new List<ExpenseEntryInterEntityAccsModel>();
                        foreach (ExpenseEntryInterEntityAccsViewModel interAcc in interPart.ExpenseEntryInterEntityAccs)
                        {
                            ExpenseEntryInterEntityAccsModel interDetailAcc = new ExpenseEntryInterEntityAccsModel()
                            {
                                InterAcc_Acc_ID = interAcc.Inter_Acc_ID,
                                InterAcc_Amount = interAcc.Inter_Amount,
                                InterAcc_Curr_ID = interAcc.Inter_Curr_ID,
                                InterAcc_Rate = interAcc.Inter_Rate,
                                InterAcc_Type_ID = interAcc.Inter_Type_ID
                            };
                            interAccsList.Add(interDetailAcc);
                        }
                        interParticular.ExpenseEntryInterEntityAccs = interAccsList;
                        interDetail.ExpenseEntryInterEntityParticular.Add(interParticular);
                    }
                    
                    expenseInter.Add(interDetail);

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
                        ExpDtl_FbtID = (ddv.fbt) ? getFbt(getAccount(ddv.account).Account_FBT_MasterID) : 0,
                        ExpDtl_Dept = ddv.dept,
                        ExpDtl_Vat = ddv.vat,
                        ExpDtl_Ewt = ddv.ewt,
                        ExpDtl_Ccy = ddv.ccy,
                        ExpDtl_Debit = ddv.debitGross,
                        ExpDtl_Credit_Ewt = ddv.credEwt,
                        ExpDtl_Credit_Cash = ddv.credCash,
                        ExpDtl_Ewt_Payor_Name_ID = ddv.ewt_Payor_Name_ID,
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
                    ExpenseEntryDetails = expenseDtls,
                    Expense_Number = getExpTransNo(expenseType)
                };

                if (entryModel.entryID == 0)
                {
                    _context.ExpenseEntry.Add(expenseEntry);
                }
                else
                {
                    // Update entity in DbSet
                    expenseEntry.Expense_ID = entryModel.entryID;
                    _context.ExpenseEntry.Update(expenseEntry);
                }
                _context.SaveChanges();
                return expenseEntry.Expense_ID;
            }

            return -1;
        }
        public int addExpense_NC(EntryNCViewModelList entryModel, int userId, int expenseType)
        {
            if (_modelState.IsValid)
            {
                List<ExpenseEntryNCDtlModel> expenseDtls = new List<ExpenseEntryNCDtlModel>();

                foreach (var ncDtls in entryModel.EntryNC.ExpenseEntryNCDtls)
                {
                    //Only if Debit and Credit is Not Equal to 0
                    if ((ncDtls.ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Amount != 0) && (ncDtls.ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Amount != 0))
                    {
                        List<ExpenseEntryNCDtlAccModel> accountDtls = new List<ExpenseEntryNCDtlAccModel>();
                        foreach (var accDtls in ncDtls.ExpenseEntryNCDtlAccs)
                        {
                            ExpenseEntryNCDtlAccModel acc = new ExpenseEntryNCDtlAccModel
                            {
                                ExpNCDtlAcc_Acc_ID = accDtls.ExpNCDtlAcc_Acc_ID,
                                ExpNCDtlAcc_Acc_Name = _context.DMAccount.Where(x => x.Account_ID == accDtls.ExpNCDtlAcc_Acc_ID).Select(x => x.Account_Name).FirstOrDefault(),
                                ExpNCDtlAcc_Curr_ID = accDtls.ExpNCDtlAcc_Curr_ID,
                                ExpNCDtlAcc_Amount = accDtls.ExpNCDtlAcc_Amount,
                                ExpNCDtlAcc_Inter_Rate = accDtls.ExpNCDtlAcc_Inter_Rate,
                                ExpNCDtlAcc_Type_ID = accDtls.ExpNCDtlAcc_Type_ID
                            };
                            accountDtls.Add(acc);
                        }
                        ExpenseEntryNCDtlModel expenseDetail = new ExpenseEntryNCDtlModel
                        {
                            ExpNCDtl_Remarks_Desc = ncDtls.ExpNCDtl_Remarks_Desc,
                            ExpNCDtl_Remarks_Period = ncDtls.ExpNCDtl_Remarks_Period,
                            ExpenseEntryNCDtlAccs = accountDtls
                        };
                        expenseDtls.Add(expenseDetail);
                    }
                }
                List<ExpenseEntryNCModel> expenseNCList = new List<ExpenseEntryNCModel>
                {
                    new ExpenseEntryNCModel
                    {
                        ExpNC_Category_ID = entryModel.EntryNC.NC_Category_ID,
                        ExpNC_DebitAmt = entryModel.EntryNC.NC_DebitAmt,
                        ExpNC_CredAmt = entryModel.EntryNC.NC_CredAmt,
                        ExpNC_CS_DebitAmt = (entryModel.EntryNC.NC_CS_DebitAmt > 0) ? entryModel.EntryNC.NC_CS_DebitAmt : (entryModel.EntryNC.ExpenseEntryNCDtls_CDD.Count >= 1 ) ? entryModel.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Amount : 0,
                        ExpNC_CS_CredAmt = (entryModel.EntryNC.NC_CS_CredAmt > 0) ? entryModel.EntryNC.NC_CS_CredAmt :  (entryModel.EntryNC.ExpenseEntryNCDtls_CDD.Count >= 1 ) ? entryModel.EntryNC.ExpenseEntryNCDtls_CDD[0].ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Amount : 0,
                        ExpNC_IE_DebitAmt = entryModel.EntryNC.NC_IE_DebitAmt,
                        ExpNC_IE_CredAmt = entryModel.EntryNC.NC_IE_CredAmt,
                        ExpenseEntryNCDtls = expenseDtls
                    }
                };

                ExpenseEntryModel expenseEntry = new ExpenseEntryModel
                {
                    Expense_Type = expenseType,
                    Expense_Date = entryModel.expenseDate,
                    Expense_Debit_Total = entryModel.EntryNC.NC_DebitAmt,
                    Expense_Credit_Total = entryModel.EntryNC.NC_CredAmt,
                    Expense_Creator_ID = userId,
                    Expense_Created_Date = DateTime.Now,
                    Expense_Last_Updated = DateTime.Now,
                    Expense_isDeleted = false,
                    Expense_Status = 1,
                    ExpenseEntryNC = expenseNCList
                };
                if(entryModel.entryID == 0)
                {
                    _context.ExpenseEntry.Add(expenseEntry);
                }
                else
                {
                    // Update entity in DbSet
                    expenseEntry.Expense_ID = entryModel.entryID;
                    _context.ExpenseEntry.Update(expenseEntry);
                }
                _context.SaveChanges();
                return expenseEntry.Expense_ID;
            }
            return -1;
        }

        //Liquidation
        public List<LiquidationMainListViewModel> populateLiquidationList(int userID)
        {
            List<LiquidationMainListViewModel> postedEntryList = new List<LiquidationMainListViewModel>();

            var dbPostedEntry = from p in _context.ExpenseEntry
                                where p.Expense_Status == GlobalSystemValues.STATUS_POSTED
                                && p.Expense_Type == GlobalSystemValues.TYPE_SS
                                && p.Expense_Last_Updated.Date < DateTime.Now.Date
                                orderby p.Expense_Last_Updated
                                select new
                                {
                                    p.Expense_ID,
                                    p.Expense_Type,
                                    p.Expense_Debit_Total,
                                    p.Expense_Payee,
                                    p.Expense_Creator_ID,
                                    p.Expense_Approver,
                                    p.Expense_Last_Updated,
                                    p.Expense_Date,
                                    p.Expense_Created_Date
                                };

            foreach (var item in dbPostedEntry)
            {
                if (_context.LiquidationEntryDetails.Where(
                    x => x.ExpenseEntryModel.Expense_ID == item.Expense_ID).Count() != 0)
                    continue;

                var linktionary = new Dictionary<int, string>
                {
                    {0,"Data Maintenance" },
                    {GlobalSystemValues.TYPE_CV,""},
                    {GlobalSystemValues.TYPE_DDV,""},
                    {GlobalSystemValues.TYPE_NC,""},
                    {GlobalSystemValues.TYPE_PC,""},
                    {GlobalSystemValues.TYPE_SS,"Liquidation_SS"},
                };

                postedEntryList.Add(new LiquidationMainListViewModel
                {
                    App_ID = item.Expense_ID,
                    App_Type = GlobalSystemValues.getApplicationType(item.Expense_Type),
                    App_Amount = item.Expense_Debit_Total,
                    App_Payee = getVendorName(item.Expense_Payee),
                    App_Maker = getUserName(item.Expense_Creator_ID),
                    App_Approver = getUserName(item.Expense_Approver),
                    App_Date = item.Expense_Created_Date,
                    App_Last_Updated = item.Expense_Last_Updated,
                    App_Link = linktionary[item.Expense_Type]
                });
            }

            return new PaginatedList<LiquidationMainListViewModel>(postedEntryList, postedEntryList.Count, 1, 10);
        }

        //retrieve expense details to Liqudate
        public LiquidationViewModel getExpenseToLiqudate(int transID)
        {
            List<LiquidationDetailsViewModel> liqList = new List<LiquidationDetailsViewModel>();

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
                                                              LiquidationCashBreakdown = from l
                                                                                      in _context.LiquidationCashBreakdown
                                                                                         where l.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                         select l,
                                                              LiquidationInterEntity = from i
                                                                                      in _context.LiquidationInterEntity
                                                                                         where i.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                         select i,
                                                              ExpenseEntryCashBreakdown = (from c
                                                                                               in _context.ExpenseEntryCashBreakdown
                                                                                           where c.ExpenseEntryDetailModel.ExpDtl_ID == d.ExpDtl_ID
                                                                                           select c).OrderByDescending(db => db.ExpenseEntryDetailModel.ExpDtl_ID).OrderByDescending(db => db.CashBreak_Denomination)
                                                          }
                                }).FirstOrDefault();

            foreach (var dtl in EntryDetails.ExpenseEntryDetails)
            {
                List<EntryGbaseRemarksViewModel> remarksDtl = new List<EntryGbaseRemarksViewModel>();
                List<LiquidationCashBreakdown> cashBreakdown = new List<LiquidationCashBreakdown>();
                List<LiquidationCashBreakdown> liqCashBreakdown = new List<LiquidationCashBreakdown>();
                List<LiquidationInterEntity> liqInterEntity = new List<LiquidationInterEntity>();

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

                foreach (var cashbd in dtl.ExpenseEntryCashBreakdown)
                {
                    LiquidationCashBreakdown cashbdTemp = new LiquidationCashBreakdown()
                    {
                        cashDenomination = cashbd.CashBreak_Denomination,
                        cashNoPC = cashbd.CashBreak_NoPcs,
                        cashAmount = cashbd.CashBreak_Amount
                    };

                    cashBreakdown.Add(cashbdTemp);
                }

                foreach (var liqCashbd in dtl.LiquidationCashBreakdown)
                {
                    LiquidationCashBreakdown liqCashbdTemp = new LiquidationCashBreakdown()
                    {
                        cashDenomination = liqCashbd.LiqCashBreak_Denomination,
                        cashNoPC = liqCashbd.LiqCashBreak_NoPcs,
                        cashAmount = liqCashbd.LiqCashBreak_Amount
                    };

                    liqCashBreakdown.Add(liqCashbdTemp);
                }

                foreach (var liqIE in dtl.LiquidationInterEntity)
                {
                    LiquidationInterEntity liqIETemp = new LiquidationInterEntity()
                    {
                        Liq_AccountID_1_1 = liqIE.Liq_AccountID_1_1,
                        Liq_AccountID_1_2 = liqIE.Liq_AccountID_1_2,
                        Liq_AccountID_2_1 = liqIE.Liq_AccountID_2_1,
                        Liq_AccountID_2_2 = liqIE.Liq_AccountID_2_2,
                        Liq_AccountID_3_1 = liqIE.Liq_AccountID_3_1,
                        Liq_AccountID_3_2 = liqIE.Liq_AccountID_3_2,
                        Liq_Amount_1_1 = liqIE.Liq_Amount_1_1,
                        Liq_Amount_1_2 = liqIE.Liq_Amount_1_2,
                        Liq_Amount_2_1 = liqIE.Liq_Amount_2_1,
                        Liq_Amount_2_2 = liqIE.Liq_Amount_2_2,
                        Liq_Amount_3_1 = liqIE.Liq_Amount_3_1,
                        Liq_Amount_3_2 = liqIE.Liq_Amount_3_2,
                        Liq_CCY_1_1 = liqIE.Liq_CCY_1_1,
                        Liq_CCY_1_2 = liqIE.Liq_CCY_1_2,
                        Liq_CCY_2_1 = liqIE.Liq_CCY_2_1,
                        Liq_CCY_2_2 = liqIE.Liq_CCY_2_2,
                        Liq_CCY_3_1 = liqIE.Liq_CCY_3_1,
                        Liq_CCY_3_2 = liqIE.Liq_CCY_3_2,
                        Liq_DebitCred_1_1 = liqIE.Liq_DebitCred_1_1,
                        Liq_DebitCred_1_2 = liqIE.Liq_DebitCred_1_2,
                        Liq_DebitCred_2_1 = liqIE.Liq_DebitCred_2_1,
                        Liq_DebitCred_2_2 = liqIE.Liq_DebitCred_2_2,
                        Liq_DebitCred_3_1 = liqIE.Liq_DebitCred_3_1,
                        Liq_DebitCred_3_2 = liqIE.Liq_DebitCred_3_2,
                        Liq_InterRate_1_1 = liqIE.Liq_InterRate_1_1,
                        Liq_InterRate_1_2 = liqIE.Liq_InterRate_1_2,
                        Liq_InterRate_2_1 = liqIE.Liq_InterRate_2_1,
                        Liq_InterRate_2_2 = liqIE.Liq_InterRate_2_2,
                        Liq_InterRate_3_1 = liqIE.Liq_InterRate_3_1,
                        Liq_InterRate_3_2 = liqIE.Liq_InterRate_3_2
                    };

                    liqInterEntity.Add(liqIETemp);
                }

                var accountInfo = _context.DMAccount.Where(x => x.Account_ID == dtl.d.ExpDtl_Account).Single();
                int liqFlag = 0;
                if(liqCashBreakdown.Count != 0)
                {
                    liqFlag = 1;
                }
                if(liqInterEntity.Count != 0 && liqCashBreakdown.Count == 0)
                {
                    liqFlag = 2;
                }
                LiquidationDetailsViewModel liqDtl = new LiquidationDetailsViewModel()
                {
                    EntryDetailsID = dtl.d.ExpDtl_ID,
                    GBaseRemarks = dtl.d.ExpDtl_Gbase_Remarks,
                    accountID = dtl.d.ExpDtl_Account,
                    accountName = accountInfo.Account_Name,
                    accountNumber = accountInfo.Account_No,
                    accountCode = accountInfo.Account_Code,
                    fbt = dtl.d.ExpDtl_Fbt,
                    deptID = dtl.d.ExpDtl_Dept,
                    deptName = GetDeptName(dtl.d.ExpDtl_Dept),
                    chkVat = (dtl.d.ExpDtl_Vat <= 0) ? false : true,
                    vatID = dtl.d.ExpDtl_Vat,
                    vatValue = (dtl.d.ExpDtl_Vat <= 0) ? 0 : getVat(dtl.d.ExpDtl_Vat),
                    chkEwt = dtl.d.ExpDtl_isEwt,
                    ewtID = dtl.d.ExpDtl_Ewt,
                    ewtValue = (dtl.d.ExpDtl_Ewt <= 0) ? 0 : GetEWTValue(dtl.d.ExpDtl_Ewt),
                    ccyID = dtl.d.ExpDtl_Ccy,
                    ccyAbbrev = GetCurrencyAbbrv(dtl.d.ExpDtl_Ccy),
                    debitGross = dtl.d.ExpDtl_Debit,
                    credEwt = dtl.d.ExpDtl_Credit_Ewt,
                    credCash = dtl.d.ExpDtl_Credit_Cash,
                    dtlSSPayee = dtl.d.ExpDtl_SS_Payee,
                    dtlSSPayeeName = getVendorName(dtl.d.ExpDtl_SS_Payee),
                    gBaseRemarksDetails = remarksDtl,
                    cashBreakdown = cashBreakdown,
                    liqCashBreakdown = liqCashBreakdown,
                    liqInterEntity = liqInterEntity,
                    modalInputFlag = (cashBreakdown == null || cashBreakdown.Count == 0) ? 0 : 1,
                    liqInputFlag = liqFlag
                };
                liqList.Add(liqDtl);
            }

            var liqStatus = _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == transID).FirstOrDefault();

            LiquidationViewModel liqModel = new LiquidationViewModel()
            {
                entryID = EntryDetails.e.Expense_ID,
                expenseDate = EntryDetails.e.Expense_Date,
                vendor = EntryDetails.e.Expense_Payee,
                expenseYear = EntryDetails.e.Expense_Date.Year.ToString(),
                expenseId = EntryDetails.e.Expense_Number.ToString().PadLeft(5,'0'),
                checkNo = EntryDetails.e.Expense_CheckNo,
                statusID = (liqStatus == null) ? 0 : liqStatus.Liq_Status,
                status = (liqStatus == null) ? "" : getStatus(liqStatus.Liq_Status),
                maker = (liqStatus == null) ? EntryDetails.e.Expense_Creator_ID : liqStatus.Liq_Created_UserID,
                verifier_1 = (liqStatus == null) ? "" : getUserName(liqStatus.Liq_Verifier1),
                verifier_2 = (liqStatus == null) ? "" : getUserName(liqStatus.Liq_Verifier2),
                approver = (liqStatus == null) ? "" : getUserName(liqStatus.Liq_Approver),
                createdDate = EntryDetails.e.Expense_Created_Date,
                LiquidationDetails = liqList,
                LiqEntryDetails = (liqStatus == null) ? new LiquidationEntryDetailModel() : liqStatus
            };

            return liqModel;
        }

        //Add liquidation details
        public int addLiquidationDetail(LiquidationViewModel vm, int userid, int count)
        {
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");
            LiquidationCashBreakdownModel model = new LiquidationCashBreakdownModel();
            foreach (var i in vm.LiquidationDetails)
            {
                ExpenseEntryDetailModel dtlModel = _context.ExpenseEntryDetails.Where(x => x.ExpDtl_ID == i.EntryDetailsID).FirstOrDefault();

                if(i.ccyAbbrev == xelem.Element("CURRENCY_PHP").Value)
                {
                    foreach (var j in i.liqCashBreakdown)
                    {
                        _context.LiquidationCashBreakdown.Add(new LiquidationCashBreakdownModel
                        {
                            ExpenseEntryDetailModel = dtlModel,
                            LiqCashBreak_Denomination = j.cashDenomination,
                            LiqCashBreak_NoPcs = j.cashNoPC,
                            LiqCashBreak_Amount = j.cashAmount
                        });
                        _context.SaveChanges();
                    }

                    _context.LiquidationInterEntity.Add(new LiquidationInterEntityModel
                    {
                        ExpenseEntryDetailModel = dtlModel,
                        Liq_DebitCred_1_1 = i.liqInterEntity[0].Liq_DebitCred_1_1,
                        Liq_AccountID_1_1 = i.liqInterEntity[0].Liq_AccountID_1_1,
                        Liq_Amount_1_1 = i.liqInterEntity[0].Liq_Amount_1_1,
                        Liq_DebitCred_1_2 = i.liqInterEntity[0].Liq_DebitCred_1_2,
                        Liq_AccountID_1_2 = i.liqInterEntity[0].Liq_AccountID_1_2,
                        Liq_Amount_1_2 = i.liqInterEntity[0].Liq_Amount_1_2,
                        Liq_DebitCred_2_1 = i.liqInterEntity[0].Liq_DebitCred_2_1,
                        Liq_AccountID_2_1 = i.liqInterEntity[0].Liq_AccountID_2_1,
                        Liq_Amount_2_1 = i.liqInterEntity[0].Liq_Amount_2_1,
                        Liq_DebitCred_2_2 = i.liqInterEntity[0].Liq_DebitCred_2_2,
                        Liq_AccountID_2_2 = i.liqInterEntity[0].Liq_AccountID_2_2,
                        Liq_Amount_2_2 = i.liqInterEntity[0].Liq_Amount_2_2,
                        Liq_DebitCred_3_1 = i.liqInterEntity[0].Liq_DebitCred_3_1,
                        Liq_AccountID_3_1 = i.liqInterEntity[0].Liq_AccountID_3_1,
                        Liq_Amount_3_1 = i.liqInterEntity[0].Liq_Amount_3_1,
                        Liq_DebitCred_3_2 = i.liqInterEntity[0].Liq_DebitCred_3_2,
                        Liq_AccountID_3_2 = i.liqInterEntity[0].Liq_AccountID_3_2,
                        Liq_Amount_3_2 = i.liqInterEntity[0].Liq_Amount_3_2,
                    });
                    _context.SaveChanges();
                }
                else
                {
                    foreach (var j in i.liqInterEntity)
                    {
                        _context.LiquidationInterEntity.Add(new LiquidationInterEntityModel
                        {
                            ExpenseEntryDetailModel = dtlModel,
                            Liq_DebitCred_1_1 = j.Liq_DebitCred_1_1,
                            Liq_AccountID_1_1 = j.Liq_AccountID_1_1,
                            Liq_InterRate_1_1 = j.Liq_InterRate_1_1,
                            Liq_CCY_1_1 = j.Liq_CCY_1_1,
                            Liq_Amount_1_1 = j.Liq_Amount_1_1,
                            Liq_DebitCred_1_2 = j.Liq_DebitCred_1_2,
                            Liq_AccountID_1_2 = j.Liq_AccountID_1_2,
                            Liq_InterRate_1_2 = j.Liq_InterRate_1_2,
                            Liq_CCY_1_2 = j.Liq_CCY_1_2,
                            Liq_Amount_1_2 = j.Liq_Amount_1_2,
                            Liq_DebitCred_2_1 = j.Liq_DebitCred_2_1,
                            Liq_AccountID_2_1 = j.Liq_AccountID_2_1,
                            Liq_InterRate_2_1 = j.Liq_InterRate_2_1,
                            Liq_CCY_2_1 = j.Liq_CCY_2_1,
                            Liq_Amount_2_1 = j.Liq_Amount_2_1,
                            Liq_DebitCred_2_2 = j.Liq_DebitCred_2_2,
                            Liq_AccountID_2_2 = j.Liq_AccountID_2_2,
                            Liq_InterRate_2_2 = j.Liq_InterRate_2_2,
                            Liq_CCY_2_2 = j.Liq_CCY_2_2,
                            Liq_Amount_2_2 = j.Liq_Amount_2_2
                        });
                        _context.SaveChanges();
                    }
                }
                
            }

            ExpenseEntryModel expenseModel = _context.ExpenseEntry.Where(x => x.Expense_ID == vm.entryID).FirstOrDefault();
            _context.LiquidationEntryDetails.Add(new LiquidationEntryDetailModel
            {
                ExpenseEntryModel = expenseModel,
                Liq_Status = GlobalSystemValues.STATUS_PENDING,
                Liq_Created_Date = (count == 0) ? DateTime.Now : vm.LiqEntryDetails.Liq_Created_Date,
                Liq_LastUpdated_Date = DateTime.Now,
                Liq_Created_UserID = userid
            });
            _context.SaveChanges();

            return vm.entryID;
        }

        //Update liquidation status
        public bool updateLiquidateStatus(int id, int status, int userid)
        {
            if (_modelState.IsValid)
            {
                var liquidateEntry = _context.LiquidationEntryDetails.Include(x => x.ExpenseEntryModel)
                .Where(x => x.ExpenseEntryModel.Expense_ID == id).FirstOrDefault();

                if (status == GlobalSystemValues.STATUS_VERIFIED)
                    liquidateEntry.Liq_Verifier1 = userid;

                if (status == GlobalSystemValues.STATUS_APPROVED)
                {
                    liquidateEntry.Liq_Approver = userid;
                    if (GlobalSystemValues.STATUS_PENDING == getCurrentLiquidationStatus(liquidateEntry.ExpenseEntryModel.Expense_ID))
                    {
                        liquidateEntry.Liq_Verifier1 = userid;
                    }
                }

                liquidateEntry.Liq_Status = status;
                liquidateEntry.Liq_LastUpdated_Date = DateTime.Now;
                _context.SaveChanges();
            }
            else { return false; }
            return true;
        }

        //Delete liquidation entry
        public bool deleteLiquidationEntry(int entryID)
        {
            var entryDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).ToList();
            foreach (var i in entryDtl)
            {
                _context.LiquidationCashBreakdown.RemoveRange(_context.LiquidationCashBreakdown
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
            }

            foreach (var i in entryDtl)
            {
                _context.LiquidationInterEntity.RemoveRange(_context.LiquidationInterEntity
                    .Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID));
            }

            _context.LiquidationEntryDetails.RemoveRange(_context.LiquidationEntryDetails
                .Where(x => x.ExpenseEntryModel.Expense_ID == entryID));

            _context.SaveChanges();

            return true;
        }

        //Check liquidation record existence
        public int getLiquidationExistence(int entryID)
        {
            return _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).Count();
        }

        //Check current status of liqudation entry
        public int getCurrentLiquidationStatus(int entryID)
        {
            return _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).SingleOrDefault().Liq_Status;
        }

        ////============[End Access Entry Tables]=========================

        ///==============[Post Entries]==============
        public bool postCV(int expID)
        {
            var expenseDetails = getExpense(expID);

            var list = new[] {
                new { expEntryID = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

            list.Clear();

            foreach (var item in expenseDetails.EntryCV)
            {
                gbaseContainer tempGbase = new gbaseContainer();

                tempGbase.valDate = expenseDetails.expenseDate;
                tempGbase.remarks = item.GBaseRemarks;
                tempGbase.maker = expenseDetails.maker;
                tempGbase.approver = _context.ExpenseEntry.FirstOrDefault(x=>x.Expense_ID == expID).Expense_Approver;

                entryContainer credit = new entryContainer();
                entryContainer debit = new entryContainer();

                credit.type = "C";
                debit.type = "D";

                credit.ccy = item.ccy;
                debit.ccy = item.ccy;
                credit.amount = item.debitGross;
                debit.amount = item.debitGross;
                credit.vendor = expenseDetails.vendor;
                debit.vendor = expenseDetails.vendor;
                credit.account = item.account;
                debit.account = item.account;
                debit.chkNo = expenseDetails.checkNo;
                debit.dept = item.dept;
                credit.dept = item.dept;

                tempGbase.entries.Add(credit);
                tempGbase.entries.Add(debit);

                goExpData = InsertGbaseEntry(tempGbase, expID);
                goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.expenseDtlID);
                list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });

                if (item.fbt)
                {
                    tempGbase.entries = new List<entryContainer>();

                    //((ExpenseAmount*.50)/.65)*.35
                    string fbt = getFbtFormula(getAccount(item.account).Account_FBT_MasterID);

                    string equation = fbt.Replace("ExpenseAmount", item.debitGross.ToString());
                    double fbtAmount = Math.Round(Convert.ToDouble(new DataTable().Compute(equation, null)),2);
                    Console.WriteLine(equation);
                    credit.amount = fbtAmount;
                    debit.amount = fbtAmount;

                    tempGbase.entries.Add(credit);
                    tempGbase.entries.Add(debit);

                    goExpData = InsertGbaseEntry(tempGbase, expID);
                    goExpHistData = new GOExpressHistModel();
                    list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });
                }
            }

            _GOContext.SaveChanges();
            _context.SaveChanges();

            List<ExpenseTransList> transactions = new List<ExpenseTransList>();

            foreach (var item in list)
            {
                ExpenseTransList tran = new ExpenseTransList
                {
                    TL_ExpenseID = item.expEntryID,
                    TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
                    TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                    TL_Liquidation = false
                };
                transactions.Add(tran);
            }

            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();
            return true;
        }
        public bool postLiq_SS(int expID)
        {
            var liquidationDetails = getExpenseToLiqudate(expID);
            var list = new[] {
                new { expEntryID = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

            list.Clear();

            foreach (var item in liquidationDetails.LiquidationDetails)
            {
                if (item.liqCashBreakdown.Count() != 0)
                {
                    gbaseContainer tempGbase = new gbaseContainer();

                    tempGbase.valDate = liquidationDetails.LiqEntryDetails.Liq_Created_Date.Date;
                    tempGbase.remarks = "S" + item.GBaseRemarks;
                    tempGbase.maker = liquidationDetails.LiqEntryDetails.Liq_Created_UserID;
                    tempGbase.approver = liquidationDetails.LiqEntryDetails.Liq_Approver;

                    if (item.liqInterEntity[0].Liq_Amount_1_1 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = item.liqInterEntity[0].Liq_DebitCred_1_1,
                            amount = item.liqInterEntity[0].Liq_Amount_1_1,
                            account = getAccountID(item.liqInterEntity[0].Liq_AccountID_1_1),
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_1_2 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = item.liqInterEntity[0].Liq_DebitCred_1_2,
                            amount = item.liqInterEntity[0].Liq_Amount_1_2,
                            account = getAccountID(item.liqInterEntity[0].Liq_AccountID_1_2),
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_2_1 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = item.liqInterEntity[0].Liq_DebitCred_2_1,
                            amount = item.liqInterEntity[0].Liq_Amount_2_1,
                            account = getAccountID(item.liqInterEntity[0].Liq_AccountID_2_1),
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_2_2 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = item.liqInterEntity[0].Liq_DebitCred_2_2,
                            amount = item.liqInterEntity[0].Liq_Amount_2_2,
                            account = getAccountID(item.liqInterEntity[0].Liq_AccountID_2_2),
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_3_1 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = item.liqInterEntity[0].Liq_DebitCred_3_1,
                            amount = item.liqInterEntity[0].Liq_Amount_3_1,
                            account = getAccountID(item.liqInterEntity[0].Liq_AccountID_3_1),
                        });
                    }

                    goExpData = InsertGbaseEntry(tempGbase, expID);
                    goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.EntryDetailsID);
                    list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });
                }
                else if (item.liqInterEntity.Count() != 0 && item.liqCashBreakdown.Count() == 0)
                {
                    foreach(var i in item.liqInterEntity)
                    {
                        if (i.Liq_Amount_1_1 == 0 && i.Liq_Amount_1_2 == 0)
                            continue;

                        gbaseContainer tempGbase = new gbaseContainer();

                        tempGbase.valDate = liquidationDetails.LiqEntryDetails.Liq_Created_Date.Date;
                        tempGbase.remarks = "S" + item.GBaseRemarks;
                        tempGbase.maker = liquidationDetails.LiqEntryDetails.Liq_Created_UserID;
                        tempGbase.approver = liquidationDetails.LiqEntryDetails.Liq_Approver;

                        if(i.Liq_Amount_1_1 != 0) { 
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = i.Liq_DebitCred_1_1,
                                ccy = getCurrencyID(i.Liq_CCY_1_1),
                                amount = i.Liq_Amount_1_1,
                                account = getAccountID(i.Liq_AccountID_1_1),
                                interate = i.Liq_InterRate_1_1
                            });
                        }
                        double amount = i.Liq_Amount_1_2;
                        string contraCcy = "";
                        string ccy = i.Liq_CCY_1_2;

                        if (i.Liq_CCY_1_1 != i.Liq_CCY_1_2)
                        {
                            amount = i.Liq_Amount_1_1;
                            ccy = i.Liq_CCY_1_1;
                            contraCcy = i.Liq_CCY_1_2;
                        }
                        if (i.Liq_Amount_1_2 != 0)
                        {
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = i.Liq_DebitCred_1_2,
                                ccy = getCurrencyID(ccy),
                                amount = amount,
                                account = getAccountID(i.Liq_AccountID_1_2),
                                interate = i.Liq_InterRate_1_2,
                                contraCcy = getCurrencyID(contraCcy)
                            });
                        }
                        if(i.Liq_Amount_2_1 > 0 || i.Liq_Amount_2_2 > 0)
                        {
                            if (i.Liq_Amount_2_1 != 0)
                            {
                                tempGbase.entries.Add(new entryContainer
                                {
                                    type = i.Liq_DebitCred_2_1,
                                    ccy = getCurrencyID(i.Liq_CCY_2_1),
                                    amount = i.Liq_Amount_2_1,
                                    account = getAccountID(i.Liq_AccountID_2_1),
                                    interate = i.Liq_InterRate_2_1
                                });
                            }
                            amount = i.Liq_Amount_2_2;
                            contraCcy = "";
                            ccy = i.Liq_CCY_2_2;

                            if (i.Liq_CCY_2_1 != i.Liq_CCY_2_2)
                            {
                                amount = i.Liq_Amount_2_1;
                                ccy = i.Liq_CCY_2_1;
                                contraCcy = i.Liq_CCY_2_2;
                            }
                            if (i.Liq_Amount_2_2 != 0)
                            {
                                tempGbase.entries.Add(new entryContainer
                                {
                                    type = i.Liq_DebitCred_2_2,
                                    ccy = getCurrencyID(ccy),
                                    amount = amount,
                                    account = getAccountID(i.Liq_AccountID_2_2),
                                    interate = i.Liq_InterRate_2_2,
                                    contraCcy = getCurrencyID(contraCcy)
                                });
                            }
                        }

                        goExpData = InsertGbaseEntry(tempGbase, expID);
                        goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.EntryDetailsID);
                        list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });
                    }
                }

                //if (item.fbt)
                //{
                //    tempGbase.entries = new List<entryContainer>();

                //    //var fbt = getAccount()


                //    credit.amount = Convert.ToDouble(new DataTable().Compute("(3+3)*2+1", null));
                //    debit.amount = Convert.ToDouble(new DataTable().Compute("(3+3)*2+1", null));
                //}
            }

            _GOContext.SaveChanges();
            _context.SaveChanges();

            List<ExpenseTransList> transactions = new List<ExpenseTransList>();

            foreach (var item in list)
            {
                ExpenseTransList tran = new ExpenseTransList
                {
                    TL_ExpenseID = item.expEntryID,
                    TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
                    TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                    TL_Liquidation = true
                };
                transactions.Add(tran);
            }

            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();

            return true;
        }
        public bool postCV(int expID, string command)
        {
            var expenseDetails = getExpense(expID);

            var list = new[] {
                new { expEntryID = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

            list.Clear();

            foreach (var item in expenseDetails.EntryCV)
            {
                gbaseContainer tempGbase = new gbaseContainer();

                tempGbase.valDate = expenseDetails.expenseDate;
                tempGbase.remarks = item.GBaseRemarks;
                tempGbase.maker = expenseDetails.maker;
                tempGbase.approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver;

                entryContainer credit = new entryContainer();
                entryContainer debit = new entryContainer();

                credit.type = (command != "R") ? "C" : "D";
                debit.type = (command != "R") ? "D" : "C";

                credit.ccy = item.ccy;
                debit.ccy = item.ccy;
                credit.amount = item.debitGross;
                debit.amount = item.debitGross;
                credit.vendor = expenseDetails.vendor;
                debit.vendor = expenseDetails.vendor;
                credit.account = item.account;
                debit.account = item.account;
                debit.chkNo = expenseDetails.checkNo;
                debit.dept = item.dept;
                credit.dept = item.dept;

                tempGbase.entries.Add(credit);
                tempGbase.entries.Add(debit);

                goExpData = InsertGbaseEntry(tempGbase, expID);
                goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.expenseDtlID);
                list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });

                if (item.fbt)
                {
                    tempGbase.entries = new List<entryContainer>();

                    //((ExpenseAmount*.50)/.65)*.35
                    string fbt = getFbtFormula(getAccount(item.account).Account_FBT_MasterID);

                    string equation = fbt.Replace("ExpenseAmount", item.debitGross.ToString());
                    double fbtAmount = Math.Round(Convert.ToDouble(new DataTable().Compute(equation, null)), 2);
                    Console.WriteLine("-=-=-=-=-=->" + equation);
                    credit.amount = fbtAmount;
                    debit.amount = fbtAmount;

                    tempGbase.entries.Add(credit);
                    tempGbase.entries.Add(debit);

                    goExpData = InsertGbaseEntry(tempGbase, expID);
                    goExpHistData = new GOExpressHistModel();
                    list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });
                }
            }
            
            _GOContext.SaveChanges();
            _context.SaveChanges();

            List<ExpenseTransList> transactions = new List<ExpenseTransList>();

            foreach (var item in list)
            {
                ExpenseTransList tran = new ExpenseTransList
                {
                    TL_ExpenseID = item.expEntryID,
                    TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
                    TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                    TL_Liquidation = false
                };
                transactions.Add(tran);
            }

            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();

            return true;
        }
        public bool postDDV(int expID, string command)
        {
            var expenseDDV = getExpenseDDV(expID);

            var list = new[] {
                new { expEntryID = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

            list.Clear();

            foreach (var item in expenseDDV.EntryDDV)
            {
                if (item.inter_entity)
                {
                    DDVInterEntityViewModel interDtls = item.interDetails;

                    foreach (var particular in item.interDetails.interPartList)
                    {
                        gbaseContainer tempGbase = new gbaseContainer
                        {
                            valDate = expenseDDV.expenseDate,
                            remarks = particular.InterPart_Particular_Title,
                            maker = expenseDDV.maker,
                            approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver
                        };
                        foreach (var accs in particular.ExpenseEntryInterEntityAccs)
                        {
                            var entryType = (accs.Inter_Type_ID == GlobalSystemValues.NC_DEBIT) ? "D" :
                                            (accs.Inter_Type_ID == GlobalSystemValues.NC_CREDIT) ? "C" : "";
                            if (command == "R")
                            {
                                entryType = (entryType == "D") ? "C" : (entryType == "C") ? "D" : "";
                            }
                            if (entryType != "")
                            {
                                entryContainer entry = new entryContainer
                                {
                                    account = accs.Inter_Acc_ID,
                                    amount = accs.Inter_Amount,
                                    ccy = accs.Inter_Curr_ID,
                                    dept = item.dept,
                                    interate = accs.Inter_Rate,
                                    //vendor = item.
                                    type = entryType
                                };
                                tempGbase.entries.Add(entry);

                                if (item.fbt)
                                {
                                    tempGbase.entries = new List<entryContainer>();
                                    string fbt = getFbtFormula(getAccount(item.account).Account_FBT_MasterID);

                                    string equation = fbt.Replace("ExpenseAmount", item.debitGross.ToString());
                                    double fbtAmount = Math.Round(Convert.ToDouble(new DataTable().Compute(equation, null)), 2);
                                    Console.WriteLine("-=-=-=-=-=->" + equation);
                                    entry.amount = fbtAmount;

                                    tempGbase.entries.Add(entry);

                                    goExpData = InsertGbaseEntry(tempGbase, expID);
                                    goExpHistData = new GOExpressHistModel();
                                    list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });
                                }
                            }
                        }
                        goExpData = InsertGbaseEntry(tempGbase, expID);
                        goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.dtlID);
                        list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });
                    }
                } else
                {
                    gbaseContainer tempGbase = new gbaseContainer();

                    tempGbase.valDate = expenseDDV.expenseDate;
                    tempGbase.remarks = item.GBaseRemarks;
                    tempGbase.maker = expenseDDV.maker;
                    tempGbase.approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver;

                    entryContainer credit = new entryContainer();
                    entryContainer debit = new entryContainer();

                    credit.type = (command != "R") ? "C" : "D";
                    debit.type = (command != "R") ? "D" : "C";

                    credit.ccy = item.ccy;
                    debit.ccy = item.ccy;
                    credit.amount = item.debitGross;
                    debit.amount = item.debitGross;
                    credit.vendor = expenseDDV.vendor;
                    debit.vendor = expenseDDV.vendor;
                    credit.account = item.account;
                    debit.account = item.account;
                    debit.chkNo = expenseDDV.checkNo;
                    debit.dept = item.dept;
                    credit.dept = item.dept;

                    tempGbase.entries.Add(credit);
                    tempGbase.entries.Add(debit);

                    goExpData = InsertGbaseEntry(tempGbase, expID);
                    goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, item.dtlID);
                    list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });

                    if (item.fbt)
                    {
                        tempGbase.entries = new List<entryContainer>();

                        //((ExpenseAmount*.50)/.65)*.35
                        string fbt = getFbtFormula(getAccount(item.account).Account_FBT_MasterID);

                        string equation = fbt.Replace("ExpenseAmount", item.debitGross.ToString());
                        double fbtAmount = Math.Round(Convert.ToDouble(new DataTable().Compute(equation, null)), 2);
                        Console.WriteLine("-=-=-=-=-=->" + equation);
                        credit.amount = fbtAmount;
                        debit.amount = fbtAmount;

                        tempGbase.entries.Add(credit);
                        tempGbase.entries.Add(debit);
                        goExpData = InsertGbaseEntry(tempGbase, expID);
                        goExpHistData = new GOExpressHistModel();
                        list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });
                    }
                }
            }

            _GOContext.SaveChanges();
            _context.SaveChanges();

            List<ExpenseTransList> transactions = new List<ExpenseTransList>();

            foreach (var item in list)
            {
                ExpenseTransList tran = new ExpenseTransList
                {
                    TL_ExpenseID = item.expEntryID,
                    TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
                    TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                    TL_Liquidation = false
                };
                transactions.Add(tran);
            }

            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();

            return true;
        }
        public bool postNC(int expID, string command)
        {
            var expenseDetails = getExpenseNC(expID);
            var list = new[] {
                new { expEntryID = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

            list.Clear();
            foreach (var dtls in expenseDetails.EntryNC.ExpenseEntryNCDtls)
            {
                gbaseContainer tempGbase = new gbaseContainer();

                tempGbase.valDate = expenseDetails.expenseDate;
                tempGbase.remarks = dtls.ExpNCDtl_Remarks_Desc + " " + dtls.ExpNCDtl_Remarks_Period;
                tempGbase.maker = expenseDetails.maker;
                tempGbase.approver = _context.ExpenseEntry.FirstOrDefault(x => x.Expense_ID == expID).Expense_Approver;
                foreach (var item in dtls.ExpenseEntryNCDtlAccs)
                {
                    if (item.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_CREDIT)
                    {
                        entryContainer credit = new entryContainer
                        {
                            type = (command != "R") ? "C" : "D",
                            ccy = item.ExpNCDtlAcc_Curr_ID,
                            amount = item.ExpNCDtlAcc_Amount,
                            account = item.ExpNCDtlAcc_Acc_ID,
                            interate = item.ExpNCDtlAcc_Inter_Rate
                        };
                        tempGbase.entries.Add(credit);
                    }
                    else if (item.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT)
                    {
                        entryContainer debit = new entryContainer
                        {
                            type = (command != "R") ? "D" : "C",
                            ccy = item.ExpNCDtlAcc_Curr_ID,
                            amount = item.ExpNCDtlAcc_Amount,
                            account = item.ExpNCDtlAcc_Acc_ID,
                            interate = item.ExpNCDtlAcc_Inter_Rate
                        };
                        tempGbase.entries.Add(debit);
                    }
                }

                tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type == "D").ToList();
                //insert
                goExpData = InsertGbaseEntry(tempGbase, expID);
                goExpHistData = convertTblCm10ToGOExHist(goExpData, expID, expenseDetails.EntryNC.NC_ID);
                list.Add(new { expEntryID = expID, goExp = goExpData, goExpHist = goExpHistData });
            }

            _GOContext.SaveChanges();
            _context.SaveChanges();

            List<ExpenseTransList> transactions = new List<ExpenseTransList>();

            foreach (var item in list)
            {
                ExpenseTransList tran = new ExpenseTransList
                {
                    TL_ExpenseID = item.expEntryID,
                    TL_GoExpress_ID = int.Parse(item.goExp.Id.ToString()),
                    TL_GoExpHist_ID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                    TL_Liquidation = false
                };
                transactions.Add(tran);
            }

            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();

            return true;
        }
        ///============[End Post Entries]============
        
        ///================[Closing]=================
        public bool closeTransaction(string transactionType, string username, string password)
        {
            string closeCommand = "cm00@E*1@E@E17-@E1210@E";

            TblRequestDetails rqDtl = new TblRequestDetails();
            GwriteTransList gWriteModel = new GwriteTransList();

            closeCommand = closeCommand.Replace("*", transactionType);
            closeCommand = closeCommand.Replace("-", username.Substring(username.Length - 4));

            rqDtl = postToGwrite(closeCommand, username, password);

            gWriteModel.GW_GWrite_ID = int.Parse(rqDtl.RequestId.ToString());
            gWriteModel.GW_Status = "pending";

            return true;
        }
        public ClosingViewModel closeLoadSheet()
        {
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");

            ClosingViewModel closeVM = new ClosingViewModel();

            PettyCashModel pcModel = _context.PettyCash.OrderByDescending(x => x.PC_CloseDate).FirstOrDefault();
            ClosingModel closeModel = _context.Closing.OrderByDescending(x => x.Close_Date).FirstOrDefault();

            closeVM.pettyBegBalance = pcModel.PC_EndBal;
            


            return closeVM;
        }
        ///==============[End Closing]===============

        ///============[Post to GWrite]==============
        public TblRequestDetails postToGwrite(string command, string username, string password)
        {
            TblRequestDetails rqDtlModel = new TblRequestDetails();
            rqDtlModel.tblRequestItems = new List<TblRequestItem>();
            TblRequestItem rqItemModel = new TblRequestItem();

            byte[] asciiBytes = System.Text.Encoding.ASCII.GetBytes(password);
            string encodedPass = "";
            int index = 0;

            foreach (byte b in asciiBytes)
            {
                string hexValue = b.ToString("X");
                encodedPass += hexValue + ",0";
                if (index != asciiBytes.Length - 1)
                    encodedPass += ",";
                index++;
            }

            rqDtlModel.RacfId = username;
            rqDtlModel.RacfPassword = encodedPass;
            rqDtlModel.RequestCreated = DateTime.Now;
            rqDtlModel.Status = "SCRIPTING";
            rqDtlModel.SystemAbbr = "EXPRESS";
            rqDtlModel.Priority = 1;

            rqItemModel.SequenceNo = 1;
            rqItemModel.ReturnFlag = true;
            rqItemModel.Command = command;

            rqDtlModel.tblRequestItems.Add(rqItemModel);

            _gWriteContext.Add(rqDtlModel);
            _gWriteContext.SaveChanges();

            return rqDtlModel;
        }
        ///==========[End Post to Gwrite]============

        ///==============[Begin Gbase Entry Section]================
        private TblCm10 InsertGbaseEntry(gbaseContainer containerModel, int expenseID)
        {
             TblCm10 goModel = new TblCm10();

            //goModel.Id = -1;
            goModel.SystemName = "EXPRESS";
            goModel.Branchno = "767"; //Replace with proper branchNo later
            goModel.AutoApproved = "Y";
            goModel.ValueDate = DateTime.Now.ToString("MMddyy");
            goModel.Section = "10";
            goModel.Remarks = containerModel.remarks;
            goModel.MakerEmpno = containerModel.maker.ToString(); //Replace with user ID later when user module is finished.
            goModel.Empno = containerModel.approver.ToString();  //Replace with user ID later when user module is finished.
            goModel.Recstatus = "READY";
            goModel.Datestamp = DateTime.Now;
            goModel.Timerespond = DateTime.Now;
            goModel.Timesent = DateTime.Now;

            if (containerModel.entries.Count > 0)
            {
                goModel.Entry11Type = containerModel.entries[0].type;
                goModel.Entry11Ccy = GetCurrencyAbbrv(containerModel.entries[0].ccy);
                goModel.Entry11Amt = containerModel.entries[0].amount.ToString();

                var entry11Account = getAccount(containerModel.entries[0].account);
                goModel.Entry11Cust = entry11Account.Account_Cust;
                goModel.Entry11Actcde = entry11Account.Account_Code;
                goModel.Entry11ActType = entry11Account.Account_No.Substring(0, 3);
                goModel.Entry11ActNo = entry11Account.Account_No.Substring(Math.Max(0, entry11Account.Account_No.Length - 6));
                goModel.Entry11ExchRate = containerModel.entries[0].interate.ToString();
                goModel.Entry11ExchCcy = (containerModel.entries[0].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[0].contraCcy) : "";
                goModel.Entry11Fund = (entry11Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                if(containerModel.entries[0].chkNo != null)
                {
                    goModel.Entry11CheckNo = containerModel.entries[0].chkNo;
                }
                goModel.Entry11Available = "";//Replace with proper available default.
                goModel.Entry11Details = "";//Replace with proper details default.
                goModel.Entry11Entity = "10";//Replace with proper entity default.
                goModel.Entry11Division = entry11Account.Account_Div;//Replace with proper division default.
                goModel.Entry11InterRate = (containerModel.entries[0].interate > 0) ? containerModel.entries[0].interate.ToString() : "";//Replace with proper interate default.
                goModel.Entry11InterAmt = "";//Replace with proper interamt default.

                if (containerModel.entries.Count > 1) {
                    goModel.Entry12Type = containerModel.entries[1].type;
                    goModel.Entry12Ccy = GetCurrencyAbbrv(containerModel.entries[1].ccy);
                    goModel.Entry12Amt = containerModel.entries[1].amount.ToString();

                    var entry12Account = getAccount(containerModel.entries[1].account);
                    goModel.Entry12Cust = entry12Account.Account_Cust;
                    goModel.Entry12Actcde = entry12Account.Account_Code;
                    goModel.Entry12ActType = entry12Account.Account_No.Substring(0, 3);
                    goModel.Entry12ActNo = entry12Account.Account_No.Substring(Math.Max(0, entry12Account.Account_No.Length - 6));
                    goModel.Entry12ExchRate = containerModel.entries[1].interate.ToString();
                    goModel.Entry12ExchCcy = (containerModel.entries[1].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[1].contraCcy) : "";
                    goModel.Entry12Fund = (entry12Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    if (containerModel.entries[1].chkNo != null)
                    {
                        goModel.Entry12CheckNo = containerModel.entries[1].chkNo;
                    }
                    goModel.Entry12Available = "";//Replace with proper available default.
                    goModel.Entry12Details = "";//Replace with proper details default.
                    goModel.Entry12Entity = "10";//Replace with proper entity default.
                    goModel.Entry12Division = entry11Account.Account_Div;//Replace with proper division default.
                    goModel.Entry12InterRate = (containerModel.entries[1].interate > 0) ? containerModel.entries[1].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry12InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 2)
                {
                    goModel.Entry21Type = containerModel.entries[2].type;
                    goModel.Entry21Ccy = GetCurrencyAbbrv(containerModel.entries[2].ccy);
                    goModel.Entry21Amt = containerModel.entries[2].amount.ToString();

                    var entry21Account = getAccount(containerModel.entries[2].account);
                    goModel.Entry21Cust = entry21Account.Account_Cust;
                    goModel.Entry21Actcde = entry21Account.Account_Code;
                    goModel.Entry21ActType = entry21Account.Account_No.Substring(0, 3);
                    goModel.Entry21ActNo = entry21Account.Account_No.Substring(Math.Max(0, entry21Account.Account_No.Length - 6));
                    goModel.Entry21ExchRate = containerModel.entries[2].interate.ToString();
                    goModel.Entry21ExchCcy = (containerModel.entries[2].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[2].contraCcy) : "";
                    goModel.Entry21Fund = (entry21Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    if (containerModel.entries[2].chkNo != null)
                    {
                        goModel.Entry21CheckNo = containerModel.entries[2].chkNo;
                    }
                    goModel.Entry21Available = "";//Replace with proper available default.
                    goModel.Entry21Details = "";//Replace with proper details default.
                    goModel.Entry21Entity = "10";//Replace with proper entity default.
                    goModel.Entry21Division = entry11Account.Account_Div;//Replace with proper division default.
                    goModel.Entry21InterRate = (containerModel.entries[2].interate > 0) ? containerModel.entries[2].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry21InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 3)
                {
                    goModel.Entry22Type = containerModel.entries[3].type;
                    goModel.Entry22Ccy = GetCurrencyAbbrv(containerModel.entries[3].ccy);
                    goModel.Entry22Amt = containerModel.entries[3].amount.ToString();

                    var entry22Account = getAccount(containerModel.entries[3].account);
                    goModel.Entry22Cust = entry22Account.Account_Cust;
                    goModel.Entry22Actcde = entry22Account.Account_Code;
                    goModel.Entry22ActType = entry22Account.Account_No.Substring(0, 3);
                    goModel.Entry22ActNo = entry22Account.Account_No.Substring(Math.Max(0, entry22Account.Account_No.Length - 6));
                    goModel.Entry22ExchRate = containerModel.entries[3].interate.ToString();
                    goModel.Entry22ExchCcy = (containerModel.entries[3].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[3].contraCcy) : "";
                    goModel.Entry22Fund = (entry22Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    if (containerModel.entries[3].chkNo != null)
                    {
                        goModel.Entry22CheckNo = containerModel.entries[3].chkNo;
                    }
                    goModel.Entry22Available = "";//Replace with proper available default.
                    goModel.Entry22Details = "";//Replace with proper details default.
                    goModel.Entry22Entity = "10";//Replace with proper entity default.
                    goModel.Entry22Division = entry11Account.Account_Div;//Replace with proper division default.
                    goModel.Entry22InterRate = (containerModel.entries[3].interate > 0) ? containerModel.entries[3].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry22InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 4)
                {
                    goModel.Entry31Type = containerModel.entries[4].type;
                    goModel.Entry31Ccy = GetCurrencyAbbrv(containerModel.entries[4].ccy);
                    goModel.Entry31Amt = containerModel.entries[4].amount.ToString();

                    var entry31Account = getAccount(containerModel.entries[4].account);
                    goModel.Entry31Cust = entry31Account.Account_Cust;
                    goModel.Entry31Actcde = entry31Account.Account_Code;
                    goModel.Entry31ActType = entry31Account.Account_No.Substring(0, 3);
                    goModel.Entry31ActNo = entry31Account.Account_No.Substring(Math.Max(0, entry31Account.Account_No.Length - 6));
                    goModel.Entry31ExchRate = containerModel.entries[4].interate.ToString();
                    goModel.Entry31ExchCcy = (containerModel.entries[4].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[4].contraCcy) : "";
                    goModel.Entry31Fund = (entry31Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    if (containerModel.entries[4].chkNo != null)
                    {
                        goModel.Entry31CheckNo = containerModel.entries[4].chkNo;
                    }
                    goModel.Entry31Available = "";//Replace with proper available default.
                    goModel.Entry31Details = "";//Replace with proper details default.
                    goModel.Entry31Entity = "10";//Replace with proper entity default.
                    goModel.Entry31Division = entry11Account.Account_Div;//Replace with proper division default.
                    goModel.Entry31InterRate = (containerModel.entries[4].interate > 0) ? containerModel.entries[4].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry31InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 5)
                {
                    goModel.Entry32Type = containerModel.entries[5].type;
                    goModel.Entry32Ccy = GetCurrencyAbbrv(containerModel.entries[5].ccy);
                    goModel.Entry32Amt = containerModel.entries[5].amount.ToString();

                    var entry32Account = getAccount(containerModel.entries[5].account);
                    goModel.Entry32Cust = entry32Account.Account_Cust;
                    goModel.Entry32Actcde = entry32Account.Account_Code;
                    goModel.Entry32ActType = entry32Account.Account_No.Substring(0, 3);
                    goModel.Entry32ActNo = entry32Account.Account_No.Substring(Math.Max(0, entry32Account.Account_No.Length - 6));
                    goModel.Entry32ExchRate = containerModel.entries[5].interate.ToString();
                    goModel.Entry32ExchCcy = (containerModel.entries[5].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[5].contraCcy) : "";
                    goModel.Entry32Fund = (entry32Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    if (containerModel.entries[5].chkNo != null)
                    {
                        goModel.Entry32CheckNo = containerModel.entries[5].chkNo;
                    }
                    goModel.Entry32Available = "";//Replace with proper available default.
                    goModel.Entry32Details = "";//Replace with proper details default.
                    goModel.Entry32Entity = "10";//Replace with proper entity default.
                    goModel.Entry32Division = entry11Account.Account_Div;//Replace with proper division default.
                    goModel.Entry32InterRate = (containerModel.entries[5].interate > 0) ? containerModel.entries[5].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry32InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 6)
                {
                    goModel.Entry41Type = containerModel.entries[6].type;
                    goModel.Entry41Ccy = GetCurrencyAbbrv(containerModel.entries[6].ccy);
                    goModel.Entry41Amt = containerModel.entries[6].amount.ToString();

                    var entry41Account = getAccount(containerModel.entries[6].account);
                    goModel.Entry41Cust = entry41Account.Account_Cust;
                    goModel.Entry41Actcde = entry41Account.Account_Code;
                    goModel.Entry41ActType = entry41Account.Account_No.Substring(0, 3);
                    goModel.Entry41ActNo = entry41Account.Account_No.Substring(Math.Max(0, entry41Account.Account_No.Length - 6));
                    goModel.Entry41ExchRate = containerModel.entries[6].interate.ToString();
                    goModel.Entry41ExchCcy = (containerModel.entries[6].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[6].contraCcy) : "";
                    goModel.Entry41Fund = (entry41Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    if (containerModel.entries[6].chkNo != null)
                    {
                        goModel.Entry41CheckNo = containerModel.entries[6].chkNo;
                    }
                    goModel.Entry41Available = "";//Replace with proper available default.
                    goModel.Entry41Details = "";//Replace with proper details default.
                    goModel.Entry41Entity = "10";//Replace with proper entity default.
                    goModel.Entry41Division = entry11Account.Account_Div;//Replace with proper division default.
                    goModel.Entry41InterRate = (containerModel.entries[6].interate > 0) ? containerModel.entries[6].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry41InterAmt = "";//Replace with proper interamt default.
                }
                if (containerModel.entries.Count > 7)
                {
                    goModel.Entry42Type = containerModel.entries[7].type;
                    goModel.Entry42Ccy = GetCurrencyAbbrv(containerModel.entries[7].ccy);
                    goModel.Entry42Amt = containerModel.entries[7].amount.ToString();

                    var entry42Account = getAccount(containerModel.entries[7].account);
                    goModel.Entry42Cust = entry42Account.Account_Cust;
                    goModel.Entry42Actcde = entry42Account.Account_Code;
                    goModel.Entry42ActType = entry42Account.Account_No.Substring(0, 3);
                    goModel.Entry42ActNo = entry42Account.Account_No.Substring(Math.Max(0, entry42Account.Account_No.Length - 6));
                    goModel.Entry42ExchRate = containerModel.entries[7].interate.ToString();
                    goModel.Entry42ExchCcy = (containerModel.entries[7].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[7].contraCcy) : "";
                    goModel.Entry42Fund = (entry42Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    if (containerModel.entries[7].chkNo != null)
                    {
                        goModel.Entry42CheckNo = containerModel.entries[7].chkNo;
                    }
                    goModel.Entry42Available = "";//Replace with proper available default.
                    goModel.Entry42Details = "";//Replace with proper details default.
                    goModel.Entry42Entity = "10";//Replace with proper entity default.
                    goModel.Entry42Division = entry11Account.Account_Div;//Replace with proper division default.
                    goModel.Entry42InterRate = (containerModel.entries[7].interate > 0) ? containerModel.entries[7].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry42InterAmt = "";//Replace with proper interamt default.
                }
            }
            else
            {
                return goModel;
            }

            _GOContext.TblCm10.Add(goModel);
           
            //_context.GOExpressHist.Add(convertTblCm10ToGOExHist(goModel, expenseID, expDtlID));

            return goModel;
        }
        ///===============[End Gbase Entry Section]=================

        ///============[Other Functions]============
        //get fbt id
        public int getFbt(int id)
        {
            return _context.DMFBT.FirstOrDefault(x=>x.FBT_MasterID==id && x.FBT_isActive == true && x.FBT_isDeleted == false).FBT_ID;
        }
        public string getFbtFormula(int id)
        {
            return _context.DMFBT.FirstOrDefault(x=>x.FBT_ID == id).FBT_Formula;
        }
        //get currency
        public DMCurrencyModel getCurrency(int id)
        {
            return _context.DMCurrency.FirstOrDefault(x => x.Curr_ID == id);
        }
        //get vendor
        public DMVendorModel getVendor(int id)
        {
            return _context.DMVendor.FirstOrDefault(x=> x.Vendor_ID == id);
        }
        //get account
        public DMAccountModel getAccount(int id)
        {
            return _context.DMAccount.FirstOrDefault(x => x.Account_ID == id);
        }
        //get account name
        public string GetAccountName(int id)
        {
            return _context.DMAccount.Where(x => x.Account_ID == id).Single().Account_Name;
        }
        //get dept name
        public string GetDeptName(int id)
        {
            return _context.DMDept.Where(x => x.Dept_ID == id).First().Dept_Name;
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
        public float getVat(int id)
        {
            return _context.DMVAT.Where(x => x.VAT_ID == id).First().VAT_Rate;
        }
        //get EWT(Tax Rate) value
        public float GetEWTValue(int id)
        {
            return _context.DMTR.Where(x => x.TR_ID == id).First().TR_Tax_Rate;
        }
        //get currency abbreviation
        public string GetCurrencyAbbrv(int id)
        {
            return (id != 0 ) ? _context.DMCurrency.Where(x => x.Curr_ID == id).First().Curr_CCY_ABBR : "PHP";
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
        //get vendor name
        public string getVendorName(int vendorID)
        {
            var vendor = _context.DMVendor.SingleOrDefault(x => x.Vendor_ID == vendorID);

            if (vendor == null)
            {
                return null;
            }

            return vendor.Vendor_Name;
        }
        public int getAccountID(string accountNo)
        {
            return _context.DMAccount.Where(x => x.Account_No.Replace("-", "") == accountNo.Replace("-", "").Substring(0, 12) && x.Account_isActive == true 
            && x.Account_isDeleted == false).FirstOrDefault().Account_ID;
        }
        public int getCurrencyID(string ccyAbbr)
        {
            return _context.DMCurrency.Where(x => x.Curr_CCY_ABBR == ccyAbbr && x.Curr_isActive == true
            && x.Curr_isDeleted == false).DefaultIfEmpty(new DMCurrencyModel { Curr_ID = 0 }).FirstOrDefault().Curr_ID;
        }
        //retrieve vendor list
        public List<SelectList> getEntrySystemVals()
        {
            List<SelectList> listOfLists = new List<SelectList>();

            listOfLists.Add(new SelectList(_context.DMVendor.Where(x => x.Vendor_isActive == true && x.Vendor_isDeleted == false).Select(q => new { q.Vendor_ID, q.Vendor_Name }),
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
        public ExpenseEntryModel getExpenseDetail(int entryID)
        {
            return _context.ExpenseEntry.Include("ExpenseEntryDetails").Where(x => x.Expense_ID == entryID).FirstOrDefault();
        }

        //Account list only Active and not deleted
        public List<DMAccountModel> getAccountList()
        {
            List<DMAccountModel> accList = new List<DMAccountModel>();
            var acc = _context.DMAccount.Where(x => x.Account_isActive == true 
                        && x.Account_isDeleted == false).ToList().OrderBy(x => x.Account_Name);
            foreach (var i in acc)
            {
                accList.Add(new DMAccountModel
                {
                    Account_ID =  i.Account_ID,
                    Account_Name = i.Account_No + " - " + i.Account_Name,
                    Account_No = i.Account_No
                });
            }

            return accList;
        }

        //Account list including history
        public List<DMAccountModel> getAccountListIncHist()
        {
            return _context.DMAccount.OrderBy(x => x.Account_No).ToList();
        }
        //retrieve latest Express transation no.
        public int getExpTransNo(int transType)
        {
            ExpenseEntryModel transNoMax;
            int transno;
            int maxNumber;
            do
            {
                maxNumber = _context.ExpenseEntry
                                        .Where(y => y.Expense_Date.Year == DateTime.Now.Year && y.Expense_Number != 0)
                                        .Max(y => y.Expense_Number);

                transNoMax = _context.ExpenseEntry.OrderByDescending(x=>x.Expense_ID).First();

                transno = (transNoMax.Expense_Created_Date.Year < DateTime.Now.Year) ? 1
                        : (maxNumber + 1);

            } while (maxNumber != _context.ExpenseEntry
                                        .Where(y => y.Expense_Date.Year == DateTime.Now.Year && y.Expense_Number != 0)
                                        .Max(y => y.Expense_Number));
            _context.Entry<ExpenseEntryModel>(transNoMax).State = EntityState.Detached;
            return transno;
        }


        public GOExpressHistModel convertTblCm10ToGOExHist(TblCm10 tblcm10, int entryID, int entryDtlID)
        {
            var goExpHist =  new GOExpressHistModel {
                GOExpHist_SystemName = tblcm10.SystemName,
                GOExpHist_Groupcode = tblcm10.Groupcode,
                GOExpHist_Branchno = tblcm10.Branchno,
                GOExpHist_OpeKind = tblcm10.OpeKind,
                GOExpHist_AutoApproved = tblcm10.AutoApproved,
                GOExpHist_WarningOverride = tblcm10.WarningOverride,
                GOExpHist_CcyFormat = tblcm10.CcyFormat,
                GOExpHist_OpeBranch = tblcm10.OpeBranch,
                GOExpHist_ValueDate = tblcm10.ValueDate,
                GOExpHist_ReferenceType = tblcm10.ReferenceType,
                GOExpHist_ReferenceNo = tblcm10.ReferenceNo,
                GOExpHist_Comment = tblcm10.Comment,
                GOExpHist_Section = tblcm10.Section,
                GOExpHist_Remarks = tblcm10.Remarks,
                GOExpHist_Memo = tblcm10.Memo,
                GOExpHist_SchemeNo = tblcm10.SchemeNo,
                GOExpHist_Entry11Type = tblcm10.Entry11Type,
                GOExpHist_Entry11IbfCode = tblcm10.Entry11IbfCode,
                GOExpHist_Entry11Ccy = tblcm10.Entry11Ccy,
                GOExpHist_Entry11Amt = tblcm10.Entry11Amt,
                GOExpHist_Entry11Cust = tblcm10.Entry11Cust,
                GOExpHist_Entry11Actcde = tblcm10.Entry11Actcde,
                GOExpHist_Entry11ActType = tblcm10.Entry11ActType,
                GOExpHist_Entry11ActNo = tblcm10.Entry11ActNo,
                GOExpHist_Entry11ExchRate = tblcm10.Entry11ExchRate,
                GOExpHist_Entry11ExchCcy = tblcm10.Entry11ExchCcy,
                GOExpHist_Entry11Fund = tblcm10.Entry11Fund,
                GOExpHist_Entry11CheckNo = tblcm10.Entry11CheckNo,
                GOExpHist_Entry11Available = tblcm10.Entry11Available,
                GOExpHist_Entry11AdvcPrnt = tblcm10.Entry11AdvcPrnt,
                GOExpHist_Entry11Details = tblcm10.Entry11Details,
                GOExpHist_Entry11Entity = tblcm10.Entry11Entity,
                GOExpHist_Entry11Division = tblcm10.Entry11Division,
                GOExpHist_Entry11InterAmt = tblcm10.Entry11InterAmt,
                GOExpHist_Entry11InterRate = tblcm10.Entry11InterRate,
                GOExpHist_Entry12Type = tblcm10.Entry12Type,
                GOExpHist_Entry12IbfCode = tblcm10.Entry12IbfCode,
                GOExpHist_Entry12Ccy = tblcm10.Entry12Ccy,
                GOExpHist_Entry12Amt = tblcm10.Entry12Amt,
                GOExpHist_Entry12Cust = tblcm10.Entry12Cust,
                GOExpHist_Entry12Actcde = tblcm10.Entry12Actcde,
                GOExpHist_Entry12ActType = tblcm10.Entry12ActType,
                GOExpHist_Entry12ActNo = tblcm10.Entry12ActNo,
                GOExpHist_Entry12ExchRate = tblcm10.Entry12ExchRate,
                GOExpHist_Entry12ExchCcy = tblcm10.Entry12ExchCcy,
                GOExpHist_Entry12Fund = tblcm10.Entry12Fund,
                GOExpHist_Entry12CheckNo = tblcm10.Entry12CheckNo,
                GOExpHist_Entry12Available = tblcm10.Entry12Available,
                GOExpHist_Entry12AdvcPrnt = tblcm10.Entry12AdvcPrnt,
                GOExpHist_Entry12Details = tblcm10.Entry12Details,
                GOExpHist_Entry12Entity = tblcm10.Entry12Entity,
                GOExpHist_Entry12Division = tblcm10.Entry12Division,
                GOExpHist_Entry12InterAmt = tblcm10.Entry12InterAmt,
                GOExpHist_Entry12InterRate = tblcm10.Entry12InterRate,
                GOExpHist_Entry21Type = tblcm10.Entry21Type,
                GOExpHist_Entry21IbfCode = tblcm10.Entry21IbfCode,
                GOExpHist_Entry21Ccy = tblcm10.Entry21Ccy,
                GOExpHist_Entry21Amt = tblcm10.Entry21Amt,
                GOExpHist_Entry21Cust = tblcm10.Entry21Cust,
                GOExpHist_Entry21Actcde = tblcm10.Entry21Actcde,
                GOExpHist_Entry21ActType = tblcm10.Entry21ActType,
                GOExpHist_Entry21ActNo = tblcm10.Entry21ActNo,
                GOExpHist_Entry21ExchRate = tblcm10.Entry21ExchRate,
                GOExpHist_Entry21ExchCcy = tblcm10.Entry21ExchCcy,
                GOExpHist_Entry21Fund = tblcm10.Entry21Fund,
                GOExpHist_Entry21CheckNo = tblcm10.Entry21CheckNo,
                GOExpHist_Entry21Available = tblcm10.Entry21Available,
                GOExpHist_Entry21AdvcPrnt = tblcm10.Entry21AdvcPrnt,
                GOExpHist_Entry21Details = tblcm10.Entry21Details,
                GOExpHist_Entry21Entity = tblcm10.Entry21Entity,
                GOExpHist_Entry21Division = tblcm10.Entry21Division,
                GOExpHist_Entry21InterAmt = tblcm10.Entry21InterAmt,
                GOExpHist_Entry21InterRate = tblcm10.Entry21InterRate,
                GOExpHist_Entry22Type = tblcm10.Entry22Type,
                GOExpHist_Entry22IbfCode = tblcm10.Entry22IbfCode,
                GOExpHist_Entry22Ccy = tblcm10.Entry22Ccy,
                GOExpHist_Entry22Amt = tblcm10.Entry22Amt,
                GOExpHist_Entry22Cust = tblcm10.Entry22Cust,
                GOExpHist_Entry22Actcde = tblcm10.Entry22Actcde,
                GOExpHist_Entry22ActType = tblcm10.Entry22ActType,
                GOExpHist_Entry22ActNo = tblcm10.Entry22ActNo,
                GOExpHist_Entry22ExchRate = tblcm10.Entry22ExchRate,
                GOExpHist_Entry22ExchCcy = tblcm10.Entry22ExchCcy,
                GOExpHist_Entry22Fund = tblcm10.Entry22Fund,
                GOExpHist_Entry22CheckNo = tblcm10.Entry22CheckNo,
                GOExpHist_Entry22Available = tblcm10.Entry22Available,
                GOExpHist_Entry22AdvcPrnt = tblcm10.Entry22AdvcPrnt,
                GOExpHist_Entry22Details = tblcm10.Entry22Details,
                GOExpHist_Entry22Entity = tblcm10.Entry22Entity,
                GOExpHist_Entry22Division = tblcm10.Entry22Division,
                GOExpHist_Entry22InterAmt = tblcm10.Entry22InterAmt,
                GOExpHist_Entry22InterRate = tblcm10.Entry22InterRate,
                GOExpHist_Entry31Type = tblcm10.Entry31Type,
                GOExpHist_Entry31IbfCode = tblcm10.Entry31IbfCode,
                GOExpHist_Entry31Ccy = tblcm10.Entry31Ccy,
                GOExpHist_Entry31Amt = tblcm10.Entry31Amt,
                GOExpHist_Entry31Cust = tblcm10.Entry31Cust,
                GOExpHist_Entry31Actcde = tblcm10.Entry31Actcde,
                GOExpHist_Entry31ActType = tblcm10.Entry31ActType,
                GOExpHist_Entry31ActNo = tblcm10.Entry31ActNo,
                GOExpHist_Entry31ExchRate = tblcm10.Entry31ExchRate,
                GOExpHist_Entry31ExchCcy = tblcm10.Entry31ExchCcy,
                GOExpHist_Entry31Fund = tblcm10.Entry31Fund,
                GOExpHist_Entry31CheckNo = tblcm10.Entry31CheckNo,
                GOExpHist_Entry31Available = tblcm10.Entry31Available,
                GOExpHist_Entry31AdvcPrnt = tblcm10.Entry31AdvcPrnt,
                GOExpHist_Entry31Details = tblcm10.Entry31Details,
                GOExpHist_Entry31Entity = tblcm10.Entry31Entity,
                GOExpHist_Entry31Division = tblcm10.Entry31Division,
                GOExpHist_Entry31InterAmt = tblcm10.Entry31InterAmt,
                GOExpHist_Entry31InterRate = tblcm10.Entry31InterRate,
                GOExpHist_Entry32Type = tblcm10.Entry32Type,
                GOExpHist_Entry32IbfCode = tblcm10.Entry32IbfCode,
                GOExpHist_Entry32Ccy = tblcm10.Entry32Ccy,
                GOExpHist_Entry32Amt = tblcm10.Entry32Amt,
                GOExpHist_Entry32Cust = tblcm10.Entry32Cust,
                GOExpHist_Entry32Actcde = tblcm10.Entry32Actcde,
                GOExpHist_Entry32ActType = tblcm10.Entry32ActType,
                GOExpHist_Entry32ActNo = tblcm10.Entry32ActNo,
                GOExpHist_Entry32ExchRate = tblcm10.Entry32ExchRate,
                GOExpHist_Entry32ExchCcy = tblcm10.Entry32ExchCcy,
                GOExpHist_Entry32Fund = tblcm10.Entry32Fund,
                GOExpHist_Entry32CheckNo = tblcm10.Entry32CheckNo,
                GOExpHist_Entry32Available = tblcm10.Entry32Available,
                GOExpHist_Entry32AdvcPrnt = tblcm10.Entry32AdvcPrnt,
                GOExpHist_Entry32Details = tblcm10.Entry32Details,
                GOExpHist_Entry32Entity = tblcm10.Entry32Entity,
                GOExpHist_Entry32Division = tblcm10.Entry32Division,
                GOExpHist_Entry32InterAmt = tblcm10.Entry32InterAmt,
                GOExpHist_Entry32InterRate = tblcm10.Entry32InterRate,
                GOExpHist_Entry41Type = tblcm10.Entry41Type,
                GOExpHist_Entry41IbfCode = tblcm10.Entry41IbfCode,
                GOExpHist_Entry41Ccy = tblcm10.Entry41Ccy,
                GOExpHist_Entry41Amt = tblcm10.Entry41Amt,
                GOExpHist_Entry41Cust = tblcm10.Entry41Cust,
                GOExpHist_Entry41Actcde = tblcm10.Entry41Actcde,
                GOExpHist_Entry41ActType = tblcm10.Entry41ActType,
                GOExpHist_Entry41ActNo = tblcm10.Entry41ActNo,
                GOExpHist_Entry41ExchRate = tblcm10.Entry41ExchRate,
                GOExpHist_Entry41ExchCcy = tblcm10.Entry41ExchCcy,
                GOExpHist_Entry41Fund = tblcm10.Entry41Fund,
                GOExpHist_Entry41CheckNo = tblcm10.Entry41CheckNo,
                GOExpHist_Entry41Available = tblcm10.Entry41Available,
                GOExpHist_Entry41AdvcPrnt = tblcm10.Entry41AdvcPrnt,
                GOExpHist_Entry41Details = tblcm10.Entry41Details,
                GOExpHist_Entry41Entity = tblcm10.Entry41Entity,
                GOExpHist_Entry41Division = tblcm10.Entry41Division,
                GOExpHist_Entry41InterAmt = tblcm10.Entry41InterAmt,
                GOExpHist_Entry41InterRate = tblcm10.Entry41InterRate,
                GOExpHist_Entry42Type = tblcm10.Entry42Type,
                GOExpHist_Entry42IbfCode = tblcm10.Entry42IbfCode,
                GOExpHist_Entry42Ccy = tblcm10.Entry42Ccy,
                GOExpHist_Entry42Amt = tblcm10.Entry42Amt,
                GOExpHist_Entry42Cust = tblcm10.Entry42Cust,
                GOExpHist_Entry42Actcde = tblcm10.Entry42Actcde,
                GOExpHist_Entry42ActType = tblcm10.Entry42ActType,
                GOExpHist_Entry42ActNo = tblcm10.Entry42ActNo,
                GOExpHist_Entry42ExchRate = tblcm10.Entry42ExchRate,
                GOExpHist_Entry42ExchCcy = tblcm10.Entry42ExchCcy,
                GOExpHist_Entry42Fund = tblcm10.Entry42Fund,
                GOExpHist_Entry42CheckNo = tblcm10.Entry42CheckNo,
                GOExpHist_Entry42Available = tblcm10.Entry42Available,
                GOExpHist_Entry42AdvcPrnt = tblcm10.Entry42AdvcPrnt,
                GOExpHist_Entry42Details = tblcm10.Entry42Details,
                GOExpHist_Entry42Entity = tblcm10.Entry42Entity,
                GOExpHist_Entry42Division = tblcm10.Entry42Division,
                GOExpHist_Entry42InterAmt = tblcm10.Entry42InterAmt,
                GOExpHist_Entry42InterRate = tblcm10.Entry42InterRate,
                ExpenseEntryID = entryID,
                ExpenseDetailID = entryDtlID
            };

            _context.GOExpressHist.Add(goExpHist);

            return goExpHist;
        }
        ///========[End of Other Functions]============
    }

    internal class gbaseContainer
    {
        public DateTime valDate { get; set; }
        public string remarks { get; set; }
        public int maker { get; set; }
        public int approver { get; set; }
        public List<entryContainer> entries { get; set; }

        public gbaseContainer()
        {
            entries = new List<entryContainer>();
        }
    }

    internal class entryContainer
    {
        public string type { get; set; }
        public int ccy { get; set; }
        public double amount { get; set; }
        public int vendor { get; set; }
        public int account { get; set; }
        public double interate { get; set; }
        public int contraCcy { get; set; }
        public string chkNo { get; set; }
        public int dept { get; set; }
    }
}
