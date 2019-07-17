using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class HomeReportAccountSummaryViewModel
    {
        //Needed in report
        public string Trans_Voucher_Number { get; set; }
        public string Trans_Check_Number { get; set; }
        public string Trans_Value_Date { get; set; }
        public string Trans_Reference_No { get; set; }
        public string Trans_Section { get; set; }
        public string Trans_Remarks { get; set; }
        public string Trans_DebitCredit { get; set; }
        public string Trans_Currency { get; set; }
        public string Trans_Amount { get; set; }
        public string Trans_Customer { get; set; }
        public string Trans_Account_Code { get; set; }
        public string Trans_Account_Number { get; set; }
        public string Trans_Account_Name { get; set; }
        public string Trans_Exchange_Rate { get; set; }
        public string Trans_Contra_Currency { get; set; }
        public string Trans_Fund { get; set; }
        public string Trans_Advice_Print { get; set; }
        public string Trans_Details { get; set; }
        public string Trans_Entity { get; set; }
        public string Trans_Division { get; set; }
        public string Trans_InterAmount { get; set; }
        public string Trans_InterRate { get; set; }

        //Etc
        public int ExpExpense_ID { get; set; }
        public int ExpExpense_Type { get; set; }
        public string ExpExpense_Date { get; set; }
        public int HistExpenseEntryID { get; set; }
        public int HistExpenseDetailID { get; set; }
        public int HistGOExpHist_Id { get; set; }
        public string HistGOExpHist_ValueDate { get; set; }
        public string HistGOExpHist_ReferenceNo { get; set; }
        public string HistGOExpHist_Section { get; set; }
        public string HistGOExpHist_Remarks { get; set; }
        public string HistGOExpHist_Memo { get; set; }
        public int TransTL_ID { get; set; }
        public int TransTL_GoExpress_ID { get; set; }
        public int TransTL_TransID { get; set; }
        public int NCExpNC_Category_ID { get; set; }
        public DateTime Trans_Last_Updated_Date { get; set; }
    }
}
