using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMVendorViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Vendor ID")]
        public int Vendor_MasterID { get; set; }
        [Display(Name = "Vendor Name")]
        [NotNullValidations, TextValidation]
        public string Vendor_Name { get; set; }
        [Display(Name = "Vendor TIN")]
        [NotNullValidations, TextValidation]
        public string Vendor_TIN { get; set; }
        [Display(Name = "Vendor Address")]
        [NotNullValidations, TextValidation]
        public string Vendor_Address { get; set; }
        [Display(Name = "Tax Rate/s")]
        public List<string> Vendor_Tax_Rate { get; set; }
        public List<int> Vendor_Tax_Rate_ID { get; set; }
        [Display(Name = "VAT/s")]
        public List<string> Vendor_VAT { get; set; }
        public List<int> Vendor_VAT_ID { get; set; }
        public int Vendor_Creator_ID { get; set; }
        public int Vendor_Approver_ID { get; set; }
        [Display(Name = "Vendor Created")]
        public DateTime Vendor_Created_Date { get; set; }
        [Display(Name = "Vendor Last Updated")]
        public DateTime Vendor_Last_Updated { get; set; }
        public string Vendor_Creator_Name { get; set; }
        public string Vendor_Approver_Name { get; set; }
        [Display(Name = "Vendor Status")]
        public int Vendor_Status_ID { get; set; }
        public string Vendor_Status { get; set; }
    }
}
