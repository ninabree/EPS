using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels
{
    public class UserManagementViewModel
    {
        public AccountViewModel NewAcc { get; set; }
        public List<UserViewModel> AccList = new List<UserViewModel>();
        public List<DMDeptViewModel> DeptList = new List<DMDeptViewModel>();
        public SelectList RoleList = new SelectList(new[]
            {
                new { ID = "admin", Name = "Admin" },
                new { ID = "maker", Name = "Maker" },
                new { ID = "verifier", Name = "Verifier" },
                new { ID = "approver", Name = "Approver" }
            },
            "ID", "Name", 1);
    }
}
