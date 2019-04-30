using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewCurrListViewModel
    {
        public List<NewCurrViewModel> NewCurrVM { get; set; }
    }
    public class NewCurrViewModel
    {
        [Display(Name = "Currency Name")]
        [NotNullValidations, TextValidation]
        public string Curr_Name { get; set; }
        [Display(Name = "Currency CCY ABBR")]
        [NotNullValidations, TextValidation]
        public string Curr_CCY_ABBR { get; set; }
    }
}
