using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class DeptFiltersViewModel
    {
        [Display(Name = "Dept Name")]
        public string DF_Name { get; set; }
        [Display(Name = "Dept Code")]
        public string DF_Code { get; set; }
        [Display(Name = "Budget Unit")]
        public string DF_Budget_Unit { get; set; }
        [Display(Name = "Dept Creator Name")]
        public string DF_Creator_Name { get; set; }
        [Display(Name = "Dept Approver Name")]
        public string DF_Approver_Name { get; set; }
        [Display(Name = "Dept Status")]
        public string DF_Status { get; set; }
    }
}
