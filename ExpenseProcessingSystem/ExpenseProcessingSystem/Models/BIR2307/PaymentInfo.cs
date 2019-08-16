using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIR_Form_Filler.Models
{
    public class PaymentInfo
    {
        private string payments;
        private string atc;
        private decimal m1Quarter;
        private decimal m2Quarter;
        private decimal m3Quarter;
        private decimal taxWithheld;

        public string Payments { get => payments; set => payments = value; }
        public string Atc { get => atc; set => atc = value; }
        public decimal M1Quarter { get => m1Quarter; set => m1Quarter = value; }
        public decimal M2Quarter { get => m2Quarter; set => m2Quarter = value; }
        public decimal M3Quarter { get => m3Quarter; set => m3Quarter = value; }
        public decimal TaxWithheld { get => taxWithheld; set => taxWithheld = value; }
    }
}
