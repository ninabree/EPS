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
        public string Pending_Type { get; set; }
        [Display(Name = "Payee")]
        public string Pending_Payee { get; set; }
        [Display(Name = "Maker")]
        public string Pending_Maker { get; set; }
        [Display(Name = "Amount")]
        public string Pending_Amount { get; set; }
        [Display(Name = "Date Submitted")]
        public DateTime Pending_Created_Date { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime Pending_Updated_Date { get; set; }
        [Display(Name = "Status")]
        public string Pending_Status { get; set; }

        public SelectList Pending_Type_Select { get; set; }

        public PendingGenFiltersViewModel()
        {
            Pending_Type_Select = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_CV), Value = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_CV)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_DDV), Value = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_DDV)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_NC), Value = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_NC)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_PC), Value = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_PC)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_SS), Value = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_SS)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_LIQ), Value = GlobalSystemValues.getApplicationType(GlobalSystemValues.TYPE_LIQ)}
                }, "Value", "Text");
        }
    }
    public class HistoryFiltersViewModel
    {
        public string Hist_Voucher_Type { get; set; }
        public string Hist_Voucher_Year { get; set; }
        [Display(Name = "Voucher No")]
        public string Hist_Voucher_No { get; set; }
        [Display(Name = "Maker Name")]
        public string Hist_Maker { get; set; }
        [Display(Name = "Approver Name")]
        public string Hist_Approver { get; set; }
        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Hist_Created_Date { get; set; }
        [Display(Name = "Date Updated")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Hist_Updated_Date { get; set; }
        [Display(Name = "Status")]
        public string Hist_Status { get; set; }

        public IEnumerable<YearList> Hist_YearList { get; set; }
        public SelectList Hist_Type_Select { get; set; }
        public HistoryFiltersViewModel()
        {
            Hist_Type_Select = new SelectList(
                new List<SelectListItem>
                {
                    //new SelectListItem { Text = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_DM), Value = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_DM)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_CV), Value = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_CV)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_DDV), Value = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_DDV)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_NC), Value = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_NC)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_PC), Value = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_PC)},
                    new SelectListItem { Text = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_SS), Value = GlobalSystemValues.getApplicationCode(GlobalSystemValues.TYPE_SS)}
                }, "Value", "Text");

            Hist_YearList = ConstantData.HomeReportConstantValue.GetYearList();
        }
    }
}
