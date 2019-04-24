#pragma checksum "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9d33ddd8f1ea038fe351f1aa6ba1b48661a7c0f2"
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9d33ddd8f1ea038fe351f1aa6ba1b48661a7c0f2", @"/Views/Home/Report.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Report : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ExpenseProcessingSystem.ViewModels.HomeReportViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("style", new global::Microsoft.AspNetCore.Html.HtmlString("width: 90%;"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-area", "", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Home", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Excel", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(63, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
  
    ViewData["Title"] = "Report";

#line default
#line hidden
            BeginContext(107, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(127, 253, true);
                WriteLiteral("\r\n    Scripts.Render(\"~/bundles/jqueryval\")\r\n    <script type=\"text/javascript\">\r\n         $(document).ready(function () {\r\n             $(\"#ReportType\").change(function () {\r\n                 var ReportType= $(this).val();\r\n                 $.getJSON(\'");
                EndContext();
                BeginContext(381, 30, false);
#line 13 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
                       Write(Url.Action("GetReportSubType"));

#line default
#line hidden
                EndContext();
                BeginContext(411, 897, true);
                WriteLiteral(@"', { ReportTypeID: ReportType }, function (data) {
                     var select = $(""#SubType"");
                     select.empty();
                     select.append($('<option/>', {
                         value: 0,
                         text: ""----Select Report Sub-Type----""
                     }));
                     $.each(data, function (index, itemData) {
                         select.append($('<option/>', {
                             value: itemData.Id,
                             text: itemData.SubTypeName
                         }));
                     });

                     if (data.length == 0) {
                         select.attr(""disabled"", ""disabled"");
                     } else {
                         select.removeAttr(""disabled"");
                     }
                 });
             });
        });
    </script>
");
                EndContext();
            }
            );
            BeginContext(1311, 510, true);
            WriteLiteral(@"
<div class=""m-t-10"">
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
            <td>Report Type</td>
            <td colspan=""2"">
                ");
            EndContext();
            BeginContext(1822, 187, false);
#line 55 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
           Write(Html.DropDownListFor(m => m.ReportTypesList, new SelectList(Model.ReportTypesList, "Id", "TypeName"), "----Select Report Type----", new { @class = "dis-inline-block", id = "ReportType" }));

#line default
#line hidden
            EndContext();
            BeginContext(2009, 98, true);
            WriteLiteral("\r\n            </td>\r\n            <td>Sub-Type</td>\r\n            <td colspan=\"2\">\r\n                ");
            EndContext();
            BeginContext(2108, 217, false);
#line 59 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
           Write(Html.DropDownListFor(m => m.ReportTypesList, new SelectList(Model.ReportSubTypesList, "Id", "SubTypeName"), "----Select Report Sub-Type----", new { @class = "dis-inline-block", id = "SubType", disabled = "disabled" }));

#line default
#line hidden
            EndContext();
            BeginContext(2325, 390, true);
            WriteLiteral(@"
            </td>
            <td colspan=""4""></td>
        </tr>
        <tr>
            <td colspan=""10""></td>
        </tr>
        <tr>
            <td><input type=""radio"" /></td>
            <td>Year</td>
            <td colspan=""1""><input class=""input"" type=""date"" /></td>
            <td>Month</td>
            <td colspan=""1""><input class=""input"" type=""date"" /></td>
");
            EndContext();
            BeginContext(2760, 544, true);
            WriteLiteral(@"            <td><input type=""radio"" /></td>
            <td>Year</td>
            <td colspan=""1""><input class=""input"" type=""date"" /></td>
            <td colspan=""1""><input class="""" type=""radio"" /> 1st Semester</td>
            <td colspan=""1""><input class="""" type=""radio"" /> 2nd Semester</td>
        </tr>
        <tr>
            <td><input type=""radio"" /></td>
            <td>Period</td>
            <td colspan=""1""><input class=""input"" /></td>
            <td>To</td>
            <td colspan=""1""><input class=""input"" /></td>
");
            EndContext();
            BeginContext(3349, 1346, true);
            WriteLiteral(@"            <td><input type=""radio"" /></td>
            <td>Check No.</td>
            <td colspan=""1""><input class=""input"" /></td>
            <td colspan=""1""><input class=""input"" /></td>
            <td rowspan=""2""><button class=""btn"" style=""width: 90%;"">Generate Preview</button></td>
        </tr>
        <tr>
            <td><input type=""radio"" /></td>
            <td>Voucher No.</td>
            <td colspan=""3""><input class=""input"" /></td>
            <td></td>
            <td colspan=""3""><input class=""input"" /></td>
        </tr>
        <tr>
            <td><input type=""radio"" /></td>
            <td>Covered Tran No.</td>
            <td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
            <td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
            <td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
            <td></td>
            <td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
            <td colspan=""1"" style=""p");
            WriteLiteral(@"adding:0px""><input class=""input"" /></td>
            <td colspan=""1"" style=""padding:0px""><input class=""input"" /></td>
        </tr>
        <tr>
            <td><input type=""radio"" /></td>
            <td>Subject Name</td>
            <td colspan=""2""><input class=""input"" /></td>
            <td colspan=""1""></td>
");
            EndContext();
            BeginContext(4829, 150, true);
            WriteLiteral("            <td>Save to File</td>\r\n            <td colspan=\"2\"><input class=\"input\" /></td>\r\n            <td>Browse</td>\r\n            <td rowspan=\"2\">");
            EndContext();
            BeginContext(4979, 114, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "a766620fd7484a2aa449127f00563fb5", async() => {
                BeginContext(5067, 22, true);
                WriteLiteral("Generate To Excel File");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Area = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(5093, 416, true);
            WriteLiteral(@"</td>
        </tr>
        <tr>
            <td colspan=""6""></td>
            <td>Excel</td>
            <td>Pdf</td>
            <td colspan=""2""></td>
        </tr>
    </table>
</div>
<div id=""tbl-lbl"">
    <div id=""tbl-lbl""><p>Preview Report:</p></div>
</div>

<div class=""tabContent"" style="" background-color: #fafafa;"">
    <div id=""voucherPreview"" style=""min-height: 50vh; max-height: 100%;"">
");
            EndContext();
            BeginContext(5549, 16, true);
            WriteLiteral("    </div>\r\n    ");
            EndContext();
            BeginContext(5566, 28, false);
#line 143 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
Write(Html.Partial("ModalPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(5594, 12, true);
            WriteLiteral("\r\n</div>\r\n\r\n");
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
