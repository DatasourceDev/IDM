using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IDM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using IDM.DAL;
using IDM.Services;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net.Mail;
using System.Text;
using IDM.DTO;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using IDM.Identity;
using System.Net;
using System.Net.Sockets;
using IDM.Extensions;
using System.Collections;
using System.Data;

namespace IDM.Controllers
{
    public class ControllerBase : Controller
    {
        public const int _pagelen = 100;

        private readonly ILogger<ControllerBase> _logger;
        public SpuContext _context;
        public ILoginServices _loginServices;
        public SystemConf _conf;
        public IUserProvider _provider;
        public ILDAPUserProvider _providerldap;

        public ControllerBase()
        {
        }
        public ControllerBase(SpuContext context, ILogger<ControllerBase> logger, ILoginServices loginServices, IOptions<SystemConf> conf, IUserProvider provider, ILDAPUserProvider providerldap)
        {
            this._context = context;
            this._logger = logger;
            this._loginServices = loginServices;
            this._conf = conf.Value;
            this._provider = provider;
            this._providerldap = providerldap;

        }

        protected SelectList ListFaculty()
        {
            var list = new List<faculty>();
            list.AddRange(this._context.table_cu_faculty.OrderBy(o => o.faculty_name));
            return new SelectList(list, "faculty_id", "faculty_name");
        }
        protected SelectList ListSubFaculty(int? fID)
        {
            var list = new List<faculty_level2>();
            list.AddRange(this._context.table_cu_faculty_level2.Where(w => w.faculty_id == fID).OrderBy(o => o.sub_office_name));
            return new SelectList(list, "sub_office_id", "sub_office_name");
        }

        [HttpGet]
        public async Task<IActionResult> LoadGetOrganizationLvl2(string ou1)
        {
            var list = await _providerldap.GetOrganizationLvl2(_context, _conf, ou1);
            return Json(list);
        }
        [HttpGet]
        public async Task<IActionResult> LoadGetOrganizationLvl3(string ou1, string ou2)
        {
            var list = await _providerldap.GetOrganizationLvl3(_context, _conf, ou1, ou2);
            return Json(list);
        }
        [HttpGet]
        public ActionResult LoadSubFaculty(int? fID)
        {
            return Json(ListSubFaculty(fID));
        }

        [HttpGet]
        public ActionResult LoadIsExistCitizenID(string pplid)
        {
            if (string.IsNullOrEmpty(pplid))
                return Json(new { result = false });
            var query = this._context.table_visual_fim_user.Where(c => c.cu_pplid.ToLower() == pplid.ToLower()).FirstOrDefault(); 
            return Json(new { result  = (query != null) });

        }

        public bool isExistNameSurNameAndCitizenID(visual_fim_user model)
        {
            if (string.IsNullOrEmpty(model.basic_givenname) | string.IsNullOrEmpty(model.basic_sn) /*| string.IsNullOrEmpty(model.cu_pplid)*/)
                return false;
            var query = this._context.table_visual_fim_user.Where(c => c.basic_givenname == model.basic_givenname & c.basic_sn == model.basic_sn /*& c.cu_pplid == model.cu_pplid*/ & (model.id > 0 ? c.id != model.id : true)).FirstOrDefault();
            return (query != null);
        }
        public bool isExistCitizenID(string pplid)
        {
            if (string.IsNullOrEmpty(pplid))
                return false;
            var query = this._context.table_visual_fim_user.Where(c => c.cu_pplid.ToLower() == pplid.ToLower()).FirstOrDefault();
            return (query != null);
        }
        public bool isExistUID(string uid)
        {
            if (string.IsNullOrEmpty(uid))
                return false;
            var query = this._context.table_visual_fim_user.Where(c => c.basic_uid.ToLower() == uid.ToLower()).FirstOrDefault();
            return (query != null);
        }
        public async Task<IActionResult> MailRequestApproveResetPassword(visual_fim_user model)
        {
            var htmlToConvert = await RenderViewAsync("MailRequestApproveResetPassword", model, true);
            var msg = sendNotificationEmail(model.basic_mail, "ยืนยันการร้องขอเปลี่ยนรหัสผ่านของท่าน", htmlToConvert.ToString());

            return Json(new { Msg = msg });
        }
        public async Task<IActionResult> MailRegister(Guest model)
        {
            var htmlToConvert = await RenderViewAsync("MailRegister", model, true);
            var msg = sendNotificationEmail(model.Email, "ลงทะเบียน ผู้ใช้ชั่วคราวสำเร็จ", htmlToConvert.ToString());

            return Json(new { Msg = msg });
        }
        public async Task<IActionResult> MailOTP(string email, OTPDTO model)
        {
            var htmlToConvert = await RenderViewAsync("MailOTP", model, true);
            var msg = sendNotificationEmail(email, "รหัสลงทะเบียน", htmlToConvert.ToString());

            return Json(new { Msg = msg });
        }
        public async Task<string> RenderViewAsync<TModel>(string viewName, TModel model, bool partial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = this.ControllerContext.ActionDescriptor.ActionName;
            }

            this.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = this.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(this.ControllerContext, viewName, !partial);

                if (viewResult.Success == false)
                {
                    return $"A view with the name {viewName} could not be found";
                }

