using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMEWTModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_EWT_ID { get; set; }
        public int Pending_EWT_MasterID { get; set; }
        public string Pending_EWT_Nature { get; set; }
        public int Pending_EWT_Tax_Rate { get; set; }
        public string Pending_EWT_ATC { get; set; }
        public string Pending_EWT_Tax_Rate_Desc { get; set; }
        public int Pending_EWT_Creator_ID { get; set; }
        public int Pending_EWT_Approver_ID { get; set; }
        public DateTime Pending_EWT_Filed_Date { get; set; }
        public string Pending_EWT_Status { get; set; }
        public bool Pending_EWT_isDeleted { get; set; }
        public bool Pending_EWT_isActive { get; set; }
    }
}
