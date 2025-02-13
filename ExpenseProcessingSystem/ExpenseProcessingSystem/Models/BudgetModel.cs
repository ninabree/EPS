﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class BudgetModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Budget_ID { get; set; }
        public int Budget_Account_ID { get; set; }
        public int Budget_Account_MasterID { get; set; }
        public decimal Budget_Amount { get; set; }
        public int Budget_Creator_ID { get; set; }
        public bool Budget_IsActive { get; set; }
        public bool Budget_isDeleted { get; set; }
        public DateTime Budget_Date_Registered { get; set; }
        public decimal Budget_New_Amount { get; set; }
        public int Budget_GWrite_Status { get; set; }
    }
}
