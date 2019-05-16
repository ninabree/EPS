using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMAccountGroupModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountGroup_ID { get; set; }
        public int AccountGroup_MasterID { get; set; }
        public string AccountGroup_Name { get; set; }
        public string AccountGroup_Code { get; set; }
        public int AccountGroup_Creator_ID { get; set; }
        public int AccountGroup_Approver_ID { get; set; }
        public DateTime AccountGroup_Created_Date { get; set; }
        public DateTime AccountGroup_Last_Updated { get; set; }
        public int AccountGroup_Status_ID { get; set; }
        public bool AccountGroup_isDeleted { get; set; }
        public bool AccountGroup_isActive { get; set; }
    }
}
