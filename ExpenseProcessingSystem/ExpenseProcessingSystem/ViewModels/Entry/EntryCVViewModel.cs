using ExpenseProcessingSystem.Services.Validations;
using ExpenseProcessingSystem.ViewModels.Entry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class EntryCVViewModel
    {
        [NotNullValidations]
        public string GBaseRemarks { get; set; }
        public int account { get; set; }
        public bool fbt { get; set; }
        public int dept { get; set; }
        public bool chkVat { get; set; }
        [IntegerValidation]
        public float vat { get; set; }
        public bool chkEwt { get; set; }
        [IntegerValidation,FalseValidation("chkEwt")]
        public int ewt { get; set; }
        public int ccy { get; set; }
        public float debitGross { get; set; }
        public float credEwt { get; set; }
        public float credCash { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int duration { get; set; }
        public List<amortizationSchedule> amtDetails { get; set; }
        public List<EntryGbaseRemarksViewModel> gBaseRemarksDetails { get; set; }


        public EntryCVViewModel()
        {
            gBaseRemarksDetails = new List<EntryGbaseRemarksViewModel>();
            amtDetails = new List<amortizationSchedule>();
        }
    }

    public class amortizationSchedule
    {
        public DateTime amtDate { get; set; }
        public float amtAmount { get; set; }
    }

    public class EntryCVViewModelList
    {
        public SysValViewModel systemValues { get; set; }
        public DateTime expenseDate { get; set; }
        public int vendor { get; set; }
        public string expenseYear { get; set; }
        public string expenseId { get; set; }
        public int checkNo { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public string verifier { get; set; }
        public List<EntryCVViewModel> EntryCV { get; set; }

        public EntryCVViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryCV = new List<EntryCVViewModel>();
        }
    }
}
