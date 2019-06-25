using ExpenseProcessingSystem.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
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
                case ConstantData.HomeReportConstantValue.AST1000:

                    ExcelAST1000(newFile, templateFile, data);
                    break;
                case ConstantData.HomeReportConstantValue.ActualBudgetReport:

                        ExcelActualBudget(newFile, templateFile, data);
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

                //Header
                worksheet.Cells["A2"].Value = data.HomeReportFilter.MonthName + " - " + data.HomeReportFilter.Year;
                worksheet.Cells["B3"].Value = data.ReportCommonVM.Header_Name;
                worksheet.Cells["B4"].Value = data.ReportCommonVM.Header_TIN;
                worksheet.Cells["B5"].Value = data.ReportCommonVM.Header_Address;

                //Content
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

                //footer
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = DateTime.Now.ToString("dddd, MMMM dd,yyyy h:mm:sstt");

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

                //Header
                worksheet.Cells["A2"].Value = data.HomeReportFilter.MonthName + " " + data.HomeReportFilter.Year + " - "
                                            + data.HomeReportFilter.MonthNameTo + " " + data.HomeReportFilter.YearTo;
                worksheet.Cells["B3"].Value = data.ReportCommonVM.Header_Name;
                worksheet.Cells["B4"].Value = data.ReportCommonVM.Header_TIN;
                worksheet.Cells["B5"].Value = data.ReportCommonVM.Header_Address;

                //Content
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
                
                //footer
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = DateTime.Now.ToString("dddd, MMMM dd,yyyy h:mm:sstt");

                package.Save();
            }
        }

        public void ExcelActualBudget(FileInfo newFile, FileInfo templateFile, HomeReportDataFilterViewModel data)
        {
            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int lastRow = worksheet.Dimension.End.Row + 1;

                //Header
                worksheet.Cells["A2"].Value = data.HomeReportFilter.MonthName + " " + data.HomeReportFilter.Year;
                worksheet.Cells["B3"].Value = data.ReportCommonVM.Header_Name;
                worksheet.Cells["B4"].Value = data.ReportCommonVM.Header_TIN;
                worksheet.Cells["B5"].Value = data.ReportCommonVM.Header_Address;

                //Content
                foreach (var i in data.HomeReportOutputActualBudget)
                {
                    worksheet.Cells["A" + lastRow + ":F" + lastRow].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["A" + lastRow + ":F" + lastRow].Style.Fill.BackgroundColor.SetColor(Color.White);
                    worksheet.Cells["A" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    if (i.Category == "BREAK")
                    {
                        worksheet.Cells["A" + lastRow + ":F" + lastRow].Merge = true;
                        worksheet.Cells["A" + lastRow].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#484f4f"));
                        worksheet.Cells["A" + lastRow].Style.Font.Color.SetColor(Color.White);
                    }
                    else if (!string.IsNullOrEmpty(i.Category))
                    {
                        worksheet.Cells["A" + lastRow].Value = i.ValueDate;
                        worksheet.Cells["A" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["A" + lastRow].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c8c3cc"));
                        worksheet.Cells["B" + lastRow].Value = i.Category;
                        worksheet.Cells["B" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["B" + lastRow].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c8c3cc"));
                        worksheet.Cells["C" + lastRow].Value = i.Remarks;
                        worksheet.Cells["C" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["C" + lastRow].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c8c3cc"));
                        worksheet.Cells["D" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["D" + lastRow].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c8c3cc"));
                        worksheet.Cells["E" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["E" + lastRow].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c8c3cc"));
                        worksheet.Cells["F" + lastRow].Value = i.BudgetBalance;
                        worksheet.Cells["F" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["F" + lastRow].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c8c3cc"));
                    }
                    else
                    {
                        worksheet.Cells["A" + lastRow].Value = i.ValueDate;
                        worksheet.Cells["A" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["B" + lastRow].Value = i.Category;
                        worksheet.Cells["B" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["C" + lastRow].Value = i.Remarks;
                        worksheet.Cells["C" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["D" + lastRow].Value = i.Department;
                        worksheet.Cells["D" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["E" + lastRow].Value = i.ExpenseAmount;
                        worksheet.Cells["E" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells["F" + lastRow].Value = i.BudgetBalance;
                        worksheet.Cells["F" + lastRow].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    lastRow += 1;
                }

                //footer
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = DateTime.Now.ToString("dddd, MMMM dd,yyyy h:mm:sstt");

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

                //Header
                worksheet.Cells["A2"].Value = data.HomeReportFilter.ReportFrom + " - " + data.HomeReportFilter.ReportTo;
                worksheet.Cells["C3"].Value = data.ReportCommonVM.Header_Name;
                worksheet.Cells["C4"].Value = data.ReportCommonVM.Header_TIN;
                worksheet.Cells["C5"].Value = data.ReportCommonVM.Header_Address;

                //Content
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

                //footer
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = DateTime.Now.ToString("dddd, MMMM dd,yyyy h:mm:sstt");

                package.Save();
            }
        }

        public string ExcelCDDIS(CDDISValuesVIewModel data, string newFileName)
        {
            string excelTemplateName = "CDDIS_template.xlsx";
            string rootFolder = "wwwroot";
            string sourcePath = "/ExcelTemplates/";
            string destPath = "/ExcelTemplatesTempFolder/";
            System.IO.File.Copy(rootFolder + sourcePath + excelTemplateName, rootFolder + destPath + newFileName, true);

            FileInfo templateFile = new FileInfo(rootFolder + sourcePath + excelTemplateName);
            FileInfo newFile = new FileInfo(rootFolder + destPath + newFileName);

            string[] col = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
                "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG",
                "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV",
                "AW", "AX", "AY", "AZ" };
            int colpnt = 0;
            int strLength = 0;
            int count1 = 0;
            int count2 = 1;
            int loopCount = 1;
            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                while (loopCount <= (data.CDDContents.Count / 2))
                {
                    ExcelWorksheet ws;
                    if (loopCount > 1)
                    {
                        ws = package.Workbook.Worksheets.Add("WorkSheet_" + loopCount, package.Workbook.Worksheets[1]);
                    }
                    else
                    {
                        ws = package.Workbook.Worksheets[loopCount];
                        ws.Name = "WorkSheet_1";
                    }

                    //Content
                    ws.Cells["V5"].Value = DateTime.Now.ToShortDateString();

                    //VALUE DATE
                    ws.Cells["E8"].Value = data.VALUE_DATE.Month.ToString("d2").Substring(0, 1);
                    ws.Cells["F8"].Value = data.VALUE_DATE.Month.ToString("d2").Substring(1, 1);
                    ws.Cells["H8"].Value = data.VALUE_DATE.Date.ToString("dd").Substring(0, 1);
                    ws.Cells["I8"].Value = data.VALUE_DATE.Date.ToString("dd").Substring(1, 1);
                    ws.Cells["K8"].Value = data.VALUE_DATE.Year.ToString().Substring(2, 1);
                    ws.Cells["L8"].Value = data.VALUE_DATE.Year.ToString().Substring(3, 1);

                    //REFERENCE NO
                    ws.Cells["T8"].Value = data.REFERENCE_NO.Substring(0, 1);
                    ws.Cells["U8"].Value = data.REFERENCE_NO.Substring(1, 1);
                    ws.Cells["V8"].Value = data.REFERENCE_NO.Substring(2, 1);
                    ws.Cells["X8"].Value = data.REFERENCE_NO.Substring(3, 1);
                    ws.Cells["Y8"].Value = data.REFERENCE_NO.Substring(4, 1);
                    ws.Cells["Z8"].Value = data.REFERENCE_NO.Substring(5, 1);
                    colpnt = 27;
                    for (int c = 6; c < 12; c++)
                    {
                        ws.Cells[col[colpnt] + "8"].Value = data.REFERENCE_NO.Substring(c, 1);
                        colpnt++;
                    }

                    //COMMENT
                    ws.Cells["AL8"].Value = data.COMMENT.Substring(0, 1);
                    ws.Cells["AM8"].Value = data.COMMENT.Substring(1, 1);

                    //SECTION
                    ws.Cells["E9"].Value = data.SECTION.Substring(0, 1);
                    ws.Cells["F9"].Value = data.SECTION.Substring(1, 1);

                    //REMARKS
                    colpnt = 10;
                    strLength = (data.REMARKS.Length <= 29) ? data.REMARKS.Length : 29;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "9"].Value = data.REMARKS.Substring(c, 1);
                        colpnt++;
                    }

                    //SCHEME NO
                    colpnt = 4;
                    strLength = (data.SCHEME_NO.Length <= 12) ? data.SCHEME_NO.Length : 12;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "10"].Value = data.SCHEME_NO.Substring(c, 1);
                        colpnt++;
                    }

                    //MEMO
                    ws.Cells["AM10"].Value = data.MEMO;

                    //DEBIT/CREDIT 1 and 2
                    ws.Cells["E14"].Value = data.CDDContents[count1].DEBIT_CREDIT;
                    ws.Cells["E25"].Value = data.CDDContents[count2].DEBIT_CREDIT;

                    //CCY 1 & 2
                    colpnt = 12;
                    strLength = (data.CDDContents[count1].CCY.Length <= 4) ? data.CDDContents[count1].CCY.Length : 4;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "14"].Value = data.CDDContents[count1].CCY.Substring(c, 1);
                        colpnt++;
                    }
                    colpnt = 12;
                    strLength = (data.CDDContents[count2].CCY.Length <= 4) ? data.CDDContents[count2].CCY.Length : 4;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "25"].Value = data.CDDContents[count2].CCY.Substring(c, 1);
                        colpnt++;
                    }

                    //AMOUNT 1 & 2
                    string amount_1 = String.Format("{0:#,##0.##}", data.CDDContents[count1].AMOUNT);
                    string amount_2 = String.Format("{0:#,##0.##}", data.CDDContents[count2].AMOUNT);

                    colpnt = 23;
                    strLength = (amount_1.Length <= 16) ? amount_1.Length : 16;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "14"].Value = amount_1.Substring(c, 1);
                        colpnt++;
                    }
                    colpnt = 23;
                    strLength = (amount_2.Length <= 16) ? amount_2.Length : 16;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "25"].Value = amount_2.Substring(c, 1);
                        colpnt++;
                    }

                    //CUSTOMER ABBR 1 & 2
                    colpnt = 4;
                    strLength = (data.CDDContents[count1].CUSTOMER_ABBR.Length <= 12) ? data.CDDContents[count1].CUSTOMER_ABBR.Length : 12;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "15"].Value = data.CDDContents[count1].CUSTOMER_ABBR.Substring(c, 1);
                        colpnt++;
                    }
                    colpnt = 4;
                    strLength = (data.CDDContents[count2].CUSTOMER_ABBR.Length <= 12) ? data.CDDContents[count2].CUSTOMER_ABBR.Length : 12;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "26"].Value = data.CDDContents[count2].CUSTOMER_ABBR.Substring(c, 1);
                        colpnt++;
                    }

                    //ACCOUNT CODE 1 & 2
                    colpnt = 4;
                    strLength = (data.CDDContents[count1].ACCOUNT_CODE.Length <= 5) ? data.CDDContents[count1].ACCOUNT_CODE.Length : 5;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "16"].Value = data.CDDContents[count1].ACCOUNT_CODE.Substring(c, 1);
                        colpnt++;
                    }
                    colpnt = 4;
                    strLength = (data.CDDContents[count2].ACCOUNT_CODE.Length <= 5) ? data.CDDContents[count2].ACCOUNT_CODE.Length : 5;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "27"].Value = data.CDDContents[count2].ACCOUNT_CODE.Substring(c, 1);
                        colpnt++;
                    }

                    //ACCOUNT NO 1 & 2
                    ws.Cells["Z16"].Value = data.CDDContents[count1].ACCOUNT_NO.Substring(0, 1);
                    ws.Cells["AA16"].Value = data.CDDContents[count1].ACCOUNT_NO.Substring(1, 1);
                    ws.Cells["AB16"].Value = data.CDDContents[count1].ACCOUNT_NO.Substring(2, 1);
                    ws.Cells["AD16"].Value = data.CDDContents[count1].ACCOUNT_NO.Substring(3, 1);
                    ws.Cells["AE16"].Value = data.CDDContents[count1].ACCOUNT_NO.Substring(4, 1);
                    ws.Cells["AF16"].Value = data.CDDContents[count1].ACCOUNT_NO.Substring(5, 1);
                    colpnt = 33;
                    strLength = (data.CDDContents[count1].ACCOUNT_NO.Length <= 12) ? data.CDDContents[count1].ACCOUNT_NO.Length : 12;
                    for (int c = 6; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "16"].Value = data.CDDContents[count1].ACCOUNT_NO.Substring(c, 1);
                        colpnt++;
                    }

                    ws.Cells["Z27"].Value = data.CDDContents[count2].ACCOUNT_NO.Substring(0, 1);
                    ws.Cells["AA27"].Value = data.CDDContents[count2].ACCOUNT_NO.Substring(1, 1);
                    ws.Cells["AB27"].Value = data.CDDContents[count2].ACCOUNT_NO.Substring(2, 1);
                    ws.Cells["AD27"].Value = data.CDDContents[count2].ACCOUNT_NO.Substring(3, 1);
                    ws.Cells["AE27"].Value = data.CDDContents[count2].ACCOUNT_NO.Substring(4, 1);
                    ws.Cells["AF27"].Value = data.CDDContents[count2].ACCOUNT_NO.Substring(5, 1);
                    colpnt = 33;
                    strLength = (data.CDDContents[count2].ACCOUNT_NO.Length <= 12) ? data.CDDContents[count2].ACCOUNT_NO.Length : 12;
                    for (int c = 6; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "27"].Value = data.CDDContents[count2].ACCOUNT_NO.Substring(c, 1);
                        colpnt++;
                    }

                    //EXCHANGE RATE 1 & 2
                    string exrate_1 = String.Format("{0:#,##0.####}", data.CDDContents[count1].EXCHANGE_RATE);
                    string exrate_2 = String.Format("{0:#,##0.####}", data.CDDContents[count2].EXCHANGE_RATE);

                    colpnt = 4;
                    strLength = (exrate_1.Length <= 10) ? exrate_1.Length : 10;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "17"].Value = exrate_1.Substring(c, 1);
                        colpnt++;
                    }
                    colpnt = 4;
                    strLength = (exrate_2.Length <= 10) ? exrate_2.Length : 10;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "28"].Value = exrate_2.Substring(c, 1);
                        colpnt++;
                    }

                    //CONTRA CCY 1 & 2
                    colpnt = 25;
                    strLength = (data.CDDContents[count1].CONTRA_CCY.Length <= 4) ? data.CDDContents[count1].CONTRA_CCY.Length : 4;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "17"].Value = data.CDDContents[count1].CONTRA_CCY.Substring(c, 1);
                        colpnt++;
                    }

                    colpnt = 25;
                    strLength = (data.CDDContents[count2].CONTRA_CCY.Length <= 4) ? data.CDDContents[count2].CONTRA_CCY.Length : 4;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "28"].Value = data.CDDContents[count2].CONTRA_CCY.Substring(c, 1);
                        colpnt++;
                    }

                    //FUND 1 & 2
                    ws.Cells["E18"].Value = data.CDDContents[count1].FUND.Substring(0, 1);
                    ws.Cells["E29"].Value = data.CDDContents[count2].FUND.Substring(0, 1);

                    //CHECK NO 1 & 2
                    colpnt = 11;
                    strLength = (data.CDDContents[count1].CHECK_NO.Length <= 10) ? data.CDDContents[count1].CHECK_NO.Length : 10;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "18"].Value = data.CDDContents[count1].CHECK_NO.Substring(c, 1);
                        colpnt++;
                    }

                    colpnt = 11;
                    strLength = (data.CDDContents[count2].CHECK_NO.Length <= 10) ? data.CDDContents[count2].CHECK_NO.Length : 10;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "29"].Value = data.CDDContents[count2].CHECK_NO.Substring(c, 1);
                        colpnt++;
                    }

                    //AVAILABLE(DATE) 1 & 2
                    ws.Cells["AH18"].Value = data.CDDContents[count1].AVAILABLE_DATE.Month.ToString("d2").Substring(0, 1);
                    ws.Cells["AI18"].Value = data.CDDContents[count1].AVAILABLE_DATE.Month.ToString("d2").Substring(1, 1);
                    ws.Cells["AJ18"].Value = data.CDDContents[count1].AVAILABLE_DATE.Date.ToString("dd").Substring(0, 1);
                    ws.Cells["AK18"].Value = data.CDDContents[count1].AVAILABLE_DATE.Date.ToString("dd").Substring(1, 1);
                    ws.Cells["AL18"].Value = data.CDDContents[count1].AVAILABLE_DATE.Year.ToString().Substring(2, 1);
                    ws.Cells["AM18"].Value = data.CDDContents[count1].AVAILABLE_DATE.Year.ToString().Substring(3, 1);

                    ws.Cells["AH29"].Value = data.CDDContents[count2].AVAILABLE_DATE.Month.ToString("d2").Substring(0, 1);
                    ws.Cells["AI29"].Value = data.CDDContents[count2].AVAILABLE_DATE.Month.ToString("d2").Substring(1, 1);
                    ws.Cells["AJ29"].Value = data.CDDContents[count2].AVAILABLE_DATE.Date.ToString("dd").Substring(0, 1);
                    ws.Cells["AK29"].Value = data.CDDContents[count2].AVAILABLE_DATE.Date.ToString("dd").Substring(1, 1);
                    ws.Cells["AL29"].Value = data.CDDContents[count2].AVAILABLE_DATE.Year.ToString().Substring(2, 1);
                    ws.Cells["AM29"].Value = data.CDDContents[count2].AVAILABLE_DATE.Year.ToString().Substring(3, 1);

                    //ADVICE PRINT 1 & 2
                    ws.Cells["E19"].Value = data.CDDContents[count1].ADVICE.Substring(0, 1);
                    ws.Cells["E30"].Value = data.CDDContents[count2].ADVICE.Substring(0, 1);

                    //DETAILS 1 & 2
                    colpnt = 9;
                    strLength = (data.CDDContents[count1].DETAILS.Length <= 30) ? data.CDDContents[count1].DETAILS.Length : 30;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "19"].Value = data.CDDContents[count1].DETAILS.Substring(c, 1);
                        colpnt++;
                    }

                    colpnt = 9;
                    strLength = (data.CDDContents[count2].DETAILS.Length <= 30) ? data.CDDContents[count2].DETAILS.Length : 30;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "30"].Value = data.CDDContents[count2].DETAILS.Substring(c, 1);
                        colpnt++;
                    }

                    //ENTITY 1 & 2
                    ws.Cells["E20"].Value = data.CDDContents[count1].ENTITY.Substring(0, 1);
                    ws.Cells["F20"].Value = data.CDDContents[count1].ENTITY.Substring(1, 1);
                    ws.Cells["G20"].Value = data.CDDContents[count1].ENTITY.Substring(2, 1);

                    ws.Cells["E31"].Value = data.CDDContents[count2].ENTITY.Substring(0, 1);
                    ws.Cells["F31"].Value = data.CDDContents[count2].ENTITY.Substring(1, 1);
                    ws.Cells["G31"].Value = data.CDDContents[count2].ENTITY.Substring(2, 1);

                    //DIVISION 1 & 2
                    ws.Cells["M20"].Value = data.CDDContents[count1].DIVISION.Substring(0, 1);
                    ws.Cells["N20"].Value = data.CDDContents[count1].DIVISION.Substring(1, 1);

                    ws.Cells["M31"].Value = data.CDDContents[count2].DIVISION.Substring(0, 1);
                    ws.Cells["N31"].Value = data.CDDContents[count2].DIVISION.Substring(1, 1);

                    //INTER AMOUNT 1 & 2
                    string interamount_1 = String.Format("{0:#,##0.##}", data.CDDContents[count1].INTER_AMOUNT);
                    string interamount_2 = String.Format("{0:#,##0.##}", data.CDDContents[count2].INTER_AMOUNT);

                    colpnt = 23;
                    strLength = (interamount_1.Length <= 16) ? interamount_1.Length : 16;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "20"].Value = interamount_1.Substring(c, 1);
                        colpnt++;
                    }
                    colpnt = 23;
                    strLength = (interamount_2.Length <= 16) ? interamount_2.Length : 16;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "31"].Value = interamount_2.Substring(c, 1);
                        colpnt++;
                    }

                    //INTER RATE 1 & 2
                    string interrate_1 = String.Format("{0:#,##0.####}", data.CDDContents[count1].INTER_RATE);
                    string interrate_2 = String.Format("{0:#,##0.####}", data.CDDContents[count2].INTER_RATE);

                    colpnt = 4;
                    strLength = (interrate_1.Length <= 10) ? interrate_1.Length : 10;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "21"].Value = interrate_1.Substring(c, 1);
                        colpnt++;
                    }
                    colpnt = 4;
                    strLength = (interrate_2.Length <= 10) ? interrate_2.Length : 10;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "32"].Value = interrate_2.Substring(c, 1);
                        colpnt++;
                    }

                    package.Save();
                    count1 += 2;
                    count2 += 2;
                    loopCount++;
                }
            }
            return destPath + newFileName;
        }
    }
}
