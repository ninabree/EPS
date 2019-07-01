using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryAmortizationModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Amor_ID { get; set; }
        public DateTime Amor_Sched_Date { get; set; }
        public float Amor_Price { get; set; }
        public string status { get; set; }
        public ExpenseEntryDetailModel ExpenseEntryDetailModel { get; set; }
    }
}
