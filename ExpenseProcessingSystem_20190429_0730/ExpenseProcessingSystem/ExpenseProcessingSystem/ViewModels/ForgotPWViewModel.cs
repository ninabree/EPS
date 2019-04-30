using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class ForgotPWViewModel
    {
        public int Forgot_UserId { get; set; }
        [Display(Name="User Name")]
        public string Forgot_UserName { get; set; }
        [Display(Name = "Email Address")]
        //[EmailValidation]
        public string Forgot_Email { get; set; }

    }
}
