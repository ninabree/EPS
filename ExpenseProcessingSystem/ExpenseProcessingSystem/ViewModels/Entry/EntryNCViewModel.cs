using ExpenseProcessingSystem.Services.Validations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class EntryNCViewModel
    {
        [NotNullValidations]
        [Display(Name = "Category of Entry")]
        public int NC_Category_ID { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Debit - Total Amount")]
        public float NC_DebitAmt { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Credit - Total Amount")]
        public float NC_CredAmt { get; set; }
        [NotNullValidations, AmountValidation]
        [Display(Name = "Total Amount")]
        public float NC_TotalAmt { get; set; }
        public List<ExpenseEntryNCDtlViewModel> ExpenseEntryNCDtls { get; set; }


        public EntryNCViewModel()
        {
            ExpenseEntryNCDtls = new List<ExpenseEntryNCDtlViewModel>();
        }
    }
    public class EntryNCViewModelList
    {
        public int entryID { get; set; }
        public SysValViewModel systemValues { get; set; }
        public DateTime expenseDate { get; set; }
        public int statusID { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public int maker { get; set; }
        public EntryNCViewModel EntryNC { get; set; }
        public List<SelectListItem> category_of_entry { get; set; }
        public List<SelectListItem> accountList { get; set; }

        public EntryNCViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryNC = new EntryNCViewModel();
        }
    }
}
