using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewEWTListViewModel
    {
        public List<NewEWTViewModel> NewEWTVM { get; set; }
    }
    public class NewEWTViewModel
    {
        [Display(Name = "EWT Nature")]
        [NotNullValidations, TextValidation]
        public string EWT_Nature { get; set; }
        [Display(Name = "EWT Tax Rate")]
        [NotNullValidations, IntegerValidation]
        public int EWT_Tax_Rate { get; set; }
        [Display(Name = "ATC")]
        [NotNullValidations, TextValidation]
        public string EWT_ATC { get; set; }
        [Display(Name = "EWT Tax Rate Description")]
        [NotNullValidations, TextValidation]
        public string EWT_Tax_Rate_Desc { get; set; }
    }
}
