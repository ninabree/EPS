using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMCheckModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_Check_ID { get; set; }
        public int Pending_Check_MasterID { get; set; }
        public DateTime Pending_Check_Input_Date { get; set; }
        public string Pending_Check_Series_From { get; set; }
        public string Pending_Check_Series_To { get; set; }
        public string Pending_Check_Type { get; set; }
        public string Pending_Check_Name { get; set; }
        public int Pending_Check_Creator_ID { get; set; }
        public int Pending_Check_Approver_ID { get; set; }
        public DateTime Pending_Check_Filed_Date { get; set; }
        public string Pending_Check_Status { get; set; }
        public bool Pending_Check_isDeleted { get; set; }
        public bool Pending_Check_isActive { get; set; }
    }
}
