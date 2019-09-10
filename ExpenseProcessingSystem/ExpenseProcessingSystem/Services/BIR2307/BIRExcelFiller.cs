using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BIR_Form_Filler.Models;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace BIR_Form_Filler.Functions
{
    public class BIRExcelFiller
    {
        // Width of the Signature
        private readonly int sigWidth = 14900;

        public string FillBirForm(FirstPartBIRForm fpbf)
        {
            FileInfo template = new FileInfo("wwwroot/ExcelTemplates/_BIR2307_New.xlsx");

            string fileName = "BIR2307_"+fpbf.VoucherNo+"_"+fpbf.PayeeName+"_" + fpbf.IncomePay[0].Atc + ".xlsx";

            System.IO.File.Copy("wwwroot/ExcelTemplates/_BIR2307_New.xlsx", "wwwroot/ExcelTemplatesTempFolder/"+fileName,true);
            FileInfo newFile = new FileInfo("wwwroot/ExcelTemplatesTempFolder/"+fileName);

            try
            {
                using (ExcelPackage package = new ExcelPackage(newFile, template))
                {
                    //ExcelWorksheet worksheet = package.Workbook.Worksheets["2307"];
                    //SetForThePeriod(worksheet, fpbf.From_Date, fpbf.To_Date);
                    //SetPayeeInfo(worksheet, fpbf);
                    //SetPayorInfo(worksheet, fpbf);
                    //SetPayments(worksheet, 36, fpbf.IncomePay);
                    //SetPayments(worksheet, 48, fpbf.MoneyPay);
                    //SetSignatories(worksheet, fpbf.PayorSig, fpbf.PayeeSig);

                    //package.Save();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["2307"];

                    //1. For the Period From==============================================================
                    {
                        DateTime startDt = GetFirstMonthDateOfQuarter(fpbf.From_Date);
                        string fDate = startDt.ToString("MMddyy");

                        var shape = worksheet.Drawings.AddShape("date1mmdd", eShapeStyle.Rect);
                        shape.Text = fDate[0].ToString() + fDate[1].ToString() + "     " + fDate[2].ToString() + fDate[3].ToString();
                        shape.SetPosition(10, 5, 9, 5);
                        shape.SetSize(71, 28);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("date1mmdd_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(10, 5, 11, 2);
                        shape.SetSize(0, 28);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date1mmdd_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 10, 4);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date1mmdd_line3", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 12, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date1yyyy", eShapeStyle.Rect);
                        shape.Text = fpbf.From_Date.Year.ToString();
                        shape.SetPosition(10, 5, 12, 16);
                        shape.SetSize(71, 28);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("date1yyyy_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 13, 14);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date1yyyy_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 14, 14);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date1yyyy_line3", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 15, 14);
                        shape.SetSize(0, 6);
                        shape.Border.Width = 1 / 4;
                        shape.Border.Fill.Color = Color.Black;
                    }
                    //====================================================================================
                    //1. For the Period To================================================================
                    {
                        DateTime endDt = GetLastMonthDateOfQuarter(fpbf.To_Date);
                        string tDate = endDt.ToString("MMddyy");

                        var shape = worksheet.Drawings.AddShape("date2mmdd", eShapeStyle.Rect);
                        shape.Text = tDate[0].ToString() + tDate[1].ToString() + "     " + tDate[2].ToString() + tDate[3].ToString();
                        shape.SetPosition(10, 5, 26, 5);
                        shape.SetSize(71, 28);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("date2mmdd_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(10, 5, 28, 2);
                        shape.SetSize(0, 28);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date2mmdd_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 27, 4);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date2mmdd_line3", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 29, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date2yyyy", eShapeStyle.Rect);
                        shape.Text = fpbf.To_Date.Year.ToString();
                        shape.SetPosition(10, 5, 30, 0);
                        shape.SetSize(71, 28);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("date2yyyy_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 30, 14);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date2yyyy_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 31, 15);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("date2yyyy_line3", eShapeStyle.StraightConnector1);
                        shape.SetPosition(11, 7, 32, 14);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                    }
                    //====================================================================================
                    //2. For Payee TIN====================================================================
                    {
                        string PayeeTin = String.Join("", fpbf.EeTin.Split('-'));

                        var shape = worksheet.Drawings.AddShape("payeetin1", eShapeStyle.Rect);
                        shape.Text = PayeeTin.Substring(0, 3);
                        shape.SetPosition(13, 6, 13, 2);
                        shape.SetSize(53, 23);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("payeetin1_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(14, 7, 14, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payeetin1_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(14, 7, 15, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payeetin2", eShapeStyle.Rect);
                        shape.Text = PayeeTin.Substring(3, 3);
                        shape.SetPosition(13, 6, 16, 14);
                        shape.SetSize(53, 23);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("payeetin2_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(14, 7, 17, 11);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payeetin2_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(14, 7, 18, 10);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payeetin3", eShapeStyle.Rect);
                        shape.Text = PayeeTin.Substring(6, 3);
                        shape.SetPosition(13, 6, 20, 3);
                        shape.SetSize(53, 23);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("payeetin3_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(14, 7, 21, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payeetin3_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(14, 7, 22, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payeetin4", eShapeStyle.Rect);
                        shape.Text = PayeeTin.Substring(9);
                        shape.SetPosition(13, 6, 23, 12);
                        shape.SetSize(64, 23);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("payeetin4_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(14, 7, 24, 7);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payeetin4_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(14, 7, 25, 7);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payeetin4_line3", eShapeStyle.StraightConnector1);
                        shape.SetPosition(14, 7, 26, 7);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                    }
                    //====================================================================================
                    //3. For Payee Name===================================================================
                    {
                        var shape = worksheet.Drawings.AddShape("payeename", eShapeStyle.Rect);
                        shape.Text = fpbf.PayeeName;
                        shape.SetPosition(16, 0, 1, 3);
                        shape.SetSize(737, 24);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Left;
                        shape.Font.SetFromFont(new System.Drawing.Font("Times New Roman", 12));
                    }
                    //====================================================================================
                    //4. For Payee Address================================================================
                    {
                        var shape = worksheet.Drawings.AddShape("payeeaddress", eShapeStyle.Rect);
                        shape.Text = fpbf.EeRegAddress;
                        shape.SetPosition(19, 0, 1, 3);
                        shape.SetSize(660, 24);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Left;
                        shape.Font.SetFromFont(new System.Drawing.Font("Times New Roman", 12));
                    }
                    //====================================================================================
                    //6. For Payor TIN====================================================================
                    {
                        string PayorTin = String.Join("", fpbf.OrTin.Split('-'));

                        var shape = worksheet.Drawings.AddShape("payortin1", eShapeStyle.Rect);
                        shape.Text = PayorTin.Substring(0, 3);
                        shape.SetPosition(25, 6, 13, 2);
                        shape.SetSize(53, 23);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("payortin1_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(26, 5, 14, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payortin1_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(26, 5, 15, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payortin2", eShapeStyle.Rect);
                        shape.Text = PayorTin.Substring(3, 3);
                        shape.SetPosition(25, 6, 16, 14);
                        shape.SetSize(53, 23);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("payortin2_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(26, 5, 17, 11);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payortin2_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(26, 5, 18, 10);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payortin3", eShapeStyle.Rect);
                        shape.Text = PayorTin.Substring(6, 3);
                        shape.SetPosition(25, 6, 20, 3);
                        shape.SetSize(53, 23);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("payortin3_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(26, 5, 21, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payortin3_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(26, 5, 22, 0);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payortin4", eShapeStyle.Rect);
                        shape.Text = PayorTin.Substring(9);
                        shape.SetPosition(25, 6, 23, 12);
                        shape.SetSize(64, 23);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Center;
                        shape.Font.SetFromFont(new System.Drawing.Font("Arial", 10));

                        shape = worksheet.Drawings.AddShape("payortin4_line1", eShapeStyle.StraightConnector1);
                        shape.SetPosition(26, 5, 24, 7);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payortin4_line2", eShapeStyle.StraightConnector1);
                        shape.SetPosition(26, 5, 25, 7);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;

                        shape = worksheet.Drawings.AddShape("payortin4_line3", eShapeStyle.StraightConnector1);
                        shape.SetPosition(26, 5, 26, 7);
                        shape.SetSize(0, 6);
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                    }
                    //====================================================================================
                    //7. For Payor Name===================================================================
                    {
                        var shape = worksheet.Drawings.AddShape("payorname", eShapeStyle.Rect);
                        shape.Text = fpbf.PayorName;
                        shape.SetPosition(28, 0, 1, 3);
                        shape.SetSize(737, 24);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Left;
                        shape.Font.SetFromFont(new System.Drawing.Font("Times New Roman", 12));
                    }
                    //====================================================================================
                    //8. For Payee Address================================================================
                    {
                        var shape = worksheet.Drawings.AddShape("payoraddress", eShapeStyle.Rect);
                        shape.Text = fpbf.OrRegAddress;
                        shape.SetPosition(31, 0, 1, 3);
                        shape.SetSize(660, 24);
                        shape.Fill.Color = Color.Transparent;
                        shape.Border.Fill.Color = Color.Black;
                        shape.Border.Width = 1 / 4;
                        shape.Font.Color = Color.Black;
                        shape.TextAlignment = eTextAlignment.Left;
                        shape.Font.SetFromFont(new System.Drawing.Font("Times New Roman", 12));
                    }
                    //====================================================================================
                    //Details of Monthly Income Payments and Taxes Withheld===============================
                    //Income Payments Subject to Expanded Withholding Tax
                    {
                        int index = 38;
                        if (fpbf.IncomePay != null)
                        {
                            IEnumerator<PaymentInfo> iterator = fpbf.IncomePay.GetEnumerator();
                            while (iterator.MoveNext())
                            {
                                worksheet.Cells[$"A{index}"].Value = iterator.Current.Payments;
                                worksheet.Cells[$"L{index}"].Value = iterator.Current.Atc;

                                if (iterator.Current.M1Quarter > 0)
                                {
                                    worksheet.Cells[$"O{index}"].Value = iterator.Current.M1Quarter;
                                }
                                if (iterator.Current.M2Quarter > 0)
                                {
                                    worksheet.Cells[$"T{index}"].Value = iterator.Current.M2Quarter;
                                }
                                if (iterator.Current.M3Quarter > 0)
                                {
                                    worksheet.Cells[$"Y{index}"].Value = iterator.Current.M3Quarter;
                                }
                                if (iterator.Current.TaxWithheld > 0)
                                {
                                    worksheet.Cells[$"AI{index}"].Value = iterator.Current.TaxWithheld;
                                }
                                index++;
                            }

                        }
                    }
                    //====================================================================================
                    //Details of Monthly Income Payments and Taxes Withheld===============================
                    //Money Payments Subject to Withholding of Business Tax (Government & Private)
                    { 
                        int index = 51;
                        if (fpbf.MoneyPay != null)
                        {
                            IEnumerator<PaymentInfo> iterator = fpbf.IncomePay.GetEnumerator();
                            while (iterator.MoveNext())
                            {
                                worksheet.Cells[$"A{index}"].Value = iterator.Current.Payments;
                                worksheet.Cells[$"L{index}"].Value = iterator.Current.Atc;

                                if (iterator.Current.M1Quarter > 0)
                                {
                                    worksheet.Cells[$"O{index}"].Value = iterator.Current.M1Quarter;
                                }
                                if (iterator.Current.M2Quarter > 0)
                                {
                                    worksheet.Cells[$"T{index}"].Value = iterator.Current.M2Quarter;
                                }
                                if (iterator.Current.M3Quarter > 0)
                                {
                                    worksheet.Cells[$"Y{index}"].Value = iterator.Current.M3Quarter;
                                }
                                if (iterator.Current.TaxWithheld > 0)
                                {
                                    worksheet.Cells[$"AI{index}"].Value = iterator.Current.TaxWithheld;
                                }
                                index++;
                            }
                        }
                    }
                    //====================================================================================
                    //Payor Signatory++================================================================
                    worksheet.Cells["P62"].Value = fpbf.PayorSig.Name;
                    string belowName = "";
                    if (!String.IsNullOrEmpty(fpbf.PayorSig.Title)) belowName = fpbf.PayorSig.Title;
                    if (!String.IsNullOrEmpty(belowName))
                    {
                        if (!String.IsNullOrEmpty(fpbf.PayorSig.Tin)) belowName = belowName + " / " + fpbf.PayorSig.Tin;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(fpbf.PayorSig.Tin)) belowName = fpbf.PayorSig.Tin;
                    }
                    worksheet.Cells["P64"].Value = belowName;
                    worksheet.Cells["P64"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    if (!String.IsNullOrEmpty(fpbf.PayorSig.ESigPath))
                    {
                        FileInfo imgSign = new FileInfo(fpbf.PayorSig.ESigPath);
                        var eSignature = worksheet.Drawings.AddPicture("sigImage", imgSign);
                        eSignature.SetSize(124, 53);
                        eSignature.SetPosition(60, 45, 17, 3);
                    }
                    //====================================================================================
                    package.Save();
                    package.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

            return "/ExcelTemplatesTempFolder/" + newFile.Name;
        }

        //private void SetSignatories(ExcelWorksheet worksheet, Signatories payor, Signatories payee)
        //{
        //    //payor
        //    worksheet.Cells["B60"].Value = payor.Name;
        //    worksheet.Cells["U60"].Value = payor.Tin;
        //    worksheet.Cells["AG60"].Value = payor.Title;
        //    worksheet.Cells["B63"].Value = payor.TaxAcc;
        //    worksheet.Cells["U63"].Value = DateTime.Now.ToString("MM/dd/yy");
        //    //worksheet.Cells["AG63"].Value = payor.DateExpiry.ToString("MM/dd/yy");
        //    if(payor.ESigPath != null) {
        //        FileInfo sig1 = new FileInfo(payor.ESigPath);
        //        if (sig1.Exists)
        //        {
        //            var sigImg = worksheet.Drawings.AddPicture("orSig", sig1);
        //            sigImg.SetSize(sigWidth / sigImg.Image.Width);
        //            sigImg.SetPosition(1062, 94);
        //        }
        //    }
        //    //payee
        //    //worksheet.Cells["B74"].Value = payee.Name;
        //    //worksheet.Cells["U74"].Value = payee.Tin;
        //    //worksheet.Cells["AD74"].Value = payee.Title;
        //    //worksheet.Cells["B77"].Value = payee.TaxAcc;
        //    //worksheet.Cells["U77"].Value = payee.DateIssue.ToString("MM/dd/yy");
        //    //worksheet.Cells["AG77"].Value = payee.DateExpiry.ToString("MM/dd/yy");
        //    //if(payee.ESigPath != null)
        //    //{
        //    //    FileInfo sig2 = new FileInfo(payee.ESigPath);
        //    //    if (sig2.Exists)
        //    //    {
        //    //        var sigImg2 = worksheet.Drawings.AddPicture("eeSig", sig2);
        //    //        sigImg2.SetSize(sigWidth / sigImg2.Image.Width);
        //    //        sigImg2.SetPosition(1155, 94);
        //    //    }
        //    //}

        //}

        //private void SetPayments(ExcelWorksheet worksheet, int index, List<PaymentInfo> payInfo = null)
        //{
        //    if(payInfo != null)
        //    {
        //        IEnumerator<PaymentInfo> iterator = payInfo.GetEnumerator();
        //        while(iterator.MoveNext())
        //        {
        //            worksheet.Cells[$"A{index}"].Value = iterator.Current.Payments;
        //            worksheet.Cells[$"M{index}"].Value = iterator.Current.Atc;

        //            if (iterator.Current.M1Quarter > 0)
        //            {
        //                worksheet.Cells[$"Q{index}"].Value = iterator.Current.M1Quarter;
        //            }
        //            if (iterator.Current.M2Quarter > 0)
        //            {
        //                worksheet.Cells[$"V{index}"].Value = iterator.Current.M2Quarter;
        //            }
        //            if (iterator.Current.M3Quarter > 0)
        //            {
        //                worksheet.Cells[$"AA{index}"].Value = iterator.Current.M3Quarter;
        //            }
        //            if (iterator.Current.TaxWithheld > 0)
        //            {
        //                worksheet.Cells[$"AK{index}"].Value = iterator.Current.TaxWithheld;
        //            }

        //            index++;    
        //        }
                
        //    }
        //}

        //private void SetPayeeInfo(ExcelWorksheet ws, FirstPartBIRForm fpbf)
        //{
        //    var offset = 9;
        //    //TIN
        //    var tin = String.Join("", fpbf.EeTin.Split('-'));
        //    for (int i = 0; i< tin.Length;i++)
        //    {
        //        var tinarr = tin.ToCharArray();
        //        if (i % 3 == 0 && i > 0)
        //            offset++;

        //        ws.Cells[13, offset + i].Value = tinarr[i].ToString();
        //    }

        //    //Payee
        //    ws.Cells[15,9].Value = fpbf.PayeeName;

        //    //address
        //    ws.Cells[17,9].Value = fpbf.EeRegAddress;
        //}

        //private void SetPayorInfo(ExcelWorksheet ws, FirstPartBIRForm fpbf)
        //{
        //    var offset = 9;
        //    //TIN
        //    var tin = String.Join("",fpbf.OrTin.Split('-'));
        //    for (int i = 0; i < tin.Length; i++)
        //    {
        //        var tinarr = tin.ToCharArray();
        //        if (i % 3 == 0 && i > 0)
        //            offset++;

        //        ws.Cells[25, offset + i].Value = tinarr[i].ToString();
        //    }

        //    //Payor
        //    ws.Cells[27, 9].Value = fpbf.PayorName;

        //    //address
        //    ws.Cells[29, 9].Value = fpbf.OrRegAddress;
        //}

        //private void SetForThePeriod(ExcelWorksheet ws, DateTime fromDate, DateTime toDate)
        //{
        //    DateTime startDt = GetFirstMonthDateOfQuarter(fromDate);
        //    DateTime endDt = GetLastMonthDateOfQuarter(toDate);
        //    string fDate = startDt.ToString("MMddyy");
        //    string tDate = endDt.ToString("MMddyy");

        //    //From Date
        //    ws.Cells["I8"].Value = fDate[0].ToString();
        //    ws.Cells["J8"].Value = fDate[1].ToString();
        //    ws.Cells["K8"].Value = fDate[2].ToString();
        //    ws.Cells["L8"].Value = fDate[3].ToString();
        //    ws.Cells["M8"].Value = fDate[4].ToString();
        //    ws.Cells["N8"].Value = fDate[5].ToString();

        //    //To Date
        //    ws.Cells["Y8"].Value = tDate[0].ToString();
        //    ws.Cells["Z8"].Value = tDate[1].ToString();
        //    ws.Cells["AA8"].Value = tDate[2].ToString();
        //    ws.Cells["AB8"].Value = tDate[3].ToString();
        //    ws.Cells["AC8"].Value = tDate[4].ToString();
        //    ws.Cells["AD8"].Value = tDate[5].ToString();

        //}

        private DateTime GetFirstMonthDateOfQuarter(DateTime date)
        {
            int[] Q1 = { 1, 2, 3 };
            int[] Q2 = { 4, 5, 6 };
            int[] Q3 = { 7, 8, 9 };
            int[] Q4 = { 10, 11, 12 };
            string format = "yyyy-M";

            if (Q1.Contains(date.Month))
            {
                return DateTime.ParseExact(date.Year + "-" + Q1[0], format, CultureInfo.InvariantCulture);
            }
            if (Q2.Contains(date.Month))
            {
                return DateTime.ParseExact(date.Year + "-" + Q2[0], format, CultureInfo.InvariantCulture);
            }
            if (Q3.Contains(date.Month))
            {
                return DateTime.ParseExact(date.Year + "-" + Q3[0], format, CultureInfo.InvariantCulture);
            }
            if (Q4.Contains(date.Month))
            {
                return DateTime.ParseExact(date.Year + "-" + Q4[0], format, CultureInfo.InvariantCulture);
            }

            return new DateTime();
        }
        private DateTime GetLastMonthDateOfQuarter(DateTime date)
        {
            int[] Q1 = { 1, 2, 3 };
            int[] Q2 = { 4, 5, 6 };
            int[] Q3 = { 7, 8, 9 };
            int[] Q4 = { 10, 11, 12 };
            string format = "yyyy-M";

            if (Q1.Contains(date.Month))
            {
                return DateTime.ParseExact(date.Year + "-" + Q1[2], format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
            }
            if (Q2.Contains(date.Month))
            {
                return DateTime.ParseExact(date.Year + "-" + Q2[2], format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
            }
            if (Q3.Contains(date.Month))
            {
                return DateTime.ParseExact(date.Year + "-" + Q3[2], format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
            }
            if (Q4.Contains(date.Month))
            {
                return DateTime.ParseExact(date.Year + "-" + Q4[2], format, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
            }

            return new DateTime();
        }
    }
}
