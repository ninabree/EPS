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
        public int User_ID { get; set; }

        [Display(Name = "Employee Code")]
        [NotNullValidations]
        public string User_EmpCode { get; set; }

        [Display(Name = "Username")]
        [NotNullValidations, LengthValidation]
        public string User_UserName { get; set; }

        [Display(Name = "Employee Name")]
        [NotNullValidations, TextValidation]
        public string User_FName { get; set; }
        public string User_LName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        //[PasswordValidation, LengthValidation]
        public string User_Password { get; set; }

        [Display(Name = "Department")]
        [NotNullValidations, IntegerValidation]
        public int User_DeptID { get; set; }

        [Display(Name = "Email")]
        [NotNullValidations, EmailValidation]
        public string User_Email { get; set; }

        [Display(Name = "Role")]
        [NotNullValidations]
        public string User_Role { get; set; }

        [Display(Name = "Comment")]
        [TextValidation]
        public string User_Comment { get; set; }

        [Display(Name = "In-Use")]
        public bool User_InUse { get; set; }

        public int User_Creator_ID { get; set; }
        public int User_Approver_ID { get; set; }
        public DateTime User_Created_Date { get; set; }
        public DateTime User_Last_Updated { get; set; }
        public string User_Status { get; set; }
    }
}
