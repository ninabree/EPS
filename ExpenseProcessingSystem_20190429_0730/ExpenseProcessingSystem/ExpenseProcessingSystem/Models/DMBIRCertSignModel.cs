using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.Models
{
    public class DMBIRCertSignModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BCS_ID { get; set; }
        public int BCS_MasterID { get; set; }
        public string BCS_Name { get; set; }
        public int BCS_TIN { get; set; }
        public string BCS_Position { get; set; }
        public string BCS_Signatures { get; set; }
        public int BCS_Creator_ID { get; set; }
        public int BCS_Approver_ID { get; set; }
        public DateTime BCS_Created_Date { get; set; }
        public DateTime BCS_Last_Updated { get; set; }
        public string BCS_Status { get; set; }
        public bool BCS_isDeleted { get; set; }
        public bool BCS_isActive { get; set; }
    }
}
