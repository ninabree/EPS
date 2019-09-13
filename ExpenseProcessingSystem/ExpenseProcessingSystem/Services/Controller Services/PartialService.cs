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
        //get xml currency details
        public List<CONSTANT_CCY_VALS> getXMLCurrency()
        {
            List<CONSTANT_CCY_VALS> valList = new List<CONSTANT_CCY_VALS>();
            InterEntityValues interEntityValues = new InterEntityValues();
            List<string> consCurrMasterId = new List<string> {
                InterEntityValues.Currency_US,
                InterEntityValues.Currency_Yen,
                InterEntityValues.Currency_PHP
            };
            foreach (string mId in consCurrMasterId)
            {
                var acc = _context.DMCurrency.Where(x => (x.Curr_MasterID == int.Parse(mId))
                                                    && x.Curr_isActive == true && x.Curr_isDeleted == false).FirstOrDefault();
                CONSTANT_CCY_VALS vals = new CONSTANT_CCY_VALS
                {
                    currID = acc.Curr_ID,
                    currMasterID = acc.Curr_MasterID,
                    currABBR = acc.Curr_CCY_ABBR
                };
                valList.Add(vals);
            }
            return valList;
        }
        //get specific xml currency details
        public CONSTANT_CCY_VALS getXMLCurrency(string type)
        {
            InterEntityValues interEntityValues = new InterEntityValues();
            var mId = "";
            if (type == "USD")
            {
                mId = InterEntityValues.Currency_US;
            }
            else if (type == "YEN")
            {
                mId = InterEntityValues.Currency_Yen;
            }
            else if (type == "PHP")
            {
                mId = InterEntityValues.Currency_PHP;
            }
            var acc = _context.DMCurrency.Where(x => (x.Curr_MasterID == int.Parse(mId))
                                                && x.Curr_isActive == true && x.Curr_isDeleted == false).FirstOrDefault();
            return new CONSTANT_CCY_VALS
            {
                currID = acc.Curr_ID,
                currMasterID = acc.Curr_MasterID,
                currABBR = acc.Curr_CCY_ABBR
            };
        }
        //Get lastest currency by its currency master ID.
        public DMCurrencyModel getCurrencyByMasterID(int masterID)
        {
            return _context.DMCurrency.Where(x => x.Curr_MasterID == masterID && x.Curr_isActive == true
                    && x.Curr_isDeleted == false).FirstOrDefault();
        }
        public List<CONSTANT_NC_VALS> getNCAccsForFilter()
        {
            var acc_pettyCash = getNCAccs("/NONCASHACCOUNTS/PCR/ACCOUNT[@class='entry1' and @id='ACCOUNT1']");
            var acc_computerSus = getNCAccs("/NONCASHACCOUNTS/PCR/ACCOUNT[@class='entry1' and @id='ACCOUNT2']");
            var acc_computerSusUSD = getNCAccs("/NONCASHACCOUNTS/RETURNOFJSPAYROLL/ACCOUNT[@class='entry1' and @id='ACCOUNT1']");
            var acc_interRF_RET = getNCAccs("/NONCASHACCOUNTS/RETURNOFJSPAYROLL/ACCOUNT[@class='entry1' and @id='ACCOUNT2']");
            var acc_interRF_JS = getNCAccs("/NONCASHACCOUNTS/JSPAYROLL/ACCOUNT[@class='entry1' and @id='ACCOUNT2']");
            var acc_interFR_JS = getNCAccs("/NONCASHACCOUNTS/JSPAYROLL/ACCOUNT[@class='entry4' and @id='ACCOUNT2']");
            var acc_ewtTax = getNCAccs("/NONCASHACCOUNTS/PSSC/ACCOUNT[@class='entry1' and @id='ACCOUNT2']");
            //Populate Constant NC Accounts
            List<CONSTANT_NC_VALS> valList = new List<CONSTANT_NC_VALS>
            {
                new CONSTANT_NC_VALS()
                {
                    accID = acc_computerSus.FirstOrDefault().accID,
                    accNo = acc_computerSus.FirstOrDefault().accNo+"",
                    accName = "CS"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_computerSusUSD.FirstOrDefault().accID,
                    accNo = acc_computerSusUSD.FirstOrDefault().accNo+"",
                    accName = "CSU"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_interRF_RET.FirstOrDefault().accID,
                    accNo = acc_interRF_RET.FirstOrDefault().accNo+"",
                    accName = "IERF_RET"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_interRF_JS.FirstOrDefault().accID,
                    accNo = acc_interRF_JS.FirstOrDefault().accNo+"",
                    accName = "IERF_JS"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_interFR_JS.FirstOrDefault().accID,
                    accNo = acc_interFR_JS.FirstOrDefault().accNo+"",
                    accName = "IEFR_JS"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_pettyCash.FirstOrDefault().accID,
                    accNo = acc_pettyCash.FirstOrDefault().accNo+"",
                    accName = "PC"
                },
                new CONSTANT_NC_VALS()
                {
                    accID = acc_ewtTax.FirstOrDefault().accID,
                    accNo = acc_ewtTax.FirstOrDefault().accNo+"",
                    accName = "ET"
                }
            };
            return valList;
        }
        //----------------------------------- [[ Populate Non Cash ]] -------------------------------------////
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
                    if (dtl.d.ExpNC_Category_ID == GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES)
                    {
                        var copyAcc = ncDtlAccs[0];
                        var emptyAccs = 4 - ncDtlAccs.Count;
                        int z = 0;
                        while (z < emptyAccs)
                        {
                            ncDtlAccs.Add(new ExpenseEntryNCDtlAccViewModel
                            {
                                ExpNCDtlAcc_Acc_ID = 0,
                                ExpNCDtlAcc_Acc_Name = "",
                                ExpNCDtlAcc_Curr_ID = copyAcc.ExpNCDtlAcc_Curr_ID,
                                ExpNCDtlAcc_Curr_Name = copyAcc.ExpNCDtlAcc_Curr_Name,
                                ExpNCDtlAcc_Inter_Rate = 0,
                                ExpNCDtlAcc_Amount = 0,
                                ExpNCDtlAcc_Type_ID = GlobalSystemValues.NC_CREDIT
                            });
                            z++;
                        }
                    }
                    entryNCDtl = new ExpenseEntryNCDtlViewModel()
                    {
                        ExpNCDtl_ID = ncDtl.g.ExpNCDtl_ID,
                        ExpNCDtl_Remarks_Desc = ncDtl.g.ExpNCDtl_Remarks_Desc,
                        ExpNCDtl_Remarks_Period = ncDtl.g.ExpNCDtl_Remarks_Period,
                        ExpNCDtl_TR_ID = ncDtl.g.ExpNCDtl_TR_ID,
                        ExpNCDtl_TR_Title = ncDtl.g.ExpNCDtl_TR_ID > 0 ? _context.DMTR.FirstOrDefault(x => x.TR_ID == ncDtl.g.ExpNCDtl_TR_ID).TR_WT_Title : "",
                        ExpNCDtl_Vendor_ID = ncDtl.g.ExpNCDtl_Vendor_ID,
                        ExpNCDtl_Vendor_Name = ncDtl.g.ExpNCDtl_Vendor_ID > 0 ? _context.DMVendor.FirstOrDefault(x => x.Vendor_ID == ncDtl.g.ExpNCDtl_Vendor_ID).Vendor_Name : "",
                        ExpNCDtl_TaxBasedAmt = ncDtl.g.ExpNCDtl_TaxBasedAmt,
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
                    NC_CS_Period = dtl.d.ExpNC_CS_Period,
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
                approver_id = EntryDetails.e.Expense_Approver,
                verifier_1_id = EntryDetails.e.Expense_Verifier_1,
                verifier_2_id = EntryDetails.e.Expense_Verifier_2,
                maker = EntryDetails.e.Expense_Creator_ID,
                lastUpdatedDate = EntryDetails.e.Expense_Last_Updated,
                EntryNC = ncDtlVM
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
            selList.Add(new SelectListItem()
            {
                Text = "--- Accounts --",
                Value = "0"
            });
            _context.DMAccount.Where(x => x.Account_isDeleted == false && x.Account_isActive == true).OrderByDescending(x=> x.Account_No.Contains("H90")).ThenBy(x=> x.Account_No).ToList().ForEach(x => {
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
            venList.Add(new SelectListItem()
            {
                Text = "--- Select Vendor Applicable --",
                Value = "0"
            });
            _context.DMVendor.Where(x => x.Vendor_isDeleted == false && x.Vendor_isActive == true).ToList().ForEach(x => {
                venList.Add(new SelectListItem() { Text = x.Vendor_Name, Value = x.Vendor_ID + "" });
            });
            return venList;
        }

        public List<SelectListItem> getTaxRateSelectList()
        {
            List<SelectListItem> trList = new List<SelectListItem>();
            trList.Add(new SelectListItem()
            {
                Text = "--- Select Tax Rate Applicable --",
                Value = "0"
            });
            _context.DMTR.Where(x => x.TR_isDeleted == false && x.TR_isActive == true).ToList().ForEach(x => {
                trList.Add(new SelectListItem() { Text = x.TR_WT_Title+" - " + x.TR_Tax_Rate, Value = x.TR_ID + "" });
            });
            return trList;
        }
        //----------------------------------- [[ Populate DM ]] -------------------------------------//
        public List<DMVendorViewModel> populateVendor(DMFiltersViewModel filters)
        {
            var properties = filters.PF.GetType().GetProperties();
            var mList = (from rate in (from ven in (from ven in _context.DMVendor
                                                    where ven.Vendor_isDeleted == false && ven.Vendor_isActive == true
                                                    select new
                                                    {
                                                        ven.Vendor_ID,
                                                        ven.Vendor_MasterID,
                                                        ven.Vendor_TIN,
                                                        ven.Vendor_Address,
                                                        ven.Vendor_Name,
                                                        ven.Vendor_Creator_ID,
                                                        ven.Vendor_Approver_ID,
                                                        ven.Vendor_Status_ID,
                                                        ven.Vendor_Created_Date,
                                                        ven.Vendor_Last_Updated
                                                    })
                                       join trvat in _context.DMVendorTRVAT
                                       on ven.Vendor_MasterID
                                       equals trvat.VTV_Vendor_ID
                                       into TrVat
                                       from trvat in TrVat.DefaultIfEmpty()
                                       select new
                                       {
                                           ven.Vendor_ID,
                                           ven.Vendor_MasterID,
                                           ven.Vendor_TIN,
                                           ven.Vendor_Address,
                                           ven.Vendor_Name,
                                           VTV_TR_ID = trvat.VTV_TR_ID > 0 ? trvat.VTV_TR_ID : 0,
                                           VTV_VAT_ID = trvat.VTV_VAT_ID > 0 ? trvat.VTV_VAT_ID : 0,
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

            var pendingList = (from rate in (from ven in (from ven in _context.DMVendor_Pending
                                                          select new
                                                          {
                                                              ven.Pending_ID,
                                                              ven.Pending_Vendor_MasterID,
                                                              ven.Pending_Vendor_TIN,
                                                              ven.Pending_Vendor_Address,
                                                              ven.Pending_Vendor_Name,
                                                              ven.Pending_Vendor_Creator_ID,
                                                              ven.Pending_Vendor_Approver_ID,
                                                              ven.Pending_Vendor_Status_ID,
                                                              ven.Pending_Vendor_Filed_Date
                                                          })
                                             join trvat in _context.DMVendorTRVAT_Pending
                                             on ven.Pending_Vendor_MasterID
                                             equals trvat.Pending_VTV_Vendor_ID
                                             into TrVat
                                             from trvat in TrVat.DefaultIfEmpty()
                                             select new
                                             {
                                                 ven.Pending_ID,
                                                 ven.Pending_Vendor_MasterID,
                                                 ven.Pending_Vendor_TIN,
                                                 ven.Pending_Vendor_Address,
                                                 ven.Pending_Vendor_Name,
                                                 VTV_TR_ID = trvat.Pending_VTV_TR_ID > 0 ? trvat.Pending_VTV_TR_ID : 0,
                                                 VTV_VAT_ID = trvat.Pending_VTV_VAT_ID > 0 ? trvat.Pending_VTV_VAT_ID : 0,
                                                 ven.Pending_Vendor_Creator_ID,
                                                 ven.Pending_Vendor_Approver_ID,
                                                 ven.Pending_Vendor_Status_ID,
                                                 ven.Pending_Vendor_Filed_Date
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
                                   rate.Pending_Vendor_TIN,
                                   rate.Pending_Vendor_Address,
                                   rate.VTV_TR_ID,
                                   rate.VTV_VAT_ID,
                                   tr.TR_WT_Title,
                                   TRRAte = tr.TR_Tax_Rate > 0 ? tr.TR_Tax_Rate : 0,
                                   vat.VAT_Name,
                                   VATRAte = vat.VAT_Rate > 0 ? vat.VAT_Rate : 0,
                                   rate.Pending_Vendor_Creator_ID,
                                   ApproverID = rate.Pending_Vendor_Approver_ID,
                                   CreatorName = user.User_LName + ", " + user.User_FName,
                                   ApproverName = rate.Pending_Vendor_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   Vendor_Created_Date = rate.Pending_Vendor_Filed_Date,
                                   Vendor_Last_Updated = rate.Pending_Vendor_Filed_Date,
                                   stat.Status_ID,
                                   stat.Status_Name
                               }).ToList();

            List<DMVendorViewModel> vmList = new List<DMVendorViewModel>();
            mList.GroupBy(o => o.Vendor_ID).Select(o => o.FirstOrDefault()).ToList().ForEach(m =>
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
                      Vendor_Tax_Rates = mList.Where(x => x.Vendor_ID == m.Vendor_ID && x.VTV_TR_ID > 0 && x.TRRAte > 0).Select(x => (x.TRRAte * 100) + "% - " + x.TR_WT_Title).ToList(),
                      Vendor_VAT = mList.Where(x => x.Vendor_ID == m.Vendor_ID && x.VTV_VAT_ID > 0 && x.VATRAte > 0).Select(x => (x.VATRAte * 100) + "% - " + x.VAT_Name).ToList()
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
                     Vendor_Tax_Rates = pendingList.Where(x => x.Pending_ID == m.Pending_ID && x.VTV_TR_ID > 0 && x.TRRAte > 0).Select(x => (x.TRRAte * 100) + "% - " + x.TR_WT_Title).ToList(),
                     Vendor_VAT = pendingList.Where(x => x.Pending_ID == m.Pending_ID && x.VTV_VAT_ID > 0 && x.VATRAte > 0).Select(x => (x.VATRAte * 100) + "% - " + x.VAT_Name).ToList()
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

                        vmList = vmList.AsQueryable().Where("Vendor_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                                .Select(e => e).ToList();
                    }
                }
            }

            return vmList;
        }

        public List<DMDeptViewModel> populateDept(DMFiltersViewModel filters)
        {
            var mList = (from e in (from c in _context.DMDept
                                    join user in _context.User
                                    on c.Dept_Creator_ID equals user.User_ID
                                    join stat in _context.StatusList
                                    on c.Dept_Status_ID equals stat.Status_ID
                                    where c.Dept_isDeleted == false && c.Dept_isActive == true
                                    select new
                                    {
                                        c.Dept_ID,
                                        c.Dept_MasterID,
                                        c.Dept_Name,
                                        c.Dept_Code,
                                        c.Dept_Budget_Unit,
                                        c.Dept_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.Dept_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.Dept_Created_Date,
                                        c.Dept_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.Dept_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         select new
                         {
                             e.Dept_ID,
                             e.Dept_MasterID,
                             e.Dept_Name,
                             e.Dept_Code,
                             e.Dept_Budget_Unit,
                             e.Dept_Creator_ID,
                             e.CreatorName,
                             e.Dept_Approver_ID,
                             ApproverName = e.Dept_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             e.Dept_Created_Date,
                             e.Dept_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();

            var pendingList = (from e in (from c in _context.DMDept_Pending
                                          join user in _context.User
                                          on c.Pending_Dept_Creator_ID equals user.User_ID
                                          join stat in _context.StatusList
                                          on c.Pending_Dept_Status_ID equals stat.Status_ID
                                          select new
                                          {
                                              c.Pending_Dept_ID,
                                              c.Pending_Dept_MasterID,
                                              c.Pending_Dept_Name,
                                              c.Pending_Dept_Code,
                                              c.Pending_Dept_Budget_Unit,
                                              c.Pending_Dept_Creator_ID,
                                              CreatorName = user.User_LName + ", " + user.User_FName,
                                              c.Pending_Dept_Approver_ID,
                                              stat.Status_ID,
                                              stat.Status_Name,
                                              c.Pending_Dept_Filed_Date,
                                          })
                               join apprv in _context.User
                               on e.Pending_Dept_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               select new
                               {
                                   e.Pending_Dept_ID,
                                   e.Pending_Dept_MasterID,
                                   e.Pending_Dept_Name,
                                   e.Pending_Dept_Code,
                                   e.Pending_Dept_Budget_Unit,
                                   e.Pending_Dept_Creator_ID,
                                   e.CreatorName,
                                   e.Pending_Dept_Approver_ID,
                                   ApproverName = e.Pending_Dept_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   e.Pending_Dept_Filed_Date,
                                   e.Status_ID,
                                   e.Status_Name
                               }).ToList();
            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            foreach (var m in mList)
            {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_MasterID = m.Dept_MasterID,
                    Dept_Name = m.Dept_Name,
                    Dept_Code = m.Dept_Code,
                    Dept_Budget_Unit = m.Dept_Budget_Unit,
                    Dept_Creator_Name = m.CreatorName ?? "N/A",
                    Dept_Approver_Name = m.ApproverName ?? "",
                    Dept_Created_Date = m.Dept_Created_Date,
                    Dept_Last_Updated = m.Dept_Last_Updated,
                    Dept_Creator_ID = m.Dept_Creator_ID,
                    Dept_Status_ID = m.Status_ID,
                    Dept_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_MasterID = m.Pending_Dept_MasterID,
                    Dept_Name = m.Pending_Dept_Name,
                    Dept_Code = m.Pending_Dept_Code,
                    Dept_Budget_Unit = m.Pending_Dept_Budget_Unit,
                    Dept_Creator_Name = m.CreatorName ?? "N/A",
                    Dept_Approver_Name = m.ApproverName ?? "",
                    Dept_Created_Date = m.Pending_Dept_Filed_Date,
                    Dept_Last_Updated = m.Pending_Dept_Filed_Date,
                    Dept_Creator_ID = m.Pending_Dept_Creator_ID,
                    Dept_Status_ID = m.Status_ID,
                    Dept_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }

            //FILTER
            var properties = filters.DF.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.DF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    vmList = vmList.AsQueryable().Where("Dept_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                            .Select(e => e).ToList();
                }
            }
            return vmList;
        }

        public List<DMCheckViewModel> populateCheck(DMFiltersViewModel filters)
        {
            var mList = (from e in (from c in _context.DMCheck
                                    join user in _context.User
                                    on c.Check_Creator_ID equals user.User_ID
                                    join stat in _context.StatusList
                                    on c.Check_Status_ID equals stat.Status_ID
                                    where c.Check_isDeleted == false && c.Check_isActive == true
                                    select new
                                    {
                                        c.Check_ID,
                                        c.Check_MasterID,
                                        c.Check_Input_Date,
                                        c.Check_Series_From,
                                        c.Check_Series_To,
                                        c.Check_Bank_Info,
                                        c.Check_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.Check_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.Check_Created_Date,
                                        c.Check_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.Check_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         select new
                         {
                             e.Check_ID,
                             e.Check_MasterID,
                             e.Check_Input_Date,
                             e.Check_Series_From,
                             e.Check_Series_To,
                             e.Check_Bank_Info,
                             e.Check_Creator_ID,
                             e.CreatorName,
                             e.Check_Approver_ID,
                             ApproverName = e.Check_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             e.Check_Created_Date,
                             e.Check_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();

            var pendingList = (from e in (from c in _context.DMCheck_Pending
                                           join user in _context.User
                                           on c.Pending_Check_Creator_ID equals user.User_ID
                                           join stat in _context.StatusList
                                           on c.Pending_Check_Status_ID equals stat.Status_ID
                                           select new
                                           {
                                               c.Pending_Check_ID,
                                               c.Pending_Check_MasterID,
                                               c.Pending_Check_Input_Date,
                                               c.Pending_Check_Series_From,
                                               c.Pending_Check_Series_To,
                                               c.Pending_Check_Bank_Info,
                                               c.Pending_Check_Creator_ID,
                                               CreatorName = user.User_LName + ", " + user.User_FName,
                                               c.Pending_Check_Approver_ID,
                                               stat.Status_ID,
                                               stat.Status_Name,
                                               c.Pending_Check_Filed_Date,
                                           })
                                join apprv in _context.User
                                on e.Pending_Check_Approver_ID
                                equals apprv.User_ID
                                into a
                                from apprv in a.DefaultIfEmpty()
                                select new
                                {
                                    e.Pending_Check_ID,
                                    e.Pending_Check_MasterID,
                                    e.Pending_Check_Input_Date,
                                    e.Pending_Check_Series_From,
                                    e.Pending_Check_Series_To,
                                    e.Pending_Check_Bank_Info,
                                    e.Pending_Check_Creator_ID,
                                    e.CreatorName,
                                    e.Pending_Check_Approver_ID,
                                    ApproverName = e.Pending_Check_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                    e.Pending_Check_Filed_Date,
                                    e.Status_ID,
                                    e.Status_Name
                                }).ToList();

            List<DMCheckViewModel> vmList = new List<DMCheckViewModel>();
            mList.GroupBy(o => o.Check_ID).Select(o => o.FirstOrDefault()).ToList().ForEach(m =>
                  vmList.Add(new DMCheckViewModel
                  {
                      Check_ID = m.Check_ID,
                      Check_MasterID = m.Check_MasterID,
                      Check_Input_Date = m.Check_Input_Date,
                      Check_Series_From = m.Check_Series_From,
                      Check_Series_To = m.Check_Series_To,
                      Check_Bank_Info = m.Check_Bank_Info,
                      Check_Creator_Name = m.CreatorName ?? "N/A",
                      Check_Approver_Name = m.ApproverName ?? "",
                      Check_Creator_ID = m.Check_Creator_ID,
                      Check_Created_Date = m.Check_Created_Date,
                      Check_Last_Updated = m.Check_Last_Updated,
                      Check_Status_ID = m.Status_ID,
                      Check_Status = m.Status_Name ?? "N/A"
                  })
            );

            pendingList.GroupBy(o => o.Pending_Check_ID).Select(o => o.FirstOrDefault()).ToList().ForEach(m =>
                 vmList.Add(new DMCheckViewModel
                 {
                     Check_ID = m.Pending_Check_ID,
                     Check_MasterID = m.Pending_Check_MasterID,
                     Check_Input_Date = m.Pending_Check_Input_Date,
                     Check_Series_From = m.Pending_Check_Series_From,
                     Check_Series_To = m.Pending_Check_Series_To,
                     Check_Bank_Info = m.Pending_Check_Bank_Info,
                     Check_Creator_Name = m.CreatorName ?? "N/A",
                     Check_Approver_Name = m.ApproverName ?? "",
                     Check_Creator_ID = m.Pending_Check_Creator_ID,
                     Check_Created_Date = m.Pending_Check_Filed_Date,
                     Check_Last_Updated = m.Pending_Check_Filed_Date,
                     Check_Status_ID = m.Status_ID,
                      Check_Status = m.Status_Name ?? "N/A"
                 })
            );
            //FILTER
            var properties = filters.CKF.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.CKF).ToString();
                if (toStr.Length > 0)
                {
                    if (toStr != "" && toStr != "0")
                    {
                        vmList = vmList.AsQueryable().Where("Check_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                                .Select(e => e).ToList();
                    }
                }
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
                      Account_Name = m.Account_Name ?? "",
                      Account_Code = m.Account_Code ?? "",
                      Account_Budget_Code = m.Account_Budget_Code ?? "",
                      Account_No = m.Account_No ?? "",
                      Account_Cust = m.Account_Cust ?? "",
                      Account_Div = m.Account_Div ?? "",
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
                     Account_Name = m.Pending_Account_Name ?? "",
                     Account_Code = m.Pending_Account_Code ?? "",
                     Account_Budget_Code = m.Pending_Account_Budget_Code ?? "",
                     Account_No = m.Pending_Account_No ?? "",
                     Account_Cust = m.Pending_Account_Cust ?? "",
                     Account_Div = m.Pending_Account_Div ?? "",
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

            //FILTER
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.AF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    vmList = vmList.AsQueryable().Where("Account_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                                .Select(e => e).ToList();
                }
            }
            return vmList;
        }

        public List<DMVATViewModel> populateVAT(DMFiltersViewModel filters)
        {
            var mList = (from e in (from c in _context.DMVAT
                                    join user in _context.User
                                    on c.VAT_Creator_ID equals user.User_ID
                                    join stat in _context.StatusList
                                    on c.VAT_Status_ID equals stat.Status_ID
                                    where c.VAT_isDeleted == false && c.VAT_isActive == true
                                    select new
                                    {
                                        c.VAT_ID,
                                        c.VAT_MasterID,
                                        c.VAT_Name,
                                        c.VAT_Rate,
                                        c.VAT_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.VAT_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.VAT_Created_Date,
                                        c.VAT_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.VAT_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         select new
                         {
                             e.VAT_ID,
                             e.VAT_MasterID,
                             e.VAT_Name,
                             e.VAT_Rate,
                             e.VAT_Creator_ID,
                             e.CreatorName,
                             e.VAT_Approver_ID,
                             ApproverName = e.VAT_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             e.VAT_Created_Date,
                             e.VAT_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();

            var pendingList = (from e in (from c in _context.DMVAT_Pending
                                          join user in _context.User
                                          on c.Pending_VAT_Creator_ID equals user.User_ID
                                          join stat in _context.StatusList
                                          on c.Pending_VAT_Status_ID equals stat.Status_ID
                                          select new
                                          {
                                              c.Pending_VAT_ID,
                                              c.Pending_VAT_MasterID,
                                              c.Pending_VAT_Name,
                                              c.Pending_VAT_Rate,
                                              c.Pending_VAT_Creator_ID,
                                              CreatorName = user.User_LName + ", " + user.User_FName,
                                              c.Pending_VAT_Approver_ID,
                                              stat.Status_ID,
                                              stat.Status_Name,
                                              c.Pending_VAT_Filed_Date,
                                          })
                               join apprv in _context.User
                               on e.Pending_VAT_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               select new
                               {
                                   e.Pending_VAT_ID,
                                   e.Pending_VAT_MasterID,
                                   e.Pending_VAT_Name,
                                   e.Pending_VAT_Rate,
                                   e.Pending_VAT_Creator_ID,
                                   e.CreatorName,
                                   e.Pending_VAT_Approver_ID,
                                   ApproverName = e.Pending_VAT_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   e.Pending_VAT_Filed_Date,
                                   e.Status_ID,
                                   e.Status_Name
                               }).ToList();
            List<DMVATViewModel> vmList = new List<DMVATViewModel>();
            foreach (var m in mList)
            {
                DMVATViewModel vm = new DMVATViewModel
                {
                    VAT_ID = m.VAT_ID,
                    VAT_MasterID = m.VAT_MasterID,
                    VAT_Name = m.VAT_Name,
                    VAT_Rate = m.VAT_Rate,
                    VAT_Creator_Name = m.CreatorName ?? "N/A",
                    VAT_Approver_Name = m.ApproverName ?? "",
                    VAT_Created_Date = m.VAT_Created_Date,
                    VAT_Last_Updated = m.VAT_Last_Updated,
                    VAT_Creator_ID = m.VAT_Creator_ID,
                    VAT_Status_ID = m.Status_ID,
                    VAT_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                DMVATViewModel vm = new DMVATViewModel
                {
                    VAT_ID = m.Pending_VAT_ID,
                    VAT_MasterID = m.Pending_VAT_MasterID,
                    VAT_Name = m.Pending_VAT_Name,
                    VAT_Rate = m.Pending_VAT_Rate,
                    VAT_Creator_Name = m.CreatorName ?? "N/A",
                    VAT_Approver_Name = m.ApproverName ?? "",
                    VAT_Created_Date = m.Pending_VAT_Filed_Date,
                    VAT_Last_Updated = m.Pending_VAT_Filed_Date,
                    VAT_Creator_ID = m.Pending_VAT_Creator_ID,
                    VAT_Status_ID = m.Status_ID,
                    VAT_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }

            //FILTER
            var properties = filters.VF.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.VF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    if (subStr == "Rate")
                    {
                        vmList = vmList.AsQueryable().Where("VAT_" + subStr + ".ToString().Contains(@0)", toStr)
                                .Select(e => e).ToList();
                    }
                    else
                    {
                        vmList = vmList.AsQueryable().Where("VAT_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                                .Select(e => e).ToList();
                    }
                }
            }
            return vmList;
        }

        public List<DMFBTViewModel> populateFBT(DMFiltersViewModel filters)
        {
            var mList = (from e in (from c in _context.DMFBT
                                    join user in _context.User
                                    on c.FBT_Creator_ID equals user.User_ID
                                    join stat in _context.StatusList
                                    on c.FBT_Status_ID equals stat.Status_ID
                                    where c.FBT_isDeleted == false && c.FBT_isActive == true
                                    select new
                                    {
                                        c.FBT_ID,
                                        c.FBT_MasterID,
                                        c.FBT_Name,
                                        c.FBT_Tax_Rate,
                                        c.FBT_Formula,
                                        c.FBT_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.FBT_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.FBT_Created_Date,
                                        c.FBT_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.FBT_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         select new
                         {
                             e.FBT_ID,
                             e.FBT_MasterID,
                             e.FBT_Name,
                             e.FBT_Tax_Rate,
                             e.FBT_Formula,
                             e.FBT_Creator_ID,
                             e.CreatorName,
                             e.FBT_Approver_ID,
                             ApproverName = e.FBT_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             e.FBT_Created_Date,
                             e.FBT_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();

            var pendingList = (from e in (from c in _context.DMFBT_Pending
                                          join user in _context.User
                                          on c.Pending_FBT_Creator_ID equals user.User_ID
                                          join stat in _context.StatusList
                                          on c.Pending_FBT_Status_ID equals stat.Status_ID
                                          select new
                                          {
                                              c.Pending_FBT_ID,
                                              c.Pending_FBT_MasterID,
                                              c.Pending_FBT_Name,
                                              c.Pending_FBT_Tax_Rate,
                                              c.Pending_FBT_Formula,
                                              c.Pending_FBT_Creator_ID,
                                              CreatorName = user.User_LName + ", " + user.User_FName,
                                              c.Pending_FBT_Approver_ID,
                                              stat.Status_ID,
                                              stat.Status_Name,
                                              c.Pending_FBT_Filed_Date,
                                          })
                               join apprv in _context.User
                               on e.Pending_FBT_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               select new
                               {
                                   e.Pending_FBT_ID,
                                   e.Pending_FBT_MasterID,
                                   e.Pending_FBT_Name,
                                   e.Pending_FBT_Tax_Rate,
                                   e.Pending_FBT_Formula,
                                   e.Pending_FBT_Creator_ID,
                                   e.CreatorName,
                                   e.Pending_FBT_Approver_ID,
                                   ApproverName = e.Pending_FBT_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   e.Pending_FBT_Filed_Date,
                                   e.Status_ID,
                                   e.Status_Name
                               }).ToList();

            List<DMFBTViewModel> vmList = new List<DMFBTViewModel>();
            foreach (var m in mList)
            {
                DMFBTViewModel vm = new DMFBTViewModel
                {
                    FBT_ID = m.FBT_ID,
                    FBT_MasterID = m.FBT_MasterID,
                    FBT_Name = m.FBT_Name,
                    FBT_Tax_Rate = m.FBT_Tax_Rate,
                    FBT_Formula = m.FBT_Formula,
                    FBT_Creator_Name = m.CreatorName ?? "N/A",
                    FBT_Approver_Name = m.ApproverName ?? "",
                    FBT_Created_Date = m.FBT_Created_Date,
                    FBT_Last_Updated = m.FBT_Last_Updated,
                    FBT_Creator_ID = m.FBT_Creator_ID,
                    FBT_Status_ID = m.Status_ID,
                    FBT_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                DMFBTViewModel vm = new DMFBTViewModel
                {
                    FBT_ID = m.Pending_FBT_ID,
                    FBT_MasterID = m.Pending_FBT_MasterID,
                    FBT_Name = m.Pending_FBT_Name,
                    FBT_Tax_Rate = m.Pending_FBT_Tax_Rate,
                    FBT_Formula = m.Pending_FBT_Formula,
                    FBT_Creator_Name = m.CreatorName ?? "N/A",
                    FBT_Approver_Name = m.ApproverName ?? "",
                    FBT_Created_Date = m.Pending_FBT_Filed_Date,
                    FBT_Last_Updated = m.Pending_FBT_Filed_Date,
                    FBT_Creator_ID = m.Pending_FBT_Creator_ID,
                    FBT_Status_ID = m.Status_ID,
                    FBT_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }

            //FILTER
            var properties = filters.FF.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.FF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    vmList = vmList .AsQueryable().Where("FBT_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                            .Select(e => e).ToList();
                }
            }
            return vmList;
        }

        public List<DMTRViewModel> populateTR(DMFiltersViewModel filters)
        {
            var mList = (from e in (from c in _context.DMTR
                                    join user in _context.User
                                    on c.TR_Creator_ID equals user.User_ID
                                    join stat in _context.StatusList
                                    on c.TR_Status_ID equals stat.Status_ID
                                    where c.TR_isDeleted == false && c.TR_isActive == true
                                    select new
                                    {
                                        c.TR_ID,
                                        c.TR_MasterID,
                                        c.TR_WT_Title,
                                        c.TR_Tax_Rate,
                                        c.TR_Nature,
                                        c.TR_Nature_Income_Payment,
                                        c.TR_ATC,
                                        c.TR_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.TR_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.TR_Created_Date,
                                        c.TR_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.TR_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         select new
                         {
                             e.TR_ID,
                             e.TR_MasterID,
                             e.TR_WT_Title,
                             e.TR_Tax_Rate,
                             e.TR_Nature,
                             e.TR_Nature_Income_Payment,
                             e.TR_ATC,
                             e.TR_Creator_ID,
                             e.CreatorName,
                             e.TR_Approver_ID,
                             ApproverName = e.TR_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             e.TR_Created_Date,
                             e.TR_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();

            var pendingList = (from e in (from c in _context.DMTR_Pending
                                          join user in _context.User
                                          on c.Pending_TR_Creator_ID equals user.User_ID
                                          join stat in _context.StatusList
                                          on c.Pending_TR_Status_ID equals stat.Status_ID
                                          select new
                                          {
                                              c.Pending_TR_ID,
                                              c.Pending_TR_MasterID,
                                              c.Pending_TR_WT_Title,
                                              c.Pending_TR_Tax_Rate,
                                              c.Pending_TR_Nature,
                                              c.Pending_TR_Nature_Income_Payment,
                                              c.Pending_TR_ATC,
                                              c.Pending_TR_Creator_ID,
                                              CreatorName = user.User_LName + ", " + user.User_FName,
                                              c.Pending_TR_Approver_ID,
                                              stat.Status_ID,
                                              stat.Status_Name,
                                              c.Pending_TR_Filed_Date,
                                          })
                               join apprv in _context.User
                               on e.Pending_TR_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               select new
                               {
                                   e.Pending_TR_ID,
                                   e.Pending_TR_MasterID,
                                   e.Pending_TR_WT_Title,
                                   e.Pending_TR_Tax_Rate,
                                   e.Pending_TR_Nature,
                                   e.Pending_TR_Nature_Income_Payment,
                                   e.Pending_TR_ATC,
                                   e.Pending_TR_Creator_ID,
                                   e.CreatorName,
                                   e.Pending_TR_Approver_ID,
                                   ApproverName = e.Pending_TR_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   e.Pending_TR_Filed_Date,
                                   e.Status_ID,
                                   e.Status_Name
                               }).ToList();

            List<DMTRViewModel> vmList = new List<DMTRViewModel>();
            foreach (var m in mList)
            {
                DMTRViewModel vm = new DMTRViewModel
                {
                    TR_ID = m.TR_ID,
                    TR_MasterID = m.TR_MasterID,
                    TR_WT_Title = m.TR_WT_Title,
                    TR_Nature = m.TR_Nature,
                    TR_Nature_Income_Payment = m.TR_Nature_Income_Payment,
                    TR_ATC = m.TR_ATC,
                    TR_Tax_Rate = m.TR_Tax_Rate,
                    TR_Creator_Name = m.CreatorName ?? "N/A",
                    TR_Approver_Name = m.ApproverName ?? "",
                    TR_Created_Date = m.TR_Created_Date,
                    TR_Last_Updated = m.TR_Last_Updated,
                    TR_Creator_ID = m.TR_Creator_ID,
                    TR_Status_ID = m.Status_ID,
                    TR_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                DMTRViewModel vm = new DMTRViewModel
                {
                    TR_ID = m.Pending_TR_ID,
                    TR_MasterID = m.Pending_TR_MasterID,
                    TR_WT_Title = m.Pending_TR_WT_Title,
                    TR_Nature = m.Pending_TR_Nature,
                    TR_Nature_Income_Payment = m.Pending_TR_Nature_Income_Payment,
                    TR_ATC = m.Pending_TR_ATC,
                    TR_Tax_Rate = m.Pending_TR_Tax_Rate,
                    TR_Creator_Name = m.CreatorName ?? "N/A",
                    TR_Approver_Name = m.ApproverName ?? "",
                    TR_Created_Date = m.Pending_TR_Filed_Date,
                    TR_Last_Updated = m.Pending_TR_Filed_Date,
                    TR_Creator_ID = m.Pending_TR_Creator_ID,
                    TR_Status_ID = m.Status_ID,
                    TR_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }

            //FILTER
            var properties = filters.TF.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.TF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    vmList = vmList .AsQueryable().Where("TR_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                            .Select(e => e).ToList();
                }
            }
            return vmList;
        }

        public List<DMCurrencyViewModel> populateCurr(DMFiltersViewModel filters)
        {
            var mList = (from e in (from c in _context.DMCurrency
                                    join user in _context.User
                                    on c.Curr_Creator_ID equals user.User_ID
                                    join stat in _context.StatusList
                                    on c.Curr_Status_ID equals stat.Status_ID
                                    where c.Curr_isDeleted == false && c.Curr_isActive == true
                                    select new
                                    {
                                        c.Curr_ID,
                                        c.Curr_MasterID,
                                        c.Curr_Name,
                                        c.Curr_CCY_ABBR,
                                        c.Curr_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.Curr_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.Curr_Created_Date,
                                        c.Curr_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.Curr_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         select new
                         {
                             e.Curr_ID,
                             e.Curr_MasterID,
                             e.Curr_Name,
                             e.Curr_CCY_ABBR,
                             e.Curr_Creator_ID,
                             e.CreatorName,
                             e.Curr_Approver_ID,
                             ApproverName = e.Curr_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             e.Curr_Created_Date,
                             e.Curr_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();

            var pendingList = (from e in (from c in _context.DMCurrency_Pending
                                          join user in _context.User
                                          on c.Pending_Curr_Creator_ID equals user.User_ID
                                          join stat in _context.StatusList
                                          on c.Pending_Curr_Status_ID equals stat.Status_ID
                                          select new
                                          {
                                              c.Pending_Curr_ID,
                                              c.Pending_Curr_MasterID,
                                              c.Pending_Curr_Name,
                                              c.Pending_Curr_CCY_ABBR,
                                              c.Pending_Curr_Creator_ID,
                                              CreatorName = user.User_LName + ", " + user.User_FName,
                                              c.Pending_Curr_Approver_ID,
                                              stat.Status_ID,
                                              stat.Status_Name,
                                              c.Pending_Curr_Filed_Date,
                                          })
                               join apprv in _context.User
                               on e.Pending_Curr_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               select new
                               {
                                   e.Pending_Curr_ID,
                                   e.Pending_Curr_MasterID,
                                   e.Pending_Curr_Name,
                                   e.Pending_Curr_CCY_ABBR,
                                   e.Pending_Curr_Creator_ID,
                                   e.CreatorName,
                                   e.Pending_Curr_Approver_ID,
                                   ApproverName = e.Pending_Curr_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   e.Pending_Curr_Filed_Date,
                                   e.Status_ID,
                                   e.Status_Name
                               }).ToList();
            List<DMCurrencyViewModel> vmList = new List<DMCurrencyViewModel>();
            foreach (var m in mList)
            {
                DMCurrencyViewModel vm = new DMCurrencyViewModel
                {
                    Curr_ID = m.Curr_ID,
                    Curr_MasterID = m.Curr_MasterID,
                    Curr_Name = m.Curr_Name,
                    Curr_CCY_ABBR = m.Curr_CCY_ABBR,
                    Curr_Creator_Name = m.CreatorName ?? "N/A",
                    Curr_Approver_Name = m.ApproverName ?? "",
                    Curr_Created_Date = m.Curr_Created_Date,
                    Curr_Last_Updated = m.Curr_Last_Updated,
                    Curr_Creator_ID = m.Curr_Creator_ID,
                    Curr_Status_ID = m.Status_ID,
                    Curr_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                DMCurrencyViewModel vm = new DMCurrencyViewModel
                {
                    Curr_ID = m.Pending_Curr_ID,
                    Curr_MasterID = m.Pending_Curr_MasterID,
                    Curr_Name = m.Pending_Curr_Name,
                    Curr_CCY_ABBR = m.Pending_Curr_CCY_ABBR,
                    Curr_Creator_Name = m.CreatorName ?? "N/A",
                    Curr_Approver_Name = m.ApproverName ?? "",
                    Curr_Created_Date = m.Pending_Curr_Filed_Date,
                    Curr_Last_Updated = m.Pending_Curr_Filed_Date,
                    Curr_Creator_ID = m.Pending_Curr_Creator_ID,
                    Curr_Status_ID = m.Status_ID,
                    Curr_Status = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }

            //FILTER
            var properties = filters.CF.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.CF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    vmList = vmList .AsQueryable().Where("Curr_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                            .Select(e => e).ToList();
                }
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
                                          where c.Pending_Emp_Type == "Regular"
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
                    vmList = vmList .AsQueryable().Where("Emp_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
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
                                          where c.Pending_Emp_Type == "Temporary"
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
                    vmList = vmList .AsQueryable().Where("Emp_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
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
            //FILTER
            var properties = filters.CUF.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.CUF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    vmList = vmList.AsQueryable().Where("AccountGroup_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                            .Select(e => e).ToList();
                }
            }
            return vmList;
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
                    vmList = vmList .AsQueryable().Where("BCS_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                            .Select(e => e).ToList();
                }
            }
            return vmList;
        }

        public List<DMAccountGroupViewModel> populateAG(DMFiltersViewModel filters)
        {
            var mList = (from e in (from c in _context.DMAccountGroup
                                    join user in _context.User
                                    on c.AccountGroup_Creator_ID equals user.User_ID
                                    join stat in _context.StatusList
                                    on c.AccountGroup_Status_ID equals stat.Status_ID
                                    where c.AccountGroup_isDeleted == false && c.AccountGroup_isActive == true
                                    select new
                                    {
                                        c.AccountGroup_ID,
                                        c.AccountGroup_MasterID,
                                        c.AccountGroup_Name,
                                        c.AccountGroup_Code,
                                        c.AccountGroup_Creator_ID,
                                        CreatorName = user.User_LName + ", " + user.User_FName,
                                        c.AccountGroup_Approver_ID,
                                        stat.Status_ID,
                                        stat.Status_Name,
                                        c.AccountGroup_Created_Date,
                                        c.AccountGroup_Last_Updated
                                    })
                         join apprv in _context.User
                         on e.AccountGroup_Approver_ID
                         equals apprv.User_ID
                         into a
                         from apprv in a.DefaultIfEmpty()
                         select new
                         {
                             e.AccountGroup_ID,
                             e.AccountGroup_MasterID,
                             e.AccountGroup_Name,
                             e.AccountGroup_Code,
                             e.AccountGroup_Creator_ID,
                             e.CreatorName,
                             e.AccountGroup_Approver_ID,
                             ApproverName = e.AccountGroup_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                             e.AccountGroup_Created_Date,
                             e.AccountGroup_Last_Updated,
                             e.Status_ID,
                             e.Status_Name
                         }).ToList();

            var pendingList = (from e in (from c in _context.DMAccountGroup_Pending
                                          join user in _context.User
                                          on c.Pending_AccountGroup_Creator_ID equals user.User_ID
                                          join stat in _context.StatusList
                                          on c.Pending_AccountGroup_Status_ID equals stat.Status_ID
                                          select new
                                          {
                                              c.Pending_AccountGroup_ID,
                                              c.Pending_AccountGroup_MasterID,
                                              c.Pending_AccountGroup_Code,
                                              c.Pending_AccountGroup_Name,
                                              c.Pending_AccountGroup_Creator_ID,
                                              CreatorName = user.User_LName + ", " + user.User_FName,
                                              c.Pending_AccountGroup_Approver_ID,
                                              stat.Status_ID,
                                              stat.Status_Name,
                                              c.Pending_AccountGroup_Filed_Date,
                                          })
                               join apprv in _context.User
                               on e.Pending_AccountGroup_Approver_ID
                               equals apprv.User_ID
                               into a
                               from apprv in a.DefaultIfEmpty()
                               select new
                               {
                                   e.Pending_AccountGroup_ID,
                                   e.Pending_AccountGroup_MasterID,
                                   e.Pending_AccountGroup_Name,
                                   e.Pending_AccountGroup_Code,
                                   e.Pending_AccountGroup_Creator_ID,
                                   e.CreatorName,
                                   e.Pending_AccountGroup_Approver_ID,
                                   ApproverName = e.Pending_AccountGroup_Approver_ID > 0 ? apprv.User_LName + ", " + apprv.User_FName : "",
                                   e.Pending_AccountGroup_Filed_Date,
                                   e.Status_ID,
                                   e.Status_Name
                               }).ToList();
            List<DMAccountGroupViewModel> vmList = new List<DMAccountGroupViewModel>();
            foreach (var m in mList)
            {
                DMAccountGroupViewModel vm = new DMAccountGroupViewModel
                {
                    AccountGroup_ID = m.AccountGroup_ID,
                    AccountGroup_MasterID = m.AccountGroup_MasterID,
                    AccountGroup_Code = m.AccountGroup_Code,
                    AccountGroup_Name = m.AccountGroup_Name,
                    AccountGroup_Creator_Name = m.CreatorName ?? "N/A",
                    AccountGroup_Approver_Name = m.ApproverName ?? "",
                    AccountGroup_Created_Date = m.AccountGroup_Created_Date,
                    AccountGroup_Last_Updated = m.AccountGroup_Last_Updated,
                    AccountGroup_Creator_ID = m.AccountGroup_Creator_ID,
                    AccountGroup_Status_ID = m.Status_ID,
                    AccountGroup_Status_Name = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }
            foreach (var m in pendingList)
            {
                DMAccountGroupViewModel vm = new DMAccountGroupViewModel
                {
                    AccountGroup_ID = m.Pending_AccountGroup_ID,
                    AccountGroup_MasterID = m.Pending_AccountGroup_MasterID,
                    AccountGroup_Name = m.Pending_AccountGroup_Name,
                    AccountGroup_Code = m.Pending_AccountGroup_Code,
                    AccountGroup_Creator_Name = m.CreatorName ?? "N/A",
                    AccountGroup_Approver_Name = m.ApproverName ?? "",
                    AccountGroup_Created_Date = m.Pending_AccountGroup_Filed_Date,
                    AccountGroup_Last_Updated = m.Pending_AccountGroup_Filed_Date,
                    AccountGroup_Creator_ID = m.Pending_AccountGroup_Creator_ID,
                    AccountGroup_Status_ID = m.Status_ID,
                    AccountGroup_Status_Name = m.Status_Name ?? "N/A"
                };
                vmList.Add(vm);
            }

            //FILTER
            var properties = filters.AGF.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var subStr = propertyName.Substring(propertyName.IndexOf("_") + 1);
                var toStr = property.GetValue(filters.AGF).ToString();
                if (toStr != "" && toStr != "0")
                {
                    vmList = vmList.AsQueryable().Where("AccountGroup_" + subStr + ".ToLower().Contains(@0)", toStr.ToLower())
                            .Select(e => e).ToList();
                }
            }
            return vmList;
        }
    }
}