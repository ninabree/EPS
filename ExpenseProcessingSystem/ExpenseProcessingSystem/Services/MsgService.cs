using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services
{
    public class MsgService
    {
        private List<SystemMessageModel> msgList;
        public MsgService()
        {
            using (EPSDbContext db = new EPSDbContext())
            {
                msgList = db.SystemMessageModels.OrderBy(a => a.Msg_Code).ToList();
            }
        }

        public static string GetMessage(string msgCode, string rplcStr1 = "", string rplcStr2 = "", string rplcStr3 = "", string rplcStr4 = "", string rplcStr5 = "")
        {
            using (var db = new EPSDbContext())
            {
                SystemMessageModel sysMsg = db.SystemMessageModels.Find(msgCode);
                if (sysMsg != null)
                    return sysMsg.Msg_Content.Replace("%1", rplcStr1).Replace("%2", rplcStr1).Replace("%3", rplcStr1).Replace("%4", rplcStr1).Replace("%5", rplcStr1);
                else
                    return String.Empty;
            }
        }
    }
}
