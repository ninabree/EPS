using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class FBTFiltersViewModel
    {
        [Display(Name = "FBT Name")]
        public string FF_Name { get; set; }
        [Display(Name = "FBT Formula")]
        public string FF_Formula { get; set; }
        [Display(Name = "FBT Tax Rate")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid Tax Rate")]
        public int FF_Tax_Rate { get; set; }
        [Display(Name = "Creator Name")]
        public string FF_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string FF_Approver_Name { get; set; }
        [Display(Name = "FBT Status")]
        public string FF_Status { get; set; }
    }
}
