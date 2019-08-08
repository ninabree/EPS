using ExpenseProcessingSystem.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ConstantData
{
    public class HomeReportConstantValue
    {
        //========================================================================
        //Public constant values

        public const int SystemYearStarted = 2000;

        public static readonly string DateToday = DateTime.Today.ToLongDateString();
        //Alphalist of Payees Subject to Withholding Tax (Monthly)
        public const int APSWT_M = 2;

        //Alphalist of Suppliers by top 10000 corporations
        public const int AST1000 = 3;

        //ESAMS (Electronic Sundry Account Management System)
        public const int ESAMS = 4;

        //Actual Budget Report
        public const int ActualBudgetReport = 5;

        //BIR Withholding Tax Report(CSV)
        public const int BIRWTCSV = 6;

        //Transaction List Report
        public const int TransListReport = 7;

        //Account Summary Report
        public const int AccSummaryReport = 8;

        //Reserved
        public const int Reserved = 9;

        //Alphalist of Payees Subject to Withholding Tax Summary
        public const int WTS = 10;

        //Outstanding Advances
        public const int OutstandingAdvances = 11;

        //Prepaid Amortization Report
        public const int PrepaidAmortReport = 12;

        //List of Instructions
        public const int LOI = 13;

        //EXCEL, PDF, Preview format ID
        public const int EXCELID = 1;
        public const int PDFID = 2;
        public const int PreviewID = 3;
        public const int CSVID = 4;

        //Semester value
        public const int SEM1 = 1;
        public const int SEM2 = 2;

        //PDF Format name
        public const string ReportLayoutFormatName = "_ReportLayout_";

        public const string ReportPdfPrevLayoutPath = "ReportLayoutFormat/";
        //PDF Footer foramt
        public static readonly string PdfFooter1 = "--footer-left \" PAGE => [page] of [toPage] \" --footer-right \" Printed Date => " + DateTime.Today.ToShortDateString() + "\" --footer-font-size \"9\" --footer-spacing 3 --footer-font-name \"calibri light\"";
        public static readonly string PdfFooter2 = "--footer-left \" " + DateTime.Now.ToString("dddd, MMMM dd,yyyy h:mm:sstt") + " \" --footer-center \" Page [page] of [toPage] \" --footer-font-size \"9\" --footer-spacing 3 --footer-font-name \"calibri light\"";

        //Non-Cash category distinguish value
        public static readonly int REP_NC_LS_PAYROLL = GlobalSystemValues.NC_LS_PAYROLL + 50;
        public static readonly int REP_NC_TAX_REMITTANCE = GlobalSystemValues.NC_TAX_REMITTANCE + 50;
        public static readonly int REP_NC_MONTHLY_ROSS_BILL = GlobalSystemValues.NC_MONTHLY_ROSS_BILL + 50;
        public static readonly int REP_NC_PSSC = GlobalSystemValues.NC_PSSC + 50;
        public static readonly int REP_NC_PCHC = GlobalSystemValues.NC_PCHC + 50;
        public static readonly int REP_NC_DEPRECIATION = GlobalSystemValues.NC_DEPRECIATION + 50;
        public static readonly int REP_NC_PETTY_CASH_REPLENISHMENT = GlobalSystemValues.NC_PETTY_CASH_REPLENISHMENT + 50;
        public static readonly int REP_NC_JS_PAYROLL = GlobalSystemValues.NC_JS_PAYROLL + 50;
        public static readonly int REP_NC_RETURN_OF_JS_PAYROLL = GlobalSystemValues.NC_RETURN_OF_JS_PAYROLL + 50;
        public static readonly int REP_NC_MISCELLANEOUS_ENTRIES = GlobalSystemValues.NC_MISCELLANEOUS_ENTRIES + 50;

        public static readonly int REP_LIQUIDATION = 10;

        //========================================================================
        //Public IEnumerable class constant values

        public static IEnumerable<SemesterList> GetSemesterList()
        {
            return new SemesterList[]
            {
                new SemesterList
                {
                    SemID = 1,
                    SemName = "1st Semester"
                },
                new SemesterList
                {
                    SemID = 2,
                    SemName = "2nd Semester"
                }
            };
        }

        public static IEnumerable<PeriodOption> GetPeriodOptionList()
        {
            return new PeriodOption[]
            {
                new PeriodOption
                {
                    PeriodOptionID = 1
                },
                new PeriodOption
                {
                    PeriodOptionID = 2
                }
            };
        }

        public static IEnumerable<FileFormatList> GetFileFormatList()
        {
            return new FileFormatList[] {
                new FileFormatList
                {
                    FileFormatID = EXCELID,
                    FileFormatName = "EXCEL"
                },
                new FileFormatList
                {
                    FileFormatID = PDFID,
                    FileFormatName = "PDF"
                },
                new FileFormatList
                {
                    FileFormatID = CSVID,
                    FileFormatName = "CSV"
                }
            };
        }

        public static IEnumerable<YearList> GetYearList()
        {
            int j = DateTime.Today.Year;
            List<YearList> yearList = new List<YearList>();
            
            while(j >= SystemYearStarted)
            {
                yearList.Add(new YearList { YearID = j});
                j -= 1;
            }
            return yearList;
        }

        public static IEnumerable<YearTerm_BIRCSV> GetTerm_BIRCSV()
        {
            int j = DateTime.Today.Year;
            List<YearTerm_BIRCSV> list = new List<YearTerm_BIRCSV>();

            while (j >= SystemYearStarted)
            {
                list.Add(new YearTerm_BIRCSV { ID = j + "1", YearTerm = j + " Jan. - June " });
                list.Add(new YearTerm_BIRCSV { ID = j + "2", YearTerm = j + " July. - Dec " });
                j -= 1;
            }
            return list;
        }

        public static IEnumerable<MonthList> GetMonthList()
        {
            return new MonthList[] {
                new MonthList
                {
                    MonthID = 1,
                    MonthName = "January"
                },
                new MonthList
                {
                    MonthID = 2,
                    MonthName = "February"
                },
                new MonthList
                {
                    MonthID = 3,
                    MonthName = "March"
                },
                new MonthList
                {
                    MonthID = 4,
                    MonthName = "April"
                },
                new MonthList
                {
                    MonthID = 5,
                    MonthName = "May"
                },
                new MonthList
                {
                    MonthID = 6,
                    MonthName = "June"
                },
                new MonthList
                {
                    MonthID = 7,
                    MonthName = "July"
                },
                new MonthList
                {
                    MonthID = 8,
                    MonthName = "August"
                },
                new MonthList
                {
                    MonthID = 9,
                    MonthName = "September"
                },
                new MonthList
                {
                    MonthID = 10,
                    MonthName = "October"
                },
                new MonthList
                {
                    MonthID = 11,
                    MonthName = "November"
                },
                new MonthList
                {
                    MonthID = 12,
                    MonthName = "December"
                }
            };
        }

        public static int GetCurrentSemester()
        {
            int currentSem = 2;

            if (DateTime.Today.Month >= 4 && DateTime.Today.Month <= 9)
                currentSem = 1;

            return currentSem;
        }
        public static IEnumerable<HomeReportTypesModel> GetReportTypeData()
        {
            return new HomeReportTypesModel[]
            {
                new HomeReportTypesModel
                {
                    Id = APSWT_M,
                    TypeName = "Alphalist of Payees Subject to Withholding Tax (Monthly)",
                    SubTypeAvail = false
                },
                new HomeReportTypesModel
                {
                    Id = AST1000,
                    TypeName = "Alphalist of Suppliers by top 10000 corporation",
                    SubTypeAvail = false
                },
                new HomeReportTypesModel
                {
                    Id = ESAMS,
                    TypeName = "ESAMS",
                    SubTypeAvail = false
                },
                new HomeReportTypesModel
                {
                    Id = ActualBudgetReport,
                    TypeName = "Actual Budget",
                    SubTypeAvail = false
                },
                new HomeReportTypesModel
                {
                    Id = BIRWTCSV,
                    TypeName = "BIR Withholding Tax (CSV)",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = TransListReport,
                    TypeName = "Transaction List",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = AccSummaryReport,
                    TypeName = "Account Summary",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = Reserved,
                    TypeName = "Reserved",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = WTS,
                    TypeName = "Withholding Taxes Summary",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = OutstandingAdvances,
                    TypeName = "Outstanding Advances",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = PrepaidAmortReport,
                    TypeName = "Prepaid Amortization Schedule",
                    SubTypeAvail = true
                },
                new HomeReportTypesModel
                {
                    Id = LOI,
                    TypeName = "Letter of Instructions (LOI)",
                    SubTypeAvail = false
                }
            };
        }

        public static IEnumerable<HomeReportSubTypesModel> GetReportSubTypeData()
        {
            return new HomeReportSubTypesModel[]
            {
                new HomeReportSubTypesModel
                {
                    Id = 0,
                    SubTypeName = "All",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = GlobalSystemValues.TYPE_CV,
                    SubTypeName = "Check Disbursement",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = GlobalSystemValues.TYPE_PC,
                    SubTypeName = "Petty Cash Disbursement",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = GlobalSystemValues.TYPE_DDV,
                    SubTypeName = "Direct Deposit Disbursement",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = GlobalSystemValues.TYPE_SS,
                    SubTypeName = "Cash Advance",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_LIQUIDATION,
                    SubTypeName = "Liquidation",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_LS_PAYROLL,
                    SubTypeName = "Local Payroll",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_TAX_REMITTANCE,
                    SubTypeName = "Tax Remittance",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_MONTHLY_ROSS_BILL,
                    SubTypeName = "Monthly Ross Bill",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_PSSC,
                    SubTypeName = "PSSC",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_PCHC,
                    SubTypeName = "PCHC",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_DEPRECIATION,
                    SubTypeName = "Depreciation",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_PETTY_CASH_REPLENISHMENT,
                    SubTypeName = "Petty Cash Replenishment",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_JS_PAYROLL,
                    SubTypeName = "JS Payroll",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_RETURN_OF_JS_PAYROLL,
                    SubTypeName = "Return of JS Payroll",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = REP_NC_MISCELLANEOUS_ENTRIES,
                    SubTypeName = "Miscellaneous Entries",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = 140,
                    SubTypeName = "1601-E Monthly Remittance Return of Creditable Taxes Withheld (Expanded)",
                    ParentTypeId = 9
                },
                new HomeReportSubTypesModel
                {
                    Id = 141,
                    SubTypeName = "1601-F Monthly Remittance Return of Final Income Taxes Withheld",
                    ParentTypeId = 9
                },
                new HomeReportSubTypesModel
                {
                    Id = 142,
                    SubTypeName = "1601-C Monthly Remittance Return of Income Taxes Withheld on Compensation (JS)",
                    ParentTypeId = 9
                },
                new HomeReportSubTypesModel
                {
                    Id = 143,
                    SubTypeName = "1601-C Monthly Remittance Return of Income Taxes Withheld on Compensation (LS)",
                    ParentTypeId = 9
                },
                new HomeReportSubTypesModel
                {
                    Id = 144,
                    SubTypeName = "1601-C Monthly Remittance Return of Income Taxes Withheld on Compensation (B)",
                    ParentTypeId = 9
                },
                new HomeReportSubTypesModel
                {
                    Id = 145,
                    SubTypeName = "1% Goods Individual",
                    ParentTypeId = 10
                },
                new HomeReportSubTypesModel
                {
                    Id = 146,
                    SubTypeName = "1% Goods Corporate",
                    ParentTypeId = 10
                },
                new HomeReportSubTypesModel
                {
                    Id = 147,
                    SubTypeName = "2% Services Individual",
                    ParentTypeId = 10
                },
                new HomeReportSubTypesModel
                {
                    Id = 148,
                    SubTypeName = "2% Services Corporate",
                    ParentTypeId = 10
                },
                new HomeReportSubTypesModel
                {
                    Id = 149,
                    SubTypeName = "5% Rent",
                    ParentTypeId = 10
                },
                new HomeReportSubTypesModel
                {
                    Id = 150,
                    SubTypeName = "10% Professional Fees",
                    ParentTypeId = 10
                },
                new HomeReportSubTypesModel
                {
                    Id = 151,
                    SubTypeName = "15% Professional Fees",
                    ParentTypeId = 10
                },
                new HomeReportSubTypesModel
                {
                    Id = 152,
                    SubTypeName = "25% Final Tax",
                    ParentTypeId = 10
                },
                new HomeReportSubTypesModel
                {
                    Id = 153,
                    SubTypeName = "30% Final Tax",
                    ParentTypeId = 10
                }
                //new HomeReportSubTypesModel
                //{
                //    Id = 159,
                //    SubTypeName = "Office Rent",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 160,
                //    SubTypeName = "Office Association Dues",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 161,
                //    SubTypeName = "Residence Rent",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 162,
                //    SubTypeName = "Residence Association Dues",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 163,
                //    SubTypeName = "Motor Vehicle Lease",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 164,
                //    SubTypeName = "Machine Maitnenance",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 165,
                //    SubTypeName = "Book & Newspaper",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 166,
                //    SubTypeName = "Car Insurance",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 167,
                //    SubTypeName = "EEI",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 168,
                //    SubTypeName = "Fire",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 169,
                //    SubTypeName = "MSPR",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 170,
                //    SubTypeName = "GPL",
                //    ParentTypeId = 12
                //},
                //new HomeReportSubTypesModel
                //{
                //    Id = 171,
                //    SubTypeName = "BBI",
                //    ParentTypeId = 12
                //}
            };
        }
    }

    //========================================================================
    //Public classes
    public class MonthList
    {
        public int MonthID { get; set; }
        public string MonthName { get; set; }
    }

    public class FileFormatList
    {
        public int FileFormatID { get; set; }
        public string FileFormatName { get; set; }
    }

    public class YearList
    {
        public int YearID { get; set; }
    }

    public class YearTerm_BIRCSV
    {
        public string ID { get; set; }
        public string YearTerm { get; set; }
    }

    public class SemesterList
    {
        public int SemID { get; set; }
        public string SemName { get; set; }
    }

    public class PeriodOption
    {
        public int PeriodOptionID { get; set; }
    }
}
