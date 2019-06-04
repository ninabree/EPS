using ExpenseProcessingSystem.ConstantData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class BMRegHistViewModel
    {
        public IEnumerable<BMViewModel> BMVM { get; set; }
        public IEnumerable<YearList> YearList { get; set; }
        public int Year { get; set; }
    }
}
