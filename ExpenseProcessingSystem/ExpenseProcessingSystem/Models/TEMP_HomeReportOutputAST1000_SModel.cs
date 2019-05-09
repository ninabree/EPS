using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class TEMP_HomeReportOutputAST1000Model
    {
        public int SeqNo { get; set; }

        public string Tin { get; set; }

        public string SupplierName { get; set; }

        public string NOIP { get; set; }

        public string ATC { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double TaxBase { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public double RateOfTax { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double AOTW { get; set; }
    }
}
