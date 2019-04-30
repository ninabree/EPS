using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class ExcelViewModel
    {
        public List<Row> RowList { get; set; }
    }
    public class Row
    {
        public List<string> DataList { get; set; }
    }
}
