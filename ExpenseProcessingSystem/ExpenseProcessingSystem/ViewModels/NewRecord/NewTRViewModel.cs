using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewTRListViewModel
    {
        public List<NewTRViewModel> NewTRVM { get; set; }
    }
    public class NewTRViewModel
    {
        [Display(Name = "Withholding Tax Title")]
        [NotNullValidations, TextValidation]
        public string TR_WT_Title { get; set; }
        [Display(Name = "Nature")]
        [NotNullValidations, TextValidation]
        public string TR_Nature { get; set; }
        [Display(Name = "Tax Rate")]
        [NotNullValidations, IntegerValidation]
        public int TR_Tax_Rate { get; set; }
        [Display(Name = "ATC")]
        [NotNullValidations, TextValidation]
        public string TR_ATC { get; set; }
        [Display(Name = "Nature of Income Payment")]
        [NotNullValidations, TextValidation]
        public string TR_Nature_Income_Payment { get; set; }
    }
}
