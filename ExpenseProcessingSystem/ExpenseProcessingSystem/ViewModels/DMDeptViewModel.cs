﻿using ExpenseProcessingSystem.Services.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMDeptViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Dept_ID { get; set; }
        [Display(Name = "Dept ID")]
        public int Dept_MasterID { get; set; }
        [Display(Name = "Dept Name")]
        [NotNullValidations, TextValidation]
        public string Dept_Name { get; set; }
        [Display(Name = "Dept Code")]
        [NotNullValidations, TextValidation]
        public string Dept_Code { get; set; }
        [Display(Name = "Dept Budget Unit")]
        [NotNullValidations, TextValidation]
        public string Dept_Budget_Unit { get; set; }
        public int Dept_Creator_ID { get; set; }
        public int Dept_Approver_ID { get; set; }
        public string Dept_Creator_Name { get; set; }
        public string Dept_Approver_Name { get; set; }
        [Display(Name = "Dept Created")]
        [DataType(DataType.Date)]
        public DateTime Dept_Created_Date { get; set; }
        [Display(Name = "Dept Last Updated")]
        [DataType(DataType.Date)]
        public DateTime Dept_Last_Updated { get; set; }
        [Display(Name = "Dept Status")]
        public int Dept_Status_ID { get; set; }
        public string Dept_Status { get; set; }
    }
}
