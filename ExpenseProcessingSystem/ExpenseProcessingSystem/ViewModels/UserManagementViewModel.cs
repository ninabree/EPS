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
    }
}
