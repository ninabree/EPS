using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseProcessingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly int pageSize = 25;
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
        public IActionResult DM(string sortOrder, string currentFilter, string searchString, int? page, string partialName)
        {
            ViewData["sortOrder"] = sortOrder;
            ViewData["currentFilter"] = currentFilter;
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
        public IActionResult UM()
        {
            return View();
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
        //---
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