using ExpenseProcessingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ConstantData
{
    public class DenominationValues
    {
        public static IEnumerable<ExpenseEntryCashBreakdownModel> GetDenominationList()
        {
            return new ExpenseEntryCashBreakdownModel[] {
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 1000M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 500M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 200M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 100M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 50M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 20M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 10M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 5M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 1M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 0.25M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 0.1M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 0.05M
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 0.01M
                }
            };
        }
    }
}
