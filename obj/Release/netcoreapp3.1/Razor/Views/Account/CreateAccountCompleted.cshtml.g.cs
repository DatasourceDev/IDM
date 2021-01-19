#pragma checksum "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d96b5313bf196fba7a76784e6369710ad7956e1c"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_CreateAccountCompleted), @"mvc.1.0.view", @"/Views/Account/CreateAccountCompleted.cshtml")]
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
#nullable restore
#line 2 "C:\Work\CU\IDM\Views\_ViewImports.cshtml"
using IDM.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
using IDM.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
using IDM.Extensions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
using IDM;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d96b5313bf196fba7a76784e6369710ad7956e1c", @"/Views/Account/CreateAccountCompleted.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7cdcedb47c863f2a4ac0c9c5c7aaadcc50b051ac", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_CreateAccountCompleted : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IDM.Models.visual_fim_user>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 5 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
  
    ViewData["Title"] = "สร้างบัญชีรายชื่อผู้ใช้สำเร็จ";


#line default
#line hidden
#nullable disable
            WriteLiteral(@"<div class=""container-fluid"">
    <div class=""row"">
        <div class=""col-sm-12"">
            <div aria-live=""polite"" aria-atomic=""true"" style=""position: relative;"">
                <div class=""page-title-box"">
                    <div class=""float-right"" style=""width:50%"">
");
#nullable restore
#line 15 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
                         if (ViewBag.ReturnCode != null)
                        {
                            

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
                             if (ViewBag.ReturnCode == ReturnCode.Success)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <div class=\"alert alert-success  border-0\" role=\"alert\">\r\n                                    <i class=\"mdi mdi-check-circle-outline font-18 mr-1\"></i>\r\n                                    <strong class=\"font-18\">");
#nullable restore
#line 21 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
                                                       Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n                                </div>\r\n");
#nullable restore
#line 23 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
                            }
                            else
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <div class=\"alert alert-danger border-0\" role=\"alert\">\r\n                                    <i class=\"mdi mdi-close-circle-outline font-18 mr-1\"></i>\r\n                                    <strong class=\"font-18\">");
#nullable restore
#line 28 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
                                                       Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n                                </div>\r\n");
#nullable restore
#line 30 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
                            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 30 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
                             
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    </div>
                    <h4 class=""page-title"">สร้างบัญชีรายชื่อผู้ใช้สำเร็จ</h4>
                    <p class=""text-muted"">
                        Account Created Successfully
                    </p>
                </div><!--end page-title-box-->
            </div>
        </div><!--end col-->
    </div>
    <!-- end page title end breadcrumb -->
    <!--end row-->
    <div class=""row"">
        <div class=""col-lg-12"">
            <div class=""card"">
                <div class=""card-body  mt-3"">
                    <div class=""row"">
                        <div class=""col-lg-12"">
                            <div class=""form-group row"">
                                <label class=""col-sm-3 col-form-label text-right"">รหัสผู้ใช้</label>
                                <div class=""col-sm-4"">
                                    <label class=""col-form-label "">");
#nullable restore
#line 52 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
                                                              Write(Model.basic_uid);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class=""row"">
                        <div class=""col-lg-12"">
                            <div class=""form-group row"">
                                <label class=""col-sm-3 col-form-label text-right"">รหัสผ่าน</label>
                                <div class=""col-sm-4"">
                                    <label class=""col-form-label "">");
#nullable restore
#line 62 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
                                                              Write(Cryptography.decrypt(Model.basic_userPassword));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class=""row"">
                        <div class=""col-lg-12"">
                            <div class=""form-group row"">
                                <label class=""col-sm-3 col-form-label text-right""></label>
                                <div class=""col-sm-8"">
                                    <a");
            BeginWriteAttribute("href", " href=\"", 3374, "\"", 3419, 1);
#nullable restore
#line 72 "C:\Work\CU\IDM\Views\Account\CreateAccountCompleted.cshtml"
WriteAttributeValue("", 3381, Url.Action("CreateAccount","Account"), 3381, 38, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" class=""btn btn-light"">กลับ</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div> <!-- end card-body -->
            </div> <!-- end card -->
        </div> <!-- end col -->
        <!-- end col -->
    </div>
</div><!-- container -->
");
            DefineSection("scripts", async() => {
                WriteLiteral("\r\n    \r\n");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IDM.Models.visual_fim_user> Html { get; private set; }
    }
}
#pragma warning restore 1591
