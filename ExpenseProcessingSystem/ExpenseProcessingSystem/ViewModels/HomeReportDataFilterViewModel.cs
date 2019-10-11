using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class HomeReportDataFilterViewModel
    {
        public IEnumerable<HomeReportOutputAPSWT_MModel> HomeReportOutputAPSWT_M { get; set; }

        public IEnumerable<HomeReportOutputAST1000Model> HomeReportOutputAST1000 { get; set; }

        public IEnumerable<HomeReportESAMSViewModel> HomeReportOutputESAMS { get; set; }

        public IEnumerable<HomeReportActualBudgetModel> HomeReportOutputActualBudget { get; set; }

        public IEnumerable<HomeReportTransactionListViewModel> HomeReportOutputTransactionList { get; set; }

        public IEnumerable<HomeReportAccountSummaryViewModel> HomeReportOutputAccountSummary { get; set; }

        public IEnumerable<HomeReportOutputAPSWT_MModel> HomeReportOutputBIRWTCSV { get; set; }

        public string ReturnPeriod_CSV { get; set; }

        public IEnumerable<HomeReportAccountSummaryViewModel> HomeReportOutputWTS { get; set; }

        public IEnumerable<Temp_RepCSBViewModel> HomeReportOutputCSB { get; set; }

        public HomeReportViewModel HomeReportFilter { get; set; }

        public ReportCommonViewModel ReportCommonVM { get; set; }

        public ReportLOIViewModel ReportLOI { get; set; }

        public List<RepAmortViewModel> ReportAmort { get; set; }

        public string ReportAccountNo { get; set; }
        public string ReportAccountCode { get; set; }

        public string ReportCurrency { get; set; }

        [DisplayFormat(DataFormatString = "{0:M/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:M/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }

    }
}
