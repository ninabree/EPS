using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class ClosingBrkDwnViewModel
    {
        public bool CBD_displayMode { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CBD_Date { get; set; }

        public double CBD_opeBalance { get; set; }
        public double CBD_recieve { get; set; }
        public double CBD_disburse { get; set; }
        public double CBD_closeBalance { get; set; }

        public bool CBD_ConfirmClose { get; set; }
        public string CBD_Comment { get; set; }

        public int CBD_oneK { get; set; }
        public double CBD_oneKAmount { get; set; }
        public int CBD_fiveH { get; set; }
        public double CBD_fiveHAmount { get; set; }
        public int CBD_twoH { get; set; }
        public double CBD_twoHAmount { get; set; }
        public int CBD_oneH { get; set; }
        public double CBD_oneHAmount { get; set; }
        public int CBD_fifty { get; set; }
        public double CBD_fiftyAmount { get; set; }
        public int CBD_twenty { get; set; }
        public double CBD_twentyAmount { get; set; }
        public int CBD_ten { get; set; }
        public double CBD_tenAmount { get; set; }
        public int CBD_five { get; set; }
        public double CBD_fiveAmount { get; set; }
        public int CBD_one { get; set; }
        public double CBD_oneAmount { get; set; }
        public int CBD_c25 { get; set; }
        public double CBD_c25Amount { get; set; }
        public int CBD_c10 { get; set; }
        public double CBD_c10Amount { get; set; }
        public int CBD_c5 { get; set; }
        public double CBD_c5Amount { get; set; }
        public int CBD_c1 { get; set; }
        public double CBD_c1Amout { get; set; }
    }
}
