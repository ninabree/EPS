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
                    CashBreak_Denomination = 1000
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 500
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 200
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 100
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 50
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 20
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 10
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 5
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 1
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 0.25
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 0.1
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 0.05
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denomination = 0.01
                }
            };
        }
    }
}
