using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services
{
    public class Mizuho
    {
        public static string toGbaseString(double _num, bool isYen)
        {
            string newVal = "";

            string[] spilt = _num.ToString().Split('.');
            int index = spilt[0].ToString().Length;

            foreach (char val in spilt[0])
            {
                if ((index % 3) == 0)
                {
                    newVal += "," + val;
                }
                else
                {
                    newVal += val;
                }
            }

            if (!isYen)
            {
                newVal += spilt[1];
            }

            return newVal;
        }

        public static double round(double _num, int scale)
        {
            decimal val = decimal.Round((decimal)_num, scale, MidpointRounding.AwayFromZero);

            return (double)val;
        }

        public static float round(float _num, int scale)
        {
            decimal val = decimal.Round((decimal)_num, scale, MidpointRounding.AwayFromZero);

            return (float)val;
        }
    }
}
