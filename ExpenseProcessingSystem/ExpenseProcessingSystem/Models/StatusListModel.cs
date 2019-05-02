using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class StatusListModel
    {
        [Key]
        public int Status_ID { get; set; }
        public string Status_Name { get; set; }
    }
}
