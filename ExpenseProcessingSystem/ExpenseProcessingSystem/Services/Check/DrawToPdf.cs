using ExpenseProcessingSystem.Models.Check;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services.Check
{
    public class DrawToPdf
    {
        private double[] payeeCoord;
        private double[] amNumCoord;
        private double[] dateCoord;
        private double[] amWorCoord;
        private double[] sigOnCoord;
        private double[] sigTwCoord;

        public DrawToPdf(double[] payeeCoord, double[] amNumCoord,
                         double[] dateCoord, double[] amWorCoord,
                         double[] sigOnCoord, double[] sigTwCoord)
        {
            this.payeeCoord = payeeCoord;
            this.amNumCoord = amNumCoord;
            this.dateCoord = dateCoord;
            this.amWorCoord = amWorCoord;
            this.sigOnCoord = sigOnCoord;
            this.sigTwCoord = sigTwCoord;
        }

        public void CreatePDF(ChequeData cd, string filename)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            //document.Info.Title = "Created with PDFsharp";

            // Create an empty page
            PdfPage page = document.AddPage();

            // Set page size in points
            page.Width = 576;
            page.Height = 216;

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //var enc1252 = Encoding.GetEncoding(1252);

            // Create a font
            XFont font = new XFont("Arial", 10, XFontStyle.Regular);

            //XImage image = XImage.FromFile("C:/Users/akio.fujiwara/Desktop/ChequePrint/chequepic_8-5_11.jpg");

            //gfx.DrawImage(image, -18, 0);


            /// Drawing information into the document. Please note that x and y coordinates
            /// are in points.
            // Payee Information
            DrawTextToPdf(gfx, "***" + cd.Payee + "***", font, payeeCoord);

            // Amount (Num) Information
            DrawTextToPdf(gfx, "***" + cd.Amount.ToString("N2") + "***", font, amNumCoord);

            // Date Information
            DrawTextToPdf(gfx, cd.Date.ToString("MMMM dd, yyyy"), font, dateCoord);

            // Amount (Word) Information
            DrawTextToPdfBox(gfx, "***" + ConvertToWord.ToWord(cd.Amount) + "***", font, amWorCoord);

            // Signatory 1
            DrawTextToPdf(gfx, cd.Signatory1, font, sigOnCoord);

            // Signatory 2
            DrawTextToPdf(gfx, cd.Signatory2, font, sigTwCoord);

            // Save the document...
            string file = $"wwwroot/ExcelTemplatesTempFolder/" + filename;

            document.Save(file);

        }


        private void DrawTextToPdf(XGraphics gfx, string toPrint,
                                    XFont font, double[] coords)
        {
            gfx.DrawString(toPrint, font, XBrushes.Black, coords[0], coords[1]);
        }

        private void DrawTextToPdfBox(XGraphics gfx, string toPrint,
                                    XFont font, double[] coords)
        {
            XTextFormatter ttx = new XTextFormatter(gfx);
            ttx.DrawString(toPrint, font, XBrushes.Black, new XRect(coords[0], coords[1], 500, 40));
        }
    }
}
