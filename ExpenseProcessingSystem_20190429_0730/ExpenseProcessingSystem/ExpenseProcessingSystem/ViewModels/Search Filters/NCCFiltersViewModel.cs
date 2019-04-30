using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class NCCFiltersViewModel
    {
        [Display(Name = "Non Cash Category Name")]
        public string NF_Name { get; set; }
        [Display(Name = "Non Cash Category Pro-Forma Entries")]
        public string NF_Pro_Forma { get; set; }
        [Display(Name = "Non Cash Category Creator Name")]
        public string NF_Creator_Name { get; set; }
        [Display(Name = "Non Cash Category Approver Name")]
        public string NF_Approver_Name { get; set; }
        [Display(Name = "Non Cash Category Status")]
        public string NF_Status { get; set; }
    }
}
