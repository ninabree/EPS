using ExpenseProcessingSystem.ConstantData;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters.Home
{
    public class PendingGenFiltersViewModel
    {
        [Display(Name = "Application Type")]
        public int Pending_Type { get; set; }
        [Display(Name = "Payee")]
        public string Pending_Payee { get; set; }
        [Display(Name = "Maker")]
        public string Pending_Maker { get; set; }
        [Display(Name = "Verifier Name")]
        public string Pending_Verifier { get; set; }
        [Display(Name = "Date Created")]
        public DateTime Pending_Created_Date { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime Pending_Updated_Date { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }

        public SelectList Pending_Type_Select { get; set; }
        public PendingGenFiltersViewModel()
        {
            Pending_Type_Select = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_DM), Value = GlobalSystemValues.TYPE_DM.ToString()},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_CV), Value = GlobalSystemValues.TYPE_CV.ToString()},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_DDV), Value = GlobalSystemValues.TYPE_DDV.ToString()},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_SS), Value = GlobalSystemValues.TYPE_SS.ToString()},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_PC), Value = GlobalSystemValues.TYPE_PC.ToString()},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_NC), Value = GlobalSystemValues.TYPE_NC.ToString()}
                }, "Value", "Text");
        }
    }
}
