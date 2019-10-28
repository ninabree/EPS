using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Reports
{
    public class RepESAMSViewModel
    {
        public int GOExpHist_Id { get; set; }
        public string GOExpHist_Branchno { get; set; }
        public string GOExpHist_ValueDate { get; set; }
        public string GOExpHist_Remarks { get; set; }

        public string GOExpHist_Entry11Type { get; set; }
        public string GOExpHist_Entry11ActType { get; set; }
        public string GOExpHist_Entry11ActNo { get; set; }
        public string GOExpHist_Entry11Amt { get; set; }

        public string GOExpHist_Entry12Type { get; set; }
        public string GOExpHist_Entry12Amt { get; set; }
        public string GOExpHist_Entry12ActType { get; set; }
        public string GOExpHist_Entry12ActNo { get; set; }

        public string GOExpHist_Entry21Type { get; set; }
        public string GOExpHist_Entry21Amt { get; set; }
        public string GOExpHist_Entry21ActType { get; set; }
        public string GOExpHist_Entry21ActNo { get; set; }

        public string GOExpHist_Entry22Type { get; set; }
        public string GOExpHist_Entry22Amt { get; set; }
        public string GOExpHist_Entry22ActType { get; set; }
        public string GOExpHist_Entry22ActNo { get; set; }

        public string GOExpHist_Entry31Type { get; set; }
        public string GOExpHist_Entry31Amt { get; set; }
        public string GOExpHist_Entry31ActType { get; set; }
        public string GOExpHist_Entry31ActNo { get; set; }

        public string GOExpHist_Entry32Type { get; set; }
        public string GOExpHist_Entry32Amt { get; set; }
        public string GOExpHist_Entry32ActType { get; set; }
        public string GOExpHist_Entry32ActNo { get; set; }

        public string GOExpHist_Entry41Type { get; set; }
        public string GOExpHist_Entry41Amt { get; set; }
        public string GOExpHist_Entry41ActType { get; set; }
        public string GOExpHist_Entry41ActNo { get; set; }

        public string GOExpHist_Entry42Type { get; set; }
        public string GOExpHist_Entry42Amt { get; set; }
        public string GOExpHist_Entry42ActType { get; set; }
        public string GOExpHist_Entry42ActNo { get; set; }

        public int Expense_Creator_ID { get; set; }
        public int Expense_Approver { get; set; }
    }
}
