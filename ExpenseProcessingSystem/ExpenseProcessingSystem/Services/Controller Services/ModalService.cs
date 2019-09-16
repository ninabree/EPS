using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Models.Gbase;
using ExpenseProcessingSystem.Models.Pending;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using ExpenseProcessingSystem.ViewModels.NewRecord;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace ExpenseProcessingSystem.Services.Controller_Services
{
    public class ModalService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private readonly GWriteContext _gWriteContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public ModalService(IHttpContextAccessor httpContextAccessor, EPSDbContext context, GWriteContext gWriteContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _gWriteContext = gWriteContext;
        }
        public List<CONSTANT_NC_VALS> getInterEntityAccs(string Loc)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("wwwroot/xml/DDVInterEntityAccounts.xml");
            XmlNodeList nodeList = doc.SelectNodes(Loc);
            List<CONSTANT_NC_VALS> valList = new List<CONSTANT_NC_VALS>();
            foreach (XmlNode no in nodeList)
            {
                var rawVal = no.InnerText;
                var acc = _context.DMAccount.Where(x => (x.Account_MasterID == int.Parse(rawVal))
                                                    && x.Account_isActive == true && x.Account_isDeleted == false).FirstOrDefault();
                CONSTANT_NC_VALS vals = new CONSTANT_NC_VALS
                {
                    accID = acc.Account_ID,
                    accNo = acc.Account_No,
                    accName = acc.Account_Name
                };
                valList.Add(vals);
            }
            return valList;
        }
        //-----------------------ADMIN-------------------------
        // [DMVendor ]
        public List<DMVendorViewModel> approveVendor(string[] IdsArr)
        {
            List<DMVendorModel_Pending> pendingList = _context.DMVendor_Pending.Where(x => IdsArr.Contains(x.Pending_Vendor_MasterID.ToString())).Distinct().ToList();

            var trList = (from a in _context.DMVendorTRVAT_Pending
                          join c in _context.DMTR on a.Pending_VTV_TR_ID equals c.TR_MasterID
                          where IdsArr.Contains(a.Pending_VTV_Vendor_ID.ToString()) && c.TR_isActive == true
                          select new { a.Pending_VTV_Vendor_ID, c.TR_ID, c.TR_Tax_Rate, c.TR_WT_Title }).ToList();

            var vatList = (from a in _context.DMVendorTRVAT_Pending
                           join d in _context.DMVAT on a.Pending_VTV_VAT_ID equals d.VAT_MasterID
                           where IdsArr.Contains(a.Pending_VTV_Vendor_ID.ToString()) && d.VAT_isActive == true
                           select new { a.Pending_VTV_Vendor_ID, d.VAT_ID, d.VAT_Rate, d.VAT_Name }).ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Vendor_Status_ID equals d.Status_ID
                            select new { a.Pending_ID, d.Status_Name }).ToList();
            List<DMVendorViewModel> tempList = new List<DMVendorViewModel>();
            foreach (DMVendorModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Vendor_MasterID == int.Parse(s))
                    {
                        DMVendorViewModel vm = new DMVendorViewModel
                        {
                            Vendor_MasterID = m.Pending_Vendor_MasterID,
                            Vendor_Name = m.Pending_Vendor_Name,
                            Vendor_TIN = m.Pending_Vendor_TIN,
                            Vendor_Address = m.Pending_Vendor_Address,
                            Vendor_Creator_ID = m.Pending_Vendor_Creator_ID,
                            Vendor_Approver_ID = m.Pending_Vendor_Approver_ID.Equals(null) ? 0 : m.Pending_Vendor_Approver_ID,
                            Vendor_Created_Date = m.Pending_Vendor_Filed_Date,
                            Vendor_Last_Updated = m.Pending_Vendor_Filed_Date,
                            Vendor_Status = statList.Where(a => a.Pending_ID == m.Pending_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A",
                            Vendor_Tax_Rates = trList.Where(x => x.Pending_VTV_Vendor_ID == m.Pending_Vendor_MasterID).Select(x => (x.TR_Tax_Rate * 100) + "% - " + x.TR_WT_Title).ToList(),
                            Vendor_VAT = vatList.Where(x => x.Pending_VTV_Vendor_ID == m.Pending_Vendor_MasterID).Select(x => (x.VAT_Rate * 100) + "% - " + x.VAT_Name).ToList()
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMVendorViewModel> rejectVendor(string[] IdsArr)
        {
            List<DMVendorModel_Pending> pendingList = _context.DMVendor_Pending.Where(x => IdsArr.Contains(x.Pending_Vendor_MasterID.ToString())).Distinct().ToList();

            var trList = (from a in _context.DMVendorTRVAT_Pending
                          join c in _context.DMTR on a.Pending_VTV_TR_ID equals c.TR_MasterID
                          where IdsArr.Contains(a.Pending_VTV_Vendor_ID.ToString()) && c.TR_isActive == true
                          select new { a.Pending_VTV_Vendor_ID, c.TR_ID, c.TR_Tax_Rate, c.TR_WT_Title }).ToList();

            var vatList = (from a in _context.DMVendorTRVAT_Pending
                           join d in _context.DMVAT on a.Pending_VTV_VAT_ID equals d.VAT_MasterID
                           where IdsArr.Contains(a.Pending_VTV_Vendor_ID.ToString()) && d.VAT_isActive == true
                           select new { a.Pending_VTV_Vendor_ID, d.VAT_ID, d.VAT_Rate, d.VAT_Name }).ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Vendor_Status_ID equals d.Status_ID
                            select new { a.Pending_ID, d.Status_Name }).ToList();
            List<DMVendorViewModel> tempList = new List<DMVendorViewModel>();
            foreach (DMVendorModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Vendor_MasterID == int.Parse(s))
                    {
                        DMVendorViewModel vm = new DMVendorViewModel
                        {
                            Vendor_MasterID = m.Pending_Vendor_MasterID,
                            Vendor_Name = m.Pending_Vendor_Name,
                            Vendor_TIN = m.Pending_Vendor_TIN,
                            Vendor_Address = m.Pending_Vendor_Address,
                            Vendor_Creator_ID = m.Pending_Vendor_Creator_ID,
                            Vendor_Approver_ID = m.Pending_Vendor_Approver_ID.Equals(null) ? 0 : m.Pending_Vendor_Approver_ID,
                            Vendor_Created_Date = m.Pending_Vendor_Filed_Date,
                            Vendor_Last_Updated = m.Pending_Vendor_Filed_Date,
                            Vendor_Status = statList.Where(a => a.Pending_ID == m.Pending_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A",
                            Vendor_Tax_Rates = trList.Where(x => x.Pending_VTV_Vendor_ID == m.Pending_Vendor_MasterID).Select(x => (x.TR_Tax_Rate * 100) + "% - " + x.TR_WT_Title).ToList(),
                            Vendor_VAT = vatList.Where(x => x.Pending_VTV_Vendor_ID == m.Pending_Vendor_MasterID).Select(x => (x.VAT_Rate * 100) + "% - " + x.VAT_Name).ToList()
                        };
                    tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Department ]
        public List<DMDeptViewModel> approveDept(string[] IdsArr)
        {
            List<DMDeptModel_Pending> pendingList = _context.DMDept_Pending.Where(x => IdsArr.Contains(x.Pending_Dept_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Dept_Status_ID equals d.Status_ID
                            select new { a.Pending_Dept_ID, d.Status_Name }).ToList();
            List<DMDeptViewModel> tempList = new List<DMDeptViewModel>();
            foreach (DMDeptModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Dept_MasterID == int.Parse(s))
                    {
                        DMDeptViewModel vm = new DMDeptViewModel
                        {
                            Dept_MasterID = m.Pending_Dept_MasterID,
                            Dept_Name = m.Pending_Dept_Name,
                            Dept_Code = m.Pending_Dept_Code,
                            Dept_Budget_Unit = m.Pending_Dept_Budget_Unit,
                            Dept_Creator_ID = m.Pending_Dept_Creator_ID,
                            Dept_Approver_ID = m.Pending_Dept_Approver_ID.Equals(null) ? 0 : m.Pending_Dept_Approver_ID,
                            Dept_Created_Date = m.Pending_Dept_Filed_Date,
                            Dept_Last_Updated = m.Pending_Dept_Filed_Date,
                            Dept_Status = statList.Where(a => a.Pending_Dept_ID == m.Pending_Dept_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMDeptViewModel> rejectDept(string[] IdsArr)
        {
            List<DMDeptModel_Pending> pendingList = _context.DMDept_Pending.Where(x => IdsArr.Contains(x.Pending_Dept_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Dept_Status_ID equals d.Status_ID
                            select new { a.Pending_Dept_ID, d.Status_Name }).ToList();
            List<DMDeptViewModel> tempList = new List<DMDeptViewModel>();
            foreach (DMDeptModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Dept_MasterID == int.Parse(s))
                    {
                        DMDeptViewModel vm = new DMDeptViewModel
                        {
                            Dept_MasterID = m.Pending_Dept_MasterID,
                            Dept_Name = m.Pending_Dept_Name,
                            Dept_Code = m.Pending_Dept_Code,
                            Dept_Budget_Unit = m.Pending_Dept_Budget_Unit,
                            Dept_Creator_ID = m.Pending_Dept_Creator_ID,
                            Dept_Approver_ID = m.Pending_Dept_Approver_ID.Equals(null) ? 0 : m.Pending_Dept_Approver_ID,
                            Dept_Created_Date = m.Pending_Dept_Filed_Date,
                            Dept_Last_Updated = m.Pending_Dept_Filed_Date,
                            Dept_Status = statList.Where(a => a.Pending_Dept_ID == m.Pending_Dept_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Check ]
        public List<DMCheckViewModel> approveCheck(string[] IdsArr)
        {
            List<DMCheckModel_Pending> pendingList = _context.DMCheck_Pending.Where(x => IdsArr.Contains(x.Pending_Check_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Check_Status_ID equals d.Status_ID
                            select new { a.Pending_Check_ID, d.Status_Name }).ToList();
            List<DMCheckViewModel> tempList = new List<DMCheckViewModel>();
            foreach (DMCheckModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Check_MasterID == int.Parse(s))
                    {
                        DMCheckViewModel vm = new DMCheckViewModel
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
                            Check_Status = statList.Where(a => a.Pending_Check_ID == m.Pending_Check_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCheckViewModel> rejectCheck(string[] IdsArr)
        {
            List<DMCheckModel_Pending> pendingList = _context.DMCheck_Pending.Where(x => IdsArr.Contains(x.Pending_Check_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Check_Status_ID equals d.Status_ID
                            select new { a.Pending_Check_ID, d.Status_Name }).ToList();
            List<DMCheckViewModel> tempList = new List<DMCheckViewModel>();
            foreach (DMCheckModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Check_MasterID == int.Parse(s))
                    {
                        DMCheckViewModel vm = new DMCheckViewModel
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
                            Check_Status = statList.Where(a => a.Pending_Check_ID == m.Pending_Check_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Account ]
        public List<DMAccountViewModel> approveAccount(string[] IdsArr)
        {
            List<DMAccountModel_Pending> pendingList = _context.DMAccount_Pending.Where(x => IdsArr.Contains(x.Pending_Account_MasterID.ToString()))
                                                        .Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Account_Status_ID equals d.Status_ID
                            select new { a.Pending_Account_ID, d.Status_Name }).ToList();
            List<DMAccountViewModel> tempList = new List<DMAccountViewModel>();
            foreach (DMAccountModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Account_MasterID == int.Parse(s))
                    {
                        DMAccountViewModel vm = new DMAccountViewModel
                        {
                            Account_MasterID = m.Pending_Account_MasterID,
                            Account_FBT_Name = _context.DMFBT.Where(x => x.FBT_MasterID == m.Pending_Account_FBT_MasterID)
                                            .Select(x => x.FBT_Name).FirstOrDefault(),
                            Account_Group_Name = _context.DMAccountGroup.Where(x => x.AccountGroup_MasterID == m.Pending_Account_Group_MasterID)
                                            .Where(x => x.AccountGroup_isActive == true)
                                            .Select(x => x.AccountGroup_Name).FirstOrDefault(),
                            Account_Currency_Name = _context.DMCurrency.Where(x => x.Curr_MasterID == m.Pending_Account_Currency_MasterID)
                                            .Where(x => x.Curr_isActive == true)
                                            .Select(x => x.Curr_Name).FirstOrDefault(),
                            Account_Currency_MasterID = m.Pending_Account_Currency_MasterID,
                            Account_Name = m.Pending_Account_Name,
                            Account_Code = m.Pending_Account_Code,
                            Account_Budget_Code = m.Pending_Account_Budget_Code,
                            Account_Cust = m.Pending_Account_Cust,
                            Account_Div = m.Pending_Account_Div,
                            Account_Fund = m.Pending_Account_Fund,
                            Account_No = m.Pending_Account_No,
                            Account_Creator_ID = m.Pending_Account_Creator_ID,
                            Account_Approver_ID = m.Pending_Account_Approver_ID.Equals(null) ? 0 : m.Pending_Account_Approver_ID,
                            Account_Created_Date = m.Pending_Account_Filed_Date,
                            Account_Last_Updated = m.Pending_Account_Filed_Date,
                            Account_Status = statList.Where(a => a.Pending_Account_ID == m.Pending_Account_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMAccountViewModel> rejectAccount(string[] IdsArr)
        {
            List<DMAccountModel_Pending> pendingList = _context.DMAccount_Pending.Where(x => IdsArr.Contains(x.Pending_Account_MasterID.ToString()))
                                                        .Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Account_Status_ID equals d.Status_ID
                            select new { a.Pending_Account_ID, d.Status_Name }).ToList();
            List<DMAccountViewModel> tempList = new List<DMAccountViewModel>();
            foreach (DMAccountModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Account_MasterID == int.Parse(s))
                    {
                        DMAccountViewModel vm = new DMAccountViewModel
                        {
                            Account_MasterID = m.Pending_Account_MasterID,
                            Account_FBT_Name = _context.DMFBT.Where(x => x.FBT_MasterID == m.Pending_Account_FBT_MasterID)
                                            .Select(x => x.FBT_Name).FirstOrDefault(),
                            Account_Group_Name = _context.DMAccountGroup.Where(x => x.AccountGroup_MasterID == m.Pending_Account_Group_MasterID)
                                            .Where(x => x.AccountGroup_isActive == true)
                                            .Select(x => x.AccountGroup_Name).FirstOrDefault(),
                            Account_Currency_Name = _context.DMCurrency.Where(x => x.Curr_MasterID == m.Pending_Account_Currency_MasterID)
                                            .Where(x=> x.Curr_isActive == true)
                                            .Select(x => x.Curr_Name).FirstOrDefault(),
                            Account_Currency_MasterID =m.Pending_Account_Currency_MasterID,
                            Account_Name = m.Pending_Account_Name,
                            Account_Code = m.Pending_Account_Code,
                            Account_Budget_Code = m.Pending_Account_Budget_Code,
                            Account_Cust = m.Pending_Account_Cust,
                            Account_Div = m.Pending_Account_Div,
                            Account_Fund = m.Pending_Account_Fund,
                            Account_No = m.Pending_Account_No,
                            Account_Creator_ID = m.Pending_Account_Creator_ID,
                            Account_Approver_ID = m.Pending_Account_Approver_ID.Equals(null) ? 0 : m.Pending_Account_Approver_ID,
                            Account_Created_Date = m.Pending_Account_Filed_Date,
                            Account_Last_Updated = m.Pending_Account_Filed_Date,
                            Account_Status = statList.Where(a => a.Pending_Account_ID == m.Pending_Account_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Account Group ]
        public List<DMAccountGroupViewModel> approveAccountGroup(string[] IdsArr)
        {
            List<DMAccountGroupModel_Pending> pendingList = _context.DMAccountGroup_Pending.Where(x => IdsArr.Contains(x.Pending_AccountGroup_MasterID.ToString()))
                                                        .Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_AccountGroup_Status_ID equals d.Status_ID
                            select new { a.Pending_AccountGroup_ID, d.Status_Name }).ToList();
            List<DMAccountGroupViewModel> tempList = new List<DMAccountGroupViewModel>();
            foreach (DMAccountGroupModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_AccountGroup_MasterID == int.Parse(s))
                    {
                        DMAccountGroupViewModel vm = new DMAccountGroupViewModel
                        {
                            AccountGroup_MasterID = m.Pending_AccountGroup_MasterID,
                            AccountGroup_Name = m.Pending_AccountGroup_Name,
                            AccountGroup_Code = m.Pending_AccountGroup_Code,
                            AccountGroup_Creator_ID = m.Pending_AccountGroup_Creator_ID,
                            AccountGroup_Approver_ID = m.Pending_AccountGroup_Approver_ID.Equals(null) ? 0 : m.Pending_AccountGroup_Approver_ID,
                            AccountGroup_Created_Date = m.Pending_AccountGroup_Filed_Date,
                            AccountGroup_Last_Updated = m.Pending_AccountGroup_Filed_Date,
                            AccountGroup_Status_Name = statList.Where(a => a.Pending_AccountGroup_ID == m.Pending_AccountGroup_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMAccountGroupViewModel> rejectAccountGroup(string[] IdsArr)
        {
            List<DMAccountGroupModel_Pending> pendingList = _context.DMAccountGroup_Pending.Where(x => IdsArr.Contains(x.Pending_AccountGroup_MasterID.ToString()))
                                                        .Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_AccountGroup_Status_ID equals d.Status_ID
                            select new { a.Pending_AccountGroup_ID, d.Status_Name }).ToList();
            List<DMAccountGroupViewModel> tempList = new List<DMAccountGroupViewModel>();
            foreach (DMAccountGroupModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_AccountGroup_MasterID == int.Parse(s))
                    {
                        DMAccountGroupViewModel vm = new DMAccountGroupViewModel
                        {
                            AccountGroup_MasterID = m.Pending_AccountGroup_MasterID,
                            AccountGroup_Name = m.Pending_AccountGroup_Name,
                            AccountGroup_Code = m.Pending_AccountGroup_Code,
                            AccountGroup_Creator_ID = m.Pending_AccountGroup_Creator_ID,
                            AccountGroup_Approver_ID = m.Pending_AccountGroup_Approver_ID.Equals(null) ? 0 : m.Pending_AccountGroup_Approver_ID,
                            AccountGroup_Created_Date = m.Pending_AccountGroup_Filed_Date,
                            AccountGroup_Last_Updated = m.Pending_AccountGroup_Filed_Date,
                            AccountGroup_Status_Name = statList.Where(a => a.Pending_AccountGroup_ID == m.Pending_AccountGroup_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        // [DMVATModel ]
        public List<DMVATViewModel> approveVAT(string[] IdsArr)
        {
            List<DMVATModel_Pending> pendingList = _context.DMVAT_Pending.Where(x => IdsArr.Contains(x.Pending_VAT_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_VAT_Status_ID equals d.Status_ID
                            select new { a.Pending_VAT_ID, d.Status_Name }).ToList();
            List<DMVATViewModel> tempList = new List<DMVATViewModel>();
            foreach (DMVATModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_VAT_MasterID == int.Parse(s))
                    {
                        DMVATViewModel vm = new DMVATViewModel
                        {
                            VAT_MasterID = m.Pending_VAT_MasterID,
                            VAT_Name = m.Pending_VAT_Name,
                            VAT_Rate = m.Pending_VAT_Rate,
                            VAT_Creator_ID = m.Pending_VAT_Creator_ID,
                            VAT_Approver_ID = m.Pending_VAT_Approver_ID.Equals(null) ? 0 : m.Pending_VAT_Approver_ID,
                            VAT_Created_Date = m.Pending_VAT_Filed_Date,
                            VAT_Last_Updated = m.Pending_VAT_Filed_Date,
                            VAT_Status = statList.Where(a => a.Pending_VAT_ID == m.Pending_VAT_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMVATViewModel> rejectVAT(string[] IdsArr)
        {
            List<DMVATModel_Pending> pendingList = _context.DMVAT_Pending.Where(x => IdsArr.Contains(x.Pending_VAT_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_VAT_Status_ID equals d.Status_ID
                            select new { a.Pending_VAT_ID, d.Status_Name }).ToList();
            List<DMVATViewModel> tempList = new List<DMVATViewModel>();
            foreach (DMVATModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_VAT_MasterID == int.Parse(s))
                    {
                        DMVATViewModel vm = new DMVATViewModel
                        {
                            VAT_MasterID = m.Pending_VAT_MasterID,
                            VAT_Name = m.Pending_VAT_Name,
                            VAT_Rate = m.Pending_VAT_Rate,
                            VAT_Creator_ID = m.Pending_VAT_Creator_ID,
                            VAT_Approver_ID = m.Pending_VAT_Approver_ID.Equals(null) ? 0 : m.Pending_VAT_Approver_ID,
                            VAT_Created_Date = m.Pending_VAT_Filed_Date,
                            VAT_Last_Updated = m.Pending_VAT_Filed_Date,
                            VAT_Status = statList.Where(a => a.Pending_VAT_ID == m.Pending_VAT_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        // [DMFBTModel ]
        public List<DMFBTViewModel> approveFBT(string[] IdsArr)
        {
            List<DMFBTModel_Pending> pendingList = _context.DMFBT_Pending.Where(x => IdsArr.Contains(x.Pending_FBT_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_FBT_Status_ID equals d.Status_ID
                            select new { a.Pending_FBT_ID, d.Status_Name }).ToList();
            List<DMFBTViewModel> tempList = new List<DMFBTViewModel>();
            foreach (DMFBTModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_FBT_MasterID == int.Parse(s))
                    {
                        DMFBTViewModel vm = new DMFBTViewModel
                        {
                            FBT_MasterID = m.Pending_FBT_MasterID,
                            FBT_Name = m.Pending_FBT_Name,
                            FBT_Formula = m.Pending_FBT_Formula,
                            FBT_Tax_Rate = m.Pending_FBT_Tax_Rate,
                            FBT_Creator_ID = m.Pending_FBT_Creator_ID,
                            FBT_Approver_ID = m.Pending_FBT_Approver_ID.Equals(null) ? 0 : m.Pending_FBT_Approver_ID,
                            FBT_Created_Date = m.Pending_FBT_Filed_Date,
                            FBT_Last_Updated = m.Pending_FBT_Filed_Date,
                            FBT_Status = statList.Where(a => a.Pending_FBT_ID == m.Pending_FBT_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMFBTViewModel> rejectFBT(string[] IdsArr)
        {
            List<DMFBTModel_Pending> pendingList = _context.DMFBT_Pending.Where(x => IdsArr.Contains(x.Pending_FBT_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_FBT_Status_ID equals d.Status_ID
                            select new { a.Pending_FBT_ID, d.Status_Name }).ToList();
            List<DMFBTViewModel> tempList = new List<DMFBTViewModel>();
            foreach (DMFBTModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_FBT_MasterID == int.Parse(s))
                    {
                        DMFBTViewModel vm = new DMFBTViewModel
                        {
                            FBT_MasterID = m.Pending_FBT_MasterID,
                            FBT_Name = m.Pending_FBT_Name,
                            FBT_Formula = m.Pending_FBT_Formula,
                            FBT_Tax_Rate = m.Pending_FBT_Tax_Rate,
                            FBT_Creator_ID = m.Pending_FBT_Creator_ID,
                            FBT_Approver_ID = m.Pending_FBT_Approver_ID.Equals(null) ? 0 : m.Pending_FBT_Approver_ID,
                            FBT_Created_Date = m.Pending_FBT_Filed_Date,
                            FBT_Last_Updated = m.Pending_FBT_Filed_Date,
                            FBT_Status = statList.Where(a => a.Pending_FBT_ID == m.Pending_FBT_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        // [DMTRModel ]
        public List<DMTRViewModel> approveTR(string[] IdsArr)
        {
            List<DMTRModel_Pending> pendingList = _context.DMTR_Pending.Where(x => IdsArr.Contains(x.Pending_TR_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_TR_Status_ID equals d.Status_ID
                            select new { a.Pending_TR_ID, d.Status_Name }).ToList();
            List<DMTRViewModel> tempList = new List<DMTRViewModel>();
            foreach (DMTRModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_TR_MasterID == int.Parse(s))
                    {
                        DMTRViewModel vm = new DMTRViewModel
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
                            TR_Status = statList.Where(a => a.Pending_TR_ID == m.Pending_TR_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMTRViewModel> rejectTR(string[] IdsArr)
        {
            List<DMTRModel_Pending> pendingList = _context.DMTR_Pending.Where(x => IdsArr.Contains(x.Pending_TR_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_TR_Status_ID equals d.Status_ID
                            select new { a.Pending_TR_ID, d.Status_Name }).ToList();
            List<DMTRViewModel> tempList = new List<DMTRViewModel>();
            foreach (DMTRModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_TR_MasterID == int.Parse(s))
                    {
                        DMTRViewModel vm = new DMTRViewModel
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
                            TR_Status = statList.Where(a => a.Pending_TR_ID == m.Pending_TR_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        // [DMCurrModel ]
        public List<DMCurrencyViewModel> approveCurr(string[] IdsArr)
        {
            List<DMCurrencyModel_Pending> pendingList = _context.DMCurrency_Pending.Where(x => IdsArr.Contains(x.Pending_Curr_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Curr_Status_ID equals d.Status_ID
                            select new { a.Pending_Curr_ID, d.Status_Name }).ToList();
            List<DMCurrencyViewModel> tempList = new List<DMCurrencyViewModel>();
            foreach (DMCurrencyModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Curr_MasterID == int.Parse(s))
                    {
                        DMCurrencyViewModel vm = new DMCurrencyViewModel
                        {
                            Curr_MasterID = m.Pending_Curr_MasterID,
                            Curr_Name = m.Pending_Curr_Name,
                            Curr_CCY_ABBR = m.Pending_Curr_CCY_ABBR,
                            Curr_Creator_ID = m.Pending_Curr_Creator_ID,
                            Curr_Approver_ID = m.Pending_Curr_Approver_ID.Equals(null) ? 0 : m.Pending_Curr_Approver_ID,
                            Curr_Created_Date = m.Pending_Curr_Filed_Date,
                            Curr_Last_Updated = m.Pending_Curr_Filed_Date,
                            Curr_Status = statList.Where(a => a.Pending_Curr_ID == m.Pending_Curr_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCurrencyViewModel> rejectCurr(string[] IdsArr)
        {
            List<DMCurrencyModel_Pending> pendingList = _context.DMCurrency_Pending.Where(x => IdsArr.Contains(x.Pending_Curr_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Curr_Status_ID equals d.Status_ID
                            select new { a.Pending_Curr_ID, d.Status_Name }).ToList();
            List<DMCurrencyViewModel> tempList = new List<DMCurrencyViewModel>();
            foreach (DMCurrencyModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Curr_MasterID == int.Parse(s))
                    {
                        DMCurrencyViewModel vm = new DMCurrencyViewModel
                        {
                            Curr_MasterID = m.Pending_Curr_MasterID,
                            Curr_Name = m.Pending_Curr_Name,
                            Curr_CCY_ABBR = m.Pending_Curr_CCY_ABBR,
                            Curr_Creator_ID = m.Pending_Curr_Creator_ID,
                            Curr_Approver_ID = m.Pending_Curr_Approver_ID.Equals(null) ? 0 : m.Pending_Curr_Approver_ID,
                            Curr_Created_Date = m.Pending_Curr_Filed_Date,
                            Curr_Last_Updated = m.Pending_Curr_Filed_Date,
                            Curr_Status = statList.Where(a => a.Pending_Curr_ID == m.Pending_Curr_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Employee ]
        public List<DMEmpViewModel> approveEmp(string[] IdsArr)
        {
            List<DMEmpModel_Pending> pendingList = _context.DMEmp_Pending.Where(x => IdsArr.Contains(x.Pending_Emp_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Emp_Status_ID equals d.Status_ID
                            select new { a.Pending_Emp_ID, d.Status_Name }).ToList();
            List<DMEmpViewModel> tempList = new List<DMEmpViewModel>();
            foreach (DMEmpModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Emp_MasterID == int.Parse(s))
                    {
                        DMEmpViewModel vm = new DMEmpViewModel
                        {
                            Emp_MasterID = m.Pending_Emp_MasterID,
                            Emp_Name = m.Pending_Emp_Name,
                            Emp_FBT_MasterID = m.Pending_Emp_FBT_MasterID,
                            Emp_FBT_Name = _context.DMFBT.Where(x => x.FBT_MasterID == m.Pending_Emp_FBT_MasterID)
                                            .Select(x => x.FBT_Name).FirstOrDefault(),
                            Emp_Category_ID = m.Pending_Emp_Category_ID,
                            Emp_Category_Name = GlobalSystemValues.EMPCATEGORY_SELECT.Where(x => x.Value == m.Pending_Emp_Category_ID+"")
                                            .Select(x => x.Text).FirstOrDefault(),
                            Emp_Acc_No = m.Pending_Emp_Acc_No ?? "",
                            Emp_Creator_ID = m.Pending_Emp_Creator_ID,
                            Emp_Approver_ID = m.Pending_Emp_Approver_ID.Equals(null) ? 0 : m.Pending_Emp_Approver_ID,
                            Emp_Created_Date = m.Pending_Emp_Filed_Date,
                            Emp_Last_Updated = m.Pending_Emp_Filed_Date,
                            Emp_Status = statList.Where(a => a.Pending_Emp_ID == m.Pending_Emp_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMEmpViewModel> rejectEmp(string[] IdsArr)
        {
            List<DMEmpModel_Pending> pendingList = _context.DMEmp_Pending.Where(x => IdsArr.Contains(x.Pending_Emp_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Emp_Status_ID equals d.Status_ID
                            select new { a.Pending_Emp_ID, d.Status_Name }).ToList();
            List<DMEmpViewModel> tempList = new List<DMEmpViewModel>();
            foreach (DMEmpModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Emp_MasterID == int.Parse(s))
                    {
                        DMEmpViewModel vm = new DMEmpViewModel
                        {
                            Emp_MasterID = m.Pending_Emp_MasterID,
                            Emp_Name = m.Pending_Emp_Name,
                            Emp_Acc_No = m.Pending_Emp_Acc_No ?? "",
                            Emp_FBT_MasterID = m.Pending_Emp_FBT_MasterID,
                            Emp_FBT_Name = _context.DMFBT.Where(x => x.FBT_MasterID == m.Pending_Emp_FBT_MasterID)
                                            .Select(x => x.FBT_Name).FirstOrDefault(),
                            Emp_Category_ID = m.Pending_Emp_Category_ID,
                            Emp_Category_Name = GlobalSystemValues.EMPCATEGORY_SELECT.Where(x => x.Value == m.Pending_Emp_Category_ID + "")
                                            .Select(x => x.Text).FirstOrDefault(),
                            Emp_Creator_ID = m.Pending_Emp_Creator_ID,
                            Emp_Approver_ID = m.Pending_Emp_Approver_ID.Equals(null) ? 0 : m.Pending_Emp_Approver_ID,
                            Emp_Created_Date = m.Pending_Emp_Filed_Date,
                            Emp_Last_Updated = m.Pending_Emp_Filed_Date,
                            Emp_Status = statList.Where(a => a.Pending_Emp_ID == m.Pending_Emp_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Customer ]
        public List<DMCustViewModel> approveCust(string[] IdsArr)
        {
            List<DMCustModel_Pending> pendingList = _context.DMCust_Pending.Where(x => IdsArr.Contains(x.Pending_Cust_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Cust_Status_ID equals d.Status_ID
                            select new { a.Pending_Cust_ID, d.Status_Name }).ToList();
            List<DMCustViewModel> tempList = new List<DMCustViewModel>();
            foreach (DMCustModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Cust_MasterID == int.Parse(s))
                    {
                        DMCustViewModel vm = new DMCustViewModel
                        {
                            Cust_MasterID = m.Pending_Cust_MasterID,
                            Cust_Name = m.Pending_Cust_Name,
                            Cust_Abbr = m.Pending_Cust_Abbr,
                            Cust_No = m.Pending_Cust_No,
                            Cust_Creator_ID = m.Pending_Cust_Creator_ID,
                            Cust_Approver_ID = m.Pending_Cust_Approver_ID.Equals(null) ? 0 : m.Pending_Cust_Approver_ID,
                            Cust_Created_Date = m.Pending_Cust_Filed_Date,
                            Cust_Last_Updated = m.Pending_Cust_Filed_Date,
                            Cust_Status = statList.Where(a => a.Pending_Cust_ID == m.Pending_Cust_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCustViewModel> rejectCust(string[] IdsArr)
        {
            List<DMCustModel_Pending> pendingList = _context.DMCust_Pending.Where(x => IdsArr.Contains(x.Pending_Cust_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_Cust_Status_ID equals d.Status_ID
                            select new { a.Pending_Cust_ID, d.Status_Name }).ToList();
            List<DMCustViewModel> tempList = new List<DMCustViewModel>();
            foreach (DMCustModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Cust_MasterID == int.Parse(s))
                    {
                        DMCustViewModel vm = new DMCustViewModel
                        {
                            Cust_MasterID = m.Pending_Cust_MasterID,
                            Cust_Name = m.Pending_Cust_Name,
                            Cust_Abbr = m.Pending_Cust_Abbr,
                            Cust_No = m.Pending_Cust_No,
                            Cust_Creator_ID = m.Pending_Cust_Creator_ID,
                            Cust_Approver_ID = m.Pending_Cust_Approver_ID.Equals(null) ? 0 : m.Pending_Cust_Approver_ID,
                            Cust_Created_Date = m.Pending_Cust_Filed_Date,
                            Cust_Last_Updated = m.Pending_Cust_Filed_Date,
                            Cust_Status = statList.Where(a => a.Pending_Cust_ID == m.Pending_Cust_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ BIR Cert Signatory ]
        public List<DMBCSViewModel> approveBCS(string[] IdsArr)
        {
            List<DMBIRCertSignModel_Pending> pendingList = _context.DMBCS_Pending.Where(x => IdsArr.Contains(x.Pending_BCS_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_BCS_Status_ID equals d.Status_ID
                            select new { a.Pending_BCS_ID, d.Status_Name }).ToList();
            List<DMBCSViewModel> tempList = new List<DMBCSViewModel>();
            foreach (DMBIRCertSignModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_BCS_MasterID == int.Parse(s))
                    {
                        DMBCSViewModel vm = new DMBCSViewModel
                        {
                            BCS_MasterID = m.Pending_BCS_MasterID,
                            BCS_User_ID = m.Pending_BCS_User_ID,
                            BCS_Name = getBCSNamePending(m.Pending_BCS_ID),
                            BCS_TIN = m.Pending_BCS_TIN,
                            BCS_Position = m.Pending_BCS_Position,
                            BCS_Signatures = m.Pending_BCS_Signatures,
                            BCS_Creator_ID = m.Pending_BCS_Creator_ID,
                            BCS_Approver_ID = m.Pending_BCS_Approver_ID.Equals(null) ? 0 : m.Pending_BCS_Approver_ID,
                            BCS_Created_Date = m.Pending_BCS_Filed_Date,
                            BCS_Last_Updated = m.Pending_BCS_Filed_Date,
                            BCS_Status = statList.Where(a => a.Pending_BCS_ID == m.Pending_BCS_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMBCSViewModel> rejectBCS(string[] IdsArr)
        {
            List<DMBIRCertSignModel_Pending> pendingList = _context.DMBCS_Pending.Where(x => IdsArr.Contains(x.Pending_BCS_MasterID.ToString())).Distinct().ToList();
            var statList = (from a in pendingList
                            join d in _context.StatusList on a.Pending_BCS_Status_ID equals d.Status_ID
                            select new { a.Pending_BCS_ID, d.Status_Name }).ToList();
            List<DMBCSViewModel> tempList = new List<DMBCSViewModel>();
            foreach (DMBIRCertSignModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_BCS_MasterID == int.Parse(s))
                    {
                        DMBCSViewModel vm = new DMBCSViewModel
                        {
                            BCS_MasterID = m.Pending_BCS_MasterID,
                            BCS_User_ID = m.Pending_BCS_User_ID,
                            BCS_Name = getBCSNamePending(m.Pending_BCS_ID),
                            BCS_TIN = m.Pending_BCS_TIN,
                            BCS_Position = m.Pending_BCS_Position,
                            BCS_Signatures = m.Pending_BCS_Signatures,
                            BCS_Creator_ID = m.Pending_BCS_Creator_ID,
                            BCS_Approver_ID = m.Pending_BCS_Approver_ID.Equals(null) ? 0 : m.Pending_BCS_Approver_ID,
                            BCS_Created_Date = m.Pending_BCS_Filed_Date,
                            BCS_Last_Updated = m.Pending_BCS_Filed_Date,
                            BCS_Status = statList.Where(a => a.Pending_BCS_ID == m.Pending_BCS_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //------------------------PENDING-----------------------------
        // [Add New Pending Entries]
        public NewVendorListViewModel addVendor()
        {
            NewVendorListViewModel mod = new NewVendorListViewModel();
            List<NewVendorViewModel> vmList = new List<NewVendorViewModel>();
            NewVendorViewModel vm = new NewVendorViewModel();
            vmList.Add(vm);
            mod.NewVendorVM = vmList;
            mod.Vendor_Tax_Rates = getTRList();
            mod.Vendor_VAT = getVATList();
            return mod;
        }
        public NewDeptListViewModel addDept()
        {
            NewDeptListViewModel mod = new NewDeptListViewModel();
            List<NewDeptViewModel> vmList = new List<NewDeptViewModel>();
            NewDeptViewModel vm = new NewDeptViewModel();
            vmList.Add(vm);
            mod.NewDeptVM = vmList;
            return mod;
        }
        public NewCheckListViewModel addCheck()
        {
            NewCheckListViewModel mod = new NewCheckListViewModel();
            List<NewCheckViewModel> vmList = new List<NewCheckViewModel>();
            NewCheckViewModel vm = new NewCheckViewModel
            {
                Check_Input_Date = DateTime.Now
            };
            vmList.Add(vm);
            mod.NewCheckVM = vmList;
            return mod;
        }
        public NewAccountListViewModel addAccount()
        {
            NewAccountListViewModel mod = new NewAccountListViewModel();
            List<NewAccountViewModel> vmList = new List<NewAccountViewModel>();
            NewAccountViewModel vm = new NewAccountViewModel();
            vmList.Add(vm);
            mod.NewAccountVM = vmList;
            mod.FbtList = getFbtSelectList();
            mod.AccGrp = getAccGroupSelectList();
            mod.CurrList = getCurrencySelectList();
            return mod;
        }
        public NewAccountGroupListViewModel addAccountGroup()
        {
            NewAccountGroupListViewModel mod = new NewAccountGroupListViewModel();
            List<NewAccountGroupViewModel> vmList = new List<NewAccountGroupViewModel>();
            NewAccountGroupViewModel vm = new NewAccountGroupViewModel();
            vmList.Add(vm);
            mod.NewAccountGroupVM = vmList;
            return mod;
        }
        public NewVATListViewModel addVAT()
        {
            NewVATListViewModel mod = new NewVATListViewModel();
            List<NewVATViewModel> vmList = new List<NewVATViewModel>();
            NewVATViewModel vm = new NewVATViewModel();
            vmList.Add(vm);
            mod.NewVATVM = vmList;
            return mod;
        }
        public NewFBTListViewModel addFBT()
        {
            NewFBTListViewModel mod = new NewFBTListViewModel();
            List<NewFBTViewModel> vmList = new List<NewFBTViewModel>();
            NewFBTViewModel vm = new NewFBTViewModel
            {
                FBT_Tax_Rate = 0
            };
            vmList.Add(vm);
            mod.NewFBTVM = vmList;
            return mod;
        }
        public NewTRListViewModel addTR()
        {
            NewTRListViewModel mod = new NewTRListViewModel();
            List<NewTRViewModel> vmList = new List<NewTRViewModel>();
            NewTRViewModel vm = new NewTRViewModel
            {
                TR_Tax_Rate = 0
            };
            vmList.Add(vm);
            mod.NewTRVM = vmList;
            return mod;
        }
        public NewCurrListViewModel addCurr()
        {
            NewCurrListViewModel mod = new NewCurrListViewModel();
            List<NewCurrViewModel> vmList = new List<NewCurrViewModel>();
            NewCurrViewModel vm = new NewCurrViewModel();
            vmList.Add(vm);
            mod.NewCurrVM = vmList;
            return mod;
        }
        public NewEmpListViewModel addEmp()
        {
            NewEmpListViewModel mod = new NewEmpListViewModel();
            List<NewEmpViewModel> vmList = new List<NewEmpViewModel>();
            NewEmpViewModel vm = new NewEmpViewModel()
            {
                Emp_FBT_MasterID = 0
            };
            vmList.Add(vm);
            mod.NewEmpVM = vmList;
            mod.FbtList = getFbtSelectList();
            mod.CatList = getEmpCategorySelectList();
            return mod;
        }
        public NewCustListViewModel addCust()
        {
            NewCustListViewModel mod = new NewCustListViewModel();
            List<NewCustViewModel> vmList = new List<NewCustViewModel>();
            NewCustViewModel vm = new NewCustViewModel();
            vmList.Add(vm);
            mod.NewCustVM = vmList;
            return mod;
        }
        public NewBCSViewModel addBCS()
        {
            NewBCSViewModel vm = new NewBCSViewModel();
            return vm;
        }
        // [Update Pending Entries]
        public List<DMVendorViewModel> editDeleteVendor(string[] IdsArr)
        {
            List<DMVendorModel> mList = _context.DMVendor.Where(x => IdsArr.Contains(x.Vendor_MasterID.ToString()) 
                                       && x.Vendor_isActive == true).Distinct().ToList();
            var trList = (from a in _context.DMVendorTRVAT
                          join c in _context.DMTR on a.VTV_TR_ID equals c.TR_MasterID
                          where IdsArr.Contains(a.VTV_Vendor_ID.ToString()) && c.TR_isActive == true
                          select new { a.VTV_Vendor_ID, c.TR_ID, c.TR_Tax_Rate, c.TR_WT_Title }).ToList();

            var vatList = (from a in _context.DMVendorTRVAT
                           join d in _context.DMVAT on a.VTV_VAT_ID equals d.VAT_MasterID
                           where IdsArr.Contains(a.VTV_Vendor_ID.ToString()) && d.VAT_isActive == true
                           select new { a.VTV_Vendor_ID, d.VAT_ID, d.VAT_Rate, d.VAT_Name }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Vendor_Status_ID equals d.Status_ID
                            select new { a.Vendor_ID, d.Status_Name }).ToList();

            List<DMVendorViewModel> tempList = new List<DMVendorViewModel>();
            foreach (DMVendorModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Vendor_MasterID == int.Parse(s))
                    {
                        DMVendorViewModel vm = new DMVendorViewModel
                        {
                            Vendor_MasterID = m.Vendor_MasterID,
                            Vendor_Name = m.Vendor_Name,
                            Vendor_TIN = m.Vendor_TIN,
                            Vendor_Address = m.Vendor_Address,
                            Vendor_Creator_ID = m.Vendor_Creator_ID,
                            Vendor_Created_Date = m.Vendor_Created_Date,
                            Vendor_Last_Updated = DateTime.Now,
                            Vendor_Status_ID = m.Vendor_Status_ID,
                            Vendor_Status = statList.Where(a => a.Vendor_ID == m.Vendor_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A",
                            Vendor_Tax_Rates = trList.Where(x => x.VTV_Vendor_ID == m.Vendor_MasterID).Select(x => (x.TR_Tax_Rate * 100) + "% - " + x.TR_WT_Title).ToList(),
                            Vendor_VAT = vatList.Where(x => x.VTV_Vendor_ID == m.Vendor_MasterID).Select(x => (x.VAT_Rate * 100) + "% - " + x.VAT_Name).ToList()
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMDeptViewModel> editDeleteDept(string[] IdsArr)
        {
            List<DMDeptModel> mList = _context.DMDept.Where(x => IdsArr.Contains(x.Dept_MasterID.ToString())
                                        && x.Dept_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Dept_Status_ID equals d.Status_ID
                            select new { a.Dept_ID, d.Status_Name }).ToList();
            List<DMDeptViewModel> tempList = new List<DMDeptViewModel>();
            foreach (DMDeptModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Dept_MasterID == int.Parse(s))
                    {
                        DMDeptViewModel vm = new DMDeptViewModel
                        {
                            Dept_MasterID = m.Dept_MasterID,
                            Dept_Name = m.Dept_Name,
                            Dept_Code = m.Dept_Code,
                            Dept_Budget_Unit = m.Dept_Budget_Unit,
                            Dept_Creator_ID = m.Dept_Creator_ID,
                            Dept_Created_Date = m.Dept_Created_Date,
                            Dept_Last_Updated = DateTime.Now,
                            Dept_Status = statList.Where(a => a.Dept_ID == m.Dept_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCheckViewModel> editDeleteCheck(string[] IdsArr)
        {
            List<DMCheckModel> mList = _context.DMCheck.Where(x => IdsArr.Contains(x.Check_MasterID.ToString())
                                        && x.Check_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Check_Status_ID equals d.Status_ID
                            select new { a.Check_ID, d.Status_Name }).ToList();
            List<DMCheckViewModel> tempList = new List<DMCheckViewModel>();
            foreach (DMCheckModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Check_MasterID == int.Parse(s))
                    {
                        DMCheckViewModel vm = new DMCheckViewModel
                        {
                            Check_MasterID = m.Check_MasterID,
                            Check_Input_Date = m.Check_Input_Date,
                            Check_Series_From = m.Check_Series_From,
                            Check_Series_To = m.Check_Series_To,
                            Check_Bank_Info = m.Check_Bank_Info,
                            Check_Creator_ID = m.Check_Creator_ID,
                            Check_Approver_ID = m.Check_Approver_ID,
                            Check_Created_Date = m.Check_Created_Date,
                            Check_Last_Updated = DateTime.Now,
                            Check_Status = statList.Where(a => a.Check_ID == m.Check_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMAccountViewModel> editDeleteAccount(string[] IdsArr)
        {
            List<DMAccountModel> mList = _context.DMAccount.Where(x => IdsArr.Contains(x.Account_MasterID.ToString())
                                       && x.Account_isActive == true).Distinct().ToList();
            var fbtList = (from a in mList
                           join d in _context.DMFBT on a.Account_FBT_MasterID equals d.FBT_MasterID
                           select new { a.Account_ID, d.FBT_Name, d.FBT_MasterID }).ToList();
            var groupList = (from a in mList
                             join d in _context.DMAccountGroup on a.Account_Group_MasterID equals d.AccountGroup_MasterID
                             select new { a.Account_ID, d.AccountGroup_Name, d.AccountGroup_MasterID }).ToList();
            var currList = (from a in mList
                             join d in _context.DMCurrency on a.Account_Currency_MasterID equals d.Curr_MasterID
                            where d.Curr_isActive == true
                            select new { a.Account_ID, d.Curr_Name, d.Curr_MasterID }).ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Account_Status_ID equals d.Status_ID
                            select new { a.Account_ID, d.Status_Name }).ToList();
            //TEMP where clause until FBT is updated
            var defaultFBT = _context.DMFBT.Where(x => x.FBT_MasterID == 1).FirstOrDefault();

            List<DMAccountViewModel> tempList = new List<DMAccountViewModel>();
            foreach (DMAccountModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Account_MasterID == int.Parse(s))
                    {
                        var fbt = fbtList.Where(a => a.Account_ID == m.Account_ID).FirstOrDefault();
                        var group = groupList.Where(a => a.Account_ID == m.Account_ID).FirstOrDefault();
                        var curr = currList.Where(a => a.Account_ID == m.Account_ID).FirstOrDefault();
                        DMAccountViewModel vm = new DMAccountViewModel
                        {
                            Account_MasterID = m.Account_MasterID,
                            Account_Name = m.Account_Name,
                            Account_FBT_MasterID = (fbt == null) || (fbt.FBT_MasterID.Equals(0)) ? defaultFBT.FBT_MasterID : fbt.FBT_MasterID,
                            Account_FBT_Name = (fbt == null) || (fbt.FBT_Name == null) ? defaultFBT.FBT_Name : fbt.FBT_Name,
                            Account_No = m.Account_No,
                            Account_Code = m.Account_Code,
                            Account_Budget_Code = m.Account_Budget_Code,
                            Account_Cust = m.Account_Cust,
                            Account_Div = m.Account_Div,
                            Account_Fund = m.Account_Fund,
                            Account_Group_MasterID = (group == null) || (group.AccountGroup_MasterID.Equals(0)) ? 0 : group.AccountGroup_MasterID,
                            Account_Group_Name = groupList.Where(a => a.Account_ID == m.Account_ID).Select(x=> x.AccountGroup_Name).FirstOrDefault() ?? "",
                            Account_Currency_MasterID = (curr == null) || (curr.Curr_MasterID.Equals(0)) ? 0 : curr.Curr_MasterID,
                            Account_Currency_Name = currList.Where(a => a.Account_ID == m.Account_ID).Select(x => x.Curr_Name).FirstOrDefault() ?? "",
                            Account_Creator_ID = m.Account_Creator_ID,
                            Account_Created_Date = m.Account_Created_Date,
                            Account_Last_Updated = DateTime.Now,
                            Account_Status = statList.Where(a => a.Account_ID == m.Account_ID).Select(x=> x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMAccountGroupViewModel> editDeleteAccountGroup(string[] IdsArr)
        {
            List<DMAccountGroupModel> mList = _context.DMAccountGroup.Where(x => IdsArr.Contains(x.AccountGroup_MasterID.ToString())
                                       && x.AccountGroup_isActive == true).Distinct().ToList();
            //TEMP where clause until FBT is updated
            var defaultFBT = _context.DMFBT.Where(x => x.FBT_MasterID == 1).FirstOrDefault();

            List<DMAccountGroupViewModel> tempList = new List<DMAccountGroupViewModel>();
            foreach (DMAccountGroupModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.AccountGroup_MasterID == int.Parse(s))
                    {
                        DMAccountGroupViewModel vm = new DMAccountGroupViewModel
                        {
                            AccountGroup_MasterID = m.AccountGroup_MasterID,
                            AccountGroup_Name = m.AccountGroup_Name,
                            AccountGroup_Code = m.AccountGroup_Code,
                            AccountGroup_Creator_ID = m.AccountGroup_Creator_ID,
                            AccountGroup_Created_Date = m.AccountGroup_Created_Date,
                            AccountGroup_Last_Updated = DateTime.Now,
                            AccountGroup_Status_ID = m.AccountGroup_Status_ID
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMVATViewModel> editDeleteVAT(string[] IdsArr)
        {
            List<DMVATModel> mList = _context.DMVAT.Where(x => IdsArr.Contains(x.VAT_MasterID.ToString())
                                       && x.VAT_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.VAT_Status_ID equals d.Status_ID
                            select new { a.VAT_ID, d.Status_Name }).ToList();
            List<DMVATViewModel> tempList = new List<DMVATViewModel>();
            foreach (DMVATModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.VAT_MasterID == int.Parse(s))
                    {
                        DMVATViewModel vm = new DMVATViewModel
                        {
                            VAT_MasterID = m.VAT_MasterID,
                            VAT_Name = m.VAT_Name,
                            VAT_Rate = m.VAT_Rate,
                            VAT_Creator_ID = m.VAT_Creator_ID,
                            VAT_Created_Date = m.VAT_Created_Date,
                            VAT_Last_Updated = DateTime.Now,
                            VAT_Status = statList.Where(a => a.VAT_ID == m.VAT_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMFBTViewModel> editDeleteFBT(string[] IdsArr)
        {
            List<DMFBTModel> mList = _context.DMFBT.Where(x => IdsArr.Contains(x.FBT_MasterID.ToString())
                                       && x.FBT_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.FBT_Status_ID equals d.Status_ID
                            select new { a.FBT_ID, d.Status_Name }).ToList();
            List<DMFBTViewModel> tempList = new List<DMFBTViewModel>();
            foreach (DMFBTModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.FBT_MasterID == int.Parse(s))
                    {
                        DMFBTViewModel vm = new DMFBTViewModel
                        {
                            FBT_MasterID = m.FBT_MasterID,
                            FBT_Name = m.FBT_Name,
                            FBT_Formula = m.FBT_Formula,
                            FBT_Tax_Rate = m.FBT_Tax_Rate,
                            FBT_Creator_ID = m.FBT_Creator_ID,
                            FBT_Created_Date = m.FBT_Created_Date,
                            FBT_Last_Updated = DateTime.Now,
                            FBT_Status = statList.Where(a => a.FBT_ID == m.FBT_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMTRViewModel> editDeleteTR(string[] IdsArr)
        {
            List<DMTRModel> mList = _context.DMTR.Where(x => IdsArr.Contains(x.TR_MasterID.ToString())
                                       && x.TR_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.TR_Status_ID equals d.Status_ID
                            select new { a.TR_ID, d.Status_Name }).ToList();
            List<DMTRViewModel> tempList = new List<DMTRViewModel>();
            foreach (DMTRModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.TR_MasterID == int.Parse(s))
                    {
                        DMTRViewModel vm = new DMTRViewModel
                        {
                            TR_MasterID = m.TR_MasterID,
                            TR_WT_Title = m.TR_WT_Title,
                            TR_Nature = m.TR_Nature,
                            TR_Tax_Rate = m.TR_Tax_Rate,
                            TR_ATC = m.TR_ATC,
                            TR_Nature_Income_Payment = m.TR_Nature_Income_Payment,
                            TR_Creator_ID = m.TR_Creator_ID,
                            TR_Created_Date = m.TR_Created_Date,
                            TR_Last_Updated = DateTime.Now,
                            TR_Status = statList.Where(a => a.TR_ID == m.TR_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCurrencyViewModel> editDeleteCurr(string[] IdsArr)
        {
            List<DMCurrencyModel> mList = _context.DMCurrency.Where(x => IdsArr.Contains(x.Curr_MasterID.ToString())
                                       && x.Curr_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Curr_Status_ID equals d.Status_ID
                            select new { a.Curr_ID, d.Status_Name }).ToList();
            List<DMCurrencyViewModel> tempList = new List<DMCurrencyViewModel>();
            foreach (DMCurrencyModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Curr_MasterID == int.Parse(s))
                    {
                        DMCurrencyViewModel vm = new DMCurrencyViewModel
                        {
                            Curr_MasterID = m.Curr_MasterID,
                            Curr_Name = m.Curr_Name,
                            Curr_CCY_ABBR = m.Curr_CCY_ABBR,
                            Curr_Creator_ID = m.Curr_Creator_ID,
                            Curr_Created_Date = m.Curr_Created_Date,
                            Curr_Last_Updated = DateTime.Now,
                            Curr_Status = statList.Where(a => a.Curr_ID == m.Curr_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMEmpViewModel> editDeleteEmp(string[] IdsArr)
        {
            List<DMEmpModel> mList = _context.DMEmp.Where(x => IdsArr.Contains(x.Emp_MasterID.ToString())
                                        && x.Emp_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Emp_Status_ID equals d.Status_ID
                            select new { a.Emp_ID, d.Status_Name }).ToList();
            var FBTList = (from a in mList
                           join d in _context.DMFBT on a.Emp_FBT_MasterID equals d.FBT_MasterID
                           where d.FBT_isActive == true && d.FBT_isDeleted == false
                           select new { a.Emp_ID, d.FBT_Name }).ToList();
            List<DMEmpViewModel> tempList = new List<DMEmpViewModel>();
            foreach (DMEmpModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Emp_MasterID == int.Parse(s))
                    {
                        DMEmpViewModel vm = new DMEmpViewModel
                        {
                            Emp_MasterID = m.Emp_MasterID,
                            Emp_Name = m.Emp_Name,
                            Emp_Acc_No = m.Emp_Acc_No,
                            Emp_Category_ID = m.Emp_Category_ID,
                            Emp_FBT_MasterID = m.Emp_FBT_MasterID,
                            Emp_Category_Name = GlobalSystemValues.EMPCATEGORY_SELECT.Where(a => a.Value == m.Emp_Category_ID + "").Select(x => x.Text).FirstOrDefault() ?? "",
                            Emp_FBT_Name = FBTList.Where(a => a.Emp_ID == m.Emp_ID).Select(x => x.FBT_Name).FirstOrDefault() ?? "N/A",
                            Emp_Creator_ID = m.Emp_Creator_ID,
                            Emp_Created_Date = m.Emp_Created_Date,
                            Emp_Last_Updated = DateTime.Now,
                            Emp_Status = statList.Where(a => a.Emp_ID == m.Emp_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCustViewModel> editDeleteCust(string[] IdsArr)
        {
            List<DMCustModel> mList = _context.DMCust.Where(x => IdsArr.Contains(x.Cust_MasterID.ToString())
                                        && x.Cust_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.Cust_Status_ID equals d.Status_ID
                            select new { a.Cust_ID, d.Status_Name }).ToList();
            List<DMCustViewModel> tempList = new List<DMCustViewModel>();
            foreach (DMCustModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Cust_MasterID == int.Parse(s))
                    {
                        DMCustViewModel vm = new DMCustViewModel
                        {
                            Cust_MasterID = m.Cust_MasterID,
                            Cust_Name = m.Cust_Name,
                            Cust_Abbr = m.Cust_Abbr,
                            Cust_No = m.Cust_No,
                            Cust_Creator_ID = m.Cust_Creator_ID,
                            Cust_Created_Date = m.Cust_Created_Date,
                            Cust_Last_Updated = DateTime.Now,
                            Cust_Status = statList.Where(a => a.Cust_ID == m.Cust_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public DMBCS2ViewModel editBCS(string[] IdsArr)
        {
            List<DMBIRCertSignModel> mList = _context.DMBCS.Where(x => IdsArr.Contains(x.BCS_MasterID.ToString())
                                        && x.BCS_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.BCS_Status_ID equals d.Status_ID
                            select new { a.BCS_ID, d.Status_Name }).ToList();
            List<DMBCS2ViewModel> tempList = new List<DMBCS2ViewModel>();

            DMBCS2ViewModel vm = new DMBCS2ViewModel();
            vm = new DMBCS2ViewModel
            {
                BCS_MasterID = mList.First().BCS_MasterID,
                BCS_User_ID = mList.First().BCS_User_ID,
                BCS_Name = getBCSName(mList.First().BCS_ID),
                BCS_TIN = mList.First().BCS_TIN,
                BCS_Position = mList.First().BCS_Position,
                BCS_Signatures_Name = mList.First().BCS_Signatures,
                BCS_Creator_ID = mList.First().BCS_Creator_ID,
                BCS_Created_Date = mList.First().BCS_Created_Date,
                BCS_Last_Updated = DateTime.Now,
                BCS_Status = statList.Where(a => a.BCS_ID == a.BCS_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
            };
            return vm;
        }
        public List<DMBCSViewModel> deleteBCS(string[] IdsArr)
        {
            List<DMBIRCertSignModel> mList = _context.DMBCS.Where(x => IdsArr.Contains(x.BCS_MasterID.ToString())
                                        && x.BCS_isActive == true).Distinct().ToList();
            var statList = (from a in mList
                            join d in _context.StatusList on a.BCS_Status_ID equals d.Status_ID
                            select new { a.BCS_ID, d.Status_Name }).ToList();
            List<DMBCSViewModel> tempList = new List<DMBCSViewModel>();

            DMBCSViewModel vm = new DMBCSViewModel();
            foreach (DMBIRCertSignModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.BCS_MasterID == int.Parse(s))
                    {
                        vm = new DMBCSViewModel
                        {
                            BCS_MasterID = m.BCS_MasterID,
                            BCS_User_ID = m.BCS_User_ID,
                            BCS_Name = getBCSName(m.BCS_ID),
                            BCS_TIN = m.BCS_TIN,
                            BCS_Position = m.BCS_Position,
                            BCS_Signatures = m.BCS_Signatures,
                            BCS_Creator_ID = m.BCS_Creator_ID,
                            BCS_Created_Date = m.BCS_Created_Date,
                            BCS_Last_Updated = DateTime.Now,
                            BCS_Status = statList.Where(a => a.BCS_ID == m.BCS_ID).Select(x => x.Status_Name).FirstOrDefault() ?? "N/A"
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        //-----------------------------------------------------------------
        //Dropdown Select List
        public List<SelectListItem> getFbtSelectList()
        {
            List<SelectListItem> fbtList = new List<SelectListItem>();
            fbtList.Add(new SelectListItem() { Text = "-- Select FBT Applicable --", Value = "0", Selected = true  });
            _context.DMFBT.Where(x => x.FBT_isDeleted == false && x.FBT_isActive == true).ToList().ForEach(x => {
                fbtList.Add(new SelectListItem() { Text = x.FBT_Name, Value = x.FBT_MasterID + "" });
            });
            return fbtList;
        }
        public List<SelectListItem> getEmpSelectList()
        {
            List<SelectListItem> empList = new List<SelectListItem>();
            _context.User.Where(x => x.User_InUse == true && (x.User_Role != "admin" || x.User_Role != "maker")).ToList().ForEach(x => {
                empList.Add(new SelectListItem() { Text = x.User_LName+", " + x.User_FName, Value = x.User_ID + "" });
            });
            return empList;
        }
        public List<SelectListItem> getEmpCategorySelectList()
        {
            return GlobalSystemValues.EMPCATEGORY_SELECT;
        }
        public List<SelectListItem> getAccGroupSelectList()
        {
            List<SelectListItem> grpList = new List<SelectListItem>();
            grpList.Add(new SelectListItem() { Text = "-- Select Account Group Applicable --", Value = "0", Selected = true });
            _context.DMAccountGroup.Where(x => x.AccountGroup_isDeleted == false && x.AccountGroup_isActive == true).ToList().ForEach(x => {
                grpList.Add(new SelectListItem() { Text = x.AccountGroup_Name, Value = x.AccountGroup_MasterID + "" });
            });
            return grpList;
        }
        public List<SelectListItem> getCurrencySelectList()
        {
            List<SelectListItem> currList = new List<SelectListItem>();
            _context.DMCurrency.Where(x => x.Curr_isDeleted == false && x.Curr_isActive == true).ToList().ForEach(x => {
                currList.Add(new SelectListItem() { Text = x.Curr_CCY_ABBR, Value = x.Curr_MasterID + "" });
            });
            return currList;
        }
        public List<SelectListItem> getCurrencyIDSelectList(int Curr1ID)
        {
            List<SelectListItem> currList = new List<SelectListItem>();
            _context.DMCurrency.Where(x => x.Curr_isDeleted == false && x.Curr_isActive == true && x.Curr_ID != Curr1ID).ToList().ForEach(x => {
                currList.Add(new SelectListItem() { Text = x.Curr_CCY_ABBR, Value = x.Curr_ID + "" });
            });
            return currList;
        }

        public List<SelectListItem> getVendorSelectList()
        {
            List<SelectListItem> venList = new List<SelectListItem>();
            _context.DMVendor.Where(x => x.Vendor_isDeleted == false && x.Vendor_isActive == true).ToList().ForEach(x => {
                venList.Add(new SelectListItem() { Text = x.Vendor_Name, Value = x.Vendor_ID + "" });
            });
            return venList;
        }
        public List<DMTRViewModel> getTRList()
        {
            List<DMTRViewModel> grpList = new List<DMTRViewModel>();
            _context.DMTR.Where(x => x.TR_isDeleted == false && x.TR_isActive == true).ToList().ForEach(x => {
                grpList.Add(new DMTRViewModel() { TR_WT_Title = (x.TR_Tax_Rate * 100) + "% - " + x.TR_WT_Title, TR_MasterID = x.TR_MasterID });
            });
            return grpList;
        }
        public List<DMVATViewModel> getVATList()
        {
            List<DMVATViewModel> grpList = new List<DMVATViewModel>();
            _context.DMVAT.Where(x => x.VAT_isDeleted == false && x.VAT_isActive == true).ToList().ForEach(x => {
                grpList.Add(new DMVATViewModel() { VAT_Name = (x.VAT_Rate * 100) + "% - " + x.VAT_Name, VAT_MasterID = x.VAT_MasterID });
            });
            return grpList;
        }
        
        public SelectList getAccountSelectList()
        {
            var select = new SelectList(_context.DMAccount.Where(x => x.Account_isActive == true
                                                 && x.Account_isDeleted == false)
                                          .OrderByDescending(x => x.Account_No.Contains("H90"))
                                          .ThenBy(x => x.Account_No)
                                          .Select(q => new {q.Account_ID, name = (q.Account_No + " - " + q.Account_Name)}),
            "Account_ID", "name");
            return select;
        }


        public string GetAccountName(string id)
        {
            return _context.DMAccount.Where(x => x.Account_ID == Int64.Parse(id)).Single().Account_Name;
        }

        //[ Budget Monitoring ]
        public void AddNewBudget(List<BMViewModel> vmList, int userid, string gwriteUsername, string gwritePassword)
        {
            string gCommand = "";
            TblRequestDetails gwriteDtl = new TblRequestDetails();
            TblRequestItem gwriteItem = new TblRequestItem();
            List<BudgetModel> dbList = new List<BudgetModel>();

            var list = new[] {
                new { listBudget = new BudgetModel(), listGwriteDtl = new TblRequestDetails() }
            }.ToList();
            list.Clear();
            //Get all current budget information.
            var allCurrBudgetInfo = _context.Budget.Where(x => x.Budget_IsActive == true && x.Budget_isDeleted == false).ToList();
            //Get all "Fund == True" account information.
            var allAccountInfo = _context.DMAccount.Where(x => x.Account_isActive == true && x.Account_isDeleted == false
                                                        && x.Account_Fund == true ).ToList();
            //Get user account information.
            var userInfo = _context.User.Where(x => x.User_ID == userid).FirstOrDefault();

            //Change IsActive and IsDeleted status of all changed budget information
            if(allCurrBudgetInfo.Count() != 0) { 
                foreach (var i in vmList)
                {
                    if(i.BM_Budget_Current != i.BM_Budget_Amount && i.BM_GWrite_StatusID != GlobalSystemValues.STATUS_PENDING)
                    {
                        var currentInfo = allCurrBudgetInfo.Where(x => x.Budget_Account_MasterID == i.BM_Account_MasterID).FirstOrDefault();
                        if(currentInfo != null)
                        {
                            currentInfo.Budget_IsActive = false;
                            currentInfo.Budget_isDeleted = true;
                            _context.Entry(currentInfo).State = EntityState.Modified;
                        }
                    }
                }
                _context.SaveChanges();
            }

            //Add all inputted budget information.
            //Insert command to G-Write side.
            //Status of inputted budget will be pending.
            foreach(var i in vmList)
            {
                BudgetModel dbBudget = new BudgetModel();
                if (i.BM_Budget_Current != i.BM_Budget_Amount && i.BM_GWrite_StatusID != GlobalSystemValues.STATUS_PENDING)
                {
                    var acc = allAccountInfo.Where(x => x.Account_ID == i.BM_Account_ID).FirstOrDefault();

                    dbBudget.Budget_Account_ID = i.BM_Account_ID;
                    dbBudget.Budget_Account_MasterID = i.BM_Account_MasterID;
                    dbBudget.Budget_Amount = i.BM_Budget_Current;
                    dbBudget.Budget_Creator_ID = userid;
                    dbBudget.Budget_IsActive = true;
                    dbBudget.Budget_isDeleted = false;
                    dbBudget.Budget_Date_Registered = DateTime.Now;
                    dbBudget.Budget_GWrite_Status = GlobalSystemValues.STATUS_PENDING;
                    dbBudget.Budget_New_Amount = i.BM_Budget_Amount;

                    dbList.Add(dbBudget);
                    
                    gCommand = "cm00@E"
                                + acc.Account_No.Replace("-", "").Substring(3, 3)
                                +"1@E@E11"
                                + gwriteUsername.Substring(gwriteUsername.Length - 4)
                                + "@E5@E1"
                                + acc.Account_Budget_Code
                                + "@E"
                                + @String.Format("{0:N}", i.BM_Budget_Amount)
                                + "@E@E";

                    gwriteDtl = postToGwrite(gCommand, gwriteUsername, gwritePassword);

                    list.Add(new { listBudget = dbBudget, listGwriteDtl = gwriteDtl });

                }
            }
            _context.Budget.AddRange(dbList);
            _gWriteContext.SaveChanges();
            _context.SaveChanges();

            List<GwriteTransList> gtransList = new List<GwriteTransList>();
            foreach (var i in list)
            {
                gtransList.Add(new GwriteTransList
                {
                    GW_GWrite_ID = int.Parse(i.listGwriteDtl.RequestId.ToString()),
                    GW_TransID = int.Parse(i.listBudget.Budget_ID.ToString()),
                    GW_Status = GlobalSystemValues.STATUS_PENDING,
                    GW_Type = "budget"
                });
            }
            _context.GwriteTransLists.AddRange(gtransList);
            _context.SaveChanges();
        }

        public void ReSendNewBudget(int userid, string gwriteUsername, string gwritePassword)
        {
            string gCommand = "";
            TblRequestDetails gwriteDtl = new TblRequestDetails();
            TblRequestItem gwriteItem = new TblRequestItem();
            List<BudgetModel> dbList = _context.Budget.Where(x => x.Budget_GWrite_Status == GlobalSystemValues.STATUS_ERROR).ToList();

            var list = new[] {
                new { listBudget = new BudgetModel(), listGwriteDtl = new TblRequestDetails() }
            }.ToList();
            list.Clear();

            //Get all "Fund == True" account information.
            var allAccountInfo = _context.DMAccount.Where(x => x.Account_isActive == true && x.Account_isDeleted == false
                                                        && x.Account_Fund == true).ToList();

            //Add all inputted budget information.
            //Insert command to G-Write side.
            //Status of inputted budget will be pending.
            foreach (var i in dbList)
            {
                var acc = allAccountInfo.Where(x => x.Account_ID == i.Budget_Account_ID).FirstOrDefault();

                i.Budget_GWrite_Status = GlobalSystemValues.STATUS_PENDING;

                gCommand = "cm00@E"
                            + acc.Account_No.Replace("-", "").Substring(3, 3)
                            + "1@E@E11"
                            + gwriteUsername.Substring(gwriteUsername.Length - 4)
                            + "@E5@E1"
                            + acc.Account_Budget_Code
                            + "@E"
                            + @String.Format("{0:N}", i.Budget_New_Amount)
                            + "@E@E";

                gwriteDtl = postToGwrite(gCommand, gwriteUsername, gwritePassword);

                list.Add(new { listBudget = i, listGwriteDtl = gwriteDtl });

            }
            _context.Budget.UpdateRange(dbList);
            _gWriteContext.SaveChanges();
            _context.SaveChanges();

            List<GwriteTransList> gtransList = new List<GwriteTransList>();
            foreach (var i in list)
            {
                gtransList.Add(new GwriteTransList
                {
                    GW_GWrite_ID = int.Parse(i.listGwriteDtl.RequestId.ToString()),
                    GW_TransID = int.Parse(i.listBudget.Budget_ID.ToString()),
                    GW_Status = GlobalSystemValues.STATUS_PENDING,
                    GW_Type = "budget"
                });
            }
            _context.GwriteTransLists.AddRange(gtransList);
            _context.SaveChanges();
        }

        public IEnumerable<DMAccountModel> GetAccountListForBudgetMonitoring()
        {
            return _context.DMAccount.Where(x => x.Account_Fund == true && x.Account_isActive == true 
                                            && x.Account_isDeleted == false).ToList().OrderBy(x => x.Account_Budget_Code);
        }

        public decimal GetCurrentBudget(int accountMasterID)
        {
            return _context.Budget.Where(x => x.Budget_Account_MasterID == accountMasterID
            && x.Budget_IsActive == true && x.Budget_isDeleted == false).DefaultIfEmpty(
                new BudgetModel { Budget_Amount = 0.00M }).First().Budget_Amount;
        }

        public List<BudgetModel> GetAllCurrentBudget()
        {
            return _context.Budget.Where(x => x.Budget_IsActive == true && x.Budget_isDeleted == false).ToList();
        }
        public IEnumerable<BMViewModel> PopulateBudgetRegHist(int? year)
        {
            List<BMViewModel> bmvmList = new List<BMViewModel>();

            var dbBudget = (from bud in _context.Budget
                            join acc in _context.DMAccount on bud.Budget_Account_ID equals acc.Account_ID
                            join accGrp in _context.DMAccountGroup on acc.Account_Group_MasterID equals accGrp.AccountGroup_MasterID
                            join user in _context.User on bud.Budget_Creator_ID equals user.User_ID
                            where accGrp.AccountGroup_isActive == true && accGrp.AccountGroup_isDeleted == false &&
                            acc.Account_Fund == true && bud.Budget_Date_Registered.Year == year
                            select new
                            {
                                bud.Budget_ID,
                                accGrp.AccountGroup_Name,
                                acc.Account_Name,
                                acc.Account_Budget_Code,
                                acc.Account_No,
                                bud.Budget_Amount,
                                bud.Budget_Date_Registered,
                                user.User_LName,
                                user.User_FName
                            }).ToList().OrderByDescending(x => x.Budget_Date_Registered).ThenBy(x => x.Account_Name);

            foreach (var i in dbBudget)
            {
                bmvmList.Add(new BMViewModel()
                {
                    BM_Budget_ID = i.Budget_ID,
                    BM_Acc_Group_Name = i.AccountGroup_Name,
                    BM_Acc_Name = i.Account_Name,
                    BM_GBase_Code = i.Account_Budget_Code,
                    BM_Acc_Num = i.Account_No,
                    BM_Budget_Amount = i.Budget_Amount,
                    BM_Date_Registered = i.Budget_Date_Registered,
                    BM_Creator_Name = i.User_LName + ", " + i.User_FName
                });
            };

            return bmvmList;
        }

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
            rqDtlModel.Status = "WAITING";
            rqDtlModel.SystemAbbr = "EXPRESS";
            rqDtlModel.Priority = 1;

            rqItemModel.SequenceNo = 1;
            rqItemModel.ReturnFlag = true;
            rqItemModel.Command = command;

            rqDtlModel.tblRequestItems.Add(rqItemModel);

            _gWriteContext.Add(rqDtlModel);

            return rqDtlModel;
        }
        //====================================================

        //get bcs name
        public string getBCSName(int id)
        {
            var name = _context.DMBCS.Where(q => q.BCS_ID == id).Join(_context.User, b => b.BCS_User_ID,
                e => e.User_ID, (b, e) => new DMBCSViewModel
                { BCS_Name = e.User_LName + ", " + e.User_FName }).SingleOrDefault();

            if (name == null)
            {
                return null;
            }

            return name.BCS_Name.ToUpper();
        }
        public string getBCSNamePending(int id)
        {
            var name = _context.DMBCS_Pending.Where(q => q.Pending_BCS_ID == id).Join(_context.User, b => b.Pending_BCS_User_ID,
                e => e.User_ID, (b, e) => new DMBCSViewModel
                { BCS_Name = e.User_LName + ", " + e.User_FName }).SingleOrDefault();

            if (name == null)
            {
                return null;
            }

            return name.BCS_Name.ToUpper();
        }

        //=============[Get Pettycash]==============
        public PettyCashModel getPC(string command)
        {
            PettyCashModel model = null;

            switch (command)
            {
                case "StartPettyCash":
                    var modStart = _context.PettyCash.OrderByDescending(x => x.PC_ID).Take(2);
                    if (modStart.FirstOrDefault() != null)
                    {
                        if (modStart.FirstOrDefault().PC_Status == GlobalSystemValues.STATUS_OPEN)
                        {
                            if (modStart.Skip(1).FirstOrDefault() != null)
                                model = modStart.Skip(1).FirstOrDefault();
                            else
                                model = new PettyCashModel();
                        }
                        else
                        {
                            model = modStart.FirstOrDefault();
                        }
                    }
                    else
                    {
                        model = new PettyCashModel();
                    }
                    break;
                case "ClosePettyCash":
                    var modClose = _context.PettyCash.OrderByDescending(x => x.PC_ID).FirstOrDefault();
                    if (modClose != null)
                        model = modClose;
                    break;
            }
            return model;
        }
        public bool confirmPC(int userID)
        {
            var lastPC = _context.PettyCash.OrderByDescending(x => x.PC_ID).FirstOrDefault();

            lastPC.PC_Status = GlobalSystemValues.STATUS_CLOSED;
            lastPC.PC_CloseDate = DateTime.Now;
            lastPC.PC_CloseUser = userID;

            PettyCashModel newPC = new PettyCashModel
            {
                PC_OpenDate = DateTime.Now,
                PC_OpenUser = userID,
                PC_StartBal = lastPC.PC_EndBal,
                PC_EndBal = lastPC.PC_EndBal,
                PC_OpenConfirm = true,
                PC_Status = GlobalSystemValues.STATUS_OPEN
            };

            _context.PettyCash.Add(newPC);

            _context.SaveChanges();

            return true;
        }
        public bool saveBrkDwnPC(ClosingBrkDwnViewModel model)
        {
            PettyCashModel pcModel = _context.PettyCash.OrderByDescending(x => x.PC_ID).FirstOrDefault();

            pcModel.PCB_OneThousand = model.CBD_oneK;
            pcModel.PCB_FiveHundred = model.CBD_fiveH;
            pcModel.PCB_TwoHundred = model.CBD_twoH;
            pcModel.PCB_OneHundred = model.CBD_oneH;
            pcModel.PCB_Fifty = model.CBD_fifty;
            pcModel.PCB_Twenty = model.CBD_twenty;
            pcModel.PCB_Ten = model.CBD_ten;
            pcModel.PCB_Five = model.CBD_five;
            pcModel.PCB_One = model.CBD_one;
            pcModel.PCB_TwentyFiveCents = model.CBD_c25;
            pcModel.PCB_TenCents = model.CBD_c10;
            pcModel.PCB_FiveCents = model.CBD_c5;
            pcModel.PCB_OneCents = model.CBD_c1;

            _context.SaveChanges();

            return true;
        }
        public bool lastPCEntry()
        {
            var lastEntry = _context.PettyCash.OrderByDescending(x => x.PC_ID).Select(x => x.PC_Status).FirstOrDefault();

            if (lastEntry == GlobalSystemValues.STATUS_CLOSED)
                return true;
            else
                return false;
        }
    }
}
