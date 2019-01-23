using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.Services.Validations;

namespace ExpenseProcessingSystem.ViewModels
{
    public class LoginViewModel
    {
        [UserNameValidation]
        public string Acc_UserName { get; set; }

        [PasswordValidation]
        [DataType(DataType.Password)]
        public string Acc_Password { get; set; }
    }
}
