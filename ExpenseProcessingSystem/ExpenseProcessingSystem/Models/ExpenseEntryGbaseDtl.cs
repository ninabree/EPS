using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryGbaseDtl
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GbaseDtl_ID { get; set; }
        public string GbaseDtl_Document_Type { get; set; }
        public string GbaseDtl_InvoiceNo { get; set; }
        public string GbaseDtl_Description { get; set; }
        public float GbaseDtl_Amount { get; set; }
    }
}
