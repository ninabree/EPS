using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Expense_ID { get; set; }
        public DateTime Expense_Date { get; set; }
        public string Expense_Number { get; set; }
        public int Expense_CheckId { get; set; }
        public int Expense_Type { get; set; }
        public string Expense_CheckNo { get; set; }
        public int Expense_Payee { get; set; }
        public float Expense_Debit_Total { get; set; }
        public float Expense_Credit_Total { get; set; }
        public int Expense_Verifier_1 { get; set; }
        public int Expense_Verifier_2 { get; set; }
        public int Expense_Approver { get; set; }
        public int Expense_Status { get; set; }
        public int Expense_Creator_ID { get; set; }
        public DateTime Expense_Created_Date { get; set; }
        public DateTime Expense_Last_Updated { get; set; }
        public bool Expense_isDeleted { get; set; }

        public ICollection<ExpenseEntryDetailModel> ExpenseEntryDetails { get; set; }
        public ICollection<ExpenseEntryNCModel> ExpenseEntryNC { get; set; }
    }
}
