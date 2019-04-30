using ExpenseProcessingSystem.Services.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.ViewModels.NewRecord
{
    public class NewNCCListViewModel
    {
        public List<NewNCCViewModel> NewNCCVM { get; set; }
    }
    public class NewNCCList2ViewModel
    {
        public List<string> NCC_Name_List { get; set; }
        public List<IFormFile> NCC_Pro_Forma_List { get; set; }
    }
    public class NewNCCViewModel
    {
        [Display(Name = "Non Cash Category Name")]
        [NotNullValidations, TextValidation]
        public string NCC_Name { get; set; }
        [Display(Name = "Non Cash Category Pro-Forma Entries")]
        public IFormFile NCC_Pro_Forma { get; set; }
    }
}
