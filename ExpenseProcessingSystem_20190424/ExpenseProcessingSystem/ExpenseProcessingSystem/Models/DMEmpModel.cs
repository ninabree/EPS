using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.Models
{
    public class DMEmpModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Emp_ID { get; set; }
        public int Emp_MasterID { get; set; }
        public string Emp_Name { get; set; }
        public string Emp_Acc_No { get; set; }
        public string Emp_Type { get; set; }
        public int Emp_Creator_ID { get; set; }
        public int Emp_Approver_ID { get; set; }
        public DateTime Emp_Created_Date { get; set; }
        public DateTime Emp_Last_Updated { get; set; }
        public string Emp_Status { get; set; }
        public bool Emp_isDeleted { get; set; }
        public bool Emp_isActive { get; set; }
    }
}