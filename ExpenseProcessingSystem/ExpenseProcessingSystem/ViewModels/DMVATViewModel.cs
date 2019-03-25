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
        public string VAT_Rate { get; set; }
        public int VAT_Creator_ID { get; set; }
        public int VAT_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string VAT_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string VAT_Approver_Name { get; set; }
        [Display(Name = "VAT Created Date")]
        public DateTime VAT_Created_Date { get; set; }
        [Display(Name = "VAT Last Updated")]
        public DateTime VAT_Last_Updated { get; set; }
        [Display(Name = "VAT Status")]
        public string VAT_Status { get; set; }
        public bool VAT_isDeleted { get; set; }
    }
}
