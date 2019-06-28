using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExpenseProcessingSystem.ConstantData
{
    public class InterEntityValues
    {
        public static string Currency_US { get; set; }
        public static string Account1_US { get; set; }
        public static string Account2_US { get; set; }
        public static string Account3_US { get; set; }
        public static string Account4_US { get; set; }
        public static string Account5_US { get; set; }
        public static string Account6_US { get; set; }
        public static string Account7_US { get; set; }
        public static string Account8_US { get; set; }
        public static string Account9_US { get; set; }
        public static string Account10_US { get; set; }

        public static string Currency_Yen { get; set; }
        public static string Account1_Yen { get; set; }
        public static string Account2_Yen { get; set; }
        public static string Account3_Yen { get; set; }
        public static string Account4_Yen { get; set; }
        public static string Account5_Yen { get; set; }
        public static string Account6_Yen { get; set; }
        public static string Account7_Yen { get; set; }
        public static string Account8_Yen { get; set; }
        public static string Account9_Yen { get; set; }
        public static string Account10_Yen { get; set; }

        public InterEntityValues()
        {
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");

            Currency_US = xelem.Element("CURRENCY_US").Value;
            Account1_US = xelem.Element("ACCOUNT1_US").Value;
            Account2_US = xelem.Element("ACCOUNT2_US").Value;
            Account3_US = xelem.Element("ACCOUNT3_US").Value;
            Account4_US = xelem.Element("ACCOUNT4_US").Value;
            Account5_US = xelem.Element("ACCOUNT5_US").Value;
            Account6_US = xelem.Element("ACCOUNT6_US").Value;
            Account7_US = xelem.Element("ACCOUNT7_US").Value;
            Account8_US = xelem.Element("ACCOUNT8_US").Value;
            Account9_US = xelem.Element("ACCOUNT9_US").Value;
            Account10_US = xelem.Element("ACCOUNT10_US").Value;

            Currency_Yen = xelem.Element("CURRENCY_Yen").Value;
            Account1_Yen = xelem.Element("ACCOUNT1_Yen").Value;
            Account2_Yen = xelem.Element("ACCOUNT2_Yen").Value;
            Account3_Yen = xelem.Element("ACCOUNT3_Yen").Value;
            Account4_Yen = xelem.Element("ACCOUNT4_Yen").Value;
            Account5_Yen = xelem.Element("ACCOUNT5_Yen").Value;
            Account6_Yen = xelem.Element("ACCOUNT6_Yen").Value;
            Account7_Yen = xelem.Element("ACCOUNT7_Yen").Value;
            Account8_Yen = xelem.Element("ACCOUNT8_Yen").Value;
            Account9_Yen = xelem.Element("ACCOUNT9_Yen").Value;
            Account10_Yen = xelem.Element("ACCOUNT10_Yen").Value;
        }
    }
}