                ViewContext viewContext = new ViewContext(
                    this.ControllerContext,
                    viewResult.View,
                    this.ViewData,
                    this.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }


        }
        public string sendNotificationEmail(string to, string header, string message)
        {
            //to = "voranun@dthai.co.th";
            var setup = this._context.table_setup.FirstOrDefault();
            var msg = new System.Text.StringBuilder();
            try
            {
                var SMTP_SERVER = setup.SMTP_Server;
                var SMTP_PORT = setup.SMTP_Port;
                var SMTP_USERNAME = setup.SMTP_Username;
                var SMTP_PASSWORD = setup.SMTP_Password;
                var SMTP_FROM = setup.SMTP_From;
                bool STMP_SSL = setup.SMTP_SSL.HasValue ? setup.SMTP_SSL.Value : false;

                SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);
                System.Net.NetworkCredential cred = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = STMP_SSL;

                var mail = new MailMessage(SMTP_FROM, to, header, message);
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                smtpClient.Credentials = cred;
                smtpClient.Send(mail);

                return msg.ToString();
            }
            catch (Exception ex)
            {
                msg.AppendLine(" EXCEPTION: " + ex.Message);
            }
            return msg.ToString();
        }

        //public bool checkrole()
        //{
        //    var group = _context.table_group.Where(w => (w.group_name == UserRole.admin | w.group_name == UserRole.helpdesk | w.group_name == UserRole.approve) & w.group_username_list.Contains(this.HttpContext.User.Identity.Name.ToLower())).FirstOrDefault();
        //    if (group == null)
        //        return false;

        //    return true;
        //}
        public bool checkrole(string[] role)
        {
            var group = _context.table_group.Where(w => role.Contains(w.group_name) & w.group_username_list.Contains(this.HttpContext.User.Identity.Name.ToLower())).FirstOrDefault();
            if (group == null)
                return false;

            return true;
        }
        public string RandomPassword(int length = 8, bool includenumber = true, bool includelower = true, bool includeupper = false)
        {
            string valid = "";
            string number = "1234567890";
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (includenumber)
                valid += number;
            if (includelower)
                valid += lower;
            if (includeupper)
                valid += upper;
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
       
        public IDMUserType getIdmUserType(string ou)
        {
            if (ou == IDMUserType.staff.ToString())
                return IDMUserType.staff;
            else if (ou == IDMUserType.student.ToString())
                return IDMUserType.student;
            else if (ou == IDMUserType.outsider.ToString())
                return IDMUserType.outsider;
            else if (ou == IDMUserType.affiliate.ToString())
                return IDMUserType.affiliate;
            else if (ou == IDMUserType.temporary.ToString())
                return IDMUserType.temporary;
            else if (ou == "internet")
                return IDMUserType.temporary;
            return IDMUserType.temporary;
        }
        public string getHostIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = "";
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress += ip.ToString() + " ";
                }
            }
            return ipAddress;
        }
        public string getClientIP()
        {
            string result = "";
            var UserHostAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();
            var UserHostName = this.HttpContext.Connection.LocalIpAddress.ToString();
            string HTTP_X_FORWARDED_FOR = HttpContext.Request.Headers["X-Forwarded-For"];
            string REMOTE_ADDR = HttpContext.Request.Headers["REMOTE_ADDR"];


            if (string.IsNullOrEmpty(REMOTE_ADDR) == false)
            {
                //---------------------------------------------------------------
                //ComputerName = System.Net.Dns.GetHostEntry(REMOTE_ADDR).HostName;
                //---------------------------------------------------------------
                if (string.IsNullOrEmpty(result)) { result = REMOTE_ADDR; }
                else { if (result.Contains(REMOTE_ADDR)) { } else { result += "," + REMOTE_ADDR; } }
            }
            if (string.IsNullOrEmpty(HTTP_X_FORWARDED_FOR) == false)
            {
                string[] ipRange = HTTP_X_FORWARDED_FOR.Split(',');
                for (int i = 0; i < ipRange.Length; i++)
                {
                    if (string.IsNullOrEmpty(result)) { result = ipRange[i]; }
                    else { if (result.Contains(ipRange[i])) { } else { result += "," + ipRange[i]; } }
                }
            }
            if (string.IsNullOrEmpty(UserHostAddress) == false)
            {
                if (string.IsNullOrEmpty(result)) { result = UserHostAddress; }
                else { if (result.Contains(UserHostAddress)) { } else { result += "," + UserHostAddress; } }
            }
            if (string.IsNullOrEmpty(UserHostName) == false)
            {
                if (string.IsNullOrEmpty(result)) { result = UserHostName; }
                else { if (result.Contains(UserHostName)) { } else { result += "," + UserHostName; } }
            }
            return UserHostAddress;
        }

        public string genNewEmailForStudent(string cu_jobcode)
        {

            string email_domain = _conf.DefaultValue_emailDomainForStudent; //"@student.chula.ac.th" *using in basic_mail*;}
            string newEmail = cu_jobcode + email_domain;
            return newEmail;
        }
        public string genNewEmail(string basic_givenname, string basic_sn, IDMUserType idm_user_type)
        {
            //check ถ้ามีชื่อกลาง---------------------------------------------------------
            //[ชื่อ].[ชื่อกลาง]@student.chula.ac.th
            //[ชื่อ].[นามสกุล]@student.chula.ac.th
            if (string.IsNullOrEmpty(basic_givenname) && string.IsNullOrEmpty(basic_sn))
                return "";

            string[] array_name = (basic_givenname.Replace("  ", " ")).Split(' ');
            if (array_name.Length > 1)
            {
                basic_givenname = array_name[0];
                basic_sn = array_name[1];
            }

            string email_domain = _conf.DefaultValue_emailDomain; /*"@Chula.ac.th" *using in basic_mail*;*/
            if (idm_user_type == IDMUserType.student)
                email_domain = _conf.DefaultValue_emailDomainForStudent;

            string newEmail = "";

            string name_eng = basic_givenname.Trim().ToLower().Replace(" ", "");
            string surname_eng = basic_sn.Trim().ToLower().Replace(" ", "");

            string[] name_eng_Split = name_eng.Split(' ');
            if (name_eng_Split.Length > 1)
                name_eng = name_eng_Split[0];

            name_eng = AppUtil.stringUpper(name_eng);
            surname_eng = AppUtil.stringUpper(surname_eng);

            if (!string.IsNullOrEmpty(basic_givenname) && !string.IsNullOrEmpty(basic_sn))
            {
                for (int i = 1; i <= surname_eng.Length; i++)
                {
                    var mail = name_eng + "." + surname_eng.Substring(0, i) + email_domain;
                    var fim_user = _context.table_visual_fim_user.Where(w => w.basic_mail.ToLower() == mail.ToLower()).FirstOrDefault();
                    if (fim_user == null)
                    {
                        /*check ldap?*/
                        newEmail = mail;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(newEmail))
                {
                    var runingNumber = 1;
                    while (string.IsNullOrEmpty(newEmail))
                    {
                        var mail = name_eng + "." + surname_eng + runingNumber.ToString() + email_domain;
                        var fim_user = _context.table_visual_fim_user.Where(w => w.basic_mail.ToLower() == mail.ToLower()).FirstOrDefault();
                        if (fim_user == null)
                        {
                            /*check ldap?*/
                            newEmail = mail;
                            break;
                        }
                        runingNumber++;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(surname_eng) && string.IsNullOrEmpty(surname_eng))
            {
                var runingNumber = 1;
                while (string.IsNullOrEmpty(newEmail))
                {
                    var mail = name_eng + "." + runingNumber.ToString() + email_domain;
                    var fim_user = _context.table_visual_fim_user.Where(w => w.basic_mail.ToLower() == mail.ToLower()).FirstOrDefault();
                    if (fim_user == null)
                    {
                        /*check ldap?*/
                        newEmail = mail;
                        break;
                    }
                    runingNumber++;
                }
            }

            return newEmail;
        }
        public string genUid(string name, string surname, IDMUserType idm_user_type, string basic_dn, string jobcode)
        {
            string basic_uid = "";
            name = name.Trim();
            surname = surname.Trim();


            if (idm_user_type == IDMUserType.temporary)
            {
                basic_uid = genUidForTemporaryAccount();
            }
            else if (idm_user_type == IDMUserType.affiliate)
            {
                basic_uid = genUidForStaff(name, surname);
            }
            else if (idm_user_type == IDMUserType.outsider)
            {
                basic_uid = genUidForStaff(name, surname);
            }
            else if (idm_user_type == IDMUserType.student)
            {
                basic_uid = genUidForStudent(basic_dn, jobcode);
            }
            else if (idm_user_type == IDMUserType.staff)
            {
                basic_uid = genUidForStaff(name, surname);
            }
            else
            {
                basic_uid = genUidForStaff(name.Trim(), surname.Trim());
            }

            return basic_uid.ToLower();
        }
        public string genUidForStaff(string name, string surname)
        {
            //check ถ้ามีชื่อกลาง---------------------------------------------------------
            //[ชื่อ].[ชื่อกลาง]@student.chula.ac.th
            //[ชื่อ].[นามสกุล]@student.chula.ac.th
            string[] array_name = (name.Replace("  ", " ")).Split(' ');
            if (array_name.Length > 1)
            {
                name = array_name[0];
                surname = array_name[1];
            }
            //-----------------------------------------------------------------------

            string new_username = "";
            int usernameLength = 8;
            if (surname.Length > 0)
            {
                if (name.Length > (usernameLength - 1))
                    new_username = surname.Substring(0, 1) + name.Substring(0, usernameLength - 1);
                else
                    new_username = surname.Substring(0, 1) + name;
            }
            else
            {
                if (name.Length > (usernameLength))
                    new_username = name.Substring(0, usernameLength);
                else
                    new_username = name;
            }
            bool haveUsernameIn_TableCuUnix = false;
            bool haveUsernameIn_TableVisualFim = false;
            bool haveUsernameIn_LDAP = false;

            //haveUsernameIn_TableCuUnix = int.Parse(DB.ExecuteQueryReturnFirstCell("SELECT count([username]) AS [have_username] FROM [table_cu_unix] WHERE [username] = '" + new_username + "';"));
            if (haveUsernameIn_TableCuUnix == false)
            {
                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == new_username.ToLower()).FirstOrDefault();
                if (fim_user != null)
                    haveUsernameIn_TableVisualFim = true;
            }

            //string[] attributeName = { new Config().ldapAuthenticationWith }; string[] attributeValue = { new_username }; bool Is_LIKE_mode = false;
            if (haveUsernameIn_TableCuUnix == false && haveUsernameIn_TableVisualFim == false)
            {

                var ldap = _providerldap.GetUser(new_username, _context);
                if (ldap != null)
                    haveUsernameIn_LDAP = true;
            }
            int counter_username = 0;
            while (haveUsernameIn_TableCuUnix == true | haveUsernameIn_TableVisualFim == true | haveUsernameIn_LDAP == true)
            {
                counter_username = (counter_username + 1);
                if (surname.Length > 0)
                {
                    if ((name + counter_username.ToString()).Length > (usernameLength - 1))
                        new_username = surname.Substring(0, 1) + name.Substring(0, usernameLength - (1 + counter_username.ToString().Length)) + counter_username.ToString();

                    else
                        new_username = surname.Substring(0, 1) + name + counter_username.ToString();

                }
                else
                {
                    if ((name + counter_username.ToString()).Length > (usernameLength))
                        new_username = name.Substring(0, usernameLength - (counter_username.ToString().Length)) + counter_username.ToString();
                    else
                        new_username = name + counter_username.ToString();

                }
                haveUsernameIn_TableCuUnix = false;
                haveUsernameIn_TableVisualFim = false;
                haveUsernameIn_LDAP = false;

                //haveUsernameIn_TableCuUnix = int.Parse(DB.ExecuteQueryReturnFirstCell("SELECT count([username]) AS [have_username] FROM [table_cu_unix] WHERE [username] = '" + new_username + "';"));
                if (haveUsernameIn_TableCuUnix == false)
                {
                    var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == new_username.ToLower()).FirstOrDefault();
                    if (fim_user != null)
                        haveUsernameIn_TableVisualFim = true;
                }

                //string[] attributeName = { new Config().ldapAuthenticationWith }; string[] attributeValue = { new_username }; bool Is_LIKE_mode = false;
                if (haveUsernameIn_TableCuUnix == false && haveUsernameIn_TableVisualFim == false)
                {

                    var ldap = _providerldap.GetUser(new_username, _context);
                    if (ldap != null)
                        haveUsernameIn_LDAP = true;
                }
            }
            return new_username.ToLower();
        }
        public string genUidForStudent(string basic_dn, string jobcode)
        {
            string basic_uid = "";


            if (string.IsNullOrEmpty(basic_uid))
            {
                if ((jobcode.Length > 0) && (System.Text.RegularExpressions.Regex.Match(jobcode[0].ToString(), "^[a-zA-Z]*$").Success))
                    basic_uid = jobcode.Trim();
                else
                    basic_uid = jobcode.Trim();
            }
            return basic_uid.ToLower();
        }
        public string genUidForTemporaryAccount()
        {
            var uid = "tmp" + RandomPassword(5).ToLower();
            bool dup = true;
            while (dup == true)
            {
                var fim_user = _context.table_visual_fim_user.Where(w => w.basic_uid.ToLower() == uid.ToLower()).FirstOrDefault();
                if (fim_user == null)
                {
                    var ldap = _providerldap.GetUser(uid, _context);
                    if (ldap == null)
                    {
                        dup = false;
                        return uid;
                    }
                    else
                        uid = "tmp" + RandomPassword(5);
                }
                else
                    uid = "tmp" + RandomPassword(5);
            }
            return uid;
        }
        public string getUnixNumber()
        {
            return getUnixNumber(1)[0];
        }
        public string[] getUnixNumber(int amount)
        {
            ArrayList ArrayListUnixNumber = new ArrayList();
            ArrayList ArrayListUnixNumberInDatabase = new ArrayList();

            var maxcuunix = _context.table_cu_unix.Select(s => s.uid_number).Max();
            var maxfim = _context.table_visual_fim_user.Select(s => Convert.ToInt64(s.unix_uidNumber)).Max();
            long startNumber = maxfim;

            if (maxcuunix > maxfim)
                startNumber = maxcuunix;
            startNumber += 1;
            for (var i = 0; i < amount; i++)
            {
                ArrayListUnixNumber.Add(startNumber.ToString());
                startNumber++;
            }
            return (string[])ArrayListUnixNumber.ToArray(typeof(string));
        }
        public void genNewAccount(SpuContext context, visual_fim_user model)
        {
            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return;

            var password = AppUtil.getNewPassword();
            var system_ou_lvl1 =  AppUtil.getOuName(model.system_ou_lvl1);
            var system_ou_lvl2 = AppUtil.getOuName(model.system_ou_lvl2);
            var system_ou_lvl3 = AppUtil.getOuName(model.system_ou_lvl3);

            var officeShotName = "";
            var firstValuePath = "";
            if (!string.IsNullOrEmpty(system_ou_lvl1))
            {
                firstValuePath += system_ou_lvl1;
            }
            if(system_ou_lvl1 == "internet")
            {
                officeShotName = system_ou_lvl1.ToUpper();
            }
            if (!string.IsNullOrEmpty(system_ou_lvl2))
            {
                officeShotName = AppUtil.getOuName(model.system_ou_lvl2, false).ToUpper();
                firstValuePath += "/" + system_ou_lvl2;
            }


            var unix_homeDirectory = "";
            if (!string.IsNullOrEmpty(firstValuePath))
            {
                unix_homeDirectory = _conf.DefaultValue_homeDirectory.Replace("[path]", firstValuePath);
            }

            model.system_idm_user_type = getIdmUserType(system_ou_lvl1);
            model.basic_displayname = model.basic_cn;
            model.basic_dn = "uid=[uid]";
            if (!string.IsNullOrEmpty(model.system_ou_lvl3))
                model.basic_dn += "," + model.system_ou_lvl3.ToLower();
            if (!string.IsNullOrEmpty(model.system_ou_lvl2))
                model.basic_dn += "," + model.system_ou_lvl2.ToLower();
            if (!string.IsNullOrEmpty(model.system_ou_lvl1))
                model.basic_dn += "," + model.system_ou_lvl1.ToLower();

            model.basic_cn = model.basic_cn;
            model.basic_displayname = model.basic_displayname;
            model.basic_dn += ",dc=chula,dc=ac,dc=th";
            model.basic_givenname = model.basic_givenname;
            model.basic_sn = model.basic_sn;
            model.basic_uid = genUid(model.basic_givenname, model.basic_sn, model.system_idm_user_type, model.basic_dn, model.cu_jobcode);
            model.basic_dn = model.basic_dn.Replace("[uid]", model.basic_uid);
            if (model.system_idm_user_type == IDMUserType.student)
                model.basic_mail = genNewEmailForStudent(model.cu_jobcode);
            else
                model.basic_mail = genNewEmail(model.basic_givenname, model.basic_sn, model.system_idm_user_type);
            model.basic_mobile = "";
            if(!string.IsNullOrEmpty(model.basic_telephonenumber))
                model.basic_telephonenumber =  model.basic_telephonenumber.Trim();
            model.basic_userPassword = Cryptography.encrypt(password);
            model.basic_userprincipalname = _conf.DefaultValue_userprincipalname.Replace("[uid]", model.basic_uid);
            if (model.cu_CUexpire_select == false & model.cu_CUexpire_day.HasValue & model.cu_CUexpire_month.HasValue & model.cu_CUexpire_year.HasValue)
                model.cu_CUexpire = model.cu_CUexpire_day + "-" + DateUtil.GetShortMonth(model.cu_CUexpire_month) + "-" + model.cu_CUexpire_year.ToString().Substring(2);
            model.cu_gecos = "";
            if (string.IsNullOrEmpty(model.basic_cn) == false)
            {
                model.cu_gecos = model.basic_cn;
                if (string.IsNullOrEmpty(officeShotName) == false)
                {
                    model.cu_gecos += ", " + officeShotName;
                    if (string.IsNullOrEmpty(model.basic_telephonenumber) == false)
                    {
                        model.cu_gecos += ", " + model.basic_telephonenumber;
                    }
                }
            }
            if (!string.IsNullOrEmpty(model.cu_jobcode))
                model.cu_jobcode = model.cu_jobcode.Trim();
            model.cu_mailacceptinggeneralid = model.basic_mail;
            model.cu_maildrop = _conf.DefaultValue_maildrop.Replace("[uid]", model.basic_uid);
            model.cu_mailhost = _conf.DefaultValue_mailhost;
            model.cu_mailRoutingAddress = _conf.DefaultValue_mailRoutingAddress.Replace("[uid]", model.basic_uid);
            model.cu_nsaccountlock = _conf.DefaultValue_nsaccountlock;
            model.cu_pplid = model.cu_pplid.Trim();
            model.cu_pwdchangedby = userlogin.basic_uid;
            model.cu_pwdchangeddate = DateUtil.Now();
            model.cu_pwdchangedloc = this.getClientIP();
            model.cu_sce_package = _conf.DefaultValue_SCE_Package;
            model.cu_thcn = model.cu_thcn.Trim();
            model.cu_thsn = model.cu_thsn.Trim();
            model.mail_miWmprefCharset = _conf.DefaultValue_miWmprefCharset;
            model.mail_miWmprefEmailAddress = model.basic_mail;
            model.mail_miWmprefFullName = model.basic_cn;
            model.mail_miWmprefReplyOption = _conf.DefaultValue_miWmprefReplyOption;
            model.mail_miWmprefTimezone = _conf.DefaultValue_miWmprefTimezone;
            model.system_answer1 = "";
            model.system_answer2 = "";
            model.system_answer3 = "";
            model.system_create_by_uid = userlogin.basic_uid;
            model.system_create_date = DateUtil.Now();
            model.system_last_accessed_by_uid = userlogin.basic_uid;
            model.system_last_accessed_date = DateUtil.Now();
            model.system_modify_by_uid = userlogin.basic_uid;
            model.system_modify_date = DateUtil.Now();
            if (model.system_sub_office_id.HasValue)
            {
                var org = context.table_cu_faculty_level2.Where(w => w.sub_office_id == model.system_sub_office_id).FirstOrDefault();
                if (org != null)
                    model.system_org = org.sub_office_name;
            }
            model.system_question1 = "";
            model.system_question2 = "";
            model.system_question3 = "";
            model.system_temporary_user_expire_date_counter = 0;
            model.system_waiting_time_for_access = 0;
            model.unix_gidNumber = ((int)model.system_idm_user_type).ToString();
            model.unix_homeDirectory = unix_homeDirectory.Replace("[uid]", model.basic_uid);

            string[] inetCOS = _conf.DefaultValue_inetCOS.Split('|');
            model.unix_inetCOS = inetCOS[0];
            model.unix_loginShell = _conf.DefaultValue_loginShell;
            model.unix_uidNumber = getUnixNumber();

            model.internetaccess = 1;
            model.netcastaccess = 1;
            if (model.system_idm_user_type == IDMUserType.staff)
            {
                model.cu_mailhost = "";
                model.unix_inetCOS = inetCOS[1];

            }
            else if (model.system_idm_user_type == IDMUserType.student)
            {
                model.cu_CUexpire = "";
                model.cu_maildrop = _conf.DefaultValue_maildropForStudent.Replace("[uid]", model.basic_uid);
                model.cu_mailhost = _conf.DefaultValue_mailhostForStudent.Replace("[uid]", model.basic_uid);
                model.cu_mailRoutingAddress = _conf.DefaultValue_mailRoutingAddressForStudent.Replace("[uid]", model.basic_uid);
                model.mail_miWmprefCharset = "";
                model.mail_miWmprefEmailAddress = "";
                model.mail_miWmprefFullName = "";
                model.mail_miWmprefReplyOption = "";
                model.mail_miWmprefTimezone = "";
                model.unix_inetCOS = _conf.DefaultValue_inetCOSForStudent;
                model.cu_nsaccountlock = _conf.DefaultValue_nsaccountlock;

            }
            else if (model.system_idm_user_type == IDMUserType.outsider)
            {
                model.cu_mailhost = "";
            }
            else if (model.system_idm_user_type == IDMUserType.affiliate)
            {

            }
            else if (model.system_idm_user_type == IDMUserType.temporary)
            {
                model.basic_cn = model.basic_uid;
                model.basic_displayname = model.basic_uid;
                model.basic_mail = "";
                model.basic_mobile = "";
                model.basic_sn = model.basic_uid;
                model.basic_telephonenumber = "";
                model.basic_userprincipalname = "";
                model.cu_gecos = "";
                model.cu_jobcode = "";
                model.cu_mailacceptinggeneralid = "";
                model.cu_maildrop = "";
                model.cu_mailhost = "";
                model.cu_mailRoutingAddress = "";
                model.cu_nsaccountlock = _conf.DefaultValue_nsaccountlockForTemporaryAccount;
                model.cu_pplid = "";
                model.cu_pwdchangedby = "";
                model.cu_pwdchangedloc = "";
                model.cu_sce_package = _conf.DefaultValue_SCE_Package;
                model.cu_thcn = "";
                model.cu_thsn = "";
                model.mail_miWmprefCharset = "";
                model.mail_miWmprefEmailAddress = "";
                model.mail_miWmprefFullName = "";
                model.mail_miWmprefReplyOption = "";
                model.mail_miWmprefTimezone = "";
                model.unix_homeDirectory = "";
                model.unix_inetCOS = "";
                model.unix_loginShell = "";
                model.basic_givenname = model.basic_uid;
            }
            if ((system_ou_lvl1 == "student" & system_ou_lvl2 == "cude" & system_ou_lvl3 == "people") | (system_ou_lvl1 == "student" & system_ou_lvl2 == "cuds" & system_ou_lvl3 == "people"))
            {
                model.basic_mail = "";
                model.cu_mailacceptinggeneralid = "";
                model.cu_maildrop = "";
                model.cu_mailhost = "";
                model.cu_mailRoutingAddress = "";
            }

            model.system_enable_password_forgot = 0;
            model.system_temporary_user_expire_date_counter = 0;
            model.system_waiting_time_for_access = 0;

            var unix = new cu_unix();
            unix.username = model.basic_uid;
            unix.userPassword = "*";
            unix.uid_number = NumUtil.ParseInteger(model.unix_uidNumber);
            unix.status_id = NumUtil.ParseInteger(model.unix_gidNumber);
            unix.full_name = model.basic_cn;
            unix.home_directory = model.unix_homeDirectory;
            unix.directory = model.unix_loginShell;
            unix.create_by_username = userlogin.basic_uid;
            unix.create_date = DateUtil.Now();
            unix.modify_by_username = userlogin.basic_uid;
            unix.create_date = DateUtil.Now();
            context.table_cu_unix.Add(unix);

            if(model.system_idm_user_type == IDMUserType.student)
            {
                var email = new cu_email_student();
                email.name_eng = model.basic_givenname;
                email.surname_eng = model.basic_sn;
                email.email = model.basic_mail;
                email.create_date = DateUtil.Now();
                context.table_cu_email_student.Add(email);
            }
            else
            {
                var email = new cu_email();
                email.name_eng = model.basic_givenname;
                email.surname_eng = model.basic_sn;
                email.email = model.basic_mail;
                email.create_date = DateUtil.Now();
                context.table_cu_email.Add(email);
            }
            if (model.system_idm_user_type == IDMUserType.temporary)
                model.unix_gidNumber = "";

            context.table_visual_fim_user.Add(model);

            if (model.unix_gidNumber == "302")
            {
                var receive = _context.table_receive_student.Where(w => w.login_name.ToLower() == model.basic_uid.ToLower()).FirstOrDefault();
                if (receive == null)
                {
                    receive = new receive_student();
                    receive.displayname = model.basic_displayname;
                    receive.login_name = model.basic_uid;
                    receive.password_initial = model.basic_userPassword;
                    receive.ticket = Cryptography.encrypt(getNewTicket());
                    receive.email_address = model.basic_mail;
                    receive.server_name = _conf.TableReceiveAccount_ServerName;
                    receive.expire = model.cu_CUexpire;
                    receive.status_id = model.unix_gidNumber;
                    receive.org = model.system_org;
                    receive.create_date = DateUtil.Now();
                    _context.table_receive_student.Add(receive);
                }
            }
            else if (model.unix_gidNumber == "305")
            {
                var receive = _context.table_receive_temp.Where(w => w.login_name.ToLower() == model.basic_uid.ToLower()).FirstOrDefault();
                if (receive == null)
                {
                    receive = new receive_temp();
                    receive.displayname = model.basic_displayname;
                    receive.login_name = model.basic_uid;
                    receive.password_initial = model.basic_userPassword;
                    receive.ticket = Cryptography.encrypt(getNewTicket());
                    receive.email_address = model.basic_mail;
                    receive.server_name = _conf.TableReceiveAccount_ServerName;
                    receive.expire = model.cu_CUexpire;
                    receive.status_id = model.unix_gidNumber;
                    receive.org = model.system_org;
                    receive.create_date = DateUtil.Now();
                    _context.table_receive_temp.Add(receive);
                }
            }
            else
            {
                var receive = _context.table_receive_staff.Where(w => w.login_name.ToLower() == model.basic_uid.ToLower()).FirstOrDefault();
                if (receive == null)
                {
                    receive = new receive_staff();
                    receive.displayname = model.basic_displayname;
                    receive.login_name = model.basic_uid;
                    receive.password_initial = model.basic_userPassword;
                    receive.ticket = Cryptography.encrypt(getNewTicket());
                    receive.email_address = model.basic_mail;
                    receive.server_name = _conf.TableReceiveAccount_ServerName;
                    receive.expire = model.cu_CUexpire;
                    receive.status_id = model.unix_gidNumber;
                    receive.org = model.system_org;
                    receive.create_date = DateUtil.Now();
                    _context.table_receive_staff.Add(receive);
                }
            }
            return;
        }

        public string writelog(LogType log_type_id, string log_status, IDMSource source, string uid, string log_description = "", string logonname = "", string log_exception = "")
        {
            if (string.IsNullOrEmpty(log_description))
            {
                if (log_type_id == LogType.log_create_account)
                    log_description = "สร้างบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_create_account_with_file)
                    log_description = "สร้างบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_edit_account)
                    log_description = "แก้ไขบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_delete_account)
                    log_description = "ลบบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_delete_account_with_file)
                    log_description = "ลบบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_change_password)
                    log_description = "เปลี่ยนรหัสผ่านบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_reset_password)
                    log_description = "เปลี่ยนรหัสผ่านบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_lock_account)
                    log_description = "ล็อกบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_unlock_account)
                    log_description = "ปลดล็อกบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_lock_account_with_file)
                    log_description = "ล็อกบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_unlock_account_with_file)
                    log_description = "ปลดล็อกบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_edit_internetaccess)
                    log_description = "แก้ไข Internet Access บัญชีผู้ใช้";
                else if (log_type_id == LogType.log_approve_reset_password)
                    log_description = "ขอเปลี่ยนรหัสผ่านบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_move_account)
                    log_description = "ย้ายกลุ่มของบัญชีรายชื่อผู้ใช้";
                else if (log_type_id == LogType.log_approved_reset_password)
                    log_description = "อนุมัติการขอเปลี่ยนรหัสผ่านบัญชีผู้ใช้";
                else if (log_type_id == LogType.log_reset_password_api)
                    log_description = "เปลี่ยนรหัสผ่านบัญชีผู้ใช้จาก API";

                log_description += " " + uid + " บน " + source.ToString();
                if (log_status == LogStatus.successfully)
                    log_description += " สำเร็จ";
                else
                    log_description += " ไม่สำเร็จ";
            }

            if (string.IsNullOrEmpty(logonname))
                logonname = this.HttpContext.User.Identity.Name;

            string controller = ControllerContext.ActionDescriptor.ControllerName;
            string action = ControllerContext.ActionDescriptor.ActionName;

            var log_target = Request.Scheme + "://" + Request.Host.Value + "/" + controller + "/" + action;
            var log_action = "";

            var datetime = DateUtil.Now();
            var curdate = datetime.Year + "_" + datetime.Month.ToString("00") + "_" + datetime.Day.ToString("00");
            var tablename = "table_system_log_" + curdate;
            if (logTableIsExist(tablename) == false)
            {
                if (logTableCreate(tablename) == false)
                    return "ไม่สามารถสร้าง log table ได้";
            }
            try
            {
                var sql = new StringBuilder();
                sql.AppendLine("INSERT INTO [DSM].[dbo].[" + tablename + "](");
                sql.AppendLine(" [log_username]");
                sql.AppendLine(" ,[log_ip]");
                sql.AppendLine(" ,[log_type_id]");
                sql.AppendLine(" ,[log_type]");
                sql.AppendLine(" ,[log_action]");
                sql.AppendLine(" ,[log_status]");
                sql.AppendLine(" ,[log_description]");
                sql.AppendLine(" ,[log_target]");
                sql.AppendLine(" ,[log_target_ip]");
                sql.AppendLine(" ,[log_datetime]");
                sql.AppendLine(" ,[log_exception])");
                sql.AppendLine(" VALUES (");
                sql.AppendLine(" '" + logonname + "'");
                sql.AppendLine(" ,'" + getClientIP() + "'");
                sql.AppendLine(" ," + (int)log_type_id);
                sql.AppendLine(" ,'" + log_type_id.ToString() + "'");
                sql.AppendLine(" ,'" + log_action + "'");
                sql.AppendLine(" ,'" + log_status + "'");
                sql.AppendLine(" ,'" + log_description + "'");
                sql.AppendLine(" ,'" + log_target + "'");
                sql.AppendLine(" ,'" + getHostIP() + "'");
                sql.AppendLine(" ,'" + DateUtil.ToDisplayDateTime(DateUtil.Now()) + "'");

                sql.AppendLine(" ,'" + log_exception + "'");
                sql.AppendLine(" )");
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql.ToString();
                    _context.Database.OpenConnection();
                    var result = command.ExecuteNonQuery();
                    _context.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {

            }
            return log_description;
        }


        public bool logTableIsExist(string tablename)
        {
            try
            {
                var object_id = "";
                var sql = "select object_id from sys.tables where name = '" + tablename + "'";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    _context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        // do something with result
                        while (result.Read())
                        {
                            object_id = result.GetValue(0).ToString();
                        }
                    }
                    _context.Database.CloseConnection();
                }
                var column_id = "";
                sql = "select column_id from sys.columns where name = 'log_exception' and object_id = '" + object_id + "'";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    _context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        // do something with result
                        while (result.Read())
                        {
                            column_id = result.GetValue(0).ToString();
                        }
                    }
                    _context.Database.CloseConnection();
                }
                if (string.IsNullOrEmpty(column_id))
                {
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "alter table " + tablename + " add [log_exception][nvarchar](max) NULL";
                        _context.Database.OpenConnection();
                        var result = command.ExecuteNonQuery();
                        _context.Database.CloseConnection();
                    }
                }
                if (!string.IsNullOrEmpty(object_id))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        public bool logTableCreate(string tablename)
        {
            try
            {
                var datetime = DateUtil.Now();
                var curdate = datetime.Year + "_" + datetime.Month.ToString("00") + "_" + datetime.Day.ToString("00");
                var sql = new StringBuilder();
                sql.AppendLine("CREATE TABLE[dbo].[" + tablename + "](");
                sql.AppendLine(" [log_id] [bigint] IDENTITY(1,1) NOT NULL,");
                sql.AppendLine(" [log_username][nvarchar](max) NULL,");
                sql.AppendLine(" [log_ip][nvarchar](max) NULL,");
                sql.AppendLine(" [log_type_id] [bigint] NULL,");
                sql.AppendLine(" [log_type][nvarchar](max) NULL,");
                sql.AppendLine(" [log_action][nvarchar](max) NULL,");
                sql.AppendLine(" [log_status][nvarchar](max) NULL,");
                sql.AppendLine(" [log_description][nvarchar](max) NULL,");
                sql.AppendLine(" [log_target][nvarchar](max) NULL,");
                sql.AppendLine(" [log_target_ip][nvarchar](max) NULL,");
                sql.AppendLine(" [log_datetime][nvarchar](max) NULL,");
                sql.AppendLine(" [log_exception][nvarchar](max) NULL");
                sql.AppendLine(") ON[PRIMARY]");
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql.ToString();
                    _context.Database.OpenConnection();
                    var result = command.ExecuteNonQuery();
                    _context.Database.CloseConnection();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CreateFolder(string filePath)
        {
            bool result = true;
            //filePath = HttpContext.Current.Server.MapPath(filePath);
            string[] filePathList = filePath.Split('\\');

            string[] pathList = new string[(filePathList.Length - 2)];
            for (int i = 0; i < filePathList.Length; i++)
            {
                if (i > 0)
                {
                    filePathList[i] = filePathList[i - 1] + "\\" + filePathList[i];
                    if (i < (filePathList.Length - 1)) { pathList[(i - 1)] = filePathList[i]; }
                }
            }

            foreach (string path in pathList)
            {
                System.IO.DirectoryInfo DirInfo = new System.IO.DirectoryInfo(path);

                //*** Create Folder ***// 
                if (!DirInfo.Exists)
                {
                    try
                    {
                        DirInfo.Create();
                        result = true;
                    }
                    catch (Exception) { result = false; }
                }
                else { result = true; }
            }

            return result;
        }

        public static bool writeTextToFile(string filename, string[] text)
        {
            bool result = false;
            //filePath = HttpContext.Current.Server.MapPath(filePath);
            try
            {
                //bool result_CreateFolder = CreateFolder(filePath);

                using (System.IO.StreamWriter StreamWriter1 = new System.IO.StreamWriter(filename, false, System.Text.Encoding.GetEncoding("windows-874")))
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        string substring = text[i].Substring(text[i].Length - 4);
                        string textLine = text[i].Substring(0, text[i].Length - 4) + text[i].Substring(text[i].Length - 4).Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
                        if (substring == "\r\n\r\n")
                        {
                            StreamWriter1.WriteLine(textLine.Trim());
                            if (i < (text.Length - 1)) { StreamWriter1.WriteLine(""); }
                        }
                        else
                        {
                            StreamWriter1.WriteLine(textLine.Trim());
                        }
                    }
                    StreamWriter1.Close();
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public static string getNewTicket()
        {
            int TicketLength = 8;
            string NewTicket = getNewTicket(TicketLength);
            return NewTicket;
        }
        public static string getNewTicket(int passwordLength)
        {
            string newPassword = "";
            //int passwordLength = 8;
            string[] numberSet = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string[] passwordSet = { "0", "9", "8", "7", "6", "5", "4", "3", "2", "1", };
            Random random = new Random();
            while (newPassword.Length < passwordLength)
            {
                int randomSet = random.Next(0, 100);
                if ((randomSet % 2) == 0) { newPassword += numberSet[(random.Next(0, (numberSet.Length - 1)))]; }
                else if ((randomSet % 2) == 1) { newPassword += passwordSet[(random.Next(0, (passwordSet.Length - 1)))]; }
            }
            return newPassword;
        }
    }
}
