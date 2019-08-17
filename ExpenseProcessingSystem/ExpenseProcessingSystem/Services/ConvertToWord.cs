using System;
using System.Text;

namespace ExpenseProcessingSystem.Services
{
    public class ConvertToWord
    {
        public static string ToWord(decimal input)
        {
            StringBuilder sb = new StringBuilder("");

            long bil = Convert.ToInt64(Convert.ToInt64(Math.Floor(input)) / 1000000000L);
            long mil = Convert.ToInt64(Convert.ToInt64(Math.Floor(input)) % 1000000000 / 1000000);
            long tho = Convert.ToInt64(Convert.ToInt64(Math.Floor(input)) % 1000000 / 1000);
            long hun = Convert.ToInt64(Convert.ToInt64(Math.Floor(input)) % 1000);
            byte cent = Convert.ToByte((input - Math.Truncate(input)) * 100);

            if (bil > 0)
            {
                sb.Append(GetBillions(bil) + " ");
            }

            if (mil > 0)
            {
                sb.Append(GetMillions(mil) + " ");
            }

            if (tho > 0)
            {
                sb.Append(GetThousands(tho) + " ");
            }

            if (hun > 0)
            {
                sb.Append(GetHundreds(hun) + " ");
            }

            if (cent > 0)
            {
                sb.Append($"& {cent}/100 ");
            }
            else
            {
                sb.Append("& xx/100 ");
            }

            sb.Append("Only");

            return sb.ToString();
        }

        private static string GetOnes(byte toConv)
        {
            switch (toConv)
            {
                case 9: return new string("Nine");
                case 8: return new string("Eight");
                case 7: return new string("Seven");
                case 6: return new string("Six");
                case 5: return new string("Five");
                case 4: return new string("Four");
                case 3: return new string("Three");
                case 2: return new string("Two");
                case 1: return new string("One");
                default: return new string("");
            }
        }

        private static string GetTens(byte toConv)
        {
            string ones = GetOnes(Convert.ToByte(toConv % 10));

            if (!string.IsNullOrEmpty(ones) && toConv > 20)
            {
                ones = string.Join("", new string("-"), ones);
            }


            switch (toConv / 10)
            {
                case 9: return String.Join("", new string("Ninety"), ones);
                case 8: return String.Join("", new string("Eighty"), ones);
                case 7: return String.Join("", new string("Seventy"), ones);
                case 6: return String.Join("", new string("Sixty"), ones);
                case 5: return String.Join("", new string("Fifty"), ones);
                case 4: return String.Join("", new string("Forty"), ones);
                case 3: return String.Join("", new string("Thirty"), ones);
                case 2: return String.Join("", new string("Twenty"), ones);
                case 1:
                    switch (toConv)
                    {
                        case 19: return new string("Nineteen");
                        case 18: return new string("Eighteen");
                        case 17: return new string("Seventeen");
                        case 16: return new string("Sixteen");
                        case 15: return new string("Fifteen");
                        case 14: return new string("Fourteen");
                        case 13: return new string("Thirteen");
                        case 12: return new string("Twelve");
                        case 11: return new string("Eleven");
                        default: return new string("Ten");
                    }
                default: return ones;
            }
        }


        private static string GetHundreds(long toConv)
        {
            if (toConv >= 100)
            {
                return string.Join(" ", GetOnes(Convert.ToByte(toConv / 100)),
                                    new string("Hundred"),
                                    GetTens(Convert.ToByte(toConv % 100)));
            }
            else
            {
                return GetTens(Convert.ToByte(toConv % 100));
            }
        }

        private static string GetThousands(long toConv)
        {
            return string.Join(" ", GetHundreds(toConv), new string("Thousand"));
        }

        private static string GetMillions(long toConv)
        {
            return string.Join(" ", GetHundreds(toConv), new string("Million"));
        }

        private static string GetBillions(long toConv)
        {
            return string.Join(" ", GetHundreds(toConv), new string("Billion"));
        }

    }
}
