using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Acc_UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Acc_Password { get; set; }
    }
}
