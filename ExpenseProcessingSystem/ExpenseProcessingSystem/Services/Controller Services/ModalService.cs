using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Models.Pending;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.NewRecord;
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
        //-----------------------ADMIN-------------------------
        // [DMPayeeModel ]
        public List<DMPayeeViewModel> approvePayee(string[] IdsArr)
        {
            List<DMPayeeModel_Pending> pendingList = _context.DMPayee_Pending.Where(x => IdsArr.Contains(x.Pending_Payee_MasterID.ToString())).Distinct().ToList();
            List<DMPayeeViewModel> tempList = new List<DMPayeeViewModel>();
            foreach (DMPayeeModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Payee_MasterID == int.Parse(s))
                    {
                        DMPayeeViewModel vm = new DMPayeeViewModel
                        {
                            Payee_MasterID = m.Pending_Payee_MasterID,
                            Payee_Name = m.Pending_Payee_Name,
                            Payee_TIN = m.Pending_Payee_TIN,
                            Payee_Address = m.Pending_Payee_Address,
                            Payee_Type = m.Pending_Payee_Type,
                            Payee_No = m.Pending_Payee_No,
                            Payee_Creator_ID = m.Pending_Payee_Creator_ID,
                            Payee_Approver_ID = m.Pending_Payee_Approver_ID.Equals(null) ? 0 : m.Pending_Payee_Approver_ID,
                            Payee_Created_Date = m.Pending_Payee_Filed_Date,
                            Payee_Last_Updated = m.Pending_Payee_Filed_Date,
                            Payee_Status = m.Pending_Payee_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMPayeeViewModel> rejectPayee(string[] IdsArr)
        {
            List<DMPayeeModel_Pending> pendingList = _context.DMPayee_Pending.Where(x => IdsArr.Contains(x.Pending_Payee_MasterID.ToString())).Distinct().ToList();
            List<DMPayeeViewModel> tempList = new List<DMPayeeViewModel>();
            foreach (DMPayeeModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Payee_MasterID == int.Parse(s))
                    {
                        DMPayeeViewModel vm = new DMPayeeViewModel
                        {
                            Payee_MasterID = m.Pending_Payee_MasterID,
                            Payee_Name = m.Pending_Payee_Name,
                            Payee_TIN = m.Pending_Payee_TIN,
                            Payee_Address = m.Pending_Payee_Address,
                            Payee_Type = m.Pending_Payee_Type,
                            Payee_No = m.Pending_Payee_No,
                            Payee_Creator_ID = m.Pending_Payee_Creator_ID,
                            Payee_Approver_ID = m.Pending_Payee_Approver_ID.Equals(null) ? 0 : m.Pending_Payee_Approver_ID,
                            Payee_Created_Date = m.Pending_Payee_Filed_Date,
                            Payee_Last_Updated = m.Pending_Payee_Filed_Date,
                            Payee_Status = m.Pending_Payee_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        //[ Department ]
        public List<DMDeptViewModel> approveDept(string[] IdsArr)
        {
            List<DMDeptModel_Pending> pendingList = _context.DMDept_Pending.Where(x => IdsArr.Contains(x.Pending_Dept_MasterID.ToString())).Distinct().ToList();
            List<DMDeptViewModel> tempList = new List<DMDeptViewModel>();
            foreach (DMDeptModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Dept_MasterID == int.Parse(s))
                    {
                        DMDeptViewModel vm = new DMDeptViewModel
                        {
                            Dept_MasterID = m.Pending_Dept_MasterID,
                            Dept_Name = m.Pending_Dept_Name,
                            Dept_Code = m.Pending_Dept_Code,
                            Dept_Creator_ID = m.Pending_Dept_Creator_ID,
                            Dept_Approver_ID = m.Pending_Dept_Approver_ID.Equals(null) ? 0 : m.Pending_Dept_Approver_ID,
                            Dept_Created_Date = m.Pending_Dept_Filed_Date,
                            Dept_Last_Updated = m.Pending_Dept_Filed_Date,
                            Dept_Status = m.Pending_Dept_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMDeptViewModel> rejectDept(string[] IdsArr)
        {
            List<DMDeptModel_Pending> pendingList = _context.DMDept_Pending.Where(x => IdsArr.Contains(x.Pending_Dept_MasterID.ToString())).Distinct().ToList();
            List<DMDeptViewModel> tempList = new List<DMDeptViewModel>();
            foreach (DMDeptModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Dept_MasterID == int.Parse(s))
                    {
                        DMDeptViewModel vm = new DMDeptViewModel
                        {
                            Dept_MasterID = m.Pending_Dept_MasterID,
                            Dept_Name = m.Pending_Dept_Name,
                            Dept_Code = m.Pending_Dept_Code,
                            Dept_Creator_ID = m.Pending_Dept_Creator_ID,
                            Dept_Approver_ID = m.Pending_Dept_Approver_ID.Equals(null) ? 0 : m.Pending_Dept_Approver_ID,
                            Dept_Created_Date = m.Pending_Dept_Filed_Date,
                            Dept_Last_Updated = m.Pending_Dept_Filed_Date,
                            Dept_Status = m.Pending_Dept_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Check ]
        public List<DMCheckViewModel> approveCheck(string[] IdsArr)
        {
            List<DMCheckModel_Pending> pendingList = _context.DMCheck_Pending.Where(x => IdsArr.Contains(x.Pending_Check_MasterID.ToString())).Distinct().ToList();
            List<DMCheckViewModel> tempList = new List<DMCheckViewModel>();
            foreach (DMCheckModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Check_MasterID == int.Parse(s))
                    {
                        DMCheckViewModel vm = new DMCheckViewModel
                        {
                            Check_MasterID = m.Pending_Check_MasterID,
                            Check_Input_Date = m.Pending_Check_Input_Date,
                            Check_Series_From = m.Pending_Check_Series_From,
                            Check_Series_To = m.Pending_Check_Series_To,
                            Check_Name = m.Pending_Check_Name,
                            Check_Type = m.Pending_Check_Type,
                            Check_Creator_ID = m.Pending_Check_Creator_ID,
                            Check_Approver_ID = m.Pending_Check_Approver_ID.Equals(null) ? 0 : m.Pending_Check_Approver_ID,
                            Check_Created_Date = m.Pending_Check_Filed_Date,
                            Check_Last_Updated = m.Pending_Check_Filed_Date,
                            Check_Status = m.Pending_Check_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCheckViewModel> rejectCheck(string[] IdsArr)
        {
            List<DMCheckModel_Pending> pendingList = _context.DMCheck_Pending.Where(x => IdsArr.Contains(x.Pending_Check_MasterID.ToString())).Distinct().ToList();
            List<DMCheckViewModel> tempList = new List<DMCheckViewModel>();
            foreach (DMCheckModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Check_MasterID == int.Parse(s))
                    {
                        DMCheckViewModel vm = new DMCheckViewModel
                        {
                            Check_MasterID = m.Pending_Check_MasterID,
                            Check_Input_Date = m.Pending_Check_Input_Date,
                            Check_Series_From = m.Pending_Check_Series_From,
                            Check_Series_To = m.Pending_Check_Series_To,
                            Check_Name = m.Pending_Check_Name,
                            Check_Type = m.Pending_Check_Type,
                            Check_Creator_ID = m.Pending_Check_Creator_ID,
                            Check_Approver_ID = m.Pending_Check_Approver_ID.Equals(null) ? 0 : m.Pending_Check_Approver_ID,
                            Check_Created_Date = m.Pending_Check_Filed_Date,
                            Check_Last_Updated = m.Pending_Check_Filed_Date,
                            Check_Status = m.Pending_Check_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Account ]
        public List<DMAccountViewModel> approveAccount(string[] IdsArr)
        {
            List<DMAccountModel_Pending> pendingList = _context.DMAccount_Pending.Where(x => IdsArr.Contains(x.Pending_Account_MasterID.ToString())).Distinct().ToList();
            List<DMAccountViewModel> tempList = new List<DMAccountViewModel>();
            foreach (DMAccountModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Account_MasterID == int.Parse(s))
                    {
                        DMAccountViewModel vm = new DMAccountViewModel
                        {
                            Account_MasterID = m.Pending_Account_MasterID,
                            Account_Name = m.Pending_Account_Name,
                            Account_Code = m.Pending_Account_Code,
                            Account_Cust = m.Pending_Account_Cust,
                            Account_Div = m.Pending_Account_Div,
                            Account_Fund = m.Pending_Account_Fund,
                            Account_No = m.Pending_Account_No,
                            Account_Creator_ID = m.Pending_Account_Creator_ID,
                            Account_Approver_ID = m.Pending_Account_Approver_ID.Equals(null) ? 0 : m.Pending_Account_Approver_ID,
                            Account_Created_Date = m.Pending_Account_Filed_Date,
                            Account_Last_Updated = m.Pending_Account_Filed_Date,
                            Account_Status = m.Pending_Account_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMAccountViewModel> rejectAccount(string[] IdsArr)
        {
            List<DMAccountModel_Pending> pendingList = _context.DMAccount_Pending.Where(x => IdsArr.Contains(x.Pending_Account_MasterID.ToString())).Distinct().ToList();
            List<DMAccountViewModel> tempList = new List<DMAccountViewModel>();
            foreach (DMAccountModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Account_MasterID == int.Parse(s))
                    {
                        DMAccountViewModel vm = new DMAccountViewModel
                        {
                            Account_MasterID = m.Pending_Account_MasterID,
                            Account_Name = m.Pending_Account_Name,
                            Account_Code = m.Pending_Account_Code,
                            Account_Cust = m.Pending_Account_Cust,
                            Account_Div = m.Pending_Account_Div,
                            Account_Fund = m.Pending_Account_Fund,
                            Account_No = m.Pending_Account_No,
                            Account_Creator_ID = m.Pending_Account_Creator_ID,
                            Account_Approver_ID = m.Pending_Account_Approver_ID.Equals(null) ? 0 : m.Pending_Account_Approver_ID,
                            Account_Created_Date = m.Pending_Account_Filed_Date,
                            Account_Last_Updated = m.Pending_Account_Filed_Date,
                            Account_Status = m.Pending_Account_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        // [DMVATModel ]
        public List<DMVATViewModel> approveVAT(string[] IdsArr)
        {
            List<DMVATModel_Pending> pendingList = _context.DMVAT_Pending.Where(x => IdsArr.Contains(x.Pending_VAT_MasterID.ToString())).Distinct().ToList();
            List<DMVATViewModel> tempList = new List<DMVATViewModel>();
            foreach (DMVATModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_VAT_MasterID == int.Parse(s))
                    {
                        DMVATViewModel vm = new DMVATViewModel
                        {
                            VAT_MasterID = m.Pending_VAT_MasterID,
                            VAT_Name = m.Pending_VAT_Name,
                            VAT_Rate = m.Pending_VAT_Rate,
                            VAT_Creator_ID = m.Pending_VAT_Creator_ID,
                            VAT_Approver_ID = m.Pending_VAT_Approver_ID.Equals(null) ? 0 : m.Pending_VAT_Approver_ID,
                            VAT_Created_Date = m.Pending_VAT_Filed_Date,
                            VAT_Last_Updated = m.Pending_VAT_Filed_Date,
                            VAT_Status = m.Pending_VAT_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMVATViewModel> rejectVAT(string[] IdsArr)
        {
            List<DMVATModel_Pending> pendingList = _context.DMVAT_Pending.Where(x => IdsArr.Contains(x.Pending_VAT_MasterID.ToString())).Distinct().ToList();
            List<DMVATViewModel> tempList = new List<DMVATViewModel>();
            foreach (DMVATModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_VAT_MasterID == int.Parse(s))
                    {
                        DMVATViewModel vm = new DMVATViewModel
                        {
                            VAT_MasterID = m.Pending_VAT_MasterID,
                            VAT_Name = m.Pending_VAT_Name,
                            VAT_Rate = m.Pending_VAT_Rate,
                            VAT_Creator_ID = m.Pending_VAT_Creator_ID,
                            VAT_Approver_ID = m.Pending_VAT_Approver_ID.Equals(null) ? 0 : m.Pending_VAT_Approver_ID,
                            VAT_Created_Date = m.Pending_VAT_Filed_Date,
                            VAT_Last_Updated = m.Pending_VAT_Filed_Date,
                            VAT_Status = m.Pending_VAT_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        // [DMFBTModel ]
        public List<DMFBTViewModel> approveFBT(string[] IdsArr)
        {
            List<DMFBTModel_Pending> pendingList = _context.DMFBT_Pending.Where(x => IdsArr.Contains(x.Pending_FBT_MasterID.ToString())).Distinct().ToList();
            List<DMFBTViewModel> tempList = new List<DMFBTViewModel>();
            foreach (DMFBTModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_FBT_MasterID == int.Parse(s))
                    {
                        DMFBTViewModel vm = new DMFBTViewModel
                        {
                            FBT_MasterID = m.Pending_FBT_MasterID,
                            FBT_Name = m.Pending_FBT_Name,
                            FBT_Account = m.Pending_FBT_Account,
                            FBT_Formula = m.Pending_FBT_Formula,
                            FBT_Tax_Rate = m.Pending_FBT_Tax_Rate,
                            FBT_Creator_ID = m.Pending_FBT_Creator_ID,
                            FBT_Approver_ID = m.Pending_FBT_Approver_ID.Equals(null) ? 0 : m.Pending_FBT_Approver_ID,
                            FBT_Created_Date = m.Pending_FBT_Filed_Date,
                            FBT_Last_Updated = m.Pending_FBT_Filed_Date,
                            FBT_Status = m.Pending_FBT_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMFBTViewModel> rejectFBT(string[] IdsArr)
        {
            List<DMFBTModel_Pending> pendingList = _context.DMFBT_Pending.Where(x => IdsArr.Contains(x.Pending_FBT_MasterID.ToString())).Distinct().ToList();
            List<DMFBTViewModel> tempList = new List<DMFBTViewModel>();
            foreach (DMFBTModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_FBT_MasterID == int.Parse(s))
                    {
                        DMFBTViewModel vm = new DMFBTViewModel
                        {
                            FBT_MasterID = m.Pending_FBT_MasterID,
                            FBT_Name = m.Pending_FBT_Name,
                            FBT_Account = m.Pending_FBT_Account,
                            FBT_Formula = m.Pending_FBT_Formula,
                            FBT_Tax_Rate = m.Pending_FBT_Tax_Rate,
                            FBT_Creator_ID = m.Pending_FBT_Creator_ID,
                            FBT_Approver_ID = m.Pending_FBT_Approver_ID.Equals(null) ? 0 : m.Pending_FBT_Approver_ID,
                            FBT_Created_Date = m.Pending_FBT_Filed_Date,
                            FBT_Last_Updated = m.Pending_FBT_Filed_Date,
                            FBT_Status = m.Pending_FBT_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        // [DMEWTModel ]
        public List<DMEWTViewModel> approveEWT(string[] IdsArr)
        {
            List<DMEWTModel_Pending> pendingList = _context.DMEWT_Pending.Where(x => IdsArr.Contains(x.Pending_EWT_MasterID.ToString())).Distinct().ToList();
            List<DMEWTViewModel> tempList = new List<DMEWTViewModel>();
            foreach (DMEWTModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_EWT_MasterID == int.Parse(s))
                    {
                        DMEWTViewModel vm = new DMEWTViewModel
                        {
                            EWT_MasterID = m.Pending_EWT_MasterID,
                            EWT_Nature = m.Pending_EWT_Nature,
                            EWT_Tax_Rate = m.Pending_EWT_Tax_Rate,
                            EWT_ATC = m.Pending_EWT_ATC,
                            EWT_Tax_Rate_Desc = m.Pending_EWT_Tax_Rate_Desc,
                            EWT_Creator_ID = m.Pending_EWT_Creator_ID,
                            EWT_Approver_ID = m.Pending_EWT_Approver_ID.Equals(null) ? 0 : m.Pending_EWT_Approver_ID,
                            EWT_Created_Date = m.Pending_EWT_Filed_Date,
                            EWT_Last_Updated = m.Pending_EWT_Filed_Date,
                            EWT_Status = m.Pending_EWT_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMEWTViewModel> rejectEWT(string[] IdsArr)
        {
            List<DMEWTModel_Pending> pendingList = _context.DMEWT_Pending.Where(x => IdsArr.Contains(x.Pending_EWT_MasterID.ToString())).Distinct().ToList();
            List<DMEWTViewModel> tempList = new List<DMEWTViewModel>();
            foreach (DMEWTModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_EWT_MasterID == int.Parse(s))
                    {
                        DMEWTViewModel vm = new DMEWTViewModel
                        {
                            EWT_MasterID = m.Pending_EWT_MasterID,
                            EWT_Nature = m.Pending_EWT_Nature,
                            EWT_Tax_Rate = m.Pending_EWT_Tax_Rate,
                            EWT_ATC = m.Pending_EWT_ATC,
                            EWT_Tax_Rate_Desc = m.Pending_EWT_Tax_Rate_Desc,
                            EWT_Creator_ID = m.Pending_EWT_Creator_ID,
                            EWT_Approver_ID = m.Pending_EWT_Approver_ID.Equals(null) ? 0 : m.Pending_EWT_Approver_ID,
                            EWT_Created_Date = m.Pending_EWT_Filed_Date,
                            EWT_Last_Updated = m.Pending_EWT_Filed_Date,
                            EWT_Status = m.Pending_EWT_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        // [DMCurrModel ]
        public List<DMCurrencyViewModel> approveCurr(string[] IdsArr)
        {
            List<DMCurrencyModel_Pending> pendingList = _context.DMCurrency_Pending.Where(x => IdsArr.Contains(x.Pending_Curr_MasterID.ToString())).Distinct().ToList();
            List<DMCurrencyViewModel> tempList = new List<DMCurrencyViewModel>();
            foreach (DMCurrencyModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Curr_MasterID == int.Parse(s))
                    {
                        DMCurrencyViewModel vm = new DMCurrencyViewModel
                        {
                            Curr_MasterID = m.Pending_Curr_MasterID,
                            Curr_Name = m.Pending_Curr_Name,
                            Curr_CCY_Code = m.Pending_Curr_CCY_Code,
                            Curr_Creator_ID = m.Pending_Curr_Creator_ID,
                            Curr_Approver_ID = m.Pending_Curr_Approver_ID.Equals(null) ? 0 : m.Pending_Curr_Approver_ID,
                            Curr_Created_Date = m.Pending_Curr_Filed_Date,
                            Curr_Last_Updated = m.Pending_Curr_Filed_Date,
                            Curr_Status = m.Pending_Curr_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCurrencyViewModel> rejectCurr(string[] IdsArr)
        {
            List<DMCurrencyModel_Pending> pendingList = _context.DMCurrency_Pending.Where(x => IdsArr.Contains(x.Pending_Curr_MasterID.ToString())).Distinct().ToList();
            List<DMCurrencyViewModel> tempList = new List<DMCurrencyViewModel>();
            foreach (DMCurrencyModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Curr_MasterID == int.Parse(s))
                    {
                        DMCurrencyViewModel vm = new DMCurrencyViewModel
                        {
                            Curr_MasterID = m.Pending_Curr_MasterID,
                            Curr_Name = m.Pending_Curr_Name,
                            Curr_CCY_Code = m.Pending_Curr_CCY_Code,
                            Curr_Creator_ID = m.Pending_Curr_Creator_ID,
                            Curr_Approver_ID = m.Pending_Curr_Approver_ID.Equals(null) ? 0 : m.Pending_Curr_Approver_ID,
                            Curr_Created_Date = m.Pending_Curr_Filed_Date,
                            Curr_Last_Updated = m.Pending_Curr_Filed_Date,
                            Curr_Status = m.Pending_Curr_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //------------------------PENDING-----------------------------
        // [Add New Pending Entries]
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
        public NewDeptListViewModel addDept()
        {
            NewDeptListViewModel mod = new NewDeptListViewModel();
            List<NewDeptViewModel> vmList = new List<NewDeptViewModel>();
            NewDeptViewModel vm = new NewDeptViewModel();
            vmList.Add(vm);
            mod.NewDeptVM = vmList;
            return mod;
        }
        public NewCheckListViewModel addCheck()
        {
            NewCheckListViewModel mod = new NewCheckListViewModel();
            List<NewCheckViewModel> vmList = new List<NewCheckViewModel>();
            NewCheckViewModel vm = new NewCheckViewModel
            {
                Check_Input_Date = DateTime.Now
            };
            vmList.Add(vm);
            mod.NewCheckVM = vmList;
            return mod;
        }
        public NewAccountListViewModel addAccount()
        {
            NewAccountListViewModel mod = new NewAccountListViewModel();
            List<NewAccountViewModel> vmList = new List<NewAccountViewModel>();
            NewAccountViewModel vm = new NewAccountViewModel();
            vmList.Add(vm);
            mod.NewAccountVM = vmList;
            return mod;
        }
        public NewVATListViewModel addVAT()
        {
            NewVATListViewModel mod = new NewVATListViewModel();
            List<NewVATViewModel> vmList = new List<NewVATViewModel>();
            NewVATViewModel vm = new NewVATViewModel();
            vmList.Add(vm);
            mod.NewVATVM = vmList;
            return mod;
        }
        public NewFBTListViewModel addFBT()
        {
            NewFBTListViewModel mod = new NewFBTListViewModel();
            List<NewFBTViewModel> vmList = new List<NewFBTViewModel>();
            NewFBTViewModel vm = new NewFBTViewModel
            {
                FBT_Tax_Rate = 0
            };
            vmList.Add(vm);
            mod.NewFBTVM = vmList;
            return mod;
        }
        public NewEWTListViewModel addEWT()
        {
            NewEWTListViewModel mod = new NewEWTListViewModel();
            List<NewEWTViewModel> vmList = new List<NewEWTViewModel>();
            NewEWTViewModel vm = new NewEWTViewModel
            {
                EWT_Tax_Rate = 0
            };
            vmList.Add(vm);
            mod.NewEWTVM = vmList;
            return mod;
        }
        public NewCurrListViewModel addCurr()
        {
            NewCurrListViewModel mod = new NewCurrListViewModel();
            List<NewCurrViewModel> vmList = new List<NewCurrViewModel>();
            NewCurrViewModel vm = new NewCurrViewModel();
            vmList.Add(vm);
            mod.NewCurrVM = vmList;
            return mod;
        }

        // [Update Pending Entries]
        public List<DMPayeeViewModel> editDeletePayee(string[] IdsArr)
        {
            List<DMPayeeModel> mList = _context.DMPayee.Where(x => IdsArr.Contains(x.Payee_MasterID.ToString()) 
                                       && x.Payee_isActive == true).Distinct().ToList();
            List<DMPayeeViewModel> tempList = new List<DMPayeeViewModel>();
            foreach (DMPayeeModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Payee_MasterID == int.Parse(s))
                    {
                        DMPayeeViewModel vm = new DMPayeeViewModel
                        {
                            Payee_MasterID = m.Payee_MasterID,
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
            List<DMDeptModel> mList = _context.DMDept.Where(x => IdsArr.Contains(x.Dept_MasterID.ToString())
                                        && x.Dept_isActive == true).Distinct().ToList();
            List<DMDeptViewModel> tempList = new List<DMDeptViewModel>();
            foreach (DMDeptModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Dept_MasterID == int.Parse(s))
                    {
                        DMDeptViewModel vm = new DMDeptViewModel
                        {
                            Dept_MasterID = m.Dept_MasterID,
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
        public List<DMCheckViewModel> editDeleteCheck(string[] IdsArr)
        {
            List<DMCheckModel> mList = _context.DMCheck.Where(x => IdsArr.Contains(x.Check_MasterID.ToString())
                                        && x.Check_isActive == true).Distinct().ToList();
            List<DMCheckViewModel> tempList = new List<DMCheckViewModel>();
            foreach (DMCheckModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Check_MasterID == int.Parse(s))
                    {
                        DMCheckViewModel vm = new DMCheckViewModel
                        {
                            Check_MasterID = m.Check_MasterID,
                            Check_Input_Date = m.Check_Input_Date,
                            Check_Series_From = m.Check_Series_From,
                            Check_Series_To = m.Check_Series_To,
                            Check_Name = m.Check_Name,
                            Check_Type = m.Check_Type,
                            Check_Creator_ID = m.Check_Creator_ID,
                            Check_Created_Date = m.Check_Created_Date,
                            Check_Last_Updated = DateTime.Now,
                            Check_Status = m.Check_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMAccountViewModel> editDeleteAccount(string[] IdsArr)
        {
            List<DMAccountModel> mList = _context.DMAccount.Where(x => IdsArr.Contains(x.Account_MasterID.ToString())
                                       && x.Account_isActive == true).Distinct().ToList();
            List<DMAccountViewModel> tempList = new List<DMAccountViewModel>();
            foreach (DMAccountModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Account_MasterID == int.Parse(s))
                    {
                        DMAccountViewModel vm = new DMAccountViewModel
                        {
                            Account_MasterID = m.Account_MasterID,
                            Account_Name = m.Account_Name,
                            Account_No = m.Account_No,
                            Account_Code = m.Account_Code,
                            Account_Cust = m.Account_Cust,
                            Account_Div = m.Account_Div,
                            Account_Fund = m.Account_Fund,
                            Account_Creator_ID = m.Account_Creator_ID,
                            Account_Created_Date = m.Account_Created_Date,
                            Account_Last_Updated = DateTime.Now,
                            Account_Status = m.Account_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMVATViewModel> editDeleteVAT(string[] IdsArr)
        {
            List<DMVATModel> mList = _context.DMVAT.Where(x => IdsArr.Contains(x.VAT_MasterID.ToString())
                                       && x.VAT_isActive == true).Distinct().ToList();
            List<DMVATViewModel> tempList = new List<DMVATViewModel>();
            foreach (DMVATModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.VAT_MasterID == int.Parse(s))
                    {
                        DMVATViewModel vm = new DMVATViewModel
                        {
                            VAT_MasterID = m.VAT_MasterID,
                            VAT_Name = m.VAT_Name,
                            VAT_Rate = m.VAT_Rate,
                            VAT_Creator_ID = m.VAT_Creator_ID,
                            VAT_Created_Date = m.VAT_Created_Date,
                            VAT_Last_Updated = DateTime.Now,
                            VAT_Status = m.VAT_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMFBTViewModel> editDeleteFBT(string[] IdsArr)
        {
            List<DMFBTModel> mList = _context.DMFBT.Where(x => IdsArr.Contains(x.FBT_MasterID.ToString())
                                       && x.FBT_isActive == true).Distinct().ToList();
            List<DMFBTViewModel> tempList = new List<DMFBTViewModel>();
            foreach (DMFBTModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.FBT_MasterID == int.Parse(s))
                    {
                        DMFBTViewModel vm = new DMFBTViewModel
                        {
                            FBT_MasterID = m.FBT_MasterID,
                            FBT_Name = m.FBT_Name,
                            FBT_Account = m.FBT_Account,
                            FBT_Formula = m.FBT_Formula,
                            FBT_Tax_Rate = m.FBT_Tax_Rate,
                            FBT_Creator_ID = m.FBT_Creator_ID,
                            FBT_Created_Date = m.FBT_Created_Date,
                            FBT_Last_Updated = DateTime.Now,
                            FBT_Status = m.FBT_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMEWTViewModel> editDeleteEWT(string[] IdsArr)
        {
            List<DMEWTModel> mList = _context.DMEWT.Where(x => IdsArr.Contains(x.EWT_MasterID.ToString())
                                       && x.EWT_isActive == true).Distinct().ToList();
            List<DMEWTViewModel> tempList = new List<DMEWTViewModel>();
            foreach (DMEWTModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.EWT_MasterID == int.Parse(s))
                    {
                        DMEWTViewModel vm = new DMEWTViewModel
                        {
                            EWT_MasterID = m.EWT_MasterID,
                            EWT_Nature = m.EWT_Nature,
                            EWT_Tax_Rate = m.EWT_Tax_Rate,
                            EWT_ATC = m.EWT_ATC,
                            EWT_Tax_Rate_Desc = m.EWT_Tax_Rate_Desc,
                            EWT_Creator_ID = m.EWT_Creator_ID,
                            EWT_Created_Date = m.EWT_Created_Date,
                            EWT_Last_Updated = DateTime.Now,
                            EWT_Status = m.EWT_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCurrencyViewModel> editDeleteCurr(string[] IdsArr)
        {
            List<DMCurrencyModel> mList = _context.DMCurrency.Where(x => IdsArr.Contains(x.Curr_MasterID.ToString())
                                       && x.Curr_isActive == true).Distinct().ToList();
            List<DMCurrencyViewModel> tempList = new List<DMCurrencyViewModel>();
            foreach (DMCurrencyModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Curr_MasterID == int.Parse(s))
                    {
                        DMCurrencyViewModel vm = new DMCurrencyViewModel
                        {
                            Curr_MasterID = m.Curr_MasterID,
                            Curr_Name = m.Curr_Name,
                            Curr_CCY_Code = m.Curr_CCY_Code,
                            Curr_Creator_ID = m.Curr_Creator_ID,
                            Curr_Created_Date = m.Curr_Created_Date,
                            Curr_Last_Updated = DateTime.Now,
                            Curr_Status = m.Curr_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
    }
}
