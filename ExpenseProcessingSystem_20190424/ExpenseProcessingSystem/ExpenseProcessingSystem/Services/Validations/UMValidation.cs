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
                var data = (User2ViewModel)validationContext.ObjectInstance;
                if (!string.IsNullOrEmpty(data.User_UserName))
                {
                    if(data.User_UserName.Length < 3 || data.User_UserName.Length > 20)
                    {
                        return new ValidationResult("Username too short");
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
