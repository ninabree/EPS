using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMPayeeModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_ID { get; set; }
        public int Pending_Payee_ID { get; set; }
        public string Pending_Payee_Name { get; set; }
        public string Pending_Payee_TIN { get; set; }
        public string Pending_Payee_Address { get; set; }
        public string Pending_Payee_Type { get; set; }
        public int Pending_Payee_No { get; set; }
        public int Pending_Payee_Creator_ID { get; set; }
        public int Pending_Payee_Approver_ID { get; set; }
        public DateTime Pending_Payee_Filed_Date { get; set; }
        public string Pending_Payee_Status { get; set; }

        public virtual DMPayeeModel Payee { get; set; }
    }
}
