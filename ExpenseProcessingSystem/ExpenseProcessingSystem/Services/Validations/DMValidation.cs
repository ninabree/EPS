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
    public class DMAccountFundValidation : ValidationAttribute
    {
        private readonly string _CheckBoxProperty;

        public DMAccountFundValidation(string CheckBoxProperty)
        {
            _CheckBoxProperty = CheckBoxProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var name = validationContext.DisplayName;
            var property = validationContext.ObjectType.GetProperty(_CheckBoxProperty);
            var val = property.GetValue(validationContext.ObjectInstance, null);
            if ((Boolean)val && value != null)
            {
                if (value.ToString() == "")
                {
                    return new ValidationResult(name + " cannot be empty.");
                }
                else if (!(int.TryParse(value.ToString(), out int temp)))
                {
                    if (temp == 0)
                    {
                        return new ValidationResult(name + " cannot be empty.");
                    }
                }
            }
            else if ((Boolean)val && value == null)
            {
                return new ValidationResult(name + " is  empty");
            }
            else if (!(Boolean)val && value != null)
            {
                if (value.ToString() != "0" || value.ToString() == "")
                {
                    return new ValidationResult(name + " can only be entered if Fund is checked.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
