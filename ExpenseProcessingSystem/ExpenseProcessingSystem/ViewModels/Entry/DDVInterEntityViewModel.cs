using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class DDVInterEntityViewModel
    {
        public string Inter_ID { get; set; }
        public string Inter_Particular_Title { get; set; }
        public string Inter_Currency1_ABBR { get; set; }
        public string Inter_Currency2_ABBR { get; set; }
        public string Inter_Currency1_Amount { get; set; }
        public string Inter_Currency2_Amount { get; set; }
        public string Inter_Rate { get; set; }
        public List<InterEntityParticular> Inter_Particular1 { get; set; }
        public List<InterEntityParticular> Inter_Particular2 { get; set; }
        public List<InterEntityParticular> Inter_Particular3 { get; set; }

        public DDVInterEntityViewModel()
        {
            Inter_Particular1 = new List<InterEntityParticular>();
            Inter_Particular2 = new List<InterEntityParticular>();
            Inter_Particular3 = new List<InterEntityParticular>();
        }
    }
    public class InterEntityParticular
    {
        public string Particular_Account_Name { get; set; }
        public double Particular_Debit_Amount { get; set; }
        public string Particular_Debit_Curr { get; set; }
        public double Particular_Credit_Amount { get; set; }
        public string Particular_Credit_Curr { get; set; }
        public double Particular_Credit_Rate { get; set; }
    }
}
