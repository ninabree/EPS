#pragma checksum "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a67b2e1902e3d24cb0780257dee10c8d8eb83afe"
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a67b2e1902e3d24cb0780257dee10c8d8eb83afe", @"/Views/Home/Report.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Report : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
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
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
  
    ViewData["Title"] = "Report";

#line default
#line hidden
            BeginContext(44, 978, true);
            WriteLiteral(@"<div class=""m-t-10"">
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
            <td colspan=""2""><input class=""input"" /></td>
            <td>Sub-Type</td>
            <td colspan=""2""><input class=""input"" /></td>
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
            BeginContext(1059, 544, true);
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
            BeginContext(1640, 1346, true);
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
            BeginContext(3084, 150, true);
            WriteLiteral("            <td>Save to File</td>\r\n            <td colspan=\"2\"><input class=\"input\" /></td>\r\n            <td>Browse</td>\r\n            <td rowspan=\"2\">");
            EndContext();
            BeginContext(3234, 114, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "6fa99a4bd8534e73a0f33a58a756e45b", async() => {
                BeginContext(3322, 22, true);
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
            BeginContext(3348, 416, true);
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
            BeginContext(3804, 16, true);
            WriteLiteral("    </div>\r\n    ");
            EndContext();
            BeginContext(3821, 28, false);
#line 106 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Report.cshtml"
Write(Html.Partial("ModalPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(3849, 12, true);
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
