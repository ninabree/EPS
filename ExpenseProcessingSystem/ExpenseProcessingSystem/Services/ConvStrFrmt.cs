using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services
{
    public class ConvStrFrmt
    {
        public static string ToNumComma(string num)
        {
            if (!String.IsNullOrEmpty(num))
            {
                return String.Format("{0:n}", double.Parse(num));
            }

            return "";
        }
    }
}
