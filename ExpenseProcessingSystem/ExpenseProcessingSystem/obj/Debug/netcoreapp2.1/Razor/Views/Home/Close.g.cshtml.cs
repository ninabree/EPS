#pragma checksum "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7a6901f3cd54b3a730ccec12e82e2cc6ea8157aa"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Close), @"mvc.1.0.view", @"/Views/Home/Close.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Close.cshtml", typeof(AspNetCore.Views_Home_Close))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7a6901f3cd54b3a730ccec12e82e2cc6ea8157aa", @"/Views/Home/Close.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Close : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Close", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 1 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml"
  
    ViewData["Title"] = "Close";

#line default
#line hidden
            BeginContext(41, 245, true);
            WriteLiteral("\r\n<div class=\"tab-content-cont m-t-10\">\r\n    <div id=\"tbl-lbl\"><p>Date:</p></div>\r\n    <div class=\"tbl-cont m-b-20\">\r\n        <table class=\"table table-striped tab-tbl m-b-20\">\r\n            <thead>\r\n                <tr>\r\n                    <th>");
            EndContext();
            BeginContext(286, 141, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "02194a967c0f471ab2c6870d35c2e219", async() => {
                BeginContext(359, 11, true);
                WriteLiteral("Tran Type<p");
                EndContext();
                BeginWriteAttribute("class", " class=\"", 370, "\"", 417, 3);
                WriteAttributeValue("", 378, "btn-xs", 378, 6, true);
                WriteAttributeValue(" ", 384, "glyphicon", 385, 10, true);
#line 11 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml"
WriteAttributeValue(" ", 394, ViewData["glyph-acc"], 395, 22, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(418, 5, true);
                WriteLiteral("></p>");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-sortOrder", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#line 11 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml"
                                                       WriteLiteral(ViewData["AccountSortParm"]);

#line default
#line hidden
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["sortOrder"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-sortOrder", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["sortOrder"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(427, 31, true);
            WriteLiteral("</th>\r\n                    <th>");
            EndContext();
            BeginContext(458, 141, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5fe3737067c54e9fbacc9713fae4fde8", async() => {
                BeginContext(528, 13, true);
                WriteLiteral("GBase Tran#<p");
                EndContext();
                BeginWriteAttribute("class", " class=\"", 541, "\"", 589, 3);
                WriteAttributeValue("", 549, "btn-xs", 549, 6, true);
                WriteAttributeValue(" ", 555, "glyphicon", 556, 10, true);
#line 12 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml"
WriteAttributeValue(" ", 565, ViewData["glyph-type"], 566, 23, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(590, 5, true);
                WriteLiteral("></p>");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-sortOrder", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#line 12 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml"
                                                       WriteLiteral(ViewData["TypeSortParm"]);

#line default
#line hidden
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["sortOrder"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-sortOrder", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["sortOrder"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(599, 947, true);
            WriteLiteral(@"</th>
                    <th>Express Tran#</th>
                    <th>Particulars</th>
                    <th>CCY</th>
                    <th>Amount</th>
                    <th>Tran Count</th>
                    <th></th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
        <div class=""tbl-controls m-t-20"">
            <button class=""tbl-btn float-l"">Close RBU</button>
            <button class=""tbl-btn float-l"">Re-Open RBU</button>
            <button class=""tbl-btn float-l"">Print RBU Balance</button>
            <button class=""tbl-btn float-r"">Print RBU Proof sheet</button>
        </div>

        <div id=""tbl-lbl""><p>Status:</p></div>
    </div>
    <div class=""tbl-cont m-t-20 m-b-20"">
        <table class=""table table-striped tab-tbl m-b-20"">
            <thead>
                <tr>
                    <th>");
            EndContext();
            BeginContext(1546, 141, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5ccbb957a80240b787139be00b139bab", async() => {
                BeginContext(1619, 11, true);
                WriteLiteral("Tran Type<p");
                EndContext();
                BeginWriteAttribute("class", " class=\"", 1630, "\"", 1677, 3);
                WriteAttributeValue("", 1638, "btn-xs", 1638, 6, true);
                WriteAttributeValue(" ", 1644, "glyphicon", 1645, 10, true);
#line 38 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml"
WriteAttributeValue(" ", 1654, ViewData["glyph-acc"], 1655, 22, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1678, 5, true);
                WriteLiteral("></p>");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-sortOrder", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#line 38 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml"
                                                       WriteLiteral(ViewData["AccountSortParm"]);

#line default
#line hidden
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["sortOrder"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-sortOrder", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["sortOrder"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1687, 31, true);
            WriteLiteral("</th>\r\n                    <th>");
            EndContext();
            BeginContext(1718, 141, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "713b2c1260f7414490a567a256419789", async() => {
                BeginContext(1788, 13, true);
                WriteLiteral("GBase Tran#<p");
                EndContext();
                BeginWriteAttribute("class", " class=\"", 1801, "\"", 1849, 3);
                WriteAttributeValue("", 1809, "btn-xs", 1809, 6, true);
                WriteAttributeValue(" ", 1815, "glyphicon", 1816, 10, true);
#line 39 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml"
WriteAttributeValue(" ", 1825, ViewData["glyph-type"], 1826, 23, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1850, 5, true);
                WriteLiteral("></p>");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-sortOrder", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#line 39 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Close.cshtml"
                                                       WriteLiteral(ViewData["TypeSortParm"]);

#line default
#line hidden
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["sortOrder"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-sortOrder", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["sortOrder"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1859, 2768, true);
            WriteLiteral(@"</th>
                    <th>Express Tran#</th>
                    <th>Particulars</th>
                    <th>CCY</th>
                    <th>Amount</th>
                    <th>Tran Count</th>
                    <th></th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>

        <div class=""tbl-controls m-t-20 m-b-20"">
            <button class=""tbl-btn float-l"">Close FCDU</button>
            <button class=""tbl-btn float-l"">Re-Open FCDU</button>
            <button class=""tbl-btn float-l"">Print FCDU Balance</button>
            <button class=""tbl-btn float-r"">Print FCDU Proof sheet</button>
        </div>

        <div id=""tbl-lbl""><div><p>Status:</p></div><div><input class=""input"" /></div></div>
    </div>
    <hr class=""dotted"" />
    <div id=""tbl-lbl""><p>Petty Cash Balance:</p></div>
    <table class=""table"" id=""close-tbl"">
        <thead>
            <tr>
                <td width=""1");
            WriteLiteral(@"0%""></td>
                <td width=""25%""></td>
                <td width=""65%""></td>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                    <p>Beginning Balance</p>
                </td>
                <td class=""login-input"">
                    <input class=""input100"" />
                    <span class=""focus-input100"" data-placeholder=""""></span>
                </td>
                <td>
                    <button class=""tbl-btn"">Beginning Cash balance Confirmation</button>
                </td>
            </tr>
            <tr>
                <td>
                    <p>Cash-IN</p>
                </td>
                <td class=""login-input"">
                    <input class=""input100"" />
                    <span class=""focus-input100"" data-placeholder=""""></span>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <p>Cash-OUT</p>
              ");
            WriteLiteral(@"  </td>
                <td class=""login-input"">
                    <input class=""input100"" />
                    <span class=""focus-input100"" data-placeholder=""""></span>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <p>Ending Balance</p>
                </td>
                <td class=""login-input"">
                    <input class=""input100"" />
                    <span class=""focus-input100"" data-placeholder=""""></span>
                </td>
                <td>
                    <button class=""tbl-btn"">Petty Cash Breakdown</button>
                </td>
            </tr>
        </tbody>
    </table>
</div>");
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
