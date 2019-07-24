using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class PrintStatusModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PS_ID { get; set; }
        public int PS_EntryID { get; set; }
        public int PS_EntryDtlID { get; set; }
        public int PS_Type { get; set; }
        public bool PS_LOI { get; set; }
        public bool PS_BIR2307 { get; set; }
        public bool PS_CDD { get; set; }
        public bool PS_Check { get; set; }
        public bool PS_Voucher { get; set; }
    }
}
