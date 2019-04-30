using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMVendorModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_ID { get; set; }
        public int Pending_Vendor_MasterID { get; set; }
        public string Pending_Vendor_Name { get; set; }
        public string Pending_Vendor_TIN { get; set; }
        public string Pending_Vendor_Address { get; set; }
        public int Pending_Vendor_Creator_ID { get; set; }
        public int Pending_Vendor_Approver_ID { get; set; }
        public DateTime Pending_Vendor_Filed_Date { get; set; }
        public string Pending_Vendor_Status { get; set; }
        public bool Pending_Vendor_IsDeleted { get; set; }
        public bool Pending_Vendor_isActive { get; set; }
    }
}
