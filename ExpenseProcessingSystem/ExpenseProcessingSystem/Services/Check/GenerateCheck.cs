using ExpenseProcessingSystem.Models.Check;
using ExpenseProcessingSystem.Services;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace ExpenseProcessingSystem.Services.Check
{
    class GenerateCheck
    {
        private string templatePath;
        private bool fileExist;

        public string Generate(ChequeData cd)
        {
            Color color = Color.Black;
            eTextAnchoringType anchorTop = eTextAnchoringType.Top;
            eTextAlignment alignLeft = eTextAlignment.Left;
            eTextAlignment alignCenter = eTextAlignment.Center;
            eFillStyle style = eFillStyle.NoFill;
            eFillStyle fillStyle = eFillStyle.NoFill;

            // Template Path
            //templatePath = @"C:\Temp\Work\Repository\Task\CheckPrinter\Template Files\template.xlsx";
            // New File Path
            //string excelPath = @"C:\Temp\Work\Repository\Task\CheckPrinter\Excel Files\" +
            //                  "CHECK_" + DateTime.Now.ToString("MMddyyyyhhmmssfff") +
            //                  ".xlsx";
            FileInfo templateFile = new FileInfo("wwwroot/ExcelTemplates/_CHECK.xlsx");

            string fileName = "CHECK_" + cd.Voucher + ".xlsx";
            System.IO.File.Copy("wwwroot/ExcelTemplates/_CHECK.xlsx", "wwwroot/ExcelTemplatesTempFolder/" + fileName, true);

            FileInfo excelFile = new FileInfo("wwwroot/ExcelTemplatesTempFolder/" + fileName);

            // Check If File Exists
            CheckFile();

            using (ExcelPackage excel = new ExcelPackage(excelFile, templateFile))
            {
                ExcelWorksheet worksheet = excel.Workbook.Worksheets["Sheet1"];
                // Create Textbox
                var payee = worksheet.Drawings.AddShape("payee", eShapeStyle.Rect);
                var date = worksheet.Drawings.AddShape("date", eShapeStyle.Rect);
                var amount = worksheet.Drawings.AddShape("amount", eShapeStyle.Rect);
                var amountWord = worksheet.Drawings.AddShape("amountWord", eShapeStyle.Rect);
                var signatory1 = worksheet.Drawings.AddShape("signatory1", eShapeStyle.Rect);
                var signatory2 = worksheet.Drawings.AddShape("signatory2", eShapeStyle.Rect);
                // Set Font
                payee.Font.SetFromFont(new Font("Times New Roman", 12));
                date.Font.SetFromFont(new Font("Times New Roman", 12));
                amount.Font.SetFromFont(new Font("Times New Roman", 12));
                amountWord.Font.SetFromFont(new Font("Times New Roman", 12));
                signatory1.Font.SetFromFont(new Font("Times New Roman", 9));
                signatory2.Font.SetFromFont(new Font("Times New Roman", 9));

                FormatTextBox(payee, "***" + cd.Payee + "***", 50, 87, 420, 28,
                color, anchorTop, alignLeft, style, fillStyle);
                FormatTextBox(date, cd.Date.ToString("MM/dd/yyyy"), 26, 530, 150, 28,
                       color, anchorTop, alignCenter, style, fillStyle);
                FormatTextBox(amount, "***" + cd.Amount.ToString("N2") + "***", 50, 520, 180, 28,
                        color, anchorTop, alignCenter, style, fillStyle);
                FormatTextBox(amountWord, "***" + ConvertToWord.ToWord(cd.Amount) + "***", 80, 65, 620, 40,
                        color, anchorTop, alignLeft, style, fillStyle);
                FormatTextBox(signatory1, cd.Signatory1, 151, 350, 180, 25,
                        color, anchorTop, alignCenter, style, fillStyle);
                FormatTextBox(signatory2, cd.Signatory2, 151, 525, 180, 25,
                        color, anchorTop, alignCenter, style, fillStyle);

                // Adjust Font Size
                if (cd.Payee.Length > 40 && cd.Payee.Length < 51)
                {
                    payee.Font.SetFromFont(new Font("Times New Roman", 11));
                    FormatTextBox(payee, "***" + cd.Payee + "***", 58, 87, 420, 16,
                    color, anchorTop, alignLeft, style, fillStyle);
                } 
                else if (cd.Payee.Length >= 51)
                {
                    payee.Font.SetFromFont(new Font("Times New Roman", 11));
                    FormatTextBox(payee, "***" + cd.Payee + "***", 41, 87, 420, 40,
                    color, anchorTop, alignLeft, style, fillStyle);
                }
                if(cd.Signatory1.Length > 25 || cd.Signatory2.Length > 25)
                {
                    signatory1.Font.SetFromFont(new Font("Times New Roman", 8));
                    signatory2.Font.SetFromFont(new Font("Times New Roman", 8));
                }

                // ====== ADDITIONAL SETTINGS =======
                // Settings
                //worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
                //worksheet.PrinterSettings.TopMargin = 0;
                //worksheet.PrinterSettings.RightMargin = 0;
                //worksheet.PrinterSettings.BottomMargin = 0;
                //worksheet.PrinterSettings.LeftMargin = 0;
                // Save

                // Remove template to the new file
                worksheet.Drawings.Remove("image");
                worksheet.Drawings.Remove("border");

                excel.SaveAs(excelFile);
                // Open
                //var p = new Process
                //{
                //    StartInfo = new ProcessStartInfo(excelPath.ToString())
                //    {
                //        UseShellExecute = true
                //    }
                //};
                //p.Start();

                return "/ExcelTemplatesTempFolder/" + excelFile.Name;
            }
        }

        private ExcelShape FormatTextBox(ExcelShape data, string text, int top, int left, int width, int height, 
                                    Color color, eTextAnchoringType anchor, eTextAlignment alignment,
                                    eFillStyle style, eFillStyle fillStyle)
        {
            data.Text = text;
            data.SetPosition(top, left);
            data.SetSize(width, height);
            data.Font.Color = color;
            data.TextAnchoring = anchor;
            data.TextAlignment = alignment;
            data.Fill.Style = style;
            data.Border.Fill.Style = fillStyle;
            return data;
        }

        private void CheckFile()
        {
            // Option 1
            Console.WriteLine(File.Exists(templatePath) ? true : false);

            // Option 2
            if (File.Exists(templatePath))
            {
                fileExist = true;
            }
            else
            {
                fileExist = false;
            }
            Console.WriteLine(fileExist);
        }

        public bool GenerateCheckPDF(ChequeData cd, string filename)
        {
            XElement xelemCoord = XElement.Load("wwwroot/xml/CheckCoordinates.xml");
            double x_payee = double.Parse(xelemCoord.Element("X_Payee").Value);
            double y_payee = double.Parse(xelemCoord.Element("Y_Payee").Value);

            double x_amount = double.Parse(xelemCoord.Element("X_Amount").Value);
            double y_amount = double.Parse(xelemCoord.Element("Y_Amount").Value);

            double x_date = double.Parse(xelemCoord.Element("X_Date").Value);
            double y_date = double.Parse(xelemCoord.Element("Y_Date").Value);

            double x_amountWord = double.Parse(xelemCoord.Element("X_AmountWord").Value);
            double y_amountWord = double.Parse(xelemCoord.Element("Y_AmountWord").Value);

            double x_sign1 = double.Parse(xelemCoord.Element("X_Signatory1").Value);
            double y_sign1 = double.Parse(xelemCoord.Element("Y_Signatory1").Value);

            double x_sign2 = double.Parse(xelemCoord.Element("X_Signatory2").Value);
            double y_sign2 = double.Parse(xelemCoord.Element("Y_Signatory2").Value);
            
            DrawToPdf dtp = new DrawToPdf(new double[] { x_payee * .48, y_payee * .48 },  // Payee
                                          new double[] { x_amount * .48, y_amount * .48 },  // Amount
                                          new double[] { x_date * .48, y_date * .48 },   // Date
                                          new double[] { x_amountWord * .48, y_amountWord * .48 },  // Amount (words)
                                          new double[] { x_sign1 * .48, x_sign1 * .48 },  // Signatory 1
                                          new double[] { x_sign2 * .48, y_sign2 * .48 }   // Signatory 2
                              );

            dtp.CreatePDF(cd, filename);

            return true;
        }


    }
}
