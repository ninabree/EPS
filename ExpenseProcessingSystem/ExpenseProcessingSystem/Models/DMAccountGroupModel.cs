using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class DMAccountGroupModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountGroup_ID { get; set; }
        public int AccountGroup_MasterID { get; set; }
        public string AccountGroup_Name { get; set; }
    }
}
