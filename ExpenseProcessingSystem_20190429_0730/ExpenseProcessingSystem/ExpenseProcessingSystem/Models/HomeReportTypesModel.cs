using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class HomeReportTypesModel
    {
        public byte Id { get; set; }

        public string TypeName { get; set; }

        public bool SubTypeAvail { get; set; }
    }
}
