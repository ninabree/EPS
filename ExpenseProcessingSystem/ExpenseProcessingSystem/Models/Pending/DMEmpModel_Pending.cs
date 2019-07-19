using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMEmpModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_Emp_ID { get; set; }
        public int Pending_Emp_MasterID { get; set; }
        public int Pending_Emp_FBT_MasterID { get; set; }
        public string Pending_Emp_Name { get; set; }
        public string Pending_Emp_Acc_No { get; set; }
        public string Pending_Emp_Type { get; set; }
        public int Pending_Emp_Category_ID { get; set; } // local or expat
        public int Pending_Emp_Creator_ID { get; set; }
        public int Pending_Emp_Approver_ID { get; set; }
        public DateTime Pending_Emp_Filed_Date { get; set; }
        public int Pending_Emp_Status_ID { get; set; }
        public bool Pending_Emp_isDeleted { get; set; }
        public bool Pending_Emp_isActive { get; set; }
    }
}
