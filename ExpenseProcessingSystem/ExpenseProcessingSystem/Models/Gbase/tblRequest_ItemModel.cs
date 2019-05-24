using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Gbase
{
    public class tblRequest_ItemModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemID { get; set; }
        public int SequenceNo { get; set; }
        public bool ReturnFlag { get; set; }
        public string Command { get; set; }
        public string ScreenCapture { get; set; }

        public int RequestID { get; set; }
        public tblRequest_DetailsModel Request_Details { get; set; }
    }
}
