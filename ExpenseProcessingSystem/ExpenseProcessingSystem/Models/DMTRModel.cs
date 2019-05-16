using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMTRModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TR_ID { get; set; }
        public int TR_MasterID { get; set; }
        public string TR_WT_Title { get; set; }
        public string TR_Nature { get; set; }
        public string TR_ATC { get; set; }
        public string TR_Nature_Income_Payment { get; set; }
        public float TR_Tax_Rate { get; set; }
        public int TR_Creator_ID { get; set; }
        public int TR_Approver_ID { get; set; }
        public DateTime TR_Created_Date { get; set; }
        public DateTime TR_Last_Updated { get; set; }
        public int TR_Status_ID { get; set; }
        public bool TR_isDeleted { get; set; }
        public bool TR_isActive { get; set; }
    }
}
