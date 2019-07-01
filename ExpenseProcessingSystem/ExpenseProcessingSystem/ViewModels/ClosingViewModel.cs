using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class ClosingViewModel
    {
        public List<CloseItems> rbuItems { get; set; }
        public List<CloseItems> fcduItems { get; set; }

        public double pettyBegBalance { get; set; }
        public double cashIn { get; set; }
        public double cashOut { get; set; }
        public double endBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }

        public StartBalConf beginningBalConf { get; set; }
        public StartBalConf pettyCashBrkDwn { get; set; }

        public ClosingViewModel()
        {
            rbuItems = new List<CloseItems>();
            fcduItems = new List<CloseItems>();
            beginningBalConf = new StartBalConf();
            pettyCashBrkDwn = new StartBalConf();
        }
    }
    
    public class StartBalConf
    {
        public BalanceHeader header { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }
        public BalanceBreakdown bills { get; set; }
        public double billsTotal { get; set; }
        public BalanceBreakdown coins { get; set; }
        public double coinsTotal { get; set; }
        public double closingBal { get; set; }

        public StartBalConf()
        {
            bills = new BalanceBreakdown(denominationList.getbillList());
            coins = new BalanceBreakdown(denominationList.getCoinList());
        }
    }

    public class CloseItems
    {
        public string gBaseTrans { get; set; }
        public string particulars { get; set; }
        public string ccy { get; set; }
        public double amount { get; set; }
        public string status { get; set; }
    }

    public class BalanceHeader
    {
        public Balance opeBal { get; set; }
        public Balance recieve { get; set; }
        public Balance Disbursed { get; set; }
        public Balance closeBal { get; set; }
    }

    public class BalanceBreakdown
    {
        public Dictionary<double, int> pieceDictio { get; set; }
        public Dictionary<double, double> amountDictio { get; set; }

        public BalanceBreakdown(List<double> Denominations)
        {
            pieceDictio = new Dictionary<double, int>();
            amountDictio = new Dictionary<double, double>();

            foreach (double deno in Denominations)
            {
                pieceDictio.Add(deno, 0);
                amountDictio.Add(deno, 0.00);
            }

        }
    }

    public class Balance
    {
        public string currency { get; set; }
        public double amount{ get; set; }
    }

    public static class denominationList
    {
        public static List<double> getbillList()
        {
            return new List<double> { 1000.00, 500.00, 200.00, 100.00, 50.00, 20.00 };
        }
        public static List<double> getCoinList()
        {
            return new List<double> { 10.00, 5.00, 1.00, 0.25, 0.10, 0.05, 0.01 };
        }
    }
}
