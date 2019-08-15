using System.ComponentModel.DataAnnotations;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class AccGroupFiltersViewModel
    {
        [Display(Name = "Account Group Name")]
        public string AGF_Name { get; set; }
        [Display(Name = "Account Group Code")]
        public string AGF_Code { get; set; }
        [Display(Name = "Creator Name")]
        public string AGF_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string AGF_Approver_Name { get; set; }
        [Display(Name = "Account Status")]
        public string AGF_Status_Name { get; set; }
    }
}
