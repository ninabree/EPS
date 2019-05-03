#pragma checksum "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "cd287dd3c16619ce531423bcef4a4b25213a3de9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Report), @"mvc.1.0.view", @"/Views/Home/Report.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Report.cshtml", typeof(AspNetCore.Views_Home_Report))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\_ViewImports.cshtml"
using ExpenseProcessingSystem;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"cd287dd3c16619ce531423bcef4a4b25213a3de9", @"/Views/Home/Report.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Report : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ExpenseProcessingSystem.ViewModels.HomeReportViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(63, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
  
	ViewData["Title"] = "Report";

#line default
#line hidden
            BeginContext(104, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(124, 416, true);
                WriteLiteral(@"
	<script type=""text/javascript"">
		$(document).ready(function () {
			$(""#ddlReportType"").change(function () {
				var ReportType = $(this).val();
				var select = $(""#ddlSubType"");
				if (ReportType == '') {
					select.empty();
					select.attr(""disabled"", ""disabled"");
					select.append($('<option/>', {
						value: 0,
						text: ""----Select Report Sub-Type----""
					}));
				}
				$.getJSON('");
                EndContext();
                BeginContext(541, 30, false);
#line 21 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                      Write(Url.Action("GetReportSubType"));

#line default
#line hidden
                EndContext();
                BeginContext(571, 1954, true);
                WriteLiteral(@"', { ReportTypeID: ReportType }, function (data) {
					select.empty();
					if (data.length == 0) {
						select.attr(""disabled"", ""disabled"");
						select.append($('<option/>', {
						value: 0,
						text: ""----Select Report Sub-Type----""
						}));
					} else {
						select.removeAttr(""disabled"");
					}
					$.each(data, function (index, itemData) {
						select.append($('<option/>', {
						value: itemData.id,
						text: itemData.subTypeName
						}));
					});
				});
			});
		});


		$(document).ready(function () {
			$(""#btnGenerateFile"").click(function (e) {
				e.preventDefault();

				window.location.href = ""/Home/GenerateFilePreview?ReportType="" + $('#ddlReportType').val()
					+ ""&ReportSubType="" + $('#ddlSubType').val()
					+ ""&FileFormat="" + $('#ddlFileFormat').val()
					+ ""&Year="" + $('#ddlYear').val()
					+ ""&Month="" + $('#ddlMonth').val()
					+ ""&YearSem="" + $('#ddlYearSem').val()
					+ ""&Semester="" + $('#radioSemester:checked').val()
					+ ""&PeriodO");
                WriteLiteral(@"ption="" + $('#radioPeriodOption:checked').val();

			});
		});

		$(document).ready(function () {
			$(""#btnGeneratePreview"").click(function (e) {
				e.preventDefault();

				$('#iframePreview').prop('src', ""/Home/GenerateFilePreview?ReportType="" + $('#ddlReportType').val()
					+ ""&ReportSubType="" + $('#ddlSubType').val()
					+ ""&FileFormat=3""
					+ ""&Year="" + $('#ddlYear').val()
					+ ""&Month="" + $('#ddlMonth').val()
					+ ""&YearSem="" + $('#ddlYearSem').val()
					+ ""&Semester="" + $('#radioSemester:checked').val()
					+ ""&PeriodOption="" + $('#radioPeriodOption:checked').val());

				var dt = new Date();
				var date_time = ('0' + (dt.getMonth() + 1)).slice(-2) + ""/"" + dt.getDate() + ""/"" + dt.getFullYear() + "" "" + dt.getHours() + "":"" + dt.getMinutes() + "":"" + dt.getSeconds();

				$('#txtAsOfLabel').text(""As of "");
				$('#txtDatePreviewShow').text(date_time);
			});
		});
	</script>
");
                EndContext();
            }
            );
            BeginContext(2528, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 82 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
 using (Html.BeginForm())
{

#line default
#line hidden
            BeginContext(2560, 372, true);
            WriteLiteral(@"	<div class=""m-t-10"">
		<table class=""table voucher-tbl"">
			<col style=""width:5%"">
			<col style=""width:10%"">
			<col style=""width:10%"">
			<col style=""width:10%"">
			<col style=""width:10%"">
			<col style=""width:5%"">
			<col style=""width:10%"">
			<col style=""width:10%"">
			<col style=""width:10%"">
			<col style=""width:20%"">

			<tr>
				<td></td>
				<td>");
            EndContext();
            BeginContext(2933, 37, false);
#line 99 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.ReportTypesList));

#line default
#line hidden
            EndContext();
            BeginContext(2970, 34, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td colspan=\"2\">\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(3005, 190, false);
#line 101 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.ReportTypesList, new SelectList(Model.ReportTypesList, "Id", "TypeName"), "----Select Report Type----", new { @class = "dis-inline-block", id = "ddlReportType" }));

#line default
#line hidden
            EndContext();
            BeginContext(3195, 21, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(3217, 40, false);
#line 103 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.ReportSubTypesList));

#line default
#line hidden
            EndContext();
            BeginContext(3257, 34, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td colspan=\"2\">\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(3292, 220, false);
#line 105 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.ReportTypesList, new SelectList(Model.ReportSubTypesList, "Id", "SubTypeName"), "----Select Report Sub-Type----", new { @class = "dis-inline-block", id = "ddlSubType", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(3512, 114, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td colspan=\"4\"></td>\r\n\t\t\t</tr>\r\n\t\t\t<tr>\r\n\t\t\t\t<td colspan=\"10\"></td>\r\n\t\t\t</tr>\r\n\t\t\t<tr>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(3627, 81, false);
#line 113 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.RadioButton("radioTimePeriod", "1", false, new { id = "radioPeriodOption" }));

#line default
#line hidden
            EndContext();
            BeginContext(3708, 15, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(3724, 30, false);
#line 114 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.YearList));

#line default
#line hidden
            EndContext();
            BeginContext(3754, 22, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(3777, 158, false);
#line 116 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.YearList, new SelectList(Model.YearList, "YearID", "YearID"), "----Year----", new { @class = "dis-inline-block", id = "ddlYear" }));

#line default
#line hidden
            EndContext();
            BeginContext(3935, 21, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(3957, 31, false);
#line 118 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.MonthList));

#line default
#line hidden
            EndContext();
            BeginContext(3988, 22, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(4011, 162, false);
#line 120 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.MonthList, new SelectList(Model.MonthList, "MonthID", "MonthName"), "--Month--", new { @class = "dis-inline-block", id = "ddlMonth" }));

#line default
#line hidden
            EndContext();
            BeginContext(4173, 21, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(4195, 81, false);
#line 122 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.RadioButton("radioTimePeriod", "2", false, new { id = "radioPeriodOption" }));

#line default
#line hidden
            EndContext();
            BeginContext(4276, 15, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(4292, 33, false);
#line 123 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.YearSemList));

#line default
#line hidden
            EndContext();
            BeginContext(4325, 22, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(4348, 167, false);
#line 125 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.YearSemList, new SelectList(Model.YearSemList, "YearID", "YearID"), "----Year----", new { @class = "dis-inline-block", id = "ddlYearSem" }));

#line default
#line hidden
            EndContext();
            BeginContext(4515, 13, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n");
            EndContext();
#line 127 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                 foreach (var sem in Model.SemesterList)
				{

#line default
#line hidden
            BeginContext(4581, 9, true);
            WriteLiteral("\t\t\t\t\t<td>");
            EndContext();
            BeginContext(4591, 81, false);
#line 129 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                   Write(Html.RadioButtonFor(m => m.YearSemList, sem.SemID, new { @id = "radioSemester" }));

#line default
#line hidden
            EndContext();
            BeginContext(4672, 1, true);
            WriteLiteral(" ");
            EndContext();
            BeginContext(4674, 11, false);
#line 129 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                                                                                                      Write(sem.SemName);

#line default
#line hidden
            EndContext();
            BeginContext(4685, 7, true);
            WriteLiteral("</td>\r\n");
            EndContext();
#line 130 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
				}

#line default
#line hidden
            BeginContext(4699, 27, true);
            WriteLiteral("\t\t\t</tr>\r\n\t\t\t<tr>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(4727, 81, false);
#line 133 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.RadioButton("radioTimePeriod", "3", false, new { id = "radioPeriodOption" }));

#line default
#line hidden
            EndContext();
            BeginContext(4808, 1245, true);
            WriteLiteral(@"</td>
				<td>Period</td>
				<td colspan=""1""><input class=""input"" /></td>
				<td>To</td>
				<td colspan=""1""><input class=""input"" /></td>
				<td>&#9679</td>
				<td>Check No.</td>
				<td colspan=""1""><input class=""input"" /></td>
				<td colspan=""1""><input class=""input"" /></td>
				<td rowspan=""2""><button class=""btn"" style=""width: 90%;"" id=""btnGeneratePreview"">Generate Preview</button></td>
			</tr>
			<tr>
				<td>&#9679</td>
				<td>Voucher No.</td>
				<td colspan=""3""><input class=""input"" /></td>
				<td></td>
				<td colspan=""3""><input class=""input"" /></td>
			</tr>
			<tr>
				<td>&#9679</td>
				<td>Covered Tran No.</td>
				<td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
				<td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
				<td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
				<td></td>
				<td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
				<td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
	");
            WriteLiteral("\t\t\t<td colspan=\"1\" style=\"padding:0px\"><input class=\"input\" /></td>\r\n\t\t\t</tr>\r\n\t\t\t<tr>\r\n\t\t\t\t<td>&#9679</td>\r\n\t\t\t\t<td>Subject Name</td>\r\n\t\t\t\t<td colspan=\"2\"><input class=\"input\" /></td>\r\n\t\t\t\t<td colspan=\"1\"></td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(6054, 36, false);
#line 167 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.FileFormatList));

#line default
#line hidden
            EndContext();
            BeginContext(6090, 22, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(6113, 193, false);
#line 169 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.FileFormatList, new SelectList(Model.FileFormatList, "FileFormatID", "FileFormatName"), "--File Format--", new { @class = "dis-inline-block", id = "ddlFileFormat" }));

#line default
#line hidden
            EndContext();
            BeginContext(6306, 7, true);
            WriteLiteral("\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(6314, 48, false);
#line 170 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.ValidationMessageFor(m => m.FileFormatList));

#line default
#line hidden
            EndContext();
            BeginContext(6362, 197, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td rowspan=\"2\">\r\n\t\t\t\t\t<button class=\"btn\" style=\"width: 90%;\" id=\"btnGenerateFile\">Generate File</button>\r\n\t\t\t\t</td>\r\n\t\t\t\t<td></td>\r\n\t\t\t\t<td></td>\r\n\t\t\t</tr>\r\n\t\t</table>\r\n\t</div>\r\n");
            EndContext();
#line 180 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
}

#line default
#line hidden
            BeginContext(6562, 430, true);
            WriteLiteral(@"<div id=""tbl-lbl"">
	<div id=""tbl-lbl""><p>Preview Report: <text id=""txtAsOfLabel""></text><text id=""txtDatePreviewShow""></text></p></div>
</div>
<div class=""tabContent"" style="" background-color: #fafafa;"">
	<div id=""voucherPreview"" style=""min-height: 50vh; max-height: 100%;"">
		<iframe id=""iframePreview"" frameborder=""0"" src="""" style=""position: relative; min-height: 50vh; max-height: 100%; width: 100%;""></iframe>
	</div>
	");
            EndContext();
            BeginContext(6993, 28, false);
#line 188 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
Write(Html.Partial("ModalPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(7021, 12, true);
            WriteLiteral("\r\n</div>\r\n\r\n");
            EndContext();
            BeginContext(7034, 23, false);
#line 191 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ExpenseProcessingSystem.ViewModels.HomeReportViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
