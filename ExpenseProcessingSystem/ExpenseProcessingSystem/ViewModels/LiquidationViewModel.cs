using ExpenseProcessingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class LiquidationViewModel
    {
        public int entryID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime expenseDate { get; set; }
        public int vendor { get; set; }
        public string expenseYear { get; set; }
        public string expenseId { get; set; }
        public string checkNo { get; set; }
        public int statusID { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public int maker { get; set; }
        public DateTime createdDate { get; set; }
        public List<LiquidationDetailsViewModel> LiquidationDetails { get; set; }
        public LiquidationEntryDetailModel LiqEntryDetails { get; set; }
    }
}
