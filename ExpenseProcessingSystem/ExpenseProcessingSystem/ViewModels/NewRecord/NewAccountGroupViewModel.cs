using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewAccountGroupListViewModel
    {
        public List<NewAccountGroupViewModel> NewAccountGroupVM { get; set; }
    }
    public class NewAccountGroupViewModel
    {
        [Display(Name = "Account Group Name")]
        [NotNullValidations, TextValidation]
        public string AccountGroup_Name { get; set; }
        [Display(Name = "Account Group Code")]
        [NotNullValidations, TextValidation]
        public string AccountGroup_Code { get; set; }
    }
}