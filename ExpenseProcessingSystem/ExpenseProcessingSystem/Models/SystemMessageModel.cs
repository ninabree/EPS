using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class SystemMessageModel
    {
        [Key]
        public string Msg_Code { get; set; }
        public string Msg_Type { get; set; }
        public string Msg_Content { get; set; }
    }
}
