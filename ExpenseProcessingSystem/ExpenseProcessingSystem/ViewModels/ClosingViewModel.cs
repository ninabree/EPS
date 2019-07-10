using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class ClosingViewModel
    {
        public List<CloseItems> rbuItems { get; set; }
        public List<CloseItems> fcduItems { get; set; }

        public double pettyBegBalance { get; set; }
        public double cashIn { get; set; }
        public double cashOut { get; set; }
        public double endBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }

        public double opeBal { get; set; }
        public double recieve { get; set; }
        public double Disbursed { get; set; }
        public double closeBal { get; set; }



        public ClosingViewModel()
        {
            rbuItems = new List<CloseItems>();
            fcduItems = new List<CloseItems>();
        }
    }
    
    public class CloseItems
    {
        public string gBaseTrans { get; set; }
        public string expTrans { get; set; }
        public string particulars { get; set; }
        public string ccy { get; set; }
        public double amount { get; set; }
        public int transCount { get; set; }
        public string status { get; set; }
    }
}
