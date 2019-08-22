using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.ViewModels.Entry;
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
                var data = (EntryNCViewModelList)validationContext.ObjectInstance;

                if (data.EntryNC.ExpenseEntryNCDtls.Count > 0)
                {
                    if (data.EntryNC.NC_Category_ID != GlobalSystemValues.NC_JS_PAYROLL && data.EntryNC.NC_Category_ID != GlobalSystemValues.NC_RETURN_OF_JS_PAYROLL && data.EntryNC.NC_Category_ID != GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES)
                    {
                        if (data.EntryNC.NC_DebitAmt != data.EntryNC.NC_CredAmt)
                        {
                            return new ValidationResult("Total Debit and Total Credit Amounts are unbalanced.");
                        }
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
    public class RemarksLimitValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (EntryNCViewModelList)validationContext.ObjectInstance;

                if (data.EntryNC.ExpenseEntryNCDtls.Count > 0)
                {
                    foreach (ExpenseEntryNCDtlViewModel dtl in data.EntryNC.ExpenseEntryNCDtls)
                    {
                        if ((dtl.ExpNCDtl_Remarks_Desc.Length + dtl.ExpNCDtl_Remarks_Period.Length) > 29)
                        {
                            return new ValidationResult("Remarks Description and Period exceeds the limit of 29 characters in total.");
                        }
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
