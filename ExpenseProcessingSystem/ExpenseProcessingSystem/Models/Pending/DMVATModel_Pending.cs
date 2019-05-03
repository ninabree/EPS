using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMVATModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_VAT_ID { get; set; }
        public int Pending_VAT_MasterID { get; set; }
        public string Pending_VAT_Name { get; set; }
        public float Pending_VAT_Rate { get; set; }
        public int Pending_VAT_Creator_ID { get; set; }
        public int Pending_VAT_Approver_ID { get; set; }
        public DateTime Pending_VAT_Filed_Date { get; set; }
        public string Pending_VAT_Status { get; set; }
        public bool Pending_VAT_isDeleted { get; set; }
        public bool Pending_VAT_isActive { get; set; }
    }
}
