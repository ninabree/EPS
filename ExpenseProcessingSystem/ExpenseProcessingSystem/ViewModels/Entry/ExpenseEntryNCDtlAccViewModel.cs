﻿using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class ExpenseEntryNCDtlAccViewModel
    {
        [NotNullValidations]
        [Display(Name = "Account")]
        public int ExpNCDtlAcc_Acc_ID { get; set; }
        [Display(Name = "Account")]
        public string ExpNCDtlAcc_Acc_Name { get; set; }
        [NotNullValidations]
        [Display(Name = "Type")]
        public int ExpNCDtlAcc_Type_ID { get; set; }
        [NotNullValidations]
        [Display(Name = "CCY")]
        public int ExpNCDtlAcc_Curr_ID { get; set; }
        public string ExpNCDtlAcc_Curr_Name { get; set; }
        [NotNullValidations]
        [Display(Name = "Inter-Rate")]
        public float ExpNCDtlAcc_Inter_Rate { get; set; }
        [NotNullValidations]
        [Display(Name = "Amount")]
        public float ExpNCDtlAcc_Amount { get; set; }
    }
}