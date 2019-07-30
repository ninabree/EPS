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
        private double m1Quarter;
        private double m2Quarter;
        private double m3Quarter;
        private double taxWithheld;

        public string Payments { get => payments; set => payments = value; }
        public string Atc { get => atc; set => atc = value; }
        public double M1Quarter { get => m1Quarter; set => m1Quarter = value; }
        public double M2Quarter { get => m2Quarter; set => m2Quarter = value; }
        public double M3Quarter { get => m3Quarter; set => m3Quarter = value; }
        public double TaxWithheld { get => taxWithheld; set => taxWithheld = value; }
    }
}
