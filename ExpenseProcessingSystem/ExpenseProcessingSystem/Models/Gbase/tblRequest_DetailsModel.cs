using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Gbase
{
    public class tblRequest_DetailsModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestID { get; set; }
        public string RacfID { get; set; }
        public string RacfPassword { get; set; }
        public DateTime RequestCreated { get; set; }
        public string ReturnMessage { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string SystemAbbr { get; set; }
        public int Priority { get; set; }

        public List<tblRequest_ItemModel> Request_Item { get; set; }
    }
}
