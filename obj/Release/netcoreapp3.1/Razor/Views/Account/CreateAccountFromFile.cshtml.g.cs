#pragma checksum "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "78b412a1766859a1e0bee196719223e160063e79"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_CreateAccountFromFile), @"mvc.1.0.view", @"/Views/Account/CreateAccountFromFile.cshtml")]
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
#line 1 "C:\Work\CU\IDM\Views\_ViewImports.cshtml"
using IDM;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Work\CU\IDM\Views\_ViewImports.cshtml"
using IDM.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
using IDM.Extensions;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"78b412a1766859a1e0bee196719223e160063e79", @"/Views/Account/CreateAccountFromFile.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7cdcedb47c863f2a4ac0c9c5c7aaadcc50b051ac", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_CreateAccountFromFile : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IDM.DTO.SearchDTO>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("form-control"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("form-horizontal form-parsley"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "CreateAccountFromFile", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Account", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("enctype", new global::Microsoft.AspNetCore.Html.HtmlString("multipart/form-data"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "CreateAccountFromFile2", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.SelectTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
  
    ViewData["Title"] = "สร้างบัญชีรายชื่อผู้ใช้จากไฟล์";


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
#line 13 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                         if (ViewBag.ReturnCode != null)
                        {
                            

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                             if (ViewBag.ReturnCode == ReturnCode.Success)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <div class=\"alert alert-success  border-0\" role=\"alert\">\r\n                                    <i class=\"mdi mdi-check-circle-outline font-18 mr-1\"></i>\r\n                                    <strong class=\"font-18\">");
#nullable restore
#line 19 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                       Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n                                </div>\r\n");
#nullable restore
#line 21 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                            }
                            else
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <div class=\"alert alert-danger border-0\" role=\"alert\">\r\n                                    <i class=\"mdi mdi-close-circle-outline font-18 mr-1\"></i>\r\n                                    <strong class=\"font-18\">");
#nullable restore
#line 26 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                       Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n                                </div>\r\n");
#nullable restore
#line 28 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 28 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                             
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    </div>
                    <h4 class=""page-title"">สร้างบัญชีรายชื่อผู้ใช้จากไฟล์</h4>
                    <p class=""text-muted"">
                        ECreate Accounts from File
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "78b412a1766859a1e0bee196719223e160063e799310", async() => {
                WriteLiteral("\r\n                        <div class=\"form-group row\">\r\n                            <label class=\"col-lg-2 col-form-label text-right\">รูปแบบไฟล์ข้อมูล</label>\r\n                            <div class=\"col-lg-3\">\r\n                                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("select", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "78b412a1766859a1e0bee196719223e160063e799822", async() => {
                    WriteLiteral("\r\n                                    ");
                    __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "78b412a1766859a1e0bee196719223e160063e7910126", async() => {
                        WriteLiteral("นิสิต:สำนักทะเบียน");
                    }
                    );
                    __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                    __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                    BeginWriteTagHelperAttribute();
#nullable restore
#line 50 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                       WriteLiteral(ImportCreateOption.student);

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
                    __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "78b412a1766859a1e0bee196719223e160063e7911940", async() => {
                        WriteLiteral("นิสิต:ศศินทร์");
                    }
                    );
                    __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                    __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                    BeginWriteTagHelperAttribute();
#nullable restore
#line 51 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                       WriteLiteral(ImportCreateOption.student_sasin);

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
                    __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "78b412a1766859a1e0bee196719223e160063e7913755", async() => {
                        WriteLiteral("นิสิต:วิทยาลัยปิโตร");
                    }
                    );
                    __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                    __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                    BeginWriteTagHelperAttribute();
#nullable restore
#line 52 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                       WriteLiteral(ImportCreateOption.student_ppc);

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
                    WriteLiteral("\r\n");
                    WriteLiteral("                                ");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.SelectTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
