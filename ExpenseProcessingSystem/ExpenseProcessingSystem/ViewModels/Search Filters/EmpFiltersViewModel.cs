using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class EmpFiltersViewModel
    {
        [Display(Name = "Employee Name")]
        public string EMF_Name { get; set; }
        [Display(Name = "Employee Account Number")]
        public string EMF_Acc_No { get; set; }
        [Display(Name = "Employee Type")]
        public string EMF_Type { get; set; }
        [Display(Name = "Employee Category")]
        public string EMF_Category_Name { get; set; }
        [Display(Name = "Employee FBT")]
        public string EMF_FBT_Name{ get; set; }
        [Display(Name = "Creator Name")]
        public string EMF_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string EMF_Approver_Name { get; set; }
        [Display(Name = "Employee Status")]
        public string EMF_Status { get; set; }
    }
}
