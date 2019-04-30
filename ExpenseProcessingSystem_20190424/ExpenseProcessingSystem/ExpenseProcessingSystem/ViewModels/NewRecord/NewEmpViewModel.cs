using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewEmpListViewModel
    {
        public List<NewEmpViewModel> NewEmpVM { get; set; }
    }
    public class NewEmpViewModel
    {
        [Display(Name = "Employee Name")]
        public string Emp_Name { get; set; }
        [Display(Name = "Employee Number")]
        public string Emp_Acc_No { get; set; }
    }
}
