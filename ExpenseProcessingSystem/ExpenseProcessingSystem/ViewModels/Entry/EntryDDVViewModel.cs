using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class EntryDDVViewModel
    {
        [NotNullValidations, TextValidation]
        [Display(Name = "GBase Remarks")]
        public string GBaseRemarks { get; set; }
        [NotNullValidations]
        [Display(Name = "Account")]
        public int account { get; set; }
        public string account_Name { get; set; }
        [NotNullValidations]
        [Display(Name = "Inter Entity")]
        public bool inter_entity { get; set; }
        [NotNullValidations]
        [Display(Name = "FBT")]
        public bool fbt { get; set; }
        [NotNullValidations]
        [Display(Name = "Department")]
        public int dept { get; set; }
        public string dept_Name { get; set; }
        [NotNullValidations]
        [Display(Name = "VAT Checkbox")]
        public bool chkVat { get; set; }
        [Display(Name = "VAT")]
        public int vat { get; set; }
        public string vat_Name { get; set; }
        [NotNullValidations]
        [Display(Name = "EWT Checkbox")]
        public bool chkEwt { get; set; }
        [IntegerValidation]
        [Display(Name = "EWT")]
        public int ewt { get; set; }
        public string ewt_Name { get; set; }
        [NotNullValidations]
        [Display(Name = "Currency")]
        public int ccy { get; set; }
        public string ccy_Name { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Debit - Gross Amount")]
        public float debitGross { get; set; }
        [NotNullValidations]
        [Display(Name = "Credit - EWT Amount")]
        public float credEwt { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Credit - Cash")]
        public float credCash { get; set; }
        [FalseValidation("chkEwt"), TextValidation]
        [Display(Name = "EWT - Tax Payor's Name")]
        public string ewtPayorName { get; set; }
        [ListValidation("inter_entity")]
        [Display(Name = "Inter-Entity Details")]
        public List<DDVInterEntityViewModel> interDetails { get; set; }
        public List<EntryGbaseRemarksViewModel> gBaseRemarksDetails { get; set; }

        public EntryDDVViewModel()
        {
            gBaseRemarksDetails = new List<EntryGbaseRemarksViewModel>();
            interDetails = new List<DDVInterEntityViewModel>();
        }

    }
    public class EntryDDVViewModelList
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
        public List<EntryDDVViewModel> EntryDDV { get; set; }

        public EntryDDVViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryDDV = new List<EntryDDVViewModel>();
        }
    }
}
