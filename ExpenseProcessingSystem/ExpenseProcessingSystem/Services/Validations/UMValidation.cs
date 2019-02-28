using ExpenseProcessingSystem.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
                if (string.IsNullOrEmpty(data.Acc_UserName))
                {
                    return new ValidationResult("User name is empty");
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
    public class UMFNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (AccountViewModel)validationContext.ObjectInstance;
                if (string.IsNullOrEmpty(data.Acc_FName))
                {
                    return new ValidationResult("First name is empty");
                }
                if (data.Acc_UserName.Any(char.IsDigit))
                {
                    return new ValidationResult("First name contains numbers");
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
    public class UMLameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (AccountViewModel)validationContext.ObjectInstance;
                if (string.IsNullOrEmpty(data.Acc_FName))
                {
                    return new ValidationResult("First name is empty");
                }
                if (data.Acc_UserName.Any(char.IsDigit))
                {
                    return new ValidationResult("First name contains numbers");
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
