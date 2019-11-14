using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Models.Gbase;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using ExpenseProcessingSystem.ViewModels.TransFailed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using ModelStateDictionary = Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

namespace ExpenseProcessingSystem.Services.Controller_Services
{
    public class TransFailedService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private readonly GOExpressContext _GOContext;
        private readonly GWriteContext _gWriteContext;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ModalService _modalservice;
        private FilterQueryService _filterservice;
        XElement xelemAcc = XElement.Load("wwwroot/xml/GlobalAccounts.xml");
        XElement xelemLiq = XElement.Load("wwwroot/xml/LiquidationValue.xml");
        XElement xelemReport = XElement.Load("wwwroot/xml/ReportHeader.xml");
        XElement xelemNC = XElement.Load("wwwroot/xml/NonCashAccounts.xml");
        private ModelStateDictionary _modelState;
        private NumberToText _class;
        private Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState;
        private IHostingEnvironment hostingEnvironment;
        private int[] status = { GlobalSystemValues.STATUS_POSTED, GlobalSystemValues.STATUS_FOR_CLOSING,
                            GlobalSystemValues.STATUS_FOR_PRINTING };

        public TransFailedService(IHttpContextAccessor httpContextAccessor, EPSDbContext context, GOExpressContext goContext, GWriteContext gWriteContext, ModelStateDictionary modelState, IHostingEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _GOContext = goContext;
            _gWriteContext = gWriteContext;
            _modelState = modelState;
            _hostingEnvironment = hostingEnvironment;
            _modalservice = new ModalService(_httpContextAccessor, _context, _gWriteContext);
            _filterservice = new FilterQueryService();
            _class = new NumberToText();
        }

        public string getUserRole(string id)
        {
            var data = _context.User.Where(x => x.User_ID == int.Parse(id))
                .Select(x => x.User_Role).FirstOrDefault() ?? "";
            return data;
        }

        public List<TransFailedTableDataViewModel> GetTransFailedList()
        {
            List<TransFailedTableDataViewModel> vm = new List<TransFailedTableDataViewModel>();
            int[] errorStat = { GlobalSystemValues.STATUS_ERROR, GlobalSystemValues.STATUS_RESENDING,
                                GlobalSystemValues.STATUS_REVERSING, GlobalSystemValues.STATUS_REVERSING_ERROR };

            List<int> listOfErrorExpense = _context.ExpenseEntry.Where(x => x.Expense_Status == GlobalSystemValues.STATUS_ERROR)
                                    .Select(x => x.Expense_ID).Distinct().ToList();

            int actionResend = GlobalSystemValues.GBaseErrResend;
            int actionReverse = GlobalSystemValues.GBaseErrReverse;
            int actionReverseResend = GlobalSystemValues.GBaseErrReverseResend;
            int[] inProcessStats = { GlobalSystemValues.STATUS_PENDING, GlobalSystemValues.STATUS_RESENDING, GlobalSystemValues.STATUS_REVERSING };

            var data = (from trans in _context.ExpenseTransLists
                        join exp in _context.ExpenseEntry on trans.TL_ExpenseID equals exp.Expense_ID
                        where exp.Expense_Status == GlobalSystemValues.STATUS_ERROR
                        select new
                        {
                            trans.TL_ID,
                            trans.TL_Liquidation,
                            trans.TL_StatusID,
                            trans.TL_TransID,
                            trans.TL_GBaseMessage,
                            trans.TL_GoExpress_ID,
                            trans.TL_GoExpHist_ID,
                            exp.Expense_ID,
                            exp.Expense_Type,
                            exp.Expense_Number
                        }).ToList();

            var goexphistlist = _context.GOExpressHist.Where(x => data.Select(y => y.TL_GoExpHist_ID).Contains(x.GOExpHist_Id)).ToList();
            ///join rev in _context.ReversalEntry on trans.TL_GoExpHist_ID equals rev.Reversal_GOExpressHistID // CHANGED
            foreach (var i in data)
            {
                var gohist = goexphistlist.Where(x => x.GOExpHist_Id == i.TL_GoExpHist_ID).FirstOrDefault();
                if (gohist == null) continue;

                string status = "";
                string actionLabel = "";
                int actionID = 0;
                bool isDisable = false;
                bool canReject = false;

                if (i.TL_StatusID == GlobalSystemValues.STATUS_ERROR)
                {
                    actionLabel = "RE-SEND";
                    actionID = actionResend;
                }
                else if (i.TL_StatusID == GlobalSystemValues.STATUS_APPROVED)
                {
                    status = GlobalSystemValues.getStatus(GlobalSystemValues.STATUS_FOR_CLOSING);
                    actionLabel = "REVERSE";
                    actionID = actionReverse;

                    int[] noReverse = { GlobalSystemValues.STATUS_REVERSING_ERROR };

                    //If already reversed but it was error, avoid double reversing process to one transaction.
                    int hasReversingError = data.Where(x => x.Expense_ID == i.Expense_ID && noReverse.Contains(x.TL_StatusID)).ToList().Count;
                    if (hasReversingError > 0)
                    {
                        isDisable = true;
                    }
                }
                else if (i.TL_StatusID == GlobalSystemValues.STATUS_REVERSING_ERROR)
                {
                    actionLabel = "RE-SEND";
                    actionID = actionReverseResend;
                }
                else if (i.TL_StatusID == GlobalSystemValues.STATUS_RESENDING_COMPLETE)
                {
                    status = GlobalSystemValues.getStatus(GlobalSystemValues.STATUS_RESENDING_COMPLETE);
                    actionLabel = "REVERSE";
                    actionID = actionReverse;
                }

                //Get number of count of related transactions are in process, in order to disabled the action button.
                int inProcess = data.Where(x => x.Expense_ID == i.Expense_ID && inProcessStats.Contains(x.TL_StatusID)).ToList().Count;
                if (inProcess > 0)
                {
                    isDisable = true;
                }
                else
                {
                    if (actionID == actionResend)
                    {
                        int[] resendProhibit = { GlobalSystemValues.STATUS_REVERSING_COMPLETE, GlobalSystemValues.STATUS_REVERSING_ERROR };
                        int isReverseProcess = data.Where(x => x.Expense_ID == i.Expense_ID && resendProhibit.Contains(x.TL_StatusID)).ToList().Count;
                        if (isReverseProcess > 0)
                        {
                            isDisable = true;
                        }
                    }
                    else if (actionID == actionReverse)
                    {
                        int[] reverseProhibit = { GlobalSystemValues.STATUS_REVERSING_COMPLETE, GlobalSystemValues.STATUS_REVERSING_ERROR };
                        int isReverseProcess = data.Where(x => x.Expense_ID == i.Expense_ID && reverseProhibit.Contains(x.TL_StatusID)).ToList().Count;
                        if (isReverseProcess > 0)
                        {
                            if (i.TL_StatusID == GlobalSystemValues.STATUS_APPROVED)
                            {
                                isDisable = true;
                            }
                        }
                    }
                }

                //Get number of count of related transactions that are all ERROR only, in order to show the REJECT button.
                int notErrorCount = data.Where(x => x.Expense_ID == i.Expense_ID && x.TL_StatusID != GlobalSystemValues.STATUS_ERROR).ToList().Count;
                if (notErrorCount == 0) canReject = true;

                vm.Add(new TransFailedTableDataViewModel
                {
                    TF_VALUE_DATE = ConvGbDateToDateTime(gohist.GOExpHist_ValueDate).ToString("MM/dd/yyyy"),
                    TF_TRANS_NO = i.TL_TransID.ToString().PadLeft(5, '0'),
                    TF_VOUCHER_NO = getVoucherNo(i.Expense_Type, ConvGbDateToDateTime(gohist.GOExpHist_ValueDate), i.Expense_Number, i.TL_Liquidation),
                    TF_REMARKS = gohist.GOExpHist_Remarks,
                    TF_DEBIT_ACCOUNTS = "",
                    TF_CREDIT_ACCONTS = "",
                    TF_STATUS = (!String.IsNullOrEmpty(status)) ? status : GlobalSystemValues.getStatus(i.TL_StatusID),
                    TF_ACTION_LABEL = actionLabel,
                    TF_ACTION_ID = actionID,
                    TF_ACTION_IS_DISABLED = isDisable,
                    TF_CAN_REJECT = canReject,
                    TF_STATUS_ID = i.TL_StatusID,
                    TF_GBASE_MESSAGE = i.TL_GBaseMessage,
                    TF_TRANS_LIST_ID = i.TL_ID,
                    TF_TRANS_IS_LIQ = i.TL_Liquidation,
                    TF_TRANS_ENTRY_ID = i.Expense_ID
                });
            }

            //Get liquidation list
            var dataLiq = (from trans in _context.ExpenseTransLists
                           join liq in _context.LiquidationEntryDetails on trans.TL_ExpenseID equals liq.ExpenseEntryModel.Expense_ID
                           join exp in _context.ExpenseEntry on liq.ExpenseEntryModel.Expense_ID equals exp.Expense_ID
                           where liq.Liq_Status == GlobalSystemValues.STATUS_ERROR && trans.TL_Liquidation == true
                           select new
                           {
                               trans.TL_ID,
                               trans.TL_Liquidation,
                               trans.TL_StatusID,
                               trans.TL_TransID,
                               trans.TL_GBaseMessage,
                               trans.TL_GoExpress_ID,
                               trans.TL_GoExpHist_ID,
                               liq.ExpenseEntryModel.Expense_ID,
                               GlobalSystemValues.TYPE_LIQ,
                               exp.Expense_Number
                           }).ToList();

            goexphistlist = _context.GOExpressHist.Where(x => dataLiq.Select(y => y.TL_GoExpHist_ID).Contains(x.GOExpHist_Id)).ToList();

            foreach (var i in dataLiq)
            {
                var gohist = goexphistlist.Where(x => x.GOExpHist_Id == i.TL_GoExpHist_ID).FirstOrDefault();
                if (gohist == null) continue;

                string status = "";
                string actionLabel = "";
                int actionID = 0;
                bool isDisable = false;
                bool canReject = false;

                if (i.TL_StatusID == GlobalSystemValues.STATUS_ERROR)
                {
                    actionLabel = "RE-SEND";
                    actionID = actionResend;
                }
                else if (i.TL_StatusID == GlobalSystemValues.STATUS_APPROVED)
                {
                    status = GlobalSystemValues.getStatus(GlobalSystemValues.STATUS_FOR_CLOSING);
                    actionLabel = "REVERSE";
                    actionID = actionReverse;

                    int[] noReverse = { GlobalSystemValues.STATUS_REVERSING_ERROR };

                    //If already reversed but it was error, avoid double reversing process to one transaction.
                    int hasReversingError = dataLiq.Where(x => x.Expense_ID == i.Expense_ID && noReverse.Contains(x.TL_StatusID)).ToList().Count;
                    if (hasReversingError > 0)
                    {
                        isDisable = true;
                    }
                }
                else if (i.TL_StatusID == GlobalSystemValues.STATUS_REVERSING_ERROR)
                {
                    actionLabel = "RE-SEND";
                    actionID = actionReverseResend;
                }
                else if (i.TL_StatusID == GlobalSystemValues.STATUS_RESENDING_COMPLETE)
                {
                    status = GlobalSystemValues.getStatus(GlobalSystemValues.STATUS_RESENDING_COMPLETE);
                    actionLabel = "REVERSE";
                    actionID = actionReverse;
                }

                //Get number of count of related transactions are in process, in order to disabled the action button.
                int inProcess = dataLiq.Where(x => x.Expense_ID == i.Expense_ID && inProcessStats.Contains(x.TL_StatusID)).ToList().Count;
                if (inProcess > 0)
                {
                    isDisable = true;
                }
                else
                {
                    if (actionID == actionResend)
                    {
                        int[] resendProhibit = { GlobalSystemValues.STATUS_REVERSING_COMPLETE, GlobalSystemValues.STATUS_REVERSING_ERROR };
                        int isReverseProcess = data.Where(x => x.Expense_ID == i.Expense_ID && resendProhibit.Contains(x.TL_StatusID)).ToList().Count;
                        if (isReverseProcess > 0)
                        {
                            isDisable = true;
                        }
                    }
                    else if (actionID == actionReverse)
                    {
                        int[] reverseProhibit = { GlobalSystemValues.STATUS_REVERSING_COMPLETE, GlobalSystemValues.STATUS_REVERSING_ERROR };
                        int isReverseProcess = data.Where(x => x.Expense_ID == i.Expense_ID && reverseProhibit.Contains(x.TL_StatusID)).ToList().Count;
                        if (isReverseProcess > 0)
                        {
                            if (i.TL_StatusID == GlobalSystemValues.STATUS_APPROVED)
                            {
                                isDisable = true;
                            }
                        }
                    }
                }

                //Get number of count of related transactions that are all ERROR only, in order to show the REJECT button.
                int notErrorCount = dataLiq.Where(x => x.Expense_ID == i.Expense_ID && x.TL_StatusID != GlobalSystemValues.STATUS_ERROR).ToList().Count;
                if (notErrorCount == 0) canReject = true;

                vm.Add(new TransFailedTableDataViewModel
                {
                    TF_VALUE_DATE = ConvGbDateToDateTime(gohist.GOExpHist_ValueDate).ToString("MM/dd/yyyy"),
                    TF_TRANS_NO = i.TL_TransID.ToString().PadLeft(5, '0'),
                    TF_VOUCHER_NO = getVoucherNo(GlobalSystemValues.TYPE_LIQ, ConvGbDateToDateTime(gohist.GOExpHist_ValueDate), i.Expense_Number, i.TL_Liquidation),
                    TF_REMARKS = gohist.GOExpHist_Remarks,
                    TF_DEBIT_ACCOUNTS = "",
                    TF_CREDIT_ACCONTS = "",
                    TF_STATUS = (!String.IsNullOrEmpty(status)) ? status : GlobalSystemValues.getStatus(i.TL_StatusID),
                    TF_ACTION_LABEL = actionLabel,
                    TF_ACTION_ID = actionID,
                    TF_ACTION_IS_DISABLED = isDisable,
                    TF_CAN_REJECT = canReject,
                    TF_STATUS_ID = i.TL_StatusID,
                    TF_GBASE_MESSAGE = i.TL_GBaseMessage,
                    TF_TRANS_LIST_ID = i.TL_ID,
                    TF_TRANS_IS_LIQ = i.TL_Liquidation,
                    TF_TRANS_ENTRY_ID = i.Expense_ID
                });
            }

            return vm.OrderBy(x => x.TF_VOUCHER_NO).ToList();
        }

        //Re-send all ERROR transactions from ExpenseTransList table.
        public bool ResendToGOExpress(int entryID, bool IsLiq, int userID)
        {
            var list = new[] {
                new { transList = new ExpenseTransList(), goExp = new TblCm10() }
            }.ToList();
            list.Clear();
            //Get all ERROR transactions
            var errTrans = _context.ExpenseTransLists.Where(x =>
                                    x.TL_ExpenseID == entryID &&
                                    x.TL_Liquidation == IsLiq &&
                                    x.TL_StatusID == GlobalSystemValues.STATUS_ERROR).ToList();
            foreach (var i in errTrans)
            {
                var goexphist = _context.GOExpressHist.Where(x => x.GOExpHist_Id == i.TL_GoExpHist_ID).FirstOrDefault();
                TblCm10 goExpData = new TblCm10();

                goExpData = InsertToGOExpress(i, goexphist, userID);
                list.Add(new { transList = i, goExp = goExpData });
            }
            _GOContext.SaveChanges();

            foreach (var i in list)
            {
                i.transList.TL_GoExpress_ID = int.Parse(i.goExp.Id.ToString());
                i.transList.TL_StatusID = GlobalSystemValues.STATUS_RESENDING;
                i.transList.TL_GBaseMessage = "";
                _context.Entry(i.transList).State = EntityState.Modified;
            }

            _context.SaveChanges();
            return true;
        }

