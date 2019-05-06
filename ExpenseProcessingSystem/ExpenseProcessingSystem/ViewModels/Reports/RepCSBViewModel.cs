using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Reports
{
    public class Temp_RepCSBViewModelList
    {
        public int CSB_Customer_No { get; set; }
        public int CSB_Account_No { get; set; }
        public string CSB_Account_Name { get; set; }
        public DateTime CSB_Start_Date { get; set; }
        public DateTime CSB_End_Date { get; set; }
        public int CSB__Currency_ID { get; set; }
        public string CSB__Currency_Name { get; set; }
        public List<Temp_RepCSBViewModel> CSBList { get; set; }
    }
    public class Temp_RepCSBViewModel
    {
        public int CSB_ID { get; set; }
        public DateTime CSB_Date { get; set; }
        public double CSB_Debit { get; set; }
        public double CSB_Credit { get; set; }
        public string CSB_Remarks { get; set; }
        public int CSB_Ref_No { get; set; }
        public double CSB_Balance { get; set; }
    }
}
