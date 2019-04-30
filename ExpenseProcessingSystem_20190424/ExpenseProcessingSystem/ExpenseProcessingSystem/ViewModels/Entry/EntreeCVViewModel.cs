using ExpenseProcessingSystem.ViewModels.Entry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class EntreeCVViewModelList
    {
        public SysValViewModel systemValues { get; set; }
        public DateTime expenseDate { get; set; }
        public int vendor { get; set; }
        public string expenseYear { get; set; }
        public string expenseId { get; set; }
        public int checkNo { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public List<string> verifier { get; set; }
        public List<EntreeCVViewModel> EntreeCV { get; set; }

        public EntreeCVViewModelList()
        {
            systemValues = new SysValViewModel();
            verifier = new List<string>();
            EntreeCV = new List<EntreeCVViewModel>();

        }
    }

    public class EntreeCVViewModel
    {
        public string GBaseRemarks { get; set; }
        public int account { get; set; }
        public bool fbt { get; set; }
        public string dept { get; set; }
        public bool chkVat { get; set; }
        public float vat { get; set; }
        public bool chkEwt { get; set; }
        public int ewt { get; set; }
        public int ccy { get; set; }
        public float debitGross { get; set; }
        public float credEwt { get; set; }
        public float credCash { get; set; }
    }
}