#nullable restore
#line 49 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.import_option);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
                            </div>
                            <div class=""col-lg-3"">
                                <button class=""btn btn-gray"" type=""button"">ไฟล์ตัวอย่าง</button>
                            </div>
                        </div>
                        <div class=""form-group row"">
                            <div class=""col-lg-3"">
                                <input type=""file"" id=""file"" name=""file"" accept=""text/plain,.csv,application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"" />
                               <br/>
                                ");
#nullable restore
#line 67 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                           Write(Html.ValidationMessage("format_error"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n                            </div>\r\n                            <div class=\"col-lg-2 text-right\">\r\n                                <button class=\"btn btn-primary\" type=\"submit\">ตรวจสอบไฟล์</button>\r\n");
                WriteLiteral("                            </div>\r\n                        </div>\r\n                    ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                    <hr />\r\n");
#nullable restore
#line 76 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                     if (Model.lists != null && Model.lists.Count() > 0)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <div class=\"form-group row\">\r\n                            <div class=\"col-lg-12\">\r\n                                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "78b412a1766859a1e0bee196719223e160063e7920372", async() => {
                WriteLiteral("\r\n                                    <button class=\"btn btn-primary\" type=\"submit\">บันทึก</button>\r\n                                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_6.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                            </div>
                        </div>
                        <div class=""form-group row"">
                            <div class=""col-lg-12"">
                                <div class=""table-rep-plugin"">
                                    <div class=""table-responsive mb-0"" data-pattern=""priority-columns"">
                                        <table id=""tech-companies-1"" class=""table table-striped mb-0"">
                                            <thead>
                                                <tr>
                                                    <th>Job Code</th>
                                                    <th>First Name</th>
                                                    <th>Last Name</th>
                                                    <th>First Name (Th)</th>
                                                    <th>Last Name (Th)</th>
                                                    <th>Citizen ID</th>
                       ");
            WriteLiteral("                             <th>Remark</th>\r\n                                                </tr>\r\n                                            </thead>\r\n                                            <tbody>\r\n");
#nullable restore
#line 102 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                 if (Model.lists != null)
                                                {
                                                    foreach (import item in Model.lists)
                                                    {
                                                        var rowcolor = "";
                                                        if (item.ImportVerify == false)
                                                        {
                                                            rowcolor = "text-danger";
                                                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                        <tr");
            BeginWriteAttribute("class", " class=\"", 6761, "\"", 6778, 1);
#nullable restore
#line 111 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
WriteAttributeValue("", 6769, rowcolor, 6769, 9, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n                                                            <td>\r\n                                                                ");
#nullable restore
#line 113 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                           Write(item.cu_jobcode);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                                            </td>\r\n                                                            <td>\r\n                                                                ");
#nullable restore
#line 116 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                           Write(item.basic_givenname);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                                            </td>\r\n                                                            <td>\r\n                                                                ");
#nullable restore
#line 119 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                           Write(item.basic_sn);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                                            </td>\r\n                                                            <td>\r\n                                                                ");
#nullable restore
#line 122 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                           Write(item.cu_thcn);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                                            </td>\r\n                                                            <td>\r\n                                                                ");
#nullable restore
#line 125 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                           Write(item.cu_thsn);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                                            </td>\r\n                                                            <td>\r\n                                                                ");
#nullable restore
#line 128 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                           Write(item.cu_pplid);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                                            </td>\r\n                                                            <td>");
#nullable restore
#line 130 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                           Write(item.ImportRemark);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                                                        </tr>\r\n");
#nullable restore
#line 132 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                    }
                                                }

#line default
#line hidden
#nullable disable
#nullable restore
#line 134 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
                                                 if (Model.lists == null || Model.lists.Count() == 0)
                                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                    <tr>\r\n                                                        <td colspan=\"7\" class=\"text-center\">ไม่พบข้อมูล</td>\r\n                                                    </tr>\r\n");
#nullable restore
#line 139 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
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
#line 147 "C:\Work\CU\IDM\Views\Account\CreateAccountFromFile.cshtml"
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
