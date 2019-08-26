using Microsoft.AspNetCore.Mvc.Rendering;
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
        public int account { get; set; }
        public string amount { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int duration { get; set; }
        public SelectList accountsList { get; set; }
        public List<AmortizationBreakdown> brkDown { get; set; }

        public bool readOnly {get;set;}

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
