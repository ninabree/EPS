using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ClosingModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Close_ID { get; set; }
        public string Close_Type { get; set; }
        public DateTime? Close_Date { get; set; }
        public DateTime Close_Open_Date { get; set; }
        public int Close_Status { get; set; }
        public int Close_User { get; set; }
    }
}
