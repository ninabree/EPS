﻿using System;
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

        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal AOIP { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public float RateOfTax { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal AOTW { get; set; }

        public int VAT_ID { get; set; }
        public int Vendor_masterID { get; set; }

        public DateTime Last_Update_Date { get; set; }

        public int SeqNo { get; set; }
    }
}
