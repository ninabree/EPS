using Microsoft.AspNetCore.Mvc;
using System;

namespace ExpenseProcessingSystem.Controllers
{
    //[System.Web.Mvc.RoutePrefix("error")]
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Error()
        {
            ViewData["filename"] = "log" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            return View();
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}