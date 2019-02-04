using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMViewModel
    {
        public PaginatedList<DMDeptViewModel> Dept { get; set; }
        public PaginatedList<DMPayeeViewModel> Payee { get; set; }
    }
}
