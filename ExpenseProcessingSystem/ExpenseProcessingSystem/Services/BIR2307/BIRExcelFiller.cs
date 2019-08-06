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

namespace BIR_Form_Filler.Functions
{
    public class BIRExcelFiller
    {
        private List<PaymentInfo> incPay;
        private List<PaymentInfo> monPay;

        // Width of the Signature
        private readonly int sigWidth = 14900;

        // For testing IncomePay and MoneyPay
        public BIRExcelFiller()
        {
            initSampIncPay();
            initSampMonPay();

        }

        // Populating sample list
        private void initSampIncPay()
        {
            incPay = new List<PaymentInfo>();
            incPay.Add(new PaymentInfo()
            {
                Payments = "YKK Zippers",
                Atc = "WV180",
                M1Quarter = 123000123.15,
                M2Quarter = 932182.21,
                M3Quarter = 657899.12,
                TaxWithheld = 638823.51
            });
            incPay.Add(new PaymentInfo()
            {
                Payments = "Baker's Depot",
                Atc = "WOW10",
                M1Quarter = 812390.21,
                TaxWithheld = 4421.99
            });
            incPay.Add(new PaymentInfo()
            {
                Payments = "The Barber's Store",
                Atc = "W210",
                M2Quarter = 11233345.01,
                M3Quarter = 654123.23,
                TaxWithheld = 5555.55
            });
            incPay.Add(new PaymentInfo()
            {
                Payments = "Marikina sa Quiapo",
                Atc = "WZ99",
                M2Quarter = 11233.88,
                TaxWithheld = 1355.53
            });

        }

        private void initSampMonPay()
        {
            monPay = new List<PaymentInfo>();
            monPay.Add(new PaymentInfo()
            {
                Payments = "Gelato Winns",
                Atc = "WKK2",
                M3Quarter = 29005.34,
                TaxWithheld = 34555.12
            });
            monPay.Add(new PaymentInfo()
            {
                Payments = "Dalton and His Friends Ltd.",
                Atc = "WAK2",
                M2Quarter = 929239.32,
                TaxWithheld = 1299.23
            });
            monPay.Add(new PaymentInfo()
            {
                Payments = "Ice Fruit Loops",
                Atc = "W11P",
                M1Quarter = 321868.20,
                M2Quarter = 100225.66,
                M3Quarter = 638234.00,
                TaxWithheld = 450123.00
            });
        }


