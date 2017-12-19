﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    
    #line 1 "..\..\Views\Security\Forgot.cshtml"
    using System.Web.Mvc;
    
    #line default
    #line hidden
    using System.Web.Mvc.Ajax;
    
    #line 2 "..\..\Views\Security\Forgot.cshtml"
    using System.Web.Mvc.Html;
    
    #line default
    #line hidden
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 3 "..\..\Views\Security\Forgot.cshtml"
    using BCS.Framework.WebExtensions.Label;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Views\Security\Forgot.cshtml"
    using Kendo.Mvc.UI;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Security/Forgot.cshtml")]
    public partial class _Views_Security_Forgot_cshtml : System.Web.Mvc.WebViewPage<BCS.Framework.Models.ForgotModel>
    {
        public _Views_Security_Forgot_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 6 "..\..\Views\Security\Forgot.cshtml"
  
    ViewBag.Title = "Forgot Password - Smart Systems Pro";
    Layout = "~/Views/Shared/LayoutPage.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<style>\r\n    .form-horizontal .form-group{\r\n        margin-left: 0px !importa" +
"nt;\r\n    }\r\n</style>\r\n\r\n<div");

WriteLiteral(" class=\"col-xs-12\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n        <br /><br /><br /><br /><br />\r\n        <div");

WriteLiteral(" class=\"text-center\"");

WriteLiteral(" style=\"font-size: 22pt; font-weight: normal; color: #FFF\"");

WriteLiteral(">FORGOT PASSWORD</div>\r\n        <br /><br />\r\n    </div>\r\n</div>\r\n<div");

WriteLiteral(" class=\"clearfix\"");

WriteLiteral("></div>\r\n");

            
            #line 25 "..\..\Views\Security\Forgot.cshtml"
 using (Html.BeginForm("Forgot", "Security", new { ReturnUrl = ViewBag.ReturnUrl }, FormMethod.Post, new { @class = "logon-form form-horizontal", id = "frm-fogot" }))
 {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"\"");

WriteLiteral(" style=\"height:420px; width:850px; margin: 0 auto;background: #FFF;-moz-border-ra" +
"dius: 2em\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(" style=\"padding: 15px;-moz-border-radius: 1em\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"col-xs-7\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"error\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 31 "..\..\Views\Security\Forgot.cshtml"
                   Write(Html.ValidationSummary(true));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                        ");

            
            #line 32 "..\..\Views\Security\Forgot.cshtml"
                   Write(Html.AntiForgeryToken());

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                    <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"col-xs-12\"");

