using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewVATListViewModel
    {
        public List<NewVATViewModel> NewVATVM { get; set; }
    }
    public class NewVATViewModel
    {
        [Display(Name = "VAT Name")]
        [NotNullValidations, TextValidation]
        public string VAT_Name { get; set; }
        [Display(Name = "VAT Rate")]
        [NotNullValidations, TextValidation]
        public float VAT_Rate { get; set; }
    }
}
