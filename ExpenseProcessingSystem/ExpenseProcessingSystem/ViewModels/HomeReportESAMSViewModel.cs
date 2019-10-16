using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class HomeReportESAMSViewModel
    {
        public string SeqNo { get; set; }
        public string DebCredType { get; set; }
        public string GbaseRemark { get; set; }
        [DisplayFormat(DataFormatString = "{0:M/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SettleDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal DRAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal CRAmount { get; set; }
        public decimal BudgetAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal Balance { get; set; }
        public string DHName { get; set; }
        public string ApprvName { get; set; }
        public string MakerName { get; set; }
        public string Remarks { get; set; }

    }
}
