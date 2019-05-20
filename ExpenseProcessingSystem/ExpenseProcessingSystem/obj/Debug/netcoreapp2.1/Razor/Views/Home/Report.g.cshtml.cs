#pragma checksum "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "bcb7b6bf8f9776af733341e7c1f4ee86f83b3e1c"
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"bcb7b6bf8f9776af733341e7c1f4ee86f83b3e1c", @"/Views/Home/Report.cshtml")]
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
            BeginContext(107, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(127, 7546, true);
                WriteLiteral(@"
    <script type=""text/javascript"">
        $(document).ready(function () {
            // set fields
            var radioPeriod1 = $('#radioPeriodOption1');
            var radioPeriod2 = $('#radioPeriodOption2');
            var radioPeriod3 = $('#radioPeriodOption3');
            var ddlMonth = $('#ddlMonth');
            var ddlYear = $('#ddlYear');
            var ddlMonthTo = $('#ddlMonthTo');
            var ddlYearTo = $('#ddlYearTo');
            //var ddlYearSem = $('#ddlYearSem');
            var ddlFileFormat = $('#ddlFileFormat');
            //var radioSemester1 = $('#radioSemester1');
            //var radioSemester2 = $('#radioSemester2');
            var dtPeriodFrom = $('#PeriodFrom');
            var dtPeriodTo = $('#PeriodTo');
            var txtCheck1 = $('#txtCheck1');
            var txtCheck2 = $('#txtCheck2');
            var txtVoucher1 = $('#txtVoucher1');
            var txtVoucher2 = $('#txtVoucher2');
            var txtCoveredTransNo1 = $('#txtCoveredTran");
                WriteLiteral(@"No1');
            var txtCoveredTransNo2 = $('#txtCoveredTranNo2');
            var txtCoveredTransNo3 = $('#txtCoveredTranNo3');
            var txtCoveredTransNo4 = $('#txtCoveredTranNo4');
            var txtCoveredTransNo5 = $('#txtCoveredTranNo5');
            var txtCoveredTransNo6 = $('#txtCoveredTranNo6');
            var txttxtSubjName = $('#txtSubjName');
            var btnGenerateFile = $(""#btnGenerateFile"");
            var btnGeneratePreview = $(""#btnGeneratePreview"");
            var lblValidation = $('#ValidationSummary');
            var dt = new Date();
            var today = dt.getFullYear() + ""-"" + ('0' + (dt.getMonth() + 1)).slice(-2) + ""-"" + ('0' + (dt.getDate())).slice(-2);
            var currentSem = $('hiddenCurrentSemester');

            $(""#ddlReportType"").change(function () {
                var ReportType = $(this).val();
                var select = $(""#ddlSubType"");
                if (ReportType == '') {
                    select.empty();
               ");
                WriteLiteral(@"     select.attr(""disabled"", ""disabled"");
                    select.append($('<option/>', {
                        value: 0,
                        text: ""----Select Report Sub-Type----""
                    }));
                }

                //Default fields filter

                radioPeriod1.attr(""disabled"", ""disabled"");
                radioPeriod2.attr(""disabled"", ""disabled"");
                radioPeriod3.attr(""disabled"", ""disabled"");
                ddlMonth.attr(""disabled"", ""disabled"");
                ddlYear.attr(""disabled"", ""disabled"");
                ddlMonthTo.attr(""disabled"", ""disabled"");
                ddlYearTo.attr(""disabled"", ""disabled"");
                //ddlYearSem.attr(""disabled"", ""disabled"");
                ddlFileFormat.attr(""disabled"", ""disabled"");
                //radioSemester1.attr(""disabled"", ""disabled"");
                //radioSemester2.attr(""disabled"", ""disabled"");
                dtPeriodFrom.attr(""disabled"", ""disabled"");
                dtPeriodT");
                WriteLiteral(@"o.attr(""disabled"", ""disabled"");
                txtCheck1.attr(""disabled"", ""disabled"");
                txtCheck2.attr(""disabled"", ""disabled"");
                txtVoucher1.attr(""disabled"", ""disabled"");
                txtVoucher2.attr(""disabled"", ""disabled"");
                txtCoveredTransNo1.attr(""disabled"", ""disabled"");
                txtCoveredTransNo2.attr(""disabled"", ""disabled"");
                txtCoveredTransNo3.attr(""disabled"", ""disabled"");
                txtCoveredTransNo4.attr(""disabled"", ""disabled"");
                txtCoveredTransNo5.attr(""disabled"", ""disabled"");
                txtCoveredTransNo6.attr(""disabled"", ""disabled"");
                txttxtSubjName.attr(""disabled"", ""disabled"");
                btnGenerateFile.attr(""disabled"", ""disabled"");
                btnGeneratePreview.attr(""disabled"", ""disabled"");
                radioPeriod1.prop('checked', false);
                radioPeriod2.prop('checked', false);
                radioPeriod3.prop('checked', false);
          ");
                WriteLiteral(@"      txtCheck1.text("""");
                txtCheck2.text("""");
                txtVoucher1.text("""");
                txtVoucher2.text("""");
                txtCoveredTransNo1.text("""");
                txtCoveredTransNo2.text("""");
                txtCoveredTransNo3.text("""");
                txtCoveredTransNo4.text("""");
                txtCoveredTransNo5.text("""");
                txtCoveredTransNo6.text("""");
                txttxtSubjName.text("""");
                ddlMonth.val(dt.getMonth() + 1);
                ddlYear.val(dt.getFullYear());
                ddlMonthTo.val(dt.getMonth() + 1);
                ddlYearTo.val(dt.getFullYear());
                //ddlYearSem.val(dt.getFullYear());
                ddlFileFormat.val('1');
                lblValidation.hide();
                dtPeriodFrom.val(today);
                dtPeriodTo.val(today);

                if (currentSem = 1) {
                    //radioSemester1.prop('checked', true);
                } else {
                    /");
                WriteLiteral(@"/radioSemester2.prop('checked', true);
                }

                if (ReportType == ""2"" || ReportType == ""5"") {
                    radioPeriod1.prop('checked', true);
                    radioPeriod1.removeAttr(""disabled"");
                    ddlYear.removeAttr(""disabled"");
                    ddlMonth.removeAttr(""disabled"");

                } else if (ReportType == ""3"") {
                    radioPeriod2.prop('checked', true);
                    ddlMonth.removeAttr(""disabled"");
                    ddlYear.removeAttr(""disabled"");
                    ddlMonthTo.removeAttr(""disabled"");
                    ddlYearTo.removeAttr(""disabled"");
                    radioPeriod1.removeAttr(""disabled"");
                    //ddlYearSem.removeAttr(""disabled"");
                    //radioSemester1.removeAttr(""disabled"");
                    //radioSemester2.removeAttr(""disabled"");
                    radioPeriod2.removeAttr(""disabled"");
                } else if (ReportType == ""4"") {
     ");
                WriteLiteral(@"               radioPeriod1.prop('checked', true);
                    ddlMonth.removeAttr(""disabled"");
                    ddlYear.removeAttr(""disabled"");
                    ddlMonthTo.removeAttr(""disabled"");
                    ddlYearTo.removeAttr(""disabled"");
                    radioPeriod1.removeAttr(""disabled"");
                } else if (ReportType == ""10"") {
                    radioPeriod1.prop('checked', true);
                    radioPeriod1.removeAttr(""disabled"");
                    radioPeriod2.removeAttr(""disabled"");
                    radioPeriod3.removeAttr(""disabled"");
                    ddlYear.removeAttr(""disabled"");
                    ddlMonth.removeAttr(""disabled"");
                } else if (ReportType == ""13"") {
                    radioPeriod1.prop('checked', true);
                    radioPeriod1.removeAttr(""disabled"");
                    radioPeriod2.removeAttr(""disabled"");
                    radioPeriod3.removeAttr(""disabled"");
                    ddlYear");
                WriteLiteral(@".removeAttr(""disabled"");
                    ddlMonth.removeAttr(""disabled"");
                }

                if (ReportType != 0 || ReportType != '') {
                    btnGenerateFile.removeAttr(""disabled"");
                    btnGeneratePreview.removeAttr(""disabled"");
                    ddlFileFormat.removeAttr(""disabled"");

                    $.getJSON('");
                EndContext();
                BeginContext(7674, 30, false);
#line 157 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                          Write(Url.Action("GetReportSubType"));

#line default
#line hidden
                EndContext();
                BeginContext(7704, 8726, true);
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
                }
            });

            $("".radioPeriodOption"").change(function () {

                //Default fields filter
                if (currentSem = 1)");
                WriteLiteral(@" {
                    //radioSemester1.prop('checked', true);
                } else {
                    //radioSemester2.prop('checked', true);
                }

                ddlMonth.attr(""disabled"", ""disabled"");
                ddlYear.attr(""disabled"", ""disabled"");
                ddlMonthTo.attr(""disabled"", ""disabled"");
                ddlYearTo.attr(""disabled"", ""disabled"");
                //ddlYearSem.attr(""disabled"", ""disabled"");
                //radioSemester1.attr(""disabled"", ""disabled"");
                //radioSemester2.attr(""disabled"", ""disabled"");
                dtPeriodFrom.attr(""disabled"", ""disabled"");
                dtPeriodTo.attr(""disabled"", ""disabled"");

                switch ($(this).val()) {
                    case ""1"":
                        ddlYear.removeAttr(""disabled"");
                        ddlMonth.removeAttr(""disabled"");
                        break;
                    case ""2"":
                        //ddlYearSem.removeAttr(""disabled"");
    ");
                WriteLiteral(@"                    //radioSemester1.removeAttr(""disabled"");
                        //radioSemester2.removeAttr(""disabled"");
                        break;
                    case ""3"":
                        dtPeriodFrom.removeAttr(""disabled"");
                        dtPeriodTo.removeAttr(""disabled"");
                        break;
                }
            });
        });

        $(document).ready(function () {
            $(""#btnGenerateFile"").click(function (e) {
                e.preventDefault();
                $.ajax({
                    type: 'POST',
                    url: '/Home/HomeReportValidation',
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                    data: {
                        ReportType: $('#ddlReportType').val(),
                        ReportSubType: $('#ddlSubType').val(),
                        FileFormat: '3',
                        Year: $('#ddlYear').val(),
 ");
                WriteLiteral(@"                       Month: $('#ddlMonth').val(),
                        YearTo: $('#ddlYearTo').val(),
                        MonthTo: $('#ddlMonthTo').val(),
                        //YearSem: $('#ddlYearSem').val(),
                        //Semester: $('.radioSemester:checked').val(),
                        PeriodOption: $('.radioPeriodOption:checked').val(),
                        PeriodFrom: $('#PeriodFrom').val(),
                        PeriodTo: $('#PeriodTo').val()
                    },
                    success: function (result) {
                        if (result.message == ""Invalid"") {
                            var $summaryUl = $("".validation-summary-valid"").find(""ul"");
                            $summaryUl.empty();
                            $.each(result.items, function (index) {
                                $summaryUl.append($(""<li>"").text(result.items[index]));
                            });
                            $('#ValidationSummary').show();
      ");
                WriteLiteral(@"                  }
                        else {
                            $('#ValidationSummary').hide();
                            window.location.href = ""/Home/GenerateFilePreview?ReportType="" + $('#ddlReportType').val()
                                + ""&ReportSubType="" + $('#ddlSubType').val()
                                + ""&FileFormat="" + $('#ddlFileFormat').val()
                                + ""&Year="" + $('#ddlYear').val()
                                + ""&Month="" + $('#ddlMonth').val()
                                + ""&YearTo="" + $('#ddlYearTo').val()
                                + ""&MonthTo="" + $('#ddlMonthTo').val()
                                //+ ""&YearSem="" + $('#ddlYearSem').val()
                                //+ ""&Semester="" + $('.radioSemester:checked').val()
                                + ""&PeriodOption="" + $('.radioPeriodOption:checked').val()
                                + ""&PeriodFrom="" + $('#PeriodFrom').val()
                               ");
                WriteLiteral(@" + ""&PeriodTo="" + $('#PeriodTo').val()
                                + ""&TestReportType="" + $('#ddlReportType').val();
                        }
                    },
                    error: function (result) {
                        alert('Error');
                    }
                });
            });
            $(""#btnGeneratePreview"").click(function (e) {
                e.preventDefault();

                $.ajax({
                    type: 'POST',
                    url: '/Home/HomeReportValidation',
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                    data: {
                        ReportType: $('#ddlReportType').val(),
                        ReportSubType: $('#ddlSubType').val(),
                        FileFormat: '3',
                        Year: $('#ddlYear').val(),
                        Month: $('#ddlMonth').val(),
                        YearTo: $('#ddlYearTo').val(),");
                WriteLiteral(@"
                        MonthTo: $('#ddlMonthTo').val(),
                        //YearSem: $('#ddlYearSem').val(),
                        //Semester: $('.radioSemester:checked').val(),
                        PeriodOption: $('.radioPeriodOption:checked').val(),
                        PeriodFrom: $('#PeriodFrom').val(),
                        PeriodTo: $('#PeriodTo').val()
                    },
                    success: function (result) {
                        if (result.message == ""Invalid"") {
                            var $summaryUl = $("".validation-summary-valid"").find(""ul"");
                            $summaryUl.empty();
                            $.each(result.items, function (index) {
                                $summaryUl.append($(""<li>"").text(result.items[index]));
                            });
                            $('#ValidationSummary').show();
                        }
                        else {
                            $('#ValidationSummary').hid");
                WriteLiteral(@"e();
                            $('#iframePreview').prop('src', ""/Home/GenerateFilePreview?ReportType="" + $('#ddlReportType').val()
                                + ""&ReportSubType="" + $('#ddlSubType').val()
                                + ""&FileFormat=3""
                                + ""&Year="" + $('#ddlYear').val()
                                + ""&Month="" + $('#ddlMonth').val()
                                + ""&YearTo="" + $('#ddlYearTo').val()
                                + ""&MonthTo="" + $('#ddlMonthTo').val()
                                //+ ""&YearSem="" + $('#ddlYearSem').val()
                                //+ ""&Semester="" + $('.radioSemester:checked').val()
                                + ""&PeriodOption="" + $('.radioPeriodOption:checked').val()
                                + ""&PeriodFrom="" + $('#PeriodFrom').val()
                                + ""&PeriodTo="" + $('#PeriodTo').val());

                            var dt = new Date();
                            var ");
                WriteLiteral(@"date_time = ('0' + (dt.getMonth() + 1)).slice(-2) + ""/"" + dt.getDate() + ""/"" + dt.getFullYear() + "" "" + dt.getHours() + "":"" + dt.getMinutes() + "":"" + dt.getSeconds();

                            $('#txtAsOfLabel').text(""As of "");
                            $('#txtDatePreviewShow').text(date_time);
                        }
                    },
                    error: function (result) {
                        alert('Error');
                    }
                });
            });
        });
    </script>
");
                EndContext();
            }
            );
#line 329 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
 using (Html.BeginForm())
{

#line default
#line hidden
            BeginContext(16463, 591, true);
            WriteLiteral(@"    <div class=""m-t-10"">
        <table class=""table voucher-tbl"">
            <colgroup>
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
            </colgroup>
            <tr>
                <td></td>
                <td>");
            EndContext();
            BeginContext(17055, 37, false);
#line 347 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.ReportTypesList));

#line default
#line hidden
            EndContext();
            BeginContext(17092, 61, true);
            WriteLiteral("</td>\r\n                <td colspan=\"2\">\r\n                    ");
            EndContext();
            BeginContext(17154, 190, false);
#line 349 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.ReportTypesList, new SelectList(Model.ReportTypesList, "Id", "TypeName"), "----Select Report Type----", new { @class = "dis-inline-block", id = "ddlReportType" }));

#line default
#line hidden
            EndContext();
            BeginContext(17344, 45, true);
            WriteLiteral("\r\n                </td>\r\n                <td>");
            EndContext();
            BeginContext(17390, 40, false);
#line 351 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.ReportSubTypesList));

#line default
#line hidden
            EndContext();
            BeginContext(17430, 61, true);
            WriteLiteral("</td>\r\n                <td colspan=\"2\">\r\n                    ");
            EndContext();
            BeginContext(17492, 220, false);
#line 353 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.ReportTypesList, new SelectList(Model.ReportSubTypesList, "Id", "SubTypeName"), "----Select Report Sub-Type----", new { @class = "dis-inline-block", id = "ddlSubType", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(17712, 136, true);
            WriteLiteral("\r\n                </td>\r\n                <td colspan=\"4\"></td>\r\n            </tr>\r\n            <tr>\r\n                <td colspan=\"10\">\r\n");
            EndContext();
            BeginContext(17892, 104, true);
            WriteLiteral("                    <div id=\"ValidationSummary\" style=\"display:none\" class=\"\">\r\n                        ");
            EndContext();
            BeginContext(17997, 68, false);
#line 361 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                   Write(Html.ValidationSummary(false, "", new { @id = "validationSummary" }));

#line default
#line hidden
            EndContext();
            BeginContext(18065, 110, true);
            WriteLiteral("\r\n                    </div>\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <td>");
            EndContext();
            BeginContext(18176, 135, false);
#line 366 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.RadioButton("radioTimePeriod", "1", false, new { id = "radioPeriodOption1", @class = "radioPeriodOption", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(18311, 33, true);
            WriteLiteral(" From:</td>\r\n                <td>");
            EndContext();
            BeginContext(18345, 30, false);
#line 367 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.YearList));

#line default
#line hidden
            EndContext();
            BeginContext(18375, 49, true);
            WriteLiteral("</td>\r\n                <td>\r\n                    ");
            EndContext();
            BeginContext(18425, 171, false);
#line 369 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.YearList, new SelectList(Model.YearList, "YearID", "YearID"), null, new { @class = "dis-inline-block", id = "ddlYear", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(18596, 45, true);
            WriteLiteral("\r\n                </td>\r\n                <td>");
            EndContext();
            BeginContext(18642, 31, false);
#line 371 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.MonthList));

#line default
#line hidden
            EndContext();
            BeginContext(18673, 49, true);
            WriteLiteral("</td>\r\n                <td>\r\n                    ");
            EndContext();
            BeginContext(18723, 181, false);
#line 373 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.CurrentMonth, new SelectList(Model.MonthList, "MonthID", "MonthName"), null, new { @class = "dis-inline-block", id = "ddlMonth", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(18904, 45, true);
            WriteLiteral("\r\n                </td>\r\n                <td>");
            EndContext();
            BeginContext(18950, 135, false);
#line 375 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.RadioButton("radioTimePeriod", "2", false, new { id = "radioPeriodOption2", @class = "radioPeriodOption", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(19085, 31, true);
            WriteLiteral(" To:</td>\r\n                <td>");
            EndContext();
            BeginContext(19117, 33, false);
#line 376 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.YearSemList));

#line default
#line hidden
            EndContext();
            BeginContext(19150, 29, true);
            WriteLiteral("</td>\r\n                <td>\r\n");
            EndContext();
            BeginContext(19386, 20, true);
            WriteLiteral("                    ");
            EndContext();
            BeginContext(19407, 173, false);
#line 379 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.YearList, new SelectList(Model.YearList, "YearID", "YearID"), null, new { @class = "dis-inline-block", id = "ddlYearTo", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(19580, 25, true);
            WriteLiteral("\r\n                </td>\r\n");
            EndContext();
            BeginContext(19896, 42, true);
            WriteLiteral("                <td>\r\n                    ");
            EndContext();
            BeginContext(19939, 183, false);
#line 386 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.CurrentMonth, new SelectList(Model.MonthList, "MonthID", "MonthName"), null, new { @class = "dis-inline-block", id = "ddlMonthTo", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(20122, 82, true);
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <td>");
            EndContext();
            BeginContext(20205, 135, false);
#line 390 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.RadioButton("radioTimePeriod", "3", false, new { id = "radioPeriodOption3", @class = "radioPeriodOption", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(20340, 72, true);
            WriteLiteral("</td>\r\n                <td>Period</td>\r\n                <td colspan=\"1\">");
            EndContext();
            BeginContext(20413, 151, false);
#line 392 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                           Write(Html.EditorFor(m => m.PeriodFrom, new { htmlAttributes = new { @class = "input h-20 w-97 fs-11 form-control", @type = "date", disabled = "disabled"} }));

#line default
#line hidden
            EndContext();
            BeginContext(20564, 68, true);
            WriteLiteral("</td>\r\n                <td>To</td>\r\n                <td colspan=\"1\">");
            EndContext();
            BeginContext(20633, 149, false);
#line 394 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                           Write(Html.EditorFor(m => m.PeriodTo, new { htmlAttributes = new { @class = "input h-20 w-97 fs-11 form-control", @type = "date", disabled = "disabled"} }));

#line default
#line hidden
            EndContext();
            BeginContext(20782, 1908, true);
            WriteLiteral(@"</td>
                <td>&#9679</td>
                <td>Check No.</td>
                <td colspan=""1""><input class=""input"" id=""txtCheck1"" disabled=""disabled"" /></td>
                <td colspan=""1""><input class=""input"" id=""txtCheck2"" disabled=""disabled"" /></td>
                <td rowspan=""2""><button class=""btn"" style=""width: 90%;"" id=""btnGeneratePreview"" disabled=""disabled"">Generate Preview</button></td>
            </tr>
            <tr>
                <td>&#9679</td>
                <td>Voucher No.</td>
                <td colspan=""3""><input class=""input"" id=""txtVoucher1"" disabled=""disabled"" /></td>
                <td></td>
                <td colspan=""3""><input class=""input"" id=""txtVoucher2"" disabled=""disabled"" /></td>
            </tr>
            <tr>
                <td>&#9679</td>
                <td>Covered Tran No.</td>
                <td colspan=""1"" style=""padding:0px""><input class=""input"" id=""txtCoveredTranNo1"" disabled=""disabled"" /></td>
                <td colspan=""1"" sty");
            WriteLiteral(@"le=""padding:0px""><input class=""input"" id=""txtCoveredTranNo2"" disabled=""disabled"" /></td>
                <td colspan=""1"" style=""padding:0px""><input class=""input"" id=""txtCoveredTranNo3"" disabled=""disabled"" /></td>
                <td></td>
                <td colspan=""1"" style=""padding:0px""><input class=""input"" id=""txtCoveredTranNo4"" disabled=""disabled"" /></td>
                <td colspan=""1"" style=""padding:0px""><input class=""input"" id=""txtCoveredTranNo5"" disabled=""disabled"" /></td>
                <td colspan=""1"" style=""padding:0px""><input class=""input"" id=""txtCoveredTranNo6"" disabled=""disabled"" /></td>
            </tr>
            <tr>
                <td>&#9679</td>
                <td>Subject Name</td>
                <td colspan=""2""><input class=""input"" id=""txtSubjName"" disabled=""disabled"" /></td>
                <td colspan=""1""></td>
                <td>");
            EndContext();
            BeginContext(22691, 36, false);
#line 424 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.LabelFor(m => m.FileFormatList));

#line default
#line hidden
            EndContext();
            BeginContext(22727, 49, true);
            WriteLiteral("</td>\r\n                <td>\r\n                    ");
            EndContext();
            BeginContext(22777, 203, false);
#line 426 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.DropDownListFor(m => m.FileFormatList, new SelectList(Model.FileFormatList, "FileFormatID", "FileFormatName"), null, new { @class = "dis-inline-block", id = "ddlFileFormat", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(22980, 22, true);
            WriteLiteral("\r\n                    ");
            EndContext();
            BeginContext(23003, 48, false);
#line 427 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
               Write(Html.ValidationMessageFor(m => m.FileFormatList));

#line default
#line hidden
            EndContext();
            BeginContext(23051, 310, true);
            WriteLiteral(@"
                </td>
                <td rowspan=""2"">
                    <button class=""btn"" style=""width: 90%;"" id=""btnGenerateFile"" disabled=""disabled"">Generate File</button>
                </td>
                <td></td>
                <td></td>
            </tr>
        </table>
    </div>
");
            EndContext();
#line 437 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
}

#line default
#line hidden
            BeginContext(23364, 454, true);
            WriteLiteral(@"<div id=""tbl-lbl"">
    <div id=""tbl-lbl""><p>Preview Report: <text id=""txtAsOfLabel""></text><text id=""txtDatePreviewShow""></text></p></div>
</div>
<div class=""tabContent"" style="" background-color: #fafafa;"">
    <div id=""voucherPreview"" style=""min-height: 50vh; max-height: 100%;"">
        <iframe id=""iframePreview"" frameborder=""0"" src="""" style=""position: relative; min-height: 50vh; max-height: 100%; width: 100%;""></iframe>
    </div>
</div>

");
            EndContext();
            BeginContext(23819, 23, false);
#line 447 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
Write(Html.AntiForgeryToken());

#line default
#line hidden
            EndContext();
            BeginContext(23842, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(23845, 109, false);
#line 448 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
Write(Html.Hidden("hiddenCurrentSemester", Model.CurrentSemester.ToString(), new { @id = "hiddenCurrentSemester" }));

#line default
#line hidden
            EndContext();
            BeginContext(23954, 2, true);
            WriteLiteral("\r\n");
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
