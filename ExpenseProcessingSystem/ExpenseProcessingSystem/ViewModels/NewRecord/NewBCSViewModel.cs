using ExpenseProcessingSystem.Services.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewBCSListViewModel
    {
        public List<NewBCSViewModel> NewBCSVM { get; set; }
    }
    public class NewBCSViewModel
    {
        [Display(Name = "BCS Name")]
        [NotNullValidations, TextValidation]
        public int BCS_User_ID { get; set; }
        public string BCS_Name { get; set; }
        [Display(Name = "BCS TIN")]
        [NotNullValidations, TINLengthValidation]
        public string BCS_TIN { get; set; }
        [Display(Name = "BCS Position")]
        [NotNullValidations, TextValidation]
        public string BCS_Position { get; set; }
        [Display(Name = "BCS Signature")]
        public IFormFile BCS_Signatures { get; set; }
    }
}
