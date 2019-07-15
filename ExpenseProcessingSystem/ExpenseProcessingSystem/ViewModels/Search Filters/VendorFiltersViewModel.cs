using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class VendorFiltersViewModel
    {
        [Display(Name = "Vendor Name")]
        public string PF_Name { get; set; }
        [Display(Name = "Vendor TIN")]
        public string PF_TIN { get; set; }
        [Display(Name = "Vendor Address")]
        public string PF_Address { get; set; }
        [Display(Name = "Vendor Creator Name")]
        public string PF_Creator_Name { get; set; }
        [Display(Name = "Vendor Approver Name")]
        public string PF_Approver_Name { get; set; }
        [Display(Name = "Vendor Status")]
        public string PF_Status { get; set; }
    }
}
