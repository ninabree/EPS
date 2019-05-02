using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class HomeReportSubTypesModel
    {
        public int Id { get; set; }

        public string SubTypeName { get; set; }

        public int ParentTypeId { get; set; }
    }
}
