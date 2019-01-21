﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem
{
    public class NumberToText
    {

        private static readonly string[] CurrencyMap = new[]
        {
            "PESOS ONLY", "US DOLLAR ONLY", "YEN ONLY"
        };
        private static readonly string[] UnitsMap = new[]
        {
            "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN",
            "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN",
            "EIGHTEEN", "NINETEEN"
        };

        private static readonly string[] TensMap = new[]
        {
            "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY",
            "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY"
        };

        private static readonly string[] ScaleMap = new[]
        { "", " THOUSAND", " MILLION", " BILLION", " TRILLION", " QUADRILLION" };

        static IEnumerable<int> SplitIntoThousands(long number)
        {
            while (number != 0)
            {
                yield return (int)(number % 1000);
                number /= 1000;
            }
        }
        static IEnumerable<int> SplitIntoThousands(double number)
        {
            while (number != 0)
            {
                yield return (int)(number % 1000);
                number /= 1000;
            }
        }
        static StringBuilder SplitDecimals(double number)
        {
            StringBuilder decword = new StringBuilder();
            String[] num = number.ToString("G17").Split('.');
            if (num.Length > 1)
            {
                if (num[1].Length == 1)
                {
                    num[1] += 0;
                }
                decword.Append(" AND " + num[1] + "/100");
            }
            return decword;
        }

        public string SmallNumberToWords(int number)
        {
            string result = null;

            if (number > 0)
            {
                if (number >= 100)
                {
                    var hundrets = SmallNumberToWords(number / 100);
                    var tens = SmallNumberToWords(number % 100);

                    result = hundrets + " HUNDRED";

                    if (tens != null)
                        result += ' ' + tens;
                }
                else if (number < 20)
                    result = UnitsMap[number];
                else
                {
                    result = TensMap[number / 10];
                    if ((number % 10) > 0)
                        result += " " + UnitsMap[number % 10];
                }
            }

            return result;
        }

        public string DoubleNumberToWords(double number)
        {
            if (number == 0)
                return "ZERO";

            if (number < 0)
                return "MINUS " + DoubleNumberToWords(-number);

            var decimals = SplitDecimals(number);
            var thousands = SplitIntoThousands(number).ToArray();

            var result = new StringBuilder();

            for (int i = thousands.Length - 1; i >= 0; i--)
            {
                var word = SmallNumberToWords(thousands[i]);

                if (word != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(' ');
                    }
                    result.Append(word);
                    result.Append(ScaleMap[i]);
                }
            }
            result.Append(decimals);
            return result.ToString();
        }

        public string DoubleNumberToWords(double number, string currency)
        {
            if (number == 0)
                return "ZERO";

            if (number < 0)
                return "MINUS " + DoubleNumberToWords(-number, currency);

            var decimals = SplitDecimals(number);
            var thousands = SplitIntoThousands(number).ToArray();

            var result = new StringBuilder();

            for (int i = thousands.Length - 1; i >= 0; i--)
            {
                var word = SmallNumberToWords(thousands[i]);

                if (word != null)
                {
                    if (result.Length > 0)
                    {
                        result.Append(' ');
                    }
                    result.Append(word);
                    result.Append(ScaleMap[i]);
                }
            }
            var j = 0;
            switch (currency)
            {
                case "PHP":
                    j = 0;
                    break;
                case "USD":
                    j = 1;
                    break;
                case "YEN":
                    j = 2;
                    break;
                default:
                    j = 0;
                    break;
            }

            result.Append(decimals + " " + CurrencyMap[j]);
            return result.ToString();
        }
    }
}
