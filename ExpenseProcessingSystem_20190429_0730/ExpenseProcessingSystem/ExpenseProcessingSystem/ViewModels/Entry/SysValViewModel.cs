using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class SysValViewModel
    {
        public SelectList vendors { get; set; }
        public SelectList ewt { get; set; }
        public SelectList dept { get; set; }
        public SelectList acc { get; set; }
        public SelectList currency { get; set; }
    }
}
