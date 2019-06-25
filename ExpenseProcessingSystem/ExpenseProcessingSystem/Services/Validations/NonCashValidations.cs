using Serilog;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseProcessingSystem.Services.Validations
{
    public class BalancedValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //var name = validationContext.DisplayName;
                //if (value == null || value.Equals(0))
                //{
                //    return new ValidationResult(name + " is empty");
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
