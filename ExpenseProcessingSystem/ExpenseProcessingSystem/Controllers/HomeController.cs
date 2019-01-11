using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseProcessingSystem.Controllers
{
    public class HomeController : Controller
    {
        //private readonly int pageSize = 25;
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ////sort
            //ViewData["CurrentSort"] = sortOrder;
            //ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            //ViewData["DateSortParm"] = sortOrder == "AppDateDesc" ? "AppDate" : "AppDateDesc";

            //if (searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    searchString = currentFilter;
            //}
            //ViewData["CurrentFilter"] = searchString;

            //IQueryable<HomeViewModel> hmvList = PopulateHome().AsQueryable();
            //var students = from e in hmvList
            //               select e;
            //switch (sortOrder)
            //{
            //    case "id_desc":
            //        students = students.OrderByDescending(s => s.Home_Id);
            //        ViewData["glyph-id"] = "glyphicon-menu-up";
            //        break;
            //    case "AppDate":
            //        students = students.OrderBy(s => s.Home_AppDate);
            //        ViewData["glyph-appDate"] = "glyphicon-menu-down";
            //        break;
            //    case "AppDateDesc":
            //        students = students.OrderByDescending(s => s.Home_AppDate);
            //        ViewData["glyph-appDate"] = "glyphicon-menu-up";
            //        break;
            //    default:
            //        students = students.OrderBy(s => s.Home_Id);
            //        ViewData["glyph-id"] = "glyphicon-menu-down";
            //        break;
            //}

            ////pagination
            //return View(PaginatedList<HomeViewModel>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
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
        public IActionResult DM(string sortOrder, string currentFilter, string searchString, int? page)
        {
            return View();
        }
        public IActionResult Report()
        {
            return View();
        }
        public IActionResult BM(string sortOrder, string currentFilter, string searchString, int? page)
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

        //---
        //public List<HomeViewModel> PopulateHome()
        //{
        //    List<HomeViewModel> hvmList = new List<HomeViewModel>();
        //    for (var i = 1; i <= 40; i++)
        //    {
        //        HomeViewModel hvm = new HomeViewModel
        //        {
        //            Home_Id = i,
        //            Home_AppId = i + 100,
        //            Home_Type = "Sample",
        //            Home_Amount = i + 5000,
        //            Home_Payee = "Payee_" + i,
        //            Home_Maker = "Maker_" + i,
        //            Home_Verifier = "Verifier_" + i,
        //            Home_AppDate = DateTime.Parse("1/12/2017", CultureInfo.GetCultureInfo("en-GB"))
        //                    .Add(DateTime.Now.TimeOfDay),
        //            Home_LastUpdated = DateTime.Parse("1/12/2017", CultureInfo.GetCultureInfo("en-GB"))
        //                    .Add(DateTime.Now.TimeOfDay),
        //            Home_Status = "For Approval"
        //        };
        //        hvmList.Add(hvm);
        //    }
        //    return hvmList;
        //}
    }
}