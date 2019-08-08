using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMCurrencyViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Curr_ID { get; set; }
        public int Curr_MasterID { get; set; }
        [Display(Name = "Currency Name")]
        public string Curr_Name { get; set; }
        [Display(Name = "Currency CCY ABBR")]
        public string Curr_CCY_ABBR { get; set; }
        public int Curr_Creator_ID { get; set; }
        public int Curr_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string Curr_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string Curr_Approver_Name { get; set; }
        [Display(Name = "Currency Created Date")]
        [DataType(DataType.Date)]
        public DateTime Curr_Created_Date { get; set; }
        [Display(Name = "Currency Last Updated")]
        [DataType(DataType.Date)]
        public DateTime Curr_Last_Updated { get; set; }
        [Display(Name = "Currency Status")]
        public int Curr_Status_ID { get; set; }
        public string Curr_Status { get; set; }
        public bool Curr_isDeleted { get; set; }
    }
}
