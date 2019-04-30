using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class CheckFiltersViewModel
    {
        [Display(Name = "Input Date")]
        [DataType(DataType.DateTime)]
        public DateTime CKF_Input_Date { get; set; }
        [Display(Name = "Series From")]
        public string CKF_Series_From { get; set; }
        [Display(Name = "Series To")]
        public string CKF_Series_To { get; set; }
        [Display(Name = "Bank Information")]
        public string CKF_Bank_Info { get; set; }
        [Display(Name = "Name")]
        public string CKF_Name { get; set; }
        [Display(Name = "Check Creator Name")]
        public string CKF_Creator_Name { get; set; }
        [Display(Name = "Check Approver Name")]
        public string CKF_Approver_Name { get; set; }
        [Display(Name = "Status")]
        public string CKF_Status { get; set; }
    }
}
