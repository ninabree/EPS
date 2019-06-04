using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class SysValViewModel
    {
        public List<SelectListItem> category_of_entries { get; set; }
        public SelectList vendors { get; set; }
        public SelectList ewt { get; set; }
        public SelectList vat { get; set; }
        public SelectList dept { get; set; }
        public List<accDetails> acc { get; set; }
        public SelectList currency { get; set; }
    }

    public class accDetails
    {
        public int accId { get; set; }
        public string accName { get; set; }
        public string accCode { get; set; }
    }

    public class vatDetails
    {
        public int vatId { get; set; }
        public string vatName { get; set; }
        public double vatRate { get; set; }
    }
}
