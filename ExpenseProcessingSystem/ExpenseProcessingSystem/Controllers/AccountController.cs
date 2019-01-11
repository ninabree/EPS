using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseProcessingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly TSContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public AccountController(IHttpContextAccessor httpContextAccessor/*, TSContext context*/)
        {
            _httpContextAccessor = httpContextAccessor;
            //_context = context;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
    }
}