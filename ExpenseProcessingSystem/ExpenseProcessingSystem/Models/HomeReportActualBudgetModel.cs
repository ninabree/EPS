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
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal ExpenseAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal BudgetBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:M/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ValueDate { get; set; }
    }

    public class AccGroupBudgetModel
    {
        public DateTime StartOfTerm { get; set; }
        public int AccountGroupMasterID { get; set; }
        public string AccountGroupName { get; set; }
        public string Remarks { get; set; }
        public decimal Budget { get; set; }
    }
}
