using ExpenseProcessingSystem.Services.Validations;
using ExpenseProcessingSystem.ViewModels.Entry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class EntryCVViewModel
    {
        [NotNullValidations, TextValidation]
        [Display(Name = "GBase Remarks")]
        public string GBaseRemarks { get; set; }
        public int account { get; set; }
        public bool fbt { get; set; }
        public int dept { get; set; }
        public bool chkVat { get; set; }
        [IntegerValidation]
        public float vat { get; set; }
        public bool chkEwt { get; set; }
        //[IntegerValidation,FalseValidation("chkEwt")]
        public int ewt { get; set; }
        public int ccy { get; set; }
        public float debitGross { get; set; }
        public float credEwt { get; set; }
        public float credCash { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int duration { get; set; }
        public int modalInputFlag { get; set; }
        public List<amortizationSchedule> amtDetails { get; set; }
        public List<EntryGbaseRemarksViewModel> gBaseRemarksDetails { get; set; }
        [NotNullValidations]
        [Display(Name = "Cash Breakdown")]
        public List<CashBreakdown> cashBreakdown { get; set; }


        public EntryCVViewModel()
        {
            gBaseRemarksDetails = new List<EntryGbaseRemarksViewModel>();
            amtDetails = new List<amortizationSchedule>();
            cashBreakdown = new List<CashBreakdown>();
        }
    }

    public class amortizationSchedule
    {
        public DateTime amtDate { get; set; }
        public float amtAmount { get; set; }
    }

    public class CashBreakdown
    {
        public double cashDenimination { get; set; }
        public double cashNoPC { get; set; }
        public double cashAmount { get; set; }
    }
    public class EntryCVViewModelList
    {
        public int entryID { get; set; }
        public SysValViewModel systemValues { get; set; }
        public DateTime expenseDate { get; set; }
        public int vendor { get; set; }
        public string expenseYear { get; set; }
        public string expenseId { get; set; }
        public string checkNo { get; set; }
        public int statusID { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public int maker { get; set; }
        public List<EntryCVViewModel> EntryCV { get; set; }

        public EntryCVViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryCV = new List<EntryCVViewModel>();
        }
    }
}
