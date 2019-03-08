using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Controller_Services
{
    public class PartialService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public PartialService(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        //Populate
        public List<DMPayeeViewModel> populatePayee(string colName, string searchString)
        {
            List<DMPayeeModel> mList = _context.DMPayee.Where(x => x.Payee_isDeleted == false).ToList();
            //FOR FILTERING
            if (!String.IsNullOrEmpty(searchString))
            {
                string[] colArr = { "No", "Creator_ID", "Approver_ID" };
                if (colArr.Contains(colName))
                {
                    mList = _context.DMPayee
                                  .Where("Payee_" + colName + " = @0 AND Payee_isDeleted == @1", searchString, false)
                                  .Select(e => e).ToList();
                }
                else
                {
                    mList = _context.DMPayee
                                  .Where("Payee_" + colName + ".Contains(@0) AND Payee_isDeleted == @1", searchString, false)
                                  .Select(e => e).ToList();
                }

            }

            var creatorList = (from a in mList
                               join b in _context.Account on a.Payee_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new
                               { a.Payee_ID, CreatorName }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.Payee_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new
                              { a.Payee_ID, ApproverName }).ToList();

            List<DMPayeeViewModel> vmList = new List<DMPayeeViewModel>();
            foreach (DMPayeeModel m in mList)
            {
                var creator = creatorList.Where(a => a.Payee_ID == m.Payee_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Payee_ID == m.Payee_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMPayeeViewModel vm = new DMPayeeViewModel
                {
                    Payee_ID = m.Payee_ID,
                    Payee_Name = m.Payee_Name,
                    Payee_TIN = m.Payee_TIN,
                    Payee_Address = m.Payee_Address,
                    Payee_Type = m.Payee_Type,
                    Payee_No = m.Payee_No,
                    Payee_Creator_Name = creator ?? "N/A",
                    Payee_Approver_Name = approver ?? "",
                    Payee_Created_Date = m.Payee_Created_Date,
                    Payee_Last_Updated = m.Payee_Last_Updated,
                    Payee_Status = m.Payee_Status
                };
                vmList.Add(vm);
            }
            return vmList;
        }

        public List<DMDeptViewModel> populateDept(string colName, string searchString)
        {
            List<DMDeptModel> mList = _context.DMDept.Where(x => x.Dept_isDeleted == false).ToList();

            //FOR FILTERING
            if (!String.IsNullOrEmpty(searchString))
            {
                string[] colArr = { "Creator_ID", "Approver_ID" };
                if (colArr.Contains(colName))
                {
                    mList = _context.DMDept
                                  .Where("Dept_" + colName + " = @0 AND Dept_isDeleted == @1", searchString, false)
                                  .Select(e => e).ToList();
                }
                else
                {
                    mList = _context.DMDept
                                  .Where("Dept_" + colName + ".Contains(@0) AND Dept_isDeleted == @1", searchString, false)
                                  .Select(e => e).ToList();
                }

            }
            var creatorList = (from a in mList
                               join b in _context.Account on a.Dept_Creator_ID equals b.Acc_UserID
                               let CreatorName = b.Acc_LName + ", " + b.Acc_FName
                               select new
                               {
                                   a.Dept_ID,
                                   CreatorName
                               }).ToList();
            var apprvrList = (from a in mList
                              join c in _context.Account on a.Dept_Approver_ID equals c.Acc_UserID
                              let ApproverName = c.Acc_LName + ", " + c.Acc_FName
                              select new
                              {
                                  a.Dept_ID,
                                  ApproverName
                              }).ToList();

            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            foreach (DMDeptModel m in mList)
            {
                var creator = creatorList.Where(a => a.Dept_ID == m.Dept_ID).Select(a => a.CreatorName).FirstOrDefault();
                var approver = apprvrList.Where(a => a.Dept_ID == m.Dept_ID).Select(a => a.ApproverName).FirstOrDefault();
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_ID = m.Dept_ID,
                    Dept_Name = m.Dept_Name,
                    Dept_Code = m.Dept_Code,
                    Dept_Creator_Name = creator ?? "N/A",
                    Dept_Approver_Name = approver ?? "",
                    Dept_Created_Date = m.Dept_Created_Date,
                    Dept_Last_Updated = m.Dept_Last_Updated,
                    Dept_Status = m.Dept_Status
                };
                vmList.Add(vm);
            }
            return vmList;
        }
    }
}
