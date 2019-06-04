using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryNCDtlAccModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpNCDtlAcc_ID { get; set; }
        public int ExpNCDtlAcc_Acc_ID { get; set; }
        public int ExpNCDtlAcc_Type_ID { get; set; }
        public int ExpNCDtlAcc_Curr_ID { get; set; }
        public string ExpNCDtlAcc_Acc_Name { get; set; }
        public float ExpNCDtlAcc_Inter_Rate { get; set; }
        public float ExpNCDtlAcc_Amount { get; set; }
        public ExpenseEntryNCDtlModel ExpenseEntryNCModel { get; set; }
    }
}
