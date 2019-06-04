﻿using ExpenseProcessingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class BMViewModel
    {
        public int BM_Budget_ID { get; set; }

        public string BM_Acc_Group_Name { get; set; }
        public string BM_Acc_Name { get; set; }
        public string BM_ISPS_Acc_Name { get; set; }
        public string BM_GBase_Code { get; set; }
        public string BM_Acc_Num { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double BM_Budget_Amount { get; set; }
        public DateTime BM_Date_Registered { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double BM_Budget_Current { get; set; }

        public IEnumerable<DMAccountModel> BM_AccountList { get; set; }

        public string BM_Creator_Name { get; set; }

        public int BM_Acc_Master_ID { get; set; }
        public string BM_Acc_Code { get; set; }
        public bool BM_Acc_IsActive { get; set; }
        public string BM_Acc_Group{ get; set; }
        public string BM_Acc_GBase { get; set; }
        public string BM_Budget_Approver_ID { get; set; }
        public string BM_Budget_Approver_Name { get; set; }
        public byte BM_Budget_Status { get; set; }
        public bool BM_Budget_isDeleted { get; set; }
        public DateTime BM_Last_Budget_Approved { get; set; }
    }
}
