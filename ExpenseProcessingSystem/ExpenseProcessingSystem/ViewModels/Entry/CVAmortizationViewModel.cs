using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class CVAmortizationViewModel
    {
        public string id { get; set; }
        public string vendor { get; set; }
        public string account { get; set; }
        public string amount { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int duration { get; set; }
        public List<AmortizationBreakdown> brkDown { get; set; }

        public CVAmortizationViewModel()
        {
            brkDown = new List<AmortizationBreakdown>();
        }
    }

    public class AmortizationBreakdown
    {
        DateTime amorDate { get; set; }
        float amorAmount { get; set; }
    }
}
