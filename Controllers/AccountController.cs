using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IDM.Models;
using IDM.DAL;
using IDM.Services;
using IDM.Identity;
using IDM.DTO;
using Microsoft.EntityFrameworkCore;
using IDM.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;

namespace IDM.Controllers
{
    [Authorize]
    public class AccountController : ControllerBase
    {
        public readonly IWebHostEnvironment _env;
        public AccountController(SpuContext context, ILogger<AccountController> logger, ILoginServices loginServices, IUserProvider provider, ILDAPUserProvider providerldap, IOptions<SystemConf> conf, IWebHostEnvironment env) : base(context, logger, loginServices, conf, provider, providerldap)
        {

            this._env = env;

        }


        #region CreateAccount
        public async Task<IActionResult> CreateAccount(ReturnCode code, string msg)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            var model = new visual_fim_user();
            model.system_faculty_id = 0;
            model.cu_CUexpire_day = DateUtil.Now().Day;
            model.cu_CUexpire_month = DateUtil.Now().Month;
            model.cu_CUexpire_year = DateUtil.Now().Year + 1;
            var orgs = await _providerldap.GetOrganizationLvl1(_context, _conf);
            if (orgs.Count() > 0)
                model.system_ou_lvl1 = orgs[0].ouname;
            ViewBag.ListOrganization = orgs;
            ViewBag.ListFaculty = this.ListFaculty();
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.Message = msg;
                ViewBag.ReturnCode = code;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(visual_fim_user model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            /*check name and surname dup*/
            //if (isExistNameSurNameAndCitizenID(model))
            //{
            //    ModelState.AddModelError("basic_givenname", "ชื่อ(อังกฤษ)ซ้ำในระบบ");
            //    ModelState.AddModelError("basic_sn", "นามสกุล(อังกฤษ)ซ้ำในระบบ"); 
            //}
            //if (isExistCitizenID(model.cu_pplid))
            //{
            //    ModelState.AddModelError("cu_pplid", "รหัสบัตรประชาชนซ้ำในระบบ");
            //}
            var system_ou_lvl1 = AppUtil.getOuName(model.system_ou_lvl1);
            model.system_idm_user_type = getIdmUserType(system_ou_lvl1);
            if (model.system_idm_user_type == IDMUserType.student)
            {
                if (string.IsNullOrEmpty(model.cu_jobcode))
                {
                    ModelState.AddModelError("cu_jobcode", "กรุณาระบุรหัสนิสิต");
                }
            }
            if (ModelState.IsValid)
            {
                ViewBag.Message = ReturnMessage.Error;
                ViewBag.ReturnCode = ReturnCode.Error;
                try
                {
                    genNewAccount(_context, model);
                    _context.SaveChanges();
                    if (model.system_idm_user_type == IDMUserType.temporary)
                    {

                    }
                    else
                    {
                        var result_ldap = _providerldap.CreateUser(model, _context);
                        model.ldap_created = result_ldap.result;
                        if (result_ldap.result == true)
                            writelog(LogType.log_create_account, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                        else
                            writelog(LogType.log_create_account, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                        var result_ad = _provider.CreateUser(model, _context);
                        model.ad_created = result_ad.result;
                        if (result_ad.result == true)
                            writelog(LogType.log_create_account, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                        else
                            writelog(LogType.log_create_account, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);
                        _context.SaveChanges();
                    }
                    writelog(LogType.log_create_account, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);
                }
                catch (Exception ex)
                {
                    writelog(LogType.log_create_account, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);
                }

                ViewBag.Message = ReturnMessage.Success;
                ViewBag.ReturnCode = ReturnCode.Success;
                return RedirectToAction("CreateAccountCompleted", new { code = ReturnCode.Success, msg = ReturnMessage.Success, id = model.id });

            }

            var orgs = await _providerldap.GetOrganizationLvl1(_context, _conf);
            ViewBag.ListOrganization = orgs;
            ViewBag.ListFaculty = this.ListFaculty();
            return View(model);
        }

        public IActionResult CreateAccountCompleted(ReturnCode code, string msg, int? id)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var model = new visual_fim_user();
            if (id.HasValue)
                model = _context.table_visual_fim_user.Where(w => w.id == id).FirstOrDefault();

            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.Message = msg;
                ViewBag.ReturnCode = code;
            }
            return View(model);
        }
        #endregion

        #region CreateAccountFromFile
        public IActionResult CreateAccountFromFile(SearchDTO model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");
            model.lists = (new List<import>()).AsQueryable();
            if (model.code == ReturnCode.Success)
                model.lists = (_context.table_import.Where(w => w.import_Type == ImportType.create)).AsQueryable();
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateAccountFromFile(IFormFile file, ImportCreateOption import_option)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var model = new SearchDTO();
            var lists = new List<import>();
            if (file != null)
            {
                _context.table_import.RemoveRange(_context.table_import.Where(w => w.import_Type == ImportType.create));
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string input;
                    var row = 1;
                    while ((input = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(input))
                            continue;
                        var columnNameList = input.Split(":");
                        if (import_option == ImportCreateOption.staff_hr | import_option == ImportCreateOption.staff_other)
                        {
                            columnNameList = input.Split("\t");
                        }
                        var remark = new StringBuilder();
                        var imp = new import();
                        imp.ImportVerify = true;
                        imp.ImportRow = row;
                        imp.basic_uid = "";
                        imp.basic_givenname = "";
                        imp.basic_sn = "";
                        imp.cu_pplid = "";
                        imp.LockStaus = "";
                        imp.import_Type = ImportType.create;
                        imp.import_create_option = import_option;
                        try
                        {
                            var j = 0;
                            if (import_option == ImportCreateOption.student)
                            {
                                if (columnNameList.Length != 11)
                                {
                                    ModelState.AddModelError("format_error", "รูปแบบไฟล์ไม่ถูกต้อง");
                                    return View(model);
                                }
                                imp.system_idm_user_types = IDMUserType.student;
                                imp.cu_jobcode = columnNameList[j]; j++;
                                /*imp.other_prename = columnNameList[j];*/j++;
                                /*imp.prename = columnNameList[j];*/j++;
                                imp.basic_givenname = columnNameList[j]; j++;
                                imp.basic_sn = columnNameList[j]; j++;
                                /*imp.other_prenameth =columnNameList[j];*/j++;
                                /*imp.prenameth = columnNameList[j];*/j++;
                                imp.cu_thcn = columnNameList[j]; j++;
                                imp.cu_thsn = columnNameList[j]; j++;
                                /*imp.barcode = columnNameList[j];*/j++;
                                imp.cu_pplid = columnNameList[j]; j++;

                                if (string.IsNullOrEmpty(imp.cu_jobcode))
                                    continue;

                                imp.basic_uid = imp.cu_jobcode;
                                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == imp.basic_uid.ToLower()).FirstOrDefault();
                                if (fim_user != null)
                                {
                                    imp.ImportVerify = false;
                                    remark.AppendLine("ผู้ใช้ซ้ำในระบบ");
                                }
                            }
                            else if (import_option == ImportCreateOption.student_sasin)
                            {
                                if (columnNameList.Length != 7)
                                {
                                    ModelState.AddModelError("format_error", "รูปแบบไฟล์ไม่ถูกต้อง");
                                    return View(model);
                                }
                                imp.system_idm_user_types = IDMUserType.student;
                                imp.faculty_shot_name = columnNameList[j]; j++;
                                imp.cu_jobcode = columnNameList[j]; j++;
                                imp.basic_givenname = columnNameList[j]; j++;
                                imp.basic_sn = columnNameList[j]; j++;
                                imp.cu_thcn = columnNameList[j]; j++;
                                imp.cu_thsn = columnNameList[j]; j++;
                                imp.cu_pplid = columnNameList[j]; j++;

                                if (string.IsNullOrEmpty(imp.cu_jobcode))
                                    continue;

                                imp.basic_uid = imp.cu_jobcode;
                                if (imp.cu_jobcode.Length > 10)
                                    imp.basic_uid = imp.cu_jobcode.Substring(0, 10);

                                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == imp.basic_uid.ToLower()).FirstOrDefault();
                                if (fim_user != null)
                                {
                                    imp.ImportVerify = false;
                                    remark.AppendLine("ผู้ใช้ซ้ำในระบบ");
                                }
                            }
                            else if (import_option == ImportCreateOption.student_ppc)
                            {
                                if (columnNameList.Length != 7)
                                {
                                    ModelState.AddModelError("format_error", "รูปแบบไฟล์ไม่ถูกต้อง");
                                    return View(model);
                                }
                                imp.system_idm_user_types = IDMUserType.student;
                                imp.faculty_shot_name = columnNameList[j]; j++;
                                imp.cu_jobcode = columnNameList[j]; j++;
                                imp.basic_givenname = columnNameList[j]; j++;
                                imp.basic_sn = columnNameList[j]; j++;
                                imp.cu_thcn = columnNameList[j]; j++;
                                imp.cu_thsn = columnNameList[j]; j++;
                                imp.cu_pplid = columnNameList[j]; j++;

                                if (string.IsNullOrEmpty(imp.cu_jobcode))
                                    continue;

                                imp.basic_uid = imp.cu_jobcode;
                                if (imp.cu_jobcode.Length > 10)
                                    imp.basic_uid = imp.cu_jobcode.Trim().Substring(0, 9) + "p";

                                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == imp.basic_uid.ToLower()).FirstOrDefault();
                                if (fim_user != null)
                                {
                                    imp.ImportVerify = false;
                                    remark.AppendLine("ผู้ใช้ซ้ำในระบบ");
                                }
                            }
                            else if (import_option == ImportCreateOption.student_other)
                            {
                                if (columnNameList.Length != 8)
                                {
                                    ModelState.AddModelError("format_error", "รูปแบบไฟล์ไม่ถูกต้อง");
                                    return View(model);
                                }
                                imp.system_idm_user_types = IDMUserType.student;
                                imp.faculty_shot_name = columnNameList[j]; j++;
                                imp.cu_jobcode = columnNameList[j]; j++;
                                imp.basic_givenname = columnNameList[j]; j++;
                                imp.basic_sn = columnNameList[j]; j++;
                                imp.cu_thcn = columnNameList[j]; j++;
                                imp.cu_thsn = columnNameList[j]; j++;
                                imp.cu_pplid = columnNameList[j]; j++;
                                imp.basic_uid = columnNameList[j]; j++;

                                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == imp.basic_uid.ToLower()).FirstOrDefault();
                                if (fim_user != null)
                                {
                                    imp.ImportVerify = false;
                                    remark.AppendLine("ผู้ใช้ซ้ำในระบบ");
                                }
                            }
                            else if (import_option == ImportCreateOption.staff_hr)
                            {
                                if (columnNameList.Length != 9)
                                {
                                    ModelState.AddModelError("format_error", "รูปแบบไฟล์ไม่ถูกต้อง");
                                    return View(model);
                                }
                                imp.system_idm_user_types = IDMUserType.staff;
                                imp.cu_jobcode = columnNameList[j]; j++;
                                imp.cu_thcn = columnNameList[j]; j++;
                                imp.cu_thsn = columnNameList[j]; j++;
                                imp.basic_givenname = columnNameList[j]; j++;
                                imp.basic_sn = columnNameList[j]; j++;
                                imp.structure_1 = columnNameList[j]; j++;
                                imp.structure_2 = columnNameList[j]; j++;
                                imp.status = columnNameList[j]; j++;
                                imp.cu_pplid = columnNameList[j]; j++;

                            }
                            else if (import_option == ImportCreateOption.staff_other)
                            {
                                if (columnNameList.Length != 10)
                                {
                                    ModelState.AddModelError("format_error", "รูปแบบไฟล์ไม่ถูกต้อง");
                                    return View(model);
                                }
                                imp.system_idm_user_types = IDMUserType.staff;
                                imp.cu_jobcode = columnNameList[j]; j++;
                                imp.cu_thcn = columnNameList[j]; j++;
                                imp.cu_thsn = columnNameList[j]; j++;
                                imp.basic_givenname = columnNameList[j]; j++;
                                imp.basic_sn = columnNameList[j]; j++;
                                imp.structure_1 = columnNameList[j]; j++;
                                imp.structure_2 = columnNameList[j]; j++;
                                imp.status = columnNameList[j]; j++;
                                imp.cu_pplid = columnNameList[j]; j++;
                                imp.cu_CUexpire = columnNameList[j]; j++;
                            }

                            if (import_option != ImportCreateOption.staff_hr)
                            {
                                if (string.IsNullOrEmpty(imp.cu_jobcode))
                                {
                                    imp.ImportVerify = false;
                                    remark.AppendLine("jobcode ไม่สามารถเป็นค่าว่าง");
                                }
                            }
                            if (string.IsNullOrEmpty(imp.basic_givenname))
                            {
                                imp.ImportVerify = false;
                                remark.AppendLine("First Name ไม่สามารถเป็นค่าว่าง");
                            }
                            if (string.IsNullOrEmpty(imp.basic_sn))
                            {
                                imp.ImportVerify = false;
                                remark.AppendLine("Last Name ไม่สามารถเป็นค่าว่าง");
                            }

                            if (string.IsNullOrEmpty(imp.cu_pplid))
                            {
                                imp.ImportVerify = false;
                                remark.AppendLine("Citizen ID ไม่สามารถเป็นค่าว่าง");
                            }
                            else
                            {
                                if (import_option == ImportCreateOption.staff_other || import_option == ImportCreateOption.staff_hr)
                                {
                                    var fim_user = _context.table_visual_fim_user
                                        .Where(w => w.cu_pplid.ToLower() == imp.cu_pplid.ToLower() & (w.system_idm_user_type == IDMUserType.staff | w.system_idm_user_type == IDMUserType.affiliate | w.system_idm_user_type == IDMUserType.outsider | w.system_idm_user_type == IDMUserType.temporary))
                                        .FirstOrDefault();
                                    if (fim_user != null)
                                    {
                                        imp.ImportVerify = false;
                                        remark.AppendLine("Citizen ID ซ้ำในระบบ");
                                    }
                                }
                                else
                                {
                                    var fim_user = _context.table_visual_fim_user
                                        .Where(w => w.cu_pplid.ToLower() == imp.cu_pplid.ToLower() & w.system_idm_user_type == IDMUserType.student)
                                        .FirstOrDefault();
                                    if (fim_user != null)
                                    {
                                        imp.ImportVerify = false;
                                        remark.AppendLine("Citizen ID ซ้ำในระบบ");
                                    }
                                }
                               
                            }


                            imp.ImportRemark = remark.ToString();
                        }
                        catch (Exception ex)
                        {
                            remark.AppendLine(ex.Message);
                            imp.ImportVerify = false;
                        }
                        lists.Add(imp);
                        if (imp.ImportVerify == true)
                            _context.table_import.Add(imp);
                        row++;
                    }
                }
                _context.SaveChanges();
            }
            model.lists = lists.AsQueryable();
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateAccountFromFile2()
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            var msg = ReturnMessage.ImportFail;
            var code = ReturnCode.Error;

            var imports = _context.table_import.Where(w => w.import_Type == ImportType.create).OrderBy(o => o.ImportRow);
            foreach (var imp in imports.ToList())
            {
                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == imp.basic_uid.ToLower()).FirstOrDefault();
                if (fim_user != null)
                {
                    imp.ImportVerify = false;
                    imp.ImportRemark = "ผู้ใช้ซ้ำในระบบ";
                    continue;
                }
                var model = new visual_fim_user();
                model.cu_jobcode = imp.cu_jobcode;
                model.basic_givenname = imp.basic_givenname;
                model.basic_sn = imp.basic_sn;
                model.cu_thcn = imp.cu_thcn;
                model.cu_thsn = imp.cu_thsn;
                model.cu_pplid = imp.cu_pplid;
                model.cu_CUexpire = imp.cu_CUexpire;
                model.basic_telephonenumber = "0";
                if (string.IsNullOrEmpty(imp.ImportRemark))
                    imp.ImportRemark = "";
                faculty faculty = null;

                if (imp.import_create_option == ImportCreateOption.student | imp.import_create_option == ImportCreateOption.student_sasin | imp.import_create_option == ImportCreateOption.student_ppc | imp.import_create_option == ImportCreateOption.student_other)
                {
                    if (imp.import_create_option == ImportCreateOption.student)
                    {
                        var faculty_id = NumUtil.ParseInteger(imp.cu_jobcode.Substring(imp.cu_jobcode.Length - 2));
                        faculty = _context.table_cu_faculty.Where(w => w.faculty_id == faculty_id).FirstOrDefault();
                    }
                    else if (imp.import_create_option == ImportCreateOption.student_sasin)
                    {
                        faculty = _context.table_cu_faculty.Where(w => w.faculty_shot_name.ToLower() == imp.faculty_shot_name.ToLower()).FirstOrDefault();
                    }
                    else if (imp.import_create_option == ImportCreateOption.student_ppc)
                    {
                        faculty = _context.table_cu_faculty.Where(w => w.faculty_shot_name.ToLower() == imp.faculty_shot_name.ToLower()).FirstOrDefault();
                    }
                    else if (imp.import_create_option == ImportCreateOption.student_other)
                    {
                        faculty = _context.table_cu_faculty.Where(w => w.faculty_shot_name.ToLower() == imp.faculty_shot_name.ToLower()).FirstOrDefault();
                    }

                }
                else if (imp.import_create_option == ImportCreateOption.staff_hr | imp.import_create_option == ImportCreateOption.staff_other)
                {
                    faculty = _context.table_cu_faculty.Where(w => w.faculty_name.ToLower() == imp.structure_1.ToLower() | w.faculty_name.ToLower() == imp.structure_2.ToLower()).FirstOrDefault();
                    if (faculty == null)
                    {
                        var subfaculty = _context.table_cu_faculty_level2.Where(w => w.sub_office_name.ToLower() == imp.structure_1.ToLower() | w.sub_office_name.ToLower() == imp.structure_2.ToLower()).FirstOrDefault();
                        if (subfaculty != null)
                        {
                            faculty = _context.table_cu_faculty.Where(w => w.faculty_id == subfaculty.faculty_id).FirstOrDefault();
                        }
                    }
                }
                if (faculty != null)
                {
                    model.system_faculty_id = (int)faculty.faculty_id;
                    var distinguish_name = "";
                    if (imp.import_create_option == ImportCreateOption.student | imp.import_create_option == ImportCreateOption.student_sasin | imp.import_create_option == ImportCreateOption.student_ppc | imp.import_create_option == ImportCreateOption.student_other)
                    {
                        distinguish_name = faculty.faculty_distinguish_name_student;
                    }
                    else if (imp.import_create_option == ImportCreateOption.staff_hr | imp.import_create_option == ImportCreateOption.staff_other)
                    {
                        if (imp.status.ToLower().Trim() == "student".ToLower())
                            distinguish_name = faculty.faculty_distinguish_name_student;
                        else if (imp.status.ToLower().Trim() == "staff".ToLower() || imp.status.ToLower().Trim() == "พนักงานปกติ".ToLower())
                            distinguish_name = faculty.faculty_distinguish_name_staff;
                        else if (imp.status.ToLower().Trim() == "outsider".ToLower())
                            distinguish_name = faculty.faculty_distinguish_name_outsider;
                        else if (imp.status.ToLower().Trim() == "affiliate".ToLower())
                            distinguish_name = faculty.faculty_distinguish_name_affiliate;
                    }
                    if (!string.IsNullOrEmpty(distinguish_name))
                    {
                        var ous = distinguish_name.Split(",");
                        if (ous.Length > 3)
                        {
                            for (var i = ous.Length - 1; i >= 0; i--)
                            {
                                var ou = ous[i];
                                if (i < 3)
                                {
                                    if (ous.Length == 6)
                                    {
                                        if (i == 2)
                                            model.system_ou_lvl1 = ou;
                                        else if (i == 1)
                                            model.system_ou_lvl2 = ou;
                                        else if (i == 0)
                                            model.system_ou_lvl3 = ou;
                                    }
                                    else if (ous.Length == 5)
                                    {
                                        if (i == 1)
                                            model.system_ou_lvl1 = ou;
                                        else if (i == 0)
                                            model.system_ou_lvl2 = ou;
                                    }
                                    else if (ous.Length == 4)
                                    {
                                        if (i == 0)
                                            model.system_ou_lvl1 = ou;
                                    }
                                }
                            }
                        }
                    }
                    try
                    {

                        genNewAccount(_context, model);
                        _context.SaveChanges();

                        var result_ldap = _providerldap.CreateUser(model, _context);
                        model.ldap_created = result_ldap.result;
                        if (result_ldap.result == true)
                            writelog(LogType.log_create_account_with_file, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                        else
                        {
                            imp.ImportVerify = false;
                            imp.ImportRemark += writelog(LogType.log_create_account_with_file, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message) + Environment.NewLine;

                        }

                        var result_ad = _provider.CreateUser(model, _context);
                        model.ad_created = result_ad.result;
                        if (result_ad.result == true)
                            writelog(LogType.log_create_account_with_file, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                        else
                        {
                            imp.ImportVerify = false;
                            imp.ImportRemark += writelog(LogType.log_create_account_with_file, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message) + Environment.NewLine;
                        }

                        writelog(LogType.log_create_account_with_file, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);

                        if (result_ldap.result == true && result_ad.result == true)
                            imp.ImportRemark = "สร้างบัญชีผู้ใช้สำเร็จ";
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        imp.ImportRemark += writelog(LogType.log_create_account_with_file, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message) + Environment.NewLine;
                    }
                }
            }
            //_context.table_import.RemoveRange(_context.table_import.Where(w => w.import_Type == ImportType.create));
            _context.SaveChanges();
            msg = ReturnMessage.ImportSuccess;
            code = ReturnCode.Success;
            return RedirectToAction("CreateAccountFromFile", new { code = code, msg = msg });
        }


        #endregion

        #region ManageAccount
        public IActionResult ManageAccount(SearchDTO model)
        {
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;

            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            if (string.IsNullOrEmpty(model.text_search))
                return View(model);

            model.text_search = model.text_search.Trim();
            if (model.text_search.Length <= 3)
            {
                ModelState.AddModelError("text_search", "คำค้นจะต้องมากกว่า 3 ตัวอักษร");
                return View(model);
            }

            var lists = this._context.table_visual_fim_user.Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(model.text_search))
            {
                lists = lists.Where(w => (!string.IsNullOrEmpty(w.basic_uid) && w.basic_uid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_givenname) && w.basic_givenname.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_sn) && w.basic_sn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thcn) && w.cu_thcn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thsn) && w.cu_thsn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_cn) && w.basic_cn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_pplid) && w.cu_pplid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_jobcode) && w.cu_jobcode.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mobile) && w.basic_mobile.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mail) && w.basic_mail.ToLower().Contains(model.text_search.ToLower())));
            }

            if (model.usertype_search.HasValue)
                lists = lists.Where(w => w.system_idm_user_type == model.usertype_search);

            lists = lists.OrderByDescending(o => o.system_create_date);

            int skipRows = (model.pageno - 1) * _pagelen;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / _pagelen;
            if (itemcnt % _pagelen > 0)
                pagelen += 1;

            model.itemcnt = itemcnt;
            model.pagelen = pagelen;
            //model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();
            return View(model);
        }
        public IActionResult AccountInfo(int? id)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var model = new visual_fim_user();
            if (id.HasValue)
            {
                model = _context.table_visual_fim_user.Where(w => w.id == id).FirstOrDefault();
                if (model != null)
                {
                    if (!string.IsNullOrEmpty(model.cu_CUexpire))
                    {
                        var exps = model.cu_CUexpire.Split("-");
                        if (exps.Length == 3)
                        {
                            model.cu_CUexpire_day = NumUtil.ParseInteger(exps[0]);
                            model.cu_CUexpire_month = DateUtil.GetMonth(exps[1]);
                            model.cu_CUexpire_year = NumUtil.ParseInteger(exps[2]);
                        }
                    }
                    else
                    {
                        model.cu_CUexpire_select = true;
                        model.cu_CUexpire_day = DateUtil.Now().Day;
                        model.cu_CUexpire_month = DateUtil.Now().Month;
                        model.cu_CUexpire_year = DateUtil.Now().Year + 1;
                    }

                    var aduser = _provider.GetAdUser(model.basic_uid, _context);
                    var ldapuser = _providerldap.GetUser(model.basic_uid, _context);

                    if (aduser != null)
                        model.ad_created = true;

                    if (ldapuser != null)
                        model.ldap_created = true;
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult AccountInfo(visual_fim_user model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            if (ModelState.IsValid)
            {
                ViewBag.Message = ReturnMessage.Error;
                ViewBag.ReturnCode = ReturnCode.Error;

                if (model.id > 0)
                {
                    var fim_user = _context.table_visual_fim_user.Where(w => w.id == model.id).FirstOrDefault();
                    if (fim_user != null)
                    {
                        try
                        {
                            string ou = "";
                            if (!string.IsNullOrEmpty(fim_user.cu_gecos))
                                ou = fim_user.cu_gecos.Split(',')[1].Trim();

                            fim_user.basic_displayname = model.basic_cn;
                            fim_user.basic_givenname = model.basic_givenname;
                            fim_user.basic_sn = model.basic_sn;
                            fim_user.cu_thcn = model.cu_thcn;
                            fim_user.cu_thsn = model.cu_thsn;
                            fim_user.basic_mobile = model.basic_mobile;
                            fim_user.basic_telephonenumber = model.basic_telephonenumber;
                            fim_user.cu_jobcode = model.cu_jobcode;
                            fim_user.cu_pplid = model.cu_pplid;
                            fim_user.cu_gecos = model.basic_givenname + " " + model.basic_sn + ", " + ou + ", " + model.basic_telephonenumber;
                            fim_user.cu_pplid = model.cu_pplid;
                            fim_user.internetaccess = model.internetaccess;
                            fim_user.unix_inetCOS = model.unix_inetCOS;
                            fim_user.system_modify_by_uid = userlogin.basic_uid;
                            fim_user.system_modify_date = DateUtil.Now();
                            fim_user.ad_created = model.ad_created;
                            fim_user.ldap_created = model.ldap_created;

                            if (model.cu_CUexpire_select == false & model.cu_CUexpire_day.HasValue & model.cu_CUexpire_month.HasValue & model.cu_CUexpire_year.HasValue)
                                fim_user.cu_CUexpire = model.cu_CUexpire_day + "-" + DateUtil.GetShortMonth(model.cu_CUexpire_month) + "-" + model.cu_CUexpire_year.ToString().Substring(2);
                            else
                                fim_user.cu_CUexpire = "";

                            _context.SaveChanges();

                            var result_ldap = _providerldap.UpdateUser(fim_user, _context);
                            if (result_ldap.result == true)
                                writelog(LogType.log_edit_account, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                            else
                                writelog(LogType.log_edit_account, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                            var result_ad = _provider.UpdateUser(fim_user, _context);
                            if (result_ad.result == true)
                                writelog(LogType.log_edit_account, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                            else
                                writelog(LogType.log_edit_account, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                            writelog(LogType.log_edit_account, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);

                            return RedirectToAction("ManageAccount", "Account", new { code = ReturnCode.Success, msg = ReturnMessage.Success });
                        }
                        catch (Exception ex)
                        {
                            writelog(LogType.log_edit_account, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);
                        }

                    }

                }
            }
            return View(model);
        }
        #endregion

        #region DeleteAccount
        public IActionResult DeleteAccount(SearchDTO model)
        {
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;

            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            if (string.IsNullOrEmpty(model.text_search))
                return View(model);

            model.text_search = model.text_search.Trim();
            if (model.text_search.Length <= 3)
            {
                ModelState.AddModelError("text_search", "คำค้นจะต้องมากกว่า 3 ตัวอักษร");
                return View(model);
            }

            var lists = this._context.table_visual_fim_user.Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(model.text_search))
            {
                lists = lists.Where(w => (!string.IsNullOrEmpty(w.basic_uid) && w.basic_uid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_givenname) && w.basic_givenname.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_sn) && w.basic_sn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thcn) && w.cu_thcn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thsn) && w.cu_thsn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_cn) && w.basic_cn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_pplid) && w.cu_pplid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_jobcode) && w.cu_jobcode.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mobile) && w.basic_mobile.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mail) && w.basic_mail.ToLower().Contains(model.text_search.ToLower())));
            }

            if (model.usertype_search.HasValue)
                lists = lists.Where(w => w.system_idm_user_type == model.usertype_search);

            lists = lists.OrderByDescending(o => o.system_create_date);

            int skipRows = (model.pageno - 1) * _pagelen;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / _pagelen;
            if (itemcnt % _pagelen > 0)
                pagelen += 1;

            model.itemcnt = itemcnt;
            model.pagelen = pagelen;
            //model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();

            return View(model);
        }

        public JsonResult Delete(string choose)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return Json(new { error = ReturnMessage.Error, result = ReturnCode.Error });

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin != null)
            {
                if (!string.IsNullOrEmpty(choose))
                {
                    var lists = choose.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (lists.Count() > 0)
                    {
                        var fim_users = _context.table_visual_fim_user.Where(w => lists.Contains(w.basic_uid));
                        if (fim_users.Count() > 0)
                        {
                            foreach (var model in fim_users.ToList())
                            {
                                try
                                {
                                    var result_ldap = _providerldap.DeleteUser(model, _context);
                                    if (result_ldap.result == true)
                                        writelog(LogType.log_delete_account, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                                    else
                                        writelog(LogType.log_delete_account, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                                    var result_ad = _provider.DeleteUser(model, _context);
                                    if (result_ad.result == true)
                                        writelog(LogType.log_delete_account, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                                    else
                                        writelog(LogType.log_delete_account, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                                    _context.Remove(model);
                                    _context.SaveChanges();
                                    writelog(LogType.log_delete_account, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);
                                }
                                catch (Exception ex)
                                {
                                    writelog(LogType.log_delete_account, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);
                                }

                            }


                            return Json(new { error = ReturnMessage.Success, result = ReturnCode.Success });
                        }
                    }
                }
            }
            return Json(new { error = ReturnMessage.Error, result = ReturnCode.Error });
        }
        #endregion

        #region DeleteAccountFromFile
        public IActionResult DeleteAccountFromFile(SearchDTO model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            model.lists = (new List<import>()).AsQueryable();
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteAccountFromFile(IFormFile file, ImportDeleteOption import_option)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var model = new SearchDTO();
            var lists = new List<import>();
            if (file != null)
            {
                _context.table_import.RemoveRange(_context.table_import.Where(w => w.import_Type == ImportType.delete));
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string input;
                    var row = 1;
                    while ((input = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(input))
                            continue;
                        var columnNameList = input.Split(":");
                        var remark = new StringBuilder();
                        var imp = new import();
                        imp.ImportVerify = true;
                        imp.ImportRow = row;
                        imp.basic_uid = "";
                        imp.basic_givenname = "";
                        imp.basic_sn = "";
                        imp.cu_pplid = "";
                        imp.LockStaus = "";
                        imp.import_Type = ImportType.delete;
                        try
                        {
                            var j = 0;
                            if (import_option == ImportDeleteOption.pplid)
                            {
                                imp.cu_pplid = columnNameList[j]; j++;
                                if (string.IsNullOrEmpty(imp.cu_pplid))
                                    continue;

                                var fim_users = _context.table_visual_fim_user.Where(w => w.cu_pplid.ToLower() == imp.cu_pplid.ToLower());
                                if (fim_users.Count() == 0)
                                {
                                    imp.ImportVerify = false;
                                    remark.AppendLine("ไม่พบข้อมูลผู้ใช้ที่มี Citizen ID นี้");
                                }
                                foreach (var fim_user in fim_users)
                                {
                                    imp.basic_uid += fim_user.basic_uid + "|";
                                    imp.basic_givenname += fim_user.basic_givenname + "|";
                                    imp.basic_sn += fim_user.basic_sn + "|";
                                    imp.LockStaus += fim_user.cu_nsaccountlock + "|";
                                }
                            }
                            else if (import_option == ImportDeleteOption.loginname)
                            {
                                imp.basic_uid = columnNameList[j]; j++;
                                if (string.IsNullOrEmpty(imp.basic_uid))
                                    continue;

                                var fim_users = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == imp.basic_uid.ToLower());
                                if (fim_users.Count() == 0)
                                {
                                    imp.ImportVerify = false;
                                    remark.AppendLine("ไม่พบข้อมูลผู้ใช้ที่มี Login Name นี้");
                                }
                                foreach (var fim_user in fim_users)
                                {
                                    imp.cu_pplid = fim_user.cu_pplid;
                                    imp.basic_givenname = fim_user.basic_givenname;
                                    imp.basic_sn = fim_user.basic_sn;
                                    imp.LockStaus = fim_user.cu_nsaccountlock;

                                }
                            }
                            imp.ImportRemark = remark.ToString();

                        }
                        catch (Exception ex)
                        {
                            remark.AppendLine(ex.Message);
                            imp.ImportVerify = false;
                        }
                        lists.Add(imp);
                        if (imp.ImportVerify == true)
                            _context.table_import.Add(imp);
                        row++;
                    }
                }
                _context.SaveChanges();


            }
            model.lists = lists.AsQueryable();
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteAccountFromFile2()
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            var msg = ReturnMessage.ImportFail;
            var code = ReturnCode.Error;

            var imports = _context.table_import.Where(w => w.import_Type == ImportType.delete).OrderBy(o => o.ImportRow);
            foreach (var import in imports.ToList())
            {
                var basic_uids = import.basic_uid.Split("|", StringSplitOptions.RemoveEmptyEntries);
                foreach (var basic_uid in basic_uids)
                {
                    var model = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == basic_uid.ToLower()).FirstOrDefault();
                    if (model != null)
                    {
                        try
                        {
                            var result_ldap = _providerldap.DeleteUser(model, _context);
                            if (result_ldap.result == true)
                                writelog(LogType.log_delete_account_with_file, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                            else
                                writelog(LogType.log_delete_account_with_file, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                            var result_ad = _provider.DeleteUser(model, _context);
                            if (result_ad.result == true)
                                writelog(LogType.log_delete_account_with_file, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                            else
                                writelog(LogType.log_delete_account_with_file, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                            _context.Remove(model);
                            _context.SaveChanges();
                            writelog(LogType.log_delete_account_with_file, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);
                        }
                        catch (Exception ex)
                        {
                            writelog(LogType.log_delete_account_with_file, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);
                        }
                    }
                }
            }
            _context.table_import.RemoveRange(_context.table_import.Where(w => w.import_Type == ImportType.delete));
            _context.SaveChanges();
            msg = ReturnMessage.Success;
            code = ReturnCode.Success;
            return RedirectToAction("DeleteAccountFromFile", new { code = code, msg = msg });
        }
        #endregion

        #region ResetPassword
        public IActionResult ResetPassword(SearchDTO model)
        {
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;

            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            if (string.IsNullOrEmpty(model.text_search))
                return View(model);

            model.text_search = model.text_search.Trim();
            if (model.text_search.Length <= 3)
            {
                ModelState.AddModelError("text_search", "คำค้นจะต้องมากกว่า 3 ตัวอักษร");
                return View(model);
            }

            var lists = this._context.table_visual_fim_user.Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(model.text_search))
            {
                lists = lists.Where(w => (!string.IsNullOrEmpty(w.basic_uid) && w.basic_uid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_givenname) && w.basic_givenname.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_sn) && w.basic_sn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thcn) && w.cu_thcn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thsn) && w.cu_thsn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_cn) && w.basic_cn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_pplid) && w.cu_pplid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_jobcode) && w.cu_jobcode.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mobile) && w.basic_mobile.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mail) && w.basic_mail.ToLower().Contains(model.text_search.ToLower())));
            }

            if (model.usertype_search.HasValue)
                lists = lists.Where(w => w.system_idm_user_type == model.usertype_search);

            lists = lists.OrderByDescending(o => o.system_create_date);

            int skipRows = (model.pageno - 1) * _pagelen;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / _pagelen;
            if (itemcnt % _pagelen > 0)
                pagelen += 1;

            model.itemcnt = itemcnt;
            model.pagelen = pagelen;
            //model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();
            return View(model);
        }

        public IActionResult ChangePassword(string id)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var model = new ChangePassword3DTO();
            model.basic_uid = id;
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePassword3DTO model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            if (ModelState.IsValid)
            {
                var msg = ReturnMessage.ChangePasswordFail;
                var code = ReturnCode.Error;
                ViewBag.Message = msg;
                ViewBag.ReturnCode = code;
                try
                {
                    var fim_user = this._context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == model.basic_uid.ToLower()).FirstOrDefault();
                    if (fim_user != null)
                    {
                        fim_user.basic_userPassword = Cryptography.encrypt(model.Password);
                        fim_user.cu_pwdchangeddate = DateUtil.Now();
                        fim_user.cu_pwdchangedby = userlogin.basic_uid;
                        fim_user.cu_pwdchangedloc = getClientIP();
                    }
                    _context.SaveChanges();
                    var result_ldap = _providerldap.ChangePwd(fim_user, model.Password, _context);
                    if (result_ldap.result == true)
                        writelog(LogType.log_reset_password, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                    else
                        writelog(LogType.log_reset_password, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                    var result_ad = _provider.ChangePwd(fim_user, model.Password, _context);
                    if (result_ad.result == true)
                        writelog(LogType.log_reset_password, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                    else
                        writelog(LogType.log_reset_password, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                    writelog(LogType.log_reset_password, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);

                    msg = ReturnMessage.ChangePasswordSuccess;
                    code = ReturnCode.Success;
                    ViewBag.Message = msg;
                    ViewBag.ReturnCode = code;
                    return RedirectToAction("ResetPassword", "Account", new { code = code, msg = msg });
                }
                catch (Exception ex)
                {
                    writelog(LogType.log_reset_password, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);
                }
            }
            return View(model);
        }
        #endregion

        #region EnableAccount
        public IActionResult EnableAccount(SearchDTO model)
        {
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;

            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            if (string.IsNullOrEmpty(model.text_search))
                return View(model);

            model.text_search = model.text_search.Trim();
            if (model.text_search.Length <= 3)
            {
                ModelState.AddModelError("text_search", "คำค้นจะต้องมากกว่า 3 ตัวอักษร");
                return View(model);
            }

            var lists = this._context.table_visual_fim_user.Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(model.text_search))
            {
                lists = lists.Where(w => (!string.IsNullOrEmpty(w.basic_uid) && w.basic_uid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_givenname) && w.basic_givenname.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_sn) && w.basic_sn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thcn) && w.cu_thcn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thsn) && w.cu_thsn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_cn) && w.basic_cn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_pplid) && w.cu_pplid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_jobcode) && w.cu_jobcode.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mobile) && w.basic_mobile.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mail) && w.basic_mail.ToLower().Contains(model.text_search.ToLower())));
            }

            if (model.usertype_search.HasValue)
                lists = lists.Where(w => w.system_idm_user_type == model.usertype_search);

            lists = lists.OrderByDescending(o => o.system_create_date);

            int skipRows = (model.pageno - 1) * _pagelen;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / _pagelen;
            if (itemcnt % _pagelen > 0)
                pagelen += 1;

            model.itemcnt = itemcnt;
            model.pagelen = pagelen;
            //model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();

            return View(model);
        }
        public JsonResult ChangeStatus(string id)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return Json(new { error = ReturnMessage.Error, result = ReturnCode.Error });

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin != null)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var model = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == id.ToLower()).FirstOrDefault();
                    if (model != null)
                    {
                        model.system_modify_date = DateUtil.Now();
                        model.system_modify_by_uid = userlogin.basic_uid;

                        if (model.cu_nsaccountlock == LockStaus.Lock)
                        {
                            try
                            {
                                model.cu_nsaccountlock = LockStaus.Unlock;
                                model.system_modify_by_uid = userlogin.basic_uid;
                                model.system_modify_date = DateUtil.Now();
                                _context.SaveChanges();
                                var result_ldap = _providerldap.NsLockUser(model, _context);
                                if (result_ldap.result == true)
                                    writelog(LogType.log_unlock_account, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                                else
                                    writelog(LogType.log_unlock_account, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                                var result_ad = _provider.EnableUser(model, _context);
                                if (result_ad.result == true)
                                    writelog(LogType.log_unlock_account, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                                else
                                    writelog(LogType.log_unlock_account, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                                writelog(LogType.log_unlock_account, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);
                            }
                            catch (Exception ex)
                            {
                                writelog(LogType.log_unlock_account, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);

                            }
                        }
                        else
                        {
                            try
                            {
                                model.cu_nsaccountlock = LockStaus.Lock;
                                model.system_modify_by_uid = userlogin.basic_uid;
                                model.system_modify_date = DateUtil.Now();
                                _context.SaveChanges();
                                var result_ldap = _providerldap.NsLockUser(model, _context);
                                if (result_ldap.result == true)
                                    writelog(LogType.log_lock_account, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                                else
                                    writelog(LogType.log_lock_account, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                                var result_ad = _provider.DisableUser(model, _context);
                                if (result_ad.result == true)
                                    writelog(LogType.log_lock_account, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                                else
                                    writelog(LogType.log_lock_account, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                                writelog(LogType.log_lock_account, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);
                            }
                            catch (Exception ex)
                            {
                                writelog(LogType.log_lock_account, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);
                            }

                        }
                        return Json(new { error = ReturnMessage.Success, result = ReturnCode.Success, status = model.cu_nsaccountlock });
                    }
                }
            }


            return Json(new { error = ReturnMessage.Error, result = ReturnCode.Error });
        }
        #endregion

        #region EnableAccountFromFile
        public IActionResult EnableAccountFromFile(SearchDTO model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            model.lists = (new List<import>()).AsQueryable();
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;
            return View(model);
        }

        [HttpPost]
        public IActionResult EnableAccountFromFile(IFormFile file, ImportLockOption import_option)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var model = new SearchDTO();
            var lists = new List<import>();
            if (file != null)
            {
                _context.table_import.RemoveRange(_context.table_import.Where(w => w.import_Type == ImportType.lockunlock));
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string input;
                    var row = 1;
                    while ((input = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(input))
                            continue;
                        var columnNameList = input.Split(":");
                        var remark = new StringBuilder();
                        var imp = new import();
                        imp.ImportVerify = true;
                        imp.ImportRow = row;
                        imp.basic_uid = "";
                        imp.basic_givenname = "";
                        imp.basic_sn = "";
                        imp.cu_pplid = "";
                        imp.LockStaus = "";
                        imp.import_Type = ImportType.lockunlock;
                        try
                        {
                            var j = 0;
                            if (import_option == ImportLockOption.pplid)
                            {
                                imp.cu_pplid = columnNameList[j]; j++;
                                if (string.IsNullOrEmpty(imp.cu_pplid))
                                    continue;

                                var fim_users = _context.table_visual_fim_user.Where(w => w.cu_pplid.ToLower() == imp.cu_pplid.ToLower());
                                if (fim_users.Count() == 0)
                                {
                                    imp.ImportVerify = false;
                                    remark.AppendLine("ไม่พบข้อมูลผู้ใช้ที่มี Citizen ID นี้");
                                }
                                foreach (var fim_user in fim_users)
                                {
                                    imp.basic_uid += fim_user.basic_uid + "|";
                                    imp.basic_givenname += fim_user.basic_givenname + "|";
                                    imp.basic_sn += fim_user.basic_sn + "|";
                                    imp.LockStaus += fim_user.cu_nsaccountlock + "|";
                                }
                            }
                            else if (import_option == ImportLockOption.loginname)
                            {
                                imp.basic_uid = columnNameList[j]; j++;
                                if (string.IsNullOrEmpty(imp.basic_uid))
                                    continue;

                                var fim_users = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == imp.basic_uid.ToLower());
                                if (fim_users.Count() == 0)
                                {
                                    imp.ImportVerify = false;
                                    remark.AppendLine("ไม่พบข้อมูลผู้ใช้ที่มี Login Name นี้");
                                }
                                foreach (var fim_user in fim_users)
                                {
                                    imp.cu_pplid = fim_user.cu_pplid;
                                    imp.basic_givenname = fim_user.basic_givenname;
                                    imp.basic_sn = fim_user.basic_sn;
                                    imp.LockStaus = fim_user.cu_nsaccountlock;

                                }
                            }
                            imp.ImportRemark = remark.ToString();

                        }
                        catch (Exception ex)
                        {
                            remark.AppendLine(ex.Message);
                            imp.ImportVerify = false;
                        }
                        lists.Add(imp);
                        if (imp.ImportVerify == true)
                            _context.table_import.Add(imp);
                        row++;
                    }
                }
                _context.SaveChanges();
            }
            model.lists = lists.AsQueryable();
            return View(model);
        }

        [HttpPost]
        public IActionResult EnableAccountFromFile2(string lockstatus)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            var msg = ReturnMessage.ImportFail;
            var code = ReturnCode.Error;

            var imports = _context.table_import.Where(w => w.import_Type == ImportType.lockunlock).OrderBy(o => o.ImportRow);
            foreach (var import in imports.ToList())
            {
                var basic_uids = import.basic_uid.Split("|", StringSplitOptions.RemoveEmptyEntries);
                foreach (var basic_uid in basic_uids)
                {
                    var model = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == basic_uid.ToLower()).FirstOrDefault();
                    if (model != null)
                    {
                        try
                        {
                            model.cu_nsaccountlock = lockstatus;
                            model.system_modify_by_uid = userlogin.basic_uid;
                            model.system_modify_date = DateUtil.Now();
                            _context.SaveChanges();

                            if (lockstatus == LockStaus.Lock)
                            {
                                var result_ldap = _providerldap.NsLockUser(model, _context);
                                if (result_ldap.result == true)
                                    writelog(LogType.log_lock_account_with_file, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                                else
                                    writelog(LogType.log_lock_account_with_file, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                                var result_ad = _provider.DisableUser(model, _context);
                                if (result_ad.result == true)
                                    writelog(LogType.log_lock_account_with_file, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                                else
                                    writelog(LogType.log_lock_account_with_file, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                                writelog(LogType.log_lock_account_with_file, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                            }
                            else
                            {
                                var result_ldap = _providerldap.NsLockUser(model, _context);
                                if (result_ldap.result == true)
                                    writelog(LogType.log_unlock_account_with_file, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                                else
                                    writelog(LogType.log_unlock_account_with_file, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                                var result_ad = _provider.EnableUser(model, _context);
                                if (result_ad.result == true)
                                    writelog(LogType.log_unlock_account_with_file, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                                else
                                    writelog(LogType.log_unlock_account_with_file, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                                writelog(LogType.log_unlock_account_with_file, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);
                            }

                        }
                        catch (Exception ex)
                        {
                            if (lockstatus == LockStaus.Lock)
                                writelog(LogType.log_lock_account_with_file, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);
                            else
                                writelog(LogType.log_unlock_account_with_file, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);
                        }
                    }
                }

            }
            _context.table_import.RemoveRange(_context.table_import.Where(w => w.import_Type == ImportType.delete));
            _context.SaveChanges();
            msg = ReturnMessage.Success;
            code = ReturnCode.Success;
            return RedirectToAction("EnableAccountFromFile", new { code = code, msg = msg });
        }
        #endregion

        #region InternetAccess
        public IActionResult InternetAccess(SearchDTO model)
        {
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;

            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            if (string.IsNullOrEmpty(model.text_search))
                return View(model);

            model.text_search = model.text_search.Trim();
            if (model.text_search.Length <= 3)
            {
                ModelState.AddModelError("text_search", "คำค้นจะต้องมากกว่า 3 ตัวอักษร");
                return View(model);
            }

            var lists = this._context.table_visual_fim_user.Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(model.text_search))
            {
                lists = lists.Where(w => (!string.IsNullOrEmpty(w.basic_uid) && w.basic_uid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_givenname) && w.basic_givenname.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_sn) && w.basic_sn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thcn) && w.cu_thcn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thsn) && w.cu_thsn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_cn) && w.basic_cn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_pplid) && w.cu_pplid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_jobcode) && w.cu_jobcode.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mobile) && w.basic_mobile.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mail) && w.basic_mail.ToLower().Contains(model.text_search.ToLower())));
            }

            if (model.usertype_search.HasValue)
                lists = lists.Where(w => w.system_idm_user_type == model.usertype_search);

            lists = lists.OrderByDescending(o => o.system_create_date);

            int skipRows = (model.pageno - 1) * _pagelen;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / _pagelen;
            if (itemcnt % _pagelen > 0)
                pagelen += 1;

            model.itemcnt = itemcnt;
            model.pagelen = pagelen;
            //model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();

            return View(model);
        }
        public IActionResult InternetAccessInfo(int? id)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var model = new visual_fim_user();
            if (id.HasValue)
                model = _context.table_visual_fim_user.Where(w => w.id == id).FirstOrDefault();

            return View(model);
        }
        [HttpPost]
        public IActionResult InternetAccessInfo(visual_fim_user model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            if (ModelState.IsValid)
            {
                ViewBag.Message = ReturnMessage.Error;
                ViewBag.ReturnCode = ReturnCode.Error;

                if (model.id > 0)
                {
                    var fim_user = _context.table_visual_fim_user.Where(w => w.id == model.id).FirstOrDefault();
                    if (fim_user != null)
                    {
                        try
                        {
                            model.basic_uid = fim_user.basic_uid;
                            fim_user.internetaccess = model.internetaccess;
                            fim_user.system_modify_by_uid = userlogin.basic_uid;
                            fim_user.system_modify_date = DateUtil.Now();
                            _context.SaveChanges();

                            var result_ldap = _providerldap.UpdateUser(fim_user, _context);
                            if (result_ldap.result == true)
                                writelog(LogType.log_edit_internetaccess, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                            else
                                writelog(LogType.log_edit_internetaccess, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                            var result_ad = _provider.UpdateUser(fim_user, _context);
                            if (result_ad.result == true)
                                writelog(LogType.log_edit_internetaccess, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                            else
                                writelog(LogType.log_edit_internetaccess, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                            writelog(LogType.log_edit_internetaccess, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);

                        }
                        catch (Exception ex)
                        {
                            writelog(LogType.log_edit_account, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);

                        }
                    }
                    return RedirectToAction("InternetAccess", "Account", new { code = ReturnCode.Success, msg = ReturnMessage.Success });

                }
            }
            return View(model);
        }
        #endregion

        #region Approve Change Password
        public IActionResult ApproveChangePassword(SearchDTO model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk, UserRole.approve }))
                return RedirectToAction("Logout", "Auth");

            var lists = this._context.table_reset_password_temp.Where(w => 1 == 1);

            var dfrom = DateUtil.ToDate(model.dfrom);
            var dto = DateUtil.ToDate(model.dto);

            if (!dfrom.HasValue)
            {
                dfrom = DateUtil.Now();
                model.dfrom = DateUtil.ToDisplayDate(DateUtil.Now());
            }
            if (!dto.HasValue)
            {
                dto = DateUtil.Now();
                model.dto = DateUtil.ToDisplayDate(DateUtil.Now());
            }
            lists = lists.Where(w => w.reset_date.Value.Date >= dfrom.Value.Date);
            lists = lists.Where(w => w.reset_date.Value.Date <= dto.Value.Date);
            lists = lists.OrderByDescending(o => o.reset_date);

            int skipRows = (model.pageno - 1) * _pagelen;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / _pagelen;
            if (itemcnt % _pagelen > 0)
                pagelen += 1;

            model.itemcnt = itemcnt;
            model.pagelen = pagelen;
            //model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;
            return View(model);
        }

        public JsonResult Approve(string choose)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk, UserRole.approve }))
                return Json(new { error = ReturnMessage.Error, result = ReturnCode.Error });

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin != null)
            {
                if (!string.IsNullOrEmpty(choose))
                {
                    var lists = choose.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (lists.Count() > 0)
                    {
                        var resets = _context.table_reset_password_temp.Where(w => lists.Contains(w.temp_id.ToString()));
                        if (resets.Count() > 0)
                        {
                            foreach (var model in resets.ToList())
                            {
                                string basic_uid = model.username;
                                try
                                {
                                    string newPassword = Cryptography.decrypt(model.password);

                                    var fim_user = this._context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == basic_uid.ToLower()).FirstOrDefault();
                                    if (fim_user != null)
                                    {
                                        fim_user.basic_userPassword = newPassword;
                                        fim_user.cu_pwdchangeddate = DateUtil.Now();
                                        fim_user.cu_pwdchangedby = userlogin.basic_uid;
                                        fim_user.cu_pwdchangedloc = getClientIP();
                                    }
                                    model.status = "approved";
                                    _context.SaveChanges();
                                    var result_ldap = _providerldap.ChangePwd(fim_user, newPassword, _context);
                                    if (result_ldap.result == true)
                                        writelog(LogType.log_approved_reset_password, LogStatus.successfully, IDMSource.LDAP, basic_uid);
                                    else
                                        writelog(LogType.log_approved_reset_password, LogStatus.failed, IDMSource.LDAP, basic_uid, log_exception: result_ldap.Message);

                                    var result_ad = _provider.ChangePwd(fim_user, newPassword, _context);
                                    if (result_ad.result == true)
                                        writelog(LogType.log_approved_reset_password, LogStatus.successfully, IDMSource.AD, basic_uid);
                                    else
                                        writelog(LogType.log_approved_reset_password, LogStatus.failed, IDMSource.AD, basic_uid, log_exception: result_ad.Message);

                                    writelog(LogType.log_approved_reset_password, LogStatus.successfully, IDMSource.VisualFim, basic_uid);
                                }
                                catch (Exception ex)
                                {
                                    writelog(LogType.log_approved_reset_password, LogStatus.failed, IDMSource.VisualFim, basic_uid, log_exception: ex.Message);
                                }
                            }
                            return Json(new { error = ReturnMessage.Success, result = ReturnCode.Success });
                        }
                    }
                }
            }
            return Json(new { error = ReturnMessage.Error, result = ReturnCode.Error });
        }
        #endregion


        #region ManageAccount
        public IActionResult MoveAccount(SearchDTO model)
        {
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;

            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            if (string.IsNullOrEmpty(model.text_search))
                return View(model);

            model.text_search = model.text_search.Trim();
            if (model.text_search.Length <= 3)
            {
                ModelState.AddModelError("text_search", "คำค้นจะต้องมากกว่า 3 ตัวอักษร");
                return View(model);
            }

            var lists = this._context.table_visual_fim_user.Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(model.text_search))
            {
                lists = lists.Where(w => (!string.IsNullOrEmpty(w.basic_uid) && w.basic_uid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_givenname) && w.basic_givenname.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_sn) && w.basic_sn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thcn) && w.cu_thcn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_thsn) && w.cu_thsn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_cn) && w.basic_cn.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_pplid) && w.cu_pplid.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.cu_jobcode) && w.cu_jobcode.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mobile) && w.basic_mobile.ToLower().Contains(model.text_search.ToLower()))
              | (!string.IsNullOrEmpty(w.basic_mail) && w.basic_mail.ToLower().Contains(model.text_search.ToLower())));
            }

            if (model.usertype_search.HasValue)
                lists = lists.Where(w => w.system_idm_user_type == model.usertype_search);

            lists = lists.OrderByDescending(o => o.system_create_date);

            int skipRows = (model.pageno - 1) * _pagelen;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / _pagelen;
            if (itemcnt % _pagelen > 0)
                pagelen += 1;

            model.itemcnt = itemcnt;
            model.pagelen = pagelen;
            //model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();
            return View(model);
        }
        public async Task<IActionResult> MoveAccountInfo(int? id)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var model = new visual_fim_user();
            if (id.HasValue)
                model = _context.table_visual_fim_user.Where(w => w.id == id).FirstOrDefault();
            var orgs = await _providerldap.GetOrganizationLvl1(_context, _conf);
            ViewBag.ListOrganization = orgs;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> MoveAccountInfo(visual_fim_user model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            if (ModelState.IsValid)
            {
                ViewBag.Message = ReturnMessage.Error;
                ViewBag.ReturnCode = ReturnCode.Error;

                if (model.id > 0)
                {
                    var fim_user = _context.table_visual_fim_user.Where(w => w.id == model.id).FirstOrDefault();
                    if (fim_user != null)
                    {
                        try
                        {
                            var olddn = fim_user.basic_dn;

                            var system_ou_lvl1 = AppUtil.getOuName(model.system_ou_lvl1);
                            var system_ou_lvl2 = AppUtil.getOuName(model.system_ou_lvl2);
                            var system_ou_lvl3 = AppUtil.getOuName(model.system_ou_lvl3);

                            fim_user.basic_dn = "uid=[uid]";
                            if (!string.IsNullOrEmpty(model.system_ou_lvl3))
                                fim_user.basic_dn += "," + model.system_ou_lvl3.ToLower();
                            if (!string.IsNullOrEmpty(model.system_ou_lvl2))
                                fim_user.basic_dn += "," + model.system_ou_lvl2.ToLower();
                            if (!string.IsNullOrEmpty(model.system_ou_lvl1))
                                fim_user.basic_dn += "," + model.system_ou_lvl1.ToLower();
                            fim_user.basic_dn = fim_user.basic_dn.Replace("[uid]", fim_user.basic_uid);
                            fim_user.system_ou_lvl1 = model.system_ou_lvl1;
                            fim_user.system_ou_lvl2 = model.system_ou_lvl2;
                            fim_user.system_ou_lvl3 = model.system_ou_lvl3;
                            fim_user.system_idm_user_type = getIdmUserType(system_ou_lvl1);

                            if (olddn != fim_user.basic_dn)
                            {
                                var officeShotName = "";
                                if (system_ou_lvl1 == "internet")
                                {
                                    officeShotName = system_ou_lvl1.ToUpper();
                                }
                                if (!string.IsNullOrEmpty(system_ou_lvl2))
                                {
                                    officeShotName = AppUtil.getOuName(fim_user.system_ou_lvl2, false).ToUpper();
                                }
                                fim_user.cu_gecos = "";
                                if (string.IsNullOrEmpty(fim_user.basic_cn) == false)
                                {
                                    fim_user.cu_gecos = fim_user.basic_cn;
                                    if (string.IsNullOrEmpty(officeShotName) == false)
                                    {
                                        fim_user.cu_gecos += ", " + officeShotName;
                                        if (string.IsNullOrEmpty(fim_user.basic_telephonenumber) == false)
                                        {
                                            fim_user.cu_gecos += ", " + fim_user.basic_telephonenumber;
                                        }
                                    }
                                }

                                fim_user.system_modify_by_uid = userlogin.basic_uid;
                                fim_user.system_modify_date = DateUtil.Now();
                                _context.SaveChanges();

                                var result_ldap = _providerldap.MoveOU(fim_user, _context);
                                if (result_ldap.result == true)
                                    writelog(LogType.log_move_account, LogStatus.successfully, IDMSource.LDAP, model.basic_uid);
                                else
                                    writelog(LogType.log_move_account, LogStatus.failed, IDMSource.LDAP, model.basic_uid, log_exception: result_ldap.Message);

                                var result_ad = _provider.MoveOU(fim_user, _context);
                                if (result_ad.result == true)
                                    writelog(LogType.log_move_account, LogStatus.successfully, IDMSource.AD, model.basic_uid);
                                else
                                    writelog(LogType.log_move_account, LogStatus.failed, IDMSource.AD, model.basic_uid, log_exception: result_ad.Message);

                                writelog(LogType.log_move_account, LogStatus.successfully, IDMSource.VisualFim, model.basic_uid);
                            }

                            return RedirectToAction("MoveAccount", "Account", new { code = ReturnCode.Success, msg = ReturnMessage.Success });
                        }
                        catch (Exception ex)
                        {
                            writelog(LogType.log_move_account, LogStatus.failed, IDMSource.VisualFim, model.basic_uid, log_exception: ex.Message);
                        }

                    }

                }
            }
            var orgs = await _providerldap.GetOrganizationLvl1(_context, _conf);
            if (orgs.Count() > 0)
                model.system_ou_lvl1 = orgs[0].ouname;
            ViewBag.ListOrganization = orgs;
            return View(model);
        }
        #endregion

        #region CheckAccount
        public IActionResult CheckAccount(SearchDTO model)
        {
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;

            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            if (string.IsNullOrEmpty(model.text_search))
                return View(model);

            model.text_search = model.text_search.Trim();
            if (model.text_search.Length <= 3)
            {
                ModelState.AddModelError("text_search", "คำค้นจะต้องมากกว่า 3 ตัวอักษร");
                return View(model);
            }

            var lists = this._context.table_visual_fim_user.Where(w => 1 == 1);

            if (!string.IsNullOrEmpty(model.text_search))
            {
                lists = lists.Where(w => (!string.IsNullOrEmpty(w.basic_uid) && w.basic_uid.ToLower().Contains(model.text_search.ToLower()))
               | (!string.IsNullOrEmpty(w.basic_givenname) && w.basic_givenname.ToLower().Contains(model.text_search.ToLower()))
               | (!string.IsNullOrEmpty(w.basic_sn) && w.basic_sn.ToLower().Contains(model.text_search.ToLower()))
               | (!string.IsNullOrEmpty(w.cu_thcn) && w.cu_thcn.ToLower().Contains(model.text_search.ToLower()))
               | (!string.IsNullOrEmpty(w.cu_thsn) && w.cu_thsn.ToLower().Contains(model.text_search.ToLower()))
               | (!string.IsNullOrEmpty(w.basic_cn) && w.basic_cn.ToLower().Contains(model.text_search.ToLower()))
               | (!string.IsNullOrEmpty(w.cu_pplid) && w.cu_pplid.ToLower().Contains(model.text_search.ToLower()))
               | (!string.IsNullOrEmpty(w.cu_jobcode) && w.cu_jobcode.ToLower().Contains(model.text_search.ToLower()))
               | (!string.IsNullOrEmpty(w.basic_mobile) && w.basic_mobile.ToLower().Contains(model.text_search.ToLower()))
               | (!string.IsNullOrEmpty(w.basic_mail) && w.basic_mail.ToLower().Contains(model.text_search.ToLower())));
            }


            if (model.usertype_search.HasValue)
                lists = lists.Where(w => w.system_idm_user_type == model.usertype_search);

            lists = lists.OrderByDescending(o => o.system_create_date);

            int skipRows = (model.pageno - 1) * _pagelen;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / _pagelen;
            if (itemcnt % _pagelen > 0)
                pagelen += 1;

            model.itemcnt = itemcnt;
            model.pagelen = pagelen;
            //model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();
            return View(model);
        }
        #endregion

        #region Create Script
        public IActionResult CreateScript(SearchDTO model)
        {
            if (!checkrole(new string[] { UserRole.admin, UserRole.helpdesk }))
                return RedirectToAction("Logout", "Auth");

            var dfrom = DateUtil.ToDate(model.dfrom);
            var dto = DateUtil.ToDate(model.dto);

            if (!dfrom.HasValue)
            {
                dfrom = DateUtil.Now();
                model.dfrom = DateUtil.ToDisplayDate(DateUtil.Now());
            }
            if (!dto.HasValue)
            {
                dto = DateUtil.Now();
                model.dto = DateUtil.ToDisplayDate(DateUtil.Now());
            }

            var script_temps = new List<script_temp>();

            var staffs = this._context.table_receive_staff.Where(w => 1 == 1);
            staffs = staffs.Where(w => w.create_date.Value.Date >= dfrom.Value.Date);
            staffs = staffs.Where(w => w.create_date.Value.Date <= dto.Value.Date);

            foreach (var item in staffs)
            {
                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == item.login_name.ToLower()).FirstOrDefault();
                if (fim_user != null)
                {
                    var script_temp = new script_temp();
                    script_temp.id = item.id;
                    script_temp.displayname = item.displayname;
                    script_temp.login_name = item.login_name;
                    script_temp.password_initial = item.password_initial;
                    script_temp.email_address = item.email_address;
                    script_temp.server_name = item.server_name;
                    script_temp.expire = item.expire;
                    script_temp.status_id = item.status_id;
                    script_temp.org = item.org;
                    script_temp.create_date = item.create_date;
                    script_temp.receive_date = item.receive_date;
                    script_temp.manage_by = item.manage_by;
                    script_temp.ticket = item.ticket;
                    script_temp.visual_fim_user = fim_user;
                    script_temps.Add(script_temp);
                }
            }

            var students = this._context.table_receive_student.Where(w => 1 == 1);
            students = students.Where(w => w.create_date.Value.Date >= dfrom.Value.Date);
            students = students.Where(w => w.create_date.Value.Date <= dto.Value.Date);

            foreach (var item in students)
            {
                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == item.login_name.ToLower()).FirstOrDefault();
                if (fim_user != null)
                {
                    var script_temp = new script_temp();
                    script_temp.id = item.id;
                    script_temp.displayname = item.displayname;
                    script_temp.login_name = item.login_name;
                    script_temp.password_initial = item.password_initial;
                    script_temp.email_address = item.email_address;
                    script_temp.server_name = item.server_name;
                    script_temp.expire = item.expire;
                    script_temp.status_id = item.status_id;
                    script_temp.org = item.org;
                    script_temp.create_date = item.create_date;
                    script_temp.receive_date = item.receive_date;
                    script_temp.manage_by = item.manage_by;
                    script_temp.ticket = item.ticket;
                    script_temp.visual_fim_user = fim_user;
                    script_temps.Add(script_temp);
                }
            }
            var temps = this._context.table_receive_temp.Where(w => 1 == 1);
            temps = temps.Where(w => w.create_date.Value.Date >= dfrom.Value.Date);
            temps = temps.Where(w => w.create_date.Value.Date <= dto.Value.Date);

            foreach (var item in temps)
            {
                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == item.login_name.ToLower()).FirstOrDefault();
                if (fim_user != null)
                {
                    var script_temp = new script_temp();
                    script_temp.id = item.id;
                    script_temp.displayname = item.displayname;
                    script_temp.login_name = item.login_name;
                    script_temp.password_initial = item.password_initial;
                    script_temp.email_address = item.email_address;
                    script_temp.server_name = item.server_name;
                    script_temp.expire = item.expire;
                    script_temp.status_id = item.status_id;
                    script_temp.org = item.org;
                    script_temp.create_date = item.create_date;
                    script_temp.receive_date = item.receive_date;
                    script_temp.manage_by = item.manage_by;
                    script_temp.ticket = item.ticket;
                    script_temp.visual_fim_user = fim_user;
                    script_temps.Add(script_temp);
                }
            }

            script_temps = script_temps.OrderByDescending(o => o.create_date).ToList();

            var Indicator = ":";
            String[] sparams = null;
            if (model.script_format == ScriptFormat.UNIX1)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.UNIX1;
                sparams = model.script_param.Split(":");
            }
            else if (model.script_format == ScriptFormat.Print)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.Print;
                sparams = model.script_param.Split(":");
            }
            else if (model.script_format == ScriptFormat.GW1)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.GW1;
                sparams = model.script_param.Split(":");
            }
            else if (model.script_format == ScriptFormat.GW2)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.GW2;
                sparams = model.script_param.Split(":");
            }
            else if (model.script_format == ScriptFormat.pigeon)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.pigeon;
                sparams = model.script_param.Split(",");
                Indicator = ",";
            }
            else if (model.script_format == ScriptFormat.cano)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.cano;
                sparams = model.script_param.Split(",");
                Indicator = ",";
            }
            else if (model.script_format == ScriptFormat.BB)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.BB;
                sparams = model.script_param.Split("|");
                Indicator = "|";
            }
            else if (model.script_format == ScriptFormat.EDMS)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.EDMS;
                sparams = model.script_param.Split(":");
            }
            else if (model.script_format == ScriptFormat.Info)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.Info;
                sparams = model.script_param.Split(":");
            }
            else if (model.script_format == ScriptFormat.pommo)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.pommo;
                sparams = model.script_param.Split(":");
            }
            else if (model.script_format == ScriptFormat.Other)
            {
                if (string.IsNullOrEmpty(model.script_param))
                    model.script_param = ScriptFormatParam.Other;
                sparams = model.script_param.Split(":");
            }
            var texts = new List<string>();
            foreach (var script in script_temps)
            {
                Type t = script.GetType();
                Type t2 = script.visual_fim_user.GetType();

                var text = new StringBuilder();
                foreach (var sparam in sparams)
                {
                    var name = sparam;
                    var name2 = name;
                    var startindex = name.IndexOf("[");
                    var endindex = name.IndexOf("]");
                    if (startindex >= 0 & endindex >= 0)
                    {
                        name = name.Substring(startindex, (endindex - startindex) + 1);
                        name2 = name;
                        name2 = name2.Replace("[", "");
                        name2 = name2.Replace("]", "");
                    }
                    PropertyInfo pro1 = t.GetProperty(name2);
                    PropertyInfo pro2 = t2.GetProperty(name2);
                    if (pro1 != null)
                    {
                        var val = pro1.GetValue(script);
                        if (name2 == "password_initial" | name2 == "ticket")
                            val = Cryptography.decrypt(val.ToString());

                        if (name != sparam)
                        {
                            var newval = sparam.Replace(name, val.ToString());
                            text.Append(newval);
                            text.Append(Indicator);
                        }
                        else
                        {
                            var newval = "";
                            if (val.GetType().Name == "DateTime")
                                newval = DateUtil.ToDisplayDate4((DateTime)val);
                            else
                            {
                                if (val != null)
                                    newval = val.ToString();
                            }
                            text.Append(newval);
                            text.Append(Indicator);
                        }


                    }
                    else if (pro2 != null)
                    {
                        var val = pro2.GetValue(script.visual_fim_user);
                        if (name2 == "password_initial" | name2 == "ticket")
                            val = Cryptography.decrypt(val.ToString());
                        if (name != sparam)
                        {
                            var newval = sparam.Replace(name, val.ToString());
                            text.Append(newval);
                            text.Append(Indicator);
                        }
                        else
                        {
                            text.Append(val);
                            text.Append(Indicator);
                        }

                    }
                    else if (string.IsNullOrEmpty(name2))
                    {
                        text.Append(Indicator);
                    }
                    else
                    {
                        text.Append(name2);
                        text.Append(Indicator);
                    }
                }
                texts.Add(text.ToString().Substring(0, text.ToString().Length - 1));
            }
            var lists = texts;

            int skipRows = (model.pageno - 1) * _pagelen;
            var itemcnt = lists.Count();
            var pagelen = itemcnt / _pagelen;
            if (itemcnt % _pagelen > 0)
                pagelen += 1;

            model.itemcnt = itemcnt;
            model.pagelen = pagelen;
            //model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();
            ViewBag.Message = model.msg;
            ViewBag.ReturnCode = model.code;

            string webRootPath = _env.WebRootPath;
            var filename = webRootPath + "\\createscript.csv";
            writeTextToFile(filename, texts.ToArray());

            return View(model);
        }
        #endregion
    }
}
