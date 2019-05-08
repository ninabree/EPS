using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMVendorTRVATModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VTV_ID { get; set; }
        public int VTV_Vendor_ID { get; set; }
        public int VTV_TR_ID { get; set; }
        public int VTV_VAT_ID { get; set; }
    }
}
