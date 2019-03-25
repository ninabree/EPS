using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class NewPayeeListViewModel
    {
        public List<NewPayeeViewModel> NewPayeeVM { get; set; }
    }
    public class NewPayeeViewModel
    {
        [Display(Name = "Payee Name")]
        [NotNullValidations, TextValidation]
        public string Payee_Name { get; set; }
        [Display(Name = "Payee TIN")]
        [NotNullValidations, TextValidation]
        public string Payee_TIN { get; set; }
        [Display(Name = "Payee Address")]
        [NotNullValidations, TextValidation]
        public string Payee_Address { get; set; }
        [Display(Name = "Payee Type")]
        [NotNullValidations, TextValidation]
        public string Payee_Type { get; set; }
        [Display(Name = "Payee No")]
        [NotNullValidations, IntegerValidation]
        public int Payee_No { get; set; }
     }
}
