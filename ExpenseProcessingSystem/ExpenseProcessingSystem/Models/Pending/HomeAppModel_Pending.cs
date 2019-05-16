using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class HomeAppModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_ID { get; set; }
        public int Pending_App_MasterID { get; set; }
        public string Pending_App_Type { get; set; }
        public double Pending_App_Amount { get; set; }
        public string Pending_App_Payee_ID { get; set; }
        public int Pending_App_Creator_ID { get; set; }
        public List<int> Pending_App_Verifier_ID { get; set; }
        public int Pending_App_Approver_ID { get; set; }
        public DateTime Pending_App_Filed_Date { get; set; }
        public int Pending_App_Status_ID { get; set; }
        public bool Pending_App_IsDeleted { get; set; }
    }
}
