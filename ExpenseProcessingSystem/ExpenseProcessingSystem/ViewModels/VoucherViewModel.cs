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
        public double grossAmt { get; set; }
    }

    public class ewtAmtList
    {
        public double ewt { get; set; }
        public double ewtAmt { get; set; }
    }

    public class particulars
    {
        public string documentType { get; set; }
        public string invoiceNo { get; set; }
        public string description { get; set; }
        public double amount { get; set; }
    }

    public class VoucherViewModelList
    {
        //public List<VoucherViewModel> itemDtl { get; set; }
        public headerVM headvm { get; set; }
        public int payeeID { get; set; }
        public string payee { get; set; }
        public double amount { get; set; }
        public double vat { get; set; }
        public double gross { get; set; }
        public double taxBase { get; set; }
        public string amountString { get; set; }
        public string checkNo { get; set; }
        public string voucherNo { get; set; }
        public int makeriD { get; set; }
        public string maker { get; set; }
        public string approver { get; set; }
        public string verifier_1 { get; set; }
        public string verifier_2 { get; set; }
        public string date { get; set; }
        public bool isFbt { get; set; }
        public List<particulars> particulars { get; set; }
        public List<ewtAmtList> vatAmtList { get; set; }

        public VoucherViewModelList()
        {
            particulars = new List<particulars>();
            vatAmtList = new List<ewtAmtList>();
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
