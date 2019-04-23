using ExpenseProcessingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class HomeReportViewModel
    {
        public IEnumerable<HomeReportTypesModel> ReportTypesList { get; set; }

        public IEnumerable<HomeReportSubTypesModel> ReportSubTypesList { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int YearSem { get; set; }

        public byte Semester { get; set; }

        public DateTime PeriodFrom { get; set; }

        public DateTime PeriodTo { get; set; }

        public string CheckNo { get; set; }

        public string VoucherNo { get; set; }

        public int MyProperty { get; set; }
    }
}
