using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Reports
{
    public class RepAmortViewModel
    {
        public string PA_VoucherNo { get; set; }
        public string PA_CheckNo { get; set; }
        public string PA_RefNo { get; set; }
        public DateTime PA_Value_Date { get; set; }
        public string PA_Section { get; set; }
        public string PA_Remarks { get; set; }
        public decimal PA_Total_Amt { get; set; }
        public string PA_Vendor_Name { get; set; }
        public string PA_Month { get; set; }
        public int PA_Day { get; set; }
        public int PA_No_Of_Months { get; set; }
        public List<AmortSched> PA_AmortScheds { get; set; }
    }
    public class AmortSched
    {
        public string as_Amort_Name { get; set; }
        public decimal as_Amount { get; set; }
    }
}
