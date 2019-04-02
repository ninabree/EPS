using ExpenseProcessingSystem.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
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
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                //sample fatal error log
                Log.Fatal(ex, "User: {user}, StackTrace : {trace}, Error Message: {message}", "[UserID]", ex.StackTrace, ex.Message);
                return new ValidationResult("Invalid input");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    public class LoginPasswordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (LoginViewModel)validationContext.ObjectInstance;
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasMiniMaxChars = new Regex(@".{8,15}");
                var hasLowerChar = new Regex(@"[a-z]+");
                var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

                if (!string.IsNullOrEmpty(data.User_Password))
                {

                    if (!hasLowerChar.IsMatch(data.User_Password))
                    {
                        return new ValidationResult("Password should contain At least one lower case letter");
                    }
                    else if (!hasUpperChar.IsMatch(data.User_Password))
                    {
                        return new ValidationResult("Password should contain At least one upper case letter");
                    }
                    else if (!hasMiniMaxChars.IsMatch(data.User_Password))
                    {
                        return new ValidationResult("Password should not be less than 8 characters or greater than 15 characters");
                    }
                    else if (!hasNumber.IsMatch(data.User_Password))
                    {
                        return new ValidationResult("Password should contain At least one numeric value");
                    }

                    else if (!hasSymbols.IsMatch(data.User_Password))
                    {
                        return new ValidationResult("Password should contain At least one special case characters");
                    }
                }
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "User: {user}, StackTrace : {trace}, Error Message: {message}", "[UserID]", ex.StackTrace, ex.Message);
                return new ValidationResult("Invalid input");
            }
        }
    }
}