WriteLiteral(@">
                            <p>If you need help resetting you password, we can help by sending you a link to rest it.</p>
                            <li>Enter either the email address and captcha on the account.</li>
                            <li>Click Send.</li>
                            <li>Check your inbox for a password reset email.</li>
                            <li>Click on the URL provided in the email and enter a new password .</li>
                            <br />
                        </div>
                    </div>
                    <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 45 "..\..\Views\Security\Forgot.cshtml"
                   Write(Html.LabelForRequire(model => model.Email, new { @class = "col-xs-2 txt-right", style = "margin-top:8px" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        <div");

WriteLiteral(" class=\"col-xs-10\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 47 "..\..\Views\Security\Forgot.cshtml"
                       Write(Html.TextBoxFor(model => model.Email, new { placeholder = "Email", @class = "form-control" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            <span");

WriteLiteral(" class=\"error\"");

WriteLiteral(">");

            
            #line 48 "..\..\Views\Security\Forgot.cshtml"
                                           Write(Html.ValidationMessageFor(model => model.Email));

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                        </div>\r\n                    </div>\r\n            " +
"        <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(" style=\"padding-top: 10px;\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 52 "..\..\Views\Security\Forgot.cshtml"
                   Write(Html.LabelForRequire(model => model.Sum, new { @class = "col-xs-2 txt-right", style = "margin-top: 8px" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        <div");

WriteLiteral(" class=\"col-xs-10\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"col-xs-6\"");

WriteLiteral(" style=\"margin-left: -15px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 55 "..\..\Views\Security\Forgot.cshtml"
                           Write(Html.TextBoxFor(model => model.Sum, new { placeholder = "Sum", @class = "form-control" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n                            <div");

WriteLiteral(" class=\"col-xs-6\"");

WriteLiteral(">\r\n                                <img");

WriteLiteral(" alt=\"Captcha\"");

WriteAttribute("src", Tuple.Create(" src=\"", 2984), Tuple.Create("\"", 3017)
            
            #line 58 "..\..\Views\Security\Forgot.cshtml"
, Tuple.Create(Tuple.Create("", 2990), Tuple.Create<System.Object, System.Int32>(Url.Action("CaptchaImage")
            
            #line default
            #line hidden
, 2990), false)
);

WriteLiteral(" style=\"\"");

WriteLiteral(" />\r\n                            </div>\r\n                            <div");

WriteLiteral(" style=\"clear: both\"");

WriteLiteral("></div>\r\n                            <span");

WriteLiteral(" class=\"error\"");

WriteLiteral(">");

            
            #line 61 "..\..\Views\Security\Forgot.cshtml"
                                           Write(Html.ValidationMessageFor(model => model.Sum));

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                        </div>\r\n                    </div>\r\n            " +
"        <br />\r\n                    <div");

WriteLiteral(" class=\"form-group logon-toolbar\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"col-xs-12\"");

WriteLiteral(" align=\"center\"");

WriteLiteral("><input");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" class=\"btn btn-primary button btnOk\"");

WriteLiteral(" id=\"btnForgot\"");

WriteLiteral(" value=\" Send \"");

WriteLiteral(" style=\"width:150px\"");

WriteLiteral(" /></div>\r\n                    </div>\r\n                     <br />\r\n             " +
"       <div");

WriteLiteral(" class=\"form-group text-center\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 70 "..\..\Views\Security\Forgot.cshtml"
                   Write(Html.ActionLink("Back To Sign In", "Login", "Security", new { @class = "button btnForgot"}));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"col-xs-5\"");

WriteLiteral(" style=\"border-left: 1px solid darkolivegreen;\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n                    <b>Already have Smart System Pro account ?</b>\r\n          " +
"      </div>\r\n                <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n                    <span>Click Sign Up button to access Budget Creator</span>" +
"\r\n                </div>\r\n                <br /><br />\r\n                <div");

WriteLiteral(" class=\"form-group text-center\"");

WriteLiteral(">\r\n                    <input");

WriteLiteral(" type=\"button\"");

WriteLiteral(" class=\"btn btn-primary button btnOk\"");

WriteLiteral(" id=\"btnSignUp\"");

WriteLiteral(" value=\"Sign Up\"");

WriteLiteral(" style=\"width:150px\"");

WriteLiteral(" />\r\n                </div>\r\n                <br /><br />\r\n                <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n                    <span><b>Having trouble?</b> Feel free to contact us</span" +
">\r\n                </div>\r\n                <br />\r\n                <div");

WriteLiteral(" class=\"form-group text-center\"");

WriteLiteral(">\r\n                    <input");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" class=\"btn btn-primary button btnOk\"");

WriteLiteral(" id=\"btnContactUs\"");

WriteLiteral(" value=\"Contact us\"");

WriteLiteral(" style=\"width:150px\"");

WriteLiteral(" />\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral("></div>\r\n                <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral("></div>\r\n                <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral("></div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

            
            #line 98 "..\..\Views\Security\Forgot.cshtml"
 }

            
            #line default
            #line hidden
WriteLiteral("<script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
    $(document).ready(function () {
        $(""#btnForgot"").click(function () {
            var form = $(""#frm-fogot"");
            form.validate();
            var isValid = form.valid();
            if (isValid) {
                return showProcessing();
            }
        });
    });
</script>


");

        }
    }
}
#pragma warning restore 1591
