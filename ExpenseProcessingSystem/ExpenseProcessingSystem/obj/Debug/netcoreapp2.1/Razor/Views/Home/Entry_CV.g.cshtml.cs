#pragma checksum "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "647611bede213d62848548d4a332aeeb6eee4728"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Entry_CV), @"mvc.1.0.view", @"/Views/Home/Entry_CV.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Entry_CV.cshtml", typeof(AspNetCore.Views_Home_Entry_CV))]
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
#line 1 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\_ViewImports.cshtml"
using ExpenseProcessingSystem;

#line default
#line hidden
#line 4 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
using ExpenseProcessingSystem.ViewModels;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"647611bede213d62848548d4a332aeeb6eee4728", @"/Views/Home/Entry_CV.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Entry_CV : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<EntreeCVViewModelList>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
  
    ViewData["Title"] = "Entry_CV";

#line default
#line hidden
            BeginContext(144, 28, true);
            WriteLiteral("\r\n<div class=\"tabContent\">\r\n");
            EndContext();
#line 9 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
     using (Html.BeginForm("Entry_NewCV", "Home", FormMethod.Post, new { @class = "validate-form" }))
    {

#line default
#line hidden
            BeginContext(282, 120, true);
            WriteLiteral("        <div id=\"tbl-lbl\" class=\"flex-sb\">\r\n            <div class=\"dis-inline-block\">Date: <p class=\"dis-inline-block\">");
            EndContext();
            BeginContext(403, 104, false);
#line 12 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                                                       Write(Html.TextBoxFor(model => model.expenseDate, new { type = "date", @readonly = "true", @class = "input" }));

#line default
#line hidden
            EndContext();
            BeginContext(507, 83, true);
            WriteLiteral("</p> </div>\r\n            <div class=\"dis-inline-block\"><p class=\"dis-inline-block\">");
            EndContext();
            BeginContext(591, 89, false);
#line 13 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                                                 Write(Html.TextBoxFor(model => model.expenseYear, new { @readonly = "true", @class = "input" }));

#line default
#line hidden
            EndContext();
            BeginContext(680, 35, true);
            WriteLiteral("</p> - <p class=\"dis-inline-block\">");
            EndContext();
            BeginContext(716, 87, false);
#line 13 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                                                                                                                                                                              Write(Html.TextBoxFor(model => model.expenseId, new { @readonly = "true", @class = "input" }));

#line default
#line hidden
            EndContext();
            BeginContext(803, 101, true);
            WriteLiteral("</p></div>\r\n            <div class=\"dis-inline-block float-r\">Check No : <p class=\"dis-inline-block\">");
            EndContext();
            BeginContext(905, 85, false);
#line 14 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                                                                    Write(Html.TextBoxFor(model => model.checkNo, new { @readonly = "true", @class = "input" }));

#line default
#line hidden
            EndContext();
            BeginContext(990, 179, true);
            WriteLiteral("</p></div>\r\n        </div>\r\n        <div id=\"tbl-lbl\">\r\n            <div class=\"dis-inline-block\"><p>Payee:</p></div>\r\n            <div class=\"dis-inline-block\">\r\n                ");
            EndContext();
            BeginContext(1170, 135, false);
#line 19 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
           Write(Html.DropDownListFor(m => m.vendor, new SelectList(Model.systemValues.vendors,"Value","Text",Model.systemValues.vendors.SelectedValue)));

#line default
#line hidden
            EndContext();
            BeginContext(1305, 1680, true);
            WriteLiteral(@"
            </div>
        </div>
        <div class=""flex-c"">
            <table class=""table table-bordered table-striped voucher-tbl w-97"" id=""inputTable"">
                <colgroup>
                    <col style=""width:20%;"" />
                    <col style=""width:15%;"" />
                    <col style=""width:3%;"" />
                    <col style=""width:10%"" />
                    <col style=""width:2%;"" />
                    <col style=""width:5%;"" />
                    <col style=""width:2%;"" />
                    <col style=""width:5%;"" />
                    <col style=""width:7%;"" />
                    <col style=""width:10%;"" />
                    <col style=""width:10%;"" />
                    <col style=""width:10%;"" />
                    <col style=""width:2%;"" />
                </colgroup>
                <thead>
                    <tr>
                        <th rowspan=""2"">Gbase Remarks</th>
                        <th rowspan=""2"">Account</th>
                     ");
            WriteLiteral(@"   <th rowspan=""2"">FBT</th>
                        <th rowspan=""2"">Department</th>
                        <th colspan=""2"" rowspan=""2"">VAT</th>
                        <th colspan=""2"" rowspan=""2"">EWT</th>
                        <th rowspan=""2"">Currency</th>
                        <th>Debit</th>
                        <th colspan=""2"">Credit</th>
                    </tr>
                    <tr>
                        <th scope=""col"">Gross Amount</th>
                        <th scope=""col"">EWT Amount</th>
                        <th scope=""col"">Cash</th>
                    </tr>
                </thead>
                <tbody>
");
            EndContext();
#line 58 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                     for (int i = 0; i < Model.EntreeCV.Count; i++)
                    {

#line default
#line hidden
            BeginContext(3077, 27, true);
            WriteLiteral("                        <tr");
            EndContext();
            BeginWriteAttribute("id", " id=\"", 3104, "\"", 3116, 2);
            WriteAttributeValue("", 3109, "item_", 3109, 5, true);
#line 60 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
WriteAttributeValue("", 3114, i, 3114, 2, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(3117, 55, true);
            WriteLiteral(">\r\n                            <td class=\"p-b-1 p-t-1\">");
            EndContext();
            BeginContext(3173, 81, false);
#line 61 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                               Write(Html.TextBoxFor(x => Model.EntreeCV[i].GBaseRemarks, new { @class = "input100" }));

#line default
#line hidden
            EndContext();
            BeginContext(3254, 73, true);
            WriteLiteral("</td>\r\n                            <td>\r\n                                ");
            EndContext();
            BeginContext(3328, 176, false);
#line 63 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                           Write(Html.DropDownListFor(x => Model.EntreeCV[i].account, new SelectList(Model.systemValues.acc, "Value", "Text", Model.systemValues.acc.SelectedValue), new { @class = "input100" }));

#line default
#line hidden
            EndContext();
            BeginContext(3504, 95, true);
            WriteLiteral("\r\n                            </td>\r\n                            <td style=\"text-align:center\">");
            EndContext();
            BeginContext(3600, 44, false);
#line 65 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                                     Write(Html.CheckBoxFor(x => Model.EntreeCV[i].fbt));

#line default
#line hidden
            EndContext();
            BeginContext(3644, 73, true);
            WriteLiteral("</td>\r\n                            <td>\r\n                                ");
            EndContext();
            BeginContext(3718, 175, false);
#line 67 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                           Write(Html.DropDownListFor(x => Model.EntreeCV[i].dept, new SelectList(Model.systemValues.dept, "Value", "Text", Model.systemValues.dept.SelectedValue), new { @class = "input100" }));

#line default
#line hidden
            EndContext();
            BeginContext(3893, 95, true);
            WriteLiteral("\r\n                            </td>\r\n                            <td style=\"text-align:center\">");
            EndContext();
            BeginContext(3989, 73, false);
#line 69 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                                     Write(Html.CheckBoxFor(x => Model.EntreeCV[i].chkVat,new { @class = "chkVat" }));

#line default
#line hidden
            EndContext();
            BeginContext(4062, 39, true);
            WriteLiteral("</td>\r\n                            <td>");
            EndContext();
            BeginContext(4102, 98, false);
#line 70 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                           Write(Html.TextBoxFor(x => Model.EntreeCV[i].vat, new { @class = "input100 txtVat", @readonly = "true"}));

#line default
#line hidden
            EndContext();
            BeginContext(4200, 65, true);
            WriteLiteral("</td>\r\n                            <td style=\"text-align:center\">");
            EndContext();
            BeginContext(4266, 47, false);
#line 71 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                                     Write(Html.CheckBoxFor(x => Model.EntreeCV[i].chkEwt));

#line default
#line hidden
            EndContext();
            BeginContext(4313, 39, true);
            WriteLiteral("</td>\r\n                            <td>");
            EndContext();
            BeginContext(4353, 92, false);
#line 72 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                           Write(Html.TextBoxFor(x => Model.EntreeCV[i].ewt, new { @class = "input100", @readonly = "true" }));

#line default
#line hidden
            EndContext();
            BeginContext(4445, 39, true);
            WriteLiteral("</td>\r\n                            <td>");
            EndContext();
            BeginContext(4485, 72, false);
#line 73 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                           Write(Html.TextBoxFor(x => Model.EntreeCV[i].ccy, new { @class = "input100" }));

#line default
#line hidden
            EndContext();
            BeginContext(4557, 39, true);
            WriteLiteral("</td>\r\n                            <td>");
            EndContext();
            BeginContext(4597, 79, false);
#line 74 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                           Write(Html.TextBoxFor(x => Model.EntreeCV[i].debitGross, new { @class = "input100" }));

#line default
#line hidden
            EndContext();
            BeginContext(4676, 39, true);
            WriteLiteral("</td>\r\n                            <td>");
            EndContext();
            BeginContext(4716, 76, false);
#line 75 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                           Write(Html.TextBoxFor(x => Model.EntreeCV[i].credEwt, new { @class = "input100" }));

#line default
#line hidden
            EndContext();
            BeginContext(4792, 39, true);
            WriteLiteral("</td>\r\n                            <td>");
            EndContext();
            BeginContext(4832, 77, false);
#line 76 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                           Write(Html.TextBoxFor(x => Model.EntreeCV[i].credCash, new { @class = "input100" }));

#line default
#line hidden
            EndContext();
            BeginContext(4909, 150, true);
            WriteLiteral("</td>\r\n                            <td><a class=\"expenseAmortization glyphicon glyphicon-list-alt\" href=\"#\"></a></td>\r\n                        </tr>\r\n");
            EndContext();
#line 79 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                    }

#line default
#line hidden
            BeginContext(5082, 932, true);
            WriteLiteral(@"
                    <tr>
                        <td style=""border:none; background-color:#f2f2f2"" rowspan=""2""><a id=""addRow"" href=""#"" class=""glyphicon glyphicon-plus""></a></td>
                        <td colspan=""8"" style=""text-align:right; border:none; background-color:#f2f2f2"">Subtotal : </td>
                        <td rowspan=""2""></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan=""8"" style=""text-align:right; border:none; background-color:#f2f2f2 "">Total : </td>
                        <td colspan=""2""></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div id=""voucherPreview"" class=""border-solid"">
            <div id=""tbl-lbl"">
                <div class=""dis-inline-block""><p>Preview of Voucher:</p></div>
            </div>
            ");
            EndContext();
            BeginContext(6015, 25, false);
#line 99 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
       Write(Html.Partial("CVPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(6040, 187, true);
            WriteLiteral("\r\n        </div>\r\n        <div id=\"entry-status\" class=\"m-t-10 m-b-10\">\r\n            <div class=\"dis-inline-block\"><label>Status:</label></div>\r\n            <div class=\"dis-inline-block\">");
            EndContext();
            BeginContext(6228, 84, false);
#line 103 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                     Write(Html.TextBoxFor(model => model.status, new { @readonly = "true", @class = "input" }));

#line default
#line hidden
            EndContext();
            BeginContext(6312, 124, true);
            WriteLiteral("</div>\r\n            <div class=\"dis-inline-block\"><label>Approver:</label></div>\r\n            <div class=\"dis-inline-block\">");
            EndContext();
            BeginContext(6437, 86, false);
#line 105 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                     Write(Html.TextBoxFor(model => model.approver, new { @readonly = "true", @class = "input" }));

#line default
#line hidden
            EndContext();
            BeginContext(6523, 126, true);
            WriteLiteral("</div>\r\n            <div class=\"dis-inline-block\"><label>Verifier:</label></div>\r\n            <div class=\"dis-inline-block\">\r\n");
            EndContext();
#line 108 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                 if (Model.verifier == null)
                {

#line default
#line hidden
            BeginContext(6714, 105, true);
            WriteLiteral("                    <p class=\"dis-inline-block\"><input type=\"text\" readonly=\"true\" class=\"input\" /></p>\r\n");
            EndContext();
#line 111 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                }
                else
                {
                    for (int i = 0; i < Model.verifier.Count; i++)
                    {

#line default
#line hidden
            BeginContext(6970, 48, true);
            WriteLiteral("                    <p class=\"dis-inline-block\">");
            EndContext();
            BeginContext(7019, 95, false);
#line 116 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                                           Write(Html.TextBoxFor(model => model.verifier[i], new { @readonly = "true", @class = "m-l-5 input" }));

#line default
#line hidden
            EndContext();
            BeginContext(7114, 6, true);
            WriteLiteral("</p>\r\n");
            EndContext();
#line 117 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                    }
                }

#line default
#line hidden
            BeginContext(7162, 1392, true);
            WriteLiteral(@"            </div>
        </div>
        <div id=""entry-controls"" class=""m-b-10"">
            <div class=""flex-sb"">
                <div>
                    <div class=""dis-inline-block tbl-btn""><button>Save</button></div>
                    <div class=""dis-inline-block tbl-btn""><button>Delete</button></div>
                    <div class=""dis-inline-block tbl-btn""><button>Modify</button></div>
                </div>
                <div>
                    <div class=""dis-inline-block tbl-btn""><button id=""reversal_entry"">Create Reversal Entry</button></div>
                </div>
                <div>
                    <div class=""dis-inline-block tbl-btn m-l-41""><button>Print Check</button></div>
                </div>
                
            </div>
            <div class=""flex-sb"">
                <div>
                    <div class=""dis-inline-block tbl-btn""><button>Approve/Verify</button></div>
                    <div class=""dis-inline-block tbl-btn""><button>Reject</butto");
            WriteLiteral(@"n></div>
                </div>
                <div>
                    <div class=""dis-inline-block tbl-btn""><button>Delete GBase Post</button></div>
                </div>
                <div>
                    <div class=""dis-inline-block tbl-btn""><button>Print BIR Certificate</button></div>
                </div>
            </div>
        </div>
");
            EndContext();
#line 149 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
    }

#line default
#line hidden
            BeginContext(8561, 6, true);
            WriteLiteral("\r\n    ");
            EndContext();
            BeginContext(8568, 28, false);
#line 151 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
Write(Html.Partial("ModalPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(8596, 333, true);
            WriteLiteral(@"
</div>

<script>
    $(function () {
        $(""#addRow"").click(function (e) {
            var itemCount = document.getElementById('inputTable').getElementsByTagName('tbody')[0].childElementCount - 2;
            var tableRef = document.getElementById('inputTable').getElementsByTagName('tbody')[0];

            var acc = ");
            EndContext();
            BeginContext(8930, 48, false);
#line 160 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                 Write(Html.Raw(Json.Serialize(Model.systemValues.acc)));

#line default
#line hidden
            EndContext();
            BeginContext(8978, 26, true);
            WriteLiteral(";\r\n            var dept = ");
            EndContext();
            BeginContext(9005, 49, false);
#line 161 "C:\Users\gene.delacruz\Documents\__AAA.EPS\source\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_CV.cshtml"
                  Write(Html.Raw(Json.Serialize(Model.systemValues.dept)));

#line default
#line hidden
            EndContext();
            BeginContext(9054, 4075, true);
            WriteLiteral(@";

            // Insert a row in the table at the last row
            var newRow = tableRef.insertRow(itemCount);
            newRow.id = ""item_"" + itemCount

            var htmlContent = '<td class=""p-b-1 p-t-1""><input class=""input100"" id=""EntreeCV_' + itemCount + '__GBaseRemarks"" name=""EntreeCV[' + itemCount + '].GBaseRemarks"" type=""text"" value=""""></td>' +
                '<td><select class=""input100"" id=""EntreeCV_' + itemCount + '__account"" name=""EntreeCV[' + itemCount + '].account"">';

            for (var i = 0; i < acc.length; i++) {
                htmlContent += '<option value=""' + acc[i][""value""] + '"">' + acc[i][""text""] + '</option>';
            }

            htmlContent += '</select></td>' +
                '<td style=""text-align:center""><input data-val=""true"" data-val-required=""The fbt field is required."" id=""EntreeCV_' + itemCount + '__fbt"" name=""EntreeCV[' + itemCount + '].fbt"" type=""checkbox"" value=""true""></td>' +
                '<td><select class=""input100"" id=""EntreeCV_' +");
            WriteLiteral(@" itemCount + '__dept"" name=""EntreeCV[' + itemCount + '].dept"">';
            for (var i = 0; i < dept.length; i++) {
                htmlContent += '<option value=""' + dept[i][""value""] + '"">' + dept[i][""text""] + '</option>';
            }

            htmlContent += '</select></td>' +
                '<td style=""text-align:center""><input class=""chkVat"" data-val=""true"" data-val-required=""The chkVat field is required."" id=""EntreeCV_' + itemCount + '__chkVat"" name=""EntreeCV[' + itemCount + '].chkVat"" type=""checkbox"" value=""true""></td>' +
                '<td><input class=""input100 txtVat"" data-val=""true"" data-val-number=""The field vat must be a number."" data-val-required=""The vat field is required."" id=""EntreeCV_' + itemCount + '__vat"" name=""EntreeCV[' + itemCount + '].vat"" readonly=""true"" type=""text"" value=""0""></td>' +
                '<td style=""text-align:center""><input data-val=""true"" data-val-required=""The chkEwt field is required."" id=""EntreeCV_' + itemCount + '__chkEwt"" name=""EntreeCV[' + itemCoun");
            WriteLiteral(@"t + '].chkEwt"" type=""checkbox"" value=""true""></td>' +
                '<td><input class=""input100"" data-val=""true"" data-val-number=""The field ewt must be a number."" data-val-required=""The ewt field is required."" id=""EntreeCV_' + itemCount + '__ewt"" name=""EntreeCV[' + itemCount + '].ewt"" readonly=""true"" type=""text"" value=""0""></td>' +
                '<td><input class=""input100"" id=""EntreeCV_' + itemCount + '__ccy"" name=""EntreeCV[' + itemCount + '].ccy"" type=""text"" value=""""></td>' +
                '<td><input class=""input100"" data-val=""true"" data-val-number=""The field debitGross must be a number."" data-val-required=""The debitGross field is required."" id=""EntreeCV_' + itemCount + '__debitGross"" name=""EntreeCV[' + itemCount + '].debitGross"" type=""text"" value=""0""></td>' +
                '<td><input class=""input100"" data-val=""true"" data-val-number=""The field credEwt must be a number."" data-val-required=""The credEwt field is required."" id=""EntreeCV_' + itemCount + '__credEwt"" name=""EntreeCV[' + itemCount + '].c");
            WriteLiteral(@"redEwt"" type=""text"" value=""0""></td>' +
                '<td><input class=""input100"" data-val=""true"" data-val-number=""The field credCash must be a number."" data-val-required=""The credCash field is required."" id=""EntreeCV_' + itemCount + '__credCash"" name=""EntreeCV[' + itemCount + '].credCash"" type=""text"" value=""0""></td>' +
                '<td><a class=""expenseAmortization glyphicon glyphicon-list-alt"" href=""#""></a></td>';

            newRow.innerHTML = htmlContent;
        });

        $(""table"").on(""change"",""input.chkVat"",function (e) {
            var pNode = $(this.parentNode)[0].parentNode;

            var itemNo = pNode.id; //jquery obj
            var readOnly = $(""#"" + itemNo).find("".txtVat"").attr(""readonly"");
            if (readOnly == ""true"") {
                $(""#"" + itemNo).find("".txtVat"").attr(""readonly"") = ""false"";
            } else {
                $(""#"" + itemNo).find("".txtVat"").attr(""readonly"") = ""true"";
            }
        });

    });
</script>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<EntreeCVViewModelList> Html { get; private set; }
    }
}
#pragma warning restore 1591
