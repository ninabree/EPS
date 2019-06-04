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
        public int Budget_AccountGroup_MasterID { get; set; }
        public int Budget_Account_MasterID { get; set; }
        public string Budget_GBase_Budget_Code { get; set; }
        public string Budget_ISPS_Account_Name { get; set; }
        public double Budget_Amount { get; set; }
        public int Budget_Creator_ID { get; set; }
        public bool Budget_IsActive { get; set; }
        public bool Budget_isDeleted { get; set; }
        public DateTime Budget_Date_Registered { get; set; }
    }
}
