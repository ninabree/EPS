using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class HomeReportActualBudgetModel
    {
        public int Record_ID { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public string Department { get; set; }
        public double ExpenseAmount { get; set; }
        public double BudgetBalance { get; set; }
        public DateTime ValueDate { get; set; }
    }
}
