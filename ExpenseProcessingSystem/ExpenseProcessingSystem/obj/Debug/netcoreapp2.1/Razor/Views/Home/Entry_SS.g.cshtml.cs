#pragma checksum "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_SS.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "562a839afad98a29ced2b66759f1265c1dc3bec5"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Entry_SS), @"mvc.1.0.view", @"/Views/Home/Entry_SS.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Entry_SS.cshtml", typeof(AspNetCore.Views_Home_Entry_SS))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"562a839afad98a29ced2b66759f1265c1dc3bec5", @"/Views/Home/Entry_SS.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Entry_SS : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_SS.cshtml"
  
    ViewData["Title"] = "Entry_SS";

#line default
#line hidden
            BeginContext(46, 4111, true);
            WriteLiteral(@"
<div class=""tabContent"">
    <div id=""tbl-lbl"">
        <div class=""dis-inline-block""><p>Date:</p></div>
        <div class=""dis-inline-block""><input class=""input"" disabled /></div>
        <div class=""dis-inline-block""><input class=""input"" type=""datetime"" disabled /></div>
    </div>
    <div>
        <table class=""table table-bordered table-striped voucher-tbl"">
            <thead>
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
                    <th rowspan=""2"">Dept</th>
                    <th rowspan=""2"" colspan=""2"">VAT</th>
                    <th rowspan");
            WriteLiteral(@"=""2"" colspan=""2"">EWT</th>
                    <th rowspan=""2"">CCY</th>
                    <th colspan=""1"" scope=""colgroup"">Debit</th>
                    <th colspan=""2"" scope=""colgroup"">Credit</th>
                    <th rowspan=""2""></th>
                </tr>
                <tr>
                    <th scope=""col"">Gross Amount</th>
                    <th scope=""col"">EWT Amount</th>
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
                </tr>
                <tr>
                    <td></td>
          ");
            WriteLiteral(@"          <td></td>
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
                </tr>
                <tr>
                    <td style=""border:none"" rowspan=""2""><p class=""float-r glyphicon glyphicon-plus""></p></td>
                    <td style=""border:none"" rowspan=""2""></");
            WriteLiteral(@"td>
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
                    <td colspan=""2""></td>
                </tr>
            </tbody>
        </table>
    </div>
    <div id=""voucherPreview"">
        <div id=""tbl-lbl"">
            <div class=""dis-inline-block""><p>Preview of Voucher:</p></div>
        <");
            WriteLiteral("/div>\r\n        ");
            EndContext();
            BeginContext(4158, 25, false);
#line 115 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_SS.cshtml"
   Write(Html.Partial("SSPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(4183, 1310, true);
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
            WriteLiteral(@"uidate</button></div>
        <div class=""float-r"">
            <div class=""dis-inline-block tbl-btn""><button>Print CDD Instruction Sheet</button></div>
            <div class=""dis-inline-block tbl-btn""><button>Print BIR Certificate</button></div>
        </div>
    </div>

    ");
            EndContext();
            BeginContext(5494, 28, false);
#line 138 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Home\Entry_SS.cshtml"
Write(Html.Partial("ModalPartial"));

#line default
#line hidden
            EndContext();
            BeginContext(5522, 10, true);
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
