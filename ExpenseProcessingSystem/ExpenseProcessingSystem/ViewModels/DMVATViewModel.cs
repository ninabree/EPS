using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMVATViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VAT_ID { get; set; }
        public int VAT_MasterID { get; set; }
        [Display(Name = "VAT Name")]
        public string VAT_Name { get; set; }
        [Display(Name = "VAT Rate")]
        public float VAT_Rate { get; set; }
        public int VAT_Creator_ID { get; set; }
        public int VAT_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string VAT_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string VAT_Approver_Name { get; set; }
        [Display(Name = "VAT Created Date")]
        [DataType(DataType.Date)]
        public DateTime VAT_Created_Date { get; set; }
        [Display(Name = "VAT Last Updated")]
        [DataType(DataType.Date)]
        public DateTime VAT_Last_Updated { get; set; }
        [Display(Name = "VAT Status")]
        public int VAT_Status_ID { get; set; }
        public string VAT_Status { get; set; }
        public bool VAT_isDeleted { get; set; }
    }
}
