using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.ViewModels;
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
            //if(partialName != null)
            //{
            //    return RedirectToAction(partialName,"Partial");
            //}
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

        [HttpPost]
        //[Route("/RedirectCont/")]
        public IActionResult RedirectCont(string Cont, string Method)
        {
            return RedirectToAction(Method, Cont);
        }
        //---
        
    }
}