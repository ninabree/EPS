using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class BCSFiltersViewModel
    {
        [Display(Name = "BCS Name")]
        public string BF_Name { get; set; }
        [Display(Name = "BCS TIN")]
        public int BF_TIN { get; set; }
        [Display(Name = "BCS Position")]
        public string BF_Position { get; set; }
        [Display(Name = "BCS Signature")]
        public string BF_Signatures { get; set; }
        [Display(Name = "BCS Creator Name")]
        public string BF_Creator_Name { get; set; }
        [Display(Name = "BCS Approver Name")]
        public string BF_Approver_Name { get; set; }
        [Display(Name = "BCS Status")]
        public string BF_Status { get; set; }
    }
}
