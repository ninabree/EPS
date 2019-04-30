using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class NewVendorListViewModel
    {
        public List<NewVendorViewModel> NewVendorVM { get; set; }
    }
    public class NewVendorViewModel
    {
        [Display(Name = "Vendor Name")]
        [NotNullValidations, TextValidation]
        public string Vendor_Name { get; set; }
        [Display(Name = "Vendor TIN")]
        [NotNullValidations, TextValidation]
        public string Vendor_TIN { get; set; }
        [Display(Name = "Vendor Address")]
        [NotNullValidations, TextValidation]
        public string Vendor_Address { get; set; }
     }
}
