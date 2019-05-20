using System;
using System.Collections.Generic;

namespace ExpenseProcessingSystem.ConstantData
{
    public class TEMP_HomeReportActualBudgetReportDummyData
    {
        public class AcountCategory
        {
            public int AccountID { get; set; }
            public string Account_Category { get; set; }
        }

        public class Expense
        {
            public int AccountID { get; set; }
            public string Remarks { get; set; }
            public string Department { get; set; }
            public double ExpenseAmount { get; set; }
            public DateTime DateReflected { get; set; }
        }

        public class BudgetMonitoring
        {
            public int AccountID { get; set; }
            public double BudgetAmount { get; set; }
            public string Remarks { get; set; }
            public DateTime TermOfBudget { get; set; }
            public DateTime Last_Updated_Date { get; set; }
        }

        public static IEnumerable<AcountCategory> GetAcountCategory()
        {
            return new AcountCategory[]
            {
                new AcountCategory
                {
                    AccountID = 1,
                    Account_Category = "Welfare"
                },
                new AcountCategory
                {
                    AccountID = 2,
                    Account_Category = "Official Residence Rent"
                },
                new AcountCategory
                {
                    AccountID = 3,
                    Account_Category = "Office Maintence"
                },
                new AcountCategory
                {
                    AccountID = 4,
                    Account_Category = "Water Fuel Electricity"
                },
                new AcountCategory
                {
                    AccountID = 5,
                    Account_Category = "Fitting"
                },
                new AcountCategory
                {
                    AccountID = 6,
                    Account_Category = "Paper & Printing"
                },
                new AcountCategory
                {
                    AccountID = 7,
                    Account_Category = "Books Newspaper"
                },
                new AcountCategory
                {
                    AccountID = 8,
                    Account_Category = "Telegraph, Telephone"
                },
                new AcountCategory
                {
                    AccountID = 9,
                    Account_Category = "Other Office Equipment"
                },
                new AcountCategory
                {
                    AccountID = 10,
                    Account_Category = "Machine Maintenance"
                },
                new AcountCategory
                {
                    AccountID = 11,
                    Account_Category = "Relocation"
                },
                new AcountCategory
                {
                    AccountID = 12,
                    Account_Category = "Travel to Other Countries"
                },
                new AcountCategory
                {
                    AccountID = 13,
                    Account_Category = "Other Travel"
                },
                new AcountCategory
                {
                    AccountID = 14,
                    Account_Category = "Traffic"
                },
                new AcountCategory
                {
                    AccountID = 15,
                    Account_Category = "Automobile"
                },
                new AcountCategory
                {
                    AccountID = 16,
                    Account_Category = "Advetisement"
                },
                new AcountCategory
                {
                    AccountID = 17,
                    Account_Category = "Entertainment"
                },
                new AcountCategory
                {
                    AccountID = 18,
                    Account_Category = "Membership"
                },
                new AcountCategory
                {
                    AccountID = 19,
                    Account_Category = "Insurance"
                },
                new AcountCategory
                {
                    AccountID = 20,
                    Account_Category = "Consignment"
                },
                new AcountCategory
                {
                    AccountID = 21,
                    Account_Category = "Sundries"
                }
            };
        }

