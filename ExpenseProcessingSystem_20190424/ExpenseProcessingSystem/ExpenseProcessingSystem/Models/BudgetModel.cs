using System;
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
        public string Budget_Acc_ID { get; set; }
        public string Budget_Amount { get; set; }
        public int Budget_Creator_ID { get; set; }
        public int Budget_Approver_ID { get; set; }
        public DateTime Budget_Created_Date { get; set; }
        public DateTime Budget_Last_Updated { get; set; }
        public string Budget_Status { get; set; }
        public bool Budget_isDeleted { get; set; }
    }
}
