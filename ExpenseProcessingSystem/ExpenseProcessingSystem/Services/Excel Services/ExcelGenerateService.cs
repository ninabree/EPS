using ExpenseProcessingSystem.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

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
                case ConstantData.HomeReportConstantValue.TransListReport:
                    ExcelTransactionList(newFile, templateFile, data);
                    break;
                case ConstantData.HomeReportConstantValue.AccSummaryReport:
                    ExcelAccountSummary(newFile, templateFile, data);
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

        public void ExcelTransactionList(FileInfo newFile, FileInfo templateFile, HomeReportDataFilterViewModel data)
        {
            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int lastRow = worksheet.Dimension.End.Row + 1;

                //Content
                foreach (var i in data.HomeReportOutputTransactionList)
                {
                    worksheet.Cells["A" + lastRow].Value = i.Trans_Voucher_Number;
                    worksheet.Cells["B" + lastRow].Value = i.Trans_Check_Number;
                    worksheet.Cells["C" + lastRow].Value = i.Trans_Value_Date;
                    worksheet.Cells["D" + lastRow].Value = i.Trans_Reference_No;
                    worksheet.Cells["E" + lastRow].Value = i.Trans_Section;
                    worksheet.Cells["F" + lastRow].Value = i.Trans_Remarks;
                    worksheet.Cells["G" + lastRow].Value = i.Trans_DebitCredit1_1;
                    worksheet.Cells["H" + lastRow].Value = i.Trans_Currency1_1;
                    worksheet.Cells["I" + lastRow].Value = i.Trans_Amount1_1;
                    worksheet.Cells["J" + lastRow].Value = i.Trans_Customer1_1;
                    worksheet.Cells["K" + lastRow].Value = i.Trans_Account_Code1_1;
                    worksheet.Cells["L" + lastRow].Value = i.Trans_Account_Number1_1;
                    worksheet.Cells["M" + lastRow].Value = i.Trans_Account_Name1_1;
                    worksheet.Cells["N" + lastRow].Value = i.Trans_Exchange_Rate1_1;
                    worksheet.Cells["O" + lastRow].Value = i.Trans_Contra_Currency1_1;
                    worksheet.Cells["P" + lastRow].Value = i.Trans_Fund1_1;
                    worksheet.Cells["Q" + lastRow].Value = i.Trans_Advice_Print1_1;
                    worksheet.Cells["R" + lastRow].Value = i.Trans_Details1_1;
                    worksheet.Cells["S" + lastRow].Value = i.Trans_Entity1_1;
                    worksheet.Cells["T" + lastRow].Value = i.Trans_Division1_1;
                    worksheet.Cells["U" + lastRow].Value = i.Trans_InterAmount1_1;
                    worksheet.Cells["V" + lastRow].Value = i.Trans_InterRate1_1;
                    worksheet.Cells["W" + lastRow].Value = i.Trans_DebitCredit1_2;
                    worksheet.Cells["X" + lastRow].Value = i.Trans_Currency1_2;
                    worksheet.Cells["Y" + lastRow].Value = i.Trans_Amount1_2;
                    worksheet.Cells["Z" + lastRow].Value = i.Trans_Customer1_2;
                    worksheet.Cells["AA" + lastRow].Value = i.Trans_Account_Code1_2;
                    worksheet.Cells["AB" + lastRow].Value = i.Trans_Account_Number1_2;
                    worksheet.Cells["AC" + lastRow].Value = i.Trans_Account_Name1_2;
                    worksheet.Cells["AD" + lastRow].Value = i.Trans_Exchange_Rate1_2;
                    worksheet.Cells["AE" + lastRow].Value = i.Trans_Contra_Currency1_2;
                    worksheet.Cells["AF" + lastRow].Value = i.Trans_Fund1_2;
                    worksheet.Cells["AG" + lastRow].Value = i.Trans_Advice_Print1_2;
                    worksheet.Cells["AH" + lastRow].Value = i.Trans_Details1_2;
                    worksheet.Cells["AI" + lastRow].Value = i.Trans_Entity1_2;
                    worksheet.Cells["AJ" + lastRow].Value = i.Trans_Division1_2;
                    worksheet.Cells["AK" + lastRow].Value = i.Trans_InterAmount1_2;
                    worksheet.Cells["AL" + lastRow].Value = i.Trans_InterRate1_2;
                    worksheet.Cells["AM" + lastRow].Value = i.Trans_DebitCredit2_1;
                    worksheet.Cells["AN" + lastRow].Value = i.Trans_Currency2_1;
                    worksheet.Cells["AO" + lastRow].Value = i.Trans_Amount2_1;
                    worksheet.Cells["AP" + lastRow].Value = i.Trans_Customer2_1;
                    worksheet.Cells["AQ" + lastRow].Value = i.Trans_Account_Code2_1;
                    worksheet.Cells["AR" + lastRow].Value = i.Trans_Account_Number2_1;
                    worksheet.Cells["AS" + lastRow].Value = i.Trans_Account_Name2_1;
                    worksheet.Cells["AT" + lastRow].Value = i.Trans_Exchange_Rate2_1;
                    worksheet.Cells["AU" + lastRow].Value = i.Trans_Contra_Currency2_1;
                    worksheet.Cells["AV" + lastRow].Value = i.Trans_Fund2_1;
                    worksheet.Cells["AW" + lastRow].Value = i.Trans_Advice_Print2_1;
                    worksheet.Cells["AX" + lastRow].Value = i.Trans_Details2_1;
                    worksheet.Cells["AY" + lastRow].Value = i.Trans_Entity2_1;
                    worksheet.Cells["AZ" + lastRow].Value = i.Trans_Division2_1;
                    worksheet.Cells["BA" + lastRow].Value = i.Trans_InterAmount2_1;
                    worksheet.Cells["BB" + lastRow].Value = i.Trans_InterRate2_1;
                    worksheet.Cells["BC" + lastRow].Value = i.Trans_DebitCredit2_2;
                    worksheet.Cells["BD" + lastRow].Value = i.Trans_Currency2_2;
                    worksheet.Cells["BE" + lastRow].Value = i.Trans_Amount2_2;
                    worksheet.Cells["BF" + lastRow].Value = i.Trans_Customer2_2;
                    worksheet.Cells["BG" + lastRow].Value = i.Trans_Account_Code2_2;
                    worksheet.Cells["BH" + lastRow].Value = i.Trans_Account_Number2_2;
                    worksheet.Cells["BI" + lastRow].Value = i.Trans_Account_Name2_2;
                    worksheet.Cells["BJ" + lastRow].Value = i.Trans_Exchange_Rate2_2;
                    worksheet.Cells["BK" + lastRow].Value = i.Trans_Contra_Currency2_2;
                    worksheet.Cells["BL" + lastRow].Value = i.Trans_Fund2_2;
                    worksheet.Cells["BM" + lastRow].Value = i.Trans_Advice_Print2_2;
                    worksheet.Cells["BN" + lastRow].Value = i.Trans_Details2_2;
                    worksheet.Cells["BO" + lastRow].Value = i.Trans_Entity2_2;
                    worksheet.Cells["BP" + lastRow].Value = i.Trans_Division2_2;
                    worksheet.Cells["BQ" + lastRow].Value = i.Trans_InterAmount2_2;
                    worksheet.Cells["BR" + lastRow].Value = i.Trans_InterRate2_2;
                    worksheet.Cells["BS" + lastRow].Value = i.Trans_DebitCredit3_1;
                    worksheet.Cells["BT" + lastRow].Value = i.Trans_Currency3_1;
                    worksheet.Cells["BU" + lastRow].Value = i.Trans_Amount3_1;
                    worksheet.Cells["BV" + lastRow].Value = i.Trans_Customer3_1;
                    worksheet.Cells["BW" + lastRow].Value = i.Trans_Account_Code3_1;
                    worksheet.Cells["BX" + lastRow].Value = i.Trans_Account_Number3_1;
                    worksheet.Cells["BY" + lastRow].Value = i.Trans_Account_Name3_1;
                    worksheet.Cells["BZ" + lastRow].Value = i.Trans_Exchange_Rate3_1;
                    worksheet.Cells["CA" + lastRow].Value = i.Trans_Contra_Currency3_1;
                    worksheet.Cells["CB" + lastRow].Value = i.Trans_Fund3_1;
                    worksheet.Cells["CC" + lastRow].Value = i.Trans_Advice_Print3_1;
                    worksheet.Cells["CD" + lastRow].Value = i.Trans_Details3_1;
                    worksheet.Cells["CE" + lastRow].Value = i.Trans_Entity3_1;
                    worksheet.Cells["CF" + lastRow].Value = i.Trans_Division3_1;
                    worksheet.Cells["CG" + lastRow].Value = i.Trans_InterAmount3_1;
                    worksheet.Cells["CH" + lastRow].Value = i.Trans_InterRate3_1;
                    worksheet.Cells["CI" + lastRow].Value = i.Trans_DebitCredit3_2;
                    worksheet.Cells["CJ" + lastRow].Value = i.Trans_Currency3_2;
                    worksheet.Cells["CK" + lastRow].Value = i.Trans_Amount3_2;
                    worksheet.Cells["CL" + lastRow].Value = i.Trans_Customer3_2;
                    worksheet.Cells["CM" + lastRow].Value = i.Trans_Account_Code3_2;
                    worksheet.Cells["CN" + lastRow].Value = i.Trans_Account_Number3_2;
                    worksheet.Cells["CO" + lastRow].Value = i.Trans_Account_Name3_2;
                    worksheet.Cells["CP" + lastRow].Value = i.Trans_Exchange_Rate3_2;
                    worksheet.Cells["CQ" + lastRow].Value = i.Trans_Contra_Currency3_2;
                    worksheet.Cells["CR" + lastRow].Value = i.Trans_Fund3_2;
                    worksheet.Cells["CS" + lastRow].Value = i.Trans_Advice_Print3_2;
                    worksheet.Cells["CT" + lastRow].Value = i.Trans_Details3_2;
                    worksheet.Cells["CU" + lastRow].Value = i.Trans_Entity3_2;
                    worksheet.Cells["CV" + lastRow].Value = i.Trans_Division3_2;
                    worksheet.Cells["CW" + lastRow].Value = i.Trans_InterAmount3_2;
                    worksheet.Cells["CX" + lastRow].Value = i.Trans_InterRate3_2;
                    worksheet.Cells["CY" + lastRow].Value = i.Trans_DebitCredit4_1;
                    worksheet.Cells["CZ" + lastRow].Value = i.Trans_Currency4_1;
                    worksheet.Cells["DA" + lastRow].Value = i.Trans_Amount4_1;
                    worksheet.Cells["DB" + lastRow].Value = i.Trans_Customer4_1;
                    worksheet.Cells["DC" + lastRow].Value = i.Trans_Account_Code4_1;
                    worksheet.Cells["DD" + lastRow].Value = i.Trans_Account_Number4_1;
                    worksheet.Cells["DE" + lastRow].Value = i.Trans_Account_Name4_1;
                    worksheet.Cells["DF" + lastRow].Value = i.Trans_Exchange_Rate4_1;
                    worksheet.Cells["DG" + lastRow].Value = i.Trans_Contra_Currency4_1;
                    worksheet.Cells["DH" + lastRow].Value = i.Trans_Fund4_1;
                    worksheet.Cells["DI" + lastRow].Value = i.Trans_Advice_Print4_1;
                    worksheet.Cells["DJ" + lastRow].Value = i.Trans_Details4_1;
                    worksheet.Cells["DK" + lastRow].Value = i.Trans_Entity4_1;
                    worksheet.Cells["DL" + lastRow].Value = i.Trans_Division4_1;
                    worksheet.Cells["DM" + lastRow].Value = i.Trans_InterAmount4_1;
                    worksheet.Cells["DN" + lastRow].Value = i.Trans_InterRate4_1;
                    worksheet.Cells["DO" + lastRow].Value = i.Trans_DebitCredit4_2;
                    worksheet.Cells["DP" + lastRow].Value = i.Trans_Currency4_2;
                    worksheet.Cells["DQ" + lastRow].Value = i.Trans_Amount4_2;
                    worksheet.Cells["DR" + lastRow].Value = i.Trans_Customer4_2;
                    worksheet.Cells["DS" + lastRow].Value = i.Trans_Account_Code4_2;
                    worksheet.Cells["DT" + lastRow].Value = i.Trans_Account_Number4_2;
                    worksheet.Cells["DU" + lastRow].Value = i.Trans_Account_Name4_2;
                    worksheet.Cells["DV" + lastRow].Value = i.Trans_Exchange_Rate4_2;
                    worksheet.Cells["DW" + lastRow].Value = i.Trans_Contra_Currency4_2;
                    worksheet.Cells["DX" + lastRow].Value = i.Trans_Fund4_2;
                    worksheet.Cells["DY" + lastRow].Value = i.Trans_Advice_Print4_2;
                    worksheet.Cells["DZ" + lastRow].Value = i.Trans_Details4_2;
                    worksheet.Cells["EA" + lastRow].Value = i.Trans_Entity4_2;
                    worksheet.Cells["EB" + lastRow].Value = i.Trans_Division4_2;
                    worksheet.Cells["EC" + lastRow].Value = i.Trans_InterAmount4_2;
                    worksheet.Cells["ED" + lastRow].Value = i.Trans_InterRate4_2;

                    lastRow += 1;
                }

                package.Save();
            }
        }

        public void ExcelAccountSummary(FileInfo newFile, FileInfo templateFile, HomeReportDataFilterViewModel data)
        {
            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int lastRow = worksheet.Dimension.End.Row + 1;

                //Content
                foreach (var i in data.HomeReportOutputAccountSummary)
                {
                    worksheet.Cells["A" + lastRow].Value = i.Trans_Voucher_Number;
                    worksheet.Cells["B" + lastRow].Value = i.Trans_Check_Number;
                    worksheet.Cells["C" + lastRow].Value = i.Trans_Value_Date;
                    worksheet.Cells["D" + lastRow].Value = i.Trans_Reference_No;
                    worksheet.Cells["E" + lastRow].Value = i.Trans_Section;
                    worksheet.Cells["F" + lastRow].Value = i.Trans_Remarks;
                    worksheet.Cells["G" + lastRow].Value = i.Trans_DebitCredit;
                    worksheet.Cells["H" + lastRow].Value = i.Trans_Currency;
                    worksheet.Cells["I" + lastRow].Value = i.Trans_Amount;
                    worksheet.Cells["J" + lastRow].Value = i.Trans_Customer;
                    worksheet.Cells["K" + lastRow].Value = i.Trans_Account_Code;
                    worksheet.Cells["L" + lastRow].Value = i.Trans_Account_Number;
                    worksheet.Cells["M" + lastRow].Value = i.Trans_Account_Name;
                    worksheet.Cells["N" + lastRow].Value = i.Trans_Exchange_Rate;
                    worksheet.Cells["O" + lastRow].Value = i.Trans_Contra_Currency;
                    worksheet.Cells["P" + lastRow].Value = i.Trans_Fund;
                    worksheet.Cells["Q" + lastRow].Value = i.Trans_Advice_Print;
                    worksheet.Cells["R" + lastRow].Value = i.Trans_Details;
                    worksheet.Cells["S" + lastRow].Value = i.Trans_Entity;
                    worksheet.Cells["T" + lastRow].Value = i.Trans_Division;
                    worksheet.Cells["U" + lastRow].Value = i.Trans_InterAmount;
                    worksheet.Cells["V" + lastRow].Value = i.Trans_InterRate;

                    lastRow += 1;
                }

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

        public string ExcelCDDIS(CDDISValuesVIewModel data, string newFileName, string excelTemplateName)
        {
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
                        //ws.Name = "WorkSheet_1";
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

                    //REMARKS
                    colpnt = 10;
                    strLength = (data.REMARKS.Length <= 29) ? data.REMARKS.Length : 29;
                    for (int c = 0; c < strLength; c++)
                    {
                        ws.Cells[col[colpnt] + "9"].Value = data.REMARKS.Substring(c, 1);
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
