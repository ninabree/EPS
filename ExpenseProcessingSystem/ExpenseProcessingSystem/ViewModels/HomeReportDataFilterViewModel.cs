using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class HomeReportDataFilterViewModel
    {
        public IEnumerable<HomeReportOutputAPSWT_MModel> HomeReportOutputAPSWT_M { get; set; }

        public IEnumerable<HomeReportOutputAST1000Model> HomeReportOutputAST1000 { get; set; }

        public IEnumerable<HomeReportActualBudgetModel> HomeReportOutputActualBudget { get; set; }

        public IEnumerable<HomeReportTransactionListViewModel> HomeReportOutputTransactionList { get; set; }

        public IEnumerable<Temp_RepWTSViewModel> HomeReportOutputWTS { get; set; }

        public IEnumerable<Temp_RepCSBViewModel> HomeReportOutputCSB { get; set; }

        public HomeReportViewModel HomeReportFilter { get; set; }

        public ReportCommonViewModel ReportCommonVM { get; set; }

    }
}
