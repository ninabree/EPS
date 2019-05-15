using ExpenseProcessingSystem.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

namespace ExpenseProcessingSystem.Services.Excel_Services
{
    public class ExcelGenerateService
    {
        public string ExcelGenerateData(string layoutName, string fileName, HomeReportDataFilterViewModel data)
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

                case ConstantData.HomeReportConstantValue.AST1000_S:
                case ConstantData.HomeReportConstantValue.AST1000_A:

                    ExcelAST1000(newFile, templateFile, data);
                    break;
                case ConstantData.HomeReportConstantValue.WTS:

                    ExcelWTS(newFile, templateFile, data);
                    break;
            }

            return destPath + fileName;
        }

        public void ExcelAPSWT_M(FileInfo newFile, FileInfo templateFile, HomeReportDataFilterViewModel data)
        {
            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int lastRow = worksheet.Dimension.End.Row;
                int dataStartRow = worksheet.Dimension.End.Row + 1;
                int dataEndRow = 0;

                worksheet.Cells["C5"].Value = data.HomeReportFilter.MonthName + " - " + data.HomeReportFilter.Year;

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

                lastRow += 3;
                worksheet.Cells["C" + lastRow].Value = "ALBERT ADVINCULA";
                worksheet.Cells["C" + lastRow].Style.Font.Bold = true;
                worksheet.Cells["C" + lastRow].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                lastRow += 1;
                worksheet.Cells["C" + lastRow].Value = "VP-Manager/ AdministrationDepartment";
                worksheet.Cells["C" + lastRow].Style.Font.Bold = true;
                worksheet.Cells["C" + lastRow].Style.Font.UnderLine = true;
                worksheet.Cells["C" + lastRow].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                package.Save();
            }
        }

        public void ExcelAST1000(FileInfo newFile, FileInfo templateFile, HomeReportDataFilterViewModel data)
        {
            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int lastRow = worksheet.Dimension.End.Row;
                int dataStartRow = worksheet.Dimension.End.Row + 1;
                int dataEndRow = 0;
                string timePeriod = "";

                if(data.HomeReportFilter.ReportType == ConstantData.HomeReportConstantValue.AST1000_S)
                {
                    if (data.HomeReportFilter.Semester == ConstantData.HomeReportConstantValue.SEM1)
                    {
                        timePeriod = data.HomeReportFilter.Semester + "st Term " + data.HomeReportFilter.YearSem;
                    }
                    else
                    {
                        timePeriod = data.HomeReportFilter.Semester + "nd Term " + data.HomeReportFilter.YearSem;
                    }
                }else if(data.HomeReportFilter.ReportType == ConstantData.HomeReportConstantValue.AST1000_A)
                {
                    timePeriod = "Year " + data.HomeReportFilter.Year;
                    
                }

                worksheet.Cells["A2"].Value = timePeriod;
                worksheet.Cells["A2"].Style.Font.UnderLine = true;

                foreach (var i in data.HomeReportOutputAST1000)
                {
                    lastRow += 1;
                    worksheet.Cells["A" + lastRow].Value = i.SeqNo;
                    worksheet.Cells["A" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["B" + lastRow].Value = i.Tin;
                    worksheet.Cells["B" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["C" + lastRow].Value = i.SupplierName;
                    worksheet.Cells["C" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["D" + lastRow].Value = i.NOIP;
                    worksheet.Cells["D" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["E" + lastRow].Value = i.ATC;
                    worksheet.Cells["E" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["F" + lastRow].Value = i.TaxBase;
                    worksheet.Cells["F" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["G" + lastRow].Value = i.RateOfTax;
                    worksheet.Cells["G" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["H" + lastRow].Value = i.AOTW;
                    worksheet.Cells["H" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }

                dataEndRow = lastRow;
                lastRow += 1;

                worksheet.Cells["A" + lastRow].Value = "***End of Report***";
                worksheet.Cells["A" + lastRow].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["E" + lastRow].Value = "TOTAL =>";
                worksheet.Cells["E" + lastRow].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["F" + lastRow].Formula = "SUM(F" + dataStartRow + ":F" + dataEndRow + ")";
                worksheet.Cells["F" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                worksheet.Cells["H" + lastRow].Formula = "SUM(H" + dataStartRow + ":H" + dataEndRow + ")";
                worksheet.Cells["H" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

                lastRow += 3;
                worksheet.Cells["F" + lastRow].Value = "ALBERT ADVINCULA";
                worksheet.Cells["F" + lastRow].Style.Font.Bold = true;
                worksheet.Cells["E" + lastRow].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                worksheet.Cells["F" + lastRow].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                worksheet.Cells["G" + lastRow].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                worksheet.Cells["F" + lastRow].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                lastRow += 1;
                worksheet.Cells["F" + lastRow].Value = "VP-Manager/ AdministrationDepartment";
                worksheet.Cells["F" + lastRow].Style.Font.Bold = true;
                worksheet.Cells["F" + lastRow].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                package.Save();
            }
        }


        public void ExcelWTS(FileInfo newFile, FileInfo templateFile, HomeReportDataFilterViewModel data)
        {
            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int lastRow = worksheet.Dimension.End.Row;
                int dataStartRow = worksheet.Dimension.End.Row + 1;
                int dataEndRow = 0;
                var cellVal = "";
                switch (data.HomeReportFilter.PeriodOption)
                {
                    case 1:
                        cellVal = data.HomeReportFilter.MonthName + " - " + data.HomeReportFilter.Year;
                        break;
                    case 2:
                        cellVal = data.HomeReportFilter.SemesterName + " - " + data.HomeReportFilter.YearSem;
                        break;
                    case 3:
                        cellVal = data.HomeReportFilter.PeriodFrom + " - " + data.HomeReportFilter.PeriodTo;
                        break;
                }
                worksheet.Cells["J2"].Value = cellVal;

                foreach (var i in data.HomeReportOutputWTS)
                {
                    lastRow += 1;
                    worksheet.Cells["A" + lastRow].Value = i.WTS_Voucher_No;
                    worksheet.Cells["A" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["B" + lastRow].Value = i.WTS_Check_No;
                    worksheet.Cells["B" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["C" + lastRow].Value = i.WTS_Val_Date;
                    worksheet.Cells["C" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["D" + lastRow].Value = i.WTS_Ref_No;
                    worksheet.Cells["D" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["E" + lastRow].Value = i.WTS_Section;
                    worksheet.Cells["E" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["F" + lastRow].Value = i.WTS_Remarks;
                    worksheet.Cells["F" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["G" + lastRow].Value = i.WTS_Deb_Cred;
                    worksheet.Cells["G" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["H" + lastRow].Value = i.WTS_Currency_Name;
                    worksheet.Cells["H" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["I" + lastRow].Value = i.WTS_Amount;
                    worksheet.Cells["I" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["J" + lastRow].Value = i.WTS_Cust;
                    worksheet.Cells["J" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["K" + lastRow].Value = i.WTS_Acc_Code;
                    worksheet.Cells["K" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["L" + lastRow].Value = i.WTS_Acc_No;
                    worksheet.Cells["L" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["M" + lastRow].Value = i.WTS_Acc_Name;
                    worksheet.Cells["M" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["N" + lastRow].Value = i.WTS_Exchange_Rate;
                    worksheet.Cells["N" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["O" + lastRow].Value = i.WTS_Contra_Currency_Name;
                    worksheet.Cells["O" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["P" + lastRow].Value = i.WTS_Fund;
                    worksheet.Cells["P" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["Q" + lastRow].Value = i.WTS_Advice_Print;
                    worksheet.Cells["Q" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["R" + lastRow].Value = i.WTS_Details;
                    worksheet.Cells["R" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["S" + lastRow].Value = i.WTS_Entity;
                    worksheet.Cells["S" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["T" + lastRow].Value = i.WTS_Division;
                    worksheet.Cells["T" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["U" + lastRow].Value = i.WTS_Inter_Amount;
                    worksheet.Cells["U" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    worksheet.Cells["V" + lastRow].Value = i.WTS_Inter_Rate;
                    worksheet.Cells["V" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                }
                dataEndRow = lastRow;
                package.Save();
            }
        }
    }
}
