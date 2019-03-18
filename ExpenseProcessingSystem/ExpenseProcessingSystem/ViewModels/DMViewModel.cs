using ExpenseProcessingSystem.ViewModels.Search_Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMViewModel
    {
        public DMFiltersViewModel DMFilters { get; set; }
        public PaginatedList<DMDeptViewModel> Dept { get; set; }
        public PaginatedList<DMPayeeViewModel> Payee { get; set; }
        public PaginatedList<DMCheckViewModel> Check { get; set; }
        public PaginatedList<DMAccountViewModel> Account { get; set; }
        public PaginatedList<DMVATViewModel> VAT { get; set; }
        public PaginatedList<DMFBTViewModel> FBT { get; set; }
        public PaginatedList<DMEWTViewModel> EWT { get; set; }
        public PaginatedList<DMCurrencyViewModel> Curr { get; set; }
    }
}
