using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class AccountModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Acc_UserID { get; set; }
        
        public string Acc_UserName { get; set; }
        
        [DataType(DataType.Password)]
        public string Acc_Password { get; set; }
        
        public int Acc_DeptID { get; set; }
        
        public string Acc_Email { get; set; }
        
        public string Acc_Role { get; set; }
        
        public string Acc_Comment { get; set; }
        
        public bool Acc_InUse { get; set; }
    }
}
