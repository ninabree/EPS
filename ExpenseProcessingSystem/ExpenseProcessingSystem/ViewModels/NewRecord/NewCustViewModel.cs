using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewCustListViewModel
    {
        public List<NewCustViewModel> NewCustVM { get; set; }
    }
    public class NewCustViewModel
    {
        [Display(Name = "Customer Name")]
        [NotNullValidations, TextValidation]
        public string Cust_Name { get; set; }
        [Display(Name = "Customer Abbr")]
        [NotNullValidations, TextValidation]
        public string Cust_Abbr { get; set; }
        [Display(Name = "Customer Number")]
        [NotNullValidations, TextValidation]
        public string Cust_No { get; set; }
    }
}
