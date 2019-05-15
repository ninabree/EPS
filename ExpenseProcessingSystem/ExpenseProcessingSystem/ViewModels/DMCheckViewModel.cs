using ExpenseProcessingSystem.Services.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMCheckViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Check_ID { get; set; }
        public int Check_MasterID { get; set; }
        [Display(Name = "Input Date")]
        [DataType(DataType.Date)]
        public DateTime Check_Input_Date { get; set; }
        [Display(Name = "Series From")]
        public string Check_Series_From { get; set; }
        [Display(Name = "Series To")]
        public string Check_Series_To { get; set; }
        [Display(Name = "Bank Information")]
        [NotNullValidations, TextValidation]
        public string Check_Bank_Info { get; set; }
        public int Check_Creator_ID { get; set; }
        public int Check_Approver_ID { get; set; }
        [Display(Name = "Input By")]
        public string Check_Creator_Name { get; set; }
        [Display(Name = "Approved By")]
        public string Check_Approver_Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime Check_Created_Date { get; set; }
        [DataType(DataType.Date)]
        public DateTime Check_Last_Updated { get; set; }
        [Display(Name = "Status")]
        public int Check_Status_ID { get; set; }
        public string Check_Status { get; set; }
        public bool Check_isDeleted { get; set; }
    }
}
