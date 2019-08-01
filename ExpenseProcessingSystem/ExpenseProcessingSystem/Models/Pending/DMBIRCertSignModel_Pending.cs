using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMBIRCertSignModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_BCS_ID { get; set; }
        public int Pending_BCS_MasterID { get; set; }
        public int Pending_BCS_User_ID { get; set; }
        public string Pending_BCS_TIN { get; set; }
        public string Pending_BCS_Position { get; set; }
        public string Pending_BCS_Signatures { get; set; }
        public int Pending_BCS_Creator_ID { get; set; }
        public int Pending_BCS_Approver_ID { get; set; }
        public DateTime Pending_BCS_Filed_Date { get; set; }
        public int Pending_BCS_Status_ID { get; set; }
        public bool Pending_BCS_isDeleted { get; set; }
        public bool Pending_BCS_isActive { get; set; }
    }
}
