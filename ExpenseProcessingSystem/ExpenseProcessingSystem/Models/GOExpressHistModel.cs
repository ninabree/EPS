﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class GOExpressHistModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GOExpHist_Id { get; set; }
        public string GOExpHist_SystemName { get; set; }
        public string GOExpHist_Groupcode { get; set; }
        public string GOExpHist_Branchno { get; set; }
        public string GOExpHist_OpeKind { get; set; }
        public string GOExpHist_AutoApproved { get; set; }
        public string GOExpHist_WarningOverride { get; set; }
        public string GOExpHist_CcyFormat { get; set; }
        public string GOExpHist_OpeBranch { get; set; }
        public string GOExpHist_ValueDate { get; set; }
        public string GOExpHist_ReferenceType { get; set; }
        public string GOExpHist_ReferenceNo { get; set; }
        public string GOExpHist_Comment { get; set; }
        public string GOExpHist_Section { get; set; }
        public string GOExpHist_Remarks { get; set; }
        public string GOExpHist_Memo { get; set; }
        public string GOExpHist_SchemeNo { get; set; }
        public string GOExpHist_Entry11Type { get; set; }
        public string GOExpHist_Entry11IbfCode { get; set; }
        public string GOExpHist_Entry11Ccy { get; set; }
        public string GOExpHist_Entry11Amt { get; set; }
        public string GOExpHist_Entry11Cust { get; set; }
        public string GOExpHist_Entry11Actcde { get; set; }
        public string GOExpHist_Entry11ActType { get; set; }
        public string GOExpHist_Entry11ActNo { get; set; }
        public string GOExpHist_Entry11ExchRate { get; set; }
        public string GOExpHist_Entry11ExchCcy { get; set; }
        public string GOExpHist_Entry11Fund { get; set; }
        public string GOExpHist_Entry11CheckNo { get; set; }
        public string GOExpHist_Entry11Available { get; set; }
        public string GOExpHist_Entry11AdvcPrnt { get; set; }
        public string GOExpHist_Entry11Details { get; set; }
        public string GOExpHist_Entry11Entity { get; set; }
        public string GOExpHist_Entry11Division { get; set; }
        public string GOExpHist_Entry11InterAmt { get; set; }
        public string GOExpHist_Entry11InterRate { get; set; }
        public string GOExpHist_Entry12Type { get; set; }
        public string GOExpHist_Entry12IbfCode { get; set; }
        public string GOExpHist_Entry12Ccy { get; set; }
        public string GOExpHist_Entry12Amt { get; set; }
        public string GOExpHist_Entry12Cust { get; set; }
        public string GOExpHist_Entry12Actcde { get; set; }
        public string GOExpHist_Entry12ActType { get; set; }
        public string GOExpHist_Entry12ActNo { get; set; }
        public string GOExpHist_Entry12ExchRate { get; set; }
        public string GOExpHist_Entry12ExchCcy { get; set; }
        public string GOExpHist_Entry12Fund { get; set; }
        public string GOExpHist_Entry12CheckNo { get; set; }
        public string GOExpHist_Entry12Available { get; set; }
        public string GOExpHist_Entry12AdvcPrnt { get; set; }
        public string GOExpHist_Entry12Details { get; set; }
        public string GOExpHist_Entry12Entity { get; set; }
        public string GOExpHist_Entry12Division { get; set; }
        public string GOExpHist_Entry12InterAmt { get; set; }
        public string GOExpHist_Entry12InterRate { get; set; }
        public string GOExpHist_Entry21Type { get; set; }
        public string GOExpHist_Entry21IbfCode { get; set; }
        public string GOExpHist_Entry21Ccy { get; set; }
        public string GOExpHist_Entry21Amt { get; set; }
        public string GOExpHist_Entry21Cust { get; set; }
        public string GOExpHist_Entry21Actcde { get; set; }
        public string GOExpHist_Entry21ActType { get; set; }
        public string GOExpHist_Entry21ActNo { get; set; }
        public string GOExpHist_Entry21ExchRate { get; set; }
        public string GOExpHist_Entry21ExchCcy { get; set; }
        public string GOExpHist_Entry21Fund { get; set; }
        public string GOExpHist_Entry21CheckNo { get; set; }
        public string GOExpHist_Entry21Available { get; set; }
        public string GOExpHist_Entry21AdvcPrnt { get; set; }
        public string GOExpHist_Entry21Details { get; set; }
        public string GOExpHist_Entry21Entity { get; set; }
        public string GOExpHist_Entry21Division { get; set; }
        public string GOExpHist_Entry21InterAmt { get; set; }
        public string GOExpHist_Entry21InterRate { get; set; }
        public string GOExpHist_Entry22Type { get; set; }
        public string GOExpHist_Entry22IbfCode { get; set; }
        public string GOExpHist_Entry22Ccy { get; set; }
        public string GOExpHist_Entry22Amt { get; set; }
        public string GOExpHist_Entry22Cust { get; set; }
        public string GOExpHist_Entry22Actcde { get; set; }
        public string GOExpHist_Entry22ActType { get; set; }
        public string GOExpHist_Entry22ActNo { get; set; }
        public string GOExpHist_Entry22ExchRate { get; set; }
        public string GOExpHist_Entry22ExchCcy { get; set; }
        public string GOExpHist_Entry22Fund { get; set; }
        public string GOExpHist_Entry22CheckNo { get; set; }
        public string GOExpHist_Entry22Available { get; set; }
        public string GOExpHist_Entry22AdvcPrnt { get; set; }
        public string GOExpHist_Entry22Details { get; set; }
        public string GOExpHist_Entry22Entity { get; set; }
        public string GOExpHist_Entry22Division { get; set; }
        public string GOExpHist_Entry22InterAmt { get; set; }
        public string GOExpHist_Entry22InterRate { get; set; }
        public string GOExpHist_Entry31Type { get; set; }
        public string GOExpHist_Entry31IbfCode { get; set; }
        public string GOExpHist_Entry31Ccy { get; set; }
        public string GOExpHist_Entry31Amt { get; set; }
        public string GOExpHist_Entry31Cust { get; set; }
        public string GOExpHist_Entry31Actcde { get; set; }
        public string GOExpHist_Entry31ActType { get; set; }
        public string GOExpHist_Entry31ActNo { get; set; }
        public string GOExpHist_Entry31ExchRate { get; set; }
        public string GOExpHist_Entry31ExchCcy { get; set; }
        public string GOExpHist_Entry31Fund { get; set; }
        public string GOExpHist_Entry31CheckNo { get; set; }
        public string GOExpHist_Entry31Available { get; set; }
        public string GOExpHist_Entry31AdvcPrnt { get; set; }
        public string GOExpHist_Entry31Details { get; set; }
        public string GOExpHist_Entry31Entity { get; set; }
        public string GOExpHist_Entry31Division { get; set; }
        public string GOExpHist_Entry31InterAmt { get; set; }
        public string GOExpHist_Entry31InterRate { get; set; }
        public string GOExpHist_Entry32Type { get; set; }
        public string GOExpHist_Entry32IbfCode { get; set; }
        public string GOExpHist_Entry32Ccy { get; set; }
        public string GOExpHist_Entry32Amt { get; set; }
        public string GOExpHist_Entry32Cust { get; set; }
        public string GOExpHist_Entry32Actcde { get; set; }
        public string GOExpHist_Entry32ActType { get; set; }
        public string GOExpHist_Entry32ActNo { get; set; }
        public string GOExpHist_Entry32ExchRate { get; set; }
        public string GOExpHist_Entry32ExchCcy { get; set; }
        public string GOExpHist_Entry32Fund { get; set; }
        public string GOExpHist_Entry32CheckNo { get; set; }
        public string GOExpHist_Entry32Available { get; set; }
        public string GOExpHist_Entry32AdvcPrnt { get; set; }
        public string GOExpHist_Entry32Details { get; set; }
        public string GOExpHist_Entry32Entity { get; set; }
        public string GOExpHist_Entry32Division { get; set; }
        public string GOExpHist_Entry32InterAmt { get; set; }
        public string GOExpHist_Entry32InterRate { get; set; }
        public string GOExpHist_Entry41Type { get; set; }
        public string GOExpHist_Entry41IbfCode { get; set; }
        public string GOExpHist_Entry41Ccy { get; set; }
        public string GOExpHist_Entry41Amt { get; set; }
        public string GOExpHist_Entry41Cust { get; set; }
        public string GOExpHist_Entry41Actcde { get; set; }
        public string GOExpHist_Entry41ActType { get; set; }
        public string GOExpHist_Entry41ActNo { get; set; }
        public string GOExpHist_Entry41ExchRate { get; set; }
        public string GOExpHist_Entry41ExchCcy { get; set; }
        public string GOExpHist_Entry41Fund { get; set; }
        public string GOExpHist_Entry41CheckNo { get; set; }
        public string GOExpHist_Entry41Available { get; set; }
        public string GOExpHist_Entry41AdvcPrnt { get; set; }
        public string GOExpHist_Entry41Details { get; set; }
        public string GOExpHist_Entry41Entity { get; set; }
        public string GOExpHist_Entry41Division { get; set; }
        public string GOExpHist_Entry41InterAmt { get; set; }
        public string GOExpHist_Entry41InterRate { get; set; }
        public string GOExpHist_Entry42Type { get; set; }
        public string GOExpHist_Entry42IbfCode { get; set; }
        public string GOExpHist_Entry42Ccy { get; set; }
        public string GOExpHist_Entry42Amt { get; set; }
        public string GOExpHist_Entry42Cust { get; set; }
        public string GOExpHist_Entry42Actcde { get; set; }
        public string GOExpHist_Entry42ActType { get; set; }
        public string GOExpHist_Entry42ActNo { get; set; }
        public string GOExpHist_Entry42ExchRate { get; set; }
        public string GOExpHist_Entry42ExchCcy { get; set; }
        public string GOExpHist_Entry42Fund { get; set; }
        public string GOExpHist_Entry42CheckNo { get; set; }
        public string GOExpHist_Entry42Available { get; set; }
        public string GOExpHist_Entry42AdvcPrnt { get; set; }
        public string GOExpHist_Entry42Details { get; set; }
        public string GOExpHist_Entry42Entity { get; set; }
        public string GOExpHist_Entry42Division { get; set; }
        public string GOExpHist_Entry42InterAmt { get; set; }
        public string GOExpHist_Entry42InterRate { get; set; }
        public int ExpenseEntryID { get; set; }
        public int ExpenseDetailID { get; set; }
    }
}
