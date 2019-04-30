using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMAccountViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Account_ID { get; set; }
        public int Account_MasterID { get; set; }
        [Display(Name = "Account Name")]
        public string Account_Name { get; set; }
        [Display(Name = "Account Code")]
        public string Account_Code { get; set; }
        [Display(Name = "Account No")]
        public string Account_No { get; set; }
        [Display(Name = "Account Cust")]
        public string Account_Cust { get; set; }
        [Display(Name = "Account Div")]
        public string Account_Div { get; set; }
        [Display(Name = "Account Fund")]
        public bool Account_Fund { get; set; }
        public int Account_Creator_ID { get; set; }
        public int Account_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string Account_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string Account_Approver_Name { get; set; }
        [Display(Name = "Account Created Date")]
        public DateTime Account_Created_Date { get; set; }
        [Display(Name = "Account Last Updated Date")]
        public DateTime Account_Last_Updated { get; set; }
        [Display(Name = "Account Status")]
        public string Account_Status { get; set; }
        public bool Account_isDeleted { get; set; }
    }
}
