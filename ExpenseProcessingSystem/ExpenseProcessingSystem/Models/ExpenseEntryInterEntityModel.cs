using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.Models
{
    public class ExpenseEntryInterEntityModel
    {
        //Currency ABBRS are masterIDs
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpDtl_DDVInter_ID { get; set; }
        public int ExpDtl_DDVInter_Curr1_ID { get; set; }
        public int ExpDtl_DDVInter_Curr2_ID { get; set; }
        public float ExpDtl_DDVInter_Amount1 { get; set; }
        public float ExpDtl_DDVInter_Amount2 { get; set; }
        public float ExpDtl_DDVInter_Conv_Amount1 { get; set; }
        public float ExpDtl_DDVInter_Conv_Amount2 { get; set; }
        public float ExpDtl_DDVInter_Rate { get; set; }
        public bool ExpDtl_DDVInter_Check1 { get; set; }
        public bool ExpDtl_DDVInter_Check2 { get; set; }

        public ICollection<ExpenseEntryInterEntityParticularModel> ExpenseEntryInterEntityParticular { get; set; }
        public ExpenseEntryDetailModel ExpenseEntryDetailModel { get; set; }
    }
    public class ExpenseEntryInterEntityParticularModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InterPart_ID { get; set; }
        public string InterPart_Particular_Title { get; set; }

        public ICollection<ExpenseEntryInterEntityAccsModel> ExpenseEntryInterEntityAccs { get; set; }
        public ExpenseEntryInterEntityModel ExpenseEntryInterEntityModel { get; set; }
    }
    public class ExpenseEntryInterEntityAccsModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InterAcc_ID { get; set; }
        public int InterAcc_Acc_ID { get; set; }
        public int InterAcc_Curr_ID { get; set; }
        public float InterAcc_Amount { get; set; }
        public float InterAcc_Rate { get; set; }
        public int InterAcc_Type_ID { get; set; }
        public ExpenseEntryInterEntityParticularModel ExpenseEntryInterEntityParticular { get; set; }
    }
}
