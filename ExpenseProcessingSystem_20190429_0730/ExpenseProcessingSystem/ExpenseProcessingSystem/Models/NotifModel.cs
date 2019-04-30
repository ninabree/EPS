using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class NotifModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Notif_ID { get; set; }
        public int Notif_Application_ID { get; set; }
        public int Notif_User_ID { get; set; }
        public int Notif_Apprvr_ID { get; set; }
        public string Notif_Message { get; set; }
        public string Notif_Link_Address { get; set; }
        public DateTime Notif_Date { get; set; }
        public bool Notif_Status { get; set; }
        public string Notif_Type_Status { get; set; }
        public string Notif_Type_Screen { get; set; }
        //public string Notif_Type_User { get; set; }
    }
}
