using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double ExpenseAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double BudgetBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:M/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ValueDate { get; set; }
    }
}
