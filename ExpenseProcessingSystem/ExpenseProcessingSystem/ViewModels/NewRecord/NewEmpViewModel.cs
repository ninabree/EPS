using ExpenseProcessingSystem.Services.Validations;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public List<SelectListItem> CatList = new List<SelectListItem>();
        public List<SelectListItem> FbtList = new List<SelectListItem>();
    }
    public class NewEmpViewModel
    {
        [Display(Name = "Employee Name")]
        [NotNullValidations]
        public string Emp_Name { get; set; }
        [Display(Name = "Employee Number")]
        public string Emp_Acc_No { get; set; }
        [Display(Name = "Employee Category")]
        [NotNullValidations]
        public int Emp_Category_ID { get; set; }
        [Display(Name = "Employee FBT")]
        public int Emp_FBT_MasterID { get; set; }
    }
}
