using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMNCCViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NCC_ID { get; set; }
        public int NCC_MasterID { get; set; }
        [Display(Name = "Non Cash Category Name")]
        public string NCC_Name { get; set; }
        [Display(Name = "Non Cash Category Pro-Forma Entries")]
        public string NCC_Pro_Forma { get; set; }
        public int NCC_Creator_ID { get; set; }
        public int NCC_Approver_ID { get; set; }
        [Display(Name = "Non Cash Category Creator Name")]
        public string NCC_Creator_Name { get; set; }
        [Display(Name = "Non Cash Category Approver Name")]
        public string NCC_Approver_Name { get; set; }
        [Display(Name = "Non Cash Category Created Date")]
        public DateTime NCC_Created_Date { get; set; }
        [Display(Name = "Non Cash Category Last Updated")]
        public DateTime NCC_Last_Updated { get; set; }
        [Display(Name = "Non Cash Category Status")]
        public string NCC_Status { get; set; }
        public bool NCC_isDeleted { get; set; }
        public bool NCC_isActive { get; set; }
    }
}
