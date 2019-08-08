using System.ComponentModel.DataAnnotations;
using ExpenseProcessingSystem.Services.Validations;

namespace ExpenseProcessingSystem.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "User Name")]
        //[NotNullValidations, LengthValidation]
        public string User_UserName { get; set; }

        [Display(Name = "Password")]
        //[PasswordValidation, NotNullValidations]
        [DataType(DataType.Password)]
        public string User_Password { get; set; }
    }
}
