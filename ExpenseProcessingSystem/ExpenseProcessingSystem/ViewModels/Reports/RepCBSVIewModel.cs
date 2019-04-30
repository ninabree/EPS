using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Reports
{
    public class RepCBSViewModelList
    {
        public string CBS_Cust_Abbr { get; set; } // from accounts TBL
        public int CBS_Acc_Code { get; set; } // from accounts TBL
        public int CBS_Acc_No { get; set; } // from accounts TBL
        public string CBS_Acc_Name { get; set; } // from accounts TBL
        public int CBS_Currency_ID { get; set; } // from curr TBL
        public DateTime CBS_Date_From { get; set; } // from Form
        public DateTime CBS_Date_To { get; set; } // from Form
        public List<RepCBSViewModel> CBSList { get; set; }
    }
    public class RepCBSViewModel
    {
        public int CBS_ID { get; set; }
        public DateTime CBS_Date { get; set; } // from Entries TBLs
        public double CBS_Debit { get; set; } // from Entries TBLs
        public double CBS_Credit { get; set; } // from Entries TBLs
        public string CBS_Remarks { get; set; } // from Entries TBLs
        public double CBS_Ref_No { get; set; } // from Entries TBLs
        public double CBS_Balance { get; set; } // from Computation
    }
}
