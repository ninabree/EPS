﻿using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class ExpenseEntryNCDtlViewModel
    {
        public int ExpNCDtl_ID { get; set; }
        //[NotNullValidations]
        [Display(Name = "Description")]
        public string ExpNCDtl_Remarks_Desc { get; set; }
        [Display(Name = "Period")]
        public string ExpNCDtl_Remarks_Period { get; set; }
        public List<ExpenseEntryNCDtlAccViewModel> ExpenseEntryNCDtlAccs { get; set; }


        public ExpenseEntryNCDtlViewModel()
        {
            ExpenseEntryNCDtlAccs = new List<ExpenseEntryNCDtlAccViewModel>();
        }
    }
}