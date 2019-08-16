using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryCashBreakdownModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CashBreak_ID { get; set; }
        public decimal CashBreak_Denomination { get; set; }
        public int CashBreak_NoPcs { get; set; }
        public decimal CashBreak_Amount { get; set; }
        public ExpenseEntryDetailModel ExpenseEntryDetailModel { get; set; }
    }
}
