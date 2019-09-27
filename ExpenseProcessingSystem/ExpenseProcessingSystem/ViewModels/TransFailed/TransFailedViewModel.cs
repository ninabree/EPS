using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.TransFailed
{
    public class TransFailedViewModel
    {
        public List<string> TF_MSGs { get; set; }
        public List<TransFailedTableDataViewModel> TF_TBL_DATA { get; set; }

        public TransFailedViewModel()
        {
            TF_MSGs = new List<string>();
            TF_TBL_DATA = new List<TransFailedTableDataViewModel>();
        }

    }
}
