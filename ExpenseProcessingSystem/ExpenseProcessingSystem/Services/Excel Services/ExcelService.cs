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

                //Add the headers
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

                //set column values to autofit
                worksheet.Cells.AutoFitColumns();

                result = package.GetAsByteArray();
            }
            return result;
        }
    }
    public class ExcelData
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EPSDbContext _context;
        private ExcelService _excelService;
        
        public ExcelData(IHttpContextAccessor httpContextAccessor, EPSDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _excelService = new ExcelService(_httpContextAccessor, _context);
        }

        public byte[] GetPayeeExcelData() {
            ExcelViewModel excelVM = new ExcelViewModel();
            List<Row> rowList = new List<Row>();
            string worksheetName = "Current Payee Information";
            //get column names in DB table
            List<string> colHeadrs = typeof(DMVendorModel).GetProperties()
                        .Select(property => property.Name)
                        .ToList();

            //Populate Excel VM (of All DMVendor Entries)
            _context.DMVendor.ToList().ForEach(x => {
                Row row = new Row();
                List<string> rowData = new List<string>
                {
                    x.Vendor_ID.ToString(),
                    x.Vendor_Name,
                    x.Vendor_TIN,
                    x.Vendor_Address,
                    x.Vendor_Created_Date.ToString("MM/dd/yyyy"),
                    x.Vendor_Creator_ID.ToString(),
                    x.Vendor_Last_Updated.ToString("MM/dd/yyyy"),
                    x.Vendor_Approver_ID.ToString(),
                    x.Vendor_Status,
                    x.Vendor_isDeleted.ToString()
                };
                row.DataList = rowData;
                rowList.Add(row);
            });
            excelVM.RowList = rowList;
            
            return _excelService.Excel(colHeadrs, excelVM, worksheetName);
        }
        public byte[] GetDeptExcelData()
        {
            ExcelViewModel excelVM = new ExcelViewModel();
            List<Row> rowList = new List<Row>();
            string worksheetName = "Current Department Information";
            //get column names in DB table
            List<string> colHeadrs = typeof(DMDeptModel).GetProperties()
                        .Select(property => property.Name)
                        .ToList();

            //Populate Excel VM (of All DMVendor Entries)
            _context.DMDept.ToList().ForEach(x => {
                Row row = new Row();
                List<string> rowData = new List<string>
                {
                    x.Dept_ID.ToString(),
                    x.Dept_Name,
                    x.Dept_Code,
                    x.Dept_Created_Date.ToString("MM/dd/yyyy"),
                    x.Dept_Creator_ID.ToString(),
                    x.Dept_Last_Updated.ToString("MM/dd/yyyy"),
                    x.Dept_Approver_ID.ToString(),
                    x.Dept_Status,
                    x.Dept_isDeleted.ToString()
                };
                row.DataList = rowData;
                rowList.Add(row);
            });
            excelVM.RowList = rowList;

            return _excelService.Excel(colHeadrs, excelVM, worksheetName);
        }
    }
}
