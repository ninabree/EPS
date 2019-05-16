using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMCustModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_Cust_ID { get; set; }
        public int Pending_Cust_MasterID { get; set; }
        public string Pending_Cust_Name { get; set; }
        public string Pending_Cust_Abbr { get; set; }
        public string Pending_Cust_No { get; set; }
        public int Pending_Cust_Creator_ID { get; set; }
        public int Pending_Cust_Approver_ID { get; set; }
        public DateTime Pending_Cust_Filed_Date { get; set; }
        public int Pending_Cust_Status_ID { get; set; }
        public bool Pending_Cust_isDeleted { get; set; }
        public bool Pending_Cust_isActive { get; set; }
    }
}
