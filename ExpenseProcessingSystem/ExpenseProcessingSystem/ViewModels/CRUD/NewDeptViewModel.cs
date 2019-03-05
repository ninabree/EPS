using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class NewDeptListViewModel
    {
        public List<NewDeptViewModel> NewDeptVM { get; set; }
    }
    public class NewDeptViewModel
    {
        [Display(Name = "Dept Name")]
        [NotNullValidations, TextValidation]
        public string Dept_Name { get; set; }
        [Display(Name = "Dept Code")]
        [NotNullValidations, TextValidation]
        public string Dept_Code { get; set; }
    }
}
