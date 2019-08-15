using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryNCModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpNC_ID { get; set; }
        public int ExpNC_Category_ID { get; set; }
        public float ExpNC_DebitAmt { get; set; }
        public float ExpNC_CredAmt { get; set; }
        public float ExpNC_CS_DebitAmt { get; set; }
        public float ExpNC_CS_CredAmt { get; set; }
        public string ExpNC_CS_Period { get; set; }
        public float ExpNC_IE_DebitAmt { get; set; }
        public float ExpNC_IE_CredAmt { get; set; }
        public ExpenseEntryModel ExpenseEntryModel { get; set; }

        public ICollection<ExpenseEntryNCDtlModel> ExpenseEntryNCDtls { get; set; }
    }
}
