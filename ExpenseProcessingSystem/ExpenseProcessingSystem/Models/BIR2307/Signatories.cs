using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BIR_Form_Filler.Models
{
    public class Signatories
    {
        private string name;
        private string tin;
        private string title;
        private string taxAcc;
        private string eSigPath;
        private DateTime dateIssue;
        private DateTime dateExpiry;


        public string Name { get => name; set => name = value; }
        public string Tin { get => tin; set => tin = value; }
        public string Title { get => title; set => title = value; }
        public string TaxAcc { get => taxAcc; set => taxAcc = value; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime DateIssue { get => dateIssue; set => dateIssue = value; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime DateExpiry { get => dateExpiry; set => dateExpiry = value; }
        public string ESigPath { get => eSigPath; set => eSigPath = value; }
    }
}
