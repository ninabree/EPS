using ExpenseProcessingSystem.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Controller_Services
{
    public class NotifService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        private ModelStateDictionary _modelState;
        public NotifService(IHttpContextAccessor httpContextAccessor, EPSDbContext context, ModelStateDictionary modelState)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _modelState = modelState;
        }
        //Copied from TSSite Request OT
        //public void AddPendingPayee_Notif(int userId)
        //{ 
        //    var userInfo = _context.Account.Where(x => x.Acc_UserID == userId).FirstOrDefault();
        //    var tlInfo = _context.Account.Where(x => x.Acc_Role == "approver" || x.Acc_Role == "verifier").FirstOrDefault();
        //    var otInfo = _context.OT.Where(x => x.EmpNo == int.Parse(userEmpNo) && x.Status == "Pending").Count();
        //    List<OTReqViewModel> otReqList = GetRequestOTs(int.Parse(userId), int.Parse(userEmpNo));

        //    int[] empNoArr = { userInfo.Acc_UserID, tlInfo.Acc_UserID, userInfo.Acc_UserID, tlInfo.Acc_UserID };
        //    int[] listCount = { otReqList.Count - 1, otReqList.Count - 1, otInfo + 1, otInfo + 1 };
        //    string[] messages = { "request/s for", "request/s for", "pending", "pending" };
        //    string[] stats = { "Request OT", "Request OT TL", "Pending OT", "Pending OT TL" };
        //    string[] linkaddress = { "OTLeaveRequest/Timesheet/request/ot", "OTLeaveManage/TL/request/ot", "OTLeaveRequest/Timesheet/pending/ot", "OTLeaveManage/TL/pending/ot" };
        //    string[] user = { "You", userInfo.Acc_FName + "", "You", userInfo.Acc_FName + "" };

        //    var i = 0;
        //    foreach (string s in stats)
        //    {
        //        var notifInfo = _context.Notif.Where(x => x.Notif_User_ID == empNoArr[i] && x.Type == s).FirstOrDefault();
        //        if (notifInfo != null)
        //        {
        //            if (listCount[i] != 0)
        //            {
        //                notifInfo.Status = 0;
        //                notifInfo.Message = user[i] + " have (" + (listCount[i]) + ") " + (messages[i]) + " OT Application/s.";
        //                //_context.Entry(notifInfo).State = EntityState.Modified;
        //               // _context.SaveChanges();
        //            }
        //            else
        //            {
        //                //_context.Notif.Remove(notifInfo);
        //                //_context.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            if (listCount[i] != 0)
        //            {
        //                NotifModel nm = new NotifModel
        //                {
        //                    EmpNo = empNoArr[i],
        //                    Message = user[i] + " have (" + (listCount[i]) + ") " + (messages[i]) + " OT Application/s.",
        //                    LinkAddress = linkaddress[i],
        //                    Status = 0,
        //                    Type = s
        //                };

        //                if (ModelState.IsValid)
        //                {
        //                    //_context.Notif.Add(nm);
        //                    //_context.SaveChanges();
        //                }
        //            }
        //        }
        //        i++;
        //    }
        //}
    }
}
