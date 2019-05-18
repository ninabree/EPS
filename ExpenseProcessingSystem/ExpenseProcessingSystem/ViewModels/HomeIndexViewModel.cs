using ExpenseProcessingSystem.ViewModels.Search_Filters.Home;
using System;
using System.Collections.Generic;

namespace ExpenseProcessingSystem.ViewModels
{
    public class HomeIndexViewModel
    {
        public AccessViewModel Access { get; set; }
        public FiltersViewModel Filters { get; set; }
        public PaginatedList<HomeNotifViewModel> NotifList { get; set; }
        public PaginatedList<ApplicationsViewModel> PersonalPendingList { get; set; }
        public PaginatedList<ApplicationsViewModel> GeneralPendingList { get; set; }
        public PaginatedList<AppHistoryViewModel> HistoryList { get; set; }
    }
    public class FiltersViewModel
    {
        public NotifFiltersViewModel NotifFil { get; set; }
        //public PendingPerFiltersViewModel PersPendFil { get; set; }
        //public PendingGenFiltersViewModel GenPendFil { get; set; }
        //public HistoryFiltersViewModel HistoryFil { get; set; }
    }
    public class AppHistoryViewModel
    {
        public int App_ID { get; set; }
        public string App_Type { get; set; }
        public double App_Amount { get; set; }
        public string App_Payee { get; set; }
        public string App_Maker { get; set; }
        public int App_Approver_ID{ get; set; }
        public List<int> App_Verifier_ID_List { get; set; }
        public DateTime App_Date { get; set; }
        public DateTime App_Last_Updated { get; set; }
        public string App_Status { get; set; }
    }
    public class ApplicationsViewModel
    {
        public int App_ID { get; set; }
        public string App_Type { get; set; }
        public double App_Amount { get; set; }
        public string App_Payee { get; set; }
        public string App_Maker { get; set; }
        public List<string> App_Verifier_ID_List { get; set; }
        public DateTime App_Date { get; set; }
        public DateTime App_Last_Updated { get; set; }
        public string App_Status { get; set; }
    }
    public class HomeNotifViewModel
    {
        public int Notif_ID { get; set; }
        public int Notif_Application_ID { get; set; }
        public string Notif_Message { get; set; }
        public DateTime Notif_Last_Updated { get; set; }
        public string Notif_Status { get; set; }
        public string Notif_Type_Status { get; set; }
        public string Notif_Verifier_Approver { get; set; }
        public int Notif_Verifier_Approver_ID { get; set; }
    }
    public class AccessViewModel
    {
        public bool isLoggedIn { get; set; }
        public bool isAdmin { get; set; }
        public string accessType { get; set; }
        public string userId { get; set; }
    }
}
