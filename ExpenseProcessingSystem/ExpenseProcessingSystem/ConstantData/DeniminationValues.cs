using ExpenseProcessingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ConstantData
{
    public class DeniminationValues
    {
        public static IEnumerable<ExpenseEntryCashBreakdownModel> GetDeniminationList()
        {
            return new ExpenseEntryCashBreakdownModel[] {
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 1000
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 500
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 200
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 100
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 50
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 20
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 10
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 5
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 1
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 0.25
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 0.1
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 0.05
                },
                new ExpenseEntryCashBreakdownModel
                {
                    CashBreak_Denimination = 0.01
                }
            };
        }
    }
}
