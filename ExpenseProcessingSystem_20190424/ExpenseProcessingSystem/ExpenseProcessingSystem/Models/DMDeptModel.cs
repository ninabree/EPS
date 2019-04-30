using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMDeptModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Dept_ID { get; set; }
        public int Dept_MasterID { get; set; }
        public string Dept_Name { get; set; }
        public string Dept_Code { get; set; }
        public string Dept_Budget_Unit { get; set; }
        public int Dept_Creator_ID { get; set; }
        public int Dept_Approver_ID { get; set; }
        public DateTime Dept_Created_Date { get; set; }
        public DateTime Dept_Last_Updated { get; set; }
        public string Dept_Status { get; set; }
        public bool Dept_isDeleted { get; set; }
        public bool Dept_isActive { get; set; }
    }
}