        public string FillBirForm(FirstPartBIRForm fpbf)
        {
            FileInfo template = new FileInfo("wwwroot/ExcelTemplates/_BIR_LAYOUT.xlsx");

            string fileName = "BIR2307_"+fpbf.VoucherNo+"_"+fpbf.PayeeName+"_" + fpbf.IncomePay[0].Atc + ".xlsx";

            System.IO.File.Copy("wwwroot/ExcelTemplates/_bir_layout.xlsx", "wwwroot/ExcelTemplatesTempFolder/"+fileName,true);
            FileInfo newFile = new FileInfo("wwwroot/ExcelTemplatesTempFolder/"+fileName);

            try
            {
                using (ExcelPackage package = new ExcelPackage(newFile, template))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["2307"];
                    SetForThePeriod(worksheet, fpbf.From_Date, fpbf.To_Date);
                    SetPayeeInfo(worksheet, fpbf);
                    SetPayorInfo(worksheet, fpbf);
                    SetPayments(worksheet, 36, fpbf.IncomePay);
                    SetPayments(worksheet, 48, fpbf.MoneyPay);
                    SetSignatories(worksheet, fpbf.PayorSig, fpbf.PayeeSig);
                    
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

        private void SetSignatories(ExcelWorksheet worksheet, Signatories payor, Signatories payee)
        {
            //payor
            worksheet.Cells["B60"].Value = payor.Name;
            worksheet.Cells["U60"].Value = payor.Tin;
            worksheet.Cells["AG60"].Value = payor.Title;
            worksheet.Cells["B63"].Value = payor.TaxAcc;
            worksheet.Cells["U63"].Value = DateTime.Now.ToString("MM/dd/yy");
            //worksheet.Cells["AG63"].Value = payor.DateExpiry.ToString("MM/dd/yy");
            if(payor.ESigPath != null) {
                FileInfo sig1 = new FileInfo(payor.ESigPath);
                if (sig1.Exists)
                {
                    var sigImg = worksheet.Drawings.AddPicture("orSig", sig1);
                    sigImg.SetSize(sigWidth / sigImg.Image.Width);
                    sigImg.SetPosition(1062, 94);
                }
            }
            //payee
            //worksheet.Cells["B74"].Value = payee.Name;
            //worksheet.Cells["U74"].Value = payee.Tin;
            //worksheet.Cells["AD74"].Value = payee.Title;
            //worksheet.Cells["B77"].Value = payee.TaxAcc;
            //worksheet.Cells["U77"].Value = payee.DateIssue.ToString("MM/dd/yy");
            //worksheet.Cells["AG77"].Value = payee.DateExpiry.ToString("MM/dd/yy");
            //if(payee.ESigPath != null)
            //{
            //    FileInfo sig2 = new FileInfo(payee.ESigPath);
            //    if (sig2.Exists)
            //    {
            //        var sigImg2 = worksheet.Drawings.AddPicture("eeSig", sig2);
            //        sigImg2.SetSize(sigWidth / sigImg2.Image.Width);
            //        sigImg2.SetPosition(1155, 94);
            //    }
            //}

        }

        private void SetPayments(ExcelWorksheet worksheet, int index, List<PaymentInfo> payInfo = null)
        {
            if(payInfo != null)
            {
                IEnumerator<PaymentInfo> iterator = payInfo.GetEnumerator();
                while(iterator.MoveNext())
                {
                    worksheet.Cells[$"A{index}"].Value = iterator.Current.Payments;
                    worksheet.Cells[$"M{index}"].Value = iterator.Current.Atc;

                    if (iterator.Current.M1Quarter > 0)
                    {
                        worksheet.Cells[$"Q{index}"].Value = iterator.Current.M1Quarter;
                    }
                    if (iterator.Current.M2Quarter > 0)
                    {
                        worksheet.Cells[$"V{index}"].Value = iterator.Current.M2Quarter;
                    }
                    if (iterator.Current.M3Quarter > 0)
                    {
                        worksheet.Cells[$"AA{index}"].Value = iterator.Current.M3Quarter;
                    }
                    if (iterator.Current.TaxWithheld > 0)
                    {
                        worksheet.Cells[$"AK{index}"].Value = iterator.Current.TaxWithheld;
                    }

                    index++;    
                }
                
            }
        }

        private void SetPayeeInfo(ExcelWorksheet ws, FirstPartBIRForm fpbf)
        {
            var offset = 9;
            //TIN
            var tin = String.Join("", fpbf.EeTin.Split('-'));
            for (int i = 0; i< tin.Length;i++)
            {
                var tinarr = tin.ToCharArray();
                if (i % 3 == 0 && i > 0)
                    offset++;

                ws.Cells[13, offset + i].Value = tinarr[i].ToString();
            }

            //Payee
            ws.Cells[15,9].Value = fpbf.PayeeName;

            //address
            ws.Cells[17,9].Value = fpbf.EeRegAddress;
        }

        private void SetPayorInfo(ExcelWorksheet ws, FirstPartBIRForm fpbf)
        {
            var offset = 9;
            //TIN
            var tin = String.Join("",fpbf.OrTin.Split('-'));
            for (int i = 0; i < tin.Length; i++)
            {
                var tinarr = tin.ToCharArray();
                if (i % 3 == 0 && i > 0)
                    offset++;

                ws.Cells[25, offset + i].Value = tinarr[i].ToString();
            }

            //Payor
            ws.Cells[27, 9].Value = fpbf.PayorName;

            //address
            ws.Cells[29, 9].Value = fpbf.OrRegAddress;
        }

        private void SetForThePeriod(ExcelWorksheet ws, DateTime fromDate, DateTime toDate)
        {
            string fDate = fromDate.ToString("MMddyy");
            string tDate = toDate.ToString("MMddyy");

            //From Date
            ws.Cells["I8"].Value = fDate[0].ToString();
            ws.Cells["J8"].Value = fDate[1].ToString();
            ws.Cells["K8"].Value = fDate[2].ToString();
            ws.Cells["L8"].Value = fDate[3].ToString();
            ws.Cells["M8"].Value = fDate[4].ToString();
            ws.Cells["N8"].Value = fDate[5].ToString();

            //To Date
            ws.Cells["Y8"].Value = tDate[0].ToString();
            ws.Cells["Z8"].Value = tDate[1].ToString();
            ws.Cells["AA8"].Value = tDate[2].ToString();
            ws.Cells["AB8"].Value = tDate[3].ToString();
            ws.Cells["AC8"].Value = tDate[4].ToString();
            ws.Cells["AD8"].Value = tDate[5].ToString();

        }
    }
}
