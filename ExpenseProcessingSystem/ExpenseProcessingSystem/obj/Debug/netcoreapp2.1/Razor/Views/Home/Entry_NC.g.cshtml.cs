#pragma checksum "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_NC.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2ca601bb9be0d290c41db8215fb2d2f23eaedcfc"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Entry_NC), @"mvc.1.0.view", @"/Views/Home/Entry_NC.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Entry_NC.cshtml", typeof(AspNetCore.Views_Home_Entry_NC))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2ca601bb9be0d290c41db8215fb2d2f23eaedcfc", @"/Views/Home/Entry_NC.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Entry_NC : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
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
#line 2 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_NC.cshtml"
  
    ViewData["Title"] = "Entry_NC";

#line default
#line hidden
            BeginContext(46, 455, true);
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
            BeginContext(501, 42, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2a25d50b5fdb4c4f84a2f22e9186b3a5", async() => {
                BeginContext(518, 16, true);
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
            BeginContext(543, 2275, true);
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
                <colgroup span=""2""></colgroup>
                <col />
                <tr>
                    <th rowspan=""2""></th>
                    <th rowspan=""2"">GBase Remarks</th>
                    <th rowspan=""2"">Account</th>
                    <th rowspan=""2"">CCY</th>
                    <th colspan=""1"" scope=""colgroup"">Debit</th>
                    <th rowspan=""2""></th>
                </tr>
                <tr>
                    <th scope=""col"">Gross Amount</th>
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
                </tr");
            WriteLiteral(@">
                <tr>
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
                </tr>
                <tr>
                    <td style=""border:none"" rowspan=""2""><p class=""float-r glyphicon glyphicon-plus""></p></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                    <td style=""border:none"">SUBTOTAL</td>
                    <td rowspan=""2""></td>
                    <td style=""border:none"" rowspan=""2""></td>
                </tr>
                <tr>
                    <td style=""border:none; background-color: #f2f2f2"">TOTAL</td>
       ");
            WriteLiteral("         </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n    <div id=\"voucherPreview\">\r\n        <div id=\"tbl-lbl\">\r\n            <div class=\"dis-inline-block\"><p>Preview of Voucher:</p></div>\r\n        </div>\r\n        ");
            EndContext();
            BeginContext(2819, 25, false);
#line 83 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_NC.cshtml"
   Write(Html.Partial("NCPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(2844, 1214, true);
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
        <div class=""dis-inline-block tbl-btn""><button>Liq");
            WriteLiteral("uidate</button></div>\r\n        <div class=\"float-r\">\r\n            <div class=\"dis-inline-block tbl-btn\"><button>Print CDD Instruction Sheet</button></div>\r\n        </div>\r\n    </div>\r\n\r\n    ");
            EndContext();
            BeginContext(4059, 28, false);
#line 105 "C:\Users\akio.fujiwara\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_NC.cshtml"
Write(Html.Partial("ModalPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(4087, 10, true);
            WriteLiteral("\r\n</div>\r\n");
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
