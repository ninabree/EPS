using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.Entry;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExpenseProcessingSystem.Services.Controller_Services;
using ExpenseProcessingSystem.Models;
using System.Xml.Linq;

namespace ExpenseProcessingSystem.Services.Validations
{
    public class NotNullValidations : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var name = validationContext.DisplayName;
                if (value == null || value.Equals(0) || value.Equals("0"))
                {
                    return new ValidationResult(name + " cannot be empty.");
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
    public class TINLengthValidation : ValidationAttribute
    {
        readonly int TINLength = 15;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //check if valid TIN length
                string val = Convert.ToString(value);
                if (val.Length < TINLength)
                    return new ValidationResult("Minimum length should be 12 digits");
                if (val.Length > TINLength)
                    return new ValidationResult("Maximum length should be 12 digits");
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
    public class EmailValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (!(value == null))
                {
                    var data = (User2ViewModel)validationContext.ObjectInstance;
                    if (!Regex.IsMatch(data.User_Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                    {
                        return new ValidationResult("Invalid Email Format");
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
    public class TextValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var name = validationContext.DisplayName;
                if (!(value == null))
                {
                    Regex r = new Regex("^[a-zA-Z0-9@#$%&*+\\-_(),+':;?.,ñ!\\[\\]\\s\\/]+$");
                    if (!r.IsMatch(value.ToString()))
                    {
                        return new ValidationResult(name + " has an invalid 'string' input");
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
    public class IntegerValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                Regex isInt = new Regex(@"^[0-9]+$");
                var name = validationContext.DisplayName;
                if (!(value.Equals(0)))
                {
                    //if (!(int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture)
                    //      , System.Globalization.NumberStyles.Any
                    //      , NumberFormatInfo.InvariantInfo
                    //      , out var number)))
                    //{
                    if (!isInt.IsMatch(value.ToString()))
                    {
                        return new ValidationResult(name + " has an invalid 'numeric' input");
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
    public class DateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var name = validationContext.DisplayName;
                if (value != null)
                {
                    if (!(DateTime.TryParse(value.ToString(), out DateTime temp)))
                    {
                        return new ValidationResult(name + " is an invalid 'date/time' input");
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
    public class PasswordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                //var hasMiniMaxChars = new Regex(@".{3,20}");
                var hasLowerChar = new Regex(@"[a-z]+");
                var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

                if (!(value == null || value.Equals(0)))
                {
                    //if (!hasMiniMaxChars.IsMatch(value.ToString()))
                    //{
                    //    return new ValidationResult("Password should not be less than 3 characters or greater than 20 characters");
                    //}
                    if (!hasLowerChar.IsMatch(value.ToString()))
                    {
                        return new ValidationResult("Password should contain At least one lower case letter");
                    }
                    if (!hasUpperChar.IsMatch(value.ToString()))
                    {
                        return new ValidationResult("Password should contain At least one upper case letter");
                    }
                    if (!hasNumber.IsMatch(value.ToString()))
                    {
                        return new ValidationResult("Password should contain At least one numeric value");
                    }
                    if (!hasSymbols.IsMatch(value.ToString()))
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
    public class LengthValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (!(value == null || value.Equals(0)))
                {
                    //public List<MsgListRec> MsgList { get; set; }
                    var name = validationContext.DisplayName;
                    if (value.ToString().Length < 3 || value.ToString().Length > 20)
                    {
                        var msgSvc = new MsgService();
                        return new ValidationResult(msgSvc.GetMessage("E0001", name));
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
    public class AmountValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (!(value == null || value.Equals(0)))
                {
                    var name = validationContext.DisplayName;
                    if (value != null)
                    {
                        if (!(float.TryParse(value.ToString(), out float temp)))
                        {
                            return new ValidationResult(name + " is an invalid 'numeric' input.");
                        }
                        else
                        {
                            if (temp == 0)
                            {
                                return new ValidationResult(name + " cannot be less than the value of one (1).");
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
    public class FalseValidation : ValidationAttribute
    {
        private readonly string _CheckBoxProperty;

        public FalseValidation(string CheckBoxProperty)
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
                    return new ValidationResult(name + " is  empty");
                }
                else if (!(int.TryParse(value.ToString(), out int temp)))
                {
                    if (value.GetType() != typeof(string) && temp == 0)
                    {
                        return new ValidationResult(name + " cannot be less than the value of one (1).");
                    }
                }
            }else if((Boolean)val && value == null)
            {
                return new ValidationResult(name + " is  empty");
            }
            return ValidationResult.Success;
        }
    }
    public class EmptyCashBreakdown : ValidationAttribute
    {
        private readonly string _CheckBoxProperty;
        private readonly string _CheckBoxProperty2;

        public EmptyCashBreakdown(string CheckBoxProperty, string CheckBoxProperty2)
        {
            _CheckBoxProperty = CheckBoxProperty;
            _CheckBoxProperty2 = CheckBoxProperty2;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = validationContext.ObjectType.GetProperty(_CheckBoxProperty).GetValue(validationContext.ObjectInstance, null);
            var val2 = validationContext.ObjectType.GetProperty(_CheckBoxProperty2).GetValue(validationContext.ObjectInstance, null);

            if ((string)val != "PCV" && (string)val != "SS")
                return ValidationResult.Success;

            if ((string)val == "SS" && (string)val2 != "PHP")
                return ValidationResult.Success;

            int flag = 0;
            List<CashBreakdown> data = value as List<CashBreakdown>;
            foreach (var i in data)
            {
                if (i.cashNoPC != 0 && i.cashAmount != 0)
                {
                    flag = 1;
                    break;
                }
            }

            if (flag == 0)
            {
                if ((string)val == "SS")
                    return new ValidationResult("All PHP currency item/s must fill up the Cash Breakdown list.");

                if((string)val == "PCV")
                    return new ValidationResult("Must fill up the Cash Breakdown list.");
            }

            return ValidationResult.Success;
        }
    }
    public class EmptyLiquidationCashBreakdown : ValidationAttribute
    {
        private readonly string _CheckBoxProperty;
        private readonly string _CheckBoxProperty2;

        public EmptyLiquidationCashBreakdown(string CheckBoxProperty, string CheckBoxProperty2)
        {
            _CheckBoxProperty = CheckBoxProperty;
            _CheckBoxProperty2 = CheckBoxProperty2;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = validationContext.ObjectType.GetProperty(_CheckBoxProperty).GetValue(validationContext.ObjectInstance, null);
            var val2 = validationContext.ObjectType.GetProperty(_CheckBoxProperty2).GetValue(validationContext.ObjectInstance, null);
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");

            int flag = 0;

            if((int)val2 == 0 && (string)val == xelem.Element("CURRENCY_PHP").Value)
            {
                List<LiquidationCashBreakdown> data = value as List<LiquidationCashBreakdown>;
                foreach (var i in data)
                {
                    if (i.cashNoPC != 0 && i.cashAmount != 0)
                    {
                        flag = 1;
                        break;
                    }
                }

                if (flag == 0)
                {
                    return new ValidationResult("You must fill up the Liqudation for each entries.");
                }
            }

            return ValidationResult.Success;
        }
    }
    public class EmptyLiquidationInterEntity : ValidationAttribute
    {
        private readonly string _CheckBoxProperty;
        private readonly string _CheckBoxProperty2;

        public EmptyLiquidationInterEntity(string CheckBoxProperty, string CheckBoxProperty2)
        {
            _CheckBoxProperty = CheckBoxProperty;
            _CheckBoxProperty2 = CheckBoxProperty2;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = validationContext.ObjectType.GetProperty(_CheckBoxProperty).GetValue(validationContext.ObjectInstance, null);
            var val2 = validationContext.ObjectType.GetProperty(_CheckBoxProperty2).GetValue(validationContext.ObjectInstance, null);
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");

            //int flag = 0;

            if ((int)val2 == 0 && (string)val != xelem.Element("CURRENCY_PHP").Value)
            {
                List<LiquidationInterEntityModel> data = value as List<LiquidationInterEntityModel>;

                if (data == null || data.Count() == 0)
                {
                    return new ValidationResult("You must fill up the Liqudation for each entries.");
                }
            }

            return ValidationResult.Success;
        }
    }
    public class ListValidation : ValidationAttribute
    {
        private readonly string _CheckBoxProperty;

        public ListValidation(string CheckBoxProperty)
        {
            _CheckBoxProperty = CheckBoxProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_CheckBoxProperty);
            var val = property.GetValue(validationContext.ObjectInstance, null);

            var name = validationContext.DisplayName;
            var data = (EntryDDVViewModel)validationContext.ObjectInstance;

            if ((Boolean)val)
            {
                foreach (var dtl in data.interDetails.interPartList)
                {
                    foreach (var acc in dtl.ExpenseEntryInterEntityAccs)
                    {
                        if ((acc.Inter_Curr_ID <= 0) || (acc.Inter_Amount <= 0))
                        {
                            return new ValidationResult(name + " is Required. Kindly fill-up the required form.");
                        }
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
