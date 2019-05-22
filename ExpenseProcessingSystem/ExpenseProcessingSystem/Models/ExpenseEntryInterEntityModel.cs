using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryInterEntityModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Inter_ID { get; set; }
        public string Inter_Particular_Title { get; set; }
        public string Inter_Currency1_ABBR { get; set; }
        public string Inter_Currency2_ABBR { get; set; }
        public string Inter_Currency1_Amount { get; set; }
        public string Inter_Currency2_Amount { get; set; }
        public string Inter_Rate { get; set; }
        public ExpenseEntryDetailModel ExpenseEntryDetailModel { get; set; }
    }
}
