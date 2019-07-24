using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryNCDtlModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpNCDtl_ID { get; set; }
        public string ExpNCDtl_Remarks_Desc { get; set; }
        public string ExpNCDtl_Remarks_Period { get; set; }
        public int ExpNCDtl_Vendor_ID { get; set; }
        public int ExpNCDtl_TR_ID { get; set; }
        public ExpenseEntryNCModel ExpenseEntryNCModel { get; set; }

        public ICollection<ExpenseEntryNCDtlAccModel> ExpenseEntryNCDtlAccs { get; set; }
    }
}
