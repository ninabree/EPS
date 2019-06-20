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
        //[NotNullValidations]
        [Display(Name = "Description")]
        public string ExpNCDtl_Remarks_Desc { get; set; }
        [Display(Name = "Period From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpNCDtl_Remarks_Period_From { get; set; }
        [Display(Name = "Period To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpNCDtl_Remarks_Period_To { get; set; }
        public string ExpNCDtl_Period_Duration { get; set; }
        public List<ExpenseEntryNCDtlAccViewModel> ExpenseEntryNCDtlAccs { get; set; }


        public ExpenseEntryNCDtlViewModel()
        {
            ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>();
        }
    }
}