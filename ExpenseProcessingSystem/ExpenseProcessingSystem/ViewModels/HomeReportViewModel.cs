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
        [ReportValidations]
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

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PeriodFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PeriodTo { get; set; }

        public string CheckNo { get; set; }

        public string VoucherNo { get; set; }

        public int ReportType { get; set; }

        public int ReportSubType { get; set; }

        public int FileFormat { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public string MonthName { get; set; }

        public string SemesterName { get; set; }

        public int YearSem { get; set; }

        public int Semester { get; set; }

        public int PeriodOption { get; set; }

        public int CurrentMonth { get { return DateTime.Today.Month; } }

        
    }
}
