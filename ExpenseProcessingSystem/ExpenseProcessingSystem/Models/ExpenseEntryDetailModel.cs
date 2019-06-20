using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryDetailModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpDtl_ID { get; set; }
        public string ExpDtl_Gbase_Remarks { get; set; }
        public int ExpDtl_Account { get; set; }
        public bool ExpDtl_Inter_Entity { get; set; }
        public bool ExpDtl_Fbt { get; set; }
        public int ExpDtl_FbtID { get; set; }
        public int ExpDtl_Dept { get; set; }
        public int ExpDtl_Vat { get; set; }
        public bool ExpDtl_isEwt { get; set; }
        public int ExpDtl_Ewt { get; set; }
        public int ExpDtl_Ccy { get; set; }
        public float ExpDtl_Debit { get; set; }
        public float ExpDtl_Credit_Ewt { get; set; }
        public float ExpDtl_Credit_Cash { get; set; }
        public int ExpDtl_Amor_Month { get; set; }
        public int ExpDtl_Amor_Day { get; set; }
        public int ExpDtl_Amor_Duration { get; set; }
        public string ExpDtl_Ewt_Payor_Name { get; set; }
        public int ExpDtl_SS_Payee { get; set; }
        public ExpenseEntryModel ExpenseEntryModel { get; set; }

        public ICollection<ExpenseEntryGbaseDtl> ExpenseEntryGbaseDtls { get; set; }
        public ICollection<ExpenseEntryAmortizationModel> ExpenseEntryAmortizations { get; set; }
        public ICollection<ExpenseEntryCashBreakdownModel> ExpenseEntryCashBreakdowns { get; set; }
        public ICollection<ExpenseEntryInterEntityModel> ExpenseEntryInterEntity { get; set; }
    }
}
