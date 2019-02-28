using System.ComponentModel.DataAnnotations;
using ExpenseProcessingSystem.Services.Validations;

namespace ExpenseProcessingSystem.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "User Name")]
        [/*UserNameValidation, */CommonValidations]
        public string Acc_UserName { get; set; }

        [Display(Name = "Password")]
        [/*PasswordValidation, */CommonValidations]
        [DataType(DataType.Password)]
        public string Acc_Password { get; set; }
    }
}
