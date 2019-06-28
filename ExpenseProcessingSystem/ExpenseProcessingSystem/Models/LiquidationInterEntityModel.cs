using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class LiquidationInterEntityModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string Liq_DebitCred_1_1 { get; set; }
        public string Liq_AccountID_1_1 { get; set; }
        public double Liq_InterRate_1_1 { get; set; }
        public string Liq_CCY_1_1 { get; set; }
        public double Liq_Amount_1_1 { get; set; }
        public string Liq_DebitCred_1_2 { get; set; }
        public string Liq_AccountID_1_2 { get; set; }
        public double Liq_InterRate_1_2 { get; set; }
        public string Liq_CCY_1_2 { get; set; }
        public double Liq_Amount_1_2 { get; set; }
        public string Liq_DebitCred_2_1 { get; set; }
        public string Liq_AccountID_2_1 { get; set; }
        public double Liq_InterRate_2_1 { get; set; }
        public string Liq_CCY_2_1 { get; set; }
        public double Liq_Amount_2_1 { get; set; }
        public string Liq_DebitCred_2_2 { get; set; }
        public string Liq_AccountID_2_2 { get; set; }
        public double Liq_InterRate_2_2 { get; set; }
        public string Liq_CCY_2_2 { get; set; }
        public double Liq_Amount_2_2 { get; set; }
        public ExpenseEntryDetailModel ExpenseEntryDetailModel { get; set; }
    }
}
