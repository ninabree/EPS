using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMTRModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_TR_ID { get; set; }
        public int Pending_TR_MasterID { get; set; }
        public string Pending_TR_WT_Title { get; set; }
        public string Pending_TR_Nature { get; set; }
        public float Pending_TR_Tax_Rate { get; set; }
        public string Pending_TR_ATC { get; set; }
        public string Pending_TR_Nature_Income_Payment { get; set; }
        public int Pending_TR_Creator_ID { get; set; }
        public int Pending_TR_Approver_ID { get; set; }
        public DateTime Pending_TR_Filed_Date { get; set; }
        public int Pending_TR_Status_ID { get; set; }
        public bool Pending_TR_isDeleted { get; set; }
        public bool Pending_TR_isActive { get; set; }
    }
}
