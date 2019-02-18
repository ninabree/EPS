using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class NewDeptListViewModel
    {
        public List<NewDeptViewModel> NewDeptVM { get; set; }
    }
    public class NewDeptViewModel
    {
        public string Dept_Name { get; set; }
        public string Dept_Code { get; set; }
    }
}
