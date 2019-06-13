using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class VoucherViewModel
    {
        public int accountid { get; set; }
        public string account { get; set; }
        public double amount { get; set; }
        public bool fbt { get; set; }
        public int ewtid { get; set; }
        public double ewt { get; set; }
        public int vatid { get; set; }
        public double vat { get; set; } 
    }

    public class VoucherViewModelList
    {
        //public List<VoucherViewModel> itemDtl { get; set; }
        public headerVM headvm { get; set; }
        public int payeeID { get; set; }
        public string payee { get; set; }
        public double amount { get; set; }
        public string amountString { get; set; }
        public string checkNo { get; set; }
        public string voucherNo { get; set; }
        public int makeriD { get; set; }
        public string maker { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public string date { get; set; }

        public VoucherViewModelList()
        {
            headvm = new headerVM();
        }
    }

    public class headerVM
    {
        public string Header_Logo { get; set; }
        public string Header_Name { get; set; }
        public string Header_TIN { get; set; }
        public string Header_Address { get; set; }
    }
}
