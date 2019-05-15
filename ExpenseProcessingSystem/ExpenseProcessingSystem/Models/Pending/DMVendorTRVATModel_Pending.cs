using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Pending
{
    public class DMVendorTRVATModel_Pending
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Pending_VTV_ID { get; set; }
        public int Pending_VTV_Vendor_ID { get; set; }
        public int Pending_VTV_TR_ID { get; set; }
        public int Pending_VTV_VAT_ID { get; set; }
        public int Pending_VTV_Status_ID { get; set; }
    }
}
