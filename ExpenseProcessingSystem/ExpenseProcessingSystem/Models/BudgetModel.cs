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
        public double Budget_Amount { get; set; }
        public double Budget_Current { get; set; }
        public string Budget_Approver_ID { get; set; }
        public byte Budget_Status { get; set; }
        public bool Budget_isDeleted { get; set; }
        public DateTime Budget_Last_Approval_Date { get; set; }

        //public int Budget_Group_ID { get; set; }
        //public int Budget_Group_MasterID { get; set; }
        //public double Budget_Group_Total { get; set; }
        //public double Budget_Group_Current { get; set; }
        //public string Budget_Group_VerifierApprover_ID { get; set; }
        //public byte Budget_Group_Status { get; set; }
        //public bool Budget_Group_IsCurrentBal { get; set; }
        //public DateTime Budget_Group_Created_Date { get; set; }
        //public DateTime Budget_Group_Last_Approved_Date { get; set; }
    }
}
