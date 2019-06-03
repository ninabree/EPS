using ExpenseProcessingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ExpenseProcessingSystem.ConstantData.DeniminationValues;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class PCVCashBreakdownViewModel
    {
        public string id { get; set; }
        public string vendor { get; set; }
        public string account { get; set; }
        public string accountName { get; set; }
        public double amount { get; set; }
        public string screencode { get; set; }
        public List<ExpenseEntryCashBreakdownModel> cashBreakdown { get; set; }
    }
}
