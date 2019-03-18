using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class PayeeFiltersViewModel
    {

        [Display(Name = "Payee Name")]
        public string PF_Name { get; set; }
        [Display(Name = "Payee TIN")]
        public int PF_TIN { get; set; }
        [Display(Name = "Payee Address")]
        public string PF_Address { get; set; }
        [Display(Name = "Payee Type")]
        public string PF_Type { get; set; }
        [Display(Name = "Payee No")]
        public int PF_No { get; set; }
        [Display(Name = "Payee Creator Name")]
        public string PF_Creator_Name { get; set; }
        [Display(Name = "Payee Approver Name")]
        public string PF_Approver_Name { get; set; }
        [Display(Name = "Payee Status")]
        public string PF_Status { get; set; }
    }
}
