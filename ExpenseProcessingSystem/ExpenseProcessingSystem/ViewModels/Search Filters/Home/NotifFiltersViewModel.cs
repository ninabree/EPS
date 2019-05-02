using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Search_Filters.Home
{
    public class NotifFiltersViewModel
    {
        [Display(Name = "Message Contains")]
        public string NotifFil_Message { get; set; }
        [Display(Name = "Date")]
        public DateTime Notif_Last_Updated { get; set; }
        [Display(Name = "Notification Status")]
        public string NotifFil_Status { get; set; }
        [Display(Name = "Verifier/Approver Name")]
        public string NotifFil_Verifier_Approver_Name { get; set; }
    }
}
