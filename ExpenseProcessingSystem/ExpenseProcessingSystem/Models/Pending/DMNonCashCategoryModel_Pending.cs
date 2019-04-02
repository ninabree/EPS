using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMNonCashCategoryModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_NCC_ID { get; set; }
        public int Pending_NCC_MasterID { get; set; }
        public string Pending_NCC_Name { get; set; }
        public string Pending_NCC_Pro_Forma { get; set; }
        public int Pending_NCC_Creator_ID { get; set; }
        public int Pending_NCC_Approver_ID { get; set; }
        public DateTime Pending_NCC_Filed_Date { get; set; }
        public string Pending_NCC_Status { get; set; }
        public bool Pending_NCC_isDeleted { get; set; }
        public bool Pending_NCC_isActive { get; set; }
    }
}
