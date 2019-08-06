using System;
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
        };
        public static readonly Dictionary<int, string> action = new Dictionary<int, string>
        {
            {GlobalSystemValues.STATUS_NEW,"created" },
            {GlobalSystemValues.STATUS_EDIT,"updated"},
            {GlobalSystemValues.STATUS_DELETE,"deleted"},
            {GlobalSystemValues.STATUS_VERIFIED,"verified" },
            {GlobalSystemValues.STATUS_APPROVED,"approved"},
            {GlobalSystemValues.STATUS_REJECTED,"rejected"}
        };
    }
}
