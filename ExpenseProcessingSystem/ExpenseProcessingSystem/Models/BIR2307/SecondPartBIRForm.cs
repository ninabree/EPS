using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIR_Form_Filler.Models
{
    public class SecondPartBIRForm
    {
        private string atcTotal;
        private string m1qTotal;
        private string m2qTotal;
        private string m3qTotal;
        private string aiptTotal;
        private string twTotal;
        private List<PaymentInfo> entryList;

        
        public string AtcTotal { get => atcTotal; set => atcTotal = value; }
        public string M1qTotal { get => m1qTotal; set => m1qTotal = value; }
        public string M2qTotal { get => m2qTotal; set => m2qTotal = value; }
        public string M3qTotal { get => m3qTotal; set => m3qTotal = value; }
        public string AiptTotal { get => aiptTotal; set => aiptTotal = value; }
        public string TwTotal { get => twTotal; set => twTotal = value; }
        public List<PaymentInfo> EntryList { get => entryList; set => entryList = value; }
    }
}
