using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class PettyCashModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PC_ID { get; set; }
        public decimal PC_StartBal { get; set; }
        public double PC_Disbursed { get; set; }
        public double PC_Recieved { get; set; }
        public decimal PC_EndBal { get; set; }
        public int PC_CloseUser { get; set; }
        public int PC_OpenUser { get; set; }
        public bool PC_OpenConfirm { get; set; }
        public string PC_ConfirmComment { get; set; }
        public int PC_Status { get; set; }
        public DateTime PC_OpenDate { get; set; }
        public DateTime PC_CloseDate { get; set; }

        public int PCB_OneThousand { get; set; }
        public int PCB_FiveHundred { get; set; }
        public int PCB_TwoHundred { get; set; }
        public int PCB_OneHundred { get; set; }
        public int PCB_Fifty { get; set; }
        public int PCB_Twenty { get; set; }
        public int PCB_Ten { get; set; }
        public int PCB_Five { get; set; }
        public int PCB_One { get; set; }
        public int PCB_TwentyFiveCents { get; set; }
        public int PCB_TenCents { get; set; }
        public int PCB_FiveCents { get; set; }
        public int PCB_OneCents { get; set; }
    }
}
