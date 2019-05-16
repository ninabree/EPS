using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMAccountListViewModel
    {
        public List<DMAccountViewModel> AccountVM { get; set; }
        public List<DMFBTViewModel> FbtList = new List<DMFBTViewModel>();
        public List<DMAccountGroupViewModel> AccGrp = new List<DMAccountGroupViewModel>();
    }
}
