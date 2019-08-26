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

        public List<string> messages { get; set; }

        public string rbuStatus { get; set; }
        public string fcduStatus { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal pettyBegBalance { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal cashIn { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal cashOut { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal endBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal opeBal { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal recieve { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal Disbursed { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal closeBal { get; set; }

        public int oneK { get; set; }
        public decimal oneKAmount { get; set; }
        public int fiveH { get; set; }
        public decimal fiveHAmount { get; set; }
        public int twoH { get; set; }
        public decimal twoHAmount { get; set; }
        public int oneH { get; set; }
        public decimal oneHAmount { get; set; }
        public int fifty { get; set; }
        public decimal fiftyAmount { get; set; }
        public int twenty { get; set; }
        public decimal twentyAmount { get; set; }
        public int ten { get; set; }
        public decimal tenAmount { get; set; }
        public int five { get; set; }
        public decimal fiveAmount { get; set; }
        public int one { get; set; }
        public decimal oneAmount { get; set; }
        public int c25 { get; set; }
        public decimal c25Amount { get; set; }
        public int c10 { get; set; }
        public decimal c10Amount { get; set; }
        public int c5 { get; set; }
        public decimal c5Amount { get; set; }
        public int c1 { get; set; }
        public decimal c1Amout { get; set; }

        public decimal billTotal { get; set; }
        public decimal coinTotal { get; set; }

        public bool pcOpen { get; set; }

        public ClosingViewModel()
        {
            messages = new List<string>();
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
        public decimal amount { get; set; } 
        public int transCount { get; set; } 
        public string status { get; set; }
    }
}
