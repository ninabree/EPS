using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMFBTModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_FBT_ID { get; set; }
        public int Pending_FBT_MasterID { get; set; }
        public string Pending_FBT_Name { get; set; }
        public string Pending_FBT_Formula { get; set; }
        public float Pending_FBT_Tax_Rate { get; set; }
        public int Pending_FBT_Creator_ID { get; set; }
        public int Pending_FBT_Approver_ID { get; set; }
        public DateTime Pending_FBT_Filed_Date { get; set; }
        public int Pending_FBT_Status_ID { get; set; }
        public bool Pending_FBT_isDeleted { get; set; }
        public bool Pending_FBT_isActive { get; set; }
    }
}
