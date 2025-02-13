﻿using ExpenseProcessingSystem.Models;
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
        [Display(Name = "Account")]
        public int account { get; set; }
        public int creditAccount1 { get; set; }
        public int creditAccount2 { get; set; }
        public string account_Name { get; set; }
        [Display(Name = "Inter Entity")]
        public bool inter_entity { get; set; }
        [Display(Name = "FBT")]
        public bool fbt { get; set; }
        [Display(Name = "Department")]
        public int dept { get; set; }
        public string dept_Name { get; set; }
        [Display(Name = "VAT Checkbox")]
        public bool chkVat { get; set; }
        [Display(Name = "VAT")]
        public int vat { get; set; }
        public string vat_Name { get; set; }
        [Display(Name = "EWT Checkbox")]
        public bool chkEwt { get; set; }
        [Display(Name = "EWT")]
        public int ewt { get; set; }
        public string ewt_Name { get; set; }
        [Display(Name = "EWT - Tax Payor's Name")]
        public int ewt_Payor_Name_ID { get; set; }
        public string ewt_Payor_Name { get; set; }
        public List<DMTRModel> vendTRList { get; set; }
        public List<DMVATModel> vendVATList { get; set; }
        [NotNullValidations]
        [Display(Name = "Currency")]
        public int ccy { get; set; }
        public string ccy_Name { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Debit - Gross Amount")]
        public decimal debitGross { get; set; }
        [NotNullValidations]
        [Display(Name = "Credit - EWT Amount")]
        public decimal credEwt { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Credit - Cash")]
        public decimal credCash { get; set; }
        public int dtlID { get; set; }
        //[ListValidation("inter_entity")]
        [Display(Name = "Inter-Entity Details")]
        public DDVInterEntityViewModel interDetails { get; set; }
        public List<EntryGbaseRemarksViewModel> gBaseRemarksDetails { get; set; }

        public EntryDDVViewModel()
        {
            gBaseRemarksDetails = new List<EntryGbaseRemarksViewModel>();
        }

    }
    public class EntryDDVViewModelList
    {
        public int entryID { get; set; }
        public SysValViewModel systemValues { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime expenseDate { get; set; }
        public int vendor { get; set; }
        public string vendor_Name { get; set; }
        public int payee_type { get; set; }
        public string payee_type_Name { get; set; }
        public string expenseYear { get; set; }
        public string expenseId { get; set; }
        public string checkNo { get; set; }
        public int statusID { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public int approver_id { get; set; }
        public int verifier_1_id { get; set; }
        public int verifier_2_id { get; set; }
        public int maker { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public List<EntryDDVViewModel> EntryDDV { get; set; }
        public List<cvBirForm> birForms { get; set; }

        public int yenCurrID { get; set; }
        public int yenCurrMasterID { get; set; }
        public string yenAbbrev { get; set; }

        public EntryDDVViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryDDV = new List<EntryDDVViewModel>();
            birForms = new List<cvBirForm>();
        }
    }
}