        public static IEnumerable<Expense> GetExpenseData()
        {
            return new Expense[]
            {
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark1",
                        Department = "Department2",
                        ExpenseAmount = 1003,
                        DateReflected = DateTime.Parse("2019/05/06 15:15:00")
                    },
                     new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark9",
                        Department = "Department10",
                        ExpenseAmount = 1011,
                        DateReflected = DateTime.Parse("2019/05/06 15:55:00")
                    },
                    new Expense
                    {
                        AccountID = 2,
                        Remarks = "Remark17",
                        Department = "Department18",
                        ExpenseAmount = 1019,
                        DateReflected = DateTime.Parse("2019/05/06 16:35:00")
                    },
                    new Expense
                    {
                        AccountID = 3,
                        Remarks = "Remark25",
                        Department = "Department26",
                        ExpenseAmount = 1027,
                        DateReflected = DateTime.Parse("2019/05/06 17:15:00")
                    },
                    new Expense
                    {
                        AccountID = 2,
                        Remarks = "Remark33",
                        Department = "Department34",
                        ExpenseAmount = 1035,
                        DateReflected = DateTime.Parse("2019/05/06 17:55:00")
                    },
                    new Expense
                    {
                        AccountID = 2,
                        Remarks = "Remark41",
                        Department = "Department42",
                        ExpenseAmount = 1043,
                        DateReflected = DateTime.Parse("2019/05/06 18:35:00")
                    },
                    new Expense
                    {
                        AccountID = 21,
                        Remarks = "Remark49",
                        Department = "Department50",
                        ExpenseAmount = 1051,
                        DateReflected = DateTime.Parse("2019/05/06 19:15:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark57",
                        Department = "Department58",
                        ExpenseAmount = 1059,
                        DateReflected = DateTime.Parse("2019/05/06 19:55:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark65",
                        Department = "Department66",
                        ExpenseAmount = 1067,
                        DateReflected = DateTime.Parse("2019/05/06 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 2,
                        Remarks = "Remark73",
                        Department = "Department74",
                        ExpenseAmount = 1075,
                        DateReflected = DateTime.Parse("2019/05/06 21:15:00")
                    },
                    new Expense
                    {
                        AccountID = 2,
                        Remarks = "Remark81",
                        Department = "Department82",
                        ExpenseAmount = 1083,
                        DateReflected = DateTime.Parse("2019/05/06 21:55:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark89",
                        Department = "Department90",
                        ExpenseAmount = 1091,
                        DateReflected = DateTime.Parse("2019/05/06 22:35:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark97",
                        Department = "Department98",
                        ExpenseAmount = 1099,
                        DateReflected = DateTime.Parse("2019/05/06 23:15:00")
                    },
                    new Expense
                    {
                        AccountID = 2,
                        Remarks = "Remark105",
                        Department = "Department106",
                        ExpenseAmount = 1107,
                        DateReflected = DateTime.Parse("2019/05/06 23:55:00")
                    },
                    new Expense
                    {
                        AccountID = 4,
                        Remarks = "Remark113",
                        Department = "Department114",
                        ExpenseAmount = 1115,
                        DateReflected = DateTime.Parse("2019/05/07 0:35:00")
                    },
                    new Expense
                    {
                        AccountID = 3,
                        Remarks = "Remark121",
                        Department = "Department122",
                        ExpenseAmount = 1123,
                        DateReflected = DateTime.Parse("2019/05/07 1:15:00")
                    },
                    new Expense
                    {
                        AccountID = 3,
                        Remarks = "Remark129",
                        Department = "Department130",
                        ExpenseAmount = 1131,
                        DateReflected = DateTime.Parse("2019/05/07 1:55:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark137",
                        Department = "Department138",
                        ExpenseAmount = 1139,
                        DateReflected = DateTime.Parse("2019/05/07 2:35:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark145",
                        Department = "Department146",
                        ExpenseAmount = 1147,
                        DateReflected = DateTime.Parse("2019/05/07 3:15:00")
                    },
                    new Expense
                    {
                        AccountID = 3,
                        Remarks = "Remark153",
                        Department = "Department154",
                        ExpenseAmount = 1155,
                        DateReflected = DateTime.Parse("2019/05/07 3:55:00")
                    },
                    new Expense
                    {
                        AccountID = 3,
                        Remarks = "Remark161",
                        Department = "Department162",
                        ExpenseAmount = 1163,
                        DateReflected = DateTime.Parse("2019/05/07 4:35:00")
                    },
                    new Expense
                    {
                        AccountID = 3,
                        Remarks = "Remark169",
                        Department = "Department170",
                        ExpenseAmount = 1171,
                        DateReflected = DateTime.Parse("2019/05/07 5:15:00")
                    },
                    new Expense
                    {
                        AccountID = 3,
                        Remarks = "Remark177",
                        Department = "Department178",
                        ExpenseAmount = 1179,
                        DateReflected = DateTime.Parse("2019/05/07 5:55:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark185",
                        Department = "Department186",
                        ExpenseAmount = 1187,
                        DateReflected = DateTime.Parse("2019/05/07 6:35:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark193",
                        Department = "Department194",
                        ExpenseAmount = 1195,
                        DateReflected = DateTime.Parse("2019/05/07 7:15:00")
                    },
                    new Expense
                    {
                        AccountID = 4,
                        Remarks = "Remark201",
                        Department = "Department202",
                        ExpenseAmount = 1203,
                        DateReflected = DateTime.Parse("2019/05/07 7:55:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark209",
                        Department = "Department210",
                        ExpenseAmount = 1211,
                        DateReflected = DateTime.Parse("2019/05/07 8:35:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark217",
                        Department = "Department218",
                        ExpenseAmount = 1219,
                        DateReflected = DateTime.Parse("2019/05/07 9:15:00")
                    },
                    new Expense
                    {
                        AccountID = 4,
                        Remarks = "Remark225",
                        Department = "Department226",
                        ExpenseAmount = 1227,
                        DateReflected = DateTime.Parse("2019/05/07 9:55:00")
                    },
                    new Expense
                    {
                        AccountID = 4,
                        Remarks = "Remark233",
                        Department = "Department234",
                        ExpenseAmount = 1235,
                        DateReflected = DateTime.Parse("2019/05/07 10:35:00")
                    },
                    new Expense
                    {
                        AccountID = 6,
                        Remarks = "Remark241",
                        Department = "Department242",
                        ExpenseAmount = 1243,
                        DateReflected = DateTime.Parse("2019/05/07 11:15:00")
                    },
                    new Expense
                    {
                        AccountID = 6,
                        Remarks = "Remark249",
                        Department = "Department250",
                        ExpenseAmount = 1251,
                        DateReflected = DateTime.Parse("2019/05/07 11:55:00")
                    },
                    new Expense
                    {
                        AccountID = 9,
                        Remarks = "Remark257",
                        Department = "Department258",
                        ExpenseAmount = 1259,
                        DateReflected = DateTime.Parse("2019/05/07 12:35:00")
                    },
                    new Expense
                    {
                        AccountID = 8,
                        Remarks = "Remark265",
                        Department = "Department266",
                        ExpenseAmount = 1267,
                        DateReflected = DateTime.Parse("2019/05/07 13:15:00")
                    },
                    new Expense
                    {
                        AccountID = 7,
                        Remarks = "Remark273",
                        Department = "Department274",
                        ExpenseAmount = 1275,
                        DateReflected = DateTime.Parse("2019/05/07 13:55:00")
                    },
                    new Expense
                    {
                        AccountID = 7,
                        Remarks = "Remark281",
                        Department = "Department282",
                        ExpenseAmount = 1283,
                        DateReflected = DateTime.Parse("2019/05/07 14:35:00")
                    },
                    new Expense
                    {
                        AccountID = 21,
                        Remarks = "Remark289",
                        Department = "Department290",
                        ExpenseAmount = 1291,
                        DateReflected = DateTime.Parse("2019/05/07 15:15:00")
                    },
                    new Expense
                    {
                        AccountID = 6,
                        Remarks = "Remark297",
                        Department = "Department298",
                        ExpenseAmount = 1299,
                        DateReflected = DateTime.Parse("2019/05/07 15:55:00")
                    },
                    new Expense
                    {
                        AccountID = 6,
                        Remarks = "Remark305",
                        Department = "Department306",
                        ExpenseAmount = 1307,
                        DateReflected = DateTime.Parse("2019/05/07 16:35:00")
                    },
                    new Expense
                    {
                        AccountID = 21,
                        Remarks = "Remark313",
                        Department = "Department314",
                        ExpenseAmount = 1315,
                        DateReflected = DateTime.Parse("2019/05/07 17:15:00")
                    },
                    new Expense
                    {
                        AccountID = 6,
                        Remarks = "Remark321",
                        Department = "Department322",
                        ExpenseAmount = 1323,
                        DateReflected = DateTime.Parse("2019/05/07 17:55:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark329",
                        Department = "Department330",
                        ExpenseAmount = 1331,
                        DateReflected = DateTime.Parse("2019/05/07 18:35:00")
                    },
                    new Expense
                    {
                        AccountID = 6,
                        Remarks = "Remark337",
                        Department = "Department338",
                        ExpenseAmount = 1339,
                        DateReflected = DateTime.Parse("2019/05/07 19:15:00")
                    },
                    new Expense
                    {
                        AccountID = 21,
                        Remarks = "Remark345",
                        Department = "Department346",
                        ExpenseAmount = 1347,
                        DateReflected = DateTime.Parse("2019/05/07 19:55:00")
                    },
                    new Expense
                    {
                        AccountID = 7,
                        Remarks = "Remark353",
                        Department = "Department354",
                        ExpenseAmount = 1355,
                        DateReflected = DateTime.Parse("2019/05/07 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark361",
                        Department = "Department362",
                        ExpenseAmount = 1363,
                        DateReflected = DateTime.Parse("2019/05/07 21:15:00")
                    },
                    new Expense
                    {
                        AccountID = 7,
                        Remarks = "Remark369",
                        Department = "Department370",
                        ExpenseAmount = 1371,
                        DateReflected = DateTime.Parse("2019/05/07 21:55:00")
                    },
                    new Expense
                    {
                        AccountID = 7,
                        Remarks = "Remark377",
                        Department = "Department378",
                        ExpenseAmount = 1379,
                        DateReflected = DateTime.Parse("2019/05/07 22:35:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark385",
                        Department = "Department386",
                        ExpenseAmount = 1387,
                        DateReflected = DateTime.Parse("2019/05/07 23:15:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark393",
                        Department = "Department394",
                        ExpenseAmount = 1395,
                        DateReflected = DateTime.Parse("2019/05/07 23:55:00")
                    },
                    new Expense
                    {
                        AccountID = 7,
                        Remarks = "Remark401",
                        Department = "Department402",
                        ExpenseAmount = 1403,
                        DateReflected = DateTime.Parse("2019/05/08 0:35:00")
                    },
                    new Expense
                    {
                        AccountID = 7,
                        Remarks = "Remark409",
                        Department = "Department410",
                        ExpenseAmount = 1411,
                        DateReflected = DateTime.Parse("2019/05/08 1:15:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark417",
                        Department = "Department418",
                        ExpenseAmount = 1419,
                        DateReflected = DateTime.Parse("2019/05/08 1:55:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark425",
                        Department = "Department426",
                        ExpenseAmount = 1427,
                        DateReflected = DateTime.Parse("2019/05/08 2:35:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark433",
                        Department = "Department434",
                        ExpenseAmount = 1435,
                        DateReflected = DateTime.Parse("2019/05/08 3:15:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark441",
                        Department = "Department442",
                        ExpenseAmount = 1443,
                        DateReflected = DateTime.Parse("2019/05/08 3:55:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark449",
                        Department = "Department450",
                        ExpenseAmount = 1451,
                        DateReflected = DateTime.Parse("2019/05/08 4:35:00")
                    },
                    new Expense
                    {
                        AccountID = 19,
                        Remarks = "Remark457",
                        Department = "Department458",
                        ExpenseAmount = 1459,
                        DateReflected = DateTime.Parse("2019/05/08 5:15:00")
                    },
                    new Expense
                    {
                        AccountID = 12,
                        Remarks = "Remark465",
                        Department = "Department466",
                        ExpenseAmount = 1467,
                        DateReflected = DateTime.Parse("2019/05/08 5:55:00")
                    },
                    new Expense
                    {
                        AccountID = 7,
                        Remarks = "Remark473",
                        Department = "Department474",
                        ExpenseAmount = 1475,
                        DateReflected = DateTime.Parse("2019/05/08 6:35:00")
                    },
                    new Expense
                    {
                        AccountID = 16,
                        Remarks = "Remark481",
                        Department = "Department482",
                        ExpenseAmount = 1483,
                        DateReflected = DateTime.Parse("2019/05/08 7:15:00")
                    },
                    new Expense
                    {
                        AccountID = 15,
                        Remarks = "Remark489",
                        Department = "Department490",
                        ExpenseAmount = 1491,
                        DateReflected = DateTime.Parse("2019/05/08 7:55:00")
                    },
                    new Expense
                    {
                        AccountID = 14,
                        Remarks = "Remark497",
                        Department = "Department498",
                        ExpenseAmount = 1499,
                        DateReflected = DateTime.Parse("2019/05/08 8:35:00")
                    },
                    new Expense
                    {
                        AccountID = 7,
                        Remarks = "Remark505",
                        Department = "Department506",
                        ExpenseAmount = 1507,
                        DateReflected = DateTime.Parse("2019/05/08 9:15:00")
                    },
                    new Expense
                    {
                        AccountID = 8,
                        Remarks = "Remark513",
                        Department = "Department514",
                        ExpenseAmount = 1515,
                        DateReflected = DateTime.Parse("2019/05/08 9:55:00")
                    },
                    new Expense
                    {
                        AccountID = 13,
                        Remarks = "Remark521",
                        Department = "Department522",
                        ExpenseAmount = 1523,
                        DateReflected = DateTime.Parse("2019/05/08 10:35:00")
                    },
                    new Expense
                    {
                        AccountID = 13,
                        Remarks = "Remark529",
                        Department = "Department530",
                        ExpenseAmount = 1531,
                        DateReflected = DateTime.Parse("2019/05/08 11:15:00")
                    },
                    new Expense
                    {
                        AccountID = 13,
                        Remarks = "Remark529",
                        Department = "Department530",
                        ExpenseAmount = 1531,
                        DateReflected = DateTime.Parse("2019/06/08 11:15:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark537",
                        Department = "Department538",
                        ExpenseAmount = 1539,
                        DateReflected = DateTime.Parse("2019/05/08 11:55:00")
                    },
                    new Expense
                    {
                        AccountID = 12,
                        Remarks = "Remark545",
                        Department = "Department546",
                        ExpenseAmount = 1547,
                        DateReflected = DateTime.Parse("2019/05/08 12:35:00")
                    },
                    new Expense
                    {
                        AccountID = 8,
                        Remarks = "Remark553",
                        Department = "Department554",
                        ExpenseAmount = 1555,
                        DateReflected = DateTime.Parse("2019/05/08 13:15:00")
                    },
                    new Expense
                    {
                        AccountID = 8,
                        Remarks = "Remark561",
                        Department = "Department562",
                        ExpenseAmount = 1563,
                        DateReflected = DateTime.Parse("2019/05/08 13:55:00")
                    },
                    new Expense
                    {
                        AccountID = 8,
                        Remarks = "Remark569",
                        Department = "Department570",
                        ExpenseAmount = 1571,
                        DateReflected = DateTime.Parse("2019/05/08 14:35:00")
                    },
                    new Expense
                    {
                        AccountID = 11,
                        Remarks = "Remark577",
                        Department = "Department578",
                        ExpenseAmount = 1579,
                        DateReflected = DateTime.Parse("2019/05/08 15:15:00")
                    },
                    new Expense
                    {
                        AccountID = 8,
                        Remarks = "Remark585",
                        Department = "Department586",
                        ExpenseAmount = 1587,
                        DateReflected = DateTime.Parse("2019/05/08 15:55:00")
                    },
                    new Expense
                    {
                        AccountID = 11,
                        Remarks = "Remark593",
                        Department = "Department594",
                        ExpenseAmount = 1595,
                        DateReflected = DateTime.Parse("2019/05/08 16:35:00")
                    },
                    new Expense
                    {
                        AccountID = 10,
                        Remarks = "Remark601",
                        Department = "Department602",
                        ExpenseAmount = 1603,
                        DateReflected = DateTime.Parse("2019/05/08 17:15:00")
                    },
                    new Expense
                    {
                        AccountID = 1,
                        Remarks = "Remark609",
                        Department = "Department610",
                        ExpenseAmount = 1611,
                        DateReflected = DateTime.Parse("2019/05/08 17:55:00")
                    },
                    new Expense
                    {
                        AccountID = 8,
                        Remarks = "Remark617",
                        Department = "Department618",
                        ExpenseAmount = 1619,
                        DateReflected = DateTime.Parse("2019/05/08 18:35:00")
                    },
                    new Expense
                    {
                        AccountID = 8,
                        Remarks = "Remark617",
                        Department = "Department618",
                        ExpenseAmount = 1619,
                        DateReflected = DateTime.Parse("2019/06/08 18:35:00")
                    },
                    new Expense
                    {
                        AccountID = 9,
                        Remarks = "Remark625",
                        Department = "Department626",
                        ExpenseAmount = 1627,
                        DateReflected = DateTime.Parse("2019/05/08 19:15:00")
                    },
                    new Expense
                    {
                        AccountID = 10,
                        Remarks = "Remark633",
                        Department = "Department634",
                        ExpenseAmount = 1635,
                        DateReflected = DateTime.Parse("2019/05/08 19:55:00")
                    },
                    new Expense
                    {
                        AccountID = 9,
                        Remarks = "Remark641",
                        Department = "Department642",
                        ExpenseAmount = 1643,
                        DateReflected = DateTime.Parse("2019/05/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 9,
                        Remarks = "Remark641",
                        Department = "Department642",
                        ExpenseAmount = 1643,
                        DateReflected = DateTime.Parse("2019/06/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2019/07/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2019/07/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2019/07/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2019/08/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2019/09/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2019/10/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2019/10/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2019/11/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2020/01/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2020/02/08 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2020/02/28 20:35:00")
                    },
                    new Expense
                    {
                        AccountID = 20,
                        Remarks = "Remark999",
                        Department = "Department999",
                        ExpenseAmount = 9999,
                        DateReflected = DateTime.Parse("2020/03/08 20:35:00")
                    }
            };
        }

        public static IEnumerable<BudgetMonitoring> GetBudgetMonitoringData()
        {
            return new BudgetMonitoring[]
            {
                new BudgetMonitoring
                {
                    AccountID = 1,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 1,
                    BudgetAmount = 2000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/10/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 2,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 3,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 4,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 5,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 6,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 7,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 8,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 9,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 10,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 11,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 12,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 13,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 14,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 15,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 16,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 16,
                    BudgetAmount = 2000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/10/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 17,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 18,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 19,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 20,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 20,
                    BudgetAmount = 2000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/10/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/10/01 12:00:00")
                },
                new BudgetMonitoring
                {
                    AccountID = 21,
                    BudgetAmount = 1000000,
                    Remarks = "Reset due to Fiscal Year",
                    TermOfBudget = DateTime.Parse("2019/04/01 12:00:00"),
                    Last_Updated_Date = DateTime.Parse("2019/04/01 12:00:00")
                }
            };
        }
    }
}