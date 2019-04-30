using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMVATModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VAT_ID { get; set; }
        public int VAT_MasterID { get; set; }
        public string VAT_Name { get; set; }
        public string VAT_Rate { get; set; }
        public int VAT_Creator_ID { get; set; }
        public int VAT_Approver_ID { get; set; }
        public DateTime VAT_Created_Date { get; set; }
        public DateTime VAT_Last_Updated { get; set; }
        public string VAT_Status { get; set; }
        public bool VAT_isDeleted { get; set; }
        public bool VAT_isActive { get; set; }
    }
}
