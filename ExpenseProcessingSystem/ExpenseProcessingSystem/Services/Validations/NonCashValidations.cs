using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.ViewModels.Entry;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml;

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
                        if (data.EntryNC.NC_Category_ID == GlobalSystemValues.NC_PCHC)
                        {
                            decimal db1Amt = data.EntryNC.ExpenseEntryNCDtls[0].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Amount;
                            decimal cd1Amt = data.EntryNC.ExpenseEntryNCDtls[0].ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Amount + data.EntryNC.ExpenseEntryNCDtls[0].ExpenseEntryNCDtlAccs[2].ExpNCDtlAcc_Amount;
                            decimal db2Amt = data.EntryNC.ExpenseEntryNCDtls[1].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Amount;
                            decimal cd2Amt = data.EntryNC.ExpenseEntryNCDtls[1].ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Amount + data.EntryNC.ExpenseEntryNCDtls[0].ExpenseEntryNCDtlAccs[2].ExpNCDtlAcc_Amount;
                            if ((db1Amt != cd1Amt) || (db2Amt != cd2Amt))
                            {
                                return new ValidationResult("Total Debit and Total Credit Amounts are unbalanced.");
                            }
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
    public class RJSPValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var data = (EntryNCViewModelList)validationContext.ObjectInstance;

                if (data.EntryNC.ExpenseEntryNCDtls.Count > 0)
                {
                    if (data.EntryNC.NC_Category_ID == GlobalSystemValues.NC_RETURN_OF_JS_PAYROLL)
                    {
                        if (data.EntryNC.ExpenseEntryNCDtls[1].ExpenseEntryNCDtlAccs[1].ExpNCDtlAcc_Inter_Rate <= 0)
                        {
                            return new ValidationResult("2nd entry inter-rate is required to compute the Credit amount.");
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
                        if (dtl.ExpNCDtl_Remarks_Desc == null)
                        {
                            return new ValidationResult("Remarks Description cannot be empty.");
                        } else if (dtl.ExpNCDtl_Remarks_Desc != null && dtl.ExpNCDtl_Remarks_Period == null)
                        {
                            if ((dtl.ExpNCDtl_Remarks_Desc.Length) > 29)
                            {
                                return new ValidationResult("Remarks Description and Period exceeds the limit of 29 characters in total.");
                            }
                        }
                        else if ((dtl.ExpNCDtl_Remarks_Desc.Length + dtl.ExpNCDtl_Remarks_Period.Length) > 29)
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
    
    public class AccountNotNullValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var name = validationContext.DisplayName;
                var data = (EntryNCViewModelList)validationContext.ObjectInstance;
                if(data.EntryNC.NC_Category_ID != GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES)
                {
                    foreach(ExpenseEntryNCDtlViewModel dtl in data.EntryNC.ExpenseEntryNCDtls)
                    {
                        foreach (ExpenseEntryNCDtlAccViewModel acc in dtl.ExpenseEntryNCDtlAccs)
                        {
                            if (value == null || value.Equals(0) || value.Equals("0"))
                            {
                                return new ValidationResult(name + " cannot be empty.");
                            }
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

    public class JSPValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var name = validationContext.DisplayName;
                var data = (EntryNCViewModelList)validationContext.ObjectInstance;
                if (data.EntryNC.NC_Category_ID == GlobalSystemValues.NC_JS_PAYROLL)
                {
                    int count = 0;
                    while(count < 5)
                    {
                        if (data.EntryNC.ExpenseEntryNCDtls[count].ExpenseEntryNCDtlAccs[0].ExpNCDtlAcc_Amount <= 0)
                        {
                            return new ValidationResult("All fields in JS Payroll are required (except Remarks Period).");
                        }
                        count++;
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
    public class DebitCreditNotNullValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var name = validationContext.DisplayName;
                var data = (EntryNCViewModelList)validationContext.ObjectInstance;
                if (data.EntryNC.NC_Category_ID == GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES)
                {
                    foreach (ExpenseEntryNCDtlViewModel dtl in data.EntryNC.ExpenseEntryNCDtls)
                    {
                        decimal dbAmt = 0;
                        decimal cdAmt = 0;
                        foreach (ExpenseEntryNCDtlAccViewModel accs in dtl.ExpenseEntryNCDtlAccs)
                        {
                            if (accs.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_DEBIT)
                            {
                                dbAmt += accs.ExpNCDtlAcc_Amount;
                            }
                            else if (accs.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_CREDIT ||
                                   accs.ExpNCDtlAcc_Type_ID == GlobalSystemValues.NC_EWT)
                            {
                                cdAmt += accs.ExpNCDtlAcc_Amount;
                            }
                        }
                        if ((dbAmt == 0) || (cdAmt == 0))
                        {
                            return new ValidationResult("Debit and Credit amount needs to be assigned accordingly.");
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
    public class TaxAccountValidations : ValidationAttribute
    {
        private readonly string _ewtID;

        public TaxAccountValidations(string ewtID)
        {
            _ewtID = ewtID;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //Parameter Property and Value
                var property = validationContext.ObjectType.GetProperty(_ewtID);
                var val = property.GetValue(validationContext.ObjectInstance, null);

                var name = validationContext.DisplayName;
                var data = (EntryNCViewModelList)validationContext.ObjectInstance;
                if (data.EntryNC.NC_Category_ID == GlobalSystemValues.NC_PSSC||
                    data.EntryNC.NC_Category_ID == GlobalSystemValues.NC_PCHC ||
                    data.EntryNC.NC_Category_ID == GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES)
                {
                    foreach (ExpenseEntryNCDtlViewModel dtl in data.EntryNC.ExpenseEntryNCDtls)
                    {
                        //Checker for MISC when multiple tax accs are submitted
                        int taxAccCounter = 0;
                        dtl.ExpenseEntryNCDtlAccs.ForEach(x=> {
                            if(x.ExpNCDtlAcc_Acc_ID == (int)val)
                            {
                                taxAccCounter++;
                            }
                        });
                        if (taxAccCounter>1)
                        {
                            return new ValidationResult("Only up to one Tax Account can be assigned.");
                        }
                        if (dtl.ExpNCDtl_TR_ID > 0 && dtl.ExpNCDtl_Vendor_ID <= 0)
                        {
                            return new ValidationResult("If a tax rate is assigned, vendor is required.");
                        }
                        if (dtl.ExpNCDtl_Vendor_ID > 0 && dtl.ExpNCDtl_TR_ID <= 0)
                        {
                            return new ValidationResult("If a vendor is assigned, tax rate is required.");
                        }
                        foreach (ExpenseEntryNCDtlAccViewModel acc in dtl.ExpenseEntryNCDtlAccs)
                        {
                            //if tax account is used, amount is required
                            if(acc.ExpNCDtlAcc_Acc_ID == (int)val && acc.ExpNCDtlAcc_Amount <= 0)
                            {
                                return new ValidationResult("If a tax account is assigned, amount is required.");
                            }
                            //if tax account is used, EWT, Vendor and Tax Base Amount is required
                            if (acc.ExpNCDtlAcc_Acc_ID == (int)val && acc.ExpNCDtlAcc_Amount > 0)
                            {
                                string valStr = "";
                                if (dtl.ExpNCDtl_Vendor_ID.Equals(0))
                                {
                                    valStr += ("Vendor is required. \n");
                                }
                                if (dtl.ExpNCDtl_TR_ID.Equals(0))
                                {
                                    valStr += ("Tax Rate/EWT is required. \n");
                                }
                                if (dtl.ExpNCDtl_TaxBasedAmt.Equals(0))
                                {
                                    valStr += ("Tax Base Amount is required. \n");
                                }
                                if (valStr.Length > 0)
                                {
                                    return new ValidationResult(valStr);
                                }
                            }
                            //if Vendor, EWT and/or Tax Based is used and tax account is assigned, tax account amount is required
                            if (acc.ExpNCDtlAcc_Acc_ID == (int)val && 
                                acc.ExpNCDtlAcc_Amount <=0 && 
                                (dtl.ExpNCDtl_Vendor_ID > 0 ||
                                dtl.ExpNCDtl_TR_ID > 0 ||
                                dtl.ExpNCDtl_TaxBasedAmt > 0 ))
                            {
                                return new ValidationResult("If Vendor, EWT and/or Tax Base Amount is assigned, amount for the tax account i.");
                            }
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
