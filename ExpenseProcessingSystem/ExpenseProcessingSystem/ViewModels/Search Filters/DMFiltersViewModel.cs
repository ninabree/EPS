using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class DMFiltersViewModel
    {
        public PayeeFiltersViewModel PF { get; set; }
        public DeptFiltersViewModel DF { get; set; }
    }
}
