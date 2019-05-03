using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Excel_Services
{
    public class ExcelGenerateService
    {
        public string ExcelGenerateData(string layoutName, string fileName, TEMP_HomeReportDataFilterViewModel data)
        {
            string excelTemplateName = layoutName + ".xlsx";
            string rootFolder = "wwwroot";
            string sourcePath = "/ExcelTemplates/";
            string destPath = "/ExcelTemplatesTempFolder/";
            System.IO.File.Copy(rootFolder + sourcePath + excelTemplateName, rootFolder + destPath + fileName, true);

            FileInfo templateFile = new FileInfo(rootFolder + sourcePath + excelTemplateName);
            FileInfo newFile = new FileInfo(rootFolder + destPath + fileName);

            switch (data.HomeReportFilter.ReportType)
            {
                case ConstantData.HomeReportConstantValue.APSWT_M:

                    ExcelAPSWT_M(newFile, templateFile, data);
                    break;
            }

            Debug.WriteLine("DEBUG1");

            return destPath + fileName;
        }

        public void ExcelAPSWT_M(FileInfo newFile, FileInfo templateFile, TEMP_HomeReportDataFilterViewModel data)
        {
            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int lastRow = worksheet.Dimension.End.Row;
                int dataStartRow = worksheet.Dimension.End.Row + 1;
                int dataEndRow = 0;

                worksheet.Cells["C5"].Value = data.HomeReportFilter.Month + " - " + data.HomeReportFilter.Year;

                foreach (var i in data.HomeReportOutputAPSWT_M)
                {
                    lastRow += 1;
                    worksheet.Cells["A" + lastRow].Value = i.Tin;
                    worksheet.Cells["A" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["B" + lastRow].Value = i.Payee;
                    worksheet.Cells["B" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["C" + lastRow].Value = i.ATC;
                    worksheet.Cells["C" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["D" + lastRow].Value = i.NOIP;
                    worksheet.Cells["D" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["E" + lastRow].Value = i.AOIP;
                    worksheet.Cells["E" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["F" + lastRow].Value = i.RateOfTax;
                    worksheet.Cells["F" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["G" + lastRow].Value = i.AOTW;
                    worksheet.Cells["G" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                dataEndRow = lastRow;
                lastRow += 1;

                worksheet.Cells["E" + lastRow].Formula = "SUM(E"+ dataStartRow + ":E" + dataEndRow + ")";
                worksheet.Cells["E" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                worksheet.Cells["G" + lastRow].Formula = "SUM(G" + dataStartRow + ":G" + dataEndRow + ")";
                worksheet.Cells["G" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                package.Save();
            }
        }
    }
}
