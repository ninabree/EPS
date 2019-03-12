using ExpenseProcessingSystem.Services.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.ViewModels
{
    public class User2ViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "User ID")]
        public int Acc_UserID { get; set; }

        [Display(Name = "Username")]
        [NotNullValidations, LengthValidation]
        public string Acc_UserName { get; set; }

        [Display(Name = "Employee Name")]
        [NotNullValidations, TextValidation]
        public string Acc_FName { get; set; }
        public string Acc_LName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [PasswordValidation, LengthValidation]
        public string Acc_Password { get; set; }

        [Display(Name = "Department")]
        [NotNullValidations, IntegerValidation]
        public int Acc_DeptID { get; set; }

        [Display(Name = "Email")]
        [NotNullValidations, EmailValidation]
        public string Acc_Email { get; set; }

        [Display(Name = "Role")]
        [NotNullValidations]
        public string Acc_Role { get; set; }

        [Display(Name = "Comment")]
        [TextValidation]
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
