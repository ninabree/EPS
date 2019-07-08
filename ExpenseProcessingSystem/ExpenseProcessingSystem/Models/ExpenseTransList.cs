using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseTransList
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TL_ID { get; set; }
        public int TL_ExpenseID { get; set; }
        public int TL_ExpenseDtlID { get; set; }
        public bool TL_Liquidation { get; set; }
        public int TL_GoExpress_ID { get; set; }
        public int TL_TransID { get; set; }
    }
}
