#pragma checksum "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Shared\EntityTabsPartial.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fabe2c0facb21e2b27214810970431e716a82eca"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_EntityTabsPartial), @"mvc.1.0.view", @"/Views/Shared/EntityTabsPartial.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Shared/EntityTabsPartial.cshtml", typeof(AspNetCore.Views_Shared_EntityTabsPartial))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fabe2c0facb21e2b27214810970431e716a82eca", @"/Views/Shared/EntityTabsPartial.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared_EntityTabsPartial : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Shared\EntityTabsPartial.cshtml"
  
    ViewData["Title"] = "EntityTabsPartial";

#line default
#line hidden
            BeginContext(55, 144, true);
            WriteLiteral("<div id=\"entry-tabs\" class=\"m-l-5 m-r-5\" onload=\"init()\">\r\n    <ul class=\"m-t-0\" id=\"entity-tabs\">\r\n        <li class=\"active ent-tabs\" id=\"cv\">");
            EndContext();
            BeginContext(200, 52, false);
#line 7 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Shared\EntityTabsPartial.cshtml"
                                       Write(Html.ActionLink("Check Payment", "Entry_CV", "Home"));

#line default
#line hidden
            EndContext();
            BeginContext(252, 45, true);
            WriteLiteral("</li>\r\n        <li class=\"ent-tabs\" id=\"ddv\">");
            EndContext();
            BeginContext(298, 55, false);
#line 8 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Shared\EntityTabsPartial.cshtml"
                                 Write(Html.ActionLink("Diirect Deposit", "Entry_DDV", "Home"));

#line default
#line hidden
            EndContext();
            BeginContext(353, 45, true);
            WriteLiteral("</li>\r\n        <li class=\"ent-tabs\" id=\"pcv\">");
            EndContext();
            BeginContext(399, 50, false);
#line 9 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Shared\EntityTabsPartial.cshtml"
                                 Write(Html.ActionLink("Petty Cash", "Entry_PCV", "Home"));

#line default
#line hidden
            EndContext();
            BeginContext(449, 44, true);
            WriteLiteral("</li>\r\n        <li class=\"ent-tabs\" id=\"ss\">");
            EndContext();
            BeginContext(494, 51, false);
#line 10 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Shared\EntityTabsPartial.cshtml"
                                Write(Html.ActionLink("Cash Advance", "Entry_SS", "Home"));

#line default
#line hidden
            EndContext();
            BeginContext(545, 44, true);
            WriteLiteral("</li>\r\n        <li class=\"ent-tabs\" id=\"nc\">");
            EndContext();
            BeginContext(590, 47, false);
#line 11 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Shared\EntityTabsPartial.cshtml"
                                Write(Html.ActionLink("Non-Cash", "Entry_NC", "Home"));

#line default
#line hidden
            EndContext();
            BeginContext(637, 28, true);
            WriteLiteral("</li>\r\n    </ul>\r\n\r\n</div>\r\n");
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
