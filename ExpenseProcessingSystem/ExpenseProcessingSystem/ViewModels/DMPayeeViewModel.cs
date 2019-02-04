using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMPayeeViewModel
    {
        public int Payee_ID { get; set; }
        public string Payee_Name { get; set; }
        public string Payee_TIN { get; set; }
        public string Payee_Address { get; set; }
        public string Payee_Type { get; set; }
        public int Payee_No { get; set; }
        public int Payee_Creator_ID { get; set; }
        public int Payee_Approver_ID { get; set; }
        public DateTime Payee_Last_Updated { get; set; }
        public string Payee_Status { get; set; }
    }
}
