using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseProcessingSystem.Controllers
{
    public class ModalController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public ModalController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        //Entry_DDV
        public IActionResult ReversalEntryModal()
        {
            return View();
        }
        //BM
        public IActionResult BudgetAdjustmentModal()
        {
            return View();
        }
        //DM Add Payee
        public IActionResult DMAddPayee()
        {
            return View();
        }
        //DM Edit Payee
        public IActionResult DMEditPayee(string[] IdsArr)
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
            return View(tempList);
        }
        public IActionResult DMDeletePayee(string[] IdsArr)
        {
            List<DMPayeeModel> mList = _context.DMPayee.Where(x=> IdsArr.Contains(x.Payee_ID.ToString())).ToList();
            List<DMPayeeViewModel> tempList = new List<DMPayeeViewModel>();
            foreach (DMPayeeModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Payee_ID == int.Parse(s))
                    {
                        DMPayeeViewModel vm = new DMPayeeViewModel {
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
            return View(tempList);
        }
        //DM Add Dept
        public IActionResult DMAddDept()
        {
            return View();
        }
        //DM Edit Payee
        public IActionResult DMEditDept(string[] IdsArr)
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
            return View(tempList);
        }

        public IActionResult DMDeleteDept(string[] IdsArr)
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
            return View(tempList);
        }
        public List<DMPayeeViewModel> PopulateDMPayee()
        {
            List<DMPayeeViewModel> vmList = new List<DMPayeeViewModel>();
            for (var i = 1; i <= 50; i++)
            {
                DMPayeeViewModel vm = new DMPayeeViewModel
                {
                    Payee_ID = i,
                    Payee_Name = "Payee_" + i,
                    Payee_TIN = "TIN_" + i + 5000,
                    Payee_Address = "Address_" + i + 5000,
                    Payee_Type = "Type_" + i + 5000,
                    Payee_No = i + 5000,
                    Payee_Creator_ID = i + 100,
                    Payee_Approver_ID = i + 200,
                    Payee_Last_Updated = DateTime.Parse("1/12/2017", CultureInfo.GetCultureInfo("en-GB"))
                            .Add(DateTime.Now.TimeOfDay),
                    Payee_Status = "For Approval"
                };
                vmList.Add(vm);
            }
            return vmList;
        }
        public List<DMDeptViewModel> PopulateDMDept()
        {
            List<DMDeptViewModel> vmList = new List<DMDeptViewModel>();
            for (var i = 1; i <= 50; i++)
            {
                DMDeptViewModel vm = new DMDeptViewModel
                {
                    Dept_ID = i,
                    Dept_Name = "Dept_" + i,
                    Dept_Code = "Code" + i + 5000,
                    Dept_Creator_ID = i + 100,
                    Dept_Approver_ID = i + 200,
                    Dept_Last_Updated = DateTime.Parse("1/12/2017", CultureInfo.GetCultureInfo("en-GB"))
                            .Add(DateTime.Now.TimeOfDay),
                    Dept_Status = "For Approval"
                };
                vmList.Add(vm);
            }
            return vmList;
        }
    }
}