using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Excel_Services
{
    public class ExcelService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        
        public ExcelService(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public byte[] Excel(List<string> colHeadrs, ExcelViewModel dataArr, string worksheetName)
        //public byte[] Excel()
        {
            byte[] result;

            using (var package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook

                var worksheet = package.Workbook.Worksheets.Add(worksheetName); //Worksheet name
                using (var cells = worksheet.Cells[1, 1, 1, colHeadrs.Count()]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                //First add the headers
                for (var i = 0; i < colHeadrs.Count(); i++)
                {
                    worksheet.Cells[1, i + 1].Value = colHeadrs[i];
                }

                //Add values
                var row = 2;
                for (int i = 0; i < dataArr.RowList.Count(); i++)
                {
                    char c = 'A';
                    for (int r = 0; r < dataArr.RowList[i].DataList.Count(); r++)
                    {
                        worksheet.Cells[c.ToString() + row].Value = dataArr.RowList[i].DataList[r];
                        c++;
                    }
                    row++;
                }
                result = package.GetAsByteArray();
            }
            return result;
        }
    }
    public class DummyData
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        
        public DummyData(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public List<DMPayeeModel> GetPayeeData()
        {
            return _context.DMPayee.ToList();
        }
    }
}
