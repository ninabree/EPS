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
    public class DMBCS2ViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BCS_ID { get; set; }
        public int BCS_MasterID { get; set; }
        [Display(Name = "BCS Name")]
        public string BCS_Name { get; set; }
        [Display(Name = "BCS TIN")]
        public string BCS_TIN { get; set; }
        [Display(Name = "BCS Position")]
        public string BCS_Position { get; set; }
        [Display(Name = "BCS Signature")]
        public IFormFile BCS_Signatures { get; set; }
        public string BCS_Signatures_Name { get; set; }
        public int BCS_Creator_ID { get; set; }
        public int BCS_Approver_ID { get; set; }
        [Display(Name = "BCS Creator Name")]
        public string BCS_Creator_Name { get; set; }
        [Display(Name = "BCS Approver Name")]
        public string BCS_Approver_Name { get; set; }
        [Display(Name = "BCS Created Date")]
        public DateTime BCS_Created_Date { get; set; }
        [Display(Name = "BCS Last Updated")]
        public DateTime BCS_Last_Updated { get; set; }
        [Display(Name = "BCS Status")]
        public int BCS_Status_ID { get; set; }
        public string BCS_Status { get; set; }
        public bool BCS_isDeleted { get; set; }
        public bool BCS_isActive { get; set; }
    }
}
