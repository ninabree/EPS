using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewFBTListViewModel
    {
        public List<NewFBTViewModel> NewFBTVM { get; set; }
    }
    public class NewFBTViewModel
    {
        [Display(Name = "FBT Name")]
        [NotNullValidations, TextValidation]
        public string FBT_Name { get; set; }
        [Display(Name = "FBT Formula")]
        [NotNullValidations, TextValidation]
        public string FBT_Formula { get; set; }
        [Display(Name = "FBT Tax Rate")]
        [NotNullValidations, IntegerValidation]
        public int FBT_Tax_Rate { get; set; }
    }
}
