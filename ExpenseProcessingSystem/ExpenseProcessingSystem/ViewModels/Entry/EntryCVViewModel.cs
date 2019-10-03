using ExpenseProcessingSystem.Models;
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
        [Display(Name = "Account")]
        public int account { get; set; }
        public int creditAccount1 { get; set; }
        public int creditAccount2 { get; set; }
        [Display(Name = "FBT")]
        public bool fbt { get; set; }
        public int fbtID { get; set; }
        [Display(Name = "Department")]
        public int dept { get; set; }
        [Display(Name = "VAT Checkbox")]
        public bool chkVat { get; set; }
        [Display(Name = "VAT")]
        public int vat { get; set; }
        [Display(Name = "EWT Checkbox")]
        public bool chkEwt { get; set; }
        [IntegerValidation]
        [Display(Name = "EWT")]
        public int ewt { get; set; }
        [Display(Name = "Currency")]
        public int ccy { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Debit - Gross Amount")]
        public decimal debitGross { get; set; }
        [NotNullValidations]
        [Display(Name = "Credit - EWT Amount")]
        public decimal credEwt { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Credit - Cash")]
        public decimal credCash { get; set; }
        public int dtlSSPayee { get; set; }
        public int dtl_Ewt_Payor_Name_ID { get; set; }
        [Display(Name = "Month")]
        public int month { get; set; }
        [Display(Name = "Day")]
        public int day { get; set; }
        [Display(Name = "Duration")]
        public int duration { get; set; }
        [Display(Name = "Duration")]
        public int amorAcc { get; set; }
        public int modalInputFlag { get; set; }
        public string screenCode { get; set; }
        public string ccyAbbrev { get; set; }
        public int ccyMasterID { get; set; }
        public int expenseDtlID { get; set; }
        public List<DMTRModel> vendTRList { get; set; }
        public List<DMVATModel> vendVATList { get; set; }
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
        public decimal amtAmount { get; set; }
    }

    public class CashBreakdown
    {
        public decimal cashDenomination { get; set; }
        public int cashNoPC { get; set; }
        public decimal cashAmount { get; set; }
    }
    public class EntryCVViewModelList
    {
        public int entryID { get; set; }
        public SysValViewModel systemValues { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime expenseDate { get; set; }
        public int selectedPayee { get; set; }
        public string vendor_Name { get; set; }
        public int payee_type { get; set; }
        public string payee_type_Name { get; set; }
        public string expenseYear { get; set; }
        public string expenseId { get; set; }
        public int checkId { get; set; }
        public string checkNo { get; set; }
        public int statusID { get; set; }
        public string status { get; set; }
        public int expenseType { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public int approver_id { get; set; }
        public int verifier_1_id { get; set; }
        public int verifier_2_id { get; set; }
        public int maker { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public DateTime createdDate { get; set; }
        public List<EntryCVViewModel> EntryCV { get; set; }
        public temp template { get; set; }
        public List<cvBirForm> birForms { get; set; }

        public EntryCVViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryCV = new List<EntryCVViewModel>();
            template = new temp();
            birForms = new List<cvBirForm>();
        }

        public int phpCurrID { get; set; }
        public int phpCurrMasterID { get; set; }
        public string phpAbbrev { get; set; }
        public int yenCurrID { get; set; }
        public int yenCurrMasterID { get; set; }
        public string yenAbbrev { get; set; }
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
        public decimal debitGross { get; set; }
        [Display(Name = "Credit - EWT Amount")]
        public decimal credEwt { get; set; }
        [Display(Name = "Credit - Cash")]
        public decimal credCash { get; set; }
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

    public class cvBirForm
    {
        public decimal amount { get; set; }
        public int vat { get; set; } //id
        public int ewt { get; set; } //id
        public int vendor { get; set; } //name
        public string approver { get; set; } //id
        public DateTime date { get; set; }
    }

}
