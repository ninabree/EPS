using ExpenseProcessingSystem.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Validations
{
    public class ReportValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (HomeReportViewModel)validationContext.ObjectInstance;
                var name = validationContext.DisplayName;
                if (!string.IsNullOrEmpty(data.ReportType))
                {
                    switch (data.PeriodOption)
                    {
                        case "1":
                            if (string.IsNullOrEmpty(data.Year))
                            {
                                return new ValidationResult("Year input is required");
                            }
                            if (string.IsNullOrEmpty(data.Month))
                            {
                                return new ValidationResult("Month input is required");
                            }
                            break;

                        case "2":
                            if (string.IsNullOrEmpty(data.YearSem))
                            {
                                return new ValidationResult("Semestral Year input is required");
                            }
                            if (string.IsNullOrEmpty(data.Semester))
                            {
                                return new ValidationResult("Semester input is required");
                            }
                            break;
                        case "3":
                            if (data.PeriodFrom == DateTime.MinValue)
                            {
                                return new ValidationResult("Period From input is required");
                            }
                            if (data.PeriodTo == DateTime.MinValue)
                            {
                                return new ValidationResult("Period To input is required");
                            }
                            break;
                    }
                }
                else
                {
                    return new ValidationResult("Report Type is required");
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
