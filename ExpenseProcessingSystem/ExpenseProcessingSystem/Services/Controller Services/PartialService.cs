﻿using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Search_Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

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
        //Populate
        public List<DMPayeeViewModel> populatePayee(DMFiltersViewModel filters)
        {
            IQueryable<DMPayeeModel> mList = _context.DMPayee.Where(x=>x.Payee_isDeleted == false && x.Payee_isActive == true).ToList().AsQueryable();
            var properties = filters.PF.GetType().GetProperties();

            //FILTER
            foreach(var property in properties)
            {
                var propertyName = property.Name;
                string[] split = propertyName.Split("_");
                var toStr = property.GetValue(filters.PF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(split[1])) // IF INT VAL
                        {
                            mList = mList.Where("Payee_" + split[1] + " = @0 AND  Payee_isDeleted == @1", property.GetValue(filters.PF), false)
                                     .Select(e => e).AsQueryable();
                        }else if (split[1] == "Creator_Name" || split[1] == "Approver_Name")
                        { //UPDATE THIS TO SEARCH FOR CREATOR OR APPROVER NAME IN USER TABLE
                            mList = mList.Where("Payee_" + split[1] + ".Contains(@0) AND  Payee_isDeleted == @1", property.GetValue(filters.PF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Payee_"+split[1]+ ".Contains(@0) AND  Payee_isDeleted == @1", property.GetValue(filters.PF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var pendingList = _context.DMPayee_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMPayeeModel[] {
                    new DMPayeeModel
                    {
                       // Payee_ID = m.Pending_Payee_MasterID,
                        Payee_MasterID = m.Pending_Payee_MasterID,
                        Payee_Name = m.Pending_Payee_Name,
                        Payee_TIN = m.Pending_Payee_TIN,
                        Payee_Address = m.Pending_Payee_Address,
                        Payee_Type = m.Pending_Payee_Type,
                        Payee_No = m.Pending_Payee_No,
                        Payee_Creator_ID = m.Pending_Payee_Creator_ID,
                        Payee_Approver_ID = m.Pending_Payee_Approver_ID.Equals(null) ? 0 : m.Pending_Payee_Approver_ID,
                        Payee_Created_Date = m.Pending_Payee_Filed_Date,
                        Payee_Last_Updated = m.Pending_Payee_Filed_Date,
                        Payee_Status = m.Pending_Payee_Status
                    }
                });
            }

            var creatorList = (from a in mList
                               join b in _context.Account on a.Payee_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new
                               { a.Payee_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.Payee_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new
                              { a.Payee_ID, ApproverName }).ToList();

            //assign values
            List<DMPayeeModel> mList2 = mList.ToList();
            List <DMPayeeViewModel> vmList = new List<DMPayeeViewModel>();
            foreach (DMPayeeModel m in mList2)
            {
                var creator = creatorList.Where(a => a.Payee_ID == m.Payee_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Payee_ID == m.Payee_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMPayeeViewModel vm = new DMPayeeViewModel
                {
                    Payee_MasterID = m.Payee_MasterID,
                    Payee_Name = m.Payee_Name,
                    Payee_TIN = m.Payee_TIN,
                    Payee_Address = m.Payee_Address,
                    Payee_Type = m.Payee_Type,
                    Payee_No = m.Payee_No,
                    Payee_Creator_Name = creator ?? "N/A",
                    Payee_Approver_Name = approver ?? "",
                    Payee_Created_Date = m.Payee_Created_Date,
                    Payee_Last_Updated = m.Payee_Last_Updated,
                    Payee_Status = m.Payee_Status
                };
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
                        Dept_Creator_ID = m.Pending_Dept_Creator_ID,
                        Dept_Approver_ID = m.Pending_Dept_Approver_ID.Equals(null) ? 0 : m.Pending_Dept_Approver_ID,
                        Dept_Created_Date = m.Pending_Dept_Filed_Date,
                        Dept_Last_Updated = m.Pending_Dept_Filed_Date,
                        Dept_Status = m.Pending_Dept_Status
                    }
                });
            }
            var properties = filters.DF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string[] split = propertyName.Split("_");
                var toStr = property.GetValue(filters.DF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (split[1] == "Creator_Name" || split[1] == "Approver_Name")
                        { //UPDATE THIS TO SEARCH FOR CREATOR OR APPROVER NAME IN USER TABLE
                            mList = mList.Where("Dept_" + split[1] + ".Contains(@0)", toStr)
                                     .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Dept_" + split[1] + ".Contains(@0)", toStr)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
           
            var creatorList = (from a in mList
                               join b in _context.Account on a.Dept_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new
                               {
                                   a.Dept_ID,
                                   CreatorName
                               }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.Dept_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new
                              {
                                  a.Dept_ID,
                                  ApproverName
                              }).ToList();

            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            foreach (DMDeptModel m in mList)
            {
                var creator = creatorList.Where(a => a.Dept_ID == m.Dept_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Dept_ID == m.Dept_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_MasterID = m.Dept_MasterID,
                    Dept_Name = m.Dept_Name,
                    Dept_Code = m.Dept_Code,
                    Dept_Creator_Name = creator ?? "N/A",
                    Dept_Approver_Name = approver ?? "",
                    Dept_Created_Date = m.Dept_Created_Date,
                    Dept_Last_Updated = m.Dept_Last_Updated,
                    Dept_Status = m.Dept_Status
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
                        Check_Name = m.Pending_Check_Name,
                        Check_Type = m.Pending_Check_Type,
                        Check_Creator_ID = m.Pending_Check_Creator_ID,
                        Check_Approver_ID = m.Pending_Check_Approver_ID.Equals(null) ? 0 : m.Pending_Check_Approver_ID,
                        Check_Created_Date = m.Pending_Check_Filed_Date,
                        Check_Last_Updated = m.Pending_Check_Filed_Date,
                        Check_Status = m.Pending_Check_Status
                    }
                });
            }
            var properties = filters.CKF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string[] split = propertyName.Split("_");
                split[1] = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.CKF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0" && toStr != "0001/01/01 0:00:00")
                    {
                        if (split[1] == "Creator_Name" || split[1] == "Approver_Name")
                        { //UPDATE THIS TO SEARCH FOR CREATOR OR APPROVER NAME IN USER TABLE
                            mList = mList.Where("Check_" + split[1] + ".Contains(@0)", toStr)
                                     .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Check_" + split[1] + ".Contains(@0)", toStr)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            
            var creatorList = (from a in mList
                               join b in _context.Account on a.Check_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new
                               {
                                   a.Check_ID,
                                   CreatorName
                               }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.Check_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new
                              {
                                  a.Check_ID,
                                  ApproverName
                              }).ToList();

            List<DMCheckViewModel> vmList = new List<DMCheckViewModel>();
            foreach (DMCheckModel m in mList)
            {
                var creator = creatorList.Where(a => a.Check_ID == m.Check_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Check_ID == m.Check_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMCheckViewModel vm = new DMCheckViewModel
                {
                    Check_MasterID = m.Check_MasterID,
                    Check_Input_Date = m.Check_Input_Date,
                    Check_Series_From = m.Check_Series_From,
                    Check_Series_To = m.Check_Series_To,
                    Check_Name = m.Check_Name,
                    Check_Type = m.Check_Type,
                    Check_Creator_Name = creator ?? "N/A",
                    Check_Approver_Name = approver ?? "",
                    Check_Created_Date = m.Check_Created_Date,
                    Check_Last_Updated = m.Check_Last_Updated,
                    Check_Status = m.Check_Status
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMAccountViewModel> populateAccount(DMFiltersViewModel filters)
        {
            IQueryable<DMAccountModel> mList = _context.DMAccount.Where(x => x.Account_isDeleted == false && x.Account_isActive == true).ToList().AsQueryable();
            var properties = filters.AF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string[] split = propertyName.Split("_");
                var toStr = property.GetValue(filters.AF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(split[1])) // IF INT VAL
                        {
                            mList = mList.Where("Account_" + split[1] + " = @0 AND  Account_isDeleted == @1", property.GetValue(filters.AF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (split[1] == "Creator_Name" || split[1] == "Approver_Name")
                        { //UPDATE THIS TO SEARCH FOR CREATOR OR APPROVER NAME IN USER TABLE
                            mList = mList.Where("Account_" + split[1] + ".Contains(@0) AND  Account_isDeleted == @1", property.GetValue(filters.AF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Account_" + split[1] + ".Contains(@0) AND  Account_isDeleted == @1", property.GetValue(filters.AF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var pendingList = _context.DMAccount_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMAccountModel[] {
                    new DMAccountModel
                    {
                       // Account_ID = m.Pending_Account_MasterID,
                        Account_MasterID = m.Pending_Account_MasterID,
                        Account_Name = m.Pending_Account_Name,
                        Account_Code = m.Pending_Account_Code,
                        Account_No = m.Pending_Account_No,
                        Account_Cust = m.Pending_Account_Cust,
                        Account_Div = m.Pending_Account_Div,
                        Account_Fund = m.Pending_Account_Fund,
                        Account_Creator_ID = m.Pending_Account_Creator_ID,
                        Account_Approver_ID = m.Pending_Account_Approver_ID.Equals(null) ? 0 : m.Pending_Account_Approver_ID,
                        Account_Created_Date = m.Pending_Account_Filed_Date,
                        Account_Last_Updated = m.Pending_Account_Filed_Date,
                        Account_Status = m.Pending_Account_Status
                    }
                });
            }
            var creatorList = (from a in mList
                               join b in _context.Account on a.Account_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new { a.Account_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.Account_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new { a.Account_ID, ApproverName }).ToList();

            List<DMAccountViewModel> vmList = new List<DMAccountViewModel>();
            foreach (DMAccountModel m in mList)
            {
                var creator = creatorList.Where(a => a.Account_ID == m.Account_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Account_ID == m.Account_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMAccountViewModel vm = new DMAccountViewModel
                {
                    Account_MasterID = m.Account_MasterID,
                    Account_Name = m.Account_Name,
                    Account_Code = m.Account_Code,
                    Account_No = m.Account_No,
                    Account_Cust = m.Account_Cust,
                    Account_Div = m.Account_Div,
                    Account_Fund = m.Account_Fund,
                    Account_Creator_Name = creator ?? "N/A",
                    Account_Approver_Name = approver ?? "",
                    Account_Created_Date = m.Account_Created_Date,
                    Account_Last_Updated = m.Account_Last_Updated,
                    Account_Status = m.Account_Status
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMVATViewModel> populateVAT(DMFiltersViewModel filters)
        {
            IQueryable<DMVATModel> mList = _context.DMVAT.Where(x => x.VAT_isDeleted == false && x.VAT_isActive == true).ToList().AsQueryable();
            var properties = filters.VF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string[] split = propertyName.Split("_");
                var toStr = property.GetValue(filters.VF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(split[1])) // IF INT VAL
                        {
                            mList = mList.Where("VAT_" + split[1] + " = @0 AND  VAT_isDeleted == @1", property.GetValue(filters.VF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (split[1] == "Creator_Name" || split[1] == "Approver_Name")
                        { //UPDATE THIS TO SEARCH FOR CREATOR OR APPROVER NAME IN USER TABLE
                            mList = mList.Where("VAT_" + split[1] + ".Contains(@0) AND  VAT_isDeleted == @1", property.GetValue(filters.VF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("VAT_" + split[1] + ".Contains(@0) AND  VAT_isDeleted == @1", property.GetValue(filters.VF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
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
                        VAT_Status = m.Pending_VAT_Status
                    }
                });
            }
            var creatorList = (from a in mList
                               join b in _context.Account on a.VAT_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new { a.VAT_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.VAT_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new { a.VAT_ID, ApproverName }).ToList();

            List<DMVATViewModel> vmList = new List<DMVATViewModel>();
            foreach (DMVATModel m in mList)
            {
                var creator = creatorList.Where(a => a.VAT_ID == m.VAT_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.VAT_ID == m.VAT_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMVATViewModel vm = new DMVATViewModel
                {
                    VAT_MasterID = m.VAT_MasterID,
                    VAT_Name = m.VAT_Name,
                    VAT_Rate = m.VAT_Rate,
                    VAT_Creator_Name = creator ?? "N/A",
                    VAT_Approver_Name = approver ?? "",
                    VAT_Created_Date = m.VAT_Created_Date,
                    VAT_Last_Updated = m.VAT_Last_Updated,
                    VAT_Status = m.VAT_Status
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMFBTViewModel> populateFBT(DMFiltersViewModel filters)
        {
            IQueryable<DMFBTModel> mList = _context.DMFBT.Where(x => x.FBT_isDeleted == false && x.FBT_isActive == true).ToList().AsQueryable();
            var properties = filters.FF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string[] split = propertyName.Split("_");
                var toStr = property.GetValue(filters.FF).ToString();
                string[] colArr = { "Tax_Rate", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(split[1])) // IF INT VAL
                        {
                            mList = mList.Where("FBT_" + split[1] + " = @0 AND  FBT_isDeleted == @1", property.GetValue(filters.FF), false)
                                     .Select(e => e).AsQueryable();
    }
                        else if (split[1] == "Creator_Name" || split[1] == "Approver_Name")
                        { //UPDATE THIS TO SEARCH FOR CREATOR OR APPROVER NAME IN USER TABLE
                            mList = mList.Where("FBT_" + split[1] + ".Contains(@0) AND  FBT_isDeleted == @1", property.GetValue(filters.FF), false)
                                     .Select(e => e).AsQueryable();
}
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("FBT_" + split[1] + ".Contains(@0) AND  FBT_isDeleted == @1", property.GetValue(filters.FF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var pendingList = _context.DMFBT_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMFBTModel[] {
                    new DMFBTModel
                    {
                        FBT_MasterID = m.Pending_FBT_MasterID,
                        FBT_Name = m.Pending_FBT_Name,
                        FBT_Tax_Rate = m.Pending_FBT_Tax_Rate,
                        FBT_Account = m.Pending_FBT_Account,
                        FBT_Formula = m.Pending_FBT_Formula,
                        FBT_Creator_ID = m.Pending_FBT_Creator_ID,
                        FBT_Approver_ID = m.Pending_FBT_Approver_ID.Equals(null) ? 0 : m.Pending_FBT_Approver_ID,
                        FBT_Created_Date = m.Pending_FBT_Filed_Date,
                        FBT_Last_Updated = m.Pending_FBT_Filed_Date,
                        FBT_Status = m.Pending_FBT_Status
                    }
                });
            }
            var creatorList = (from a in mList
                               join b in _context.Account on a.FBT_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new
                               { a.FBT_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.FBT_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new
                              { a.FBT_ID, ApproverName }).ToList();

            List<DMFBTViewModel> vmList = new List<DMFBTViewModel>();
            foreach (DMFBTModel m in mList)
            {
                var creator = creatorList.Where(a => a.FBT_ID == m.FBT_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.FBT_ID == m.FBT_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMFBTViewModel vm = new DMFBTViewModel
                {
                    FBT_MasterID = m.FBT_MasterID,
                    FBT_Name = m.FBT_Name,
                    FBT_Account = m.FBT_Account,
                    FBT_Formula = m.FBT_Formula,
                    FBT_Tax_Rate = m.FBT_Tax_Rate,
                    FBT_Creator_Name = creator ?? "N/A",
                    FBT_Approver_Name = approver ?? "",
                    FBT_Created_Date = m.FBT_Created_Date,
                    FBT_Last_Updated = m.FBT_Last_Updated,
                    FBT_Status = m.FBT_Status
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMEWTViewModel> populateEWT(DMFiltersViewModel filters)
        {
            IQueryable<DMEWTModel> mList = _context.DMEWT.Where(x => x.EWT_isDeleted == false && x.EWT_isActive == true).ToList().AsQueryable();
            var properties = filters.EF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string[] split = propertyName.Split("_");
                var toStr = property.GetValue(filters.EF).ToString();
                string[] colArr = { "Tax_Rate", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(split[1])) // IF INT VAL
                        {
                            mList = mList.Where("EWT_" + split[1] + " = @0 AND  EWT_isDeleted == @1", property.GetValue(filters.EF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (split[1] == "Creator_Name" || split[1] == "Approver_Name")
                        { //UPDATE THIS TO SEARCH FOR CREATOR OR APPROVER NAME IN USER TABLE
                            mList = mList.Where("EWT_" + split[1] + ".Contains(@0) AND  EWT_isDeleted == @1", property.GetValue(filters.EF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("EWT_" + split[1] + ".Contains(@0) AND  EWT_isDeleted == @1", property.GetValue(filters.EF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var pendingList = _context.DMEWT_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMEWTModel[] {
                    new DMEWTModel
                    {
                        EWT_MasterID = m.Pending_EWT_MasterID,
                        EWT_Nature = m.Pending_EWT_Nature,
                        EWT_Tax_Rate = m.Pending_EWT_Tax_Rate,
                        EWT_ATC = m.Pending_EWT_ATC,
                        EWT_Tax_Rate_Desc = m.Pending_EWT_Tax_Rate_Desc,
                        EWT_Creator_ID = m.Pending_EWT_Creator_ID,
                        EWT_Approver_ID = m.Pending_EWT_Approver_ID.Equals(null) ? 0 : m.Pending_EWT_Approver_ID,
                        EWT_Created_Date = m.Pending_EWT_Filed_Date,
                        EWT_Last_Updated = m.Pending_EWT_Filed_Date,
                        EWT_Status = m.Pending_EWT_Status
                    }
                });
            }
            var creatorList = (from a in mList
                               join b in _context.Account on a.EWT_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new
                               { a.EWT_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.EWT_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new
                              { a.EWT_ID, ApproverName }).ToList();

            List<DMEWTViewModel> vmList = new List<DMEWTViewModel>();
            foreach (DMEWTModel m in mList)
            {
                var creator = creatorList.Where(a => a.EWT_ID == m.EWT_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.EWT_ID == m.EWT_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMEWTViewModel vm = new DMEWTViewModel
                {
                    EWT_MasterID = m.EWT_MasterID,
                    EWT_Nature = m.EWT_Nature,
                    EWT_Tax_Rate = m.EWT_Tax_Rate,
                    EWT_ATC = m.EWT_ATC,
                    EWT_Tax_Rate_Desc = m.EWT_Tax_Rate_Desc,
                    EWT_Creator_Name = creator ?? "N/A",
                    EWT_Approver_Name = approver ?? "",
                    EWT_Created_Date = m.EWT_Created_Date,
                    EWT_Last_Updated = m.EWT_Last_Updated,
                    EWT_Status = m.EWT_Status
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMCurrencyViewModel> populateCurr(DMFiltersViewModel filters)
        {
            IQueryable<DMCurrencyModel> mList = _context.DMCurrency.Where(x => x.Curr_isDeleted == false && x.Curr_isActive == true).ToList().AsQueryable();
            var properties = filters.CF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string[] split = propertyName.Split("_");
                var toStr = property.GetValue(filters.CF).ToString();
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if (colArr.Contains(split[1])) // IF INT VAL
                        {
                            mList = mList.Where("Curr_" + split[1] + " = @0 AND  Curr_isDeleted == @1", property.GetValue(filters.CF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (split[1] == "Creator_Name" || split[1] == "Approver_Name")
                        { //UPDATE THIS TO SEARCH FOR CREATOR OR APPROVER NAME IN USER TABLE
                            mList = mList.Where("Curr_" + split[1] + ".Contains(@0) AND  Curr_isDeleted == @1", property.GetValue(filters.CF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            mList = mList.Where("Curr_" + split[1] + ".Contains(@0) AND  Curr_isDeleted == @1", property.GetValue(filters.CF).ToString(), false)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            var pendingList = _context.DMCurrency_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMCurrencyModel[] {
                    new DMCurrencyModel
                    {
                       // Curr_ID = m.Pending_Curr_MasterID,
                        Curr_MasterID = m.Pending_Curr_MasterID,
                        Curr_Name = m.Pending_Curr_Name,
                        Curr_Creator_ID = m.Pending_Curr_Creator_ID,
                        Curr_Approver_ID = m.Pending_Curr_Approver_ID.Equals(null) ? 0 : m.Pending_Curr_Approver_ID,
                        Curr_Created_Date = m.Pending_Curr_Filed_Date,
                        Curr_Last_Updated = m.Pending_Curr_Filed_Date,
                        Curr_Status = m.Pending_Curr_Status
                    }
                });
            }
            var creatorList = (from a in mList
                               join b in _context.Account on a.Curr_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new
                               {
                                   a.Curr_ID,
                                   CreatorName
                               }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.Curr_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new
                              {
                                  a.Curr_ID,
                                  ApproverName
                              }).ToList();

            List<DMCurrencyViewModel> vmList = new List<DMCurrencyViewModel>();
            foreach (DMCurrencyModel m in mList)
            {
                var creator = creatorList.Where(a => a.Curr_ID == m.Curr_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Curr_ID == m.Curr_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMCurrencyViewModel vm = new DMCurrencyViewModel
                {
                    Curr_MasterID = m.Curr_MasterID,
                    Curr_Name = m.Curr_Name,
                    Curr_CCY_Code = m.Curr_CCY_Code,
                    Curr_Creator_Name = creator ?? "N/A",
                    Curr_Approver_Name = approver ?? "",
                    Curr_Created_Date = m.Curr_Created_Date,
                    Curr_Last_Updated = m.Curr_Last_Updated,
                    Curr_Status = m.Curr_Status
                };
                vmList.Add(vm);
            }
            return vmList;
        }

    }
}
