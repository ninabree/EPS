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

        public decimal CBD_opeBalance { get; set; }
        public decimal CBD_recieve { get; set; }
        public decimal CBD_disburse { get; set; }
        public decimal CBD_closeBalance { get; set; }

        public bool CBD_ConfirmClose { get; set; }
        public string CBD_Comment { get; set; }

        public int CBD_oneK { get; set; }
        public decimal CBD_oneKAmount { get; set; }
        public int CBD_fiveH { get; set; }
        public decimal CBD_fiveHAmount { get; set; }
        public int CBD_twoH { get; set; }
        public decimal CBD_twoHAmount { get; set; }
        public int CBD_oneH { get; set; }
        public decimal CBD_oneHAmount { get; set; }
        public int CBD_fifty { get; set; }
        public decimal CBD_fiftyAmount { get; set; }
        public int CBD_twenty { get; set; }
        public decimal CBD_twentyAmount { get; set; }
        public int CBD_ten { get; set; }
        public decimal CBD_tenAmount { get; set; }
        public int CBD_five { get; set; }
        public decimal CBD_fiveAmount { get; set; }
        public int CBD_one { get; set; }
        public decimal CBD_oneAmount { get; set; }
        public int CBD_c25 { get; set; }
        public decimal CBD_c25Amount { get; set; }
        public int CBD_c10 { get; set; }
        public decimal CBD_c10Amount { get; set; }
        public int CBD_c5 { get; set; }
        public decimal CBD_c5Amount { get; set; }
        public int CBD_c1 { get; set; }
        public decimal CBD_c1Amount { get; set; }

        public decimal CBD_coinTotal { get; set; }
        public decimal CBD_billTotal { get; set; } 

        public bool enableBtn { get; set; }
        public string message { get; set; }
    }
}
