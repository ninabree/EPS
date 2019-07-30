using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIR_Form_Filler.Models
{
    public class BirPayeeInfo
    {
        private string tin;
        private string payeeName;
        private string regAddress;
        private string zipCodeA;
        private string frgnAddress;
        private string zipCodeB;

        public string Tin { get => tin; set => tin = value; }
        public string PayeeName { get => payeeName; set => payeeName = value; }
        public string RegAddress { get => regAddress; set => regAddress = value; }
        public string ZipCodeA { get => zipCodeA; set => zipCodeA = value; }
        public string FrgnAddress { get => frgnAddress; set => frgnAddress = value; }
        public string ZipCodeB { get => zipCodeB; set => zipCodeB = value; }
    }
}
