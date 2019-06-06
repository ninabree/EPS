using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryNCModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpNC_ID { get; set; }
        public int ExpNC_Category_ID { get; set; }
        public ExpenseEntryModel ExpenseEntryModel { get; set; }

        public ICollection<ExpenseEntryNCDtlModel> ExpenseEntryNCDtls { get; set; }
    }
}
