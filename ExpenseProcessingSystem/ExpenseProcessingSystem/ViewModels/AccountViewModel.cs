using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class AccountViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Employee ID")]
        [CommonValidations]
        public int Acc_UserID { get; set; }

        [Display(Name = "Username")]
        [CommonValidations]
        public string Acc_UserName { get; set; }

        [Display(Name = "Employee Name")]
        [CommonValidations]
        public string Acc_FName { get; set; }
        public string Acc_LName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Acc_Password { get; set; }

        [Display(Name = "Department")]
        [CommonValidations]
        public int Acc_DeptID { get; set; }

        [Display(Name = "Email")]
        [CommonValidations]
        public string Acc_Email { get; set; }

        [Display(Name = "Role")]
        [CommonValidations]
        public string Acc_Role { get; set; }

        [Display(Name = "Comment")]
        public string Acc_Comment { get; set; }

        [Display(Name = "In-Use")]
        public bool Acc_InUse { get; set; }

        public int Acc_Creator_ID { get; set; }
        public int Acc_Approver_ID { get; set; }
        public DateTime Acc_Created_Date { get; set; }
        public DateTime Acc_Last_Updated { get; set; }
        public string Acc_Status { get; set; }
    }
}
