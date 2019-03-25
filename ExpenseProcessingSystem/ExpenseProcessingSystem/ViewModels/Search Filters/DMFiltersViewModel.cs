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
        public CheckFiltersViewModel CKF { get; set; }
        public AccFiltersViewModel AF { get; set; }
        public EWTFiltersViewModel EF { get; set; }
        public FBTFiltersViewModel FF { get; set; }
        public VATFiltersViewModel VF { get; set; }
        public CurrFiltersViewModel CF { get; set; }
    }
}
