using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMDeptModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_Dept_ID { get; set; }
        public int Pending_Dept_MasterID { get; set; }
        public string Pending_Dept_Name { get; set; }
        public string Pending_Dept_Code { get; set; }
        public int Pending_Dept_Creator_ID { get; set; }
        public int Pending_Dept_Approver_ID { get; set; }
        public DateTime Pending_Dept_Filed_Date { get; set; }
        public string Pending_Dept_Status { get; set; }
        public bool Pending_Dept_isDeleted { get; set; }
        public bool Pending_Dept_isActive { get; set; }
    }
}
