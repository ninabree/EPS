using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class EntryDDVViewModel
    {
        [NotNullValidations]
        public string GBaseRemarks { get; set; }
        public int account { get; set; }
        public bool inter_entity { get; set; }
        public bool fbt { get; set; }
        public int dept { get; set; }
        public bool chkVat { get; set; }
        [IntegerValidation]
        public float vat { get; set; }
        public bool chkEwt { get; set; }
        [IntegerValidation, FalseValidation("chkEwt")]
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


        public EntryDDVViewModel()
        {
            gBaseRemarksDetails = new List<EntryGbaseRemarksViewModel>();
            amtDetails = new List<amortizationSchedule>();
        }

    }
    public class EntryDDVViewModelList
    {
        public int entryID { get; set; }
        public SysValViewModel systemValues { get; set; }
        public DateTime expenseDate { get; set; }
        public int vendor { get; set; }
        public string expenseYear { get; set; }
        public string expenseId { get; set; }
        public string checkNo { get; set; }
        public int statusID { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public int maker { get; set; }
        public List<EntryDDVViewModel> EntryDDV { get; set; }

        public EntryDDVViewModelList()
        {
            systemValues = new SysValViewModel();
            EntryDDV = new List<EntryDDVViewModel>();
        }
    }
}
