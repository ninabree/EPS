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
        public int Notif_ID { get; set; } // auto increment
        public int Notif_Application_ID { get; set; } // like master ID
        public int Notif_User_ID { get; set; }
        public int Notif_Verifr_Apprvr_ID { get; set; }
        public string Notif_Message { get; set; } // ex. You have (1) approved Check Voucher Application/s.
        //where to redirect user to view the application being notified 
        //public string Notif_Link_Address { get; set; }//ex.Entry_CV/Home/apprv
        public DateTime Notif_Created_Date { get; set; }
        public DateTime Notif_Last_Updated { get; set; }
        public bool Notif_Status { get; set; } // if already seen
        public string Notif_Type_Status { get; set; } // ex. Verified
        //public string Notif_Type_Screen { get; set; } // ex. Entry_CV
        //public string Notif_Type_User { get; set; } // ex. Maker
    }
}
