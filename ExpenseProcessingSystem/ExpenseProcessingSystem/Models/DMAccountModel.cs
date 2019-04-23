using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMAccountModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Account_ID { get; set; }
        public int Account_MasterID { get; set; }
        public int Account_FBT_MasterID { get; set; }
        public string Account_Name { get; set; }
        public string Account_Code { get; set; }
        public string Account_No { get; set; }
        public string Account_Cust { get; set; }
        public string Account_Div { get; set; }
        public bool Account_Fund { get; set; }
        public int Account_Creator_ID { get; set; }
        public int Account_Approver_ID { get; set; }
        public DateTime Account_Created_Date { get; set; }
        public DateTime Account_Last_Updated { get; set; }
        public string Account_Status { get; set; }
        public bool Account_isDeleted { get; set; }
        public bool Account_isActive { get; set; }
    }
}
