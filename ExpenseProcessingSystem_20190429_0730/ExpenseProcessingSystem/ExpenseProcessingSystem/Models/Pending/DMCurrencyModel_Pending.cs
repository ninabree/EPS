using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMCurrencyModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_Curr_ID { get; set; }
        public int Pending_Curr_MasterID { get; set; }
        public string Pending_Curr_Name { get; set; }
        public string Pending_Curr_CCY_ABBR { get; set; }
        public int Pending_Curr_Creator_ID { get; set; }
        public int Pending_Curr_Approver_ID { get; set; }
        public DateTime Pending_Curr_Filed_Date { get; set; }
        public string Pending_Curr_Status { get; set; }
        public bool Pending_Curr_isDeleted { get; set; }
        public bool Pending_Curr_isActive { get; set; }
    }
}
