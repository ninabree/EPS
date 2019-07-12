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

        public string rbuStatus { get; set; }
        public string fcduStatus { get; set; }

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

        public int oneK { get; set; }
        public double oneKAmount { get; set; }
        public int fiveH { get; set; }
        public double fiveHAmount { get; set; }
        public int twoH { get; set; }
        public double twoHAmount { get; set; }
        public int oneH { get; set; }
        public double oneHAmount { get; set; }
        public int fifty { get; set; }
        public double fiftyAmount { get; set; }
        public int twenty { get; set; }
        public double twentyAmount { get; set; }
        public int ten { get; set; }
        public double tenAmount { get; set; }
        public int five { get; set; }
        public double fiveAmount { get; set; }
        public int one { get; set; }
        public double oneAmount { get; set; }
        public int c25 { get; set; }
        public double c25Amount { get; set; }
        public int c10 { get; set; }
        public double c10Amount { get; set; }
        public int c5 { get; set; }
        public double c5Amount { get; set; }
        public int c1 { get; set; }
        public double c1Amout { get; set; }

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
