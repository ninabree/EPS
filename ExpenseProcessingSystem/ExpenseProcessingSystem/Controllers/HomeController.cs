using System;
using System.Collections.Generic; 
using System.Globalization;
using System.Linq;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseProcessingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly int pageSize = 30;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private HomeService _service;
        //public ImportModelStateFromTempData _importTempDataService;
        //private ExportModelStateToTempData _exportTempDataService;

        public HomeController(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _service = new HomeService(_httpContextAccessor, _context, this.ModelState);
            //_importTempDataService = new ImportModelStateFromTempData(this.ModelState, this.TempData);
            //_exportTempDataService = new ExportModelStateToTempData(this.ModelState, this.TempData);
        }

        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            return View();
        }
        public IActionResult Entry()
        {
            return View();
        }
        public IActionResult Close(string sortOrder, string currentFilter, string searchString, int? page)
        {
            return View();
        }
        public IActionResult DM(string sortOrder, string currentFilter, string tblName, string colName, string searchString, int? page, string partialName)
        {
            ViewData["sortOrder"] = sortOrder;
            ViewData["currentFilter"] = searchString;
            ViewData["tblName"] = tblName;
            ViewData["colName"] = colName;
            ViewData["searchString"] = searchString;
            ViewData["page"] = page;
            ViewData["partialName"] = (partialName == null) ? "DMPartial_Payee" : partialName;
            return View();
        }
        public IActionResult Report()
        {
            return View();
        }
        public IActionResult BM(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //CheckScreenRes();

            //sort
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AccountSortParm"] = String.IsNullOrEmpty(sortOrder) ? "acc_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type_desc" ? "Type" : "Type_desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var bmmvList = PopulateBM().AsQueryable();
            var bm = from e in bmmvList
                     select e;
            switch (sortOrder)
            {
                case "acc_desc":
                    bm = bm.OrderByDescending(s => s.BM_Account);
                    ViewData["glyph-acc"] = "glyphicon-menu-up";
                    break;
                case "Type":
                    bm = bm.OrderBy(s => s.BM_Type);
                    ViewData["glyph-type"] = "glyphicon-menu-down";
                    break;
                case "Type_desc":
                    bm = bm.OrderByDescending(s => s.BM_Type);
                    ViewData["glyph-type"] = "glyphicon-menu-up";
                    break;
                default:
                    bm = bm.OrderBy(s => s.BM_Account);
                    ViewData["glyph-acc"] = "glyphicon-menu-down";
                    break;
            }

            //pagination
            return View(PaginatedList<BMViewModel>.CreateAsync(bm.AsNoTracking(), page ?? 1, pageSize));
        }
        [ImportModelState]
        public IActionResult UM()
        {
            List<AccountViewModel> vmList = new List<AccountViewModel>();
            _context.Account.ToList().ForEach(x => {
                AccountViewModel vm = new AccountViewModel
                {
                    Acc_UserID = x.Acc_UserID,
                    Acc_UserName = x.Acc_UserName,
                    Acc_FName = x.Acc_FName,
                    Acc_LName = x.Acc_LName,
                    Acc_DeptID = x.Acc_DeptID,
                    Acc_Email = x.Acc_Email,
                    Acc_Role = x.Acc_Role,
                    Acc_InUse = x.Acc_InUse,
                    Acc_Comment = x.Acc_Comment,
                    Acc_Creator_ID = x.Acc_Creator_ID,
                    Acc_Approver_ID = x.Acc_Approver_ID,
                    Acc_Created_Date = x.Acc_Created_Date,
                    Acc_Last_Updated = x.Acc_Last_Updated,
                    Acc_Status = x.Acc_Status
                };
                vmList.Add(vm);
            });
            UserManagementViewModel mod = new UserManagementViewModel
            {
                NewAcc = new AccountViewModel(),
                AccList = vmList
            };
            //ModelState errors not kept when redirected
            if (ModelState.IsValid)
            {

            }
            return View(mod);
        }
        public IActionResult Entry_CV()
        {
            return View();
        }
        public IActionResult Entry_DDV()
        {
            return View();
        }
        public IActionResult Entry_PCV()
        {
            return View();
        }
        public IActionResult Entry_SS()
        {
            return View();
        }
        public IActionResult Entry_NC()
        {
            return View();
        }

        [HttpPost]
        //[Route("/RedirectCont/")]
        public IActionResult RedirectCont(string Cont, string Method)
        {
            return RedirectToAction(Method, Cont);
        }
        [HttpPost]
        public IActionResult RedirectCont2(string Cont, string Method, string[] IdsArr)
        {
            return RedirectToAction(Method, Cont, new { IdsArr = IdsArr });
        }

        //------------------------------------------------------------------
        //CRUD
        //PAYEE
        [HttpPost]
        public IActionResult AddPayee(NewPayeeListViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMPayeeModel> vmList = new List<DMPayeeModel>();
            foreach (NewPayeeViewModel dm in model.NewPayeeVM)
            {
                DMPayeeModel m = new DMPayeeModel
                {
                    Payee_Name = dm.Payee_Name,
                    Payee_TIN = dm.Payee_TIN,
                    Payee_Address = dm.Payee_Address,
                    Payee_Type = dm.Payee_Type,
                    Payee_No = dm.Payee_No,
                    Payee_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Payee_Created_Date = DateTime.Now,
                    Payee_Last_Updated = DateTime.Now,
                    Payee_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (ModelState.IsValid)
            {
                _context.DMPayee.AddRange(vmList);
                _context.SaveChanges();
            }

            return RedirectToAction("DM", "Home");
        }

        [HttpPost]
        public IActionResult EditPayee(List<DMPayeeViewModel> model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<int> intList = model.Select(c => c.Payee_ID).ToList();
            List<DMPayeeModel> vmList = _context.DMPayee.Where(x => intList.Contains(x.Payee_ID)).ToList();

            vmList.ForEach(dm =>
            {
                model.ForEach( m =>
                {
                    if (m.Payee_ID == dm.Payee_ID)
                    {
                        dm.Payee_Name = m.Payee_Name;
                        dm.Payee_TIN = m.Payee_TIN;
                        dm.Payee_Address = m.Payee_Address;
                        dm.Payee_Type = m.Payee_Type;
                        dm.Payee_No = m.Payee_No;
                        dm.Payee_Approver_ID = int.Parse(_session.GetString("UserID"));
                        dm.Payee_Last_Updated = DateTime.Now;
                        dm.Payee_Status = "Is Updated";
                        dm.Payee_isDeleted = false;
                    }
                });
            });

            if (ModelState.IsValid)
            {
                _context.SaveChanges();
            }

            return RedirectToAction("DM", "Home");
        }

        [HttpPost]
        public IActionResult DeletePayee(List<DMPayeeViewModel> model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<int> intList = model.Select(c => c.Payee_ID).ToList();
            List<DMPayeeModel> vmList = _context.DMPayee.Where(x => intList.Contains(x.Payee_ID)).ToList();

            vmList.ForEach(dm =>
            {
                dm.Payee_Approver_ID = int.Parse(_session.GetString("UserID"));
                dm.Payee_Last_Updated = DateTime.Now;
                dm.Payee_Status = "Is Deleted";
                dm.Payee_isDeleted = true;
            });

            if (ModelState.IsValid)
            {
                _context.SaveChanges();
            }

            return RedirectToAction("DM", "Home");
        }
        //DEPT
        [HttpPost]
        public IActionResult AddDept(NewDeptListViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<DMDeptModel> vmList = new List<DMDeptModel>();
            foreach(NewDeptViewModel dm in model.NewDeptVM)
            {
                DMDeptModel m = new DMDeptModel
                {
                    Dept_Name = dm.Dept_Name,
                    Dept_Code = dm.Dept_Code,
                    Dept_Creator_ID = int.Parse(_session.GetString("UserID")),
                    Dept_Created_Date = DateTime.Now,
                    Dept_Last_Updated = DateTime.Now,
                    Dept_Status = "For Approval"
                };
                vmList.Add(m);
            }

            if (ModelState.IsValid)
            {
                _context.DMDept.AddRange(vmList);
                _context.SaveChanges();
            }

            return RedirectToAction("DM", "Home");
        }

        [HttpPost]
        public IActionResult EditDept(List<DMDeptViewModel> model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<int> intList = model.Select(c => c.Dept_ID).ToList();
            List<DMDeptModel> vmList = _context.DMDept.Where(x => intList.Contains(x.Dept_ID)).ToList();

            vmList.ForEach(dm =>
            {
                model.ForEach(m =>
                {
                    if (m.Dept_ID == dm.Dept_ID)
                    {
                        dm.Dept_Name = m.Dept_Name;
                        dm.Dept_Code = m.Dept_Code;
                        dm.Dept_Approver_ID = int.Parse(_session.GetString("UserID"));
                        dm.Dept_Last_Updated = DateTime.Now;
                        dm.Dept_Status = "Is Updated";
                        dm.Dept_isDeleted = false;
                    }
                });
            });

            if (ModelState.IsValid)
            {
                _context.SaveChanges();
            }

            return RedirectToAction("DM", "Home");
        }

        [HttpPost]
        public IActionResult DeleteDept(List<DMDeptViewModel> model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<int> intList = model.Select(c => c.Dept_ID).ToList();
            List<DMDeptModel> vmList = _context.DMDept.Where(x => intList.Contains(x.Dept_ID)).ToList();

            vmList.ForEach(dm =>
            {
                dm.Dept_Approver_ID = int.Parse(_session.GetString("UserID"));
                dm.Dept_Last_Updated = DateTime.Now;
                dm.Dept_Status = "Is Deleted";
                dm.Dept_isDeleted = true;
            });

            if (ModelState.IsValid)
            {
                _context.SaveChanges();
            }

            return RedirectToAction("DM", "Home");
        }
        //USER
        [HttpPost]
        [ExportModelState]
        public IActionResult AddEditUser(UserManagementViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                _service.addUser(model, userId);
            }
            

            return RedirectToAction("UM", "Home");
        }
        //MISC
        public void CheckScreenRes()
        {
            //int h = Screen.AllScreens.GetLowerBound.height;
            //int h = Screen.PrimaryScreen.WorkingArea.Height;
            //int height = SystemInformation.PrimaryMonitorSize.Height;
            //int width = SystemInformation.VirtualScreen.Width;
            //string h = _session.GetString("clientScreenHeight");
            //if (int.Parse(h) <= 768)
            //{
            //    pageSize = 20;
            //}
        }
        public List<BMViewModel> PopulateBM()
        {
            List<BMViewModel> bmvmList = new List<BMViewModel>();
            for (var i = 1; i <= 40; i++)
            {
                BMViewModel bmvm = new BMViewModel
                {
                    BM_Id = i,
                    BM_Creator_ID = i + 100,
                    BM_Approver_ID = i + 200,
                    BM_Account = "Account_" + i,
                    BM_Type = "Sample_Type_" + i,
                    BM_Budget = i + 100,
                    BM_Curr_Budget = i + 110,
                    BM_Last_Trans_Date = DateTime.Parse("1/12/2017", CultureInfo.GetCultureInfo("en-GB"))
                            .Add(DateTime.Now.TimeOfDay),
                    BM_Last_Budget_Approval = "Sample"
                };
                bmvmList.Add(bmvm);
            }
            return bmvmList;
        }
    }
}