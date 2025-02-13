﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ConstantData
{
    public class NotificationMessageValues
    {
        public static readonly string commonUser = "You ";
        public static readonly string commonstr = " an entry/s in ";

        public static readonly Dictionary<int, string> TYTIONARY = new Dictionary<int, string>
        {
            {GlobalSystemValues.TYPE_DM,"Data Maintenance" },
            {GlobalSystemValues.TYPE_CV,"Check"},
            {GlobalSystemValues.TYPE_DDV,"Direct Deposit"},
            {GlobalSystemValues.TYPE_NC,"Non Cash"},
            {GlobalSystemValues.TYPE_PC,"Petty Cash"},
            {GlobalSystemValues.TYPE_SS,"Suspense Sundry"},
            {GlobalSystemValues.TYPE_LIQ,"Suspense Sundry (Liquidation)"},
            {GlobalSystemValues.TYPE_BUDGET,"G-Write"},
            {GlobalSystemValues.TYPE_CLOSING,"Closing"}
        };
        public static readonly Dictionary<int, string> DMTIONARY = new Dictionary<int, string>
        {
            {GlobalSystemValues.TYPE_DM_VEN,"Vendor" },
            {GlobalSystemValues.TYPE_DM_CHK,"Check"},
            {GlobalSystemValues.TYPE_DM_ACC,"Account"},
            {GlobalSystemValues.TYPE_DM_ACC_GRP,"Account Group"},
            {GlobalSystemValues.TYPE_DM_DEPT,"Department"},
            {GlobalSystemValues.TYPE_DM_VAT,"Value Added Tax"},
            {GlobalSystemValues.TYPE_DM_FBT,"Fringe Benefit Tax"},
            {GlobalSystemValues.TYPE_DM_TR,"Tax Rates"},
            {GlobalSystemValues.TYPE_DM_CURR,"Currency"},
            {GlobalSystemValues.TYPE_DM_REG_EMP,"Regular Employees"},
            {GlobalSystemValues.TYPE_DM_TEMP_EMP,"Temporary Employees"},
            {GlobalSystemValues.TYPE_DM_BCS,"BIR Cert Signatories"},
            {GlobalSystemValues.TYPE_DM_CUST,"Customer"}
        };
        public static readonly Dictionary<int, string> action = new Dictionary<int, string>
        {
            {GlobalSystemValues.STATUS_NEW,"created" },
            {GlobalSystemValues.STATUS_EDIT,"updated"},
            {GlobalSystemValues.STATUS_DELETE,"deleted"},
            {GlobalSystemValues.STATUS_VERIFIED,"verified" },
            {GlobalSystemValues.STATUS_APPROVED,"approved"},
            {GlobalSystemValues.STATUS_REJECTED,"rejected"},
            {GlobalSystemValues.STATUS_REVERSED,"reversed"}
        };
    }
}
