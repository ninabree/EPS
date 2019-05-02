using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class EntryGbaseRemarksViewModel
    {
        public string docType { get; set; }
        public string invNo { get; set; }
        public string desc { get; set; }
        public float amount { get; set; }
    }
}
