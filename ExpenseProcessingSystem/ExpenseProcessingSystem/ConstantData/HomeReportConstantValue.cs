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

        //Alphalist of Payees Subject to Withholding Tax (Monthly)
        public const string APSWT_M = "2";

        //EXCEL, PDF, Preview format ID
        public const string EXCELID = "1";
        public const string PDFID = "2";
        public const string PreviewID = "3";

        //PDF Format name
        public const string ReportLayoutFormatName = "ReportLayoutFormat/_ReportLayout_";

        //PDF Footer foramt
        public static readonly string PdfFooter1 = "--footer-left \" PAGE => [page] of [toPage] \" --footer-right \" Printed Date => " + DateTime.Today.ToShortDateString() + "\" --footer-font-size \"9\" --footer-spacing 3 --footer-font-name \"calibri light\"";
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

        public static IEnumerable<FileFormatList> GetFileFormatList()
        {
            return new FileFormatList[] {
                new FileFormatList
                {
                    FileFormatID = 1,
                    FileFormatName = "EXCEL"
                },
                new FileFormatList
                {
                    FileFormatID = 2,
                    FileFormatName = "PDF"
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

        public static IEnumerable<HomeReportSubTypesModel> GetReportSubTypeData()
        {
            return new HomeReportSubTypesModel[]
            {
                new HomeReportSubTypesModel
                {
                    Id = 1,
                    SubTypeName = "All",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = 2,
                    SubTypeName = "Check Disbursement",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = 3,
                    SubTypeName = "Petty Cash Disbursement",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = 4,
                    SubTypeName = "Direct Deposit Disbursement",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = 5,
                    SubTypeName = "Cash Advance",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = 6,
                    SubTypeName = "Local Payroll",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = 7,
                    SubTypeName = "Japanese Payroll",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = 8,
                    SubTypeName = "Tax Remittances",
                    ParentTypeId = 7
                },
                new HomeReportSubTypesModel
                {
                    Id = 9,
                    SubTypeName = "(*All Accounts)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 10,
                    SubTypeName = "H99767111115 - P.E. JAP STAFF (SALARY)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 11,
                    SubTypeName = "H99767111199 - P.E. JAP STAFF (WELFARE)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 12,
                    SubTypeName = "H99767111204 - P.E. JAP STAFF (FRINGE BENEFIT)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 13,
                    SubTypeName = "H99767111123 - P.E. LOC STAFF (SALARY)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 14,
                    SubTypeName = "H99767111220 - P.E. LOC STAFF (WELFARE)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 15,
                    SubTypeName = "H99767111238 - P.E. LOC STAFF (FRINGE BENEFIT)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 16,
                    SubTypeName = "H99767111212 - P.E. LOC STAFF (BONUS)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 17,
                    SubTypeName = "H99767111131 - CONT TO RETIREMENT FUND",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 18,
                    SubTypeName = "H90767121113 - Office Rent",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 19,
                    SubTypeName = "H90767121121 - Company House Rent",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 20,
                    SubTypeName = "H90767121139 - Office Repair",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 21,
                    SubTypeName = "H90767121147 - House Repair",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 22,
                    SubTypeName = "H90767121155 - Common Service Fee",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 23,
                    SubTypeName = "H90767121171 - Office Cleaning",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 24,
                    SubTypeName = "H90767121189 - Office Maintenance",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 25,
                    SubTypeName = "H90767121197 - Electricity",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 26,
                    SubTypeName = "H90767121202 - Gas",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 27,
                    SubTypeName = "H90767121210 - Oil",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 28,
                    SubTypeName = "H90767121228 - Water",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 29,
                    SubTypeName = "H90767121236 - Rental-Computer",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 30,
                    SubTypeName = "H90767121244 - Rental-Others",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 31,
                    SubTypeName = "H90767121252 - Equipment Maintenance",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 32,
                    SubTypeName = "H90767121260 - Equipment Repair",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 33,
                    SubTypeName = "H90767121278 - Equipment",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 34,
                    SubTypeName = "H90767121286 - Consumables",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 35,
                    SubTypeName = "H90767121294 - Computer Consumables",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 36,
                    SubTypeName = "H90767121309 - Computer Form",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 37,
                    SubTypeName = "H90767121317 - Paper & Printings",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 38,
                    SubTypeName = "H90767121325 - Postage-Domestic",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 39,
                    SubTypeName = "H90767121333 - Postage-International",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 40,
                    SubTypeName = "H90767121341 - Swift/Telex",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 41,
                    SubTypeName = "H90767121359 - Tel/Fax",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 42,
                    SubTypeName = "H90767121367 - Cable",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 43,
                    SubTypeName = "H90767121375 - Books & Newspapers",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 44,
                    SubTypeName = "H90767121383 - Market Information",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 45,
                    SubTypeName = "H90767121391 - Credit Research",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 46,
                    SubTypeName = "H90767121406 - Market Research",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 47,
                    SubTypeName = "H90767121414 - Shipping",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 48,
                    SubTypeName = "H90767121422 - Deposit Insurance",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 49,
                    SubTypeName = "H90767121430 - Car Insurance",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 50,
                    SubTypeName = "H90767121448 - Cash Insurance",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 51,
                    SubTypeName = "H90767121456 - Computer Insurance",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 52,
                    SubTypeName = "H90767121464 - Other Insurance",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 53,
                    SubTypeName = "H90767121472 - Transportation-Taxi",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 54,
                    SubTypeName = "H90767121480 - Transportation-Others",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 55,
                    SubTypeName = "H90767121498 - Motor Vehicle-Fuel",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 56,
                    SubTypeName = "H90767121503 - Motor Vehicle-Lease",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 57,
                    SubTypeName = "H90767121511 - Motor Vehicle-Others",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 58,
                    SubTypeName = "H90767121529 - Domestic Travel",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 59,
                    SubTypeName = "H90767121537 - Appointment Travel",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 60,
                    SubTypeName = "H90767121545 - Educational Travel",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 61,
                    SubTypeName = "H90767121553 - International Travel",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 62,
                    SubTypeName = "H90767121561 - Conference Travel",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 63,
                    SubTypeName = "H90767121579 - Advertising-Newspaper",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 64,
                    SubTypeName = "H90767121587 - Advertising-Others",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 65,
                    SubTypeName = "H90767121595 - Membership-BAP",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 66,
                    SubTypeName = "H90767121600 - Membership-Others",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 67,
                    SubTypeName = "H90767121618 - Donations",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 68,
                    SubTypeName = "H90767121626 - Gift",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 69,
                    SubTypeName = "H90767121634 - Entertainment",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 70,
                    SubTypeName = "H90767121642 - Meeting",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 71,
                    SubTypeName = "H90767121650 - Welfare-Medicine",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 72,
                    SubTypeName = "H90767121668 - Welfare-GM House",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 73,
                    SubTypeName = "H90767121676 - Welfare-Home Leave",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 74,
                    SubTypeName = "H90767121684 - Welfare-Others",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 75,
                    SubTypeName = "H90767121692 - Training & Education",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 76,
                    SubTypeName = "H90767121707 - Office Functions",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 77,
                    SubTypeName = "H90767121715 - Outsource-Driver/Messenger",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 78,
                    SubTypeName = "H90767121723 - Outsource-Security",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 79,
                    SubTypeName = "H90767121731 - Outsource-System",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 80,
                    SubTypeName = "H90767121749 - Outsource-Clerical",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 81,
                    SubTypeName = "H90767121757 - Outsource-Others",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 82,
                    SubTypeName = "H90767121765 - Sundry-Pro/Consultant",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 83,
                    SubTypeName = "H90767121773 - Sundry-Recruitment",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 84,
                    SubTypeName = "H90767121781 - Sundry-Others",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 85,
                    SubTypeName = "H60767801042 - BDO MNL",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 86,
                    SubTypeName = "H60767801000 - BSP MNL",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 87,
                    SubTypeName = "H60767801026 - CITI MNL (RBU)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 88,
                    SubTypeName = "F79767111111 - MHCB HO",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 89,
                    SubTypeName = "B79767111111 - MHCB NY",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 90,
                    SubTypeName = "H10767500661 - INGLES E M",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 91,
                    SubTypeName = "H10767102138 - NIPPON LIFE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 92,
                    SubTypeName = "H10767101718 - SAGAWA",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 93,
                    SubTypeName = "H10767100164 - WESTERN PHIL",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 94,
                    SubTypeName = "H10767103016 - ASAHI MANILA",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 95,
                    SubTypeName = "H10767102236 - KDDI PHIL",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 96,
                    SubTypeName = "H30767000072 - SUNDRY",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 97,
                    SubTypeName = "H79767151129 - PETTY CASH",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 98,
                    SubTypeName = "F60789801042 - CITI MNL (FCD)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 99,
                    SubTypeName = "H79767151137 - SUSPENSE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 100,
                    SubTypeName = "H79767151381 - HDMF LOANS PAYABLE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 101,
                    SubTypeName = "H79767151357 - HDMF CONTRIBUTION",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 102,
                    SubTypeName = "H79767151349 - PHILHEALTH CONTRIBUTION",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 103,
                    SubTypeName = "H79767151349 - SSS CONTRIBUTION",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 104,
                    SubTypeName = "H79767151365 - SSS SALARY LOAN REPAYMENT",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 105,
                    SubTypeName = "H99767111123 - PE-SALARY/LOCAL STAFF",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 106,
                    SubTypeName = "H99767111220 - PE-WELFARE/LOCAL STAFF",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 107,
                    SubTypeName = "B79767111111 - COMPUTER SUSPENSE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 108,
                    SubTypeName = "H99767111220 - PE WELFARE LOCAL STAFF",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 109,
                    SubTypeName = "B79767111111 - PREPAID EXPENSE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 110,
                    SubTypeName = "B79767111111 - COMPUTER SUSPENSE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 111,
                    SubTypeName = "B79789111111 - INTERENTITY LIABILITY",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 112,
                    SubTypeName = "H30767000072 - SUNDRY DEPOSIT OPERATIONAL",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 113,
                    SubTypeName = "H30767110005 - SUNDRY DEPOSIT STANDARD",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 114,
                    SubTypeName = "H79767151242 - WHT PAYABLE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 115,
                    SubTypeName = "H79767160005 - LEASEHOLD IMPROVEMENT",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 116,
                    SubTypeName = "H79767160013 - LEASEHOLD IMPROVEMENT",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 117,
                    SubTypeName = "H79767160021 - LEASEHOLD IMPROVEMENT",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 118,
                    SubTypeName = "H79767170000 - FURNITURE & FIXTURE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 119,
                    SubTypeName = "H79767170018 - FURNITURE & FIXTURE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 120,
                    SubTypeName = "H79767170026 - FURNITURE & FIXTURE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 121,
                    SubTypeName = "B79767111111 - CONSTRUCTION",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 122,
                    SubTypeName = "H79767180005 - SECURITY DEPOSIT",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 123,
                    SubTypeName = "H79767180013 - SECURITY DEPOSIT",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 124,
                    SubTypeName = "H79767180039 - SECURITY DEPOSIT HOUSING",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 125,
                    SubTypeName = "H79767180047 - SECURITY DEPOSIT OTHERS",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 126,
                    SubTypeName = "H79767151226 - TAXES WITHHELD",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 127,
                    SubTypeName = "H79767151234 - TAXES WITHHELD",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 128,
                    SubTypeName = "H79767151307 - TAXES WITHHELD",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 129,
                    SubTypeName = "B99767111115 - MISCELLANEOUS LOSS",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 130,
                    SubTypeName = "B99767111115 - LOSS FURNITURE AND FIXTURE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 131,
                    SubTypeName = "B99767111115 - LOSS ON SALE - MOVABLE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 132,
                    SubTypeName = "B99767111115 - LOSS ON DISPOSAL - FIXTURE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 133,
                    SubTypeName = "B99767111115 - LOSS ON DISPOSAL - MOVABLE",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 134,
                    SubTypeName = "F79789151446 - EXPENSE HO (US$)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 135,
                    SubTypeName = "F79789810008 - REIMBURSE (US$)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 136,
                    SubTypeName = "H79767151146 - ADV EXP HO",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 137,
                    SubTypeName = "F79789810016 - REIMBURSE (YEN)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 138,
                    SubTypeName = "H79767152002 - DST-AD (PHP)",
                    ParentTypeId = 8
                },
                new HomeReportSubTypesModel
                {
                    Id = 139,
                    SubTypeName = " H79767151446 - ADVANC EXPENSES HO",
                    ParentTypeId = 8
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
                },
                new HomeReportSubTypesModel
                {
                    Id = 154,
                    SubTypeName = "H79767151446 - ADV EXP HO-PHP",
                    ParentTypeId = 11
                },
                new HomeReportSubTypesModel
                {
                    Id = 155,
                    SubTypeName = "F79789151446 - EXPENSE HO (US$)",
                    ParentTypeId = 11
                },
                new HomeReportSubTypesModel
                {
                    Id = 156,
                    SubTypeName = "F79789810008 - REIMBURSE (US$)",
                    ParentTypeId = 11
                },
                new HomeReportSubTypesModel
                {
                    Id = 157,
                    SubTypeName = "H30767110005 - SUNDRY DEPOSIT STANDARD",
                    ParentTypeId = 11
                },
                new HomeReportSubTypesModel
                {
                    Id = 158,
                    SubTypeName = "H79767151137 - SUSPENSE (GE)",
                    ParentTypeId = 11
                },
                new HomeReportSubTypesModel
                {
                    Id = 159,
                    SubTypeName = "Office Rent",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 160,
                    SubTypeName = "Office Association Dues",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 161,
                    SubTypeName = "Residence Rent",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 162,
                    SubTypeName = "Residence Association Dues",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 163,
                    SubTypeName = "Motor Vehicle Lease",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 164,
                    SubTypeName = "Machine Maitnenance",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 165,
                    SubTypeName = "Book & Newspaper",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 166,
                    SubTypeName = "Car Insurance",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 167,
                    SubTypeName = "EEI",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 168,
                    SubTypeName = "Fire",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 169,
                    SubTypeName = "MSPR",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 170,
                    SubTypeName = "GPL",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 171,
                    SubTypeName = "BBI",
                    ParentTypeId = 12
                },
                new HomeReportSubTypesModel
                {
                    Id = 172,
                    SubTypeName = "Computer Suspense Account under section 10 only",
                    ParentTypeId = 13
                }
            };
        }
    }

    //========================================================================
    //Public classes
    public class MonthList
    {
        public byte MonthID { get; set; }
        public string MonthName { get; set; }
    }

    public class FileFormatList
    {
        public byte FileFormatID { get; set; }
        public string FileFormatName { get; set; }
    }

    public class YearList
    {
        public int YearID { get; set; }
    }

    public class SemesterList
    {
        public byte SemID { get; set; }
        public string SemName { get; set; }
    }

}
