using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class UserViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Employee ID")]
        public int User_ID { get; set; }

        [Display(Name = "Employee Code")]
        public string User_EmpCode { get; set; }

        [Display(Name = "Username")]
        public string User_UserName { get; set; }

        [Display(Name = "Employee Name")]
        public string User_FName { get; set; }
        public string User_LName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string User_Password { get; set; }

        [Display(Name = "Department")]
        public int User_Dept_ID { get; set; }
        public string User_Dept_Name { get; set; }

        [Display(Name = "Email")]
        public string User_Email { get; set; }

        [Display(Name = "Role")]
        public string User_Role { get; set; }

        [Display(Name = "Comment")]
        public string User_Comment { get; set; }

        [Display(Name = "In-Use")]
        public bool User_InUse { get; set; }
        
        public string User_Creator_Name { get; set; }
        public string User_Approver_Name { get; set; }
        public DateTime User_Created_Date { get; set; }
        public DateTime User_Last_Updated { get; set; }
        public string User_Status { get; set; }
    }
}
