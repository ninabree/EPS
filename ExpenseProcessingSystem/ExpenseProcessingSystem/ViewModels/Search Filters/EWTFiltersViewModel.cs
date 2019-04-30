﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class TRFiltersViewModel
    {
        [Display(Name = "TR Nature")]
        public string EF_Nature { get; set; }
        [Display(Name = "TR Nature of Income Payment")]
        public string EF_Nature_Income_Payment { get; set; }
        [Display(Name = "TR Tax Rate")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid Tax Rate")]
        public int EF_Tax_Rate { get; set; }
        [Display(Name = "ATC")]
        public string EF_ATC { get; set; }
        [Display(Name = "TR Tax Rate Description")]
        public string EF_Tax_Rate_Desc { get; set; }
        [Display(Name = "Creator Name")]
        public string EF_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string EF_Approver_Name { get; set; }
        [Display(Name = "TR Status")]
        public string EF_Status { get; set; }
    }
}
