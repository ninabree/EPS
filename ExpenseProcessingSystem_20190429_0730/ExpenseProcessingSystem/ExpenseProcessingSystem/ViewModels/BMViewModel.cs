using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class BMViewModel
    {
        public int BM_Id { get; set; }
        public string BM_Account { get; set; }
        public string BM_Type { get; set; }
        public int BM_Budget { get; set; }
        public int BM_Curr_Budget { get; set; }
        public DateTime BM_Last_Trans_Date { get; set; }
        public string BM_Last_Budget_Approval { get; set; }
        public int BM_Creator_ID { get; set; }
        public int BM_Approver_ID { get; set; }
    }
}
