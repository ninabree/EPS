using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.Models
{
    public class DMVendorModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Vendor_ID { get; set; }
        public int Vendor_MasterID { get; set; }
        public string Vendor_Name { get; set; }
        public string Vendor_TIN { get; set; }
        public int Vendor_Zip { get; set; }
        public string Vendor_Address { get; set; }
        public int Vendor_Creator_ID { get; set; }
        public int Vendor_Approver_ID { get; set; }
        public DateTime Vendor_Created_Date { get; set; }
        public DateTime Vendor_Last_Updated { get; set; }
        public int Vendor_Status_ID { get; set; }
        public bool Vendor_isDeleted { get; set; }
        public bool Vendor_isActive { get; set; }
    }
}
