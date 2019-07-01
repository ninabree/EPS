using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseTransList
    {
        public int TL_ID { get; set; }
        public int TL_ExpenseID { get; set; }
        public bool TL_Liquidation { get; set; }
        public int TL_GoExpress_ID { get; set; }
        public int TL_TransID { get; set; }
    }
}
