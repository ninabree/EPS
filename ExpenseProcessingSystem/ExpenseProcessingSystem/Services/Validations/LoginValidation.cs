using ExpenseProcessingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Validations
{
    public class UserNameValidation : ValidationAttribute 
    {
        protected override ValidationResult IsValid (object value, ValidationContext validationContext)
        {
            try
            {
                var data = (LoginViewModel) validationContext.ObjectInstance;
                if(string.IsNullOrEmpty(data.Acc_UserName))
                {
                    return new ValidationResult("User name is empty");
                }
                if(data.Acc_UserName.Any(char.IsDigit))
                {
                    return new ValidationResult("User name contains numbers");
                }
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult("Invalid input");
            }
        }
    }

    public class PasswordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (LoginViewModel)validationContext.ObjectInstance;
                if (string.IsNullOrEmpty(data.Acc_Password))
                {
                    return new ValidationResult("Password is empty");
                }
                if (data.Acc_Password.Contains("123"))
                {
                    return new ValidationResult("Password contains 123");
                }
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult("Invalid input");
            }
        }
    }
}
