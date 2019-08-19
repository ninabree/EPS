using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class AccFiltersViewModel
    {
        [Display(Name = "Account Name")]
        public string AF_Name { get; set; }
        [Display(Name = "Code")]
        public string AF_Code { get; set; }
        [Display(Name = "Budget Code")]
        public string AF_Budget_Code { get; set; }
        [Display(Name = "Account No")]
        public string AF_No { get; set; }
        [Display(Name = "Cust")]
        public string AF_Cust { get; set; }
        [Display(Name = "Div")]
        public string AF_Div { get; set; }
        [Display(Name = "Account Group")]
        public string AF_Group_Name { get; set; }
        [Display(Name = "Currency")]
        public string AF_Currency_Name { get; set; }
        [Display(Name = "FBT")]
        public string AF_FBT_Name { get; set; }
        [Display(Name = "Creator Name")]
        public string AF_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string AF_Approver_Name { get; set; }
        [Display(Name = "Status")]
        public string AF_Status { get; set; }
    }
}
