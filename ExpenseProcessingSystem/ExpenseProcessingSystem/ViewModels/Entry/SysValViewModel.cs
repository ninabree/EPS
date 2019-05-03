﻿using Microsoft.AspNetCore.Mvc.Rendering;
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
        public List<accDetails> acc { get; set; }
        public SelectList currency { get; set; }
    }

    public class accDetails
    {
        public int accId { get; set; }
        public string accName { get; set; }
        public string accCode { get; set; }
    }
}
