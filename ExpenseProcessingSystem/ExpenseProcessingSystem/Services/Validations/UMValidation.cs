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
    public class UMUserNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (AccountViewModel)validationContext.ObjectInstance;
                //(?=.{8,20}$) username is 8-20 characters long / [a-zA-Z0-9._] allowed characters
                //var regex = @"^{8,20}$";
                if (!string.IsNullOrEmpty(data.Acc_UserName))
                {
                    //var match = Regex.Match(data.Acc_UserName, regex, RegexOptions.IgnoreCase);
                    //if (!match.Success)
                    if(data.Acc_UserName.Length < 8 || data.Acc_UserName.Length > 20)
                    {
                        return new ValidationResult("Sorry your username can't be stored on our system");
                    }
                }
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
}
