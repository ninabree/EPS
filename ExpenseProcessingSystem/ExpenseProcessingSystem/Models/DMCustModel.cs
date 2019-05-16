using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.Models
{
    public class DMCustModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Cust_ID { get; set; }
        public int Cust_MasterID { get; set; }
        public string Cust_Name { get; set; }
        public string Cust_Abbr { get; set; }
        public string Cust_No { get; set; }
        public int Cust_Creator_ID { get; set; }
        public int Cust_Approver_ID { get; set; }
        public DateTime Cust_Created_Date { get; set; }
        public DateTime Cust_Last_Updated { get; set; }
        public int Cust_Status_ID { get; set; }
        public bool Cust_isDeleted { get; set; }
        public bool Cust_isActive { get; set; }
    }
}
