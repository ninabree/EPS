using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class HomeReportOutputAPSWT_MModel
    {
        public string Tin { get; set; }

        public string Payee { get; set; }

        public string ATC { get; set; }

        public string NOIP { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double AOIP { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public double RateOfTax { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double AOTW { get; set; }

        public int Payee_SS_ID { get; set; }
        public int Payee_ID { get; set; }
    }
}
