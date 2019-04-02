using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewCheckListViewModel
    {
        public List<NewCheckViewModel> NewCheckVM { get; set; }
    }
    public class NewCheckViewModel
    {
        [Display(Name = "Input Date")]
        [NotNullValidations, DateValidation]
        public DateTime Check_Input_Date { get; set; }
        [Display(Name = "Series From")]
        [NotNullValidations, TextValidation]
        public string Check_Series_From { get; set; }
        [Display(Name = "Series To")]
        [NotNullValidations, TextValidation]
        public string Check_Series_To { get; set; }
        [Display(Name = "Bank Information")]
        [NotNullValidations, TextValidation]
        public string Check_Bank_Info { get; set; }
    }
}
