using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BIR_Form_Filler.Models
{
    public class FirstPartBIRForm
    {
        private DateTime from_date;
        private DateTime to_date;
        private string eeTin;
        private string payeeName;
        private string eeRegAddress;
        private string eeZipCodeA;
        private string eeFrgnAddress;
        private string eeZipCodeB;
        private string orTin;
        private string payorName;
        private string orRegAddress;
        private string orZipCodeA;
        private List<PaymentInfo> incomePay;
        private List<PaymentInfo> moneyPay;
        private Signatories payorSig;
        private Signatories payeeSig;
        private string voucherNo;
        
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime From_Date{ get => from_date; set => from_date = value; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime To_Date { get => to_date; set => to_date = value; }

        public string EeTin { get => eeTin; set => eeTin = value; }
        public string PayeeName { get => payeeName; set => payeeName = value; }
        public string EeRegAddress { get => eeRegAddress; set => eeRegAddress = value; }
        public string EeZipCodeA { get => eeZipCodeA; set => eeZipCodeA = value; }
        public string EeFrgnAddress { get => eeFrgnAddress; set => eeFrgnAddress = value; }
        public string EeZipCodeB { get => eeZipCodeB; set => eeZipCodeB = value; }
        public string OrTin { get => orTin; set => orTin = value; }
        public string PayorName { get => payorName; set => payorName = value; }
        public string OrRegAddress { get => orRegAddress; set => orRegAddress = value; }
        public string OrZipCodeA { get => orZipCodeA; set => orZipCodeA = value; }
        public List<PaymentInfo> IncomePay { get => incomePay; set => incomePay = value; }
        public List<PaymentInfo> MoneyPay { get => moneyPay; set => moneyPay = value; }
        public Signatories PayorSig { get => payorSig; set => payorSig = value; }
        public Signatories PayeeSig { get => payeeSig; set => payeeSig = value; }
        public string VoucherNo { get => voucherNo; set => voucherNo = value; }

        public FirstPartBIRForm()
        {
            IncomePay = new List<PaymentInfo>();
            MoneyPay = new List<PaymentInfo>();
        }
    }
}
