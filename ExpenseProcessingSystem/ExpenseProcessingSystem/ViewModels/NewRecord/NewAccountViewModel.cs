﻿using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewAccountListViewModel
    {
        public List<NewAccountViewModel> NewAccountVM { get; set; }
    }
    public class NewAccountViewModel
    {
        [Display(Name = "Account Name")]
        [NotNullValidations, TextValidation]
        public string Account_Name { get; set; }
        [Display(Name = "Account Code")]
        [NotNullValidations, TextValidation]
        public string Account_Code { get; set; }
        [Display(Name = "Account No")]
        [NotNullValidations, IntegerValidation]
        public int Account_No { get; set; }
        [Display(Name = "Account Cust")]
        [NotNullValidations, TextValidation]
        public string Account_Cust { get; set; }
        [Display(Name = "Account Div")]
        [NotNullValidations, TextValidation]
        public string Account_Div { get; set; }
        [Display(Name = "Account Fund")]
        [NotNullValidations, TextValidation]
        public string Account_Fund { get; set; }
    }
}
