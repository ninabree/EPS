using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class TRFiltersViewModel
    {
        [Display(Name = "Withholding Tax Title")]
        public string TR_WT_Title { get; set; }
        [Display(Name = "Tax Rate Nature")]
        public string TR_Nature { get; set; }
        [Display(Name = "Tax Rate")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid Tax Rate")]
        public int TR_Tax_Rate { get; set; }
        [Display(Name = "ATC")]
        public string TR_ATC { get; set; }
        [Display(Name = "Nature of Income Payment")]
        public string TR_Nature_Income_Payment { get; set; }
        [Display(Name = "Creator Name")]
        public string TR_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string TR_Approver_Name { get; set; }
        [Display(Name = "Status")]
        public string TR_Status { get; set; }
    }
}
