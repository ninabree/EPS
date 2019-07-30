using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BIR_Form_Filler.Models;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing;
using System.Drawing;

namespace BIR_Form_Filler.Functions
{
    public class BIRExcelCreator
    {

        public bool GenerateBIRExcelFile()
        {
            FileInfo newFile = new FileInfo("C:/Temp/NeuFile/neufile.xlsx");
            FileInfo template = new FileInfo("C:/Temp/NeuFile/Template/bir_template.xlsx");
            BirFromTo bft = new BirFromTo();
            BirPayeeInfo bpi = new BirPayeeInfo();
            BirPayorInfo bpoi = new BirPayorInfo();
            using (ExcelPackage package = new ExcelPackage(newFile, template))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                   //           var tryShape = (ExcelShape) worksheet.Drawings["TextBox 1"];
                   //           string val = tryShape.Text;
                   //           tryShape.Text = "Say Hello To Me";
                SetForThePeriod(worksheet, bft);
                SetPayeeInfo(worksheet, bpi);
                SetPayorInfo(worksheet, bpoi);
                package.Save();
                return true;
            }

        }

        private void SetForThePeriod(ExcelWorksheet worksheet, BirFromTo bft)
        {
            // For The Period --- From
            // MM
            var forTPFMM = worksheet.Drawings.AddShape("FromMM", eShapeStyle.Rect);
            DrawTextBox(forTPFMM, 38, 28, 105, 121, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "1 2", eTextAlignment.Center);
            var forTPFMML = worksheet.Drawings.AddShape("FromMML", eShapeStyle.Line);
            DrawLine(forTPFMML, 0, 4, 124, 144, 1, Color.Black);
            // DD
            var forTPFDD = worksheet.Drawings.AddShape("FromDD", eShapeStyle.Rect);
            DrawTextBox(forTPFDD, 38, 28, 143, 121, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "2 4", eTextAlignment.Center);
            var forTPFDDL = worksheet.Drawings.AddShape("FromDDL", eShapeStyle.Line);
            DrawLine(forTPFDDL, 0, 4, 162, 144, 1, Color.Black);
            // YY
            var forTPFYY = worksheet.Drawings.AddShape("FromYY", eShapeStyle.Rect);
            DrawTextBox(forTPFYY, 38, 28, 181, 121, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "1 9", eTextAlignment.Center);
            var forTPFYYL = worksheet.Drawings.AddShape("FromYYL", eShapeStyle.Line);
            DrawLine(forTPFYYL, 0, 4, 200, 144, 1, Color.Black);

            // -- To
            // MM
            var forTPTMM = worksheet.Drawings.AddShape("ToMM", eShapeStyle.Rect);
            DrawTextBox(forTPTMM, 38, 28, 416, 121, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "0 6", eTextAlignment.Center);
            var forTPTMML = worksheet.Drawings.AddShape("ToMML", eShapeStyle.Line);
            DrawLine(forTPTMML, 0, 4, 435, 144, 1, Color.Black);
            // DD
            var forTPTDD = worksheet.Drawings.AddShape("ToDD", eShapeStyle.Rect);
            DrawTextBox(forTPTDD, 38, 28, 454, 121, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "2 2", eTextAlignment.Center);
            var forTPTDDL = worksheet.Drawings.AddShape("ToDDL", eShapeStyle.Line);
            DrawLine(forTPTDDL, 0, 4, 473, 144, 1, Color.Black);
            // YY
            var forTPTYY = worksheet.Drawings.AddShape("ToYY", eShapeStyle.Rect);
            DrawTextBox(forTPTYY, 38, 28, 492, 121, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "2 0", eTextAlignment.Center);
            var forTPTYYL = worksheet.Drawings.AddShape("ToYYL", eShapeStyle.Line);
            DrawLine(forTPTYYL, 0, 4, 511, 144, 1, Color.Black);

        }



