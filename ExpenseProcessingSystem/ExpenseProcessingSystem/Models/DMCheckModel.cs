using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMCheckModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Check_ID { get; set; }
        public DateTime Check_Input_Date { get; set; }
        public string Check_Series_From { get; set; }
        public string Check_Series_To { get; set; }
        public string Check_Type { get; set; }
        public string Check_Name { get; set; }
        public int Check_Creator_ID { get; set; }
        public int Check_Approver_ID { get; set; }
        public DateTime Check_Created_Date { get; set; }
        public DateTime Check_Last_Updated { get; set; }
        public string Check_Status { get; set; }
        public bool Check_isDeleted { get; set; }
    }
}
