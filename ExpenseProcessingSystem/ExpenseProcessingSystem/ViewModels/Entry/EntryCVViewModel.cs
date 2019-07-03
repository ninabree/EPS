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
        [NotNullValidations]
        [Display(Name = "Account")]
        public int account { get; set; }
        [NotNullValidations]
        [Display(Name = "FBT")]
        public bool fbt { get; set; }
        public int fbtID { get; set; }
        [NotNullValidations]
        [Display(Name = "Department")]
        public int dept { get; set; }
        [NotNullValidations]
        [Display(Name = "VAT Checkbox")]
        public bool chkVat { get; set; }
        [Display(Name = "VAT")]
        public int vat { get; set; }
        [NotNullValidations]
        [Display(Name = "EWT Checkbox")]
        public bool chkEwt { get; set; }
        [IntegerValidation]
        [Display(Name = "EWT")]
        public int ewt { get; set; }
        [Display(Name = "Currency")]
        public int ccy { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Debit - Gross Amount")]
        public float debitGross { get; set; }
        [NotNullValidations]
        [Display(Name = "Credit - EWT Amount")]
        public float credEwt { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Credit - Cash")]
        public float credCash { get; set; }
        public int dtlSSPayee { get; set; }
        [Display(Name = "Month")]
        public int month { get; set; }
        [Display(Name = "Day")]
        public int day { get; set; }
        [Display(Name = "Duration")]
        public int duration { get; set; }
        public int modalInputFlag { get; set; }
        public string screenCode { get; set; }
        public string ccyAbbrev { get; set; }
        public int expenseDtlID { get; set; }
        public List<amortizationSchedule> amtDetails { get; set; }
        public List<EntryGbaseRemarksViewModel> gBaseRemarksDetails { get; set; }
        [EmptyCashBreakdown("screenCode", "ccyAbbrev")]
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
        public double cashDenomination { get; set; }
        public int cashNoPC { get; set; }
        public double cashAmount { get; set; }
    }
    public class EntryCVViewModelList
    {
        public int entryID { get; set; }
        public SysValViewModel systemValues { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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
        public DateTime createdDate { get; set; }
        public List<EntryCVViewModel> EntryCV { get; set; }
        public temp template { get; set; }

        public EntryCVViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryCV = new List<EntryCVViewModel>();
            template = new temp();
        }
    }

    public class temp
    {
        [Display(Name = "GBase Remarks")]
        public string GBaseRemarks { get; set; }
        [Display(Name = "Account")]
        public int account { get; set; }
        [Display(Name = "FBT")]
        public bool fbt { get; set; }
        [Display(Name = "Department")]
        public int dept { get; set; }
        [Display(Name = "VAT Checkbox")]
        public bool chkVat { get; set; }
        [Display(Name = "VAT")]
        public int vat { get; set; }
        [Display(Name = "EWT Checkbox")]
        public bool chkEwt { get; set; }
        [Display(Name = "EWT")]
        public int ewt { get; set; }
        [Display(Name = "Currency")]
        public int ccy { get; set; }
        [Display(Name = "Debit - Gross Amount")]
        public float debitGross { get; set; }
        [Display(Name = "Credit - EWT Amount")]
        public float credEwt { get; set; }
        [Display(Name = "Credit - Cash")]
        public float credCash { get; set; }
        [Display(Name = "Month")]
        public int month { get; set; }
        [Display(Name = "Day")]
        public int day { get; set; }
        [Display(Name = "Duration")]
        public int duration { get; set; }
        public int modalInputFlag { get; set; }
        public string screenCode { get; set; }
        public List<amortizationSchedule> amtDetails { get; set; }
        public List<EntryGbaseRemarksViewModel> gBaseRemarksDetails { get; set; }


        public temp()
        {
            gBaseRemarksDetails = new List<EntryGbaseRemarksViewModel>();
            amtDetails = new List<amortizationSchedule>();
        }
    }
}
