using ExpenseProcessingSystem.Services.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMNCC2ViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NCC_ID { get; set; }
        public int NCC_MasterID { get; set; }
        [Display(Name = "Category Name")]
        public string NCC_Name { get; set; }
        [Display(Name = "Pro-Forma Entries")]
        public IFormFile NCC_Pro_Forma { get; set; }
        public string NCC_Pro_Forma_Name { get; set; }
        public int NCC_Creator_ID { get; set; }
        public int NCC_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string NCC_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string NCC_Approver_Name { get; set; }
        [Display(Name = "Created Date")]
        public DateTime NCC_Created_Date { get; set; }
        [Display(Name = "Last Updated")]
        public DateTime NCC_Last_Updated { get; set; }
        [Display(Name = "Status")]
        public string NCC_Status { get; set; }
        public bool NCC_isDeleted { get; set; }
        public bool NCC_isActive { get; set; }
    }
}