        //Reverse all POSTED/RE-SEND COMPLETED transactions from ExpenseTransList table.
        public bool ReverseToGOExpress(int entryID, bool IsLiq, int userID)
        {
            var list = new[] {
                new { expEntryID = 0, expDtl = 0, expType = 0, goExp = new TblCm10(), goExpHist = new GOExpressHistModel()}
            }.ToList();
            list.Clear();

            var expEntry = _context.ExpenseEntry.Where(x => x.Expense_ID == entryID).FirstOrDefault();

            //Get all POSTED/RE-SEND transactions
            var postResendTrans = _context.ExpenseTransLists.Where(x =>
                                    x.TL_ExpenseID == entryID &&
                                    x.TL_Liquidation == IsLiq &&
                                    (x.TL_StatusID == GlobalSystemValues.STATUS_APPROVED ||
                                     x.TL_StatusID == GlobalSystemValues.STATUS_RESENDING_COMPLETE)).ToList();

            foreach (var i in postResendTrans)
            {
                var goexphist = _context.GOExpressHist.Where(x => x.GOExpHist_Id == i.TL_GoExpHist_ID).FirstOrDefault();
                TblCm10 goExpDataNew = new TblCm10();
                GOExpressHistModel goExpHistNew = new GOExpressHistModel();

                goExpDataNew = ReverseThenInsertToGOExpress(i, goexphist, userID);
                goExpHistNew = ConvertTblCm10ToGOExHist(goExpDataNew, entryID, goexphist.ExpenseDetailID);

                list.Add(new { expEntryID = entryID, expDtl = goexphist.ExpenseDetailID, expType = expEntry.Expense_Type, goExp = goExpDataNew, goExpHist = goExpHistNew });
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
                    TL_Liquidation = IsLiq,
                    TL_StatusID = GlobalSystemValues.STATUS_REVERSING
                };
                transactions.Add(tran);

                if (IsLiq)
                {
                    var liqDtlID = _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == item.expEntryID).FirstOrDefault().Liq_DtlID;
                    //var liqIntID = _context.
                    _context.ReversalEntry.Add(new ReversalEntryModel
                    {
                        Reversal_ExpenseEntryID = item.expEntryID,
                        Reversal_ExpenseDtlID = item.expDtl,
                        Reversal_ExpenseType = item.expType,
                        Reversal_LiqDtlID = liqDtlID,
                        Reversal_LiqInterEntityID = 1,
                        Reversal_GOExpressID = int.Parse(item.goExp.Id.ToString()),
                        Reversal_GOExpressHistID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                        Reversal_ReversedDate = DateTime.Now,
                        Reversal_ReversedUserID = userID
                    });
                }
                else
                {
                    _context.ReversalEntry.Add(new ReversalEntryModel
                    {
                        Reversal_ExpenseEntryID = item.expEntryID,
                        Reversal_ExpenseDtlID = item.expDtl,
                        Reversal_ExpenseType = item.expType,
                        Reversal_NonCashDtlID = (expEntry.Expense_Type == GlobalSystemValues.TYPE_NC) ? 1 : 0,
                        Reversal_GOExpressID = int.Parse(item.goExp.Id.ToString()),
                        Reversal_GOExpressHistID = int.Parse(item.goExpHist.GOExpHist_Id.ToString()),
                        Reversal_ReversedDate = DateTime.Now,
                        Reversal_ReversedUserID = userID
                    });
                }

            }
            _context.ExpenseTransLists.AddRange(transactions);
            _context.SaveChanges();

            return true;
        }

        //Re-send all REVERSING ERROR transcations from ExpenseTransList table. BACK UP BECAUSE OF MISUDERSTANDING PART
        //public bool ReverseToGOExpress(int entryID, bool IsLiq, int userID)
        //{
        //    var list = new[] {
        //        new { transList = new ExpenseTransList(), goExp = new TblCm10() }
        //    }.ToList();
        //    list.Clear();

        //    var expEntry = _context.ExpenseEntry.Where(x => x.Expense_ID == entryID).FirstOrDefault();

        //    //Get all POSTED/RE-SEND transactions
        //    var postResendTrans = _context.ExpenseTransLists.Where(x =>
        //                            x.TL_ExpenseID == entryID &&
        //                            x.TL_Liquidation == IsLiq &&
        //                            (x.TL_StatusID == GlobalSystemValues.STATUS_APPROVED ||
        //                             x.TL_StatusID == GlobalSystemValues.STATUS_RESENDING_COMPLETE)).ToList();

        //    foreach (var i in postResendTrans)
        //    {
        //        var goexphist = _context.GOExpressHist.Where(x => x.GOExpHist_Id == i.TL_GoExpHist_ID).FirstOrDefault();
        //        TblCm10 goExpDataNew = new TblCm10();

        //        goExpDataNew = ReverseThenInsertToGOExpress(i, goexphist, userID);
        //        list.Add(new { transList = i, goExp = goExpDataNew });
        //    }

        //    _GOContext.SaveChanges();
        //    _context.SaveChanges();

        //    foreach (var i in list)
        //    {
        //        i.transList.TL_GoExpress_ID = int.Parse(i.goExp.Id.ToString());
        //        i.transList.TL_StatusID = GlobalSystemValues.STATUS_REVERSING;
        //        i.transList.TL_GBaseMessage = "";
        //        _context.Entry(i.transList).State = EntityState.Modified;
        //    }
        //    _context.SaveChanges();

        //    return true;
        //}

        //Back up. Resend Reversing error function
        public bool ResendReversingErrorToGOExpress(int entryID, bool IsLiq, int userID)
        {
            var list = new[] {
                new { transList = new ExpenseTransList(), goExp = new TblCm10() }
            }.ToList();
            list.Clear();
            //Get all REVERSING ERROR transactions
            var revErrTrans = _context.ExpenseTransLists.Where(x =>
                                    x.TL_ExpenseID == entryID &&
                                    x.TL_Liquidation == IsLiq &&
                                    x.TL_StatusID == GlobalSystemValues.STATUS_REVERSING_ERROR).ToList();
            foreach (var i in revErrTrans)
            {
                var goexphist = _context.GOExpressHist.Where(x => x.GOExpHist_Id == i.TL_GoExpHist_ID).FirstOrDefault();
                TblCm10 goExpData = new TblCm10();

                goExpData = InsertToGOExpress(i, goexphist, userID);
                list.Add(new { transList = i, goExp = goExpData });
            }
            _GOContext.SaveChanges();

            foreach (var i in list)
            {
                i.transList.TL_GoExpress_ID = int.Parse(i.goExp.Id.ToString());
                i.transList.TL_StatusID = GlobalSystemValues.STATUS_REVERSING;
                i.transList.TL_GBaseMessage = "";
                _context.Entry(i.transList).State = EntityState.Modified;
            }

            _context.SaveChanges();

            return true;
        }

        //Update status of all resending transactions. BACK UP BECAUSE OF MISUDERSTANDING PART
        //public bool ResendReversingErrorToGOExpress(int entryID, bool IsLiq, int userID)
        //{
        //    var list = new[] {
        //        new { transList = new ExpenseTransList(), goExp = new TblCm10() }
        //    }.ToList();
        //    list.Clear();
        //    //Get all REVERSING ERROR transactions
        //    var revErrTrans = _context.ExpenseTransLists.Where(x =>
        //                            x.TL_ExpenseID == entryID &&
        //                            x.TL_Liquidation == IsLiq &&
        //                            x.TL_StatusID == GlobalSystemValues.STATUS_REVERSING_ERROR).ToList();
        //    foreach (var i in revErrTrans)
        //    {
        //        var goexphist = _context.GOExpressHist.Where(x => x.GOExpHist_Id == i.TL_GoExpHist_ID).FirstOrDefault();
        //        TblCm10 goExpData = new TblCm10();

        //        goExpData = ReverseThenInsertToGOExpress(i, goexphist, userID);
        //        list.Add(new { transList = i, goExp = goExpData });
        //    }
        //    _GOContext.SaveChanges();

        //    foreach (var i in list)
        //    {
        //        i.transList.TL_GoExpress_ID = int.Parse(i.goExp.Id.ToString());
        //        i.transList.TL_StatusID = GlobalSystemValues.STATUS_REVERSING;
        //        i.transList.TL_GBaseMessage = "";
        //        _context.Entry(i.transList).State = EntityState.Modified;
        //    }

        //    _context.SaveChanges();

        //    return true;
        //}

        //Reject Entry because of G-Base transaction all error/s
        public bool RejectExpenseEntry(int entryID, bool IsLiq, int userID)
        {
            if (IsLiq)
            {
                var liqEntry = _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).FirstOrDefault();
                var transList = _context.ExpenseTransLists.Where(x => x.TL_ExpenseID == entryID && x.TL_Liquidation == IsLiq).ToList();
                var goExpHist = _context.GOExpressHist.Where(x => transList.Select(y => y.TL_GoExpHist_ID).Contains(x.GOExpHist_Id)).ToList();

                _context.GOExpressHist.RemoveRange(goExpHist);
                _context.ExpenseTransLists.RemoveRange(transList);
                liqEntry.Liq_LastUpdated_Date = DateTime.Now;
                liqEntry.Liq_Status = GlobalSystemValues.STATUS_REJECTED;
                liqEntry.Liq_Approver = userID;
                _context.Entry(liqEntry).State = EntityState.Modified;
                _context.SaveChanges();
            }
            else
            {
                var expEntry = _context.ExpenseEntry.Where(x => x.Expense_ID == entryID).FirstOrDefault();
                var transList = _context.ExpenseTransLists.Where(x => x.TL_ExpenseID == entryID && x.TL_Liquidation == IsLiq).ToList();
                var goExpHist = _context.GOExpressHist.Where(x => transList.Select(y => y.TL_GoExpHist_ID).Contains(x.GOExpHist_Id)).ToList();

                _context.GOExpressHist.RemoveRange(goExpHist);
                _context.ExpenseTransLists.RemoveRange(transList);
                expEntry.Expense_Last_Updated = DateTime.Now;
                expEntry.Expense_Status = GlobalSystemValues.STATUS_REJECTED;
                expEntry.Expense_Approver = userID;
                _context.Entry(expEntry).State = EntityState.Modified;
                _context.SaveChanges();
            }

            return true;
        }

        public void UpdateResendingTransactions()
        {
            var trans = _context.ExpenseTransLists.Where(x => x.TL_StatusID == GlobalSystemValues.STATUS_RESENDING).ToList();
            var complete = new[] {
                new { expID = 0, IsLiq = false }
            }.ToList();
            complete.Clear();

            if (trans.Count > 0)
            {
                foreach (var i in trans)
                {
                    var goexpress = _GOContext.TblCm10.Where(x => x.Id == i.TL_GoExpress_ID).FirstOrDefault();
                    if (goexpress.Recstatus == "ERROR")
                    {
                        UpdateTransRecord(i.TL_ID, GlobalSystemValues.STATUS_ERROR, "", goexpress.Xmlmsg);
                    }
                    else if (goexpress.Recstatus == "COMPLETED")
                    {
                        UpdateTransRecord(i.TL_ID, GlobalSystemValues.STATUS_RESENDING_COMPLETE, goexpress.Transno, goexpress.Xmlmsg);
                        //UpdateTransRecord(i.TL_ID, GlobalSystemValues.STATUS_RESENDING_COMPLETE, "", "");
                        if (!complete.Select(x => x.expID).Contains(i.TL_ExpenseID))
                        {
                            complete.Add(new { expID = i.TL_ExpenseID, IsLiq = i.TL_Liquidation });
                        }
                    }
                }

                _context.SaveChanges();

                //var compDistinct = complete.Distinct().ToList();
                foreach (var i in complete)
                {
                    AddPrintStatAndChangeExpStat(i.expID, i.IsLiq);
                    //UpdateExpStatusToApproved(i.expID, i.IsLiq);
                }
                _context.SaveChanges();
            }
        }

        //Update transaction record
        public void UpdateTransRecord(int transID, int status, string transNo, string gbaseMsg)
        {
            var trans = _context.ExpenseTransLists.Where(x => x.TL_ID == transID).FirstOrDefault();

            trans.TL_StatusID = status;
            trans.TL_GBaseMessage = gbaseMsg;
            trans.TL_TransID = (!String.IsNullOrEmpty(transNo)) ? int.Parse(transNo) : 0;
            _context.Entry(trans).State = EntityState.Modified;
        }

        //Update Expense status if all transactions are completed.
        public void AddPrintStatAndChangeExpStat(int entryID, bool IsLiq)
        {
            int[] validStat = { GlobalSystemValues.STATUS_APPROVED, GlobalSystemValues.STATUS_RESENDING_COMPLETE };
            bool flag = true;

            var trans = _context.ExpenseTransLists.Where(x => x.TL_ExpenseID == entryID && x.TL_Liquidation == IsLiq).ToList();

            foreach (var i in trans)
            {
                if (!validStat.Contains(i.TL_StatusID))
                {
                    flag = false;
                }
            }

            if (flag)
            {
                bool isAllTrue = InsertRecToPrintStatusTbl(entryID, IsLiq);
                ChangeExpStatToPrintingOrClosing(entryID, IsLiq, isAllTrue);

                //Update Petty Cash Balance
                var petty = _context.PettyCash.LastOrDefault();
                var pc2 = int.Parse(xelemAcc.Element("C_PC2").Value);
                var pettyCashAcc = GetAccountByMasterID(int.Parse(xelemAcc.Element("C_PC2").Value));

                if (pettyCashAcc != null)
                {
                    decimal cashIn = 0.0M;
                    decimal cashOut = 0.0M;

                    foreach (var i in trans)
                    {
                        (string, decimal) result = GetePettyCashInOut(i, pettyCashAcc);
                        if (result.Item1 == "D")
                        {
                            cashIn += result.Item2;
                        }
                        else if (result.Item1 == "C")
                        {
                            cashOut += result.Item2;
                        }
                    }

                    petty.PC_Recieved = petty.PC_Recieved + cashIn;
                    petty.PC_Disbursed = petty.PC_Disbursed + cashOut;
                    petty.PC_EndBal = petty.PC_StartBal + (petty.PC_Recieved - petty.PC_Disbursed);
                    _context.Entry(petty).State = EntityState.Modified;
                }
                
            }
        }

        private (string, decimal) GetePettyCashInOut(ExpenseTransList trans, DMAccountModel pettyCashAcc)
        {
            var gohist = _context.GOExpressHist.Where(x => x.GOExpHist_Id == trans.TL_GoExpHist_ID).FirstOrDefault();
            if (gohist == null) return ("", 0M);

            if (!String.IsNullOrEmpty(gohist.GOExpHist_Entry11Type))
            {
                string gohistAccount = gohist.GOExpHist_Entry11ActType + "-" + gohist.GOExpHist_Branchno + "-" + gohist.GOExpHist_Entry11ActNo;
                int currMasterID = GetCurrency(gohist.GOExpHist_Entry11Ccy).Curr_MasterID;

                if (gohistAccount == pettyCashAcc.Account_No && gohist.GOExpHist_Entry11Actcde == pettyCashAcc.Account_Code &&
                    currMasterID == pettyCashAcc.Account_Currency_MasterID)
                {
                    return (gohist.GOExpHist_Entry11Type, decimal.Parse(gohist.GOExpHist_Entry11Amt));
                }
            }
            if (!String.IsNullOrEmpty(gohist.GOExpHist_Entry12Type))
            {
                string gohistAccount = gohist.GOExpHist_Entry12ActType + "-" + gohist.GOExpHist_Branchno + "-" + gohist.GOExpHist_Entry12ActNo;
                int currMasterID = GetCurrency(gohist.GOExpHist_Entry12Ccy).Curr_MasterID;

                if (gohistAccount == pettyCashAcc.Account_No && gohist.GOExpHist_Entry12Actcde == pettyCashAcc.Account_Code &&
                    currMasterID == pettyCashAcc.Account_Currency_MasterID)
                {
                    return (gohist.GOExpHist_Entry12Type, decimal.Parse(gohist.GOExpHist_Entry12Amt));
                }
            }
            if (!String.IsNullOrEmpty(gohist.GOExpHist_Entry21Type))
            {
                string gohistAccount = gohist.GOExpHist_Entry21ActType + "-" + gohist.GOExpHist_Branchno + "-" + gohist.GOExpHist_Entry21ActNo;
                int currMasterID = GetCurrency(gohist.GOExpHist_Entry21Ccy).Curr_MasterID;

                if (gohistAccount == pettyCashAcc.Account_No && gohist.GOExpHist_Entry21Actcde == pettyCashAcc.Account_Code &&
                    currMasterID == pettyCashAcc.Account_Currency_MasterID)
                {
                    return (gohist.GOExpHist_Entry21Type, decimal.Parse(gohist.GOExpHist_Entry21Amt));
                }
            }
            if (!String.IsNullOrEmpty(gohist.GOExpHist_Entry22Type))
            {
                string gohistAccount = gohist.GOExpHist_Entry22ActType + "-" + gohist.GOExpHist_Branchno + "-" + gohist.GOExpHist_Entry22ActNo;
                int currMasterID = GetCurrency(gohist.GOExpHist_Entry22Ccy).Curr_MasterID;

                if (gohistAccount == pettyCashAcc.Account_No && gohist.GOExpHist_Entry22Actcde == pettyCashAcc.Account_Code &&
                    currMasterID == pettyCashAcc.Account_Currency_MasterID)
                {
                    return (gohist.GOExpHist_Entry22Type, decimal.Parse(gohist.GOExpHist_Entry22Amt));
                }
            }
            if (!String.IsNullOrEmpty(gohist.GOExpHist_Entry31Type))
            {
                string gohistAccount = gohist.GOExpHist_Entry31ActType + "-" + gohist.GOExpHist_Branchno + "-" + gohist.GOExpHist_Entry31ActNo;
                int currMasterID = GetCurrency(gohist.GOExpHist_Entry31Ccy).Curr_MasterID;

                if (gohistAccount == pettyCashAcc.Account_No && gohist.GOExpHist_Entry31Actcde == pettyCashAcc.Account_Code &&
                    currMasterID == pettyCashAcc.Account_Currency_MasterID)
                {
                    return (gohist.GOExpHist_Entry31Type, decimal.Parse(gohist.GOExpHist_Entry31Amt));
                }
            }
            if (!String.IsNullOrEmpty(gohist.GOExpHist_Entry32Type))
            {
                string gohistAccount = gohist.GOExpHist_Entry32ActType + "-" + gohist.GOExpHist_Branchno + "-" + gohist.GOExpHist_Entry32ActNo;
                int currMasterID = GetCurrency(gohist.GOExpHist_Entry32Ccy).Curr_MasterID;

                if (gohistAccount == pettyCashAcc.Account_No && gohist.GOExpHist_Entry32Actcde == pettyCashAcc.Account_Code &&
                    currMasterID == pettyCashAcc.Account_Currency_MasterID)
                {
                    return (gohist.GOExpHist_Entry32Type, decimal.Parse(gohist.GOExpHist_Entry32Amt));
                }
            }
            if (!String.IsNullOrEmpty(gohist.GOExpHist_Entry41Type))
            {
                string gohistAccount = gohist.GOExpHist_Entry41ActType + "-" + gohist.GOExpHist_Branchno + "-" + gohist.GOExpHist_Entry41ActNo;
                int currMasterID = GetCurrency(gohist.GOExpHist_Entry41Ccy).Curr_MasterID;

                if (gohistAccount == pettyCashAcc.Account_No && gohist.GOExpHist_Entry41Actcde == pettyCashAcc.Account_Code &&
                    currMasterID == pettyCashAcc.Account_Currency_MasterID)
                {
                    return (gohist.GOExpHist_Entry41Type, decimal.Parse(gohist.GOExpHist_Entry41Amt));
                }
            }
            if (!String.IsNullOrEmpty(gohist.GOExpHist_Entry42Type))
            {
                string gohistAccount = gohist.GOExpHist_Entry42ActType + "-" + gohist.GOExpHist_Branchno + "-" + gohist.GOExpHist_Entry42ActNo;
                int currMasterID = GetCurrency(gohist.GOExpHist_Entry42Ccy).Curr_MasterID;

                if (gohistAccount == pettyCashAcc.Account_No && gohist.GOExpHist_Entry42Actcde == pettyCashAcc.Account_Code &&
                    currMasterID == pettyCashAcc.Account_Currency_MasterID)
                {
                    return (gohist.GOExpHist_Entry42Type, decimal.Parse(gohist.GOExpHist_Entry42Amt));
                }
            }

            return ("", 0M);
        }

        //Update status of all reversing transactions
        public void UpdateReversingTransactions()
        {
            var trans = _context.ExpenseTransLists.Where(x => x.TL_StatusID == GlobalSystemValues.STATUS_REVERSING).ToList();
            var complete = new[] {
                                new { expID = 0, IsLiq = false }
                            }.ToList();
            complete.Clear();

            if (trans.Count > 0)
            {
                foreach (var i in trans)
                {
                    var goexpress = _GOContext.TblCm10.Where(x => x.Id == i.TL_GoExpress_ID).FirstOrDefault();
                    if (goexpress.Recstatus == "ERROR")
                    {
                        UpdateTransRecord(i.TL_ID, GlobalSystemValues.STATUS_REVERSING_ERROR, "", goexpress.Xmlmsg);
                    }
                    else if (goexpress.Recstatus == "COMPLETED")
                    {
                        UpdateTransRecord(i.TL_ID, GlobalSystemValues.STATUS_REVERSING_COMPLETE, goexpress.Transno, goexpress.Xmlmsg);
                        if (!complete.Select(x => x.expID).Contains(i.TL_ExpenseID))
                        {
                            complete.Add(new { expID = i.TL_ExpenseID, IsLiq = i.TL_Liquidation });
                        }

                    }
                }
                _context.SaveChanges();

                foreach (var i in complete)
                {
                    UpdateStatusToReversedDueToGBaseError(i.expID, i.IsLiq);
                }
                _context.SaveChanges();
            }
        }

        //Updat status of all completed reversing transactions
        private void UpdateStatusToReversedDueToGBaseError(int entryID, bool IsLiq)
        {
            var trans = _context.ExpenseTransLists.Where(x => x.TL_ExpenseID == entryID && x.TL_Liquidation == IsLiq).ToList();
            bool flag = true;

            int[] prohibitStat = { GlobalSystemValues.STATUS_PENDING, GlobalSystemValues.STATUS_RESENDING,
                                    GlobalSystemValues.STATUS_REVERSING, GlobalSystemValues.STATUS_REVERSING_ERROR };

            if (trans.Count > 0)
            {
                foreach (var i in trans)
                {
                    if (prohibitStat.Contains(i.TL_StatusID))
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    UpdateExpenseLiquidationEntryStatus(entryID, IsLiq);

                    //Update Petty Cash Balance
                    var petty = _context.PettyCash.LastOrDefault();
                    var pettyCashAcc = GetAccountByMasterID(int.Parse(xelemAcc.Element("C_PC2").Value));

                    if (pettyCashAcc != null)
                    {
                        decimal cashIn = 0.0M;
                        decimal cashOut = 0.0M;

                        foreach (var i in trans)
                        {
                            if (i.TL_StatusID == GlobalSystemValues.STATUS_ERROR) continue;

                            (string, decimal) result = GetePettyCashInOut(i, pettyCashAcc);
                            if (result.Item1 == "D")
                            {
                                cashIn += result.Item2;
                            }
                            else if (result.Item1 == "C")
                            {
                                cashOut += result.Item2;
                            }
                        }

                        petty.PC_Recieved = petty.PC_Recieved + cashIn;
                        petty.PC_Disbursed = petty.PC_Disbursed + cashOut;
                        petty.PC_EndBal = petty.PC_StartBal + (petty.PC_Recieved - petty.PC_Disbursed);
                        _context.Entry(petty).State = EntityState.Modified;
                    }
                }
            }

        }

        //Insert record to PrintStatus table
        public bool InsertRecToPrintStatusTbl(int entryID, bool IsLiq)
        {
            var expEntry = _context.ExpenseEntry.Where(x => x.Expense_ID == entryID).FirstOrDefault();

            switch (expEntry.Expense_Type)
            {
                case GlobalSystemValues.TYPE_CV:
                    return AddPrintStatusCV(expEntry);
                case GlobalSystemValues.TYPE_DDV:
                    return AddPrintStatusDDV(expEntry);
                case GlobalSystemValues.TYPE_PC:
                    return AddPrintStatusPCV(expEntry);
                case GlobalSystemValues.TYPE_SS:
                    if (IsLiq)
                    {
                        return AddPrintStatusLiquidation(expEntry);
                    }
                    else
                    {
                        return AddPrintStatusSS(expEntry);
                    }
                case GlobalSystemValues.TYPE_NC:
                    return AddPrintStatusNC(expEntry);
            }

            return false;
        }

        private bool AddPrintStatusCV(ExpenseEntryModel expEntry)
        {
            bool IsAllTrue = false;
            var expDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expEntry.Expense_ID).ToList();

            foreach (var i in expDtl)
            {
                bool BIR2307 = true;
                if (i.ExpDtl_Ewt > 0) BIR2307 = false;

                _context.PrintStatus.Add(new PrintStatusModel
                {
                    PS_EntryID = expEntry.Expense_ID,
                    PS_EntryDtlID = i.ExpDtl_ID,
                    PS_Type = GlobalSystemValues.TYPE_CV,
                    PS_LOI = true,
                    PS_BIR2307 = BIR2307,
                    PS_CDD = true,
                    PS_Check = false,
                    PS_Voucher = false
                });
            }
            return IsAllTrue;
        }

        private bool AddPrintStatusDDV(ExpenseEntryModel expEntry)
        {
            bool IsAllTrue = false;
            var expDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expEntry.Expense_ID).ToList();

            foreach (var i in expDtl)
            {
                bool BIR2307 = true;
                if (i.ExpDtl_Ewt > 0) BIR2307 = false;

                _context.PrintStatus.Add(new PrintStatusModel
                {
                    PS_EntryID = expEntry.Expense_ID,
                    PS_EntryDtlID = i.ExpDtl_ID,
                    PS_Type = GlobalSystemValues.TYPE_DDV,
                    PS_LOI = false,
                    PS_BIR2307 = BIR2307,
                    PS_CDD = true,
                    PS_Check = true,
                    PS_Voucher = false
                });
            }
            return IsAllTrue;
        }

        private bool AddPrintStatusPCV(ExpenseEntryModel expEntry)
        {
            bool IsAllTrue = true;
            var expDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expEntry.Expense_ID).ToList();

            foreach (var i in expDtl)
            {
                bool BIR2307 = true;
                if (i.ExpDtl_Ewt > 0)
                {
                    BIR2307 = false;
                    IsAllTrue = false;
                }

                _context.PrintStatus.Add(new PrintStatusModel
                {
                    PS_EntryID = expEntry.Expense_ID,
                    PS_EntryDtlID = i.ExpDtl_ID,
                    PS_Type = GlobalSystemValues.TYPE_PC,
                    PS_LOI = true,
                    PS_BIR2307 = BIR2307,
                    PS_CDD = true,
                    PS_Check = true,
                    PS_Voucher = true
                });
            }

            return IsAllTrue;
        }

        private bool AddPrintStatusSS(ExpenseEntryModel expEntry)
        {
            bool IsAllTrue = true;
            var expDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expEntry.Expense_ID).ToList();

            foreach (var i in expDtl)
            {
                bool BIR2307 = true;
                bool CDD = true;

                if (i.ExpDtl_Ewt > 0)
                {
                    BIR2307 = false;
                    IsAllTrue = false;
                }

                if (GetCurrency(i.ExpDtl_Ccy).Curr_MasterID != int.Parse(xelemLiq.Element("CURRENCY_PHP").Value))
                {
                    CDD = false;
                    IsAllTrue = false;
                }

                _context.PrintStatus.Add(new PrintStatusModel
                {
                    PS_EntryID = expEntry.Expense_ID,
                    PS_EntryDtlID = i.ExpDtl_ID,
                    PS_Type = GlobalSystemValues.TYPE_SS,
                    PS_LOI = true,
                    PS_BIR2307 = BIR2307,
                    PS_CDD = CDD,
                    PS_Check = true,
                    PS_Voucher = true
                });
            }
            return IsAllTrue;
        }

        private bool AddPrintStatusNC(ExpenseEntryModel expEntry)
        {
            bool IsAllTrue = true;
            int EWTMasterID = int.Parse(xelemNC.Element("PSSC").Elements("ACCOUNT").ElementAt(1).Value);
            var NonCash = (from nc in _context.ExpenseEntryNonCash
                           join ncDtl in _context.ExpenseEntryNonCashDetails
                           on nc.ExpNC_ID equals ncDtl.ExpenseEntryNCModel.ExpNC_ID
                           where nc.ExpenseEntryModel.Expense_ID == expEntry.Expense_ID
                           select new
                           {
                               nc.ExpNC_ID,
                               nc.ExpNC_Category_ID,
                               nc.ExpenseEntryModel.Expense_ID,
                               ncDtl.ExpNCDtl_ID
                           }).ToList();

            foreach (var i in NonCash)
            {
                bool BIR2307 = true;
                bool CDD = true;

                if (i.ExpNC_Category_ID == GlobalSystemValues.NC_PCHC ||
                    i.ExpNC_Category_ID == GlobalSystemValues.NC_PSSC)
                {
                    BIR2307 = false;
                    IsAllTrue = false;
                }
                if (i.ExpNC_Category_ID == GlobalSystemValues.NC_PETTY_CASH_REPLENISHMENT ||
                    i.ExpNC_Category_ID == GlobalSystemValues.NC_RETURN_OF_JS_PAYROLL)
                {
                    CDD = false;
                    IsAllTrue = false;
                }
                if (i.ExpNC_Category_ID == GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES)
                {
                    var NonCashDtlAcc = (from ncDtlAcc in _context.ExpenseEntryNonCashDetailAccounts
                                         join acc in _context.DMAccount
                                         on ncDtlAcc.ExpNCDtlAcc_Acc_ID equals acc.Account_ID
                                         where ncDtlAcc.ExpenseEntryNCDtlModel.ExpNCDtl_ID == i.ExpNCDtl_ID
                                         select new
                                         {
                                             ncDtlAcc.ExpNCDtlAcc_Acc_ID,
                                             acc.Account_ID,
                                             acc.Account_MasterID
                                         }).ToList();

                    if (NonCashDtlAcc.Select(x => x.Account_MasterID).Contains(EWTMasterID))
                    {
                        BIR2307 = false;
                        IsAllTrue = false;
                    }
                }

                _context.PrintStatus.Add(new PrintStatusModel
                {
                    PS_EntryID = expEntry.Expense_ID,
                    PS_EntryDtlID = i.ExpNCDtl_ID,
                    PS_Type = GlobalSystemValues.TYPE_NC,
                    PS_LOI = true,
                    PS_BIR2307 = BIR2307,
                    PS_CDD = CDD,
                    PS_Check = true,
                    PS_Voucher = true
                });
            }
            return IsAllTrue;
        }

        private bool AddPrintStatusLiquidation(ExpenseEntryModel expEntry)
        {
            bool IsAllTrue = true;
            var expDtl = _context.ExpenseEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == expEntry.Expense_ID).ToList();

            foreach (var i in expDtl)
            {
                bool CDD = true;
                if (GetCurrency(i.ExpDtl_Ccy).Curr_MasterID != int.Parse(xelemLiq.Element("CURRENCY_PHP").Value))
                {
                    var liqInter = _context.LiquidationInterEntity.Where(x => x.ExpenseEntryDetailModel.ExpDtl_ID == i.ExpDtl_ID).FirstOrDefault().Liq_Amount_1_1;
                    if (liqInter > 0)
                    {
                        CDD = false;
                        IsAllTrue = false;
                    }
                }

                _context.PrintStatus.Add(new PrintStatusModel
                {
                    PS_EntryID = expEntry.Expense_ID,
                    PS_EntryDtlID = i.ExpDtl_ID,
                    PS_Type = GlobalSystemValues.TYPE_LIQ,
                    PS_LOI = true,
                    PS_BIR2307 = true,
                    PS_CDD = CDD,
                    PS_Check = true,
                    PS_Voucher = true
                });
            }
            return IsAllTrue;
        }

        private void ChangeExpStatToPrintingOrClosing(int entryID, bool IsLiq, bool isAllTrue)
        {
            if (IsLiq)
            {
                var liqEntry = _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).FirstOrDefault();

                if (isAllTrue)
                {
                    liqEntry.Liq_Status = GlobalSystemValues.STATUS_FOR_CLOSING;
                    liqEntry.Liq_LastUpdated_Date = DateTime.Now;
                }
                else
                {
                    liqEntry.Liq_Status = GlobalSystemValues.STATUS_FOR_PRINTING;
                    liqEntry.Liq_LastUpdated_Date = DateTime.Now;
                }
                _context.Entry(liqEntry).State = EntityState.Modified;
            }
            else
            {
                var expEntry = _context.ExpenseEntry.Where(x => x.Expense_ID == entryID).FirstOrDefault();

                if (isAllTrue)
                {
                    expEntry.Expense_Status = GlobalSystemValues.STATUS_FOR_CLOSING;
                    expEntry.Expense_Last_Updated = DateTime.Now;
                }
                else
                {
                    expEntry.Expense_Status = GlobalSystemValues.STATUS_FOR_PRINTING;
                    expEntry.Expense_Last_Updated = DateTime.Now;
                }
                _context.Entry(expEntry).State = EntityState.Modified;
            }

        }

        //Update Expense status back to APPROVED if all transactions are completed.
        //public void UpdateExpStatusToApproved(int entryID, bool IsLiq)
        //{
        //    int[] validStat = { GlobalSystemValues.STATUS_APPROVED, GlobalSystemValues.STATUS_RESENDING_COMPLETE };
        //    int flag = 0;

        //    var trans = _context.ExpenseTransLists.Where(x => x.TL_ExpenseID == entryID && x.TL_Liquidation == IsLiq).ToList();

        //    foreach (var i in trans)
        //    {
        //        if (!validStat.Contains(i.TL_StatusID))
        //        {
        //            flag = 1;
        //        }
        //    }

        //    if (flag == 0)
        //    {
        //        foreach(var i in trans)
        //        {
        //            i.TL_TransID = 0;
        //            i.TL_StatusID = GlobalSystemValues.STATUS_PENDING;
        //            i.TL_GBaseMessage = "";
        //            _context.Entry(i).State = EntityState.Modified;
        //        }

        //        if (IsLiq)
        //        {
        //            var liqDetail = _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).FirstOrDefault();
        //            liqDetail.Liq_Status = GlobalSystemValues.STATUS_APPROVED;
        //            liqDetail.Liq_LastUpdated_Date = DateTime.Now;
        //            _context.Entry(liqDetail).State = EntityState.Modified;
        //        }
        //        else
        //        {
        //            var expEntry = _context.ExpenseEntry.Where(x => x.Expense_ID == entryID).FirstOrDefault();
        //            expEntry.Expense_Status = GlobalSystemValues.STATUS_APPROVED;
        //            expEntry.Expense_Last_Updated = DateTime.Now;
        //            _context.Entry(expEntry).State = EntityState.Modified;
        //        }
        //    }
        //}

        private TblCm10 InsertToGOExpress(ExpenseTransList errTrans, GOExpressHistModel goexphist, int userID)
        {
            var user = _context.User.FirstOrDefault(x => x.User_ID == userID);

            TblCm10 goexpress = new TblCm10();

            goexpress.SystemName = goexphist.GOExpHist_SystemName;
            goexpress.Groupcode = goexphist.GOExpHist_Groupcode;
            goexpress.Branchno = goexphist.GOExpHist_Branchno;
            goexpress.OpeKind = goexphist.GOExpHist_OpeKind;
            goexpress.AutoApproved = goexphist.GOExpHist_AutoApproved;
            goexpress.WarningOverride = goexphist.GOExpHist_WarningOverride;
            goexpress.CcyFormat = goexphist.GOExpHist_CcyFormat;
            goexpress.OpeBranch = goexphist.GOExpHist_OpeBranch;
            goexpress.ValueDate = goexphist.GOExpHist_ValueDate;
            goexpress.ReferenceType = goexphist.GOExpHist_ReferenceType;
            goexpress.ReferenceNo = goexphist.GOExpHist_ReferenceNo;
            goexpress.Comment = goexphist.GOExpHist_Comment;
            goexpress.Section = goexphist.GOExpHist_Section;
            goexpress.Remarks = goexphist.GOExpHist_Remarks;
            goexpress.Memo = goexphist.GOExpHist_Memo;
            goexpress.SchemeNo = goexphist.GOExpHist_SchemeNo;

            goexpress.Entry11Type = goexphist.GOExpHist_Entry11Type;
            goexpress.Entry11Ccy = goexphist.GOExpHist_Entry11Ccy;
            goexpress.Entry11Amt = goexphist.GOExpHist_Entry11Amt;
            goexpress.Entry11Cust = goexphist.GOExpHist_Entry11Cust;
            goexpress.Entry11Actcde = goexphist.GOExpHist_Entry11Actcde;
            goexpress.Entry11ActType = goexphist.GOExpHist_Entry11ActType;
            goexpress.Entry11ActNo = goexphist.GOExpHist_Entry11ActNo;
            goexpress.Entry11ExchRate = goexphist.GOExpHist_Entry11ExchRate;
            goexpress.Entry11ExchCcy = goexphist.GOExpHist_Entry11ExchCcy;
            goexpress.Entry11Fund = goexphist.GOExpHist_Entry11Fund;
            goexpress.Entry11Available = goexphist.GOExpHist_Entry11Available;
            goexpress.Entry11Details = goexphist.GOExpHist_Entry11Details;
            goexpress.Entry11Entity = goexphist.GOExpHist_Entry11Entity;
            goexpress.Entry11Division = goexphist.GOExpHist_Entry11Division;
            goexpress.Entry11InterAmt = goexphist.GOExpHist_Entry11InterAmt;
            goexpress.Entry11InterRate = goexphist.GOExpHist_Entry11InterRate;
            goexpress.Entry12Type = goexphist.GOExpHist_Entry12Type;
            goexpress.Entry12Ccy = goexphist.GOExpHist_Entry12Ccy;
            goexpress.Entry12Amt = goexphist.GOExpHist_Entry12Amt;
            goexpress.Entry12Cust = goexphist.GOExpHist_Entry12Cust;
            goexpress.Entry12Actcde = goexphist.GOExpHist_Entry12Actcde;
            goexpress.Entry12ActType = goexphist.GOExpHist_Entry12ActType;
            goexpress.Entry12ActNo = goexphist.GOExpHist_Entry12ActNo;
            goexpress.Entry12ExchRate = goexphist.GOExpHist_Entry12ExchRate;
            goexpress.Entry12ExchCcy = goexphist.GOExpHist_Entry12ExchCcy;
            goexpress.Entry12Fund = goexphist.GOExpHist_Entry12Fund;
            goexpress.Entry12Available = goexphist.GOExpHist_Entry12Available;
            goexpress.Entry12Details = goexphist.GOExpHist_Entry12Details;
            goexpress.Entry12Entity = goexphist.GOExpHist_Entry12Entity;
            goexpress.Entry12Division = goexphist.GOExpHist_Entry12Division;
            goexpress.Entry12InterAmt = goexphist.GOExpHist_Entry12InterAmt;
            goexpress.Entry12InterRate = goexphist.GOExpHist_Entry12InterRate;
            goexpress.Entry21Type = goexphist.GOExpHist_Entry21Type;
            goexpress.Entry21Ccy = goexphist.GOExpHist_Entry21Ccy;
            goexpress.Entry21Amt = goexphist.GOExpHist_Entry21Amt;
            goexpress.Entry21Cust = goexphist.GOExpHist_Entry21Cust;
            goexpress.Entry21Actcde = goexphist.GOExpHist_Entry21Actcde;
            goexpress.Entry21ActType = goexphist.GOExpHist_Entry21ActType;
            goexpress.Entry21ActNo = goexphist.GOExpHist_Entry21ActNo;
            goexpress.Entry21ExchRate = goexphist.GOExpHist_Entry21ExchRate;
            goexpress.Entry21ExchCcy = goexphist.GOExpHist_Entry21ExchCcy;
            goexpress.Entry21Fund = goexphist.GOExpHist_Entry21Fund;
            goexpress.Entry21Available = goexphist.GOExpHist_Entry21Available;
            goexpress.Entry21Details = goexphist.GOExpHist_Entry21Details;
            goexpress.Entry21Entity = goexphist.GOExpHist_Entry21Entity;
            goexpress.Entry21Division = goexphist.GOExpHist_Entry21Division;
            goexpress.Entry21InterAmt = goexphist.GOExpHist_Entry21InterAmt;
            goexpress.Entry21InterRate = goexphist.GOExpHist_Entry21InterRate;
            goexpress.Entry22Type = goexphist.GOExpHist_Entry22Type;
            goexpress.Entry22Ccy = goexphist.GOExpHist_Entry22Ccy;
            goexpress.Entry22Amt = goexphist.GOExpHist_Entry22Amt;
            goexpress.Entry22Cust = goexphist.GOExpHist_Entry22Cust;
            goexpress.Entry22Actcde = goexphist.GOExpHist_Entry22Actcde;
            goexpress.Entry22ActType = goexphist.GOExpHist_Entry22ActType;
            goexpress.Entry22ActNo = goexphist.GOExpHist_Entry22ActNo;
            goexpress.Entry22ExchRate = goexphist.GOExpHist_Entry22ExchRate;
            goexpress.Entry22ExchCcy = goexphist.GOExpHist_Entry22ExchCcy;
            goexpress.Entry22Fund = goexphist.GOExpHist_Entry22Fund;
            goexpress.Entry22Available = goexphist.GOExpHist_Entry22Available;
            goexpress.Entry22Details = goexphist.GOExpHist_Entry22Details;
            goexpress.Entry22Entity = goexphist.GOExpHist_Entry22Entity;
            goexpress.Entry22Division = goexphist.GOExpHist_Entry22Division;
            goexpress.Entry22InterAmt = goexphist.GOExpHist_Entry22InterAmt;
            goexpress.Entry22InterRate = goexphist.GOExpHist_Entry22InterRate;
            goexpress.Entry31Type = goexphist.GOExpHist_Entry31Type;
            goexpress.Entry31Ccy = goexphist.GOExpHist_Entry31Ccy;
            goexpress.Entry31Amt = goexphist.GOExpHist_Entry31Amt;
            goexpress.Entry31Cust = goexphist.GOExpHist_Entry31Cust;
            goexpress.Entry31Actcde = goexphist.GOExpHist_Entry31Actcde;
            goexpress.Entry31ActType = goexphist.GOExpHist_Entry31ActType;
            goexpress.Entry31ActNo = goexphist.GOExpHist_Entry31ActNo;
            goexpress.Entry31ExchRate = goexphist.GOExpHist_Entry31ExchRate;
            goexpress.Entry31ExchCcy = goexphist.GOExpHist_Entry31ExchCcy;
            goexpress.Entry31Fund = goexphist.GOExpHist_Entry31Fund;
            goexpress.Entry31Available = goexphist.GOExpHist_Entry31Available;
            goexpress.Entry31Details = goexphist.GOExpHist_Entry31Details;
            goexpress.Entry31Entity = goexphist.GOExpHist_Entry31Entity;
            goexpress.Entry31Division = goexphist.GOExpHist_Entry31Division;
            goexpress.Entry31InterAmt = goexphist.GOExpHist_Entry31InterAmt;
            goexpress.Entry31InterRate = goexphist.GOExpHist_Entry31InterRate;
            goexpress.Entry32Type = goexphist.GOExpHist_Entry32Type;
            goexpress.Entry32Ccy = goexphist.GOExpHist_Entry32Ccy;
            goexpress.Entry32Amt = goexphist.GOExpHist_Entry32Amt;
            goexpress.Entry32Cust = goexphist.GOExpHist_Entry32Cust;
            goexpress.Entry32Actcde = goexphist.GOExpHist_Entry32Actcde;
            goexpress.Entry32ActType = goexphist.GOExpHist_Entry32ActType;
            goexpress.Entry32ActNo = goexphist.GOExpHist_Entry32ActNo;
            goexpress.Entry32ExchRate = goexphist.GOExpHist_Entry32ExchRate;
            goexpress.Entry32ExchCcy = goexphist.GOExpHist_Entry32ExchCcy;
            goexpress.Entry32Fund = goexphist.GOExpHist_Entry32Fund;
            goexpress.Entry32Available = goexphist.GOExpHist_Entry32Available;
            goexpress.Entry32Details = goexphist.GOExpHist_Entry32Details;
            goexpress.Entry32Entity = goexphist.GOExpHist_Entry32Entity;
            goexpress.Entry32Division = goexphist.GOExpHist_Entry32Division;
            goexpress.Entry32InterAmt = goexphist.GOExpHist_Entry32InterAmt;
            goexpress.Entry32InterRate = goexphist.GOExpHist_Entry32InterRate;
            goexpress.Entry41Type = goexphist.GOExpHist_Entry41Type;
            goexpress.Entry41Ccy = goexphist.GOExpHist_Entry41Ccy;
            goexpress.Entry41Amt = goexphist.GOExpHist_Entry41Amt;
            goexpress.Entry41Cust = goexphist.GOExpHist_Entry41Cust;
            goexpress.Entry41Actcde = goexphist.GOExpHist_Entry41Actcde;
            goexpress.Entry41ActType = goexphist.GOExpHist_Entry41ActType;
            goexpress.Entry41ActNo = goexphist.GOExpHist_Entry41ActNo;
            goexpress.Entry41ExchRate = goexphist.GOExpHist_Entry41ExchRate;
            goexpress.Entry41ExchCcy = goexphist.GOExpHist_Entry41ExchCcy;
            goexpress.Entry41Fund = goexphist.GOExpHist_Entry41Fund;
            goexpress.Entry41Available = goexphist.GOExpHist_Entry41Available;
            goexpress.Entry41Details = goexphist.GOExpHist_Entry41Details;
            goexpress.Entry41Entity = goexphist.GOExpHist_Entry41Entity;
            goexpress.Entry41Division = goexphist.GOExpHist_Entry41Division;
            goexpress.Entry41InterAmt = goexphist.GOExpHist_Entry41InterAmt;
            goexpress.Entry41InterRate = goexphist.GOExpHist_Entry41InterRate;
            goexpress.Entry42Type = goexphist.GOExpHist_Entry42Type;
            goexpress.Entry42Ccy = goexphist.GOExpHist_Entry42Ccy;
            goexpress.Entry42Amt = goexphist.GOExpHist_Entry42Amt;
            goexpress.Entry42Cust = goexphist.GOExpHist_Entry42Cust;
            goexpress.Entry42Actcde = goexphist.GOExpHist_Entry42Actcde;
            goexpress.Entry42ActType = goexphist.GOExpHist_Entry42ActType;
            goexpress.Entry42ActNo = goexphist.GOExpHist_Entry42ActNo;
            goexpress.Entry42ExchRate = goexphist.GOExpHist_Entry42ExchRate;
            goexpress.Entry42ExchCcy = goexphist.GOExpHist_Entry42ExchCcy;
            goexpress.Entry42Fund = goexphist.GOExpHist_Entry42Fund;
            goexpress.Entry42Available = goexphist.GOExpHist_Entry42Available;
            goexpress.Entry42Details = goexphist.GOExpHist_Entry42Details;
            goexpress.Entry42Entity = goexphist.GOExpHist_Entry42Entity;
            goexpress.Entry42Division = goexphist.GOExpHist_Entry42Division;
            goexpress.Entry42InterAmt = goexphist.GOExpHist_Entry42InterAmt;
            goexpress.Entry42InterRate = goexphist.GOExpHist_Entry42InterRate;

            goexpress.MakerEmpno = user.User_EmpCode;
            goexpress.Empno = user.User_EmpCode.Substring(2);
            goexpress.Datestamp = DateTime.Now; ;
            goexpress.Timesent = DateTime.Now; ;
            goexpress.Timerespond = DateTime.Now;
            goexpress.Recstatus = "READY";

            _GOContext.Add(goexpress);

            return goexpress;
        }

        private TblCm10 ReverseThenInsertToGOExpress(ExpenseTransList i, GOExpressHistModel goexphist, int userID)
        {
            var user = _context.User.FirstOrDefault(x => x.User_ID == userID);
            TblCm10 goexpress = new TblCm10();
            List<GOExpContainer> container = ArrangeToGOExpContainer(goexphist);
            container = container.OrderByDescending(x => x.GOExpCont_Entry_Type).ToList();

            goexpress = ConvGOExpContToTblCM10(container);

            goexpress.SystemName = goexphist.GOExpHist_SystemName;
            goexpress.Groupcode = goexphist.GOExpHist_Groupcode;
            goexpress.Branchno = goexphist.GOExpHist_Branchno;
            goexpress.OpeKind = goexphist.GOExpHist_OpeKind;
            goexpress.AutoApproved = goexphist.GOExpHist_AutoApproved;
            goexpress.WarningOverride = goexphist.GOExpHist_WarningOverride;
            goexpress.CcyFormat = goexphist.GOExpHist_CcyFormat;
            goexpress.OpeBranch = goexphist.GOExpHist_OpeBranch;
            goexpress.ValueDate = goexphist.GOExpHist_ValueDate;
            goexpress.ReferenceType = goexphist.GOExpHist_ReferenceType;
            goexpress.ReferenceNo = goexphist.GOExpHist_ReferenceNo;
            goexpress.Comment = goexphist.GOExpHist_Comment;
            goexpress.Section = goexphist.GOExpHist_Section;
            goexpress.Remarks = goexphist.GOExpHist_Remarks;
            goexpress.Memo = goexphist.GOExpHist_Memo;
            goexpress.SchemeNo = goexphist.GOExpHist_SchemeNo;

            goexpress.MakerEmpno = user.User_EmpCode;
            goexpress.Empno = user.User_EmpCode.Substring(2);
            goexpress.Datestamp = DateTime.Now;
            goexpress.Timesent = DateTime.Now;
            goexpress.Timerespond = DateTime.Now;
            goexpress.Recstatus = "READY";

            _GOContext.Add(goexpress);

            return goexpress;
        }

        public GOExpressHistModel ConvertTblCm10ToGOExHist(TblCm10 tblcm10, int entryID, int entryDtlID)
        {
            var goExpHist = new GOExpressHistModel
            {
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

        private void UpdateExpenseLiquidationEntryStatus(int entryID, bool IsLiq)
        {
            if (IsLiq)
            {
                var liqEntry = _context.LiquidationEntryDetails.Where(x => x.ExpenseEntryModel.Expense_ID == entryID).FirstOrDefault();
                liqEntry.Liq_Status = GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR;
                liqEntry.Liq_LastUpdated_Date = DateTime.Now;
            }
            else
            {
                var expEntry = _context.ExpenseEntry.Where(x => x.Expense_ID == entryID).FirstOrDefault();
                expEntry.Expense_Status = GlobalSystemValues.STATUS_REVERSED_GBASE_ERROR;
                expEntry.Expense_Last_Updated = DateTime.Now;
            }
        }

        private List<GOExpContainer> ArrangeToGOExpContainer(GOExpressHistModel goexphist)
        {
            List<GOExpContainer> container = new List<GOExpContainer>();

            if (!String.IsNullOrEmpty(goexphist.GOExpHist_Entry11Type))
            {
                container.Add(new GOExpContainer
                {
                    GOExpCont_Entry_Type = (goexphist.GOExpHist_Entry11Type == "D") ? "C" : "D",
                    GOExpCont_Entry_Ccy = goexphist.GOExpHist_Entry11Ccy,
                    GOExpCont_Entry_Amt = goexphist.GOExpHist_Entry11Amt,
                    GOExpCont_Entry_Cust = goexphist.GOExpHist_Entry11Cust,
                    GOExpCont_Entry_Actcde = goexphist.GOExpHist_Entry11Actcde,
                    GOExpCont_Entry_ActType = goexphist.GOExpHist_Entry11ActType,
                    GOExpCont_Entry_ActNo = goexphist.GOExpHist_Entry11ActNo,
                    GOExpCont_Entry_ExchRate = goexphist.GOExpHist_Entry11ExchRate,
                    GOExpCont_Entry_ExchCcy = goexphist.GOExpHist_Entry11ExchCcy,
                    GOExpCont_Entry_Fund = goexphist.GOExpHist_Entry11Fund,
                    GOExpCont_Entry_Available = goexphist.GOExpHist_Entry11Available,
                    GOExpCont_Entry_Details = goexphist.GOExpHist_Entry11Details,
                    GOExpCont_Entry_Entity = goexphist.GOExpHist_Entry11Entity,
                    GOExpCont_Entry_Division = goexphist.GOExpHist_Entry11Division,
                    GOExpCont_Entry_InterAmt = goexphist.GOExpHist_Entry11InterAmt,
                    GOExpCont_Entry_InterRate = goexphist.GOExpHist_Entry11InterRate
                });
            }
            if (!String.IsNullOrEmpty(goexphist.GOExpHist_Entry12Type))
            {
                container.Add(new GOExpContainer
                {
                    GOExpCont_Entry_Type = (goexphist.GOExpHist_Entry12Type == "D") ? "C" : "D",
                    GOExpCont_Entry_Ccy = goexphist.GOExpHist_Entry12Ccy,
                    GOExpCont_Entry_Amt = goexphist.GOExpHist_Entry12Amt,
                    GOExpCont_Entry_Cust = goexphist.GOExpHist_Entry12Cust,
                    GOExpCont_Entry_Actcde = goexphist.GOExpHist_Entry12Actcde,
                    GOExpCont_Entry_ActType = goexphist.GOExpHist_Entry12ActType,
                    GOExpCont_Entry_ActNo = goexphist.GOExpHist_Entry12ActNo,
                    GOExpCont_Entry_ExchRate = goexphist.GOExpHist_Entry12ExchRate,
                    GOExpCont_Entry_ExchCcy = goexphist.GOExpHist_Entry12ExchCcy,
                    GOExpCont_Entry_Fund = goexphist.GOExpHist_Entry12Fund,
                    GOExpCont_Entry_Available = goexphist.GOExpHist_Entry12Available,
                    GOExpCont_Entry_Details = goexphist.GOExpHist_Entry12Details,
                    GOExpCont_Entry_Entity = goexphist.GOExpHist_Entry12Entity,
                    GOExpCont_Entry_Division = goexphist.GOExpHist_Entry12Division,
                    GOExpCont_Entry_InterAmt = goexphist.GOExpHist_Entry12InterAmt,
                    GOExpCont_Entry_InterRate = goexphist.GOExpHist_Entry12InterRate
                });
            }
            if (!String.IsNullOrEmpty(goexphist.GOExpHist_Entry21Type))
            {
                container.Add(new GOExpContainer
                {
                    GOExpCont_Entry_Type = (goexphist.GOExpHist_Entry21Type == "D") ? "C" : "D",
                    GOExpCont_Entry_Ccy = goexphist.GOExpHist_Entry21Ccy,
                    GOExpCont_Entry_Amt = goexphist.GOExpHist_Entry21Amt,
                    GOExpCont_Entry_Cust = goexphist.GOExpHist_Entry21Cust,
                    GOExpCont_Entry_Actcde = goexphist.GOExpHist_Entry21Actcde,
                    GOExpCont_Entry_ActType = goexphist.GOExpHist_Entry21ActType,
                    GOExpCont_Entry_ActNo = goexphist.GOExpHist_Entry21ActNo,
                    GOExpCont_Entry_ExchRate = goexphist.GOExpHist_Entry21ExchRate,
                    GOExpCont_Entry_ExchCcy = goexphist.GOExpHist_Entry21ExchCcy,
                    GOExpCont_Entry_Fund = goexphist.GOExpHist_Entry21Fund,
                    GOExpCont_Entry_Available = goexphist.GOExpHist_Entry21Available,
                    GOExpCont_Entry_Details = goexphist.GOExpHist_Entry21Details,
                    GOExpCont_Entry_Entity = goexphist.GOExpHist_Entry21Entity,
                    GOExpCont_Entry_Division = goexphist.GOExpHist_Entry21Division,
                    GOExpCont_Entry_InterAmt = goexphist.GOExpHist_Entry21InterAmt,
                    GOExpCont_Entry_InterRate = goexphist.GOExpHist_Entry21InterRate
                });
            }
            if (!String.IsNullOrEmpty(goexphist.GOExpHist_Entry22Type))
            {
                container.Add(new GOExpContainer
                {
                    GOExpCont_Entry_Type = (goexphist.GOExpHist_Entry22Type == "D") ? "C" : "D",
                    GOExpCont_Entry_Ccy = goexphist.GOExpHist_Entry22Ccy,
                    GOExpCont_Entry_Amt = goexphist.GOExpHist_Entry22Amt,
                    GOExpCont_Entry_Cust = goexphist.GOExpHist_Entry22Cust,
                    GOExpCont_Entry_Actcde = goexphist.GOExpHist_Entry22Actcde,
                    GOExpCont_Entry_ActType = goexphist.GOExpHist_Entry22ActType,
                    GOExpCont_Entry_ActNo = goexphist.GOExpHist_Entry22ActNo,
                    GOExpCont_Entry_ExchRate = goexphist.GOExpHist_Entry22ExchRate,
                    GOExpCont_Entry_ExchCcy = goexphist.GOExpHist_Entry22ExchCcy,
                    GOExpCont_Entry_Fund = goexphist.GOExpHist_Entry22Fund,
                    GOExpCont_Entry_Available = goexphist.GOExpHist_Entry22Available,
                    GOExpCont_Entry_Details = goexphist.GOExpHist_Entry22Details,
                    GOExpCont_Entry_Entity = goexphist.GOExpHist_Entry22Entity,
                    GOExpCont_Entry_Division = goexphist.GOExpHist_Entry22Division,
                    GOExpCont_Entry_InterAmt = goexphist.GOExpHist_Entry22InterAmt,
                    GOExpCont_Entry_InterRate = goexphist.GOExpHist_Entry22InterRate
                });
            }
            if (!String.IsNullOrEmpty(goexphist.GOExpHist_Entry31Type))
            {
                container.Add(new GOExpContainer
                {
                    GOExpCont_Entry_Type = (goexphist.GOExpHist_Entry31Type == "D") ? "C" : "D",
                    GOExpCont_Entry_Ccy = goexphist.GOExpHist_Entry31Ccy,
                    GOExpCont_Entry_Amt = goexphist.GOExpHist_Entry31Amt,
                    GOExpCont_Entry_Cust = goexphist.GOExpHist_Entry31Cust,
                    GOExpCont_Entry_Actcde = goexphist.GOExpHist_Entry31Actcde,
                    GOExpCont_Entry_ActType = goexphist.GOExpHist_Entry31ActType,
                    GOExpCont_Entry_ActNo = goexphist.GOExpHist_Entry31ActNo,
                    GOExpCont_Entry_ExchRate = goexphist.GOExpHist_Entry31ExchRate,
                    GOExpCont_Entry_ExchCcy = goexphist.GOExpHist_Entry31ExchCcy,
                    GOExpCont_Entry_Fund = goexphist.GOExpHist_Entry31Fund,
                    GOExpCont_Entry_Available = goexphist.GOExpHist_Entry31Available,
                    GOExpCont_Entry_Details = goexphist.GOExpHist_Entry31Details,
                    GOExpCont_Entry_Entity = goexphist.GOExpHist_Entry31Entity,
                    GOExpCont_Entry_Division = goexphist.GOExpHist_Entry31Division,
                    GOExpCont_Entry_InterAmt = goexphist.GOExpHist_Entry31InterAmt,
                    GOExpCont_Entry_InterRate = goexphist.GOExpHist_Entry31InterRate
                });
            }
            if (!String.IsNullOrEmpty(goexphist.GOExpHist_Entry32Type))
            {
                container.Add(new GOExpContainer
                {
                    GOExpCont_Entry_Type = (goexphist.GOExpHist_Entry32Type == "D") ? "C" : "D",
                    GOExpCont_Entry_Ccy = goexphist.GOExpHist_Entry32Ccy,
                    GOExpCont_Entry_Amt = goexphist.GOExpHist_Entry32Amt,
                    GOExpCont_Entry_Cust = goexphist.GOExpHist_Entry32Cust,
                    GOExpCont_Entry_Actcde = goexphist.GOExpHist_Entry32Actcde,
                    GOExpCont_Entry_ActType = goexphist.GOExpHist_Entry32ActType,
                    GOExpCont_Entry_ActNo = goexphist.GOExpHist_Entry32ActNo,
                    GOExpCont_Entry_ExchRate = goexphist.GOExpHist_Entry32ExchRate,
                    GOExpCont_Entry_ExchCcy = goexphist.GOExpHist_Entry32ExchCcy,
                    GOExpCont_Entry_Fund = goexphist.GOExpHist_Entry32Fund,
                    GOExpCont_Entry_Available = goexphist.GOExpHist_Entry32Available,
                    GOExpCont_Entry_Details = goexphist.GOExpHist_Entry32Details,
                    GOExpCont_Entry_Entity = goexphist.GOExpHist_Entry32Entity,
                    GOExpCont_Entry_Division = goexphist.GOExpHist_Entry32Division,
                    GOExpCont_Entry_InterAmt = goexphist.GOExpHist_Entry32InterAmt,
                    GOExpCont_Entry_InterRate = goexphist.GOExpHist_Entry32InterRate
                });
            }
            if (!String.IsNullOrEmpty(goexphist.GOExpHist_Entry41Type))
            {
                container.Add(new GOExpContainer
                {
                    GOExpCont_Entry_Type = (goexphist.GOExpHist_Entry41Type == "D") ? "C" : "D",
                    GOExpCont_Entry_Ccy = goexphist.GOExpHist_Entry41Ccy,
                    GOExpCont_Entry_Amt = goexphist.GOExpHist_Entry41Amt,
                    GOExpCont_Entry_Cust = goexphist.GOExpHist_Entry41Cust,
                    GOExpCont_Entry_Actcde = goexphist.GOExpHist_Entry41Actcde,
                    GOExpCont_Entry_ActType = goexphist.GOExpHist_Entry41ActType,
                    GOExpCont_Entry_ActNo = goexphist.GOExpHist_Entry41ActNo,
                    GOExpCont_Entry_ExchRate = goexphist.GOExpHist_Entry41ExchRate,
                    GOExpCont_Entry_ExchCcy = goexphist.GOExpHist_Entry41ExchCcy,
                    GOExpCont_Entry_Fund = goexphist.GOExpHist_Entry41Fund,
                    GOExpCont_Entry_Available = goexphist.GOExpHist_Entry41Available,
                    GOExpCont_Entry_Details = goexphist.GOExpHist_Entry41Details,
                    GOExpCont_Entry_Entity = goexphist.GOExpHist_Entry41Entity,
                    GOExpCont_Entry_Division = goexphist.GOExpHist_Entry41Division,
                    GOExpCont_Entry_InterAmt = goexphist.GOExpHist_Entry41InterAmt,
                    GOExpCont_Entry_InterRate = goexphist.GOExpHist_Entry41InterRate
                });
            }
            if (!String.IsNullOrEmpty(goexphist.GOExpHist_Entry42Type))
            {
                container.Add(new GOExpContainer
                {
                    GOExpCont_Entry_Type = (goexphist.GOExpHist_Entry42Type == "D") ? "C" : "D",
                    GOExpCont_Entry_Ccy = goexphist.GOExpHist_Entry42Ccy,
                    GOExpCont_Entry_Amt = goexphist.GOExpHist_Entry42Amt,
                    GOExpCont_Entry_Cust = goexphist.GOExpHist_Entry42Cust,
                    GOExpCont_Entry_Actcde = goexphist.GOExpHist_Entry42Actcde,
                    GOExpCont_Entry_ActType = goexphist.GOExpHist_Entry42ActType,
                    GOExpCont_Entry_ActNo = goexphist.GOExpHist_Entry42ActNo,
                    GOExpCont_Entry_ExchRate = goexphist.GOExpHist_Entry42ExchRate,
                    GOExpCont_Entry_ExchCcy = goexphist.GOExpHist_Entry42ExchCcy,
                    GOExpCont_Entry_Fund = goexphist.GOExpHist_Entry42Fund,
                    GOExpCont_Entry_Available = goexphist.GOExpHist_Entry42Available,
                    GOExpCont_Entry_Details = goexphist.GOExpHist_Entry42Details,
                    GOExpCont_Entry_Entity = goexphist.GOExpHist_Entry42Entity,
                    GOExpCont_Entry_Division = goexphist.GOExpHist_Entry42Division,
                    GOExpCont_Entry_InterAmt = goexphist.GOExpHist_Entry42InterAmt,
                    GOExpCont_Entry_InterRate = goexphist.GOExpHist_Entry42InterRate
                });
            }

            return container;
        }

        private TblCm10 ConvGOExpContToTblCM10(List<GOExpContainer> container)
        {
            TblCm10 goexpress = new TblCm10();

            if (container.Count > 0)
            {

                goexpress.Entry11Type = container[0].GOExpCont_Entry_Type;
                goexpress.Entry11Ccy = container[0].GOExpCont_Entry_Ccy;
                goexpress.Entry11Amt = container[0].GOExpCont_Entry_Amt;
                goexpress.Entry11Cust = container[0].GOExpCont_Entry_Cust;
                goexpress.Entry11Actcde = container[0].GOExpCont_Entry_Actcde;
                goexpress.Entry11ActType = container[0].GOExpCont_Entry_ActType;
                goexpress.Entry11ActNo = container[0].GOExpCont_Entry_ActNo;
                goexpress.Entry11ExchRate = container[0].GOExpCont_Entry_ExchRate;
                goexpress.Entry11ExchCcy = container[0].GOExpCont_Entry_ExchCcy;
                goexpress.Entry11Fund = container[0].GOExpCont_Entry_Fund;
                goexpress.Entry11Available = container[0].GOExpCont_Entry_Available;
                goexpress.Entry11Details = container[0].GOExpCont_Entry_Details;
                goexpress.Entry11Entity = container[0].GOExpCont_Entry_Entity;
                goexpress.Entry11Division = container[0].GOExpCont_Entry_Division;
                goexpress.Entry11InterAmt = container[0].GOExpCont_Entry_InterAmt;
                goexpress.Entry11InterRate = container[0].GOExpCont_Entry_InterRate;
            }
            if (container.Count > 1)
            {
                goexpress.Entry12Type = container[1].GOExpCont_Entry_Type;
                goexpress.Entry12Ccy = container[1].GOExpCont_Entry_Ccy;
                goexpress.Entry12Amt = container[1].GOExpCont_Entry_Amt;
                goexpress.Entry12Cust = container[1].GOExpCont_Entry_Cust;
                goexpress.Entry12Actcde = container[1].GOExpCont_Entry_Actcde;
                goexpress.Entry12ActType = container[1].GOExpCont_Entry_ActType;
                goexpress.Entry12ActNo = container[1].GOExpCont_Entry_ActNo;
                goexpress.Entry12ExchRate = container[1].GOExpCont_Entry_ExchRate;
                goexpress.Entry12ExchCcy = container[1].GOExpCont_Entry_ExchCcy;
                goexpress.Entry12Fund = container[1].GOExpCont_Entry_Fund;
                goexpress.Entry12Available = container[1].GOExpCont_Entry_Available;
                goexpress.Entry12Details = container[1].GOExpCont_Entry_Details;
                goexpress.Entry12Entity = container[1].GOExpCont_Entry_Entity;
                goexpress.Entry12Division = container[1].GOExpCont_Entry_Division;
                goexpress.Entry12InterAmt = container[1].GOExpCont_Entry_InterAmt;
                goexpress.Entry12InterRate = container[1].GOExpCont_Entry_InterRate;
            }
            if (container.Count > 2)
            {
                goexpress.Entry21Type = container[2].GOExpCont_Entry_Type;
                goexpress.Entry21Ccy = container[2].GOExpCont_Entry_Ccy;
                goexpress.Entry21Amt = container[2].GOExpCont_Entry_Amt;
                goexpress.Entry21Cust = container[2].GOExpCont_Entry_Cust;
                goexpress.Entry21Actcde = container[2].GOExpCont_Entry_Actcde;
                goexpress.Entry21ActType = container[2].GOExpCont_Entry_ActType;
                goexpress.Entry21ActNo = container[2].GOExpCont_Entry_ActNo;
                goexpress.Entry21ExchRate = container[2].GOExpCont_Entry_ExchRate;
                goexpress.Entry21ExchCcy = container[2].GOExpCont_Entry_ExchCcy;
                goexpress.Entry21Fund = container[2].GOExpCont_Entry_Fund;
                goexpress.Entry21Available = container[2].GOExpCont_Entry_Available;
                goexpress.Entry21Details = container[2].GOExpCont_Entry_Details;
                goexpress.Entry21Entity = container[2].GOExpCont_Entry_Entity;
                goexpress.Entry21Division = container[2].GOExpCont_Entry_Division;
                goexpress.Entry21InterAmt = container[2].GOExpCont_Entry_InterAmt;
                goexpress.Entry21InterRate = container[2].GOExpCont_Entry_InterRate;
            }
            if (container.Count > 3)
            {
                goexpress.Entry22Type = container[3].GOExpCont_Entry_Type;
                goexpress.Entry22Ccy = container[3].GOExpCont_Entry_Ccy;
                goexpress.Entry22Amt = container[3].GOExpCont_Entry_Amt;
                goexpress.Entry22Cust = container[3].GOExpCont_Entry_Cust;
                goexpress.Entry22Actcde = container[3].GOExpCont_Entry_Actcde;
                goexpress.Entry22ActType = container[3].GOExpCont_Entry_ActType;
                goexpress.Entry22ActNo = container[3].GOExpCont_Entry_ActNo;
                goexpress.Entry22ExchRate = container[3].GOExpCont_Entry_ExchRate;
                goexpress.Entry22ExchCcy = container[3].GOExpCont_Entry_ExchCcy;
                goexpress.Entry22Fund = container[3].GOExpCont_Entry_Fund;
                goexpress.Entry22Available = container[3].GOExpCont_Entry_Available;
                goexpress.Entry22Details = container[3].GOExpCont_Entry_Details;
                goexpress.Entry22Entity = container[3].GOExpCont_Entry_Entity;
                goexpress.Entry22Division = container[3].GOExpCont_Entry_Division;
                goexpress.Entry22InterAmt = container[3].GOExpCont_Entry_InterAmt;
                goexpress.Entry22InterRate = container[3].GOExpCont_Entry_InterRate;
            }
            if (container.Count > 4)
            {
                goexpress.Entry31Type = container[4].GOExpCont_Entry_Type;
                goexpress.Entry31Ccy = container[4].GOExpCont_Entry_Ccy;
                goexpress.Entry31Amt = container[4].GOExpCont_Entry_Amt;
                goexpress.Entry31Cust = container[4].GOExpCont_Entry_Cust;
                goexpress.Entry31Actcde = container[4].GOExpCont_Entry_Actcde;
                goexpress.Entry31ActType = container[4].GOExpCont_Entry_ActType;
                goexpress.Entry31ActNo = container[4].GOExpCont_Entry_ActNo;
                goexpress.Entry31ExchRate = container[4].GOExpCont_Entry_ExchRate;
                goexpress.Entry31ExchCcy = container[4].GOExpCont_Entry_ExchCcy;
                goexpress.Entry31Fund = container[4].GOExpCont_Entry_Fund;
                goexpress.Entry31Available = container[4].GOExpCont_Entry_Available;
                goexpress.Entry31Details = container[4].GOExpCont_Entry_Details;
                goexpress.Entry31Entity = container[4].GOExpCont_Entry_Entity;
                goexpress.Entry31Division = container[4].GOExpCont_Entry_Division;
                goexpress.Entry31InterAmt = container[4].GOExpCont_Entry_InterAmt;
                goexpress.Entry31InterRate = container[4].GOExpCont_Entry_InterRate;
            }
            if (container.Count > 5)
            {
                goexpress.Entry32Type = container[5].GOExpCont_Entry_Type;
                goexpress.Entry32Ccy = container[5].GOExpCont_Entry_Ccy;
                goexpress.Entry32Amt = container[5].GOExpCont_Entry_Amt;
                goexpress.Entry32Cust = container[5].GOExpCont_Entry_Cust;
                goexpress.Entry32Actcde = container[5].GOExpCont_Entry_Actcde;
                goexpress.Entry32ActType = container[5].GOExpCont_Entry_ActType;
                goexpress.Entry32ActNo = container[5].GOExpCont_Entry_ActNo;
                goexpress.Entry32ExchRate = container[5].GOExpCont_Entry_ExchRate;
                goexpress.Entry32ExchCcy = container[5].GOExpCont_Entry_ExchCcy;
                goexpress.Entry32Fund = container[5].GOExpCont_Entry_Fund;
                goexpress.Entry32Available = container[5].GOExpCont_Entry_Available;
                goexpress.Entry32Details = container[5].GOExpCont_Entry_Details;
                goexpress.Entry32Entity = container[5].GOExpCont_Entry_Entity;
                goexpress.Entry32Division = container[5].GOExpCont_Entry_Division;
                goexpress.Entry32InterAmt = container[5].GOExpCont_Entry_InterAmt;
                goexpress.Entry32InterRate = container[5].GOExpCont_Entry_InterRate;
            }
            if (container.Count > 6)
            {
                goexpress.Entry41Type = container[6].GOExpCont_Entry_Type;
                goexpress.Entry41Ccy = container[6].GOExpCont_Entry_Ccy;
                goexpress.Entry41Amt = container[6].GOExpCont_Entry_Amt;
                goexpress.Entry41Cust = container[6].GOExpCont_Entry_Cust;
                goexpress.Entry41Actcde = container[6].GOExpCont_Entry_Actcde;
                goexpress.Entry41ActType = container[6].GOExpCont_Entry_ActType;
                goexpress.Entry41ActNo = container[6].GOExpCont_Entry_ActNo;
                goexpress.Entry41ExchRate = container[6].GOExpCont_Entry_ExchRate;
                goexpress.Entry41ExchCcy = container[6].GOExpCont_Entry_ExchCcy;
                goexpress.Entry41Fund = container[6].GOExpCont_Entry_Fund;
                goexpress.Entry41Available = container[6].GOExpCont_Entry_Available;
                goexpress.Entry41Details = container[6].GOExpCont_Entry_Details;
                goexpress.Entry41Entity = container[6].GOExpCont_Entry_Entity;
                goexpress.Entry41Division = container[6].GOExpCont_Entry_Division;
                goexpress.Entry41InterAmt = container[6].GOExpCont_Entry_InterAmt;
                goexpress.Entry41InterRate = container[6].GOExpCont_Entry_InterRate;
            }
            if (container.Count > 7)
            {
                goexpress.Entry42Type = container[7].GOExpCont_Entry_Type;
                goexpress.Entry42Ccy = container[7].GOExpCont_Entry_Ccy;
                goexpress.Entry42Amt = container[7].GOExpCont_Entry_Amt;
                goexpress.Entry42Cust = container[7].GOExpCont_Entry_Cust;
                goexpress.Entry42Actcde = container[7].GOExpCont_Entry_Actcde;
                goexpress.Entry42ActType = container[7].GOExpCont_Entry_ActType;
                goexpress.Entry42ActNo = container[7].GOExpCont_Entry_ActNo;
                goexpress.Entry42ExchRate = container[7].GOExpCont_Entry_ExchRate;
                goexpress.Entry42ExchCcy = container[7].GOExpCont_Entry_ExchCcy;
                goexpress.Entry42Fund = container[7].GOExpCont_Entry_Fund;
                goexpress.Entry42Available = container[7].GOExpCont_Entry_Available;
                goexpress.Entry42Details = container[7].GOExpCont_Entry_Details;
                goexpress.Entry42Entity = container[7].GOExpCont_Entry_Entity;
                goexpress.Entry42Division = container[7].GOExpCont_Entry_Division;
                goexpress.Entry42InterAmt = container[7].GOExpCont_Entry_InterAmt;
                goexpress.Entry42InterRate = container[7].GOExpCont_Entry_InterRate;
            }
            return goexpress;
        }

        public bool IsPendingTransactionInGOExpress()
        {
            int count = _context.ExpenseTransLists.Where(x =>
                        x.TL_StatusID == GlobalSystemValues.STATUS_PENDING ||
                        x.TL_StatusID == GlobalSystemValues.STATUS_RESENDING ||
                        x.TL_StatusID == GlobalSystemValues.STATUS_REVERSING).ToList().Count;
            if (count > 0) return true;

            return false;
        }

        public List<TransFailedTableDataViewModel> GetCurrentTransListStatus(int entryID, bool IsLiq)
        {
            return _context.ExpenseTransLists.Where(x => x.TL_ExpenseID == entryID && x.TL_Liquidation == IsLiq)
                    .Select(x => new TransFailedTableDataViewModel
                    {
                        TF_TRANS_LIST_ID = x.TL_ID,
                        TF_STATUS_ID = x.TL_StatusID,
                        TF_TRANS_ENTRY_ID = x.TL_ExpenseID
                    }).ToList();
        }

        public DateTime ConvGbDateToDateTime(string date)
        {
            string month = date.Substring(0, 2);
            string day = date.Substring(2, 2);
            string year = (int.Parse(date.Substring(4, 2)) + 2000).ToString();

            return DateTime.ParseExact(month + "-" + day + "-" + year, "M-dd-yyyy", CultureInfo.InvariantCulture);
        }

        public string getVoucherNo(int type, DateTime year, int number, bool liq = false)
        {
            string type_code = "";
            if (liq == false)
                type_code = GlobalSystemValues.getApplicationCode(type);
            else
                type_code = "LIQ";

            return type_code + "-" + GetSelectedYearMonthOfTerm(year.Month, year.Year).Year + "-" +
                                       number.ToString().PadLeft(5, '0'); ;
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

        //get currency
        public DMCurrencyModel GetCurrency(int id)
        {
            return _context.DMCurrency.FirstOrDefault(x => x.Curr_ID == id);
        }

        public DMCurrencyModel GetCurrency(string abbrev)
        {
            return _context.DMCurrency.FirstOrDefault(x => x.Curr_CCY_ABBR == abbrev);
        }

        public DMAccountModel GetAccount(string accountType, string accountNumber, string accountCode, string ccy)
        {
            return _context.DMAccount.Where(x => x.Account_No.Contains(accountType)
                                                && x.Account_No.Contains(accountNumber)
                                                && x.Account_Code == accountCode
                                                && x.Account_Currency_MasterID == GetCurrency(ccy).Curr_MasterID).LastOrDefault();
        }
        
        public DMAccountModel GetAccountByMasterID(int masterID)
        {
            return _context.DMAccount.Where(x => x.Account_MasterID == masterID &&
                                                        x.Account_isActive == true && 
                                                        x.Account_isDeleted == false).FirstOrDefault();
        }

        public List<LiqIDsList> GetLiqGOExpHistIDAndLiqInterID(int expID)
        {
            var liquidationDetails = getExpenseToLiqudate(expID);
            List<LiqIDsList> list = new List<LiqIDsList>();

            TblCm10 goExpData = new TblCm10();
            GOExpressHistModel goExpHistData = new GOExpressHistModel();

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
                            account = item.liqInterEntity[0].Liq_AccountID_1_1,
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_1_2 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = item.liqInterEntity[0].Liq_DebitCred_1_2,
                            amount = item.liqInterEntity[0].Liq_Amount_1_2,
                            account = item.liqInterEntity[0].Liq_AccountID_1_2,
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_2_1 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = item.liqInterEntity[0].Liq_DebitCred_2_1,
                            amount = item.liqInterEntity[0].Liq_Amount_2_1,
                            account = item.liqInterEntity[0].Liq_AccountID_2_1,
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_2_2 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = item.liqInterEntity[0].Liq_DebitCred_2_2,
                            amount = item.liqInterEntity[0].Liq_Amount_2_2,
                            account = item.liqInterEntity[0].Liq_AccountID_2_2,
                        });
                    }
                    if (item.liqInterEntity[0].Liq_Amount_3_1 != 0)
                    {
                        tempGbase.entries.Add(new entryContainer
                        {
                            type = item.liqInterEntity[0].Liq_DebitCred_3_1,
                            amount = item.liqInterEntity[0].Liq_Amount_3_1,
                            account = item.liqInterEntity[0].Liq_AccountID_3_1,
                        });
                    }
                    tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                    goExpHistData = ConvertTblCm10ToGOExHistNoSave(ConvertToTblCm10(tempGbase, expID, 0), expID, item.EntryDetailsID);
                    list.Add(new LiqIDsList
                    {
                        expEntryID = expID,
                        expEntryDtlID = item.EntryDetailsID,
                        liqDtlID = liquidationDetails.LiqEntryDetails.Liq_DtlID,
                        LiqInterID = item.liqInterEntity[0].Liq_InterEntityID,
                        goExpHistID = GetGOExpHistIDByGOExpHist(goExpHistData)
                    });
                }
                else if (item.liqInterEntity.Count() != 0 && item.liqCashBreakdown.Count() == 0)
                {
                    foreach (var i in item.liqInterEntity)
                    {
                        if (i.Liq_Amount_1_1 == 0 && i.Liq_Amount_1_2 == 0)
                            continue;

                        gbaseContainer tempGbase = new gbaseContainer();

                        tempGbase.valDate = liquidationDetails.LiqEntryDetails.Liq_Created_Date.Date;
                        tempGbase.remarks = "S" + item.GBaseRemarks;
                        tempGbase.maker = liquidationDetails.LiqEntryDetails.Liq_Created_UserID;
                        tempGbase.approver = liquidationDetails.LiqEntryDetails.Liq_Approver;

                        if (i.Liq_Amount_1_1 != 0)
                        {
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = i.Liq_DebitCred_1_1,
                                ccy = i.Liq_CCY_1_1,
                                amount = i.Liq_Amount_1_1,
                                account = i.Liq_AccountID_1_1,
                                interate = i.Liq_InterRate_1_2,
                                contraCcy = (i.Liq_CCY_1_1 != i.Liq_CCY_1_2) ? i.Liq_CCY_1_2 : 0
                            });
                        }

                        if (i.Liq_Amount_1_2 != 0)
                        {
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = i.Liq_DebitCred_1_2,
                                ccy = i.Liq_CCY_1_2,
                                amount = i.Liq_Amount_1_2,
                                account = i.Liq_AccountID_1_2,
                                interate = i.Liq_InterRate_1_2,
                                contraCcy = (i.Liq_CCY_1_1 != i.Liq_CCY_1_2) ? i.Liq_CCY_1_1 : 0
                            });
                        }

                        if (i.Liq_Amount_2_1 != 0)
                        {
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = i.Liq_DebitCred_2_1,
                                ccy = i.Liq_CCY_2_1,
                                amount = i.Liq_Amount_2_1,
                                account = i.Liq_AccountID_2_1,
                                interate = i.Liq_InterRate_2_1
                            });
                        }

                        if (i.Liq_Amount_2_2 != 0)
                        {
                            tempGbase.entries.Add(new entryContainer
                            {
                                type = i.Liq_DebitCred_2_2,
                                ccy = i.Liq_CCY_2_2,
                                amount = i.Liq_Amount_2_2,
                                account = i.Liq_AccountID_2_2,
                                interate = i.Liq_InterRate_2_2
                            });
                        }

                        tempGbase.entries = tempGbase.entries.OrderByDescending(x => x.type).ToList();
                        goExpHistData = ConvertTblCm10ToGOExHistNoSave(ConvertToTblCm10(tempGbase, expID, 0), expID, item.EntryDetailsID);
                        list.Add(new LiqIDsList
                        {
                            expEntryID = expID,
                            expEntryDtlID = item.EntryDetailsID,
                            liqDtlID = liquidationDetails.LiqEntryDetails.Liq_DtlID,
                            LiqInterID = i.Liq_InterEntityID,
                            goExpHistID = GetGOExpHistIDByGOExpHist(goExpHistData)
                        });
                    }
                }
            }

            return list;
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
                    ViewModels.Entry.EntryGbaseRemarksViewModel gbaseTemp = new EntryGbaseRemarksViewModel()
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
                        Liq_InterEntityID = liqIE.id,
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
                        Liq_InterRate_3_2 = liqIE.Liq_InterRate_3_2,
                        Liq_Tax_Rate = liqIE.Liq_TaxRate,
                        Liq_VendorID = liqIE.Liq_VendorID
                    };

                    liqInterEntity.Add(liqIETemp);
                }

                var accountInfo = _context.DMAccount.Where(x => x.Account_ID == dtl.d.ExpDtl_Account).Single();
                int liqFlag = 0;
                if (liqCashBreakdown.Count != 0)
                {
                    liqFlag = 1;
                }
                if (liqInterEntity.Count != 0 && liqCashBreakdown.Count == 0)
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
                    vatValue = (dtl.d.ExpDtl_Vat <= 0) ? 0 : (float)Mizuho.round(getVat(dtl.d.ExpDtl_Vat) * 100, 2),
                    chkEwt = dtl.d.ExpDtl_isEwt,
                    ewtID = dtl.d.ExpDtl_Ewt,
                    ewtValue = (dtl.d.ExpDtl_Ewt <= 0) ? 0 : GetEWTValue(dtl.d.ExpDtl_Ewt) * 100,
                    ccyID = dtl.d.ExpDtl_Ccy,
                    ccyMasterID = getCurrency(dtl.d.ExpDtl_Ccy).Curr_MasterID,
                    ccyAbbrev = GetCurrencyAbbrv(dtl.d.ExpDtl_Ccy),
                    debitGross = dtl.d.ExpDtl_Debit,
                    credEwt = dtl.d.ExpDtl_Credit_Ewt,
                    credCash = dtl.d.ExpDtl_Credit_Cash,
                    dtlSSPayee = dtl.d.ExpDtl_SS_Payee,
                    dtl_Ewt_Payor_Name_ID = dtl.d.ExpDtl_Ewt_Payor_Name_ID,
                    dtlSSPayeeName = getVendorName(dtl.d.ExpDtl_SS_Payee, GlobalSystemValues.PAYEETYPE_REGEMP),
                    //vendTRList = (dtl.d.ExpDtl_Ewt_Payor_Name_ID > 0) ? getVendorTaxList(getVendor(dtl.d.ExpDtl_Ewt_Payor_Name_ID).Vendor_MasterID) : new List<DMTRModel> { new DMTRModel { TR_ID = 0, TR_Tax_Rate = 0 } },
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
                expenseId = EntryDetails.e.Expense_Number.ToString().PadLeft(5, '0'),
                checkNo = EntryDetails.e.Expense_CheckNo,
                statusID = (liqStatus == null) ? 0 : liqStatus.Liq_Status,
                status = (liqStatus == null) ? "" : getStatus(liqStatus.Liq_Status),
                maker = (liqStatus == null) ? EntryDetails.e.Expense_Creator_ID : liqStatus.Liq_Created_UserID,
                verifier_1 = (liqStatus == null) ? "" : (liqStatus.Liq_Verifier1 > 0) ? getUserName(liqStatus.Liq_Verifier1) : "",
                verifier_2 = (liqStatus == null) ? "" : (liqStatus.Liq_Verifier2 > 0) ? getUserName(liqStatus.Liq_Verifier2) : "",
                approver = (liqStatus == null) ? "" : (liqStatus.Liq_Approver > 0) ? getUserName(liqStatus.Liq_Approver) : "",
                approver_id = (liqStatus == null) ? 0 : liqStatus.Liq_Approver,
                verifier_1_id = (liqStatus == null) ? 0 : liqStatus.Liq_Verifier1,
                verifier_2_id = (liqStatus == null) ? 0 : liqStatus.Liq_Verifier2,
                createdDate = EntryDetails.e.Expense_Created_Date,
                LiquidationDetails = liqList,
                LiqEntryDetails = (liqStatus == null) ? new LiquidationEntryDetailModel() : liqStatus
            };

            return liqModel;
        }

        private TblCm10 ConvertToTblCm10(gbaseContainer containerModel, int expenseID, int userID)
        {
            TblCm10 goModel = new TblCm10();

            var phpID = getCurrencyByMasterID(int.Parse(xelemLiq.Element("CURRENCY_PHP").Value)).Curr_ID;

            //goModel.Id = -1;
            goModel.SystemName = "EXPRESS";
            goModel.Branchno = getAccount(containerModel.entries[0].account).Account_No.Substring(4, 3);
            goModel.AutoApproved = "Y";
            goModel.ValueDate = DateTime.Now.ToString("MMddyy");
            goModel.Section = "10";
            goModel.WarningOverride = "Y";
            goModel.Remarks = containerModel.remarks;
            goModel.MakerEmpno = ""; //Replace with user ID later when user module is finished.
            goModel.Empno = "";  //Replace with user ID later when user module is finished.
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
                goModel.Entry11ExchRate = containerModel.entries[0].interate == 0 ? "" : containerModel.entries[0].interate.ToString();
                goModel.Entry11ExchCcy = (containerModel.entries[0].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[0].contraCcy) : "";
                goModel.Entry11Fund = (entry11Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                goModel.Entry11Available = "";//Replace with proper available default.
                goModel.Entry11Details = "";//Replace with proper details default.
                goModel.Entry11Entity = "010";//Replace with proper entity default.
                goModel.Entry11Division = entry11Account.Account_Div;//Replace with proper division default.
                //if(containerModel.entries[0].type == "C")
                if (containerModel.entries[0].ccy != containerModel.entries[1].ccy && containerModel.entries[0].ccy != phpID)
                    goModel.Entry11InterRate = (containerModel.entries[0].interate > 0) ? containerModel.entries[0].interate.ToString() : "";//Replace with proper interate default.
                goModel.Entry11InterAmt = "";//Replace with proper interamt default.

                if (containerModel.entries.Count > 1)
                {
                    goModel.Entry12Type = containerModel.entries[1].type;
                    goModel.Entry12Ccy = GetCurrencyAbbrv(containerModel.entries[1].ccy);
                    goModel.Entry12Amt = containerModel.entries[1].amount.ToString();

                    var entry12Account = getAccount(containerModel.entries[1].account);
                    goModel.Entry12Cust = entry12Account.Account_Cust;
                    goModel.Entry12Actcde = entry12Account.Account_Code;
                    goModel.Entry12ActType = entry12Account.Account_No.Substring(0, 3);
                    goModel.Entry12ActNo = entry12Account.Account_No.Substring(Math.Max(0, entry12Account.Account_No.Length - 6));
                    goModel.Entry12ExchRate = containerModel.entries[1].interate == 0 ? "" : containerModel.entries[1].interate.ToString();
                    goModel.Entry12ExchCcy = (containerModel.entries[1].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[1].contraCcy) : "";
                    goModel.Entry12Fund = (entry12Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry12Available = "";//Replace with proper available default.
                    goModel.Entry12Details = "";//Replace with proper details default.
                    goModel.Entry12Entity = "010";//Replace with proper entity default.
                    goModel.Entry12Division = entry12Account.Account_Div;//Replace with proper division default.
                    //if (containerModel.entries[1].type == "C")
                    if ((containerModel.entries[0].ccy != containerModel.entries[1].ccy) && containerModel.entries[1].ccy != phpID)
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
                    goModel.Entry21ExchRate = containerModel.entries[2].interate == 0 ? "" : containerModel.entries[2].interate.ToString();
                    goModel.Entry21ExchCcy = (containerModel.entries[2].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[2].contraCcy) : "";
                    goModel.Entry21Fund = (entry21Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry21Available = "";//Replace with proper available default.
                    goModel.Entry21Details = "";//Replace with proper details default.
                    goModel.Entry21Entity = "010";//Replace with proper entity default.
                    goModel.Entry21Division = entry21Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[2].type == "C")
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
                    goModel.Entry22ExchRate = containerModel.entries[3].interate == 0 ? "" : containerModel.entries[3].interate.ToString();
                    goModel.Entry22ExchCcy = (containerModel.entries[3].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[3].contraCcy) : "";
                    goModel.Entry22Fund = (entry22Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry22Available = "";//Replace with proper available default.
                    goModel.Entry22Details = "";//Replace with proper details default.
                    goModel.Entry22Entity = "010";//Replace with proper entity default.
                    goModel.Entry22Division = entry22Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[3].type == "C")
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
                    goModel.Entry31ExchRate = containerModel.entries[4].interate == 0 ? "" : containerModel.entries[4].interate.ToString();
                    goModel.Entry31ExchCcy = (containerModel.entries[4].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[4].contraCcy) : "";
                    goModel.Entry31Fund = (entry31Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry31Available = "";//Replace with proper available default.
                    goModel.Entry31Details = "";//Replace with proper details default.
                    goModel.Entry31Entity = "010";//Replace with proper entity default.
                    goModel.Entry31Division = entry31Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[4].type == "C")
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
                    goModel.Entry32ExchRate = containerModel.entries[5].interate == 0 ? "" : containerModel.entries[5].interate.ToString();
                    goModel.Entry32ExchCcy = (containerModel.entries[5].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[5].contraCcy) : "";
                    goModel.Entry32Fund = (entry32Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry32Available = "";//Replace with proper available default.
                    goModel.Entry32Details = "";//Replace with proper details default.
                    goModel.Entry32Entity = "010";//Replace with proper entity default.
                    goModel.Entry32Division = entry32Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[5].type == "C")
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
                    goModel.Entry41ExchRate = containerModel.entries[6].interate == 0 ? "" : containerModel.entries[6].interate.ToString();
                    goModel.Entry41ExchCcy = (containerModel.entries[6].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[6].contraCcy) : "";
                    goModel.Entry41Fund = (entry41Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry41Available = "";//Replace with proper available default.
                    goModel.Entry41Details = "";//Replace with proper details default.
                    goModel.Entry41Entity = "010";//Replace with proper entity default.
                    goModel.Entry41Division = entry41Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[6].type == "C")
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
                    goModel.Entry42ExchRate = containerModel.entries[7].interate == 0 ? "" : containerModel.entries[7].interate.ToString();
                    goModel.Entry42ExchCcy = (containerModel.entries[7].contraCcy > 0) ? GetCurrencyAbbrv(containerModel.entries[7].contraCcy) : "";
                    goModel.Entry42Fund = (entry42Account.Account_Fund == true) ? "O" : "";//Replace with proper fund default.
                    goModel.Entry42Available = "";//Replace with proper available default.
                    goModel.Entry42Details = "";//Replace with proper details default.
                    goModel.Entry42Entity = "010";//Replace with proper entity default.
                    goModel.Entry42Division = entry42Account.Account_Div;//Replace with proper division default.
                    if (containerModel.entries[7].type == "C")
                        goModel.Entry42InterRate = (containerModel.entries[7].interate > 0) ? containerModel.entries[7].interate.ToString() : "";//Replace with proper interate default.
                    goModel.Entry42InterAmt = "";//Replace with proper interamt default.
                }
            }
            else
            {
                return goModel;
            }

            return goModel;
        }

        private GOExpressHistModel ConvertTblCm10ToGOExHistNoSave(TblCm10 tblcm10, int entryID, int entryDtlID)
        {
            var goExpHist = new GOExpressHistModel
            {
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

            return goExpHist;
        }

        private int GetGOExpHistIDByGOExpHist(GOExpressHistModel goCompare)
        {
            var goexphist = _context.GOExpressHist.Where(x => x.ExpenseEntryID == goCompare.ExpenseEntryID &&
                                                        x.ExpenseDetailID == goCompare.ExpenseDetailID).ToList();

            foreach (var i in goexphist)
            {
                if (i.GOExpHist_SystemName == goCompare.GOExpHist_SystemName &&
                    i.GOExpHist_Groupcode == goCompare.GOExpHist_Groupcode &&
                    i.GOExpHist_Branchno == goCompare.GOExpHist_Branchno &&
                    i.GOExpHist_OpeKind == goCompare.GOExpHist_OpeKind &&
                    i.GOExpHist_AutoApproved == goCompare.GOExpHist_AutoApproved &&
                    i.GOExpHist_WarningOverride == goCompare.GOExpHist_WarningOverride &&
                    i.GOExpHist_CcyFormat == goCompare.GOExpHist_CcyFormat &&
                    i.GOExpHist_OpeBranch == goCompare.GOExpHist_OpeBranch &&
                    i.GOExpHist_ValueDate == goCompare.GOExpHist_ValueDate &&
                    i.GOExpHist_ReferenceType == goCompare.GOExpHist_ReferenceType &&
                    i.GOExpHist_ReferenceNo == goCompare.GOExpHist_ReferenceNo &&
                    i.GOExpHist_Comment == goCompare.GOExpHist_Comment &&
                    i.GOExpHist_Section == goCompare.GOExpHist_Section &&
                    i.GOExpHist_Remarks == goCompare.GOExpHist_Remarks &&
                    i.GOExpHist_Memo == goCompare.GOExpHist_Memo &&
                    i.GOExpHist_SchemeNo == goCompare.GOExpHist_SchemeNo &&
                    i.GOExpHist_Entry11Type == goCompare.GOExpHist_Entry11Type &&
                    i.GOExpHist_Entry11IbfCode == goCompare.GOExpHist_Entry11IbfCode &&
                    i.GOExpHist_Entry11Ccy == goCompare.GOExpHist_Entry11Ccy &&
                    i.GOExpHist_Entry11Amt == goCompare.GOExpHist_Entry11Amt &&
                    i.GOExpHist_Entry11Cust == goCompare.GOExpHist_Entry11Cust &&
                    i.GOExpHist_Entry11Actcde == goCompare.GOExpHist_Entry11Actcde &&
                    i.GOExpHist_Entry11ActType == goCompare.GOExpHist_Entry11ActType &&
                    i.GOExpHist_Entry11ActNo == goCompare.GOExpHist_Entry11ActNo &&
                    i.GOExpHist_Entry11ExchRate == goCompare.GOExpHist_Entry11ExchRate &&
                    i.GOExpHist_Entry11ExchCcy == goCompare.GOExpHist_Entry11ExchCcy &&
                    i.GOExpHist_Entry11Fund == goCompare.GOExpHist_Entry11Fund &&
                    i.GOExpHist_Entry11CheckNo == goCompare.GOExpHist_Entry11CheckNo &&
                    i.GOExpHist_Entry11Available == goCompare.GOExpHist_Entry11Available &&
                    i.GOExpHist_Entry11AdvcPrnt == goCompare.GOExpHist_Entry11AdvcPrnt &&
                    i.GOExpHist_Entry11Details == goCompare.GOExpHist_Entry11Details &&
                    i.GOExpHist_Entry11Entity == goCompare.GOExpHist_Entry11Entity &&
                    i.GOExpHist_Entry11Division == goCompare.GOExpHist_Entry11Division &&
                    i.GOExpHist_Entry11InterAmt == goCompare.GOExpHist_Entry11InterAmt &&
                    i.GOExpHist_Entry11InterRate == goCompare.GOExpHist_Entry11InterRate &&
                    i.GOExpHist_Entry12Type == goCompare.GOExpHist_Entry12Type &&
                    i.GOExpHist_Entry12IbfCode == goCompare.GOExpHist_Entry12IbfCode &&
                    i.GOExpHist_Entry12Ccy == goCompare.GOExpHist_Entry12Ccy &&
                    i.GOExpHist_Entry12Amt == goCompare.GOExpHist_Entry12Amt &&
                    i.GOExpHist_Entry12Cust == goCompare.GOExpHist_Entry12Cust &&
                    i.GOExpHist_Entry12Actcde == goCompare.GOExpHist_Entry12Actcde &&
                    i.GOExpHist_Entry12ActType == goCompare.GOExpHist_Entry12ActType &&
                    i.GOExpHist_Entry12ActNo == goCompare.GOExpHist_Entry12ActNo &&
                    i.GOExpHist_Entry12ExchRate == goCompare.GOExpHist_Entry12ExchRate &&
                    i.GOExpHist_Entry12ExchCcy == goCompare.GOExpHist_Entry12ExchCcy &&
                    i.GOExpHist_Entry12Fund == goCompare.GOExpHist_Entry12Fund &&
                    i.GOExpHist_Entry12CheckNo == goCompare.GOExpHist_Entry12CheckNo &&
                    i.GOExpHist_Entry12Available == goCompare.GOExpHist_Entry12Available &&
                    i.GOExpHist_Entry12AdvcPrnt == goCompare.GOExpHist_Entry12AdvcPrnt &&
                    i.GOExpHist_Entry12Details == goCompare.GOExpHist_Entry12Details &&
                    i.GOExpHist_Entry12Entity == goCompare.GOExpHist_Entry12Entity &&
                    i.GOExpHist_Entry12Division == goCompare.GOExpHist_Entry12Division &&
                    i.GOExpHist_Entry12InterAmt == goCompare.GOExpHist_Entry12InterAmt &&
                    i.GOExpHist_Entry12InterRate == goCompare.GOExpHist_Entry12InterRate &&
                    i.GOExpHist_Entry21Type == goCompare.GOExpHist_Entry21Type &&
                    i.GOExpHist_Entry21IbfCode == goCompare.GOExpHist_Entry21IbfCode &&
                    i.GOExpHist_Entry21Ccy == goCompare.GOExpHist_Entry21Ccy &&
                    i.GOExpHist_Entry21Amt == goCompare.GOExpHist_Entry21Amt &&
                    i.GOExpHist_Entry21Cust == goCompare.GOExpHist_Entry21Cust &&
                    i.GOExpHist_Entry21Actcde == goCompare.GOExpHist_Entry21Actcde &&
                    i.GOExpHist_Entry21ActType == goCompare.GOExpHist_Entry21ActType &&
                    i.GOExpHist_Entry21ActNo == goCompare.GOExpHist_Entry21ActNo &&
                    i.GOExpHist_Entry21ExchRate == goCompare.GOExpHist_Entry21ExchRate &&
                    i.GOExpHist_Entry21ExchCcy == goCompare.GOExpHist_Entry21ExchCcy &&
                    i.GOExpHist_Entry21Fund == goCompare.GOExpHist_Entry21Fund &&
                    i.GOExpHist_Entry21CheckNo == goCompare.GOExpHist_Entry21CheckNo &&
                    i.GOExpHist_Entry21Available == goCompare.GOExpHist_Entry21Available &&
                    i.GOExpHist_Entry21AdvcPrnt == goCompare.GOExpHist_Entry21AdvcPrnt &&
                    i.GOExpHist_Entry21Details == goCompare.GOExpHist_Entry21Details &&
                    i.GOExpHist_Entry21Entity == goCompare.GOExpHist_Entry21Entity &&
                    i.GOExpHist_Entry21Division == goCompare.GOExpHist_Entry21Division &&
                    i.GOExpHist_Entry21InterAmt == goCompare.GOExpHist_Entry21InterAmt &&
                    i.GOExpHist_Entry21InterRate == goCompare.GOExpHist_Entry21InterRate &&
                    i.GOExpHist_Entry22Type == goCompare.GOExpHist_Entry22Type &&
                    i.GOExpHist_Entry22IbfCode == goCompare.GOExpHist_Entry22IbfCode &&
                    i.GOExpHist_Entry22Ccy == goCompare.GOExpHist_Entry22Ccy &&
                    i.GOExpHist_Entry22Amt == goCompare.GOExpHist_Entry22Amt &&
                    i.GOExpHist_Entry22Cust == goCompare.GOExpHist_Entry22Cust &&
                    i.GOExpHist_Entry22Actcde == goCompare.GOExpHist_Entry22Actcde &&
                    i.GOExpHist_Entry22ActType == goCompare.GOExpHist_Entry22ActType &&
                    i.GOExpHist_Entry22ActNo == goCompare.GOExpHist_Entry22ActNo &&
                    i.GOExpHist_Entry22ExchRate == goCompare.GOExpHist_Entry22ExchRate &&
                    i.GOExpHist_Entry22ExchCcy == goCompare.GOExpHist_Entry22ExchCcy &&
                    i.GOExpHist_Entry22Fund == goCompare.GOExpHist_Entry22Fund &&
                    i.GOExpHist_Entry22CheckNo == goCompare.GOExpHist_Entry22CheckNo &&
                    i.GOExpHist_Entry22Available == goCompare.GOExpHist_Entry22Available &&
                    i.GOExpHist_Entry22AdvcPrnt == goCompare.GOExpHist_Entry22AdvcPrnt &&
                    i.GOExpHist_Entry22Details == goCompare.GOExpHist_Entry22Details &&
                    i.GOExpHist_Entry22Entity == goCompare.GOExpHist_Entry22Entity &&
                    i.GOExpHist_Entry22Division == goCompare.GOExpHist_Entry22Division &&
                    i.GOExpHist_Entry22InterAmt == goCompare.GOExpHist_Entry22InterAmt &&
                    i.GOExpHist_Entry22InterRate == goCompare.GOExpHist_Entry22InterRate &&
                    i.GOExpHist_Entry31Type == goCompare.GOExpHist_Entry31Type &&
                    i.GOExpHist_Entry31IbfCode == goCompare.GOExpHist_Entry31IbfCode &&
                    i.GOExpHist_Entry31Ccy == goCompare.GOExpHist_Entry31Ccy &&
                    i.GOExpHist_Entry31Amt == goCompare.GOExpHist_Entry31Amt &&
                    i.GOExpHist_Entry31Cust == goCompare.GOExpHist_Entry31Cust &&
                    i.GOExpHist_Entry31Actcde == goCompare.GOExpHist_Entry31Actcde &&
                    i.GOExpHist_Entry31ActType == goCompare.GOExpHist_Entry31ActType &&
                    i.GOExpHist_Entry31ActNo == goCompare.GOExpHist_Entry31ActNo &&
                    i.GOExpHist_Entry31ExchRate == goCompare.GOExpHist_Entry31ExchRate &&
                    i.GOExpHist_Entry31ExchCcy == goCompare.GOExpHist_Entry31ExchCcy &&
                    i.GOExpHist_Entry31Fund == goCompare.GOExpHist_Entry31Fund &&
                    i.GOExpHist_Entry31CheckNo == goCompare.GOExpHist_Entry31CheckNo &&
                    i.GOExpHist_Entry31Available == goCompare.GOExpHist_Entry31Available &&
                    i.GOExpHist_Entry31AdvcPrnt == goCompare.GOExpHist_Entry31AdvcPrnt &&
                    i.GOExpHist_Entry31Details == goCompare.GOExpHist_Entry31Details &&
                    i.GOExpHist_Entry31Entity == goCompare.GOExpHist_Entry31Entity &&
                    i.GOExpHist_Entry31Division == goCompare.GOExpHist_Entry31Division &&
                    i.GOExpHist_Entry31InterAmt == goCompare.GOExpHist_Entry31InterAmt &&
                    i.GOExpHist_Entry31InterRate == goCompare.GOExpHist_Entry31InterRate &&
                    i.GOExpHist_Entry32Type == goCompare.GOExpHist_Entry32Type &&
                    i.GOExpHist_Entry32IbfCode == goCompare.GOExpHist_Entry32IbfCode &&
                    i.GOExpHist_Entry32Ccy == goCompare.GOExpHist_Entry32Ccy &&
                    i.GOExpHist_Entry32Amt == goCompare.GOExpHist_Entry32Amt &&
                    i.GOExpHist_Entry32Cust == goCompare.GOExpHist_Entry32Cust &&
                    i.GOExpHist_Entry32Actcde == goCompare.GOExpHist_Entry32Actcde &&
                    i.GOExpHist_Entry32ActType == goCompare.GOExpHist_Entry32ActType &&
                    i.GOExpHist_Entry32ActNo == goCompare.GOExpHist_Entry32ActNo &&
                    i.GOExpHist_Entry32ExchRate == goCompare.GOExpHist_Entry32ExchRate &&
                    i.GOExpHist_Entry32ExchCcy == goCompare.GOExpHist_Entry32ExchCcy &&
                    i.GOExpHist_Entry32Fund == goCompare.GOExpHist_Entry32Fund &&
                    i.GOExpHist_Entry32CheckNo == goCompare.GOExpHist_Entry32CheckNo &&
                    i.GOExpHist_Entry32Available == goCompare.GOExpHist_Entry32Available &&
                    i.GOExpHist_Entry32AdvcPrnt == goCompare.GOExpHist_Entry32AdvcPrnt &&
                    i.GOExpHist_Entry32Details == goCompare.GOExpHist_Entry32Details &&
                    i.GOExpHist_Entry32Entity == goCompare.GOExpHist_Entry32Entity &&
                    i.GOExpHist_Entry32Division == goCompare.GOExpHist_Entry32Division &&
                    i.GOExpHist_Entry32InterAmt == goCompare.GOExpHist_Entry32InterAmt &&
                    i.GOExpHist_Entry32InterRate == goCompare.GOExpHist_Entry32InterRate &&
                    i.GOExpHist_Entry41Type == goCompare.GOExpHist_Entry41Type &&
                    i.GOExpHist_Entry41IbfCode == goCompare.GOExpHist_Entry41IbfCode &&
                    i.GOExpHist_Entry41Ccy == goCompare.GOExpHist_Entry41Ccy &&
                    i.GOExpHist_Entry41Amt == goCompare.GOExpHist_Entry41Amt &&
                    i.GOExpHist_Entry41Cust == goCompare.GOExpHist_Entry41Cust &&
                    i.GOExpHist_Entry41Actcde == goCompare.GOExpHist_Entry41Actcde &&
                    i.GOExpHist_Entry41ActType == goCompare.GOExpHist_Entry41ActType &&
                    i.GOExpHist_Entry41ActNo == goCompare.GOExpHist_Entry41ActNo &&
                    i.GOExpHist_Entry41ExchRate == goCompare.GOExpHist_Entry41ExchRate &&
                    i.GOExpHist_Entry41ExchCcy == goCompare.GOExpHist_Entry41ExchCcy &&
                    i.GOExpHist_Entry41Fund == goCompare.GOExpHist_Entry41Fund &&
                    i.GOExpHist_Entry41CheckNo == goCompare.GOExpHist_Entry41CheckNo &&
                    i.GOExpHist_Entry41Available == goCompare.GOExpHist_Entry41Available &&
                    i.GOExpHist_Entry41AdvcPrnt == goCompare.GOExpHist_Entry41AdvcPrnt &&
                    i.GOExpHist_Entry41Details == goCompare.GOExpHist_Entry41Details &&
                    i.GOExpHist_Entry41Entity == goCompare.GOExpHist_Entry41Entity &&
                    i.GOExpHist_Entry41Division == goCompare.GOExpHist_Entry41Division &&
                    i.GOExpHist_Entry41InterAmt == goCompare.GOExpHist_Entry41InterAmt &&
                    i.GOExpHist_Entry41InterRate == goCompare.GOExpHist_Entry41InterRate &&
                    i.GOExpHist_Entry42Type == goCompare.GOExpHist_Entry42Type &&
                    i.GOExpHist_Entry42IbfCode == goCompare.GOExpHist_Entry42IbfCode &&
                    i.GOExpHist_Entry42Ccy == goCompare.GOExpHist_Entry42Ccy &&
                    i.GOExpHist_Entry42Amt == goCompare.GOExpHist_Entry42Amt &&
                    i.GOExpHist_Entry42Cust == goCompare.GOExpHist_Entry42Cust &&
                    i.GOExpHist_Entry42Actcde == goCompare.GOExpHist_Entry42Actcde &&
                    i.GOExpHist_Entry42ActType == goCompare.GOExpHist_Entry42ActType &&
                    i.GOExpHist_Entry42ActNo == goCompare.GOExpHist_Entry42ActNo &&
                    i.GOExpHist_Entry42ExchRate == goCompare.GOExpHist_Entry42ExchRate &&
                    i.GOExpHist_Entry42ExchCcy == goCompare.GOExpHist_Entry42ExchCcy &&
                    i.GOExpHist_Entry42Fund == goCompare.GOExpHist_Entry42Fund &&
                    i.GOExpHist_Entry42CheckNo == goCompare.GOExpHist_Entry42CheckNo &&
                    i.GOExpHist_Entry42Available == goCompare.GOExpHist_Entry42Available &&
                    i.GOExpHist_Entry42AdvcPrnt == goCompare.GOExpHist_Entry42AdvcPrnt &&
                    i.GOExpHist_Entry42Details == goCompare.GOExpHist_Entry42Details &&
                    i.GOExpHist_Entry42Entity == goCompare.GOExpHist_Entry42Entity &&
                    i.GOExpHist_Entry42Division == goCompare.GOExpHist_Entry42Division &&
                    i.GOExpHist_Entry42InterAmt == goCompare.GOExpHist_Entry42InterAmt &&
                    i.GOExpHist_Entry42InterRate == goCompare.GOExpHist_Entry42InterRate)
                {
                    return i.GOExpHist_Id;
                }
            }
            return 0;
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
        public decimal getVat()
        {
            var vat = _context.DMVAT.FirstOrDefault(q => q.VAT_isActive == true);

            if (vat == null)
            {
                return 0;
            }

            return (decimal)vat.VAT_Rate;
        }

        public decimal getVat(int id)
        {
            var vat = _context.DMVAT.Where(x => x.VAT_ID == id).FirstOrDefault();
            if (vat == null)
            {
                return 0;
            }
            return (decimal)vat.VAT_Rate;
        }

        //get EWT(Tax Rate) value
        public float GetEWTValue(int id)
        {
            return _context.DMTR.Where(x => x.TR_ID == id).First().TR_Tax_Rate;
        }

        //get currency
        public DMCurrencyModel getCurrency(int id)
        {
            return _context.DMCurrency.FirstOrDefault(x => x.Curr_ID == id);
        }

        //get currency abbreviation
        public string GetCurrencyAbbrv(int id)
        {
            return (id != 0) ? _context.DMCurrency.Where(x => x.Curr_ID == id).First().Curr_CCY_ABBR : "PHP";
        }

        //get vendor name
        public string getVendorName(int vendorID, int payeeTypeID)
        {
            if (payeeTypeID == GlobalSystemValues.PAYEETYPE_VENDOR)
            {
                return _context.DMVendor.Where(x => x.Vendor_ID == vendorID).Select(x => x.Vendor_Name).FirstOrDefault();
            }
            if (payeeTypeID == GlobalSystemValues.PAYEETYPE_REGEMP || payeeTypeID == GlobalSystemValues.PAYEETYPE_TEMPEMP)
            {
                return _context.DMEmp.Where(x => x.Emp_ID == vendorID).Select(x => x.Emp_Name).FirstOrDefault();
            }
            if (payeeTypeID == GlobalSystemValues.PAYEETYPE_CUST)
            {
                return _context.DMCust.Where(x => x.Cust_ID == vendorID).Select(x => x.Cust_Name).FirstOrDefault();
            }
            return null;
        }

        //get account
        public DMAccountModel getAccount(int id)
        {
            return _context.DMAccount.FirstOrDefault(x => x.Account_ID == id);
        }

        //Get lastest currency by its currency master ID.
        public DMCurrencyModel getCurrencyByMasterID(int masterID)
        {
            return _context.DMCurrency.Where(x => x.Curr_MasterID == masterID && x.Curr_isActive == true
                    && x.Curr_isDeleted == false).FirstOrDefault();
        }

    }

    public class GOExpContainer
    {
        public string GOExpCont_Entry_Type;
        public string GOExpCont_Entry_Ccy;
        public string GOExpCont_Entry_Amt;
        public string GOExpCont_Entry_Cust;
        public string GOExpCont_Entry_Actcde;
        public string GOExpCont_Entry_ActType;
        public string GOExpCont_Entry_ActNo;
        public string GOExpCont_Entry_ExchRate;
        public string GOExpCont_Entry_ExchCcy;
        public string GOExpCont_Entry_Fund;
        public string GOExpCont_Entry_Available;
        public string GOExpCont_Entry_Details;
        public string GOExpCont_Entry_Entity;
        public string GOExpCont_Entry_Division;
        public string GOExpCont_Entry_InterAmt;
        public string GOExpCont_Entry_InterRate;
    }

    public class LiqIDsList
    {
        public int expEntryID { get; set; }
        public int expEntryDtlID { get; set; }
        public int liqDtlID { get; set; }
        public int LiqInterID { get; set; }
        public int goExpHistID { get; set; }
    }
}