        private void SetPayeeInfo(ExcelWorksheet worksheet, BirPayeeInfo bpi)
        {
            //Payee Information
            //-- Tax Identification Number
            SetTIN(worksheet, bpi.Tin);
            //-- Payee Name
            var payeeN = worksheet.Drawings.AddShape("payeeN", eShapeStyle.Rect);
            DrawTextBox(payeeN, 661, 24, 129, 202, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F),
                        "This Is A Sample Of A Long Company Name (TIASALCN), Inc.",
                        eTextAlignment.Center);
            //-- Registered Address
            // Address
            var regA = worksheet.Drawings.AddShape("regAddress", eShapeStyle.Rect);
            DrawTextBox(regA, 494, 25, 129, 240, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 8F),
                        "#12 Delta Alpha Gamma St., Omega Zetta Epsilon Subd., Neu Sigma Lambda City, Lingua",
                        eTextAlignment.Center);
            // Zip Code
            var zipRA = worksheet.Drawings.AddShape("zipRegAddEEI", eShapeStyle.Rect);
            DrawTextBox(zipRA, 83, 25, 709, 240, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "1   2   3   4", eTextAlignment.Center);
            var zipRAL1 = worksheet.Drawings.AddShape("zipRegAddEEIL1", eShapeStyle.Line);
            DrawLine(zipRAL1, 0, 4, 730, 261, 1, Color.Black);
            var zipRAL2 = worksheet.Drawings.AddShape("zipRegAddEEIL2", eShapeStyle.Line);
            DrawLine(zipRAL2, 0, 4, 751, 261, 1, Color.Black);
            var zipRAL3 = worksheet.Drawings.AddShape("zipRegAddEEIL3", eShapeStyle.Line);
            DrawLine(zipRAL3, 0, 4, 772, 261, 1, Color.Black);
            //-- Foreign Address
            // Address
            var forA = worksheet.Drawings.AddShape("forAddress", eShapeStyle.Rect);
            DrawTextBox(forA, 494, 25, 129, 269, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 8F),
                        @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                        eTextAlignment.Center);
            // Zip Code
            var zipFA = worksheet.Drawings.AddShape("zipForAddEEI", eShapeStyle.Rect);
            DrawTextBox(zipFA, 83, 25, 709, 269, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "1   2   3   4", eTextAlignment.Center);
            var zipFAL1 = worksheet.Drawings.AddShape("zipForAddEEIL1", eShapeStyle.Line);
            DrawLine(zipFAL1, 0, 4, 731, 289, 1, Color.Black);
            var zipFAL2 = worksheet.Drawings.AddShape("zipForAddEEIL2", eShapeStyle.Line);
            DrawLine(zipFAL2, 0, 4, 752, 289, 1, Color.Black);
            var zipFAL3 = worksheet.Drawings.AddShape("zipForAddEEIL3", eShapeStyle.Line);
            DrawLine(zipFAL3, 0, 4, 773, 289, 1, Color.Black);

        }


        private void SetPayorInfo(ExcelWorksheet worksheet, BirPayorInfo bpi)
        {
            //Payee Information
            //-- Tax Identification Number
            SetTINORI(worksheet, bpi.Tin);
            //-- Payor Name
            var payorN = worksheet.Drawings.AddShape("payorN", eShapeStyle.Rect);
            DrawTextBox(payorN, 661, 24, 129, 344, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F),
                        "This Is A Sample Of A Long Company Name (TIASALCN), Inc.",
                        eTextAlignment.Center);
            //-- Registered Address
            // Address
            var regA = worksheet.Drawings.AddShape("regAddressORI", eShapeStyle.Rect);
            DrawTextBox(regA, 494, 25, 129, 384, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 8F),
                        "#12 Delta Alpha Gamma St., Omega Zetta Epsilon Subd., Neu Sigma Lambda City, Lingua",
                        eTextAlignment.Center);
            // Zip Code
            var zipRA = worksheet.Drawings.AddShape("zipRegAddORI", eShapeStyle.Rect);
            DrawTextBox(zipRA, 83, 25, 709, 384, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "1   2   3   4", eTextAlignment.Center);
            var zipRAL1 = worksheet.Drawings.AddShape("zipRegAddORIL1", eShapeStyle.Line);
            DrawLine(zipRAL1, 0, 4, 730, 404, 1, Color.Black);
            var zipRAL2 = worksheet.Drawings.AddShape("zipRegAddORIL2", eShapeStyle.Line);
            DrawLine(zipRAL2, 0, 4, 751, 404, 1, Color.Black);
            var zipRAL3 = worksheet.Drawings.AddShape("zipRegAddORIL3", eShapeStyle.Line);
            DrawLine(zipRAL3, 0, 4, 772, 404, 1, Color.Black);

        }



        private void SetTIN(ExcelWorksheet worksheet, string TIN)
        {
            // T1
            var taxT1 = worksheet.Drawings.AddShape("taxT1", eShapeStyle.Rect);
            DrawTextBox(taxT1, 49, 24, 129, 173, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "9 9 9", eTextAlignment.Center);
            var taxT1L1 = worksheet.Drawings.AddShape("taxT1L1", eShapeStyle.Line);
            DrawLine(taxT1L1, 0, 4, 145, 193, 1, Color.Black);
            var taxT1L2 = worksheet.Drawings.AddShape("taxT1L2", eShapeStyle.Line);
            DrawLine(taxT1L2, 0, 4, 161, 193, 1, Color.Black);
            // T2
            var taxT2 = worksheet.Drawings.AddShape("taxT2", eShapeStyle.Rect);
            DrawTextBox(taxT2, 49, 24, 194, 173, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "9 9 9", eTextAlignment.Center);
            var taxT2L1 = worksheet.Drawings.AddShape("taxT2L1", eShapeStyle.Line);
            DrawLine(taxT2L1, 0, 4, 210, 193, 1, Color.Black);
            var taxT2L2 = worksheet.Drawings.AddShape("taxT2L2", eShapeStyle.Line);
            DrawLine(taxT2L2, 0, 4, 226, 193, 1, Color.Black);
            // T3
            var taxT3 = worksheet.Drawings.AddShape("taxT3", eShapeStyle.Rect);
            DrawTextBox(taxT3, 49, 24, 259, 173, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "9 9 9", eTextAlignment.Center);
            var taxT3L1 = worksheet.Drawings.AddShape("taxT3L1", eShapeStyle.Line);
            DrawLine(taxT3L1, 0, 4, 275, 193, 1, Color.Black);
            var taxT3L2 = worksheet.Drawings.AddShape("taxT3L2", eShapeStyle.Line);
            DrawLine(taxT3L2, 0, 4, 291, 193, 1, Color.Black);
            // T4
            var taxT4 = worksheet.Drawings.AddShape("taxT4", eShapeStyle.Rect);
            DrawTextBox(taxT4, 49, 24, 324, 173, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "9 9 9", eTextAlignment.Center);
            var taxT4L1 = worksheet.Drawings.AddShape("taxT4L1", eShapeStyle.Line);
            DrawLine(taxT4L1, 0, 4, 340, 193, 1, Color.Black);
            var taxT4L2 = worksheet.Drawings.AddShape("taxT4L2", eShapeStyle.Line);
            DrawLine(taxT4L2, 0, 4, 356, 193, 1, Color.Black);

        }

        private void SetTINORI(ExcelWorksheet worksheet, string TIN)
        {
            // T1
            var taxT1 = worksheet.Drawings.AddShape("taxORIT1", eShapeStyle.Rect);
            DrawTextBox(taxT1, 49, 24, 129, 316, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "9 9 9", eTextAlignment.Center);
            var taxT1L1 = worksheet.Drawings.AddShape("taxORIT1L1", eShapeStyle.Line);
            DrawLine(taxT1L1, 0, 4, 145, 336, 1, Color.Black);
            var taxT1L2 = worksheet.Drawings.AddShape("taxORIT1L2", eShapeStyle.Line);
            DrawLine(taxT1L2, 0, 4, 161, 336, 1, Color.Black);
            // T2
            var taxT2 = worksheet.Drawings.AddShape("taxORIT2", eShapeStyle.Rect);
            DrawTextBox(taxT2, 49, 24, 194, 316, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "9 9 9", eTextAlignment.Center);
            var taxT2L1 = worksheet.Drawings.AddShape("taxORIT2L1", eShapeStyle.Line);
            DrawLine(taxT2L1, 0, 4, 210, 336, 1, Color.Black);
            var taxT2L2 = worksheet.Drawings.AddShape("taxORIT2L2", eShapeStyle.Line);
            DrawLine(taxT2L2, 0, 4, 226, 336, 1, Color.Black);
            // T3
            var taxT3 = worksheet.Drawings.AddShape("taxORIT3", eShapeStyle.Rect);
            DrawTextBox(taxT3, 49, 24, 259, 316, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "9 9 9", eTextAlignment.Center);
            var taxT3L1 = worksheet.Drawings.AddShape("taxORIT3L1", eShapeStyle.Line);
            DrawLine(taxT3L1, 0, 4, 275, 336, 1, Color.Black);
            var taxT3L2 = worksheet.Drawings.AddShape("taxORIT3L2", eShapeStyle.Line);
            DrawLine(taxT3L2, 0, 4, 291, 336, 1, Color.Black);
            // T4
            var taxT4 = worksheet.Drawings.AddShape("taxORIT4", eShapeStyle.Rect);
            DrawTextBox(taxT4, 49, 24, 324, 316, Color.White, 1, Color.Black, Color.Black,
                        new Font("Times New Roman", 11F), "9 9 9", eTextAlignment.Center);
            var taxT4L1 = worksheet.Drawings.AddShape("taxORIT4L1", eShapeStyle.Line);
            DrawLine(taxT4L1, 0, 4, 340, 336, 1, Color.Black);
            var taxT4L2 = worksheet.Drawings.AddShape("taxORIT4L2", eShapeStyle.Line);
            DrawLine(taxT4L2, 0, 4, 356, 336, 1, Color.Black);

        }

        private void DrawLine(ExcelShape eshape, int width, int height, int x,
                                int y, int borderWidth, Color borderColor)
        {
            eshape.SetSize(width, height);
            eshape.SetPosition(y, x);
            eshape.Border.Width = 1;
            eshape.Border.Fill.Color = Color.Black;
        }

        private void DrawTextBox(ExcelShape eshape, int width, int height, int x, int y,
                                 Color fillColor, int borderWidth, Color borderColor,
                                 Color fontColor, Font sfFont, string text, eTextAlignment tAlign)
        {
            eshape.SetSize(width, height);
            eshape.SetPosition(y, x);
            eshape.Fill.Color = fillColor;
            eshape.Border.Width = borderWidth;
            eshape.Border.Fill.Color = borderColor;
            eshape.Font.Color = fontColor;
            eshape.Font.SetFromFont(sfFont);
            eshape.Text = text;
            eshape.TextAlignment = tAlign;
        }
    }
}
