using ExpenseProcessingSystem.Services.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewAccountListViewModel
    {
        public List<NewAccountViewModel> NewAccountVM { get; set; }
        public List<SelectListItem> FbtList = new List<SelectListItem>();
        public List<SelectListItem> AccGrp = new List<SelectListItem>();
        public List<SelectListItem> CurrList = new List<SelectListItem>();
    }
    public class NewAccountViewModel
    {
        [Display(Name = "Account Name")]
        [NotNullValidations, TextValidation]
        public string Account_Name { get; set; }

        [Display(Name = "Account Code")]
        [NotNullValidations, TextValidation]
        public string Account_Code { get; set; }

        [Display(Name = "Account Budget Code")]
        [DMAccountFundValidation("Account_Fund")]
        public string Account_Budget_Code { get; set; }

        [Display(Name = "Account No")]
        [NotNullValidations]
        public string Account_No { get; set; }

        [Display(Name = "Account Cust")]
        [NotNullValidations, TextValidation]
        public string Account_Cust { get; set; }

        [Display(Name = "Account Div")]
        public string Account_Div { get; set; }

        [Display(Name = "Account Fund")]
        [NotNullValidations]
        public bool Account_Fund { get; set; }

        [Display(Name = "Account FBT")]
        public int Account_FBT_MasterID { get; set; }

        [Display(Name = "Account Group")]
        [DMAccountFundValidation("Account_Fund")]
        public int Account_Group_MasterID { get; set; }

        [Display(Name = "Account Currency")]
        public int Account_Currency_MasterID { get; set; }
    }
}
