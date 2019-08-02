using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class HomeReportOutputAST1000Model
    {
        public int SeqNo { get; set; }

        public string Tin { get; set; }

        public string SupplierName { get; set; }

        public string NOIP { get; set; }

        public string ATC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public double TaxBase { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public double RateOfTax { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public double AOTW { get; set; }

        public int Payee_SS_ID { get; set; }
        public int Vendor_masterID { get; set; }

        public DateTime Last_Update_Date { get; set; }

    }
}
