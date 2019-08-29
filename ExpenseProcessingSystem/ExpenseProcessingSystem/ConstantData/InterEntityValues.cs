using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExpenseProcessingSystem.ConstantData
{
    //Get Currency Master IDs from the XML file
    //used for setting up conditions that compare values by currency type
    public class InterEntityValues
    {
        public static string Currency_US { get; set; }
        public static string Currency_Yen { get; set; }
        public static string Currency_PHP { get; set; }

        public InterEntityValues()
        {
            XElement xelem = XElement.Load("wwwroot/xml/LiquidationValue.xml");

            Currency_US = xelem.Element("CURRENCY_US").Value;
            Currency_Yen = xelem.Element("CURRENCY_Yen").Value;
            Currency_PHP = xelem.Element("CURRENCY_PHP").Value;
        }
    }
}
