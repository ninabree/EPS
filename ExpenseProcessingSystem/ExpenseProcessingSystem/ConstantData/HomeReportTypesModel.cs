using ExpenseProcessingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ConstantData
{
    public class ReportTypeData
    {
        public static IEnumerable<HomeReportTypesModel> GetReportTypeData()
        {
            return new HomeReportTypesModel[]
            {
                new HomeReportTypesModel
                {
                    Id = 2,
                    TypeName = "Alphalist of Payees Subject to Withholding Tax (Monthly)",
                    SubTypeAvail = false
                },
                new HomeReportTypesModel
                {
                    Id = 3,
                    TypeName = "Alphalist of Suppliers by top 10000 corporation (Semestral)",
                    SubTypeAvail = false
                },
                new HomeReportTypesModel
                {
                    Id = 4,
                    TypeName = "Alphalist of Suppliers by top 10000 corporation (Annual)",
                    SubTypeAvail = false
                },
                new HomeReportTypesModel
                {
                    Id = 5,
                    TypeName = "Actual Budget Report",
                    SubTypeAvail = false
                },
                new HomeReportTypesModel
                {
                    Id = 6,
                    TypeName = "General Expense Distribution Report",
                    SubTypeAvail = false
                },
                new HomeReportTypesModel
                {
                    Id = 7,
                    TypeName = "Transaction List",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = 8,
                    TypeName = "Account Summary",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = 9,
                    TypeName = "BIR Forms",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = 10,
                    TypeName = "Withholding Taxes Summary",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = 11,
                    TypeName = "Outstanding Advances",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = 12,
                    TypeName = "Prepaid Amortization Schedule",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = 13,
                    TypeName = "GA Computer Suspense Balance Report",
                    SubTypeAvail = true
                }
            };
        }
    }
}
