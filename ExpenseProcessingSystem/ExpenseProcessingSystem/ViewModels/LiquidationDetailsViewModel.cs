using ExpenseProcessingSystem.ViewModels.Entry;
using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.Models;

namespace ExpenseProcessingSystem.ViewModels
{
    public class LiquidationDetailsViewModel
    {
        public int EntryDetailsID { get; set; }
        public string GBaseRemarks { get; set; }
        public int accountID { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string accountCode { get; set; }
        public bool fbt { get; set; }
        public int deptID { get; set; }
        public string deptName { get; set; }
        public bool chkVat { get; set; }
        public int vatID { get; set; }
        public float vatValue { get; set; }
        public bool chkEwt { get; set; }
        public int ewtID { get; set; }
        public float ewtValue { get; set; }
        public int ccyID { get; set; }
        public int ccyMasterID { get; set; }
        public string ccyAbbrev { get; set; }
        public float debitGross { get; set; }
        public float credEwt { get; set; }
        public float credCash { get; set; }
        public int dtlSSPayee { get; set; }
        public string dtlSSPayeeName { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int duration { get; set; }
        public int modalInputFlag { get; set; }
        public int liqInputFlag { get; set; }
        public string screenCode { get; set; }
        public List<EntryGbaseRemarksViewModel> gBaseRemarksDetails { get; set; }
        public List<LiquidationCashBreakdown> cashBreakdown { get; set; }
        [EmptyLiquidationCashBreakdown("ccyAbbrev", "liqInputFlag")]
        public List<LiquidationCashBreakdown> liqCashBreakdown { get; set; }
        [EmptyLiquidationInterEntity("ccyAbbrev", "liqInputFlag")]
        public List<LiquidationInterEntity> liqInterEntity { get; set; }

        public LiquidationDetailsViewModel()
        {
            gBaseRemarksDetails = new List<EntryGbaseRemarksViewModel>();
            cashBreakdown = new List<LiquidationCashBreakdown>();
            liqCashBreakdown = new List<LiquidationCashBreakdown>();
            liqInterEntity = new List<LiquidationInterEntity>();
        }
    }

    public class LiquidationCashBreakdown
    {
        public double cashDenomination { get; set; }
        public int cashNoPC { get; set; }
        public double cashAmount { get; set; }
    }

    public class LiquidationInterEntity
    {
        public string Liq_DebitCred_1_1 { get; set; }
        public int Liq_AccountID_1_1 { get; set; }
        public double Liq_InterRate_1_1 { get; set; }
        public int Liq_CCY_1_1 { get; set; }
        public double Liq_Amount_1_1 { get; set; }
        public string Liq_DebitCred_1_2 { get; set; }
        public int Liq_AccountID_1_2 { get; set; }
        public double Liq_InterRate_1_2 { get; set; }
        public int Liq_CCY_1_2 { get; set; }
        public double Liq_Amount_1_2 { get; set; }
        public string Liq_DebitCred_2_1 { get; set; }
        public int Liq_AccountID_2_1 { get; set; }
        public double Liq_InterRate_2_1 { get; set; }
        public int Liq_CCY_2_1 { get; set; }
        public double Liq_Amount_2_1 { get; set; }
        public string Liq_DebitCred_2_2 { get; set; }
        public int Liq_AccountID_2_2 { get; set; }
        public double Liq_InterRate_2_2 { get; set; }
        public int Liq_CCY_2_2 { get; set; }
        public double Liq_Amount_2_2 { get; set; }
        public string Liq_DebitCred_3_1 { get; set; }
        public int Liq_AccountID_3_1 { get; set; }
        public double Liq_InterRate_3_1 { get; set; }
        public int Liq_CCY_3_1 { get; set; }
        public double Liq_Amount_3_1 { get; set; }
        public string Liq_DebitCred_3_2 { get; set; }
        public int Liq_AccountID_3_2 { get; set; }
        public double Liq_InterRate_3_2 { get; set; }
        public int Liq_CCY_3_2 { get; set; }
        public double Liq_Amount_3_2 { get; set; }
        public double Liq_Tax_Rate { get; set; }
    }
}
