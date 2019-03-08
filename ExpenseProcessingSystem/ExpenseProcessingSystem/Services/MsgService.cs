using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services
{
    public class MsgService
    {
        private readonly SystemMessageContext _msgContext;
        public MsgService()
        {
            _msgContext = new SystemMessageContext();
        }

        public string GetMessage(string msgCode, string rplcStr1 = "", string rplcStr2 = "", string rplcStr3 = "", string rplcStr4 = "", string rplcStr5 = "")
        {
            SystemMessageModel sysMsg = _msgContext.SystemMessageModels.Find(msgCode);
            if (sysMsg != null)
                return sysMsg.Msg_Content.Replace("%1", rplcStr1).Replace("%2", rplcStr1).Replace("%3", rplcStr1).Replace("%4", rplcStr1).Replace("%5", rplcStr1);
            else
                return String.Empty;
        }
    }
}
