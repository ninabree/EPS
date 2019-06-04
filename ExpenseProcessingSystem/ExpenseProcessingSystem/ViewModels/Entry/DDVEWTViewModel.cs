using ExpenseProcessingSystem.Services.Validations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class DDVEWTViewModel
    {
        [NotNullValidations]
        [Display(Name = "GBase Remarks")]
        public string tax_payor { get; set; }
        [Display(Name = "GBase Remarks")]
        public string table_ID { get; set; }
        public List<SelectListItem> vendor_list { get; set; }
    }
}
