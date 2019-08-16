using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class CDDISValueContentsViewModel
    {
        public string DEBIT_CREDIT { get; set; }
        public string CCY { get; set; }
        public decimal AMOUNT { get; set; }
        public string CUSTOMER_ABBR { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string ACCOUNT_NO { get; set; }
        public decimal EXCHANGE_RATE { get; set; }
        public string CONTRA_CCY { get; set; }
        public string FUND { get; set; }
        public string CHECK_NO { get; set; }
        public DateTime AVAILABLE_DATE { get; set; }
        public string ADVICE { get; set; }
        public string DETAILS { get; set; }
        public string ENTITY { get; set; }
        public string DIVISION { get; set; }
        public decimal INTER_AMOUNT { get; set; }
        public decimal INTER_RATE { get; set; }
    }
    public class CDDISValuesVIewModel
    {
        public DateTime VALUE_DATE { get; set; }
        public int CURRENCY { get; set; }
        public string COMMENT { get; set; }
        public string SECTION { get; set; }
        public string REMARKS { get; set; }
        public string SCHEME_NO { get; set; }
        public string MEMO { get; set; }
        
        public List<CDDISValueContentsViewModel> CDDContents { get; set; }

        public CDDISValuesVIewModel()
        {
            CDDContents = new List<CDDISValueContentsViewModel>();
        }
    }
}
