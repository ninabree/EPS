using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.Models
{
    public class DMNonCashCategoryModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NCC_ID { get; set; }
        public int NCC_MasterID { get; set; }
        public string NCC_Name { get; set; }
        public string NCC_Pro_Forma { get; set; }
        public int NCC_Creator_ID { get; set; }
        public int NCC_Approver_ID { get; set; }
        public DateTime NCC_Created_Date { get; set; }
        public DateTime NCC_Last_Updated { get; set; }
        public string NCC_Status { get; set; }
        public bool NCC_isDeleted { get; set; }
        public bool NCC_isActive { get; set; }
    }
}
