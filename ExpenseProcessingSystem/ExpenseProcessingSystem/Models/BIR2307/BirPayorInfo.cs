using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIR_Form_Filler.Models
{
    public class BirPayorInfo
    {
        private string tin;
        private string payorName;
        private string regAddress;
        private string zipCodeA;

        public string Tin { get => tin; set => tin = value; }
        public string PayorName { get => payorName; set => payorName = value; }
        public string RegAddress { get => regAddress; set => regAddress = value; }
        public string ZipCodeA { get => zipCodeA; set => zipCodeA = value; }
    }
}
