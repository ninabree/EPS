using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ExpenseProcessingSystem.ViewModels
{
    public class DMAccountGroupViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountGroup_ID { get; set; }
        public int AccountGroup_MasterID { get; set; }
        [Display(Name = "Account Group Name")]
        public string AccountGroup_Name { get; set; }
        [Display(Name = "Account Group Code")]
        public string AccountGroup_Code { get; set; }
        public int AccountGroup_Creator_ID { get; set; }
        public int AccountGroup_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string AccountGroup_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string AccountGroup_Approver_Name { get; set; }
        [Display(Name = "Account Group Created Date")]
        [DataType(DataType.Date)]
        public DateTime AccountGroup_Created_Date { get; set; }
        [Display(Name = "Account Group Last Updated Date")]
        [DataType(DataType.Date)]
        public DateTime AccountGroup_Last_Updated { get; set; }
        [Display(Name = "Account Group Status")]
        public int AccountGroup_Status_ID { get; set; }
        public string AccountGroup_Status_Name { get; set; }
        public bool AccountGroup_isDeleted { get; set; }
    }
}
