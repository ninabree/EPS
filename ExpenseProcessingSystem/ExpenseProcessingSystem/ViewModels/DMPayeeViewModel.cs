using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMPayeeViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Payee ID")]
        public int Payee_ID { get; set; }
        [Display(Name = "Payee Name")]
        [NotNullValidations, TextValidation]
        public string Payee_Name { get; set; }
        [Display(Name = "Payee TIN")]
        [NotNullValidations, TextValidation]
        public string Payee_TIN { get; set; }
        [Display(Name = "Payee Address")]
        [NotNullValidations, TextValidation]
        public string Payee_Address { get; set; }
        [Display(Name = "Payee Type")]
        [NotNullValidations, TextValidation]
        public string Payee_Type { get; set; }
        [Display(Name = "Payee No")]
        [NotNullValidations, IntegerValidation]
        public int Payee_No { get; set; }
        public int Payee_Creator_ID { get; set; }
        public int Payee_Approver_ID { get; set; }
        [Display(Name = "Payee Created")]
        public DateTime Payee_Created_Date { get; set; }
        [Display(Name = "Payee Last Updated")]
        public DateTime Payee_Last_Updated { get; set; }
        public string Payee_Creator_Name { get; set; }
        public string Payee_Approver_Name { get; set; }
        [Display(Name = "Payee Status")]
        public string Payee_Status { get; set; }
    }
}
