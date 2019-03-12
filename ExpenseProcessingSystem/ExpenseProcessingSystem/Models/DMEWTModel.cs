using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMEWTModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EWT_ID { get; set; }
        public string EWT_Nature { get; set; }
        public int EWT_Tax_Rate { get; set; }
        public string EWT_ATC { get; set; }
        public string EWT_Tax_Rate_Desc { get; set; }
        public int EWT_Creator_ID { get; set; }
        public int EWT_Approver_ID { get; set; }
        public DateTime EWT_Created_Date { get; set; }
        public DateTime EWT_Last_Updated { get; set; }
        public string EWT_Status { get; set; }
        public bool EWT_isDeleted { get; set; }
    }
}
