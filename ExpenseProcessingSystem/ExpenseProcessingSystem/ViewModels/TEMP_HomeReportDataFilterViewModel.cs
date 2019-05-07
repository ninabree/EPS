using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class TEMP_HomeReportDataFilterViewModel
    {
        public IEnumerable<TEMP_HomeReportOutputAPSWT_MModel> HomeReportOutputAPSWT_M { get; set; }

        public IEnumerable<TEMP_HomeReportOutputAST1000_SModel> HomeReportOutputAST1000_S { get; set; }

        public IEnumerable<Temp_RepWTSViewModel> HomeReportOutputWTS { get; set; }

        public IEnumerable<Temp_RepCSBViewModel> HomeReportOutputCSB { get; set; }

        public HomeReportViewModel HomeReportFilter { get; set; }
    }
}
