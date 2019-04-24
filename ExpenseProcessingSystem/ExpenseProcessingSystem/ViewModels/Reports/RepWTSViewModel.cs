using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Reports
{
    public class RepWTSViewModelList
    {
        public List<RepWTSViewModel> WTSList { get; set; }
    }
    public class RepWTSViewModel
    {
        public int WTS_Voucher_No { get; set; }
        public int WTS_Check_No { get; set; }
        public DateTime WTS_Val_Date { get; set; }
        public int WTS_Ref_No { get; set; }
        public string WTS_Section { get; set; }
        public string WTS_Remarks { get; set; }
        public double WTS_Deb_Cred { get; set; }
        public int WTS_Currency_ID { get; set; }
        public double WTS_Amount { get; set; }
        public string WTS_Cust { get; set; }
        public int WTS_Acc_Code { get; set; }
        public int WTS_Acc_No { get; set; }
        public string WTS_Acc_Name { get; set; }
        public double WTS_Exchange_Rate { get; set; }
        public int WTS_Contra_Currency_ID { get; set; }
        public string WTS_Fund { get; set; }
        public string WTS_Advice_Print { get; set; }
        public string WTS_Details { get; set; }
        public string WTS_Entity { get; set; }
        public string WTS_Division { get; set; }
        public double WTS_Inter_Amount { get; set; }
        public double WTS_Inter_Rate { get; set; }
    }
}
