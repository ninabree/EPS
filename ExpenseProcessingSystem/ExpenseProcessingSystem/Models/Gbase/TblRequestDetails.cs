using System;
using System.Collections.Generic;

namespace ExpenseProcessingSystem.Models.Gbase
{
    public partial class TblRequestDetails
    {
        public decimal RequestId { get; set; }
        public string RacfId { get; set; }
        public string RacfPassword { get; set; }
        public DateTime RequestCreated { get; set; }
        public string ReturnMessage { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string SystemAbbr { get; set; }
        public int Priority { get; set; }

        public ICollection<TblRequestItem> tblRequestItems { get; set; }
    }
}
