#pragma checksum "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Account\Login.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "443fe3cd6cf678233f674b036b329d7e1c46c13f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Login), @"mvc.1.0.view", @"/Views/Account/Login.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Account/Login.cshtml", typeof(AspNetCore.Views_Account_Login))]
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
#line 4 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Account\Login.cshtml"
using ExpenseProcessingSystem.ViewModels;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"443fe3cd6cf678233f674b036b329d7e1c46c13f", @"/Views/Account/Login.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c1f162b82d147410b496f61669a4098f632dccc", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Login : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<LoginViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Account\Login.cshtml"
  
    ViewData["Title"] = "Login";

#line default
#line hidden
            BeginContext(107, 61, true);
            WriteLiteral("<div class=\"container-login\">\r\n    <div class=\"form-login\">\r\n");
            EndContext();
#line 8 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Account\Login.cshtml"
         using (Html.BeginForm("Index", "Home", FormMethod.Post, new { @class = "validate-form" }))
        {

#line default
#line hidden
            BeginContext(280, 101, true);
            WriteLiteral("            <div class=\"login-input m-t-20 m-b-15\" data-validate=\"Enter User Name\">\r\n                ");
            EndContext();
            BeginContext(382, 73, false);
#line 11 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Account\Login.cshtml"
           Write(Html.TextBoxFor(model => model.Acc_UserName, new { @class = "input100" }));

#line default
#line hidden
            EndContext();
            BeginContext(455, 108, true);
            WriteLiteral("\r\n                <span class=\"focus-input100 UN\" data-placeholder=\"User Name\"></span>\r\n            </div>\r\n");
            EndContext();
            BeginContext(565, 93, true);
            WriteLiteral("            <div class=\"login-input m-b-40\" data-validate=\"Enter Password\">\r\n                ");
            EndContext();
            BeginContext(659, 74, false);
#line 16 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Account\Login.cshtml"
           Write(Html.PasswordFor(model => model.Acc_Password, new { @class = "input100" }));

#line default
#line hidden
            EndContext();
            BeginContext(733, 107, true);
            WriteLiteral("\r\n                <span class=\"focus-input100 PW\" data-placeholder=\"Password\"></span>\r\n            </div>\r\n");
            EndContext();
            BeginContext(842, 197, true);
            WriteLiteral("            <div class=\"container-login-form-btn m-t-25\">\r\n                <button class=\"login-form-btn\" type=\"submit\">\r\n                    Submit\r\n                </button>\r\n            </div>\r\n");
            EndContext();
#line 25 "C:\Work\Mizuho EPS\eps_source\ExpenseProcessingSystem\ExpenseProcessingSystem\Views\Account\Login.cshtml"
        }

#line default
#line hidden
            BeginContext(1050, 18, true);
            WriteLiteral("    </div>\r\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<LoginViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
