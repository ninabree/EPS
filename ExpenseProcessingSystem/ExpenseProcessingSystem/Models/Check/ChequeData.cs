using ExpenseProcessingSystem.Services;
using System;

namespace ExpenseProcessingSystem.Models.Check
{
    public struct ChequeData
    {
        
        private DateTime date;
        private string payee;
        private decimal amount;
        private string signatory1;
        private string signatory2;
        private string voucher;

        public DateTime Date { get => date; set => date = value; }
        public string Payee { get => payee; set => payee = value; }
        public decimal Amount { get => amount; set => amount = value; }
        public string Signatory1 { get => signatory1; set => signatory1 = value; }
        public string Signatory2 { get => signatory2; set => signatory2 = value; }
        public string Voucher { get => voucher; set => voucher = value; }
        
        public override string ToString()
        {
            return date.ToString("MM/dd/yyyy") + " " + 
                "**" + payee + "**" + " " +
                "**" + amount.ToString("N2") + "**" + " " +
                 "**" + ConvertToWord.ToWord(amount) + "**" + " " +
                signatory1 + " " + signatory2;
        }


    }
}
