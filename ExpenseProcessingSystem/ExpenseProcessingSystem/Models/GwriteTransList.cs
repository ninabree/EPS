using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class GwriteTransList
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GW_ID { get; set; }
        public int GW_GWrite_ID { get; set; }
        public int GW_TransID { get; set; }
        public int GW_Status { get; set; }
    }
}
