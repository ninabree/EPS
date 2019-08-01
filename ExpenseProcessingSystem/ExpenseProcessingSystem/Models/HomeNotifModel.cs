using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class HomeNotifModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Notif_ID { get; set; }
        public int Notif_Application_Type_ID { get; set; } // in GlobalValuesSystem ex.Type_CV...
        public int Notif_Application_Maker_ID { get; set; }
        public int Notif_UserFor_ID { get; set; } // in GLobalValuesSystem ex. UF_ALL == 0, each for the involved maker, verifier and approver
        public string Notif_Message { get; set; } // ex. You have (1) approved Check Voucher Application/s. from dbo.SystemMessages
        public DateTime Notif_Date { get; set; }
        public int Notif_Status_ID { get; set; } // in GlobalValuesSystem ex.Status_Pending
    }
}
