using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseProcessingSystem.Controllers
{
    public class ModalController : Controller
    {
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
            List<DMPayeeViewModel> vmList = PopulateDMPayee();
            List<DMPayeeViewModel> tempList = new List<DMPayeeViewModel>();
            foreach (DMPayeeViewModel vm in vmList)
            {
                foreach (string s in IdsArr)
                {
                    if (vm.Payee_ID == int.Parse(s))
                    {
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
            List<DMDeptViewModel> vmList = PopulateDMDept();
            List<DMDeptViewModel> tempList = new List<DMDeptViewModel>();
            foreach (DMDeptViewModel vm in vmList)
            {
                foreach (string s in IdsArr)
                {
                    if (vm.Dept_ID == int.Parse(s))
                    {
                        tempList.Add(vm);
                    }
                }
            }
            return View(tempList);
        }

        //TEMP, no database yet
        //-----
        //public List<DMPayeeViewModel> GetAllIds(string[] IdsArr)
        //{
        //    List<DMPayeeViewModel> vmList = PopulateDMPayee();
        //}
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