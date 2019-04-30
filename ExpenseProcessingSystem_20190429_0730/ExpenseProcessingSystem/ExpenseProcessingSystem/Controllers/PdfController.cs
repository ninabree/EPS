using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rotativa;

namespace ExpenseProcessingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        public ActionResult<PartialViewAsPdf> GenerateFile(HomeReportViewModel model)
        {
            string PDFLayout = "";
            IEnumerable<TEMP_HomeReportOutputModel> Data = null;

            PDFLayout = ConstantData.HomeReportConstantValue.ReportLayoutFormatName + model.ReportType;
            Data = ConstantData.TEMP_HomeReportDummyData.GetTEMP_HomeReportOutputModelData();

            return new PartialViewAsPdf(PDFLayout, Data)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.A4,
                CustomSwitches = "--footer-left \" PAGE => [page] of [toPage] \" --footer-right \" Printed Date => " + DateTime.Today.ToShortDateString() + "\" --footer-font-size \"9\" --footer-spacing 3 --footer-font-name \"calibri light\"",
                FileName = "TestPDFTestPartialViewAsPdf.pdf"
            };
        }
    }
}