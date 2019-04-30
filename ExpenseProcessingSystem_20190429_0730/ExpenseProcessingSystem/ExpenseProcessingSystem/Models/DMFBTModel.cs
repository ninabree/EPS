using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMFBTModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FBT_ID { get; set; }
        public int FBT_MasterID { get; set; }
        public string FBT_Name { get; set; }
        public string FBT_Account { get; set; }
        public string FBT_Formula { get; set; }
        public int FBT_Tax_Rate { get; set; }
        public int FBT_Creator_ID { get; set; }
        public int FBT_Approver_ID { get; set; }
        public DateTime FBT_Created_Date { get; set; }
        public DateTime FBT_Last_Updated { get; set; }
        public string FBT_Status { get; set; }
        public bool FBT_isDeleted { get; set; }
        public bool FBT_isActive { get; set; }
    }
}
