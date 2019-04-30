using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class VATFiltersViewModel
    {
        [Display(Name = "VAT Name")]
        public string VF_Name { get; set; }
        [Display(Name = "VAT Rate")]
        public string VF_Rate { get; set; }
        [Display(Name = "Creator Name")]
        public string VF_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string VF_Approver_Name { get; set; }
        [Display(Name = "VAT Status")]
        public string VF_Status { get; set; }
    }
}
