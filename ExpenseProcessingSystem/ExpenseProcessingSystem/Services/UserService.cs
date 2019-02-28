using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services
{
    public class UserService
    {
        private ModelStateDictionary _modelState;
        public UserService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        public bool addUser()
        {
            return true;
        }
    }
}
