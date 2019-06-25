using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class LiquidationEntryDetailModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Liq_DtlID { get; set; }
        public int Liq_Status { get; set; }
        public DateTime Liq_Created_Date { get; set; }
        public DateTime Liq_LastUpdated_Date { get; set; }
        public int Liq_Created_UserID { get; set; }
        public int Liq_Verifier1 { get; set; }
        public int Liq_Verifier2 { get; set; }
        public int Liq_Approver { get; set; }
        public ExpenseEntryModel ExpenseEntryModel { get; set; }

    }
}
