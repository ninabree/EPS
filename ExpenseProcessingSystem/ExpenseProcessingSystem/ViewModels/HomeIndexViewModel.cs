using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class HomeIndexViewModel
    {
        List<NotifViewModel> NotifList { get; set; }
        List<ApplicationsViewModel> AppList { get; set; }
        //public AccessViewModel Access { get; set; }
    }
    public class ApplicationsViewModel
    {
        public int App_ID { get; set; }
        public string App_Type { get; set; }
        public double App_Amount { get; set; }
        public string App_Payee { get; set; }
        public string App_Maker { get; set; }
        public string App_Verifier { get; set; }
        public DateTime App_Date { get; set; }
        public DateTime App_Last_Updated { get; set; }
        public string App_Status { get; set; }
    }
    public class NotifViewModel
    {
        public int Notif_ID { get; set; }
        public string Notif_Message { get; set; }
        public DateTime Notif_Date { get; set; }
        public string Notif_Status { get; set; }
        public string Notif_Verifier_approver { get; set; }
    }
    public class AccessViewModel
    {
        public bool isLoggedIn { get; set; }
        public bool isAdmin { get; set; }
        public string accessType { get; set; }
    }
}
