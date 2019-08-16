using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class LiquidationMainListViewModel
    {
        public int App_ID { get; set; }
        public string App_Type { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public decimal App_Amount { get; set; }
        public string App_Payee { get; set; }
        public string App_Maker { get; set; }
        public string App_Approver { get; set; }
        public List<string> App_Verifier_ID_List { get; set; }
        public DateTime App_Date { get; set; }
        public DateTime App_Last_Updated { get; set; }
        public string App_Status { get; set; }
        public string App_Link { get; set; }
    }
}
