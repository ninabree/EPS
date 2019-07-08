using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class HomeReportSubTypeAccModel
    {
        public string Id { get; set; }

        public string SubTypeName { get; set; }

        public int ParentTypeId { get; set; }
    }
}
