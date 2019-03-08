using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Controller_Services
{
    public class ModalService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public ModalService(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public NewPayeeListViewModel addPayee()
        {
            NewPayeeListViewModel mod = new NewPayeeListViewModel();
            List<NewPayeeViewModel> vmList = new List<NewPayeeViewModel>();
            NewPayeeViewModel vm = new NewPayeeViewModel
            {
                Payee_No = 0
            };
            vmList.Add(vm);
            mod.NewPayeeVM = vmList;
            return mod;
        }

        public List<DMPayeeViewModel> editDeletePayee(string[] IdsArr)
        {
            List<DMPayeeModel> mList = _context.DMPayee.Where(x => IdsArr.Contains(x.Payee_ID.ToString())).ToList();
            List<DMPayeeViewModel> tempList = new List<DMPayeeViewModel>();
            foreach (DMPayeeModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Payee_ID == int.Parse(s))
                    {
                        DMPayeeViewModel vm = new DMPayeeViewModel
                        {
                            Payee_ID = m.Payee_ID,
                            Payee_Name = m.Payee_Name,
                            Payee_TIN = m.Payee_TIN,
                            Payee_Address = m.Payee_Address,
                            Payee_Type = m.Payee_Type,
                            Payee_No = m.Payee_No,
                            Payee_Creator_ID = m.Payee_Creator_ID,
                            Payee_Created_Date = m.Payee_Created_Date,
                            Payee_Last_Updated = DateTime.Now,
                            Payee_Status = m.Payee_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMDeptViewModel> editDeleteDept(string[] IdsArr)
        {
            List<DMDeptModel> mList = _context.DMDept.Where(x => IdsArr.Contains(x.Dept_ID.ToString())).ToList();
            List<DMDeptViewModel> tempList = new List<DMDeptViewModel>();
            foreach (DMDeptModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Dept_ID == int.Parse(s))
                    {
                        DMDeptViewModel vm = new DMDeptViewModel
                        {
                            Dept_ID = m.Dept_ID,
                            Dept_Name = m.Dept_Name,
                            Dept_Code = m.Dept_Code,
                            Dept_Creator_ID = m.Dept_Creator_ID,
                            Dept_Created_Date = m.Dept_Created_Date,
                            Dept_Last_Updated = DateTime.Now,
                            Dept_Status = m.Dept_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
    }
}
