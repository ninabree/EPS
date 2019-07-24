using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ReversalEntryModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Reversal_ID { get; set; }
        public int Reversal_ExpenseEntryID { get; set; }
        public int Reversal_ExpenseType { get; set; }
        public int Reversal_ExpenseDtlID { get; set; }
        public int Reversal_NonCashDtlID { get; set; }
        public int Reversal_LiqDtlID { get; set; }
        public int Reversal_LiqInterEntityID { get; set; }
        public int Reversal_GOExpressID { get; set; }
        public int Reversal_GOExpressHistID { get; set; }
        public int Reversal_TransNo { get; set; }
        public DateTime Reversal_ReversedDate { get; set; }
        public int Reversal_ReversedUserID { get; set; }
    }
}
