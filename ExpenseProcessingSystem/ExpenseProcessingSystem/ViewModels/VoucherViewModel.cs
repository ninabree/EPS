using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class accountList
    {
        public int accountid { get; set; }
        public string account { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal amount { get; set; }
    }

    public class particulars
    {
        public string documentType { get; set; }
        public string invoiceNo { get; set; }
        public string description { get; set; }
        public decimal amount { get; set; }
    }

    public class taxInfo
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal taxInfo_vat { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal taxInfo_gross { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal taxInfo_taxBase { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal ewtAmt { get; set; }
        public float ewt { get; set; }
        public int ewtID { get; set; }
    }

    public class VoucherViewModelList
    {
        //public List<VoucherViewModel> itemDtl { get; set; }
        public headerVM headvm { get; set; }
        public int payeeID { get; set; }
        public string payee { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal amountCredit { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal amountGross { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal taxWithheld { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal fbtAmount { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public decimal fbtGross { get; set; }
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
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public List<accountList> accountsDebit { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public List<accountList> accountCredit { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public List<particulars> particulars { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n}")]
        public List<taxInfo> taxInfos { get; set; }
        public bool isCheck { get; set; }

        public VoucherViewModelList()
        {
            particulars = new List<particulars>();
            accountsDebit = new List<accountList>();
            accountCredit = new List<accountList>();
            taxInfos = new List<taxInfo>();
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
