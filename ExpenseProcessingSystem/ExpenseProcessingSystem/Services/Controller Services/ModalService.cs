using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Models.Pending;
using ExpenseProcessingSystem.ViewModels;
using ExpenseProcessingSystem.ViewModels.NewRecord;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        // [DMVendor ]
        public List<DMVendorViewModel> approveVendor(string[] IdsArr)
        {
            List<DMVendorModel_Pending> pendingList = _context.DMVendor_Pending.Where(x => IdsArr.Contains(x.Pending_Vendor_MasterID.ToString())).Distinct().ToList();
            List<DMVendorViewModel> tempList = new List<DMVendorViewModel>();
            foreach (DMVendorModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Vendor_MasterID == int.Parse(s))
                    {
                        DMVendorViewModel vm = new DMVendorViewModel
                        {
                            Vendor_MasterID = m.Pending_Vendor_MasterID,
                            Vendor_Name = m.Pending_Vendor_Name,
                            Vendor_TIN = m.Pending_Vendor_TIN,
                            Vendor_Address = m.Pending_Vendor_Address,
                            Vendor_Creator_ID = m.Pending_Vendor_Creator_ID,
                            Vendor_Approver_ID = m.Pending_Vendor_Approver_ID.Equals(null) ? 0 : m.Pending_Vendor_Approver_ID,
                            Vendor_Created_Date = m.Pending_Vendor_Filed_Date,
                            Vendor_Last_Updated = m.Pending_Vendor_Filed_Date,
                            Vendor_Status = m.Pending_Vendor_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMVendorViewModel> rejectVendor(string[] IdsArr)
        {
            List<DMVendorModel_Pending> pendingList = _context.DMVendor_Pending.Where(x => IdsArr.Contains(x.Pending_Vendor_MasterID.ToString())).Distinct().ToList();
            List<DMVendorViewModel> tempList = new List<DMVendorViewModel>();
            foreach (DMVendorModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Vendor_MasterID == int.Parse(s))
                    {
                        DMVendorViewModel vm = new DMVendorViewModel
                        {
                            Vendor_MasterID = m.Pending_Vendor_MasterID,
                            Vendor_Name = m.Pending_Vendor_Name,
                            Vendor_TIN = m.Pending_Vendor_TIN,
                            Vendor_Address = m.Pending_Vendor_Address,
                            Vendor_Creator_ID = m.Pending_Vendor_Creator_ID,
                            Vendor_Approver_ID = m.Pending_Vendor_Approver_ID.Equals(null) ? 0 : m.Pending_Vendor_Approver_ID,
                            Vendor_Created_Date = m.Pending_Vendor_Filed_Date,
                            Vendor_Last_Updated = m.Pending_Vendor_Filed_Date,
                            Vendor_Status = m.Pending_Vendor_Status
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
                            Check_Bank_Info = m.Pending_Check_Bank_Info,
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
                            Check_Bank_Info = m.Pending_Check_Bank_Info,
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

        // [DMTRModel ]
        public List<DMTRViewModel> approveTR(string[] IdsArr)
        {
            List<DMTRModel_Pending> pendingList = _context.DMTR_Pending.Where(x => IdsArr.Contains(x.Pending_TR_MasterID.ToString())).Distinct().ToList();
            List<DMTRViewModel> tempList = new List<DMTRViewModel>();
            foreach (DMTRModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_TR_MasterID == int.Parse(s))
                    {
                        DMTRViewModel vm = new DMTRViewModel
                        {
                            TR_MasterID = m.Pending_TR_MasterID,
                            TR_WT_Title = m.Pending_TR_WT_Title,
                            TR_Nature = m.Pending_TR_Nature,
                            TR_Tax_Rate = m.Pending_TR_Tax_Rate,
                            TR_ATC = m.Pending_TR_ATC,
                            TR_Creator_ID = m.Pending_TR_Creator_ID,
                            TR_Approver_ID = m.Pending_TR_Approver_ID.Equals(null) ? 0 : m.Pending_TR_Approver_ID,
                            TR_Created_Date = m.Pending_TR_Filed_Date,
                            TR_Last_Updated = m.Pending_TR_Filed_Date,
                            TR_Status = m.Pending_TR_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMTRViewModel> rejectTR(string[] IdsArr)
        {
            List<DMTRModel_Pending> pendingList = _context.DMTR_Pending.Where(x => IdsArr.Contains(x.Pending_TR_MasterID.ToString())).Distinct().ToList();
            List<DMTRViewModel> tempList = new List<DMTRViewModel>();
            foreach (DMTRModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_TR_MasterID == int.Parse(s))
                    {
                        DMTRViewModel vm = new DMTRViewModel
                        {
                            TR_WT_Title = m.Pending_TR_WT_Title,
                            TR_MasterID = m.Pending_TR_MasterID,
                            TR_Nature = m.Pending_TR_Nature,
                            TR_Tax_Rate = m.Pending_TR_Tax_Rate,
                            TR_ATC = m.Pending_TR_ATC,
                            TR_Creator_ID = m.Pending_TR_Creator_ID,
                            TR_Approver_ID = m.Pending_TR_Approver_ID.Equals(null) ? 0 : m.Pending_TR_Approver_ID,
                            TR_Created_Date = m.Pending_TR_Filed_Date,
                            TR_Last_Updated = m.Pending_TR_Filed_Date,
                            TR_Status = m.Pending_TR_Status
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
                            Curr_CCY_ABBR = m.Pending_Curr_CCY_ABBR,
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
                            Curr_CCY_ABBR = m.Pending_Curr_CCY_ABBR,
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

        //[ Employee ]
        public List<DMEmpViewModel> approveEmp(string[] IdsArr)
        {
            List<DMEmpModel_Pending> pendingList = _context.DMEmp_Pending.Where(x => IdsArr.Contains(x.Pending_Emp_MasterID.ToString())).Distinct().ToList();
            List<DMEmpViewModel> tempList = new List<DMEmpViewModel>();
            foreach (DMEmpModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Emp_MasterID == int.Parse(s))
                    {
                        DMEmpViewModel vm = new DMEmpViewModel
                        {
                            Emp_MasterID = m.Pending_Emp_MasterID,
                            Emp_Name = m.Pending_Emp_Name,
                            Emp_Acc_No = m.Pending_Emp_Acc_No ?? "",
                            Emp_Creator_ID = m.Pending_Emp_Creator_ID,
                            Emp_Approver_ID = m.Pending_Emp_Approver_ID.Equals(null) ? 0 : m.Pending_Emp_Approver_ID,
                            Emp_Created_Date = m.Pending_Emp_Filed_Date,
                            Emp_Last_Updated = m.Pending_Emp_Filed_Date,
                            Emp_Status = m.Pending_Emp_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMEmpViewModel> rejectEmp(string[] IdsArr)
        {
            List<DMEmpModel_Pending> pendingList = _context.DMEmp_Pending.Where(x => IdsArr.Contains(x.Pending_Emp_MasterID.ToString())).Distinct().ToList();
            List<DMEmpViewModel> tempList = new List<DMEmpViewModel>();
            foreach (DMEmpModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Emp_MasterID == int.Parse(s))
                    {
                        DMEmpViewModel vm = new DMEmpViewModel
                        {
                            Emp_MasterID = m.Pending_Emp_MasterID,
                            Emp_Name = m.Pending_Emp_Name,
                            Emp_Acc_No = m.Pending_Emp_Acc_No ?? "",
                            Emp_Creator_ID = m.Pending_Emp_Creator_ID,
                            Emp_Approver_ID = m.Pending_Emp_Approver_ID.Equals(null) ? 0 : m.Pending_Emp_Approver_ID,
                            Emp_Created_Date = m.Pending_Emp_Filed_Date,
                            Emp_Last_Updated = m.Pending_Emp_Filed_Date,
                            Emp_Status = m.Pending_Emp_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Customer ]
        public List<DMCustViewModel> approveCust(string[] IdsArr)
        {
            List<DMCustModel_Pending> pendingList = _context.DMCust_Pending.Where(x => IdsArr.Contains(x.Pending_Cust_MasterID.ToString())).Distinct().ToList();
            List<DMCustViewModel> tempList = new List<DMCustViewModel>();
            foreach (DMCustModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Cust_MasterID == int.Parse(s))
                    {
                        DMCustViewModel vm = new DMCustViewModel
                        {
                            Cust_MasterID = m.Pending_Cust_MasterID,
                            Cust_Name = m.Pending_Cust_Name,
                            Cust_Abbr = m.Pending_Cust_Abbr,
                            Cust_No = m.Pending_Cust_No,
                            Cust_Creator_ID = m.Pending_Cust_Creator_ID,
                            Cust_Approver_ID = m.Pending_Cust_Approver_ID.Equals(null) ? 0 : m.Pending_Cust_Approver_ID,
                            Cust_Created_Date = m.Pending_Cust_Filed_Date,
                            Cust_Last_Updated = m.Pending_Cust_Filed_Date,
                            Cust_Status = m.Pending_Cust_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCustViewModel> rejectCust(string[] IdsArr)
        {
            List<DMCustModel_Pending> pendingList = _context.DMCust_Pending.Where(x => IdsArr.Contains(x.Pending_Cust_MasterID.ToString())).Distinct().ToList();
            List<DMCustViewModel> tempList = new List<DMCustViewModel>();
            foreach (DMCustModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_Cust_MasterID == int.Parse(s))
                    {
                        DMCustViewModel vm = new DMCustViewModel
                        {
                            Cust_MasterID = m.Pending_Cust_MasterID,
                            Cust_Name = m.Pending_Cust_Name,
                            Cust_Abbr = m.Pending_Cust_Abbr,
                            Cust_No = m.Pending_Cust_No,
                            Cust_Creator_ID = m.Pending_Cust_Creator_ID,
                            Cust_Approver_ID = m.Pending_Cust_Approver_ID.Equals(null) ? 0 : m.Pending_Cust_Approver_ID,
                            Cust_Created_Date = m.Pending_Cust_Filed_Date,
                            Cust_Last_Updated = m.Pending_Cust_Filed_Date,
                            Cust_Status = m.Pending_Cust_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ Non Cash Category ]
        public List<DMNCCViewModel> approveNCC(string[] IdsArr)
        {
            List<DMNonCashCategoryModel_Pending> pendingList = _context.DMNCC_Pending.Where(x => IdsArr.Contains(x.Pending_NCC_MasterID.ToString())).Distinct().ToList();
            List<DMNCCViewModel> tempList = new List<DMNCCViewModel>();
            foreach (DMNonCashCategoryModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_NCC_MasterID == int.Parse(s))
                    {
                        DMNCCViewModel vm = new DMNCCViewModel
                        {
                            NCC_MasterID = m.Pending_NCC_MasterID,
                            NCC_Name = m.Pending_NCC_Name,
                            NCC_Pro_Forma = m.Pending_NCC_Pro_Forma,
                            NCC_Creator_ID = m.Pending_NCC_Creator_ID,
                            NCC_Approver_ID = m.Pending_NCC_Approver_ID.Equals(null) ? 0 : m.Pending_NCC_Approver_ID,
                            NCC_Created_Date = m.Pending_NCC_Filed_Date,
                            NCC_Last_Updated = m.Pending_NCC_Filed_Date,
                            NCC_Status = m.Pending_NCC_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMNCCViewModel> rejectNCC(string[] IdsArr)
        {
            List<DMNonCashCategoryModel_Pending> pendingList = _context.DMNCC_Pending.Where(x => IdsArr.Contains(x.Pending_NCC_MasterID.ToString())).Distinct().ToList();
            List<DMNCCViewModel> tempList = new List<DMNCCViewModel>();
            foreach (DMNonCashCategoryModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_NCC_MasterID == int.Parse(s))
                    {
                        DMNCCViewModel vm = new DMNCCViewModel
                        {
                            NCC_MasterID = m.Pending_NCC_MasterID,
                            NCC_Name = m.Pending_NCC_Name,
                            NCC_Pro_Forma = m.Pending_NCC_Pro_Forma,
                            NCC_Creator_ID = m.Pending_NCC_Creator_ID,
                            NCC_Approver_ID = m.Pending_NCC_Approver_ID.Equals(null) ? 0 : m.Pending_NCC_Approver_ID,
                            NCC_Created_Date = m.Pending_NCC_Filed_Date,
                            NCC_Last_Updated = m.Pending_NCC_Filed_Date,
                            NCC_Status = m.Pending_NCC_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //[ BIR Cert Signatory ]
        public List<DMBCSViewModel> approveBCS(string[] IdsArr)
        {
            List<DMBIRCertSignModel_Pending> pendingList = _context.DMBCS_Pending.Where(x => IdsArr.Contains(x.Pending_BCS_MasterID.ToString())).Distinct().ToList();
            List<DMBCSViewModel> tempList = new List<DMBCSViewModel>();
            foreach (DMBIRCertSignModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_BCS_MasterID == int.Parse(s))
                    {
                        DMBCSViewModel vm = new DMBCSViewModel
                        {
                            BCS_MasterID = m.Pending_BCS_MasterID,
                            BCS_Name = m.Pending_BCS_Name,
                            BCS_TIN = m.Pending_BCS_TIN,
                            BCS_Position = m.Pending_BCS_Position,
                            BCS_Signatures = m.Pending_BCS_Signatures,
                            BCS_Creator_ID = m.Pending_BCS_Creator_ID,
                            BCS_Approver_ID = m.Pending_BCS_Approver_ID.Equals(null) ? 0 : m.Pending_BCS_Approver_ID,
                            BCS_Created_Date = m.Pending_BCS_Filed_Date,
                            BCS_Last_Updated = m.Pending_BCS_Filed_Date,
                            BCS_Status = m.Pending_BCS_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMBCSViewModel> rejectBCS(string[] IdsArr)
        {
            List<DMBIRCertSignModel_Pending> pendingList = _context.DMBCS_Pending.Where(x => IdsArr.Contains(x.Pending_BCS_MasterID.ToString())).Distinct().ToList();
            List<DMBCSViewModel> tempList = new List<DMBCSViewModel>();
            foreach (DMBIRCertSignModel_Pending m in pendingList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Pending_BCS_MasterID == int.Parse(s))
                    {
                        DMBCSViewModel vm = new DMBCSViewModel
                        {
                            BCS_MasterID = m.Pending_BCS_MasterID,
                            BCS_Name = m.Pending_BCS_Name,
                            BCS_TIN = m.Pending_BCS_TIN,
                            BCS_Position = m.Pending_BCS_Position,
                            BCS_Signatures = m.Pending_BCS_Signatures,
                            BCS_Creator_ID = m.Pending_BCS_Creator_ID,
                            BCS_Approver_ID = m.Pending_BCS_Approver_ID.Equals(null) ? 0 : m.Pending_BCS_Approver_ID,
                            BCS_Created_Date = m.Pending_BCS_Filed_Date,
                            BCS_Last_Updated = m.Pending_BCS_Filed_Date,
                            BCS_Status = m.Pending_BCS_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }

        //------------------------PENDING-----------------------------
        // [Add New Pending Entries]
        public NewVendorListViewModel addVendor()
        {
            NewVendorListViewModel mod = new NewVendorListViewModel();
            List<NewVendorViewModel> vmList = new List<NewVendorViewModel>();
            NewVendorViewModel vm = new NewVendorViewModel();
            vmList.Add(vm);
            mod.NewVendorVM = vmList;
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
        public NewTRListViewModel addTR()
        {
            NewTRListViewModel mod = new NewTRListViewModel();
            List<NewTRViewModel> vmList = new List<NewTRViewModel>();
            NewTRViewModel vm = new NewTRViewModel
            {
                TR_Tax_Rate = 0
            };
            vmList.Add(vm);
            mod.NewTRVM = vmList;
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
        public NewEmpListViewModel addEmp()
        {
            NewEmpListViewModel mod = new NewEmpListViewModel();
            List<NewEmpViewModel> vmList = new List<NewEmpViewModel>();
            NewEmpViewModel vm = new NewEmpViewModel();
            vmList.Add(vm);
            mod.NewEmpVM = vmList;
            return mod;
        }
        public NewCustListViewModel addCust()
        {
            NewCustListViewModel mod = new NewCustListViewModel();
            List<NewCustViewModel> vmList = new List<NewCustViewModel>();
            NewCustViewModel vm = new NewCustViewModel();
            vmList.Add(vm);
            mod.NewCustVM = vmList;
            return mod;
        }
        public NewNCCViewModel addNCC()
        {
            //NewNCCListViewModel mod = new NewNCCListViewModel();
            //List<NewNCCViewModel> vmList = new List<NewNCCViewModel>();
            NewNCCViewModel vm = new NewNCCViewModel();
            //vmList.Add(vm);
            //mod.NewNCCVM = vmList;
            return vm;
        }
        public NewBCSViewModel addBCS()
        {
            NewBCSViewModel vm = new NewBCSViewModel();
            return vm;
        }


        // [Update Pending Entries]
        public List<DMVendorViewModel> editDeleteVendor(string[] IdsArr)
        {
            List<DMVendorModel> mList = _context.DMVendor.Where(x => IdsArr.Contains(x.Vendor_MasterID.ToString()) 
                                       && x.Vendor_isActive == true).Distinct().ToList();
            List<DMVendorViewModel> tempList = new List<DMVendorViewModel>();
            foreach (DMVendorModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Vendor_MasterID == int.Parse(s))
                    {
                        DMVendorViewModel vm = new DMVendorViewModel
                        {
                            Vendor_MasterID = m.Vendor_MasterID,
                            Vendor_Name = m.Vendor_Name,
                            Vendor_TIN = m.Vendor_TIN,
                            Vendor_Address = m.Vendor_Address,
                            Vendor_Creator_ID = m.Vendor_Creator_ID,
                            Vendor_Created_Date = m.Vendor_Created_Date,
                            Vendor_Last_Updated = DateTime.Now,
                            Vendor_Status = m.Vendor_Status
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
                            Check_Bank_Info = m.Check_Bank_Info,
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
        public List<DMTRViewModel> editDeleteTR(string[] IdsArr)
        {
            List<DMTRModel> mList = _context.DMTR.Where(x => IdsArr.Contains(x.TR_MasterID.ToString())
                                       && x.TR_isActive == true).Distinct().ToList();
            List<DMTRViewModel> tempList = new List<DMTRViewModel>();
            foreach (DMTRModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.TR_MasterID == int.Parse(s))
                    {
                        DMTRViewModel vm = new DMTRViewModel
                        {
                            TR_MasterID = m.TR_MasterID,
                            TR_WT_Title = m.TR_WT_Title,
                            TR_Nature = m.TR_Nature,
                            TR_Tax_Rate = m.TR_Tax_Rate,
                            TR_ATC = m.TR_ATC,
                            TR_Creator_ID = m.TR_Creator_ID,
                            TR_Created_Date = m.TR_Created_Date,
                            TR_Last_Updated = DateTime.Now,
                            TR_Status = m.TR_Status
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
                            Curr_CCY_ABBR = m.Curr_CCY_ABBR,
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
        public List<DMEmpViewModel> editDeleteEmp(string[] IdsArr)
        {
            List<DMEmpModel> mList = _context.DMEmp.Where(x => IdsArr.Contains(x.Emp_MasterID.ToString())
                                        && x.Emp_isActive == true).Distinct().ToList();
            List<DMEmpViewModel> tempList = new List<DMEmpViewModel>();
            foreach (DMEmpModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Emp_MasterID == int.Parse(s))
                    {
                        DMEmpViewModel vm = new DMEmpViewModel
                        {
                            Emp_MasterID = m.Emp_MasterID,
                            Emp_Name = m.Emp_Name,
                            Emp_Acc_No = m.Emp_Acc_No,
                            Emp_Creator_ID = m.Emp_Creator_ID,
                            Emp_Created_Date = m.Emp_Created_Date,
                            Emp_Last_Updated = DateTime.Now,
                            Emp_Status = m.Emp_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public List<DMCustViewModel> editDeleteCust(string[] IdsArr)
        {
            List<DMCustModel> mList = _context.DMCust.Where(x => IdsArr.Contains(x.Cust_MasterID.ToString())
                                        && x.Cust_isActive == true).Distinct().ToList();
            List<DMCustViewModel> tempList = new List<DMCustViewModel>();
            foreach (DMCustModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.Cust_MasterID == int.Parse(s))
                    {
                        DMCustViewModel vm = new DMCustViewModel
                        {
                            Cust_MasterID = m.Cust_MasterID,
                            Cust_Name = m.Cust_Name,
                            Cust_Abbr = m.Cust_Abbr,
                            Cust_No = m.Cust_No,
                            Cust_Creator_ID = m.Cust_Creator_ID,
                            Cust_Created_Date = m.Cust_Created_Date,
                            Cust_Last_Updated = DateTime.Now,
                            Cust_Status = m.Cust_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public DMNCC2ViewModel editNCC(string[] IdsArr)
        {
            List<DMNonCashCategoryModel> mList = _context.DMNCC.Where(x => IdsArr.Contains(x.NCC_MasterID.ToString())
                                        && x.NCC_isActive == true).Distinct().ToList();
            List<DMNCC2ViewModel> tempList = new List<DMNCC2ViewModel>();

            DMNCC2ViewModel vm = new DMNCC2ViewModel();
            //foreach (DMNonCashCategoryModel m in mList)
            //{
            //    foreach (string s in IdsArr)
            //    {
            //        if (m.NCC_MasterID == int.Parse(s))
            //        {
                        //byte[] byteArray = Encoding.UTF8.GetBytes(m.NCC_Pro_Forma);
                        //MemoryStream stream = new MemoryStream(byteArray);
                        //var ms = new MemoryStream();
                        //try
                        //{
                        //    stream.FileStream.CopyTo(ms);
                        //    return new FormFile(ms, 0, ms.Length);
                        //}
                        //finally
                        //{
                        //    ms.Dispose();
                        //}
                        vm = new DMNCC2ViewModel
                        {
                            NCC_MasterID = mList.First().NCC_MasterID,
                            NCC_Name = mList.First().NCC_Name,
                            NCC_Pro_Forma_Name = mList.First().NCC_Pro_Forma,
                            NCC_Creator_ID = mList.First().NCC_Creator_ID,
                            NCC_Created_Date = mList.First().NCC_Created_Date,
                            NCC_Last_Updated = DateTime.Now,
                            NCC_Status = mList.First().NCC_Status
                        };
                        //tempList.Add(vm);
            //        }
            //    }
            //}
            return vm;
        }
        public List<DMNCCViewModel> deleteNCC(string[] IdsArr)
        {
            List<DMNonCashCategoryModel> mList = _context.DMNCC.Where(x => IdsArr.Contains(x.NCC_MasterID.ToString())
                                        && x.NCC_isActive == true).Distinct().ToList();
            List<DMNCCViewModel> tempList = new List<DMNCCViewModel>();

            DMNCCViewModel vm = new DMNCCViewModel();
            foreach (DMNonCashCategoryModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.NCC_MasterID == int.Parse(s))
                    {
                        vm = new DMNCCViewModel
                        {
                            NCC_MasterID = m.NCC_MasterID,
                            NCC_Name = m.NCC_Name,
                            NCC_Pro_Forma = m.NCC_Pro_Forma,
                            NCC_Creator_ID = m.NCC_Creator_ID,
                            NCC_Created_Date = m.NCC_Created_Date,
                            NCC_Last_Updated = DateTime.Now,
                            NCC_Status = m.NCC_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
        public DMBCS2ViewModel editBCS(string[] IdsArr)
        {
            List<DMBIRCertSignModel> mList = _context.DMBCS.Where(x => IdsArr.Contains(x.BCS_MasterID.ToString())
                                        && x.BCS_isActive == true).Distinct().ToList();
            List<DMBCS2ViewModel> tempList = new List<DMBCS2ViewModel>();

            DMBCS2ViewModel vm = new DMBCS2ViewModel();
            vm = new DMBCS2ViewModel
            {
                BCS_MasterID = mList.First().BCS_MasterID,
                BCS_Name = mList.First().BCS_Name,
                BCS_TIN = mList.First().BCS_TIN,
                BCS_Position = mList.First().BCS_Position,
                BCS_Signatures_Name = mList.First().BCS_Signatures,
                BCS_Creator_ID = mList.First().BCS_Creator_ID,
                BCS_Created_Date = mList.First().BCS_Created_Date,
                BCS_Last_Updated = DateTime.Now,
                BCS_Status = mList.First().BCS_Status
            };
            return vm;
        }
        public List<DMBCSViewModel> deleteBCS(string[] IdsArr)
        {
            List<DMBIRCertSignModel> mList = _context.DMBCS.Where(x => IdsArr.Contains(x.BCS_MasterID.ToString())
                                        && x.BCS_isActive == true).Distinct().ToList();
            List<DMBCSViewModel> tempList = new List<DMBCSViewModel>();

            DMBCSViewModel vm = new DMBCSViewModel();
            foreach (DMBIRCertSignModel m in mList)
            {
                foreach (string s in IdsArr)
                {
                    if (m.BCS_MasterID == int.Parse(s))
                    {
                        vm = new DMBCSViewModel
                        {
                            BCS_MasterID = m.BCS_MasterID,
                            BCS_Name = m.BCS_Name,
                            BCS_TIN = m.BCS_TIN,
                            BCS_Position = m.BCS_Position,
                            BCS_Signatures = m.BCS_Signatures,
                            BCS_Creator_ID = m.BCS_Creator_ID,
                            BCS_Created_Date = m.BCS_Created_Date,
                            BCS_Last_Updated = DateTime.Now,
                            BCS_Status = m.BCS_Status
                        };
                        tempList.Add(vm);
                    }
                }
            }
            return tempList;
        }
    }
}
