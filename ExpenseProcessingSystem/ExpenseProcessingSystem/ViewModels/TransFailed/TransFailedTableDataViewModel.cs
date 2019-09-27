using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.TransFailed
{
    public class TransFailedTableDataViewModel
    {
        public string TF_VALUE_DATE { get; set; }
        public string TF_TRANS_NO { get; set; }
        public string TF_VOUCHER_NO { get; set; }
        public string TF_REMARKS { get; set; }
        public string TF_DEBIT_ACCOUNTS { get; set; }
        public string TF_CREDIT_ACCONTS { get; set; }
        public string TF_STATUS { get; set; }
        public string TF_ACTION_LABEL { get; set; }
        public bool TF_ACTION_IS_DISABLED { get; set; }
        public string TF_GBASE_MESSAGE { get; set; }
        public int TF_STATUS_ID { get; set; }
        public int TF_ACTION_ID { get; set; }
        public int TF_TRANS_LIST_ID { get; set; }
        public int TF_TRANS_ENTRY_ID { get; set; }
        public bool TF_TRANS_IS_LIQ { get; set; }
    }
}
