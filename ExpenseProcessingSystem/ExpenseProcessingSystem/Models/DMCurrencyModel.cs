using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMCurrencyModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Curr_ID { get; set; }
        public int Curr_MasterID { get; set; }
        public string Curr_Name { get; set; }
        public string Curr_CCY_ABBR { get; set; }
        public int Curr_Creator_ID { get; set; }
        public int Curr_Approver_ID { get; set; }
        public DateTime Curr_Created_Date { get; set; }
        public DateTime Curr_Last_Updated { get; set; }
        public int Curr_Status_ID { get; set; }
        public bool Curr_isDeleted { get; set; }
        public bool Curr_isActive { get; set; }
    }
}
