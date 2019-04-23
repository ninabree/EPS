using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMFBTViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FBT_ID { get; set; }
        public int FBT_MasterID { get; set; }
        [Display(Name = "FBT Name")]
        public string FBT_Name { get; set; }
        [Display(Name = "FBT Formula")]
        public string FBT_Formula { get; set; }
        [Display(Name = "FBT Tax Rate")]
        public int FBT_Tax_Rate { get; set; }
        public int FBT_Creator_ID { get; set; }
        public int FBT_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string FBT_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string FBT_Approver_Name { get; set; }
        [Display(Name = "FBT Created Date")]
        public DateTime FBT_Created_Date { get; set; }
        [Display(Name = "FBT Last Updated")]
        public DateTime FBT_Last_Updated { get; set; }
        [Display(Name = "FBT Status")]
        public string FBT_Status { get; set; }
        public bool FBT_isDeleted { get; set; }
    }
}
