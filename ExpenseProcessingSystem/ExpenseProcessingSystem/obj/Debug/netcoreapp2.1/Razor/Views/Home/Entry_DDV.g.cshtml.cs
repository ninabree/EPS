#pragma checksum "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_DDV.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "755d7f68c575b0d37c9c9283e1e5d7d5f4ce4f77"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Entry_DDV), @"mvc.1.0.view", @"/Views/Home/Entry_DDV.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Entry_DDV.cshtml", typeof(AspNetCore.Views_Home_Entry_DDV))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"755d7f68c575b0d37c9c9283e1e5d7d5f4ce4f77", @"/Views/Home/Entry_DDV.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Entry_DDV : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_DDV.cshtml"
  
    ViewData["Title"] = "Entry_DDV";

#line default
#line hidden
            BeginContext(47, 455, true);
            WriteLiteral(@"
<div class=""tabContent"">
    <div id=""tbl-lbl"">
        <div class=""dis-inline-block""><p>Date:</p></div>
        <div class=""dis-inline-block""><input class=""input"" disabled /></div>
        <div class=""dis-inline-block""><input class=""input"" type=""datetime"" disabled /></div>
    </div>
    <div id=""tbl-lbl"">
        <div class=""dis-inline-block""><p>Payee:</p></div>
        <div class=""dis-inline-block"">
            <select>
                ");
            EndContext();
            BeginContext(502, 42, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "cd8e50808c364a97b9d494ab55395c18", async() => {
                BeginContext(519, 16, true);
                WriteLiteral("Yoshikazu Yamada");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(544, 4331, true);
            WriteLiteral(@"
            </select>
        </div>
    </div>
    <div>
        <table class=""table table-bordered table-striped voucher-tbl"">
            <thead>
                <col />
                <col />
                <col />
                <col />
                <col />
                <col />
                <colgroup span=""2""></colgroup>
                <colgroup span=""2""></colgroup>
                <col />
                <col />
                <colgroup span=""2""></colgroup>
                <col />
                <tr>
                    <th rowspan=""2""></th>
                    <th rowspan=""2"">GBase Remarks</th>
                    <th rowspan=""2"">Account</th>
                    <th rowspan=""2"">Inter-Entity</th>
                    <th rowspan=""2"">FBT</th>
                    <th rowspan=""2"">Dept</th>
                    <th rowspan=""2"" colspan=""2"">VAT</th>
                    <th rowspan=""2"" colspan=""2"">TR</th>
                    <th rowspan=""2"">CCY</th>
                  ");
            WriteLiteral(@"  <th colspan=""1"" scope=""colgroup"">Debit</th>
                    <th colspan=""2"" scope=""colgroup"">Credit</th>
                    <th rowspan=""2""></th>
                </tr>
                <tr>
                    <th scope=""col"">Gross Amount</th>
                    <th scope=""col"">TR Amount</th>
                    <th scope=""col"">Cash</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
         ");
            WriteLiteral(@"           <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td style=""border:none"" rowspan=""2""><p class=""float-r g");
            WriteLiteral(@"lyphicon glyphicon-plus""></p></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"">SUBTOTAL</td>
                    <td rowspan=""2""></td>
                    <td colspan=""1""></td>
                    <td colspan=""1""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                </tr>
                <tr>
                    <td style=""border:none; background-color: #f2f2f2"">TOTAL</td>
                    <td colspan=""2""></td>");
            WriteLiteral("\n                </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n    <div id=\"voucherPreview\">\r\n        <div id=\"tbl-lbl\">\r\n            <div class=\"dis-inline-block\"><p>Preview of Voucher:</p></div>\r\n        </div>\r\n        ");
            EndContext();
            BeginContext(4876, 26, false);
#line 135 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_DDV.cshtml"
   Write(Html.Partial("DDVPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(4902, 1212, true);
            WriteLiteral(@"
    </div>
    <div id=""entry-status"" class=""m-t-10 m-b-10"">
        <div class=""dis-inline-block""><label>Status:</label></div>
        <div class=""dis-inline-block""><input class=""input"" /></div>
        <div class=""dis-inline-block""><label>Approver:</label></div>
        <div class=""dis-inline-block""><input class=""input"" /></div>
        <div class=""dis-inline-block""><label>Verifier:</label></div>
        <div class=""dis-inline-block""><input class=""input"" /></div>
    </div>
    <div id=""entry-controls"" class=""m-b-10"">
        <div class=""dis-inline-block tbl-btn""><button>Save</button></div>
        <div class=""dis-inline-block tbl-btn""><button>Delete</button></div>
        <div class=""dis-inline-block tbl-btn""><button>Modify</button></div>
        <div class=""dis-inline-block tbl-btn""><button id=""reversal_entry"">Create Reversal Entry</button></div>
        <div class=""dis-inline-block tbl-btn""><button>Delete GBase Post</button></div>
        <div class=""float-r"">
            <div class=""di");
            WriteLiteral("s-inline-block tbl-btn\"><button>Print LOI</button></div>\r\n            <div class=\"dis-inline-block tbl-btn\"><button>Print BIR Certificate</button></div>\r\n        </div>\r\n    </div>\r\n\r\n    ");
            EndContext();
            BeginContext(6115, 28, false);
#line 157 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_DDV.cshtml"
Write(Html.Partial("ModalPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(6143, 12, true);
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
