using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class ExpenseEntryNCDtlViewModel
    {
        public int ExpNCDtl_ID { get; set; }
        //[NotNullValidations]
        [Display(Name = "Description")]
        public string ExpNCDtl_Remarks_Desc { get; set; }
        [Display(Name = "Period")]
        public string ExpNCDtl_Remarks_Period { get; set; }
        [Display(Name = "Vendor")]
        public int ExpNCDtl_Vendor_ID { get; set; }
        public string ExpNCDtl_Vendor_Name { get; set; }
        [Display(Name = "Tax Rate")]
        public int ExpNCDtl_TR_ID { get; set; }
        public string ExpNCDtl_TR_Title { get; set; }
        [Display(Name = "Tax Based Amount")]
        public decimal ExpNCDtl_TaxBasedAmt { get; set; }
        public List<ExpenseEntryNCDtlAccViewModel> ExpenseEntryNCDtlAccs { get; set; }


        public ExpenseEntryNCDtlViewModel()
        {
            ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>();
        }
    }
}