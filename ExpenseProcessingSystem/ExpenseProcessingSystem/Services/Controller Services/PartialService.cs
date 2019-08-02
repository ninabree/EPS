using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using ExpenseProcessingSystem.ViewModels.Search_Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Xml;

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
        public List<CONSTANT_NC_VALS> getNCAccs(string Loc)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("wwwroot/xml/NonCashAccounts.xml");
            //var xLSPayroll = xelem.Element("LSPAYROLL").Value;
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
        public List<CONSTANT_NC_VALS> getNCAccsForFilter()
        {
            List<CONSTANT_NC_VALS> ts = new List<CONSTANT_NC_VALS>
            {
                //accId is really MasterId
                new CONSTANT_NC_VALS()
                {
                    accID = 68,
                    accName = "COMPUTER SUSPENSE"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = 76,
                    accName = "COMPUTER SUSPENSE (US$)"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = 72,
                    accName = "INTER ENTITY REG to FCDU"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = 77,
                    accName = "	INTER ENTITY FCDU to REG"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = 95,
                    accName = "PETTY CASH"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = 98,
                    accName = "EWT TAX"
                }
            };
            List<CONSTANT_NC_VALS> valList = new List<CONSTANT_NC_VALS>();
            foreach (CONSTANT_NC_VALS val in ts)
            {
                var acc = _context.DMAccount.Where(x => (x.Account_MasterID == val.accID)
                                                    && x.Account_isActive == true && x.Account_isDeleted == false).FirstOrDefault();
                CONSTANT_NC_VALS vals = new CONSTANT_NC_VALS
                {
                    accID = acc.Account_ID,
                    accNo = acc.Account_MasterID+"",
                    accName = acc.Account_Name
                };
                valList.Add(vals);
            }
            return valList;
        }
        //----------------------------------- [[ Populate Non Cash ]] -------------------------------------////
        // [RETRIEVE NC EXPENSE DETAILS]
        public EntryNCViewModelList getExpenseNC(int transID)
        {
            EntryNCViewModel ncVM = new EntryNCViewModel();

            var EntryDetails = (from e
                                in _context.ExpenseEntry
                                where e.Expense_ID == transID
                                select new
                                {
                                    e,
                                    ExpenseEntryNCModel = from d
                                                          in _context.ExpenseEntryNonCash
                                                          where d.ExpenseEntryModel.Expense_ID == e.Expense_ID
                                                          select new
                                                          {
                                                              d,
                                                              ExpenseEntryNCDtlModel = from g
                                                                                      in _context.ExpenseEntryNonCashDetails
                                                                                    where g.ExpenseEntryNCModel.ExpNC_ID == d.ExpNC_ID
                                                                                       select new
                                                                                       {
                                                                                           g,
                                                                                           ExpenseEntryNCDtlAccModel = from a
                                                                                                                       in _context.ExpenseEntryNonCashDetailAccounts
                                                                                                                       where a.ExpenseEntryNCDtlModel.ExpNCDtl_ID == g.ExpNCDtl_ID
                                                                                                                       select a
                                                                                       }

                                                          }
                                }).FirstOrDefault();

            foreach (var nc in EntryDetails.ExpenseEntryNCModel)
            {

                
                //NC Details
                List<ExpenseEntryNCDtlViewModel> ncDtlList = new List<ExpenseEntryNCDtlViewModel>();
                ExpenseEntryNCDtlViewModel ncDtlVM = new ExpenseEntryNCDtlViewModel();
                //NC Detail Accounts
                List<ExpenseEntryNCDtlAccViewModel> ncDtlAccList = new List<ExpenseEntryNCDtlAccViewModel>();
                ExpenseEntryNCDtlAccViewModel ncDtlAccVM = new ExpenseEntryNCDtlAccViewModel();

                foreach (var ncDtl in nc.ExpenseEntryNCDtlModel)
                {
                    ncDtlAccList = new List<ExpenseEntryNCDtlAccViewModel>();
                    foreach (var ncDtlAcc in ncDtl.ExpenseEntryNCDtlAccModel)
                    {
                        ncDtlAccVM = new ExpenseEntryNCDtlAccViewModel()
                        {
                            ExpNCDtlAcc_Acc_ID = ncDtlAcc.ExpNCDtlAcc_Acc_ID,
                            ExpNCDtlAcc_Acc_Name = ncDtlAcc.ExpNCDtlAcc_Acc_Name,
                            ExpNCDtlAcc_Curr_ID = ncDtlAcc.ExpNCDtlAcc_Curr_ID,
                            ExpNCDtlAcc_Curr_Name = _context.DMCurrency.Where(x=> x.Curr_ID == ncDtlAcc.ExpNCDtlAcc_Curr_ID).Select(x=> x.Curr_CCY_ABBR).FirstOrDefault(),
                            ExpNCDtlAcc_Inter_Rate = ncDtlAcc.ExpNCDtlAcc_Inter_Rate,
                            ExpNCDtlAcc_Amount = ncDtlAcc.ExpNCDtlAcc_Amount,
                            ExpNCDtlAcc_Type_ID = ncDtlAcc.ExpNCDtlAcc_Type_ID
                        };
                        ncDtlAccList.Add(ncDtlAccVM);
                    }

                    ncDtlVM = new ExpenseEntryNCDtlViewModel()
                    {
                        ExpNCDtl_Remarks_Desc = ncDtl.g.ExpNCDtl_Remarks_Desc,
                        ExpNCDtl_Remarks_Period = ncDtl.g.ExpNCDtl_Remarks_Period,
                        ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>()
                    };
                    if (ncDtlVM.ExpenseEntryNCDtlAccs.Count <= 0)
                    {
                        ncDtlVM.ExpenseEntryNCDtlAccs = ncDtlAccList;
                    }
                    ncDtlList.Add(ncDtlVM);
                }


                ncVM = new EntryNCViewModel()
                {
                    NC_Category_ID = nc.d.ExpNC_Category_ID,
                    ExpenseEntryNCDtls = ncDtlList
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
                EntryNC = ncVM
            };

            return ncModel;
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

        public List<SelectListItem> getAccountSelectList()
        {
            List<SelectListItem> selList = new List<SelectListItem>();
            _context.DMAccount.Where(x => x.Account_isDeleted == false && x.Account_isActive == true).ToList().ForEach(x => {
                selList.Add(new SelectListItem() { Text = x.Account_No + " - " + x.Account_Name, Value = x.Account_ID + "" });
            });
            return selList;
        }

        public List<SelectListItem> getCurrencySelectList()
        {
            List<SelectListItem> currList = new List<SelectListItem>();
            _context.DMCurrency.Where(x => x.Curr_isDeleted == false && x.Curr_isActive == true).ToList().ForEach(x => {
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

        public List<SelectListItem> getTaxRateSelectList()
        {
            List<SelectListItem> trList = new List<SelectListItem>();
            _context.DMTR.Where(x => x.TR_isDeleted == false && x.TR_isActive == true).ToList().ForEach(x => {
                trList.Add(new SelectListItem() { Text = x.TR_WT_Title+" - " + x.TR_Tax_Rate, Value = x.TR_ID + "" });
            });
            return trList;
        }
        //----------------------------------- [[ Populate DM ]] -------------------------------------//
        public List<DMVendorViewModel> populateVendor(DMFiltersViewModel filters)
        {
            var properties = filters.PF.GetType().GetProperties();
            var mList = (from rate in (from ven in _context.DMVendor
                                       from trvat in _context.DMVendorTRVAT
                                       where ven.Vendor_MasterID == trvat.VTV_Vendor_ID
                                       && ven.Vendor_isDeleted == false && ven.Vendor_isActive == true
                                       select new
                                       {
                                           ven.Vendor_ID,
                                           ven.Vendor_MasterID,
                                           ven.Vendor_TIN,
                                           ven.Vendor_Address,
                                           ven.Vendor_Name,
                                           trvat.VTV_TR_ID,
                                           trvat.VTV_VAT_ID,
                                           ven.Vendor_Creator_ID,
                                           ven.Vendor_Approver_ID,
                                           ven.Vendor_Status_ID,
                                           ven.Vendor_Created_Date,
                                           ven.Vendor_Last_Updated
                                       })
                         join tr in _context.DMTR
                         on new { masterID = rate.VTV_TR_ID, isActive = true, isDeleted = false }
                         equals new { masterID = tr.TR_MasterID, isActive = tr.TR_isActive, isDeleted = tr.TR_isDeleted }
                         into withTr
                         from tr in withTr.DefaultIfEmpty()
                         join vat in _context.DMVAT
                         on new { masterID = rate.VTV_VAT_ID, isActive = true, isDeleted = false }
                         equals new { masterID = vat.VAT_MasterID, isActive = vat.VAT_isActive, isDeleted = vat.VAT_isDeleted }
                         into withVat
                         from vat in withVat.DefaultIfEmpty()
                         join user in _context.User
                         on rate.Vendor_Creator_ID
                         equals user.User_ID
                         into c
                         from user in c.DefaultIfEmpty()
                         join apprv in _context.User
                         on rate.Vendor_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         join stat in _context.StatusList
                         on rate.Vendor_Status_ID
                         equals stat.Status_ID
                         into s
                         from stat in s.DefaultIfEmpty()
                         select new
                         {
                             rate.Vendor_ID,
                             rate.Vendor_MasterID,
                             rate.Vendor_Name,
                             rate.Vendor_TIN,
                             rate.Vendor_Address,
                             rate.VTV_TR_ID,
                             rate.VTV_VAT_ID,
                             tr.TR_WT_Title,
                             TRRAte = tr.TR_Tax_Rate > 0 ? tr.TR_Tax_Rate : 0,
                             vat.VAT_Name,
                             VATRAte = vat.VAT_Rate > 0 ? vat.VAT_Rate : 0,
                             rate.Vendor_Creator_ID,
                             ApproverID = rate.Vendor_Approver_ID,
                             CreatorName = user.User_LName + ", " + user.User_FName,
                             ApproverName = rate.Vendor_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             rate.Vendor_Created_Date,
                             rate.Vendor_Last_Updated,
                             stat.Status_ID,
                             stat.Status_Name
                         }).ToList();

            var pendingList = (from rate in (from ven in _context.DMVendor_Pending
                                             select new
                                             {
                                                 ven.Pending_ID,
                                                 ven.Pending_Vendor_MasterID,
                                                 ven.Pending_Vendor_Name,
                                                 ven.Pending_Vendor_TIN,
                                                 ven.Pending_Vendor_Address,
                                                 ven.Pending_Vendor_Creator_ID,
                                                 ven.Pending_Vendor_Approver_ID,
                                                 ven.Pending_Vendor_Status_ID,
                                                 ven.Pending_Vendor_Filed_Date
                                             })
                               join trvat in _context.DMVendorTRVAT_Pending
                               on rate.Pending_Vendor_MasterID
                               equals trvat.Pending_VTV_Vendor_ID
                               into withTrVat
                               from trvat in withTrVat.DefaultIfEmpty()
                               join tr in _context.DMTR
                               on new { masterID = trvat != null ? trvat.Pending_VTV_TR_ID : 0, isActive = true, isDeleted = false }
                               equals new { masterID = tr.TR_MasterID, isActive = tr.TR_isActive, isDeleted = tr.TR_isDeleted }
                               into withTr
                               from tr in withTr.DefaultIfEmpty()
                               join vat in _context.DMVAT
                               on new { masterID = trvat != null ? trvat.Pending_VTV_VAT_ID : 0, isActive = true, isDeleted = false }
                               equals new { masterID = vat.VAT_MasterID, isActive = vat.VAT_isActive, isDeleted = vat.VAT_isDeleted }
                               into withVat
                               from vat in withVat.DefaultIfEmpty()
                               join user in _context.User
                               on rate.Pending_Vendor_Creator_ID
                               equals user.User_ID
                               into c
                               from user in c.DefaultIfEmpty()
                               join apprv in _context.User
                               on rate.Pending_Vendor_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               join stat in _context.StatusList
                               on rate.Pending_Vendor_Status_ID
                               equals stat.Status_ID
                               into s
                               from stat in s.DefaultIfEmpty()
                               select new
                               {
                                   rate.Pending_ID,
                                   rate.Pending_Vendor_MasterID,
                                   rate.Pending_Vendor_Name,
                                   VTV_TRID = trvat != null ? trvat.Pending_VTV_TR_ID : 0,
                                   VTV_VATID = trvat != null ? trvat.Pending_VTV_VAT_ID : 0,
                                   tr.TR_WT_Title,
                                   TRRAte = tr.TR_Tax_Rate > 0 ? tr.TR_Tax_Rate : 0,
                                   vat.VAT_Name,
                                   VATRAte = vat.VAT_Rate > 0 ? vat.VAT_Rate : 0,
                                   rate.Pending_Vendor_TIN,
                                   rate.Pending_Vendor_Address,
                                   rate.Pending_Vendor_Creator_ID,
                                   rate.Pending_Vendor_Approver_ID,
                                   CreatorName = user.User_LName + ", " + user.User_FName,
                                   ApproverName = rate.Pending_Vendor_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   Vendor_Created_Date = rate.Pending_Vendor_Filed_Date,
                                   Vendor_Last_Updated = rate.Pending_Vendor_Filed_Date,
                                   stat.Status_ID,
                                   stat.Status_Name
                               }).ToList();

            List<DMVendorViewModel> vmList = new List<DMVendorViewModel>();
            mList.GroupBy(o=> o.Vendor_ID).Select(o=> o.FirstOrDefault()).ToList().ForEach(m =>
                vmList.Add(new DMVendorViewModel
                {
                    Vendor_MasterID = m.Vendor_MasterID,
                    Vendor_Name = m.Vendor_Name,
                    Vendor_TIN = m.Vendor_TIN,
                    Vendor_Address = m.Vendor_Address,
                    Vendor_Creator_Name = m.CreatorName ?? "N/A",
                    Vendor_Approver_Name = m.ApproverName ?? "",
                    Vendor_Created_Date = m.Vendor_Created_Date,
                    Vendor_Creator_ID = m.Vendor_Creator_ID,
                    Vendor_Last_Updated = m.Vendor_Last_Updated,
                    Vendor_Status_ID = m.Status_ID,
                    Vendor_Status = m.Status_Name ?? "N/A",
                    Vendor_Tax_Rates = mList.Where(x=> x.Vendor_ID == m.Vendor_ID).Select(x => (x.TRRAte * 100) + "% - " + x.TR_WT_Title).ToList(),
                    Vendor_VAT = mList.Where(x => x.Vendor_ID == m.Vendor_ID).Select(x => (x.VATRAte * 100) + "% - " + x.VAT_Name).ToList()
                })
            );

            pendingList.GroupBy(o => o.Pending_ID).Select(o => o.FirstOrDefault()).ToList().ForEach(m =>
                 vmList.Add(new DMVendorViewModel
                 {
                     Vendor_MasterID = m.Pending_Vendor_MasterID,
                     Vendor_Name = m.Pending_Vendor_Name,
                     Vendor_TIN = m.Pending_Vendor_TIN,
                     Vendor_Address = m.Pending_Vendor_Address,
                     Vendor_Creator_Name = m.CreatorName ?? "N/A",
                     Vendor_Approver_Name = m.ApproverName ?? "",
                     Vendor_Created_Date = m.Vendor_Created_Date,
                     Vendor_Creator_ID = m.Pending_Vendor_Creator_ID,
                     Vendor_Last_Updated = m.Vendor_Last_Updated,
                     Vendor_Status_ID = m.Status_ID,
                     Vendor_Status = m.Status_Name ?? "N/A",
                     Vendor_Tax_Rates = pendingList.Where(x => x.Pending_ID == m.Pending_ID).Select(x => (x.TRRAte * 100) + "% - " + x.TR_WT_Title).ToList(),
                     Vendor_VAT = pendingList.Where(x => x.Pending_ID == m.Pending_ID).Select(x => (x.VATRAte * 100) + "% - " + x.VAT_Name).ToList()
                 })
            );
            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.PF).ToString();
                if (toStr.Length > 0)
                {
                    if (toStr != "" && toStr != "0")
                    {

                        vmList = vmList.AsQueryable().Where("Vendor_" + subStr + ".Contains(@0)", toStr)
                                .Select(e => e).ToList();
                    }
                }
            }

            return vmList;
        }

        public List<DMDeptViewModel> populateDept(DMFiltersViewModel filters)
        {
            IQueryable<DMDeptModel> mList = _context.DMDept.Where(x => x.Dept_isDeleted == false && x.Dept_isActive == true).ToList().AsQueryable();
            var properties = filters.DF.GetType().GetProperties();

            var pendingList = _context.DMDept_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMDeptModel[] {
                    new DMDeptModel
                    {
                        Dept_ID = m.Pending_Dept_ID,
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

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string subStr = propertyName.Substring(propertyName.IndexOf('_') + 1);
                var toStr = property.GetValue(filters.DF).ToString();
                if (toStr.Length > 0)
                {
                    if (toStr != "" && toStr != "0")
                    {
                        if (subStr == "Creator_Name" || subStr == "Approver_Name" || subStr == "Status")
                        { 
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.DF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.DF).ToString())))
                              .Select(x => x.User_ID).ToList(); 
                            //get all status IDs that contains string
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
            var userList = (from a in mList
                            from b in _context.User.Where(x => a.Dept_Creator_ID == x.User_ID).DefaultIfEmpty()
                            from c in _context.User.Where(x => a.Dept_Approver_ID == x.User_ID).DefaultIfEmpty()
                            select new
                            {
                                a.Dept_ID,
                                a.Dept_MasterID,
                                CreatorName = b.User_LName + ", " + b.User_FName,
                                ApproverName = (c == null) ? "" : c.User_LName + ", " + c.User_FName
                            }).ToList();
            var statusList = (from a in mList
                              join c in _context.StatusList on a.Dept_Status_ID equals c.Status_ID
                              select new { a.Dept_ID, a.Dept_MasterID, c.Status_Name }).ToList();

            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            foreach (DMDeptModel m in mList)
            {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_MasterID = m.Dept_MasterID,
                    Dept_Name = m.Dept_Name,
                    Dept_Code = m.Dept_Code,
                    Dept_Budget_Unit = m.Dept_Budget_Unit,
                    Dept_Creator_Name = userList.Where(a => a.Dept_MasterID == m.Dept_MasterID && a.Dept_ID == m.Dept_ID).Select(a => a.CreatorName).FirstOrDefault() ?? "N/A",
                    Dept_Approver_Name = userList.Where(a => a.Dept_MasterID == m.Dept_MasterID && a.Dept_ID == m.Dept_ID).Select(a => a.ApproverName).FirstOrDefault() ?? "",
                    Dept_Created_Date = m.Dept_Created_Date,
                    Dept_Last_Updated = m.Dept_Last_Updated,
                    Dept_Creator_ID = m.Dept_Creator_ID,
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
            var properties = filters.CKF.GetType().GetProperties();

            var pendingList = _context.DMCheck_Pending.ToList();
            foreach (var m in pendingList)
            {
                mList = mList.Concat(new DMCheckModel[] {
                    new DMCheckModel
                    {
                        Check_ID = m.Pending_Check_ID,
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

            var userList = (from a in mList
                            from b in _context.User.Where(x => a.Check_Creator_ID == x.User_ID).DefaultIfEmpty()
                            from c in _context.User.Where(x => a.Check_Approver_ID == x.User_ID).DefaultIfEmpty()
                            select new
                            {
                                a.Check_ID,
                                a.Check_MasterID,
                                CreatorName = b.User_LName + ", " + b.User_FName,
                                ApproverName = (c == null) ? "" : c.User_LName + ", " + c.User_FName
                            }).ToList();
            var statusList = (from a in mList
                              join c in _context.StatusList on a.Check_Status_ID equals c.Status_ID
                              select new { a.Check_ID, a.Check_MasterID, c.Status_Name }).ToList();

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
                    Check_Creator_Name = userList.Where(a => a.Check_ID == m.Check_ID && a.Check_MasterID == m.Check_MasterID).Select(a => a.CreatorName).FirstOrDefault() ?? "N/A",
                    Check_Approver_Name = userList.Where(a => a.Check_ID == m.Check_ID && a.Check_MasterID == m.Check_MasterID).Select(a => a.ApproverName).FirstOrDefault() ?? "",
                    Check_Created_Date = m.Check_Created_Date,
                    Check_Last_Updated = m.Check_Last_Updated,
                    Check_Creator_ID = m.Check_Creator_ID,
                    Check_Status_ID = m.Check_Status_ID,
                    Check_Status = statusList.Where(a => a.Check_ID == m.Check_ID && a.Check_MasterID == m.Check_MasterID).Select(a => a.Status_Name).FirstOrDefault() ?? "N/A"
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMAccountViewModel> populateAccount(DMFiltersViewModel filters)
        {
            var properties = filters.AF.GetType().GetProperties();
            var mList = (from data in (from accs in _context.DMAccount
                                       where accs.Account_isDeleted == false && accs.Account_isActive == true
                                       select new
                                       {
                                           accs.Account_ID,
                                           accs.Account_MasterID,
                                           accs.Account_Name,
                                           accs.Account_Code,
                                           accs.Account_Budget_Code,
                                           accs.Account_No,
                                           accs.Account_Cust,
                                           accs.Account_Div,
                                           accs.Account_Fund,
                                           accs.Account_Created_Date,
                                           accs.Account_Last_Updated,
                                           accs.Account_Creator_ID,
                                           accs.Account_Approver_ID,
                                           accs.Account_Currency_MasterID,
                                           accs.Account_Group_MasterID,
                                           accs.Account_FBT_MasterID,
                                           accs.Account_Status_ID
                                       })
                         join grp in _context.DMAccountGroup
                         on new { masterID = data.Account_Group_MasterID, isActive = true, isDeleted = false }
                         equals new { masterID = grp.AccountGroup_MasterID, isActive = grp.AccountGroup_isActive, isDeleted = grp.AccountGroup_isDeleted }
                         into withgroup
                         from grp in withgroup.DefaultIfEmpty()

                         join fbt in _context.DMFBT
                         on new { masterID = data.Account_FBT_MasterID, isActive = true, isDeleted = false }
                         equals new { masterID = fbt.FBT_MasterID, isActive = fbt.FBT_isActive, isDeleted = fbt.FBT_isDeleted }
                         into withFbt
                         from fbt in withFbt.DefaultIfEmpty()

                         join curr in _context.DMCurrency
                         on new { masterID = data.Account_Currency_MasterID, isActive = true, isDeleted = false }
                         equals new { masterID = curr.Curr_MasterID, isActive = curr.Curr_isActive, isDeleted = curr.Curr_isDeleted }
                         into withCurr
                         from curr in withCurr.DefaultIfEmpty()

                         join user in _context.User
                         on data.Account_Creator_ID
                         equals user.User_ID
                         into c
                         from user in c.DefaultIfEmpty()

                         join apprv in _context.User
                         on data.Account_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()

                         join stat in _context.StatusList
                         on data.Account_Status_ID
                         equals stat.Status_ID
                         into s
                         from stat in s.DefaultIfEmpty()
                         select new
                         {
                             data.Account_MasterID,
                             data.Account_ID,
                             data.Account_Name,
                             data.Account_Code,
                             data.Account_Budget_Code,
                             data.Account_No,
                             data.Account_Cust,
                             data.Account_Div,
                             data.Account_Fund,
                             data.Account_Created_Date,
                             data.Account_Last_Updated,
                             data.Account_Creator_ID,
                             data.Account_Approver_ID,
                             data.Account_Status_ID,
                             CreatorName = user.User_LName + ", " + user.User_FName,
                             ApproverName = data.Account_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             curr.Curr_CCY_ABBR,
                             grp.AccountGroup_Name,
                             FBT = fbt.FBT_Name + " - " + fbt.FBT_Tax_Rate,
                             stat.Status_ID,
                             stat.Status_Name
                         }).ToList();

            var pendingList = (from data in (from accs in _context.DMAccount_Pending
                           select new
                           {
                               accs.Pending_Account_ID,
                               accs.Pending_Account_MasterID,
                               accs.Pending_Account_Name,
                               accs.Pending_Account_Code,
                               accs.Pending_Account_Budget_Code,
                               accs.Pending_Account_No,
                               accs.Pending_Account_Cust,
                               accs.Pending_Account_Div,
                               accs.Pending_Account_Fund,
                               accs.Pending_Account_Filed_Date,
                               accs.Pending_Account_Creator_ID,
                               accs.Pending_Account_Approver_ID,
                               accs.Pending_Account_Currency_MasterID,
                               accs.Pending_Account_Group_MasterID,
                               accs.Pending_Account_FBT_MasterID,
                               accs.Pending_Account_Status_ID
                           })
             join grp in _context.DMAccountGroup
             on new { masterID = data.Pending_Account_Group_MasterID, isActive = true, isDeleted = false }
             equals new { masterID = grp.AccountGroup_MasterID, isActive = grp.AccountGroup_isActive, isDeleted = grp.AccountGroup_isDeleted }
             into withgroup
             from grp in withgroup.DefaultIfEmpty()

             join fbt in _context.DMFBT
             on new { masterID = data.Pending_Account_FBT_MasterID, isActive = true, isDeleted = false }
             equals new { masterID = fbt.FBT_MasterID, isActive = fbt.FBT_isActive, isDeleted = fbt.FBT_isDeleted }
             into withFbt
             from fbt in withFbt.DefaultIfEmpty()

             join curr in _context.DMCurrency
             on new { masterID = data.Pending_Account_Currency_MasterID, isActive = true, isDeleted = false }
             equals new { masterID = curr.Curr_MasterID, isActive = curr.Curr_isActive, isDeleted = curr.Curr_isDeleted }
             into withCurr
             from curr in withCurr.DefaultIfEmpty()

             join user in _context.User
             on data.Pending_Account_Creator_ID
             equals user.User_ID
             into c
             from user in c.DefaultIfEmpty()

             join apprv in _context.User
             on data.Pending_Account_Approver_ID
             equals apprv.User_ID
             into a
             from apprv in a.DefaultIfEmpty()

             join stat in _context.StatusList
             on data.Pending_Account_Status_ID
             equals stat.Status_ID
             into s
             from stat in s.DefaultIfEmpty()
             select new
             {
                 data.Pending_Account_ID,
                 data.Pending_Account_MasterID,
                 data.Pending_Account_Name,
                 data.Pending_Account_Code,
                 data.Pending_Account_Budget_Code,
                 data.Pending_Account_No,
                 data.Pending_Account_Cust,
                 data.Pending_Account_Div,
                 data.Pending_Account_Fund,
                 data.Pending_Account_Filed_Date,
                 data.Pending_Account_Creator_ID,
                 data.Pending_Account_Approver_ID,
                 data.Pending_Account_Status_ID,
                 CreatorName = user.User_LName + ", " + user.User_FName,
                 ApproverName = data.Pending_Account_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                 curr.Curr_CCY_ABBR,
                 grp.AccountGroup_Name,
                 FBT = fbt.FBT_Name + " - " + fbt.FBT_Tax_Rate,
                 stat.Status_ID,
                 stat.Status_Name
             }).ToList();

            //TEMP where clause until FBT is updated
            var defaultFBT = _context.DMFBT.Where(x => x.FBT_MasterID == 1).Select(x => x.FBT_Name).FirstOrDefault();

            List<DMAccountViewModel> vmList = new List<DMAccountViewModel>();
            mList.GroupBy(o => o.Account_ID).Select(o => o.FirstOrDefault()).ToList().ForEach(m =>
                  vmList.Add(new DMAccountViewModel
                  {
                      Account_MasterID = m.Account_MasterID,
                      Account_Name = m.Account_Name,
                      Account_Code = m.Account_Code,
                      Account_Budget_Code = m.Account_Budget_Code,
                      Account_No = m.Account_No,
                      Account_Cust = m.Account_Cust,
                      Account_Div = m.Account_Div,
                      Account_Fund = m.Account_Fund,
                      Account_Group_Name = m.AccountGroup_Name ?? "",
                      Account_FBT_Name = m.FBT ?? defaultFBT,
                      Account_Creator_Name = m.CreatorName ?? "N/A",
                      Account_Approver_Name = m.ApproverName ?? "",
                      Account_Created_Date = m.Account_Created_Date,
                      Account_Last_Updated = m.Account_Last_Updated,
                      Account_Status_ID = m.Account_Status_ID,
                      Account_Creator_ID = m.Account_Creator_ID,
                      Account_Currency_Name = m.Curr_CCY_ABBR ?? "",
                      Account_Status = m.Status_Name ?? "N/A"
                  })
            );

            pendingList.GroupBy(o => o.Pending_Account_ID).Select(o => o.FirstOrDefault()).ToList().ForEach(m =>
                 vmList.Add(new DMAccountViewModel
                 {
                     Account_MasterID = m.Pending_Account_MasterID,
                     Account_Name = m.Pending_Account_Name,
                     Account_Code = m.Pending_Account_Code,
                     Account_Budget_Code = m.Pending_Account_Budget_Code,
                     Account_No = m.Pending_Account_No,
                     Account_Cust = m.Pending_Account_Cust,
                     Account_Div = m.Pending_Account_Div,
                     Account_Fund = m.Pending_Account_Fund,
                     Account_Group_Name = m.AccountGroup_Name ?? "",
                     Account_FBT_Name = m.FBT ?? defaultFBT,
                     Account_Creator_Name = m.CreatorName ?? "N/A",
                     Account_Approver_Name = m.ApproverName ?? "",
                     Account_Created_Date = m.Pending_Account_Filed_Date,
                     Account_Last_Updated = m.Pending_Account_Filed_Date,
                     Account_Status_ID = m.Pending_Account_Status_ID,
                     Account_Creator_ID = m.Pending_Account_Creator_ID,
                     Account_Currency_Name = m.Curr_CCY_ABBR ?? "",
                     Account_Status = m.Status_Name ?? "N/A"
                 })
            );

            var vmList2 = vmList.AsQueryable();
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
                            vmList2 = vmList2.Where("Account_" + subStr + " = @0 AND  Account_isDeleted == @1", property.GetValue(filters.AF), false)
                                     .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Creator_Name")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.AF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            vmList2 = vmList2.Where(x => names.Contains(x.Account_Creator_ID) && x.Account_isDeleted == false)
                                        .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Approver_Name")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.User
                              .Where(x => (x.User_FName.Contains(property.GetValue(filters.AF).ToString())
                              || x.User_LName.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.User_ID).ToList();
                            vmList2 = vmList2.Where(x => names.Contains(x.Account_Approver_ID) && x.Account_isDeleted == false)
                                        .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Status")
                        {
                            //get all status IDs that contains string
                            var status = _context.StatusList
                              .Where(x => (x.Status_Name.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.Status_ID).ToList();
                            vmList2 = vmList2.Where(x => status.Contains(x.Account_Status_ID) && x.Account_isDeleted == false)
                                        .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Group")
                        {
                            //get all account group IDs that contains string
                            var grp = _context.DMAccountGroup
                              .Where(x => (x.AccountGroup_Name.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.AccountGroup_MasterID).ToList();
                            vmList2 = vmList2.Where(x => grp.Contains(x.Account_Group_MasterID) && x.Account_isDeleted == false)
                                        .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "Currency")
                        {
                            //get all currenct IDs that contains string
                            var curr = _context.DMCurrency
                              .Where(x => (x.Curr_Name.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.Curr_MasterID).ToList();
                            vmList2 = vmList2.Where(x => curr.Contains(x.Account_Currency_MasterID) && x.Account_isDeleted == false)
                                        .Select(e => e).AsQueryable();
                        }
                        else if (subStr == "FBT")
                        {
                            //get all userIDs of creator or approver that contains string
                            var names = _context.DMFBT
                              .Where(x => (x.FBT_Name.Contains(property.GetValue(filters.AF).ToString())))
                              .Select(x => x.FBT_MasterID).ToList();
                            vmList2 = vmList2.Where(x => names.Contains(x.Account_FBT_MasterID) && x.Account_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                        }
                        else // IF STRING VALUE
                        {
                            vmList2 = vmList2.Where("Account_" + subStr + ".Contains(@0)", property.GetValue(filters.AF).ToString())
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }

            return vmList2.ToList();
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
                        VAT_ID = m.Pending_VAT_ID,
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
            var userList = (from a in mList
                            from b in _context.User.Where(x => a.VAT_Creator_ID == x.User_ID).DefaultIfEmpty()
                            from c in _context.User.Where(x => a.VAT_Approver_ID == x.User_ID).DefaultIfEmpty()
                            select new
                            {
                                a.VAT_ID,
                                a.VAT_MasterID,
                                CreatorName = b.User_LName + ", " + b.User_FName,
                                ApproverName = (c == null) ? "" : c.User_LName + ", " + c.User_FName
                            }).ToList();
            var statusList = (from a in mList
                              join c in _context.StatusList on a.VAT_Status_ID equals c.Status_ID
                              select new { a.VAT_ID, a.VAT_MasterID, c.Status_Name }).ToList();

            List<DMVATViewModel> vmList = new List<DMVATViewModel>();
            foreach (DMVATModel m in mList)
            {
                var creator = userList.Where(a => a.VAT_ID == m.VAT_ID && a.VAT_MasterID == m.VAT_MasterID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = userList.Where(a => a.VAT_ID == m.VAT_ID && a.VAT_MasterID == m.VAT_MasterID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statusList.Where(a => a.VAT_ID == m.VAT_ID && a.VAT_MasterID == m.VAT_MasterID).Select(a => a.Status_Name).FirstOrDefault();
                DMVATViewModel vm = new DMVATViewModel
                {
                    VAT_MasterID = m.VAT_MasterID,
                    VAT_Name = m.VAT_Name,
                    VAT_Rate = m.VAT_Rate,
                    VAT_Creator_Name = creator ?? "N/A",
                    VAT_Approver_Name = approver ?? "",
                    VAT_Creator_ID = m.VAT_Creator_ID,
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
                        FBT_ID = m.Pending_FBT_ID,
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
            var userList = (from a in mList
                            from b in _context.User.Where(x => a.FBT_Creator_ID == x.User_ID).DefaultIfEmpty()
                            from c in _context.User.Where(x => a.FBT_Approver_ID == x.User_ID).DefaultIfEmpty()
                            select new
                            {
                                a.FBT_ID,
                                a.FBT_MasterID,
                                CreatorName = b.User_LName + ", " + b.User_FName,
                                ApproverName = (c == null) ? "" : c.User_LName + ", " + c.User_FName
                            }).ToList();
            var statusList = (from a in mList
                              join c in _context.StatusList on a.FBT_Status_ID equals c.Status_ID
                              select new { a.FBT_ID, a.FBT_MasterID, c.Status_Name }).ToList();

            List<DMFBTViewModel> vmList = new List<DMFBTViewModel>();
            foreach (DMFBTModel m in mList)
            {
                var creator = userList.Where(a => a.FBT_ID == m.FBT_ID && a.FBT_MasterID == m.FBT_MasterID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = userList.Where(a => a.FBT_ID == m.FBT_ID && a.FBT_MasterID == m.FBT_MasterID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statusList.Where(a => a.FBT_ID == m.FBT_ID && a.FBT_MasterID == m.FBT_MasterID).Select(a => a.Status_Name).FirstOrDefault();
                DMFBTViewModel vm = new DMFBTViewModel
                {
                    FBT_MasterID = m.FBT_MasterID,
                    FBT_Name = m.FBT_Name,
                    FBT_Formula = m.FBT_Formula,
                    FBT_Tax_Rate = m.FBT_Tax_Rate,
                    FBT_Creator_Name = creator ?? "N/A",
                    FBT_Approver_Name = approver ?? "",
                    FBT_Creator_ID = m.FBT_Creator_ID,
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
                        TR_ID = m.Pending_TR_ID,
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
                if (toStr != "")
                {
                    if (toStr != "0")
                    {
                        if ("Tax_Rate" == subStr) // IF INT VAL
                        {
                            float newVal = Convert.ToSingle(property.GetValue(filters.TF));
                            mList = mList.Where("TR_" + subStr + " = @0 AND  TR_isDeleted == @1", newVal / 100, false)
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
            var userList = (from a in mList
                            from b in _context.User.Where(x => a.TR_Creator_ID == x.User_ID).DefaultIfEmpty()
                            from c in _context.User.Where(x => a.TR_Approver_ID == x.User_ID).DefaultIfEmpty()
                            select new
                            {
                                a.TR_ID,
                                a.TR_MasterID,
                                CreatorName = b.User_LName + ", " + b.User_FName,
                                ApproverName = (c == null) ? "" : c.User_LName + ", " + c.User_FName
                            }).ToList();
            var statList = (from a in mList
                              join c in _context.StatusList on a.TR_Status_ID equals c.Status_ID
                              select new { a.TR_ID, a.TR_MasterID, c.Status_Name }).ToList();

            List<DMTRViewModel> vmList = new List<DMTRViewModel>();
            foreach (DMTRModel m in mList)
            {
                var creator = userList.Where(a => a.TR_ID == m.TR_ID && a.TR_MasterID == m.TR_MasterID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = userList.Where(a => a.TR_ID == m.TR_ID && a.TR_MasterID == m.TR_MasterID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statList.Where(a => a.TR_ID == m.TR_ID && a.TR_MasterID == m.TR_MasterID).Select(a => a.Status_Name).FirstOrDefault();
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
                    TR_Creator_ID = m.TR_Creator_ID,
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
                        Curr_ID = m.Pending_Curr_ID,
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
            var userList = (from a in mList
                            from b in _context.User.Where(x => a.Curr_Creator_ID == x.User_ID).DefaultIfEmpty()
                            from c in _context.User.Where(x => a.Curr_Approver_ID == x.User_ID).DefaultIfEmpty()
                            select new
                            {
                                a.Curr_ID,
                                a.Curr_MasterID,
                                CreatorName = b.User_LName + ", " + b.User_FName,
                                ApproverName = (c == null) ? "" : c.User_LName + ", " + c.User_FName
                            }).ToList();
            var statusList = (from a in mList
                              join c in _context.StatusList on a.Curr_Status_ID equals c.Status_ID
                              select new { a.Curr_ID, a.Curr_MasterID, c.Status_Name }).ToList();

            List<DMCurrencyViewModel> vmList = new List<DMCurrencyViewModel>();
            foreach (DMCurrencyModel m in mList)
            {
                var creator = userList.Where(a => a.Curr_ID == m.Curr_ID && a.Curr_MasterID == m.Curr_MasterID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = userList.Where(a => a.Curr_ID == m.Curr_ID && a.Curr_MasterID == m.Curr_MasterID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statusList.Where(a => a.Curr_ID == m.Curr_ID && a.Curr_MasterID == m.Curr_MasterID).Select(a => a.Status_Name).FirstOrDefault();
                DMCurrencyViewModel vm = new DMCurrencyViewModel
                {
                    Curr_MasterID = m.Curr_MasterID,
                    Curr_Name = m.Curr_Name,
                    Curr_CCY_ABBR = m.Curr_CCY_ABBR,
                    Curr_Creator_Name = creator ?? "N/A",
                    Curr_Approver_Name = approver ?? "",
                    Curr_Creator_ID = m.Curr_Creator_ID,
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
            var mList = (from e in (from c in _context.DMEmp
                                    join user in _context.User
                                    on c.Emp_Creator_ID equals user.User_ID
                                    join stat in _context.StatusList
                                    on c.Emp_Status_ID equals stat.Status_ID
                                    where c.Emp_isDeleted == false && c.Emp_isActive == true && c.Emp_Type == "Regular"
                                    select new
                                    {
                                        c.Emp_ID,
                                        c.Emp_MasterID,
                                        c.Emp_Name,
                                        c.Emp_Acc_No,
                                        c.Emp_Type,
                                        c.Emp_Category_ID,
                                        c.Emp_FBT_MasterID,
                                        c.Emp_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.Emp_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.Emp_Created_Date,
                                        c.Emp_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.Emp_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         join f in _context.DMFBT
                         on e.Emp_FBT_MasterID
                         equals f.FBT_MasterID
                         into b
                         from f in b.DefaultIfEmpty()
                         select new
                         {
                             e.Emp_ID,
                             e.Emp_MasterID,
                             e.Emp_Name,
                             e.Emp_Acc_No,
                             e.Emp_Type,
                             e.Emp_Category_ID,
                             e.Emp_FBT_MasterID,
                             e.Emp_Creator_ID,
                             e.CreatorName,
                             e.Emp_Approver_ID,
                             ApproverName = e.Emp_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             CategoryName = "",
                             FBTName = e.Emp_FBT_MasterID > 0 ? f.FBT_Name + " - " + (f.FBT_Tax_Rate * 100) + "%" : "",
                             e.Emp_Created_Date,
                             e.Emp_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();
            var pendingList = (from e in (from c in _context.DMEmp_Pending
                                          join user in _context.User
                                          on c.Pending_Emp_Creator_ID equals user.User_ID
                                          join stat in _context.StatusList
                                          on c.Pending_Emp_Status_ID equals stat.Status_ID
                                          where c.Pending_Emp_isDeleted == false && c.Pending_Emp_isActive == true
                                          && c.Pending_Emp_Type == "Regular"
                                          select new
                                          {
                                              c.Pending_Emp_ID,
                                              c.Pending_Emp_MasterID,
                                              c.Pending_Emp_Name,
                                              c.Pending_Emp_Acc_No,
                                              c.Pending_Emp_Type,
                                              c.Pending_Emp_Category_ID,
                                              c.Pending_Emp_FBT_MasterID,
                                              c.Pending_Emp_Creator_ID,
                                              CreatorName = user.User_LName + ", " + user.User_FName,
                                              c.Pending_Emp_Approver_ID,
                                              stat.Status_ID,
                                              stat.Status_Name,
                                              c.Pending_Emp_Filed_Date
                                          })
                               join apprv in _context.User
                               on e.Pending_Emp_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               join f in _context.DMFBT
                               on e.Pending_Emp_FBT_MasterID
                               equals f.FBT_MasterID
                               into b
                               from f in b.DefaultIfEmpty()
                               select new
                               {
                                   e.Pending_Emp_ID,
                                   e.Pending_Emp_MasterID,
                                   e.Pending_Emp_Name,
                                   e.Pending_Emp_Acc_No,
                                   e.Pending_Emp_Type,
                                   e.Pending_Emp_Category_ID,
                                   e.Pending_Emp_FBT_MasterID,
                                   e.Pending_Emp_Creator_ID,
                                   e.CreatorName,
                                   e.Pending_Emp_Approver_ID,
                                   ApproverName = e.Pending_Emp_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   CategoryName = "",
                                   FBTName = e.Pending_Emp_FBT_MasterID > 0 ? f.FBT_Name + " - " + (f.FBT_Tax_Rate * 100) + "%" : "",
                                   e.Pending_Emp_Filed_Date,
                                   e.Status_ID,
                                   e.Status_Name
                               }).ToList();
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            foreach (var m in mList)
            {
                var cat = m.Emp_Category_ID == GlobalSystemValues.EMPCAT_LOCAL ? "LOCAL" : m.Emp_Category_ID == GlobalSystemValues.EMPCAT_EXPAT ? "EXPAT" : "N/A";

                DMEmpViewModel vm = new DMEmpViewModel
                {
                    Emp_MasterID = m.Emp_MasterID,
                    Emp_FBT_MasterID = m.Emp_MasterID,
                    Emp_Category_ID = m.Emp_Category_ID,
                    Emp_FBT_Name = m.FBTName,
                    Emp_Category_Name = cat,
                    Emp_Name = m.Emp_Name,
                    Emp_Acc_No = m.Emp_Acc_No,
                    Emp_Creator_ID = m.Emp_Creator_ID,
                    Emp_Creator_Name = m.CreatorName ?? "N/A",
                    Emp_Approver_Name = m.ApproverName ?? "",
                    Emp_Created_Date = m.Emp_Created_Date,
                    Emp_Last_Updated = m.Emp_Last_Updated,
                    Emp_Status_ID = m.Status_ID,
                    Emp_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                var cat = m.Pending_Emp_Category_ID == GlobalSystemValues.EMPCAT_LOCAL ? "LOCAL" : m.Pending_Emp_Category_ID == GlobalSystemValues.EMPCAT_EXPAT ? "EXPAT" : "N/A";

                DMEmpViewModel vm = new DMEmpViewModel
                {
                    Emp_MasterID = m.Pending_Emp_MasterID,
                    Emp_FBT_MasterID = m.Pending_Emp_MasterID,
                    Emp_Category_ID = m.Pending_Emp_Category_ID,
                    Emp_FBT_Name = m.FBTName,
                    Emp_Category_Name = cat,
                    Emp_Name = m.Pending_Emp_Name,
                    Emp_Acc_No = m.Pending_Emp_Acc_No,
                    Emp_Creator_ID = m.Pending_Emp_Creator_ID,
                    Emp_Creator_Name = m.CreatorName ?? "N/A",
                    Emp_Approver_Name = m.ApproverName ?? "",
                    Emp_Created_Date = m.Pending_Emp_Filed_Date,
                    Emp_Last_Updated = m.Pending_Emp_Filed_Date,
                    Emp_Status_ID = m.Status_ID,
                    Emp_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            var properties = filters.EMF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_")+1);
                var toStr = property.GetValue(filters.EMF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    mList = mList.AsQueryable().Where("Emp_" + subStr + ".Contains(@0)", toStr)
                            .Select(e => e).ToList();
                }
            }
            return vmList;
        }

        public List<DMEmpViewModel> populateTempEmp(DMFiltersViewModel filters)
        {
            var mList = (from e in (from c in _context.DMEmp
                                    join user in _context.User
                                    on c.Emp_Creator_ID equals user.User_ID
                                    join stat in _context.StatusList
                                    on c.Emp_Status_ID equals stat.Status_ID
                                    where c.Emp_isDeleted == false && c.Emp_isActive == true && c.Emp_Type == "Temporary"
                                    select new
                                    {
                                        c.Emp_ID,
                                        c.Emp_MasterID,
                                        c.Emp_Name,
                                        c.Emp_Acc_No,
                                        c.Emp_Type,
                                        c.Emp_Category_ID,
                                        c.Emp_FBT_MasterID,
                                        c.Emp_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.Emp_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.Emp_Created_Date,
                                        c.Emp_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.Emp_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         join f in _context.DMFBT
                         on e.Emp_FBT_MasterID
                         equals f.FBT_MasterID
                         into b
                         from f in b.DefaultIfEmpty()
                         select new
                         {
                             e.Emp_ID,
                             e.Emp_MasterID,
                             e.Emp_Name,
                             e.Emp_Acc_No,
                             e.Emp_Type,
                             e.Emp_Category_ID,
                             e.Emp_FBT_MasterID,
                             e.Emp_Creator_ID,
                             e.CreatorName,
                             e.Emp_Approver_ID,
                             ApproverName = e.Emp_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             CategoryName = "",
                             FBTName = e.Emp_FBT_MasterID > 0 ? f.FBT_Name + " - " + (f.FBT_Tax_Rate * 100) + "%" : "",
                             e.Emp_Created_Date,
                             e.Emp_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();
            var pendingList = (from e in (from c in _context.DMEmp_Pending
                                          join user in _context.User
                                          on c.Pending_Emp_Creator_ID equals user.User_ID
                                          join stat in _context.StatusList
                                          on c.Pending_Emp_Status_ID equals stat.Status_ID
                                          where c.Pending_Emp_isDeleted == false && c.Pending_Emp_isActive == true && c.Pending_Emp_Type == "Temporary"
                                          select new
                                          {
                                              c.Pending_Emp_ID,
                                              c.Pending_Emp_MasterID,
                                              c.Pending_Emp_Name,
                                              c.Pending_Emp_Acc_No,
                                              c.Pending_Emp_Type,
                                              c.Pending_Emp_Category_ID,
                                              c.Pending_Emp_FBT_MasterID,
                                              c.Pending_Emp_Creator_ID,
                                              CreatorName = user.User_LName + ", " + user.User_FName,
                                              c.Pending_Emp_Approver_ID,
                                              stat.Status_ID,
                                              stat.Status_Name,
                                              c.Pending_Emp_Filed_Date
                                          })
                               join apprv in _context.User
                               on e.Pending_Emp_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               join f in _context.DMFBT
                               on e.Pending_Emp_FBT_MasterID
                               equals f.FBT_MasterID
                               into b
                               from f in b.DefaultIfEmpty()
                               select new
                               {
                                   e.Pending_Emp_ID,
                                   e.Pending_Emp_MasterID,
                                   e.Pending_Emp_Name,
                                   e.Pending_Emp_Acc_No,
                                   e.Pending_Emp_Type,
                                   e.Pending_Emp_Category_ID,
                                   e.Pending_Emp_FBT_MasterID,
                                   e.Pending_Emp_Creator_ID,
                                   e.CreatorName,
                                   e.Pending_Emp_Approver_ID,
                                   ApproverName = e.Pending_Emp_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   CategoryName = "",
                                   FBTName = e.Pending_Emp_FBT_MasterID > 0 ? f.FBT_Name + " - " + (f.FBT_Tax_Rate * 100) + "%" : "",
                                   e.Pending_Emp_Filed_Date,
                                   e.Status_ID,
                                   e.Status_Name
                               }).ToList();
            List<DMEmpViewModel> vmList = new List<DMEmpViewModel>();
            foreach (var m in mList)
            {
                var cat = m.Emp_Category_ID == GlobalSystemValues.EMPCAT_LOCAL ? "LOCAL" : m.Emp_Category_ID == GlobalSystemValues.EMPCAT_EXPAT ? "EXPAT" : "N/A";

                DMEmpViewModel vm = new DMEmpViewModel
                {
                    Emp_MasterID = m.Emp_MasterID,
                    Emp_FBT_MasterID = m.Emp_MasterID,
                    Emp_Category_ID = m.Emp_Category_ID,
                    Emp_FBT_Name = m.FBTName,
                    Emp_Category_Name = cat,
                    Emp_Name = m.Emp_Name,
                    Emp_Acc_No = m.Emp_Acc_No,
                    Emp_Creator_ID = m.Emp_Creator_ID,
                    Emp_Creator_Name = m.CreatorName ?? "N/A",
                    Emp_Approver_Name = m.ApproverName ?? "",
                    Emp_Created_Date = m.Emp_Created_Date,
                    Emp_Last_Updated = m.Emp_Last_Updated,
                    Emp_Status_ID = m.Status_ID,
                    Emp_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                var cat = m.Pending_Emp_Category_ID == GlobalSystemValues.EMPCAT_LOCAL ? "LOCAL" : m.Pending_Emp_Category_ID == GlobalSystemValues.EMPCAT_EXPAT ? "EXPAT" : "N/A";

                DMEmpViewModel vm = new DMEmpViewModel
                {
                    Emp_MasterID = m.Pending_Emp_MasterID,
                    Emp_FBT_MasterID = m.Pending_Emp_MasterID,
                    Emp_Category_ID = m.Pending_Emp_Category_ID,
                    Emp_FBT_Name = m.FBTName,
                    Emp_Category_Name = cat,
                    Emp_Name = m.Pending_Emp_Name,
                    Emp_Acc_No = m.Pending_Emp_Acc_No,
                    Emp_Creator_ID = m.Pending_Emp_Creator_ID,
                    Emp_Creator_Name = m.CreatorName ?? "N/A",
                    Emp_Approver_Name = m.ApproverName ?? "",
                    Emp_Created_Date = m.Pending_Emp_Filed_Date,
                    Emp_Last_Updated = m.Pending_Emp_Filed_Date,
                    Emp_Status_ID = m.Status_ID,
                    Emp_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            var properties = filters.EMF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.EMF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    mList = mList.AsQueryable().Where("Emp_" + subStr + ".Contains(@0)", toStr)
                            .Select(e => e).ToList();
                }
            }
            return vmList;
        }

        public List<DMCustViewModel> populateCust(DMFiltersViewModel filters)
        {
            var mList = (from rate in (from c in _context.DMCust
                                       join user in _context.User
                                       on c.Cust_Creator_ID equals user.User_ID
                                       join stat in _context.StatusList
                                       on c.Cust_Status_ID equals stat.Status_ID
                                       where c.Cust_isDeleted == false && c.Cust_isActive == true
                                       select new
                                       {
                                           c.Cust_ID,
                                           c.Cust_MasterID,
                                           c.Cust_Name,
                                           c.Cust_Abbr,
                                           c.Cust_No,
                                           c.Cust_Creator_ID,
                                           CreatorName = user.User_LName + ", " + user.User_FName,
                                           c.Cust_Approver_ID,
                                           stat.Status_ID,
                                           stat.Status_Name,
                                           c.Cust_Created_Date,
                                           c.Cust_Last_Updated
                                       })
                         join apprv in _context.User
                         on rate.Cust_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         select new
                         {
                             rate.Cust_ID,
                             rate.Cust_MasterID,
                             rate.Cust_Name,
                             rate.Cust_Abbr,
                             rate.Cust_No,
                             rate.Cust_Creator_ID,
                             rate.CreatorName,
                             ApproverID = rate.Cust_Approver_ID,
                             ApproverName = rate.Cust_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             rate.Cust_Created_Date,
                             rate.Cust_Last_Updated,
                             rate.Status_ID,
                             rate.Status_Name
                         }).ToList();
            var pendingList = (from rate in (from c in _context.DMCust_Pending
                                             join user in _context.User
                                             on c.Pending_Cust_Creator_ID equals user.User_ID
                                             join stat in _context.StatusList
                                             on c.Pending_Cust_Status_ID equals stat.Status_ID
                                             where c.Pending_Cust_isDeleted == false && c.Pending_Cust_isActive == true
                                             select new
                                             {
                                                 c.Pending_Cust_ID,
                                                 c.Pending_Cust_MasterID,
                                                 c.Pending_Cust_Name,
                                                 c.Pending_Cust_Abbr,
                                                 c.Pending_Cust_No,
                                                 c.Pending_Cust_Creator_ID,
                                                 CreatorName = user.User_LName + ", " + user.User_FName,
                                                 c.Pending_Cust_Approver_ID,
                                                 stat.Status_ID,
                                                 stat.Status_Name,
                                                 c.Pending_Cust_Filed_Date
                                             })
                               join apprv in _context.User
                               on rate.Pending_Cust_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               select new
                               {
                                   rate.Pending_Cust_ID,
                                   rate.Pending_Cust_MasterID,
                                   rate.Pending_Cust_Name,
                                   rate.Pending_Cust_Abbr,
                                   rate.Pending_Cust_No,
                                   rate.Pending_Cust_Creator_ID,
                                   rate.CreatorName,
                                   ApproverID = rate.Pending_Cust_Approver_ID,
                                   ApproverName = rate.Pending_Cust_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   rate.Pending_Cust_Filed_Date,
                                   rate.Status_ID,
                                   rate.Status_Name
                               }).ToList();
            List<DMCustViewModel> vmList = new List<DMCustViewModel>();
            foreach (var m in mList)
            {
                DMCustViewModel vm = new DMCustViewModel
                {
                    Cust_MasterID = m.Cust_MasterID,
                    Cust_Name = m.Cust_Name,
                    Cust_Abbr = m.Cust_Abbr,
                    Cust_No = m.Cust_No,
                    Cust_Creator_ID = m.Cust_Creator_ID,
                    Cust_Creator_Name = m.CreatorName ?? "N/A",
                    Cust_Approver_Name = m.ApproverName ?? "",
                    Cust_Created_Date = m.Cust_Created_Date,
                    Cust_Last_Updated = m.Cust_Last_Updated,
                    Cust_Status_ID = m.Status_ID,
                    Cust_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                DMCustViewModel vm = new DMCustViewModel
                {
                    Cust_MasterID = m.Pending_Cust_MasterID,
                    Cust_Name = m.Pending_Cust_Name,
                    Cust_Abbr = m.Pending_Cust_Abbr,
                    Cust_No = m.Pending_Cust_No,
                    Cust_Creator_ID = m.Pending_Cust_Creator_ID,
                    Cust_Creator_Name = m.CreatorName ?? "N/A",
                    Cust_Approver_Name = m.ApproverName ?? "",
                    Cust_Created_Date = m.Pending_Cust_Filed_Date,
                    Cust_Last_Updated = m.Pending_Cust_Filed_Date,
                    Cust_Status_ID = m.Status_ID,
                    Cust_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            var properties = filters.CUF.GetType().GetProperties();
            IQueryable<DMCustViewModel> vmList2 = vmList.AsQueryable();
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
                                vmList2 = vmList2.Where(x => names.Contains(x.Cust_Approver_ID) && x.Cust_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Creator_Name")
                            {
                                vmList2 = vmList2.Where(x => names.Contains(x.Cust_Creator_ID) && x.Cust_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                            else if (subStr == "Status")
                            {
                                vmList2 = vmList2.Where(x => status.Contains(x.Cust_Status_ID) && x.Cust_isDeleted == false)
                                         .Select(e => e).AsQueryable();
                            }
                        }
                        else // IF STRING VALUE
                        {
                            vmList2 = vmList2.Where("Cust_" + subStr + ".Contains(@0)", toStr)
                                    .Select(e => e).AsQueryable();
                        }
                    }
                }
            }
            return vmList2.ToList();
        }

        public List<DMBCSViewModel> populateBCS(DMFiltersViewModel filters)
        {

            var mList = (from e in (from c in _context.DMBCS
                                    join user in _context.User
                                    on c.BCS_Creator_ID equals user.User_ID
                                    join emp in _context.User
                                    on c.BCS_User_ID equals emp.User_ID
                                    join stat in _context.StatusList
                                    on c.BCS_Status_ID equals stat.Status_ID
                                    where c.BCS_isDeleted == false && c.BCS_isActive == true
                                    select new
                                    {
                                        c.BCS_ID,
                                        c.BCS_MasterID,
                                        c.BCS_User_ID,
                                        EmpName = emp.User_LName + ", " + emp.User_FName,
                                        c.BCS_TIN,
                                        c.BCS_Position,
                                        c.BCS_Signatures,
                                        c.BCS_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.BCS_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.BCS_Created_Date,
                                        c.BCS_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.BCS_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         select new
                         {
                             e.BCS_ID,
                             e.BCS_MasterID,
                             e.BCS_User_ID,
                             e.EmpName,
                             e.BCS_TIN,
                             e.BCS_Position,
                             e.BCS_Signatures,
                             e.BCS_Creator_ID,
                             e.CreatorName,
                             e.BCS_Approver_ID,
                             ApproverName = e.BCS_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             e.BCS_Created_Date,
                             e.BCS_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();
            var pendingList = (from e in (from c in _context.DMBCS_Pending
                                          join user in _context.User
                                          on c.Pending_BCS_Creator_ID equals user.User_ID
                                          join emp in _context.User
                                          on c.Pending_BCS_User_ID equals emp.User_ID
                                          join stat in _context.StatusList
                                          on c.Pending_BCS_Status_ID equals stat.Status_ID
                                          select new
                                          {
                                              c.Pending_BCS_ID,
                                              c.Pending_BCS_MasterID,
                                              c.Pending_BCS_User_ID,
                                              EmpName = emp.User_LName + ", " + emp.User_FName,
                                              c.Pending_BCS_TIN,
                                              c.Pending_BCS_Position,
                                              c.Pending_BCS_Signatures,
                                              c.Pending_BCS_Creator_ID,
                                              CreatorName = user.User_LName + ", " + user.User_FName,
                                              c.Pending_BCS_Approver_ID,
                                              stat.Status_ID,
                                              stat.Status_Name,
                                              c.Pending_BCS_Filed_Date
                                          })
                               join apprv in _context.User
                               on e.Pending_BCS_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               select new
                               {
                                   e.Pending_BCS_ID,
                                   e.Pending_BCS_MasterID,
                                   e.Pending_BCS_User_ID,
                                   e.EmpName,
                                   e.Pending_BCS_TIN,
                                   e.Pending_BCS_Position,
                                   e.Pending_BCS_Signatures,
                                   e.Pending_BCS_Creator_ID,
                                   e.CreatorName,
                                   e.Pending_BCS_Approver_ID,
                                   ApproverName = e.Pending_BCS_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   e.Pending_BCS_Filed_Date,
                                   e.Status_ID,
                                   e.Status_Name
                               }).ToList();
            List<DMBCSViewModel> vmList = new List<DMBCSViewModel>();
            foreach (var m in mList)
            {
                DMBCSViewModel vm = new DMBCSViewModel
                {
                    BCS_ID = m.BCS_ID,
                    BCS_User_ID = m.BCS_User_ID,
                    BCS_MasterID = m.BCS_MasterID,
                    BCS_Name = m.EmpName,
                    BCS_TIN = m.BCS_TIN,
                    BCS_Position = m.BCS_Position,
                    BCS_Signatures = m.BCS_Signatures,
                    BCS_Creator_ID = m.BCS_Creator_ID,
                    BCS_Creator_Name = m.CreatorName ?? "N/A",
                    BCS_Approver_ID = m.BCS_Approver_ID,
                    BCS_Approver_Name = m.ApproverName ?? "",
                    BCS_Created_Date = m.BCS_Created_Date,
                    BCS_Last_Updated = m.BCS_Last_Updated,
                    BCS_Status_ID = m.Status_ID,
                    BCS_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                DMBCSViewModel vm = new DMBCSViewModel
                {
                    BCS_ID = m.Pending_BCS_ID,
                    BCS_User_ID = m.Pending_BCS_User_ID,
                    BCS_MasterID = m.Pending_BCS_MasterID,
                    BCS_Name = m.EmpName,
                    BCS_TIN = m.Pending_BCS_TIN,
                    BCS_Position = m.Pending_BCS_Position,
                    BCS_Signatures = m.Pending_BCS_Signatures,
                    BCS_Creator_ID = m.Pending_BCS_Creator_ID,
                    BCS_Creator_Name = m.CreatorName ?? "N/A",
                    BCS_Approver_ID = m.Pending_BCS_Approver_ID,
                    BCS_Approver_Name = m.ApproverName ?? "",
                    BCS_Created_Date = m.Pending_BCS_Filed_Date,
                    BCS_Last_Updated = m.Pending_BCS_Filed_Date,
                    BCS_Status_ID = m.Status_ID,
                    BCS_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            var properties = filters.BF.GetType().GetProperties();

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.BF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    mList = mList.AsQueryable().Where("BCS_" + subStr + ".Contains(@0)", toStr)
                            .Select(e => e).ToList();
                }
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
                        AccountGroup_ID = m.Pending_AccountGroup_ID,
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
            var userList = (from a in mList
                            from b in _context.User.Where(x => a.AccountGroup_Creator_ID == x.User_ID).DefaultIfEmpty()
                            from c in _context.User.Where(x => a.AccountGroup_Approver_ID == x.User_ID).DefaultIfEmpty()
                            select new
                            {
                                a.AccountGroup_ID,
                                a.AccountGroup_MasterID,
                                CreatorName = b.User_LName + ", " + b.User_FName,
                                ApproverName = (c == null) ? "" : c.User_LName + ", " + c.User_FName
                            }).ToList();
            var statusList = (from a in mList
                              join c in _context.StatusList on a.AccountGroup_Status_ID equals c.Status_ID
                              select new { a.AccountGroup_ID, a.AccountGroup_MasterID, c.Status_Name }).ToList();
            //TEMP where clause until FBT is updated
            var defaultFBT = _context.DMFBT.Where(x => x.FBT_MasterID == 1).Select(x => x.FBT_Name).FirstOrDefault();

            List<DMAccountGroupViewModel> vmList = new List<DMAccountGroupViewModel>();
            foreach (DMAccountGroupModel m in mList)
            {
                var creator = userList.Where(a => a.AccountGroup_ID == m.AccountGroup_ID && a.AccountGroup_MasterID == m.AccountGroup_MasterID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = userList.Where(a => a.AccountGroup_ID == m.AccountGroup_ID && a.AccountGroup_MasterID == m.AccountGroup_MasterID).Select(a => a.ApproverName).FirstOrDefault();
                var stat = statusList.Where(a => a.AccountGroup_ID == m.AccountGroup_ID && a.AccountGroup_MasterID == m.AccountGroup_MasterID).Select(a => a.Status_Name).FirstOrDefault();
                DMAccountGroupViewModel vm = new DMAccountGroupViewModel
                {
                    AccountGroup_MasterID = m.AccountGroup_MasterID,
                    AccountGroup_Name = m.AccountGroup_Name,
                    AccountGroup_Code = m.AccountGroup_Code,
                    AccountGroup_Creator_Name = creator ?? "N/A",
                    AccountGroup_Approver_Name = approver ?? "",
                    AccountGroup_Creator_ID = m.AccountGroup_Creator_ID,
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