using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class SystemMessageModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Msg_ID { get; set; }
        public string Msg_Code { get; set; }
        public string Msg_Type { get; set; }
        public string Msg_Content { get; set; }
    }
}
