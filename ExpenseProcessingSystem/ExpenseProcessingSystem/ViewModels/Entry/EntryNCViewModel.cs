using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Services.Validations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class EntryNCViewModel
    {
        [NotNullValidations]
        [Display(Name = "Category of Entry")]
        public int NC_Category_ID { get; set; }
        public string NC_Category_Name { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Debit - Total Amount")]
        public decimal NC_DebitAmt { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Credit - Total Amount")]
        public decimal NC_CredAmt { get; set; }
        //[NotNullValidations, AmountValidation]
        [Display(Name = "Computer Suspense: Debit - Total Amount")]
        public decimal NC_CS_DebitAmt { get; set; }
        //[NotNullValidations, AmountValidation]
        [Display(Name = "Computer Suspense: Credit - Total Amount")]
        public decimal NC_CS_CredAmt { get; set; }
        [Display(Name = "Computer Suspense: Remarks Period")]
        public string NC_CS_Period { get; set; }
        //[NotNullValidations, AmountValidation]
        [Display(Name = "Computer Suspense: Total Amount")]
        public decimal NC_CS_TotalAmt { get; set; }
        //[NotNullValidations, AmountValidation]
        [Display(Name = "Inter-Entity: Debit - Total Amount")]
        public decimal NC_IE_DebitAmt { get; set; }
        //[NotNullValidations, AmountValidation]
        [Display(Name = "Inter-Entity: Credit - Total Amount")]
        public decimal NC_IE_CredAmt { get; set; }
        //[NotNullValidations, AmountValidation]
        [Display(Name = "Inter-Entity: Total Amount")]
        public decimal NC_IE_TotalAmt { get; set; }
        //[NotNullValidations, AmountValidation]
        [Display(Name = "Total Amount")]
        public decimal NC_TotalAmt { get; set; }
        public int NC_ID { get; set; }
        public List<ExpenseEntryNCDtlViewModel> ExpenseEntryNCDtls { get; set; }
        public List<ExpenseEntryNCDtlViewModel> ExpenseEntryNCDtls_CDD { get; set; }


        public EntryNCViewModel()
        {
            ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>();
            ExpenseEntryNCDtls_CDD = new List<ExpenseEntryNCDtlViewModel>();
        }
    }
    public class EntryNCViewModelList
    {
        public int entryID { get; set; }
        public SysValViewModel systemValues { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime expenseDate { get; set; }
        public int statusID { get; set; }
        public int taxaccID { get; set; }
        public int usdCashAccID { get; set; }
        public int yenCashAccID { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public int approver_id { get; set; }
        public int verifier_1_id { get; set; }
        public int verifier_2_id { get; set; }
        public int maker { get; set; }
        public int amortizationID { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        [BalancedValidations, RemarksLimitValidations, AccountNotNullValidations, TaxAccountValidations("taxaccID"), DebitCreditNotNullValidations, JSPValidations]
        public EntryNCViewModel EntryNC { get; set; }
        public List<SelectListItem> category_of_entry { get; set; }
        public List<SelectListItem> accountList { get; set; }
        public List<SelectListItem> currList { get; set; }
        public List<SelectListItem> vendorList { get; set; }
        public List<SelectListItem> trList { get; set; }
        public List<CONSTANT_NC_VALS> accList { get; set; }
        public List<CONSTANT_NC_VALS> consCurrList { get; set; }
        public List<cvBirForm> birForms { get; set; }

        public EntryNCViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryNC = new EntryNCViewModel();
            birForms = new List<cvBirForm>();
        }
    }
}
