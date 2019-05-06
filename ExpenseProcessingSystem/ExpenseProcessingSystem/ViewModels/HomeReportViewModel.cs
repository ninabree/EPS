using ExpenseProcessingSystem.ConstantData;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class HomeReportViewModel
    {
        [Display(Name = "Report Type")]
        //[ReportValidations]
        public IEnumerable<HomeReportTypesModel> ReportTypesList { get; set; }

        [Display(Name = "Sub-Type")]
        public IEnumerable<HomeReportSubTypesModel> ReportSubTypesList { get; set; }

        [Display(Name = "File Format")]
        public IEnumerable<FileFormatList> FileFormatList { get; set; }

        [Display(Name = "Month")]
        public IEnumerable<MonthList> MonthList { get; set; }

        [Display(Name = "Year")]
        public IEnumerable<YearList> YearList { get; set; }

        [Display(Name = "Year")]
        public IEnumerable<YearList> YearSemList { get; set; }

        public IEnumerable<SemesterList> SemesterList { get; set; }

        public IEnumerable<PeriodOption> PeriodOptionList { get; set; }

        public DateTime PeriodFrom { get; set; }

        public DateTime PeriodTo { get; set; }

        public string CheckNo { get; set; }

        public string VoucherNo { get; set; }

        public string ReportType { get; set; }

        public string ReportSubType { get; set; }

        public string FileFormat { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }

        public string YearSem { get; set; }

        public string Semester { get; set; }

        public string PeriodOption { get; set; }
    }
}
