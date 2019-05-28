using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class EntryNCViewModel
    {
        [NotNullValidations, TextValidation]
        [Display(Name = "GBase Remarks")]
        public string GBaseRemarks { get; set; }
        [NotNullValidations]
        [Display(Name = "Account")]
        public int account { get; set; }
        [NotNullValidations]
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
        //public List<amortizationSchedule> amtDetails { get; set; }
        //public List<EntryGbaseRemarksViewModel> gBaseRemarksDetails { get; set; }
        //[NotNullValidations]
        //[Display(Name = "Cash Breakdown")]
        //public List<CashBreakdown> cashBreakdown { get; set; }


        public EntryNCViewModel()
        {
            //gBaseRemarksDetails = new List<EntryGbaseRemarksViewModel>();
            //amtDetails = new List<amortizationSchedule>();
            //cashBreakdown = new List<CashBreakdown>();
        }
    }
    //public class amortizationSchedule
    //{
    //    public DateTime amtDate { get; set; }
    //    public float amtAmount { get; set; }
    //}

    //public class CashBreakdown
    //{
    //    public double cashDenimination { get; set; }
    //    public double cashNoPC { get; set; }
    //    public double cashAmount { get; set; }
    //}
    public class EntryNCViewModelList
    {
        public int entryID { get; set; }
        public SysValViewModel systemValues { get; set; }
        public DateTime expenseDate { get; set; }
        public int category_of_entry { get; set; }
        public int statusID { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public int maker { get; set; }
        public List<EntryNCViewModel> EntryNC { get; set; }

        public EntryNCViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryNC = new List<EntryNCViewModel>();
        }
    }
}
