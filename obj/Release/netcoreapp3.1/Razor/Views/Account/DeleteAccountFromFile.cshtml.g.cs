#pragma checksum "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e5682755b385fb47af3e7971eb56f982af6ec4af"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_DeleteAccountFromFile), @"mvc.1.0.view", @"/Views/Account/DeleteAccountFromFile.cshtml")]
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
#line 1 "C:\Work\CU\CUIDM\Views\_ViewImports.cshtml"
using IDM;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Work\CU\CUIDM\Views\_ViewImports.cshtml"
using IDM.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
using IDM.Extensions;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e5682755b385fb47af3e7971eb56f982af6ec4af", @"/Views/Account/DeleteAccountFromFile.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7cdcedb47c863f2a4ac0c9c5c7aaadcc50b051ac", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_DeleteAccountFromFile : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IDM.DTO.SearchDTO>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("form-horizontal form-parsley"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "DeleteAccountFromFile", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Account", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("enctype", new global::Microsoft.AspNetCore.Html.HtmlString("multipart/form-data"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "DeleteAccountFromFile2", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
  
    ViewData["Title"] = "ลบบัญชีผู้ใช้จากไฟล์";


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
#line 13 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                         if (ViewBag.ReturnCode != null)
                        {
                            

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                             if (ViewBag.ReturnCode == ReturnCode.Success)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <div class=\"alert alert-success  border-0\" role=\"alert\">\r\n                                    <i class=\"mdi mdi-check-circle-outline font-18 mr-1\"></i>\r\n                                    <strong class=\"font-18\">");
#nullable restore
#line 19 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                       Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n                                </div>\r\n");
#nullable restore
#line 21 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                            }
                            else
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <div class=\"alert alert-danger border-0\" role=\"alert\">\r\n                                    <i class=\"mdi mdi-close-circle-outline font-18 mr-1\"></i>\r\n                                    <strong class=\"font-18\">");
#nullable restore
#line 26 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                       Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n                                </div>\r\n");
#nullable restore
#line 28 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 28 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                             
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    </div>
                    <h4 class=""page-title"">ลบบัญชีผู้ใช้จากไฟล์</h4>
                    <p class=""text-muted"">
                        Delete Accounts from File
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
                <div class=""card-body"">
                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e5682755b385fb47af3e7971eb56f982af6ec4af8829", async() => {
                WriteLiteral(@"
                        <div class=""form-group row"">
                            <label class=""col-lg-2 col-form-label text-right"">รูปแบบไฟล์ข้อมูล</label>
                            <div class=""col-lg-3"">
                                <select class=""form-control"" name=""import_option"">
                                    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e5682755b385fb47af3e7971eb56f982af6ec4af9426", async() => {
                    WriteLiteral("Login Name");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#nullable restore
#line 50 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                       WriteLiteral(ImportLockOption.loginname);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n                                    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e5682755b385fb47af3e7971eb56f982af6ec4af11161", async() => {
                    WriteLiteral("Citizen ID");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#nullable restore
#line 51 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                       WriteLiteral(ImportLockOption.pplid);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
                                </select>
                            </div>
                        </div>
                        <div class=""form-group row"">
                            <div class=""col-lg-3"">
                                <input type=""file"" id=""file"" name=""file"" accept=""text/plain,.csv,application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"" />
                            </div>
                            <div class=""col-lg-2 text-right"">
                                <button class=""btn btn-primary"" type=""submit"">ตรวจสอบไฟล์</button>
");
                WriteLiteral("                            </div>\r\n                        </div>\r\n                    ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                    <hr />\r\n");
#nullable restore
#line 66 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                     if (Model.lists.Count() > 0)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <div class=\"form-group row\">\r\n                            <div class=\"col-lg-12\">\r\n                                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e5682755b385fb47af3e7971eb56f982af6ec4af15777", async() => {
                WriteLiteral("\r\n                                    <button class=\"btn btn-danger\" type=\"submit\">ลบ</button>\r\n                                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                            </div>\r\n                        </div>\r\n");
            WriteLiteral(@"                        <div class=""form-group row"">
                            <div class=""col-lg-12"">
                                <div class=""table-rep-plugin"">
                                    <div class=""table-responsive mb-0"" data-pattern=""priority-columns"">
                                        <table id=""tech-companies-1"" class=""table table-striped mb-0"">
                                            <thead>
                                                <tr>
                                                    <th>Citizen ID</th>
                                                    <th>Login Name</th>
                                                    <th>First Name</th>
                                                    <th>Last Name</th>
                                                    <th>Remark</th>
                                                </tr>
                                            </thead>
                                            <tbody>
");
#nullable restore
#line 91 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                 foreach (import item in Model.lists)
                                                {
                                                    var basic_givennames = item.basic_givenname.Split("|", StringSplitOptions.RemoveEmptyEntries);
                                                    var basic_sns = item.basic_sn.Split("|", StringSplitOptions.RemoveEmptyEntries);
                                                    var cu_pplids = item.cu_pplid.Split("|", StringSplitOptions.RemoveEmptyEntries);
                                                    var basic_uids = item.basic_uid.Split("|", StringSplitOptions.RemoveEmptyEntries);

                                                    var rowcolor = "";
                                                    if (item.ImportVerify == false)
                                                    {
                                                        rowcolor = "text-danger";
                                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                    <tr");
            BeginWriteAttribute("class", " class=\"", 6065, "\"", 6082, 1);
#nullable restore
#line 103 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
WriteAttributeValue("", 6073, rowcolor, 6073, 9, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n                                                        <td>\r\n");
#nullable restore
#line 105 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                             foreach (var cu_pplid in cu_pplids)
                                                            {
                                                                

#line default
#line hidden
#nullable disable
#nullable restore
#line 107 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                           Write(cu_pplid);

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                                <br />\r\n");
#nullable restore
#line 109 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                        </td>\r\n                                                        <td>\r\n");
#nullable restore
#line 112 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                             foreach (var basic_uid in basic_uids)
                                                            {
                                                                

#line default
#line hidden
#nullable disable
#nullable restore
#line 114 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                           Write(basic_uid);

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                                <br />\r\n");
#nullable restore
#line 116 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                        </td>\r\n                                                        <td>\r\n");
#nullable restore
#line 119 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                             foreach (var basic_givenname in basic_givennames)
                                                            {
                                                                

#line default
#line hidden
#nullable disable
#nullable restore
#line 121 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                           Write(basic_givenname);

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                                <br />\r\n");
#nullable restore
#line 123 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                        </td>\r\n                                                        <td>\r\n");
#nullable restore
#line 126 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                             foreach (var basic_sn in basic_sns)
                                                            {
                                                                

#line default
#line hidden
#nullable disable
#nullable restore
#line 128 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                           Write(basic_sn);

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                                <br />\r\n");
#nullable restore
#line 130 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                        </td>\r\n                                                        <td>");
#nullable restore
#line 132 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                       Write(item.ImportRemark);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                                                    </tr>\r\n");
#nullable restore
#line 134 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                }

#line default
#line hidden
#nullable disable
#nullable restore
#line 135 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                 if (Model.lists.Count() == 0)
                                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                    <tr>\r\n                                                        <td colspan=\"5\" class=\"text-center\">ไม่พบข้อมูล</td>\r\n                                                    </tr>\r\n");
#nullable restore
#line 140 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                                                }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                                            </tbody>

                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
");
#nullable restore
#line 148 "C:\Work\CU\CUIDM\Views\Account\DeleteAccountFromFile.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </div> <!-- end card-body -->\r\n            </div> <!-- end card -->\r\n        </div> <!-- end col -->\r\n        <!-- end col -->\r\n    </div>\r\n</div><!-- container -->\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IDM.DTO.SearchDTO> Html { get; private set; }
    }
}
#pragma warning restore 1591
