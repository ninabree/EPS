using ExpenseProcessingSystem.ConstantData;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters.Home
{
    public class NotifFiltersViewModel
    {
        [Display(Name = "Application Type")]
        public string NotifFil_Application_Type_Name { get; set; }
        [Display(Name = "Message Contains")]
        public string NotifFil_Message { get; set; }
        [Display(Name = "Date")]
        public DateTime Notif_Date { get; set; }
        [Display(Name = "Notification Status")]
        public string NotifFil_Status_Name { get; set; }
        [Display(Name = "Application Maker Name")]
        public string NotifFil_Application_Maker_Name { get; set; }
        public SelectList NotifFil_Application_Type_Select { get; set; }

        public NotifFiltersViewModel()
        {
            NotifFil_Application_Type_Select = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Text = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_CV], Value = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_CV]},
                    new SelectListItem { Text =NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_DM], Value = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_DM]},
                    new SelectListItem { Text = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_DDV], Value = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_DDV]},
                    new SelectListItem { Text =NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_LIQ], Value = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_LIQ]},
                    new SelectListItem { Text = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_NC], Value = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_NC]},
                    new SelectListItem { Text = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_PC], Value = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_PC]},
                    new SelectListItem { Text = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_SS], Value = NotificationMessageValues.TYTIONARY[GlobalSystemValues.TYPE_SS]}
                }, "Value", "Text");
        }
    }
}
