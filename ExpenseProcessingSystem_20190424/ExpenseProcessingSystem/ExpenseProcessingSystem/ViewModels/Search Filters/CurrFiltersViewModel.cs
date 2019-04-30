using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class CurrFiltersViewModel
    {
        [Display(Name = "Currency Name")]
        public string CF_Name { get; set; }
        [Display(Name = "Currency CCY Code")]
        public string CF_CCY_Code { get; set; }
        [Display(Name = "Creator Name")]
        public string CF_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string CF_Approver_Name { get; set; }
        [Display(Name = "Currency Status")]
        public string CF_Status { get; set; }
    }
}
