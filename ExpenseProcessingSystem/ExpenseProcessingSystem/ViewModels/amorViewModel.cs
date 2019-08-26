using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class amorViewModel
    {
        public string voucherNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime sched { get; set; }
        public string maker { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal amount { get; set; }
        public string account { get; set; }
        public string link { get; set; }
    }

    public class AmortizationList
    {
        public List<amorViewModel> amortizations { get; set; }

        public AmortizationList()
        {
            amortizations = new List<amorViewModel>();
        }
    }
}
