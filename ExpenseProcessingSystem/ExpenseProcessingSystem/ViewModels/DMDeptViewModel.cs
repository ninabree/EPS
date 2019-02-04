using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMDeptViewModel
    {
        public int Dept_ID { get; set; }
        public string Dept_Name { get; set; }
        public string Dept_Code { get; set; }
        public int Dept_Creator_ID { get; set; }
        public int Dept_Approver_ID { get; set; }
        public DateTime Dept_Last_Updated { get; set; }
        public string Dept_Status { get; set; }
    }
}
