using ExpenseProcessingSystem.ViewModels.Search_Filters.Home;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public PendingGenFiltersViewModel GenPendFil { get; set; }
        public HistoryFiltersViewModel HistoryFil { get; set; }
        public FiltersViewModel()
        {
            NotifFil = new NotifFiltersViewModel
            {
                NotifFil_Application_Maker_Name = "",
                NotifFil_Application_Type_Name = "",
                NotifFil_Message = "",
                NotifFil_Status_Name = "",
                Notif_Date = new DateTime()
            };
        }
    }
    public class AppHistoryViewModel
    {
        public string App_Voucher_No { get; set; }
        public int App_Entry_ID { get; set; }
        public int App_Maker_ID { get; set; }
        public string App_Maker_Name { get; set; }
        public int App_Approver_ID { get; set; }
        public string App_Approver_Name { get; set; }
        public List<string> App_Verifier_Name_List { get; set; }
        [DataType(DataType.Date)]
        public DateTime App_Date { get; set; }
        [DataType(DataType.Date)]
        public DateTime App_Last_Updated { get; set; }
        public string App_Status { get; set; }
        public string App_Link { get; set; }
    }
    public class ApplicationsViewModel
    {
        public int App_ID { get; set; }
        public string App_Type { get; set; }
        public double App_Amount { get; set; }
        public string App_Payee { get; set; }
        public string App_Maker { get; set; }
        public List<string> App_Verifier_ID_List { get; set; }
        [DataType(DataType.Date)]
        public DateTime App_Date { get; set; }
        [DataType(DataType.Date)]
        public DateTime App_Last_Updated { get; set; }
        public string App_Status { get; set; }
        public string App_Link { get; set; }
    }
    public class HomeNotifViewModel
    {
        public int Notif_ID { get; set; }
        public int Notif_Application_Type_ID { get; set; }
        public string Notif_Application_Type_Name { get; set; }
        public int Notif_Application_Maker_ID { get; set; }
        public string Notif_Application_Maker_Name { get; set; }
        public int Notif_UserFor_ID { get; set; } 
        public string Notif_Message { get; set; }
        [DataType(DataType.Date)]
        public DateTime Notif_Date { get; set; }
        public int Notif_Status_ID { get; set; }
        public string Notif_Status_Name { get; set; }
    }
    public class AccessViewModel
    {
        public bool isLoggedIn { get; set; }
        public bool isAdmin { get; set; }
        public string accessType { get; set; }
    }
}
