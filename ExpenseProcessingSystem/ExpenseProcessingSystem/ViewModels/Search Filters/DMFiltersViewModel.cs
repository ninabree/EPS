using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters
{
    public class DMFiltersViewModel
    {
        public VendorFiltersViewModel PF { get; set; }
        public DeptFiltersViewModel DF { get; set; }
        public CheckFiltersViewModel CKF { get; set; }
        public AccFiltersViewModel AF { get; set; }
        public TRFiltersViewModel EF { get; set; }
        public FBTFiltersViewModel FF { get; set; }
        public VATFiltersViewModel VF { get; set; }
        public CurrFiltersViewModel CF { get; set; }
        public EmpFiltersViewModel EMF { get; set; }
        public CustFiltersViewModel CUF { get; set; }
        public BCSFiltersViewModel BF { get; set; }
        //public BCSFiltersViewModel BF { get; set; }
    }
}
