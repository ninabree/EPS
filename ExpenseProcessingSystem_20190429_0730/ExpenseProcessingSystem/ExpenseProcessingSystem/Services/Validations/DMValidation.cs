using ExpenseProcessingSystem.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Validations
{
    public class DMDeptValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (DMViewModel)validationContext.ObjectInstance;
                //if(string.IsNullOrEmpty(data.Acc_UserName))
                //{
                //    return new ValidationResult("User name is empty");
                //}
                //if(data.Acc_UserName.Any(char.IsDigit))
                //{
                //    return new ValidationResult("User name contains numbers");
                //}
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
    public class DMPayeeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (DMViewModel)validationContext.ObjectInstance;
                //if(string.IsNullOrEmpty(data.Acc_UserName))
                //{
                //    return new ValidationResult("User name is empty");
                //}
                //if(data.Acc_UserName.Any(char.IsDigit))
                //{
                //    return new ValidationResult("User name contains numbers");
                //}
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
