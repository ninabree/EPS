using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class UserModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Acc_UserID { get; set; }
        public string Acc_UserName { get; set; }
        public string Acc_FName { get; set; }
        public string Acc_LName { get; set; }
        [DataType(DataType.Password)]
        public string Acc_Password { get; set; }
        public int Acc_DeptID { get; set; }
        public string Acc_Email { get; set; }
        public string Acc_Role { get; set; }
        public string Acc_Comment { get; set; }
        public bool Acc_InUse { get; set; }
        public int Acc_Creator_ID { get; set; }
        public int Acc_Approver_ID { get; set; }
        public DateTime Acc_Created_Date { get; set; }
        public DateTime Acc_Last_Updated { get; set; }
        public string Acc_Status { get; set; }
    }
}
