using System.ComponentModel.DataAnnotations;
using ExpenseProcessingSystem.Services.Validations;

namespace ExpenseProcessingSystem.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "User Name")]
        [NotNullValidations]
        public string User_UserName { get; set; }

        [Display(Name = "Password")]
        [NotNullValidations]
        [DataType(DataType.Password)]
        public string User_Password { get; set; }
    }
}
