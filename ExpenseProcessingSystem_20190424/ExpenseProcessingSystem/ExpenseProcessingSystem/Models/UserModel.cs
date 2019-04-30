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
        public int User_ID { get; set; }
        public string User_UserName { get; set; }
        public string User_FName { get; set; }
        public string User_LName { get; set; }
        [DataType(DataType.Password)]
        public string User_Password { get; set; }
        public int User_DeptID { get; set; }
        public string User_Email { get; set; }
        public string User_Role { get; set; }
        public string User_Comment { get; set; }
        public bool User_InUse { get; set; }
        public int User_Creator_ID { get; set; }
        public int User_Approver_ID { get; set; }
        public DateTime User_Created_Date { get; set; }
        public DateTime User_Last_Updated { get; set; }
        public string User_Status { get; set; }
    }
}
