using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class LiquidationCashBreakdownModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LiqCashBreak_ID { get; set; }
        public double LiqCashBreak_Denomination { get; set; }
        public int LiqCashBreak_NoPcs { get; set; }
        public decimal LiqCashBreak_Amount { get; set; }
        public ExpenseEntryDetailModel ExpenseEntryDetailModel { get; set; }
    }
}
