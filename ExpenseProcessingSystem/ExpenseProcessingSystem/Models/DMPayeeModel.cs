using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMPayeeModel
    {
        //public DMPayeeModel()
        //{
        //    this.Payee_Pending = new HashSet<DMPayeeModel_Pending>();
        //}
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Payee_ID { get; set; }
        public int Payee_MasterID { get; set; }
        public string Payee_Name { get; set; }
        public string Payee_TIN { get; set; }
        public string Payee_Address { get; set; }
        public string Payee_Type { get; set; }
        public int Payee_No { get; set; }
        public int Payee_Creator_ID { get; set; }
        public int Payee_Approver_ID { get; set; }
        public DateTime Payee_Created_Date { get; set; }
        public DateTime Payee_Last_Updated { get; set; }
        public string Payee_Status { get; set; }
        public bool Payee_isDeleted { get; set; }
        public bool Payee_isActive { get; set; }

        //public virtual ICollection<DMPayeeModel_Pending> Payee_Pending { get; set; }
    }
}
