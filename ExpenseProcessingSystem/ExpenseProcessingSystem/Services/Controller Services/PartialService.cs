using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Search_Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ExpenseProcessingSystem.Services.Controller_Services
{
    public class PartialService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public PartialService(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        //-----------------------------------Populate-------------------------------------//
        public List<DMVendorViewModel> populateVendor(DMFiltersViewModel filters)
        {
            IQueryable<DMVendorModel> mList = _context.DMVendor.Where(x=>x.Vendor_isDeleted == false && x.Vendor_isActive == true).ToList().AsQueryable();
            var properties = filters.PF.GetType().GetProperties();

            var pendingList = _context.DMVendor_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMVendorModel[] {
                    new DMVendorModel
                    {
                        Vendor_ID = m.Pending_ID,
                        Vendor_MasterID = m.Pending_Vendor_MasterID,
                        Vendor_Name = m.Pending_Vendor_Name,
                        Vendor_TIN = m.Pending_Vendor_TIN,
                        Vendor_Address = m.Pending_Vendor_Address,
                        Vendor_Creator_ID = m.Pending_Vendor_Creator_ID,
                        Vendor_Approver_ID = m.Pending_Vendor_Approver_ID.Equals(null) ? 0 : m.Pending_Vendor_Approver_ID,
                        Vendor_Created_Date = m.Pending_Vendor_Filed_Date,
                        Vendor_Last_Updated = m.Pending_Vendor_Filed_Date,
                        Vendor_Status_ID = m.Pending_Vendor_Status_ID
                    }
                });
            }

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.PF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(subStr)) // IF INT VAL
                        {
                            mList = mList.Where("Vendor_" + subStr + " = @0 AND  Vendor_isDeleted == @1", property.GetValue(filters.PF), false)
                                     .Select(e => e).AsQueryable();
                        }else if (subStr == "Creator" || subStr == "Approver")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.PF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.PF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            if (subStr == "Approver")
                            {
                                mList = mList.Where(x => names.Contains(x.Vendor_Approver_ID) && x.Vendor_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }else if (subStr == "Creator")
                            {
                                mList = mList.Where(x=> names.Contains(x.Vendor_Creator_ID) &&  x.Vendor_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                           
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Vendor_"+subStr+ ".Contains(@0) AND  Vendor_isDeleted == @1", property.GetValue(filters.PF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }

            List<int> mListIDs = mList.Select(x => x.Vendor_MasterID).ToList();
            var userList = (from a in mList
                               from b in _context.User.Where(x=> a.Vendor_Creator_ID == x.User_ID).DefaultIfEmpty()
                               from c in _context.User.Where(x => a.Vendor_Approver_ID == x.User_ID).DefaultIfEmpty()
                               select new
                               { a.Vendor_MasterID,
                                 CreatorName = b.User_LName + ", " + b.User_FName,
                                 ApproverName = (c == null) ? "" : c.User_LName + ", " + c.User_FName }).ToList();
            //var vtvList = (from m in mList
            //                  from vtv in _context.DMVendorTRVAT_Pending.Where(x=> x.Pending_VTV_Vendor_ID == m.Vendor_ID).DefaultIfEmpty()
            //                  select new
            //                  {
            //                      vtv.Pending_VTV_ID,
            //                      vtv.Pending_VTV_Vendor_ID,
            //                      vtv.Pending_VTV_TR_ID,
            //                      vtv.Pending_VTV_VAT_ID,
            //                  }).ToList();

            //var taxRatesList = (from a in vtvList
            //                    from tr in _context.DMTR.Where(x => x.TR_MasterID == a.Pending_VTV_TR_ID && x.TR_isActive == true).DefaultIfEmpty()
            //                    from trPen in _context.DMTR_Pending.Where(x => x.Pending_TR_MasterID == a.Pending_VTV_TR_ID && x.Pending_TR_isActive == true).DefaultIfEmpty()
            //                    select new
            //                    {
            //                        vendId = a.Pending_VTV_Vendor_ID,
            //                        trId = (tr == null || trPen == null) ? 0 : (tr != null) && (tr.TR_MasterID != 0) ? tr.TR_MasterID : trPen.Pending_TR_MasterID,
            //                        trRate = (tr == null || trPen == null) ? 0 : (tr != null) && (tr.TR_Tax_Rate != 0) ? tr.TR_Tax_Rate : trPen.Pending_TR_Tax_Rate,
            //                        trTitle = (tr == null || trPen == null) ? "" : (tr != null) && (tr.TR_WT_Title != "") ? tr.TR_WT_Title : trPen.Pending_TR_WT_Title

            //                    }).ToList();

            var trList = (from a in _context.DMVendorTRVAT
                          join c in _context.DMTR on a.VTV_TR_ID equals c.TR_MasterID
                          where mListIDs.Contains(a.VTV_Vendor_ID) && c.TR_isActive == true
                          select new { a.VTV_Vendor_ID, c.TR_ID, c.TR_Tax_Rate, c.TR_WT_Title }).ToList();

            var trpendingList = (from a in _context.DMVendorTRVAT_Pending
                                 join c in _context.DMTR on a.Pending_VTV_TR_ID equals c.TR_MasterID
                                 where mListIDs.Contains(a.Pending_VTV_Vendor_ID) && c.TR_isActive == true
                                 select new { a.Pending_VTV_Vendor_ID, c.TR_ID, c.TR_Tax_Rate, c.TR_WT_Title }).ToList();

            var vatList = (from a in _context.DMVendorTRVAT
                          join d in _context.DMVAT on a.VTV_VAT_ID equals d.VAT_MasterID
                           where mListIDs.Contains(a.VTV_Vendor_ID) && d.VAT_isActive == true
                           select new { a.VTV_Vendor_ID, d.VAT_ID, d.VAT_Rate, d.VAT_Name }).ToList();

            var vatpendingList = (from a in _context.DMVendorTRVAT_Pending
                           join d in _context.DMVAT on a.Pending_VTV_VAT_ID equals d.VAT_MasterID
                                  where mListIDs.Contains(a.Pending_VTV_Vendor_ID) && d.VAT_isActive == true
                                  select new { a.Pending_VTV_Vendor_ID, d.VAT_ID, d.VAT_Rate, d.VAT_Name }).ToList();

            var statusList = (from a in mList
                              join c in _context.StatusList on a.Vendor_Status_ID equals c.Status_ID
                              select new { a.Vendor_ID, a.Vendor_MasterID, c.Status_Name }).ToList();
            //assign values
            List<DMVendorModel> mList2 = mList.ToList();
            List <DMVendorViewModel> vmList = new List<DMVendorViewModel>();
            foreach (DMVendorModel m in mList2)
            {
                DMVendorViewModel vm = new DMVendorViewModel
                {
                    Vendor_MasterID = m.Vendor_MasterID,
                    Vendor_Name = m.Vendor_Name,
                    Vendor_TIN = m.Vendor_TIN,
                    Vendor_Address = m.Vendor_Address,
                    Vendor_Creator_Name = userList.Where(a => a.Vendor_MasterID == m.Vendor_MasterID).Select(a => a.CreatorName).FirstOrDefault() ?? "N/A",
                    Vendor_Approver_Name = userList.Where(a => a.Vendor_MasterID == m.Vendor_MasterID).Select(a => a.ApproverName).FirstOrDefault() ?? "",
                    Vendor_Created_Date = m.Vendor_Created_Date,
                    Vendor_Last_Updated = m.Vendor_Last_Updated,
                    Vendor_Tax_Rates = new List<string>(),
                    Vendor_VAT = new List<string>(),
                    Vendor_Status_ID = m.Vendor_Status_ID,
                    Vendor_Status = statusList.Where(a => a.Vendor_ID == m.Vendor_ID).Select(a => a.Status_Name).FirstOrDefault() ?? "N/A"
                };
                //temp -- in case the entry is an Approved record
                if (vm.Vendor_Status_ID != 3)
                {
                    //add values from pending VTV List
                    vm.Vendor_Tax_Rates.AddRange(trpendingList.Where(x => x.Pending_VTV_Vendor_ID == m.Vendor_MasterID).Select(x => (x.TR_Tax_Rate * 100) + "% - " + x.TR_WT_Title).ToList());
                    vm.Vendor_VAT.AddRange(vatpendingList.Where(x => x.Pending_VTV_Vendor_ID == m.Vendor_MasterID).Select(x => (x.VAT_Rate * 100) + "% - " + x.VAT_Name).ToList());
                }
                else {
                    vm.Vendor_Tax_Rates = trList.Where(x => x.VTV_Vendor_ID == m.Vendor_MasterID).Select(x => (x.TR_Tax_Rate * 100) + "% - " + x.TR_WT_Title).ToList();
                    vm.Vendor_VAT = vatList.Where(x => x.VTV_Vendor_ID == m.Vendor_MasterID).Select(x => (x.VAT_Rate * 100) + "% - " + x.VAT_Name).ToList();
                }
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMDeptViewModel> populateDept(DMFiltersViewModel filters)
        {
            IQueryable<DMDeptModel> mList = _context.DMDept.Where(x => x.Dept_isDeleted == false && x.Dept_isActive == true).ToList().AsQueryable();
            var pendingList = _context.DMDept_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMDeptModel[] {
                    new DMDeptModel
                    {
                        Dept_MasterID = m.Pending_Dept_MasterID,
                        Dept_Name = m.Pending_Dept_Name,
                        Dept_Code = m.Pending_Dept_Code,
                        Dept_Budget_Unit = m.Pending_Dept_Budget_Unit,
                        Dept_Creator_ID = m.Pending_Dept_Creator_ID,
                        Dept_Approver_ID = m.Pending_Dept_Approver_ID.Equals(null) ? 0 : m.Pending_Dept_Approver_ID,
                        Dept_Created_Date = m.Pending_Dept_Filed_Date,
                        Dept_Last_Updated = m.Pending_Dept_Filed_Date,
                        Dept_Status_ID = m.Pending_Dept_Status_ID
                    }
                });
            }
            var properties = filters.DF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string subStr = propertyName.Substring(propertyName.IndexOf('_') + 1);
                var toStr = property.GetValue(filters.DF).ToString();
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        { 
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.DF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.DF).ToString())))
                              .Select(x => x.User_ID).ToList(); //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.DF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver")
                            {
                                mList = mList.Where(x => names.Contains(x.Dept_Approver_ID) && x.Dept_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator")
                            {
                                mList = mList.Where(x => names.Contains(x.Dept_Creator_ID) && x.Dept_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.Dept_Status_ID) && x.Dept_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Dept_" + subStr + ".Contains(@0) AND  Dept_isDeleted == @1", property.GetValue(filters.DF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var creatorList = (from a in mList
                          join b in _context.User on a.Dept_Creator_ID equals b.User_ID
                          let CreatorName = b.User_LName + ", " + b.User_FName
                          select new { a.Dept_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                          join c in _context.User on a.Dept_Approver_ID equals c.User_ID
                          let ApproverName = c.User_LName + ", " + c.User_FName
                          select new { a.Dept_ID, ApproverName }).ToList();
            var statusList = (from a in mList
                              join c in _context.StatusList on a.Dept_Status_ID equals c.Status_ID
                              select new { a.Dept_ID, c.Status_Name }).ToList();

            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            foreach (DMDeptModel m in mList)
            {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_MasterID = m.Dept_MasterID,
                    Dept_Name = m.Dept_Name,
                    Dept_Code = m.Dept_Code,
                    Dept_Budget_Unit = m.Dept_Budget_Unit,
                    Dept_Creator_Name = creatorList.Where(a => a.Dept_ID == m.Dept_ID).Select(a => a.CreatorName).FirstOrDefault() ?? "N/A",
                    Dept_Approver_Name = apprvrList.Where(a => a.Dept_ID == m.Dept_ID).Select(a => a.ApproverName).FirstOrDefault() ?? "",
                    Dept_Created_Date = m.Dept_Created_Date,
                    Dept_Last_Updated = m.Dept_Last_Updated,
                    Dept_Status_ID = m.Dept_Status_ID,
                    Dept_Status = statusList.Where(a => a.Dept_ID == m.Dept_ID).Select(a => a.Status_Name).FirstOrDefault() ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMCheckViewModel> populateCheck(DMFiltersViewModel filters)
        {
            IQueryable<DMCheckModel> mList = _context.DMCheck.Where(x => x.Check_isDeleted == false && x.Check_isActive == true).ToList().AsQueryable();
            var pendingList = _context.DMCheck_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMCheckModel[] {
                    new DMCheckModel
                    {
                        Check_MasterID = m.Pending_Check_MasterID,
                        Check_Input_Date = m.Pending_Check_Input_Date,
                        Check_Series_From = m.Pending_Check_Series_From,
                        Check_Series_To = m.Pending_Check_Series_To,
                        Check_Bank_Info = m.Pending_Check_Bank_Info,
                        Check_Creator_ID = m.Pending_Check_Creator_ID,
                        Check_Approver_ID = m.Pending_Check_Approver_ID.Equals(null) ? 0 : m.Pending_Check_Approver_ID,
                        Check_Created_Date = m.Pending_Check_Filed_Date,
                        Check_Last_Updated = m.Pending_Check_Filed_Date,
                        Check_Status_ID = m.Pending_Check_Status_ID
                    }
                });
            }
            var properties = filters.CKF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.CKF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "" && toStr != "0001/01/01 0:00:00")
                {
                    if (toStr != "0")
                    {
                        if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.CKF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.CKF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.CKF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver")
                            {
                                mList = mList.Where(x => names.Contains(x.Check_Approver_ID) && x.Check_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator")
                            {
                                mList = mList.Where(x => names.Contains(x.Check_Creator_ID) && x.Check_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.Check_Status_ID) && x.Check_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else if (subStr == "Input_Date")
                        {
                            mList = mList.Where("Check_" + subStr + ".Date.ToString() == @0", toStr)
                                   .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Check_" + subStr + ".Contains(@0) AND  Check_isDeleted == @1", toStr, false)
                                   .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            
            var creatorList = (from a in mList
                               join b in _context.User on a.Check_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new { a.Check_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.Check_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new { a.Check_ID, ApproverName }).ToList();
            var statusList = (from a in mList
                              join c in _context.StatusList on a.Check_Status_ID equals c.Status_ID
                              select new { a.Check_ID, c.Status_Name }).ToList();

            List<DMCheckViewModel> vmList = new List<DMCheckViewModel>();
            foreach (var m in mList)
            {
                DMCheckViewModel vm = new DMCheckViewModel
                {
                    Check_MasterID = m.Check_MasterID,
                    Check_Input_Date = m.Check_Input_Date,
                    Check_Series_From = m.Check_Series_From,
                    Check_Series_To = m.Check_Series_To,
                    Check_Bank_Info = m.Check_Bank_Info,
                    Check_Creator_Name = creatorList.Where(a => a.Check_ID == m.Check_ID).Select(a => a.CreatorName).FirstOrDefault() ?? "N/A",
                    Check_Approver_Name = apprvrList.Where(a => a.Check_ID == m.Check_ID).Select(a => a.ApproverName).FirstOrDefault() ?? "",
                    Check_Created_Date = m.Check_Created_Date,
                    Check_Last_Updated = m.Check_Last_Updated,
                    Check_Status_ID = m.Check_Status_ID,
                    Check_Status = statusList.Where(a => a.Check_ID == m.Check_ID).Select(a => a.Status_Name).FirstOrDefault() ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMAccountViewModel> populateAccount(DMFiltersViewModel filters)
        {
            IQueryable<DMAccountModel> mList = _context.DMAccount.Where(x => x.Account_isDeleted == false && x.Account_isActive == true).ToList().AsQueryable();
            var properties = filters.AF.GetType().GetProperties();

            var pendingList = _context.DMAccount_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMAccountModel[] {
                    new DMAccountModel
                    {
                        Account_MasterID = m.Pending_Account_MasterID,
                        Account_Name = m.Pending_Account_Name,
                        Account_Code = m.Pending_Account_Code,
                        Account_No = m.Pending_Account_No,
                        Account_Cust = m.Pending_Account_Cust,
                        Account_Div = m.Pending_Account_Div,
                        Account_Fund = m.Pending_Account_Fund,
                        Account_FBT_MasterID = m.Pending_Account_FBT_MasterID,
                        Account_Group_MasterID = m.Pending_Account_Group_MasterID,
                        Account_Creator_ID = m.Pending_Account_Creator_ID,
                        Account_Approver_ID = m.Pending_Account_Approver_ID.Equals(null) ? 0 : m.Pending_Account_Approver_ID,
                        Account_Created_Date = m.Pending_Account_Filed_Date,
                        Account_Last_Updated = m.Pending_Account_Filed_Date,
                        Account_Status_ID = m.Pending_Account_Status_ID
                    }
                });
            }
            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.AF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(subStr)) // IF INT VAL
                        {
                            mList = mList.Where("Account_" + subStr + " = @0 AND  Account_isDeleted == @1", property.GetValue(filters.AF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status" || subStr == "Group")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.AF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            //get all account group IDs that contains string
                            var grp = _context.DMAccountGroup
                              .Where(x => (x.AccountGroup_Name.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.AccountGroup_MasterID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Account_Approver_ID) && x.Account_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Account_Creator_ID) && x.Account_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.Account_Status_ID) && x.Account_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Group")
                            {
                                mList = mList.Where(x => grp.Contains(x.Account_Group_MasterID) && x.Account_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else if (subStr == "FBT")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.DMFBT
                              .Where(x => (x.FBT_Name.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.FBT_MasterID).ToList();
                            mList = mList.Where(x => names.Contains(x.Account_FBT_MasterID) && x.Account_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Account_" + subStr + ".Contains(@0) AND  Account_isDeleted == @1", property.GetValue(filters.AF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var creatorList = (from a in mList
                               join b in _context.User on a.Account_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new { a.Account_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.Account_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new { a.Account_ID, ApproverName }).ToList();
            var fbtList = (from a in mList
                           join d in _context.DMFBT on a.Account_FBT_MasterID equals d.FBT_MasterID
                           select new { a.Account_ID, d.FBT_Name }).ToList();
            var groupList = (from a in mList
                           join d in _context.DMAccountGroup on a.Account_Group_MasterID equals d.AccountGroup_MasterID
                             select new { a.Account_ID, d.AccountGroup_Name }).ToList();
            var statList = (from a in mList
                             join d in _context.StatusList on a.Account_Status_ID equals d.Status_ID
                             select new { a.Account_ID, d.Status_Name }).ToList();
            //TEMP where clause until FBT is updated
            var defaultFBT = _context.DMFBT.Where(x => x.FBT_MasterID == 1).Select(x=> x.FBT_Name).FirstOrDefault();

            List<DMAccountViewModel> vmList = new List<DMAccountViewModel>();
            foreach (DMAccountModel m in mList)
            {
                DMAccountViewModel vm = new DMAccountViewModel
                {
                    Account_MasterID = m.Account_MasterID,
                    Account_Name = m.Account_Name,
                    Account_Code = m.Account_Code,
                    Account_No = m.Account_No,
                    Account_Cust = m.Account_Cust,
                    Account_Div = m.Account_Div,
                    Account_Fund = m.Account_Fund,
                    Account_Group_Name = groupList.Where(a => a.Account_ID == m.Account_ID).Select(a => a.AccountGroup_Name).FirstOrDefault() ?? "",
                    Account_FBT_Name = fbtList.Where(a => a.Account_ID == m.Account_ID).Select(a => a.FBT_Name).FirstOrDefault() ?? defaultFBT,
                    Account_Creator_Name = creatorList.Where(a => a.Account_ID == m.Account_ID).Select(a => a.CreatorName).FirstOrDefault() ?? "N/A",
                    Account_Approver_Name = apprvrList.Where(a => a.Account_ID == m.Account_ID).Select(a => a.ApproverName).FirstOrDefault() ?? "",
                    Account_Created_Date = m.Account_Created_Date,
                    Account_Last_Updated = m.Account_Last_Updated,
                    Account_Status_ID = m.Account_Status_ID,
                    Account_Status = statList.Where(a => a.Account_ID == m.Account_ID).Select(a => a.Status_Name).FirstOrDefault() ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMVATViewModel> populateVAT(DMFiltersViewModel filters)
        {
            IQueryable<DMVATModel> mList = _context.DMVAT.Where(x => x.VAT_isDeleted == false && x.VAT_isActive == true).ToList().AsQueryable();
            var properties = filters.VF.GetType().GetProperties();

            var pendingList = _context.DMVAT_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMVATModel[] {
                    new DMVATModel
                    {
                       // VAT_ID = m.Pending_VAT_MasterID,
                        VAT_MasterID = m.Pending_VAT_MasterID,
                        VAT_Name = m.Pending_VAT_Name,
                        VAT_Rate = m.Pending_VAT_Rate,
                        VAT_Creator_ID = m.Pending_VAT_Creator_ID,
                        VAT_Approver_ID = m.Pending_VAT_Approver_ID.Equals(null) ? 0 : m.Pending_VAT_Approver_ID,
                        VAT_Created_Date = m.Pending_VAT_Filed_Date,
                        VAT_Last_Updated = m.Pending_VAT_Filed_Date,
                        VAT_Status_ID = m.Pending_VAT_Status_ID
                    }
                });
            }
            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.VF).ToString();
                string[] colArr = { "Rate", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(subStr)) // IF INT VAL
                        {
                            mList = mList.Where("VAT_" + subStr + " = @0 AND  VAT_isDeleted == @1", property.GetValue(filters.VF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.VF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.VF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.VF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.VAT_Approver_ID) && x.VAT_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.VAT_Creator_ID) && x.VAT_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.VAT_Status_ID) && x.VAT_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("VAT_" + subStr + ".Contains(@0) AND  VAT_isDeleted == @1", property.GetValue(filters.VF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var creatorList = (from a in mList
                               join b in _context.User on a.VAT_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new { a.VAT_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.VAT_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new { a.VAT_ID, ApproverName }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.VAT_Status_ID equals d.Status_ID
                            select new { a.VAT_ID, d.Status_Name }).ToList();

            List<DMVATViewModel> vmList = new List<DMVATViewModel>();
            foreach (DMVATModel m in mList)
            {
                var creator = creatorList.Where(a => a.VAT_ID == m.VAT_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.VAT_ID == m.VAT_ID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.VAT_ID == m.VAT_ID).Select(a => a.Status_Name).FirstOrDefault();
                DMVATViewModel vm = new DMVATViewModel
                {
                    VAT_MasterID = m.VAT_MasterID,
                    VAT_Name = m.VAT_Name,
                    VAT_Rate = m.VAT_Rate,
                    VAT_Creator_Name = creator ?? "N/A",
                    VAT_Approver_Name = approver ?? "",
                    VAT_Created_Date = m.VAT_Created_Date,
                    VAT_Last_Updated = m.VAT_Last_Updated,
                    VAT_Status_ID = m.VAT_Status_ID,
                    VAT_Status = stat ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMFBTViewModel> populateFBT(DMFiltersViewModel filters)
        {
            IQueryable<DMFBTModel> mList = _context.DMFBT.Where(x => x.FBT_isDeleted == false && x.FBT_isActive == true).ToList().AsQueryable();
            var properties = filters.FF.GetType().GetProperties();

            var pendingList = _context.DMFBT_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMFBTModel[] {
                    new DMFBTModel
                    {
                        FBT_MasterID = m.Pending_FBT_MasterID,
                        FBT_Name = m.Pending_FBT_Name,
                        FBT_Tax_Rate = m.Pending_FBT_Tax_Rate,
                        FBT_Formula = m.Pending_FBT_Formula,
                        FBT_Creator_ID = m.Pending_FBT_Creator_ID,
                        FBT_Approver_ID = m.Pending_FBT_Approver_ID.Equals(null) ? 0 : m.Pending_FBT_Approver_ID,
                        FBT_Created_Date = m.Pending_FBT_Filed_Date,
                        FBT_Last_Updated = m.Pending_FBT_Filed_Date,
                        FBT_Status_ID = m.Pending_FBT_Status_ID
                    }
                });
            }
            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.FF).ToString();
                string[] colArr = { "Tax_Rate", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(subStr)) // IF INT VAL
                        {
                            mList = mList.Where("FBT_" + subStr + " = @0 AND  FBT_isDeleted == @1", property.GetValue(filters.FF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.FF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.FF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.FF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.FBT_Approver_ID) && x.FBT_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.FBT_Creator_ID) && x.FBT_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.FBT_Status_ID) && x.FBT_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("FBT_" + subStr + ".Contains(@0) AND  FBT_isDeleted == @1", property.GetValue(filters.FF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var creatorList = (from a in mList
                               join b in _context.User on a.FBT_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new
                               { a.FBT_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.FBT_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new
                              { a.FBT_ID, ApproverName }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.FBT_Status_ID equals d.Status_ID
                            select new { a.FBT_ID, d.Status_Name }).ToList();

            List<DMFBTViewModel> vmList = new List<DMFBTViewModel>();
            foreach (DMFBTModel m in mList)
            {
                var creator = creatorList.Where(a => a.FBT_ID == m.FBT_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.FBT_ID == m.FBT_ID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.FBT_ID == m.FBT_ID).Select(a => a.Status_Name).FirstOrDefault();
                DMFBTViewModel vm = new DMFBTViewModel
                {
                    FBT_MasterID = m.FBT_MasterID,
                    FBT_Name = m.FBT_Name,
                    FBT_Formula = m.FBT_Formula,
                    FBT_Tax_Rate = m.FBT_Tax_Rate,
                    FBT_Creator_Name = creator ?? "N/A",
                    FBT_Approver_Name = approver ?? "",
                    FBT_Created_Date = m.FBT_Created_Date,
                    FBT_Last_Updated = m.FBT_Last_Updated,
                    FBT_Status_ID = m.FBT_Status_ID,
                    FBT_Status = stat ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMTRViewModel> populateTR(DMFiltersViewModel filters)
        {
            IQueryable<DMTRModel> mList = _context.DMTR.Where(x => x.TR_isDeleted == false && x.TR_isActive == true).ToList().AsQueryable();
            var properties = filters.TF.GetType().GetProperties();

            var pendingList = _context.DMTR_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMTRModel[] {
                    new DMTRModel
                    {
                        TR_MasterID = m.Pending_TR_MasterID,
                        TR_WT_Title = m.Pending_TR_WT_Title,
                        TR_Nature = m.Pending_TR_Nature,
                        TR_Tax_Rate = m.Pending_TR_Tax_Rate,
                        TR_ATC = m.Pending_TR_ATC,
                        TR_Nature_Income_Payment = m.Pending_TR_Nature_Income_Payment,
                        TR_Creator_ID = m.Pending_TR_Creator_ID,
                        TR_Approver_ID = m.Pending_TR_Approver_ID.Equals(null) ? 0 : m.Pending_TR_Approver_ID,
                        TR_Created_Date = m.Pending_TR_Filed_Date,
                        TR_Last_Updated = m.Pending_TR_Filed_Date,
                        TR_Status_ID = m.Pending_TR_Status_ID
                    }
                });
            }
            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.TF).ToString();
                string[] colArr = { "Tax_Rate", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(subStr)) // IF INT VAL
                        {
                            mList = mList.Where("TR_" + subStr + " = @0 AND  TR_isDeleted == @1", property.GetValue(filters.TF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.TF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.TF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.TF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.TR_Approver_ID) && x.TR_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.TR_Creator_ID) && x.TR_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.TR_Status_ID) && x.TR_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("TR_" + subStr + ".Contains(@0) AND  TR_isDeleted == @1", property.GetValue(filters.TF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var creatorList = (from a in mList
                               join b in _context.User on a.TR_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new
                               { a.TR_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.TR_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new
                              { a.TR_ID, ApproverName }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.TR_Status_ID equals d.Status_ID
                            select new { a.TR_ID, d.Status_Name }).ToList();

            List<DMTRViewModel> vmList = new List<DMTRViewModel>();
            foreach (DMTRModel m in mList)
            {
                var creator = creatorList.Where(a => a.TR_ID == m.TR_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.TR_ID == m.TR_ID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.TR_ID == m.TR_ID).Select(a => a.Status_Name).FirstOrDefault();
                DMTRViewModel vm = new DMTRViewModel
                {
                    TR_MasterID = m.TR_MasterID,
                    TR_WT_Title = m.TR_WT_Title,
                    TR_Nature = m.TR_Nature,
                    TR_Tax_Rate = m.TR_Tax_Rate * 100,
                    TR_ATC = m.TR_ATC,
                    TR_Nature_Income_Payment = m.TR_Nature_Income_Payment,
                    TR_Creator_Name = creator ?? "N/A",
                    TR_Approver_Name = approver ?? "",
                    TR_Created_Date = m.TR_Created_Date,
                    TR_Last_Updated = m.TR_Last_Updated,
                    TR_Status_ID = m.TR_Status_ID,
                    TR_Status = stat ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMCurrencyViewModel> populateCurr(DMFiltersViewModel filters)
        {
            IQueryable<DMCurrencyModel> mList = _context.DMCurrency.Where(x => x.Curr_isDeleted == false && x.Curr_isActive == true).ToList().AsQueryable();
            var properties = filters.CF.GetType().GetProperties();

            var pendingList = _context.DMCurrency_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMCurrencyModel[] {
                    new DMCurrencyModel
                    {
                       // Curr_ID = m.Pending_Curr_MasterID,
                        Curr_MasterID = m.Pending_Curr_MasterID,
                        Curr_Name = m.Pending_Curr_Name,
                        Curr_CCY_ABBR = m.Pending_Curr_CCY_ABBR,
                        Curr_Creator_ID = m.Pending_Curr_Creator_ID,
                        Curr_Approver_ID = m.Pending_Curr_Approver_ID.Equals(null) ? 0 : m.Pending_Curr_Approver_ID,
                        Curr_Created_Date = m.Pending_Curr_Filed_Date,
                        Curr_Last_Updated = m.Pending_Curr_Filed_Date,
                        Curr_Status_ID = m.Pending_Curr_Status_ID
                    }
                });
            }
            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.CF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(subStr)) // IF INT VAL
                        {
                            mList = mList.Where("Curr_" + subStr + " = @0 AND  Curr_isDeleted == @1", property.GetValue(filters.CF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.CF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.CF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.CF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Curr_Approver_ID) && x.Curr_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Curr_Creator_ID) && x.Curr_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.Curr_Status_ID) && x.Curr_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Curr_" + subStr + ".Contains(@0) AND  Curr_isDeleted == @1", property.GetValue(filters.CF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var creatorList = (from a in mList
                               join b in _context.User on a.Curr_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new { a.Curr_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.Curr_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new { a.Curr_ID, ApproverName }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Curr_Status_ID equals d.Status_ID
                            select new { a.Curr_ID, d.Status_Name }).ToList();

            List<DMCurrencyViewModel> vmList = new List<DMCurrencyViewModel>();
            foreach (DMCurrencyModel m in mList)
            {
                var creator = creatorList.Where(a => a.Curr_ID == m.Curr_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Curr_ID == m.Curr_ID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.Curr_ID == m.Curr_ID).Select(a => a.Status_Name).FirstOrDefault();
                DMCurrencyViewModel vm = new DMCurrencyViewModel
                {
                    Curr_MasterID = m.Curr_MasterID,
                    Curr_Name = m.Curr_Name,
                    Curr_CCY_ABBR = m.Curr_CCY_ABBR,
                    Curr_Creator_Name = creator ?? "N/A",
                    Curr_Approver_Name = approver ?? "",
                    Curr_Created_Date = m.Curr_Created_Date,
                    Curr_Last_Updated = m.Curr_Last_Updated,
                    Curr_Status_ID = m.Curr_Status_ID,
                    Curr_Status = stat ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMEmpViewModel> populateRegEmp(DMFiltersViewModel filters)
        {
            IQueryable<DMEmpModel> mList = _context.DMEmp.Where(x => x.Emp_isDeleted == false && x.Emp_isActive == true 
                                            && x.Emp_Type == "Regular").ToList().AsQueryable();
            var pendingList = _context.DMEmp_Pending.Where(x=> x.Pending_Emp_Type == "Regular").ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMEmpModel[] {
                    new DMEmpModel
                    {
                        Emp_MasterID = m.Pending_Emp_MasterID,
                        Emp_Name = m.Pending_Emp_Name,
                        Emp_Acc_No = m.Pending_Emp_Acc_No,
                        Emp_Creator_ID = m.Pending_Emp_Creator_ID,
                        Emp_Approver_ID = m.Pending_Emp_Approver_ID.Equals(null) ? 0 : m.Pending_Emp_Approver_ID,
                        Emp_Created_Date = m.Pending_Emp_Filed_Date,
                        Emp_Last_Updated = m.Pending_Emp_Filed_Date,
                        Emp_Status_ID = m.Pending_Emp_Status_ID
                    }
                });
            }
            var properties = filters.EMF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.EMF).ToString();
                string[] colArr = { "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.EMF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.EMF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.EMF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Emp_Approver_ID) && x.Emp_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Emp_Creator_ID) && x.Emp_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.Emp_Status_ID) && x.Emp_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Emp_" + subStr + ".Contains(@0)", toStr)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }

            var creatorList = (from a in mList
                               join b in _context.User on a.Emp_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new { a.Emp_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.Emp_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new { a.Emp_ID, ApproverName }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Emp_Status_ID equals d.Status_ID
                            select new { a.Emp_ID, d.Status_Name }).ToList();

            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            foreach (DMEmpModel m in mList)
            {
                var creator = creatorList.Where(a => a.Emp_ID == m.Emp_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Emp_ID == m.Emp_ID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.Emp_ID == m.Emp_ID).Select(a => a.Status_Name).FirstOrDefault();
                DMEmpViewModel vm = new DMEmpViewModel
                {
                    Emp_MasterID = m.Emp_MasterID,
                    Emp_Name = m.Emp_Name,
                    Emp_Acc_No = m.Emp_Acc_No,
                    Emp_Creator_Name = creator ?? "N/A",
                    Emp_Approver_Name = approver ?? "",
                    Emp_Created_Date = m.Emp_Created_Date,
                    Emp_Last_Updated = m.Emp_Last_Updated,
                    Emp_Status_ID = m.Emp_Status_ID,
                    Emp_Status = stat ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMEmpViewModel> populateTempEmp(DMFiltersViewModel filters)
        {
            IQueryable<DMEmpModel> mList = _context.DMEmp.Where(x => x.Emp_isDeleted == false && x.Emp_isActive == true
                                            && x.Emp_Type == "Temporary").ToList().AsQueryable();
            var pendingList = _context.DMEmp_Pending.Where(x => x.Pending_Emp_Type == "Temporary").ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMEmpModel[] {
                    new DMEmpModel
                    {
                        Emp_MasterID = m.Pending_Emp_MasterID,
                        Emp_Name = m.Pending_Emp_Name,
                        Emp_Acc_No = m.Pending_Emp_Acc_No,
                        Emp_Creator_ID = m.Pending_Emp_Creator_ID,
                        Emp_Approver_ID = m.Pending_Emp_Approver_ID.Equals(null) ? 0 : m.Pending_Emp_Approver_ID,
                        Emp_Created_Date = m.Pending_Emp_Filed_Date,
                        Emp_Last_Updated = m.Pending_Emp_Filed_Date,
                        Emp_Status_ID = m.Pending_Emp_Status_ID
                    }
                });
            }
            var properties = filters.EMF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.EMF).ToString();
                string[] colArr = { "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.EMF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.EMF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.EMF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Emp_Approver_ID) && x.Emp_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Emp_Creator_ID) && x.Emp_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.Emp_Status_ID) && x.Emp_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Emp_" + subStr + ".Contains(@0)", toStr)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }

            var creatorList = (from a in mList
                               join b in _context.User on a.Emp_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new { a.Emp_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.Emp_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new { a.Emp_ID, ApproverName }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Emp_Status_ID equals d.Status_ID
                            select new { a.Emp_ID, d.Status_Name }).ToList();

            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            foreach (DMEmpModel m in mList)
            {
                var creator = creatorList.Where(a => a.Emp_ID == m.Emp_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Emp_ID == m.Emp_ID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.Emp_ID == m.Emp_ID).Select(a => a.Status_Name).FirstOrDefault();
                DMEmpViewModel vm = new DMEmpViewModel
                {
                    Emp_MasterID = m.Emp_MasterID,
                    Emp_Name = m.Emp_Name,
                    Emp_Acc_No = m.Emp_Acc_No ?? "",
                    Emp_Creator_Name = creator ?? "N/A",
                    Emp_Approver_Name = approver ?? "",
                    Emp_Created_Date = m.Emp_Created_Date,
                    Emp_Last_Updated = m.Emp_Last_Updated,
                    Emp_Status_ID = m.Emp_Status_ID,
                    Emp_Status = stat ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMCustViewModel> populateCust(DMFiltersViewModel filters)
        {
            IQueryable<DMCustModel> mList = _context.DMCust.Where(x => x.Cust_isDeleted == false && x.Cust_isActive == true).ToList().AsQueryable();
            var pendingList = _context.DMCust_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMCustModel[] {
                    new DMCustModel
                    {
                        Cust_MasterID = m.Pending_Cust_MasterID,
                        Cust_Name = m.Pending_Cust_Name,
                        Cust_Abbr = m.Pending_Cust_Abbr,
                        Cust_No = m.Pending_Cust_No,
                        Cust_Creator_ID = m.Pending_Cust_Creator_ID,
                        Cust_Approver_ID = m.Pending_Cust_Approver_ID.Equals(null) ? 0 : m.Pending_Cust_Approver_ID,
                        Cust_Created_Date = m.Pending_Cust_Filed_Date,
                        Cust_Last_Updated = m.Pending_Cust_Filed_Date,
                        Cust_Status_ID = m.Pending_Cust_Status_ID
                    }
                });
            }
            var properties = filters.CUF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.CUF).ToString();
                string[] colArr = { "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.CUF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.CUF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.CUF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Cust_Approver_ID) && x.Cust_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.Cust_Creator_ID) && x.Cust_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.Cust_Status_ID) && x.Cust_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Cust_" + subStr + ".Contains(@0)", toStr)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }

            var creatorList = (from a in mList
                               join b in _context.User on a.Cust_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new { a.Cust_ID, CreatorName  }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.Cust_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new { a.Cust_ID, ApproverName }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Cust_Status_ID equals d.Status_ID
                            select new { a.Cust_ID, d.Status_Name }).ToList();

            List<DMCustViewModel> vmList = new List<DMCustViewModel>();
            foreach (DMCustModel m in mList)
            {
                var creator = creatorList.Where(a => a.Cust_ID == m.Cust_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Cust_ID == m.Cust_ID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.Cust_ID == m.Cust_ID).Select(a => a.Status_Name).FirstOrDefault();
                DMCustViewModel vm = new DMCustViewModel
                {
                    Cust_MasterID = m.Cust_MasterID,
                    Cust_Name = m.Cust_Name,
                    Cust_Abbr = m.Cust_Abbr,
                    Cust_No = m.Cust_No,
                    Cust_Creator_Name = creator ?? "N/A",
                    Cust_Approver_Name = approver ?? "",
                    Cust_Created_Date = m.Cust_Created_Date,
                    Cust_Last_Updated = m.Cust_Last_Updated,
                    Cust_Status_ID = m.Cust_Status_ID,
                    Cust_Status = stat ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMBCSViewModel> populateBCS(DMFiltersViewModel filters)
        {
            IQueryable<DMBIRCertSignModel> mList = _context.DMBCS.Where(x => x.BCS_isDeleted == false && x.BCS_isActive == true).ToList().AsQueryable();
            var pendingList = _context.DMBCS_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMBIRCertSignModel[] {
                    new DMBIRCertSignModel
                    {
                        BCS_MasterID = m.Pending_BCS_MasterID,
                        BCS_Name = m.Pending_BCS_Name,
                        BCS_TIN = m.Pending_BCS_TIN,
                        BCS_Position = m.Pending_BCS_Position,
                        BCS_Signatures = m.Pending_BCS_Signatures,
                        BCS_Creator_ID = m.Pending_BCS_Creator_ID,
                        BCS_Approver_ID = m.Pending_BCS_Approver_ID.Equals(null) ? 0 : m.Pending_BCS_Approver_ID,
                        BCS_Created_Date = m.Pending_BCS_Filed_Date,
                        BCS_Last_Updated = m.Pending_BCS_Filed_Date,
                        BCS_Status_ID = m.Pending_BCS_Status_ID
                    }
                });
            }
            var properties = filters.BF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.BF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.BF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.BF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.BF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.BCS_Approver_ID) && x.BCS_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.BCS_Creator_ID) && x.BCS_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.BCS_Status_ID) && x.BCS_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("BCS_" + subStr + ".Contains(@0)", toStr)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }

            var creatorList = (from a in mList
                               join b in _context.User on a.BCS_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new { a.BCS_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.BCS_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new { a.BCS_ID, ApproverName }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.BCS_Status_ID equals d.Status_ID
                            select new { a.BCS_ID, d.Status_Name }).ToList();

            List<DMBCSViewModel> vmList = new List<DMBCSViewModel>();
            FileService fs = new FileService();
            foreach (DMBIRCertSignModel m in mList)
            {
                var creator = creatorList.Where(a => a.BCS_ID == m.BCS_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.BCS_ID == m.BCS_ID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.BCS_ID == m.BCS_ID).Select(a => a.Status_Name).FirstOrDefault();
                DMBCSViewModel vm = new DMBCSViewModel
                {
                    BCS_MasterID = m.BCS_MasterID,
                    BCS_Name = m.BCS_Name,
                    BCS_TIN = m.BCS_TIN,
                    BCS_Position = m.BCS_Position,
                    BCS_Signatures = m.BCS_Signatures,
                    BCS_Creator_Name = creator ?? "N/A",
                    BCS_Approver_Name = approver ?? "",
                    BCS_Created_Date = m.BCS_Created_Date,
                    BCS_Last_Updated = m.BCS_Last_Updated,
                    BCS_Status_ID = m.BCS_Status_ID,
                    BCS_Status = stat ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMAccountGroupViewModel> populateAG(DMFiltersViewModel filters)
        {
            IQueryable<DMAccountGroupModel> mList = _context.DMAccountGroup.Where(x => x.AccountGroup_isDeleted == false && x.AccountGroup_isActive == true).ToList().AsQueryable();
            var properties = filters.AGF.GetType().GetProperties();

            var pendingList = _context.DMAccountGroup_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMAccountGroupModel[] {
                    new DMAccountGroupModel
                    {
                        AccountGroup_MasterID = m.Pending_AccountGroup_MasterID,
                        AccountGroup_Name = m.Pending_AccountGroup_Name,
                        AccountGroup_Code = m.Pending_AccountGroup_Code,
                        AccountGroup_Creator_ID = m.Pending_AccountGroup_Creator_ID,
                        AccountGroup_Approver_ID = m.Pending_AccountGroup_Approver_ID.Equals(null) ? 0 : m.Pending_AccountGroup_Approver_ID,
                        AccountGroup_Created_Date = m.Pending_AccountGroup_Filed_Date,
                        AccountGroup_Last_Updated = m.Pending_AccountGroup_Filed_Date,
                        AccountGroup_Status_ID = m.Pending_AccountGroup_Status_ID
                    }
                });
            }
            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.AGF).ToString();
                string[] colArr = { "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.AGF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.AGF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.AGF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            if (subStr == "Approver_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.AccountGroup_Approver_ID) && x.AccountGroup_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                mList = mList.Where(x => names.Contains(x.AccountGroup_Creator_ID) && x.AccountGroup_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                mList = mList.Where(x => status.Contains(x.AccountGroup_Status_ID) && x.AccountGroup_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("AccountGroup_" + subStr + ".Contains(@0) AND  AccountGroup_isDeleted == @1", property.GetValue(filters.AGF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var creatorList = (from a in mList
                               join b in _context.User on a.AccountGroup_Creator_ID equals b.User_ID
                               let CreatorName = b.User_LName + ", " + b.User_FName
                               select new { a.AccountGroup_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.User on a.AccountGroup_Approver_ID equals c.User_ID
                              let ApproverName = c.User_LName + ", " + c.User_FName
                              select new { a.AccountGroup_ID, ApproverName }).ToList();
            var statList = (from a in mList
                              join c in _context.StatusList on a.AccountGroup_Status_ID equals c.Status_ID
                              select new { a.AccountGroup_ID, c.Status_Name }).ToList();
            //TEMP where clause until FBT is updated
            var defaultFBT = _context.DMFBT.Where(x => x.FBT_MasterID == 1).Select(x => x.FBT_Name).FirstOrDefault();

            List<DMAccountGroupViewModel> vmList = new List<DMAccountGroupViewModel>();
            foreach (DMAccountGroupModel m in mList)
            {
                var creator = creatorList.Where(a => a.AccountGroup_ID == m.AccountGroup_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.AccountGroup_ID == m.AccountGroup_ID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.AccountGroup_ID == m.AccountGroup_ID).Select(a => a.Status_Name).FirstOrDefault();
                DMAccountGroupViewModel vm = new DMAccountGroupViewModel
                {
                    AccountGroup_MasterID = m.AccountGroup_MasterID,
                    AccountGroup_Name = m.AccountGroup_Name,
                    AccountGroup_Code = m.AccountGroup_Code,
                    AccountGroup_Creator_Name = creator ?? "N/A",
                    AccountGroup_Approver_Name = approver ?? "",
                    AccountGroup_Created_Date = m.AccountGroup_Created_Date,
                    AccountGroup_Last_Updated = m.AccountGroup_Last_Updated,
                    AccountGroup_Status_ID = m.AccountGroup_Status_ID,
                    AccountGroup_Status_Name = stat ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }
    }
}
