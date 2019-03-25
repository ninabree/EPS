using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMEWTViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EWT_ID { get; set; }
        public int EWT_MasterID { get; set; }
        [Display(Name = "EWT Nature")]
        public string EWT_Nature { get; set; }
        [Display(Name = "EWT Tax Rate")]
        public int EWT_Tax_Rate { get; set; }
        [Display(Name = "ATC")]
        public string EWT_ATC { get; set; }
        [Display(Name = "EWT Tax Rate Description")]
        public string EWT_Tax_Rate_Desc { get; set; }
        public int EWT_Creator_ID { get; set; }
        public int EWT_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string EWT_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string EWT_Approver_Name { get; set; }
        [Display(Name = "EWT Created Date")]
        public DateTime EWT_Created_Date { get; set; }
        [Display(Name = "EWT Last Updated")]
        public DateTime EWT_Last_Updated { get; set; }
        [Display(Name = "EWT Status")]
        public string EWT_Status { get; set; }
        public bool EWT_isDeleted { get; set; }
    }
}
