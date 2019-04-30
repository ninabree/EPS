using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class CustFiltersViewModel
    {
        [Display(Name = "Customer Name")]
        public string CUF_Name { get; set; }
        [Display(Name = "Customer Abbr")]
        public string CUF_Abbr { get; set; }
        [Display(Name = "Customer Number")]
        public string CUF_No { get; set; }
        [Display(Name = "Customer Creator Name")]
        public string CUF_Creator_Name { get; set; }
        [Display(Name = "Customer Approver Name")]
        public string CUF_Approver_Name { get; set; }
        [Display(Name = "Customer Status")]
        public string CUF_Status { get; set; }
    }
}
