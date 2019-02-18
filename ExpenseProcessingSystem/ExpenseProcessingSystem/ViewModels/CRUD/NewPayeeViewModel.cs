using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class NewPayeeListViewModel
    {
        public List<NewPayeeViewModel> NewPayeeVM { get; set; }
    }
    public class NewPayeeViewModel
    {
        public string Payee_Name { get; set; }
        public string Payee_TIN { get; set; }
        public string Payee_Address { get; set; }
        public string Payee_Type { get; set; }
        public int Payee_No { get; set; }
    }
}
