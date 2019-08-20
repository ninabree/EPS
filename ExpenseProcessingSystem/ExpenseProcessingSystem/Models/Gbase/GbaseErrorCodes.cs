using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models.Gbase
{
    public class GbaseErrorCodes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string GERR_ID { get; set; }
        public string GERR_NO { get; set; }
        public string GERR_STATUS { get; set; }
        public string GERR_MESSAGE { get; set; }
        public string GERR_COMMENT { get; set; }
    }
}
