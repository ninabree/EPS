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
        public int Liq_AccountID_1_1 { get; set; }
        public double Liq_InterRate_1_1 { get; set; }
        public int Liq_CCY_1_1 { get; set; }
        public double Liq_Amount_1_1 { get; set; }
        public string Liq_DebitCred_1_2 { get; set; }
        public int Liq_AccountID_1_2 { get; set; }
        public double Liq_InterRate_1_2 { get; set; }
        public int Liq_CCY_1_2 { get; set; }
        public double Liq_Amount_1_2 { get; set; }
        public string Liq_DebitCred_2_1 { get; set; }
        public int Liq_AccountID_2_1 { get; set; }
        public double Liq_InterRate_2_1 { get; set; }
        public int Liq_CCY_2_1 { get; set; }
        public double Liq_Amount_2_1 { get; set; }
        public string Liq_DebitCred_2_2 { get; set; }
        public int Liq_AccountID_2_2 { get; set; }
        public double Liq_InterRate_2_2 { get; set; }
        public int Liq_CCY_2_2 { get; set; }
        public double Liq_Amount_2_2 { get; set; }
        public string Liq_DebitCred_3_1 { get; set; }
        public int Liq_AccountID_3_1 { get; set; }
        public double Liq_InterRate_3_1 { get; set; }
        public int Liq_CCY_3_1 { get; set; }
        public double Liq_Amount_3_1 { get; set; }
        public string Liq_DebitCred_3_2 { get; set; }
        public int Liq_AccountID_3_2 { get; set; }
        public double Liq_InterRate_3_2 { get; set; }
        public int Liq_CCY_3_2 { get; set; }
        public double Liq_Amount_3_2 { get; set; }
        public int Liq_VendorID { get; set; }
        public double Liq_TaxRate { get; set; }
        public ExpenseEntryDetailModel ExpenseEntryDetailModel { get; set; }
    }
}
