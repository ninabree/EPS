using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Reports
{
    public class ReportLOIViewModel
    {
        public List<int> Rep_LOIEntryIDList { get; set; }
        public List<string> Rep_DDVNoList { get; set; }
        public string Rep_AmountInString { get; set; }
        public decimal Rep_Amount { get; set; }
        //public string Rep_Curr_Abbr { get; set; }
        public List<LOIAccount> Rep_LOIAccList { get; set; }
        //formatted string to concat after table
        public string Rep_String1 { get; set; }
        public string Rep_String2 { get; set; }
        public string Rep_String3 { get; set; }
        public string Rep_String4 { get; set; }
        //signatories
        public string Rep_Approver_Name { get; set; }
        public string Rep_Verifier1_Name { get; set; }
        //public string Rep_Verifier2_Name { get; set; }
    }
    public class LOIAccount
    {
        public string loi_Emp_Name { get; set; }
        public string loi_Acc_Type { get; set; }
        public string loi_Acc_No { get; set; }
        public decimal loi_Amount { get; set; }
    }
}
