using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class FileLocationModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FL_ID { get; set; }
        public string FL_Type { get; set; }
        public string FL_Location { get; set; }
    }
}
