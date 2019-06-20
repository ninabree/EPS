using System;
using System.Collections.Generic;

namespace ExpenseProcessingSystem.Models.Gbase
{
    public partial class TblRequestItem
    {
        public int ItemId { get; set; }
        public int SequenceNo { get; set; }
        public bool ReturnFlag { get; set; }
        public string Command { get; set; }
        public string ScreenCapture { get; set; }
        public int RequestId { get; set; }

        public TblRequestDetails Request { get; set; }
    }
}
