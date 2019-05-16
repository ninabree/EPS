using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class DMCustViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Cust_ID { get; set; }
        public int Cust_MasterID { get; set; }
        [Display(Name = "Customer Name")]
        public string Cust_Name { get; set; }
        [Display(Name = "Customer Abbr")]
        public string Cust_Abbr { get; set; }
        [Display(Name = "Customer Number")]
        public string Cust_No { get; set; }
        public int Cust_Creator_ID { get; set; }
        public int Cust_Approver_ID { get; set; }
        [Display(Name = "Creator Name")]
        public string Cust_Creator_Name { get; set; }
        [Display(Name = "Approver Name")]
        public string Cust_Approver_Name { get; set; }
        [Display(Name = "Customer Created Date")]
        public DateTime Cust_Created_Date { get; set; }
        [Display(Name = "Customer Last Updated")]
        public DateTime Cust_Last_Updated { get; set; }
        [Display(Name = "Customer Status")]
        public int Cust_Status_ID { get; set; }
        public string Cust_Status { get; set; }
        public bool Cust_isDeleted { get; set; }
        public bool Cust_isActive { get; set; }
    }
}
 