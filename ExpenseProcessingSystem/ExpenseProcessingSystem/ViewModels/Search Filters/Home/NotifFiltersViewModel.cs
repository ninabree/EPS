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
    }
}
