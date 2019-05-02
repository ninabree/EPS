using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMTRViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TR_ID { get; set; }
        public int TR_MasterID { get; set; }
        [Display(Name = "Withholding Tax Title")]
        public string TR_WT_Title { get; set; }
        [Display(Name = "Nature")]
        public string TR_Nature { get; set; }
        [Display(Name = "Tax Rate")]
        public float TR_Tax_Rate { get; set; }
        [Display(Name = "ATC")]
        public string TR_ATC { get; set; }
        [Display(Name = "Nature of Income Payment")]
        public string TR_Nature_Income_Payment { get; set; }
        public int TR_Creator_ID { get; set; }
        public int TR_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string TR_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string TR_Approver_Name { get; set; }
        [Display(Name = "Created Date")]
        public DateTime TR_Created_Date { get; set; }
        [Display(Name = "Last Updated")]
        public DateTime TR_Last_Updated { get; set; }
        [Display(Name = "Status")]
        public string TR_Status { get; set; }
        public bool TR_isDeleted { get; set; }
    }
}
