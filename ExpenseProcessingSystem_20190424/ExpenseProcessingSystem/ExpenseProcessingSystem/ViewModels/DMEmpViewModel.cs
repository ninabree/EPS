using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMEmpViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Emp_ID { get; set; }
        public int Emp_MasterID { get; set; }
        [Display(Name = "Employee Name")]
        public string Emp_Name { get; set; }
        [Display(Name = "Employee Number")]
        public string Emp_Acc_No { get; set; }
        [Display(Name = "Employee Type")]
        public string Emp_Type { get; set; }
        public int Emp_Creator_ID { get; set; }
        public int Emp_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string Emp_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string Emp_Approver_Name { get; set; }
        [Display(Name = "Employee Created Date")]
        public DateTime Emp_Created_Date { get; set; }
        [Display(Name = "Employee Last Updated")]
        public DateTime Emp_Last_Updated { get; set; }
        [Display(Name = "Employee Status")]
        public string Emp_Status { get; set; }
        public bool Emp_isDeleted { get; set; }
        public bool Emp_isActive { get; set; }
    }
}
