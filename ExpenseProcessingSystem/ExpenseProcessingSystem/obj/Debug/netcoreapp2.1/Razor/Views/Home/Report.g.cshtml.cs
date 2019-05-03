#pragma checksum "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "acb66da728dcbbdd820003f9324881b1c670a095"
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
#line 1 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\_ViewImports.cshtml"
using ExpenseProcessingSystem;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"acb66da728dcbbdd820003f9324881b1c670a095", @"/Views/Home/Report.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Report : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ExpenseProcessingSystem.ViewModels.TEMP_HomeReportDataFilterViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(78, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
  
	ViewData["Title"] = "Report";

#line default
#line hidden
            BeginContext(119, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(139, 416, true);
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
                BeginContext(556, 30, false);
#line 21 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                      Write(Url.Action("GetReportSubType"));

#line default
#line hidden
                EndContext();
                BeginContext(586, 2798, true);
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
                window.location.href = ""/Home/GenerateFilePreview?HomeReportFilter.ReportType="" + $('#ddlReportType').val()
                    + ""&HomeReportFilter.ReportSubType="" + $('#ddlSubType').val()
                    + ""&HomeReportFilter.FileFormat="" + $('#ddlFileFormat').val()
                    + ""&HomeReportFilter.Year="" + $('#ddlYear').val()
                    + ""&HomeReportFi");
                WriteLiteral(@"lter.Month="" + $('#ddlMonth').val()
                    + ""&HomeReportFilter.YearSem="" + $('#ddlYearSem').val()
                    + ""&HomeReportFilter.Semester="" + $('#radioSemester:checked').val()
                    + ""&HomeReportFilter.PeriodOption="" + $('#radioPeriodOption:checked').val()
                    + ""&HomeReportFilter.PeriodFrom="" + $('#PeriodFrom').val()
                    + ""&HomeReportFilter.PeriodTo="" + $('#PeriodTo').val();

			});
		});

		$(document).ready(function () {
			$(""#btnGeneratePreview"").click(function (e) {
                e.preventDefault();
                
                $('#iframePreview').prop('src', ""/Home/GenerateFilePreview?HomeReportFilter.ReportType="" + $('#ddlReportType').val()
                    + ""&HomeReportFilter.ReportSubType="" + $('#ddlSubType').val()
                    + ""&HomeReportFilter.FileFormat=3""
                    + ""&HomeReportFilter.Year="" + $('#ddlYear').val()
                    + ""&HomeReportFilter.Month="" + $('#ddlMonth'");
                WriteLiteral(@").val()
                    + ""&HomeReportFilter.YearSem="" + $('#ddlYearSem').val()
                    + ""&HomeReportFilter.Semester="" + $('#radioSemester:checked').val()
                    + ""&HomeReportFilter.PeriodOption="" + $('#radioPeriodOption:checked').val()
                    + ""&HomeReportFilter.PeriodFrom="" + $('#PeriodFrom').val()
                    + ""&HomeReportFilter.PeriodTo="" + $('#PeriodTo').val());

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
            BeginContext(3387, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 85 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
 using (Html.BeginForm())
{

#line default
#line hidden
            BeginContext(3419, 372, true);
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
            BeginContext(3792, 54, false);
#line 102 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.HomeReportFilter.ReportTypesList));

#line default
#line hidden
            EndContext();
            BeginContext(3846, 34, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td colspan=\"2\">\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(3881, 224, false);
#line 104 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.HomeReportFilter.ReportTypesList, new SelectList(Model.HomeReportFilter.ReportTypesList, "Id", "TypeName"), "----Select Report Type----", new { @class = "dis-inline-block", id = "ddlReportType" }));

#line default
#line hidden
            EndContext();
            BeginContext(4105, 21, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(4127, 57, false);
#line 106 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.HomeReportFilter.ReportSubTypesList));

#line default
#line hidden
            EndContext();
            BeginContext(4184, 34, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td colspan=\"2\">\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(4219, 254, false);
#line 108 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.HomeReportFilter.ReportTypesList, new SelectList(Model.HomeReportFilter.ReportSubTypesList, "Id", "SubTypeName"), "----Select Report Sub-Type----", new { @class = "dis-inline-block", id = "ddlSubType", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(4473, 114, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td colspan=\"4\"></td>\r\n\t\t\t</tr>\r\n\t\t\t<tr>\r\n\t\t\t\t<td colspan=\"10\"></td>\r\n\t\t\t</tr>\r\n\t\t\t<tr>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(4588, 81, false);
#line 116 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.RadioButton("radioTimePeriod", "1", false, new { id = "radioPeriodOption" }));

#line default
#line hidden
            EndContext();
            BeginContext(4669, 15, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(4685, 47, false);
#line 117 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.HomeReportFilter.YearList));

#line default
#line hidden
            EndContext();
            BeginContext(4732, 22, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(4755, 192, false);
#line 119 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.HomeReportFilter.YearList, new SelectList(Model.HomeReportFilter.YearList, "YearID", "YearID"), "----Year----", new { @class = "dis-inline-block", id = "ddlYear" }));

#line default
#line hidden
            EndContext();
            BeginContext(4947, 21, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(4969, 48, false);
#line 121 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.HomeReportFilter.MonthList));

#line default
#line hidden
            EndContext();
            BeginContext(5017, 22, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(5040, 196, false);
#line 123 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.HomeReportFilter.MonthList, new SelectList(Model.HomeReportFilter.MonthList, "MonthID", "MonthName"), "--Month--", new { @class = "dis-inline-block", id = "ddlMonth" }));

#line default
#line hidden
            EndContext();
            BeginContext(5236, 21, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(5258, 81, false);
#line 125 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.RadioButton("radioTimePeriod", "2", false, new { id = "radioPeriodOption" }));

#line default
#line hidden
            EndContext();
            BeginContext(5339, 15, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(5355, 50, false);
#line 126 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.HomeReportFilter.YearSemList));

#line default
#line hidden
            EndContext();
            BeginContext(5405, 22, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(5428, 201, false);
#line 128 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.HomeReportFilter.YearSemList, new SelectList(Model.HomeReportFilter.YearSemList, "YearID", "YearID"), "----Year----", new { @class = "dis-inline-block", id = "ddlYearSem" }));

#line default
#line hidden
            EndContext();
            BeginContext(5629, 13, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n");
            EndContext();
#line 130 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                 foreach (var sem in Model.HomeReportFilter.SemesterList)
				{

#line default
#line hidden
            BeginContext(5712, 9, true);
            WriteLiteral("\t\t\t\t\t<td>");
            EndContext();
            BeginContext(5722, 98, false);
#line 132 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                   Write(Html.RadioButtonFor(m => m.HomeReportFilter.YearSemList, sem.SemID, new { @id = "radioSemester" }));

#line default
#line hidden
            EndContext();
            BeginContext(5820, 1, true);
            WriteLiteral(" ");
            EndContext();
            BeginContext(5822, 11, false);
#line 132 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                                                                                                                       Write(sem.SemName);

#line default
#line hidden
            EndContext();
            BeginContext(5833, 7, true);
            WriteLiteral("</td>\r\n");
            EndContext();
#line 133 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
				}

#line default
#line hidden
            BeginContext(5847, 27, true);
            WriteLiteral("\t\t\t</tr>\r\n\t\t\t<tr>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(5875, 81, false);
#line 136 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.RadioButton("radioTimePeriod", "3", false, new { id = "radioPeriodOption" }));

#line default
#line hidden
            EndContext();
            BeginContext(5956, 60, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>Period</td>\r\n                <td colspan=\"1\">");
            EndContext();
            BeginContext(6017, 131, false);
#line 138 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                           Write(Html.EditorFor(m => m.HomeReportFilter.PeriodFrom, new { htmlAttributes = new { @class = "input h-20 w-97 fs-11 form -control" } }));

#line default
#line hidden
            EndContext();
            BeginContext(6148, 56, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>To</td>\r\n                <td colspan=\"1\">");
            EndContext();
            BeginContext(6205, 129, false);
#line 140 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                           Write(Html.EditorFor(m => m.HomeReportFilter.PeriodTo, new { htmlAttributes = new { @class = "input h-20 w-97 fs-11 form -control" } }));

#line default
#line hidden
            EndContext();
            BeginContext(6334, 1107, true);
            WriteLiteral(@"</td>
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
				<td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
			</tr>
			<tr>
				<td>&#9679</td>
				<td>Subject Name</td>
		");
            WriteLiteral("\t\t<td colspan=\"2\"><input class=\"input\" /></td>\r\n\t\t\t\t<td colspan=\"1\"></td>\r\n\t\t\t\t<td>");
            EndContext();
            BeginContext(7442, 53, false);
#line 170 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.HomeReportFilter.FileFormatList));

#line default
#line hidden
            EndContext();
            BeginContext(7495, 22, true);
            WriteLiteral("</td>\r\n\t\t\t\t<td>\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(7518, 227, false);
#line 172 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.HomeReportFilter.FileFormatList, new SelectList(Model.HomeReportFilter.FileFormatList, "FileFormatID", "FileFormatName"), "--File Format--", new { @class = "dis-inline-block", id = "ddlFileFormat" }));

#line default
#line hidden
            EndContext();
            BeginContext(7745, 7, true);
            WriteLiteral("\r\n\t\t\t\t\t");
            EndContext();
            BeginContext(7753, 65, false);
#line 173 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.ValidationMessageFor(m => m.HomeReportFilter.FileFormatList));

#line default
#line hidden
            EndContext();
            BeginContext(7818, 197, true);
            WriteLiteral("\r\n\t\t\t\t</td>\r\n\t\t\t\t<td rowspan=\"2\">\r\n\t\t\t\t\t<button class=\"btn\" style=\"width: 90%;\" id=\"btnGenerateFile\">Generate File</button>\r\n\t\t\t\t</td>\r\n\t\t\t\t<td></td>\r\n\t\t\t\t<td></td>\r\n\t\t\t</tr>\r\n\t\t</table>\r\n\t</div>\r\n");
            EndContext();
#line 183 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
}

#line default
#line hidden
            BeginContext(8018, 430, true);
            WriteLiteral(@"<div id=""tbl-lbl"">
	<div id=""tbl-lbl""><p>Preview Report: <text id=""txtAsOfLabel""></text><text id=""txtDatePreviewShow""></text></p></div>
</div>
<div class=""tabContent"" style="" background-color: #fafafa;"">
	<div id=""voucherPreview"" style=""min-height: 50vh; max-height: 100%;"">
		<iframe id=""iframePreview"" frameborder=""0"" src="""" style=""position: relative; min-height: 50vh; max-height: 100%; width: 100%;""></iframe>
	</div>
	");
            EndContext();
            BeginContext(8449, 28, false);
#line 191 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
Write(Html.Partial("ModalPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(8477, 12, true);
            WriteLiteral("\r\n</div>\r\n\r\n");
            EndContext();
            BeginContext(8490, 23, false);
#line 194 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ExpenseProcessingSystem.ViewModels.TEMP_HomeReportDataFilterViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
