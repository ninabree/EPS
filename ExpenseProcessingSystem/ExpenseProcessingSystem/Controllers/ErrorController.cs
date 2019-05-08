using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.Services;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseProcessingSystem.Controllers
{
    public class ErrorController : Controller
    {
        [ImportModelState]
        public IActionResult Index()
        {
            AccessViewModel accessVM = new AccessViewModel
            {
                isLoggedIn = true,
                accessType = "",
                isAdmin = false
            };
            ViewBag.access = accessVM;
            return View();
        }
    }
}