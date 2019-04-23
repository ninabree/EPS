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
        [Display(Name = "Account Code")]
        public string AF_Code { get; set; }
        [Display(Name = "Account No")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid Account number")]
        public int AF_No { get; set; }
        [Display(Name = "Account Cust")]
        public string AF_Cust { get; set; }
        [Display(Name = "Account Div")]
        public string AF_Div { get; set; }
        [Display(Name = "Account Fund")]
        public string AF_Fund { get; set; }
        [Display(Name = "Account FBT")]
        public string AF_FBT { get; set; }
        [Display(Name = "Creator Name")]
        public string AF_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string AF_Approver_Name { get; set; }
        [Display(Name = "Account Status")]
        public string AF_Status { get; set; }
    }
}
