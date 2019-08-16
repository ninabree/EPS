using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace ExpenseProcessingSystem.ViewModels.Entry
{
    public class DDVInterEntityViewModel
    {
        public int Inter_ID { get; set; }
        public int Inter_Currency1_ID { get; set; }
        public int Inter_Currency2_ID { get; set; }
        public string Inter_Currency1_ABBR { get; set; }
        public string Inter_Currency2_ABBR { get; set; }
        public decimal Inter_Currency1_Amount { get; set; }
        public decimal Inter_Currency2_Amount { get; set; }
        public decimal Inter_Convert1_Amount { get; set; }
        public decimal Inter_Convert2_Amount { get; set; }
        public bool Inter_Check1 { get; set; }
        public bool Inter_Check2 { get; set; }
        public float Inter_Rate { get; set; }
        public List<ExpenseEntryInterEntityParticularViewModel> interPartList { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }

        public DDVInterEntityViewModel()
        {
            interPartList = new List<ExpenseEntryInterEntityParticularViewModel>();
        }
    }
    public class ExpenseEntryInterEntityParticularViewModel
    {
        public int InterPart_ID { get; set; }
        public string InterPart_Particular_Title { get; set; }
        public List<InterEntityParticular> Inter_Particular1 { get; set; }
        public List<InterEntityParticular> Inter_Particular2 { get; set; }
        public List<InterEntityParticular> Inter_Particular3 { get; set; }
        public List<ExpenseEntryInterEntityAccsViewModel> ExpenseEntryInterEntityAccs { get; set; }

        public ExpenseEntryInterEntityParticularViewModel()
        {
            Inter_Particular1 = new List<InterEntityParticular>();
            Inter_Particular2 = new List<InterEntityParticular>();
            Inter_Particular3 = new List<InterEntityParticular>();
        }
    }
    public class ExpenseEntryInterEntityAccsViewModel
    {
        public int Inter_Acc_ID { get; set; }
        public int Inter_Curr_ID { get; set; }
        public decimal Inter_Amount { get; set; }
        public decimal Inter_Rate { get; set; }
        public int Inter_Type_ID { get; set; }
    }
    public class InterEntityParticular
    {
        public int Particular_Acc_ID { get; set; }
        public string Particular_Account_Name { get; set; }
        public decimal Particular_Debit_Amount { get; set; }
        public int Particular_DebCurr_ID { get; set; }
        public string Particular_Debit_Curr { get; set; }
        public decimal Particular_Credit_Amount { get; set; }
        public int Particular_CredCurr_ID { get; set; }
        public string Particular_Credit_Curr { get; set; }
        public float Particular_Credit_Rate { get; set; }
        public int Particular_Type_ID { get; set; }
    }
}
