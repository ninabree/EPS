using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMAccountGroupModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_AccountGroup_ID { get; set; }
        public int Pending_AccountGroup_MasterID { get; set; }
        public string Pending_AccountGroup_Name { get; set; }
        public string Pending_AccountGroup_Code { get; set; }
        public int Pending_AccountGroup_Creator_ID { get; set; }
        public int Pending_AccountGroup_Approver_ID { get; set; }
        public DateTime Pending_AccountGroup_Filed_Date { get; set; }
        public int Pending_AccountGroup_Status_ID { get; set; }
        public bool Pending_AccountGroup_isDeleted { get; set; }
        public bool Pending_AccountGroup_isActive { get; set; }
    }
}
