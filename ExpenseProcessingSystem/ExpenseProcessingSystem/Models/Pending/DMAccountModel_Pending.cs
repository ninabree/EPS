using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMAccountModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_Account_ID { get; set; }
        public int Pending_Account_MasterID { get; set; }
        public int Pending_Account_FBT_MasterID { get; set; }
        public int Pending_Account_Group_MasterID { get; set; }
        public int Pending_Account_Currency_MasterID { get; set; }
        public string Pending_Account_Name { get; set; }
        public string Pending_Account_Code { get; set; }
        public string Pending_Account_No { get; set; }
        public string Pending_Account_Cust { get; set; }
        public string Pending_Account_Div { get; set; }
        public bool Pending_Account_Fund { get; set; }
        public int Pending_Account_Creator_ID { get; set; }
        public int Pending_Account_Approver_ID { get; set; }
        public DateTime Pending_Account_Filed_Date { get; set; }
        public int Pending_Account_Status_ID { get; set; }
        public bool Pending_Account_isDeleted { get; set; }
        public bool Pending_Account_isActive { get; set; }
    }
}
