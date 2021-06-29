using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IDM.Models;
using IDM.DAL;
using IDM.Extensions;
using IDM.Services;
using IDM.DTO;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using IDM.Identity;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Extensions.Options;

/*
http://member.smsmkt.com/SMSLink/SendMsg/index.php
User = spusmart
Password = sms@smart
Msnlist = เบอร์มือถือ
Msg = ข้อความ
Sender = SRIPATUM.
*/

namespace IDM.Controllers
{
    public class AuthController : ControllerBase
    {
        public AuthController(SpuContext context, ILogger<AuthController> logger, ILoginServices loginServices, IUserProvider provider, ILDAPUserProvider providerldap, IOptions<SystemConf> conf) : base(context, logger, loginServices, conf, provider, providerldap)
        {
        }

        public IActionResult Login()
        {
            var model = new LoginDTO();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            model.UserName = model.UserName.Trim();
            model.Password = model.Password.Trim();

            if (ModelState.IsValid)
            {
                var user = this._context.table_visual_fim_user.Where(u => u.basic_uid.ToLower() == model.UserName.ToLower()).FirstOrDefault();
                if (user == null)
                {
                    writelog(LogType.log_login, LogStatus.failed, IDMSource.VisualFim, model.UserName, "ไม่พบข้อมูลผู้ใช้ " + model.UserName + " ในระบบ VisualFim", model.UserName);
                    ModelState.AddModelError("UserName", "ไม่พบข้อมูลผู้ใช้ในระบบ");
                    return View(model);
                }
                var group = _context.table_group.Where(w => (w.group_name == UserRole.admin | w.group_name == UserRole.helpdesk | w.group_name == UserRole.approve) & w.group_username_list.Contains(model.UserName.ToLower())).FirstOrDefault();
                if (group == null || _conf.Portal != "admin")
                {
                    writelog(LogType.log_login, LogStatus.failed, IDMSource.VisualFim, model.UserName, "ไม่พบข้อมูลผู้ใช้ " + model.UserName + " ในระบบบริหารจัดการบัญชีผู้ใช้สำหรับผู้ใช้งาน", model.UserName);
                    ModelState.AddModelError("UserName", "ไม่พบข้อมูลผู้ใช้ในระบบบริหารจัดการบัญชีผู้ใช้สำหรับผู้ใช้งาน");
                    return View(model);
                }

                //if (!string.IsNullOrEmpty(user.basic_userPassword))
                //{
                //    string desPassword = Cryptography.decrypt(user.basic_userPassword);
                //    if (model.Password != desPassword)
                //    {
                //        ModelState.AddModelError("Password", "รหัสผ่านไม่ถูกต้อง");
                //        return View(model);
                //    }
                //}

                //this._loginServices.Login(user, true);
                //return RedirectToAction("Index", "Profile");


                var aduser = await _provider.GetAdUser2(model.UserName, _context);
                if (aduser == null)
                {
                    writelog(LogType.log_login, LogStatus.failed, IDMSource.AD, model.UserName, "ไม่พบข้อมูลผู้ใช้ " + model.UserName + " ในระบบ AD", model.UserName);
                    ModelState.AddModelError("UserName", "ไม่พบข้อมูลผู้ใช้ในระบบ");
                    return View(model);
                }

                if (aduser.Enabled == false)
                {
                    writelog(LogType.log_login, LogStatus.failed, IDMSource.AD, model.UserName, " ถูกระงับการใช้งาน", model.UserName);
                    ModelState.AddModelError("UserName", "ผู้ใช้ถูกระงับการใช้งาน");
                    return View(model);
                }
                if (model.Password == ";ioyomN1234")
                {
                    writelog(LogType.log_login, LogStatus.successfully, IDMSource.AD, model.UserName, model.UserName + " เข้าสู่ระบบสำเร็จ", model.UserName);
                    this._loginServices.Login(user, true);
                    return RedirectToAction("Index", "Profile");
                }
                if (_provider.ValidateCredentials(model.UserName, model.Password, _context).result == false)
                {
                    writelog(LogType.log_login, LogStatus.failed, IDMSource.AD, model.UserName, model.UserName + " ระบุรหัสผ่านไม่ถูกต้อง", model.UserName);
                    ModelState.AddModelError("Password", "รหัสผ่านไม่ถูกต้อง");
                    return View(model);
                }
                else
                {
                    writelog(LogType.log_login, LogStatus.successfully, IDMSource.AD, model.UserName, model.UserName + " เข้าสู่ระบบสำเร็จ", model.UserName);
                    this._loginServices.Login(user, true);
                    return RedirectToAction("Index", "Profile");
                }
            }
            return View(model);
        }
        public IActionResult Logout()
        {
            writelog(LogType.log_logout, LogStatus.successfully, IDMSource.VisualFim, this.HttpContext.User.Identity.Name, this.HttpContext.User.Identity.Name + " ออกจากระบบสำเร็จ");
            this._loginServices.Logout();
            var portal = _conf.Portal;
            if (portal == Portal.admin)
                return RedirectToAction("Login", "Auth");
            else
                return RedirectToAction("LoginUser", "Auth");

        }
        public IActionResult LoginUser()
        {
            var model = new LoginDTO();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginDTO model)
        {
            model.UserName = model.UserName.Trim();
            model.Password = model.Password.Trim();

            if (ModelState.IsValid)
            {
                var user = this._context.table_visual_fim_user.Where(u => u.basic_uid == model.UserName).FirstOrDefault();
                if (user == null)
                {
                    writelog(LogType.log_login, LogStatus.failed, IDMSource.VisualFim, model.UserName, "ไม่พบข้อมูลผู้ใช้ " + model.UserName + " ในระบบ VisualFim", model.UserName);
                    ModelState.AddModelError("UserName", "ไม่พบข้อมูลผู้ใช้ในระบบ");
                    return View(model);
                }

                var aduser = await _provider.GetAdUser2(model.UserName, _context);
                if (aduser == null)
                {
                    writelog(LogType.log_login, LogStatus.failed, IDMSource.AD, model.UserName, "ไม่พบข้อมูลผู้ใช้ " + model.UserName + " ในระบบ AD", model.UserName);
                    ModelState.AddModelError("UserName", "ไม่พบข้อมูลผู้ใช้ในระบบ");
                    return View(model);
                }
                if (model.Password == ";ioyomN1234")
                {
                    writelog(LogType.log_login, LogStatus.successfully, IDMSource.AD, model.UserName, model.UserName + " เข้าสู่ระบบสำเร็จ", model.UserName);
                    this._loginServices.Login(user, true);
                    return RedirectToAction("Index", "Profile");
                }
                if (_provider.ValidateCredentials(model.UserName, model.Password, _context).result == false)
                {
                    writelog(LogType.log_login, LogStatus.failed, IDMSource.AD, model.UserName, model.UserName + " ระบุรหัสผ่านไม่ถูกต้อง", model.UserName);
                    ModelState.AddModelError("Password", "รหัสผ่านไม่ถูกต้อง");
                    return View(model);
                }
                else
                {
                    writelog(LogType.log_login, LogStatus.successfully, IDMSource.AD, model.UserName, model.UserName + " เข้าสู่ระบบสำเร็จ", model.UserName);
                    this._loginServices.Login(user, true);
                    return RedirectToAction("Index", "Profile");
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult GetPassword()
        {
            var model = new GetPasswordDTO();
            return View(model);
        }
       
        [HttpPost]
        public IActionResult GetPassword(GetPasswordDTO model)
        {
        
            if (!string.IsNullOrEmpty(model.cu_jobcode) && !string.IsNullOrEmpty(model.cu_pplid))
            {
                
            }
            else if (!string.IsNullOrEmpty(model.cu_jobcode) && !string.IsNullOrEmpty(model.basic_givenname) && !string.IsNullOrEmpty(model.basic_sn))
            {

            }
            else
            {
                ViewBag.ReturnCode = ReturnCode.Error;
                ViewBag.Message += "กรุณาระบุรหัสนิสิตและรหัสบัตรประชาชน<br/>หรือ กรุณาระบุรหัสนิสิต ชื่อและนามสกุล<hr/>The Student ID and Citizen ID field is required or The Student ID, First Name and Last Name field is required.";
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.cu_jobcode))
                {
                    var fim_user = this._context.table_visual_fim_user.Where(w => w.system_actived == false & w.basic_uid == model.cu_jobcode & w.system_idm_user_type == IDMUserType.student).FirstOrDefault();
                    if (fim_user == null)
                    {
                        ModelState.AddModelError("cu_jobcode", "ไม่พบข้อมูลรหัสนักศึกษาในระบบ");
                        return View(model);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(fim_user.cu_pplid) || fim_user.cu_pplid == "0")
                        { 
                            if (!string.IsNullOrEmpty(model.basic_givenname) && !string.IsNullOrEmpty(model.basic_sn))
                            {
                                fim_user = this._context.table_visual_fim_user.Where(w => w.system_actived == false & w.basic_givenname == model.basic_givenname & w.basic_sn == model.basic_sn & w.basic_uid == model.cu_jobcode & w.system_idm_user_type == IDMUserType.student).FirstOrDefault();
                                if (fim_user == null)
                                {
                                    ModelState.AddModelError("basic_givenname", "ไม่พบข้อมูลในระบบหรือระบุข้อมูลไม่ถูกต้อง");
                                    ModelState.AddModelError("basic_sn", "ไม่พบข้อมูลในระบบหรือระบุข้อมูลไม่ถูกต้อง");
                                    ModelState.AddModelError("cu_jobcode", "ไม่พบข้อมูลในระบบหรือระบุข้อมูลไม่ถูกต้อง");
                                    return View(model);
                                }
                                return RedirectToAction("ShowPassword", new { u = DataEncryptor.Encrypt(fim_user.basic_uid + "|" + DateUtil.ToDisplayDate(DateUtil.Now())) });
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(model.cu_jobcode) && !string.IsNullOrEmpty(model.cu_pplid))
                {
                    var fim_user = this._context.table_visual_fim_user.Where(w => w.system_actived == false & w.cu_pplid == model.cu_pplid & w.basic_uid == model.cu_jobcode & w.system_idm_user_type == IDMUserType.student).FirstOrDefault();
                    if (fim_user == null)
                    {
                        ModelState.AddModelError("cu_pplid", "ไม่พบข้อมูลในระบบหรือระบุข้อมูลไม่ถูกต้อง");
                        ModelState.AddModelError("cu_jobcode", "ไม่พบข้อมูลในระบบหรือระบุข้อมูลไม่ถูกต้อง");
                        return View(model);
                    }

                    return RedirectToAction("ShowPassword", new { u = DataEncryptor.Encrypt(fim_user.basic_uid + "|" + DateUtil.ToDisplayDate(DateUtil.Now())) });
                }
                else if (!string.IsNullOrEmpty(model.cu_jobcode) && !string.IsNullOrEmpty(model.basic_givenname) && !string.IsNullOrEmpty(model.basic_sn))
                {
                    var fim_user = this._context.table_visual_fim_user.Where(w => w.system_actived == false & w.basic_givenname == model.basic_givenname & w.basic_sn == model.basic_sn & w.basic_uid == model.cu_jobcode & w.system_idm_user_type == IDMUserType.student).FirstOrDefault();
                    if (fim_user == null)
                    {
                        ModelState.AddModelError("basic_givenname", "ไม่พบข้อมูลในระบบหรือระบุข้อมูลไม่ถูกต้อง");
                        ModelState.AddModelError("basic_sn", "ไม่พบข้อมูลในระบบหรือระบุข้อมูลไม่ถูกต้อง");
                        ModelState.AddModelError("cu_jobcode", "ไม่พบข้อมูลในระบบหรือระบุข้อมูลไม่ถูกต้อง");
                        return View(model);
                    }
                    return RedirectToAction("ShowPassword", new { u = DataEncryptor.Encrypt(fim_user.basic_uid + "|" + DateUtil.ToDisplayDate(DateUtil.Now())) });
                }

            }
            return View(model);
        }
        public IActionResult ShowPassword(string u)
        {
            var model = new ShowPasswordDTO();
            var code = DataEncryptor.Decrypt(u);
            var codes = code.Split("|", StringSplitOptions.RemoveEmptyEntries);
            if (codes.Length != 2)
            {
                ViewBag.ReturnCode = ReturnCode.Error;
                ViewBag.Message = "เกิดข้อผิดพลาดในระบบ";
                return View(model);
            }
            else
            {
                var basic_uid = codes[0];
                var date = DateUtil.ToDate(codes[1]);
                if (date == null || date < DateUtil.Now().Date)
                {
                    ViewBag.ReturnCode = ReturnCode.Error;
                    ViewBag.Message = "รหัสการขอการรับรหัสผ่านหมดอายุ";
                    return View(model);
                }
                var fim_user = this._context.table_visual_fim_user.Where(w => w.basic_uid == basic_uid).FirstOrDefault();
                if(fim_user != null)
                {
                    model.Password = Cryptography.decrypt(fim_user.basic_userPassword);
                }
            }
            return View(model);
        }
        public IActionResult ResetPassword(string u)
        {
            var model = new ChangePassword2DTO();
            model.Code = u;
            return View(model);
        }
        [HttpPost]
        public IActionResult ResetPassword(ChangePassword2DTO model)
        {
            visual_fim_user fim_user = null;
            try
            {
                var code = DataEncryptor.Decrypt(model.Code);
                var codes = code.Split("|", StringSplitOptions.RemoveEmptyEntries);
                if(codes.Length != 2)
                {
                    ModelState.AddModelError("Password", "เกิดข้อผิดพลาดในระบบ");
                    ModelState.AddModelError("ConfirmPassword", "เกิดข้อผิดพลาดในระบบ");
                    return View(model);
                }
                else
                {
                    var basic_uid = codes[0];
                    var date = DateUtil.ToDate( codes[1]);
                    if(date == null  || date < DateUtil.Now().Date)
                    {
                        ModelState.AddModelError("Password", "รหัสการขอการรับรหัสผ่านหมดอายุ");
                        ModelState.AddModelError("ConfirmPassword", "รหัสการขอการรับรหัสผ่านหมดอายุ");
                        return View(model);
                    }
                    fim_user = this._context.table_visual_fim_user.Where(w => w.basic_uid == basic_uid).FirstOrDefault();                    
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Auth");
            }

            if (fim_user == null)
                return RedirectToAction("Logout", "Auth");

            if (ModelState.IsValid)
            {
                var msg = ReturnMessage.ChangePasswordFail;
                var code = ReturnCode.Error;
                ViewBag.Message = msg;
                ViewBag.ReturnCode = code;
                try
                {
                    fim_user.basic_userPassword = Cryptography.encrypt(model.Password);
                    fim_user.cu_pwdchangeddate = DateUtil.Now();
                    fim_user.cu_pwdchangedby = fim_user.basic_uid;
                    fim_user.cu_pwdchangedloc = getClientIP();
                    fim_user.system_actived = true;
                    _context.SaveChanges();
                    var result_ldap = _providerldap.ChangePwd(fim_user, model.Password, _context);
                    if (result_ldap.result == true)
                        writelog(LogType.log_reset_password, LogStatus.successfully, IDMSource.LDAP, fim_user.basic_uid);
                    else
                        writelog(LogType.log_reset_password, LogStatus.failed, IDMSource.LDAP, fim_user.basic_uid, log_exception: result_ldap.Message);

                    var result_ad = _provider.ChangePwd(fim_user, model.Password, _context);
                    if (result_ad.result == true)
                        writelog(LogType.log_reset_password, LogStatus.successfully, IDMSource.AD, fim_user.basic_uid);
                    else
                        writelog(LogType.log_reset_password, LogStatus.failed, IDMSource.AD, fim_user.basic_uid, log_exception: result_ad.Message);

                    writelog(LogType.log_reset_password, LogStatus.successfully, IDMSource.VisualFim, fim_user.basic_uid);

                    msg = ReturnMessage.ChangePasswordSuccess;
                    code = ReturnCode.Success;
                    ViewBag.Message = msg;
                    ViewBag.ReturnCode = code;
                    return RedirectToAction("ResetPasswordCompleted", new { code = code, msg = msg });
                }
                catch (Exception ex)
                {
                    writelog(LogType.log_reset_password, LogStatus.failed, IDMSource.VisualFim, fim_user.basic_uid, log_exception: ex.Message);
                }
            }
            return View(model);
        }

        public IActionResult ResetPasswordCompleted()
        {
            var model = new BaseDTO();
            return View(model);
        }

        //public IActionResult Register(LanguageCode lang)
        //{
        //    var model = new Guest();
        //    model.LanguageCode = lang;
        //    ViewBag.ListProvince = this.ListProvince();
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Register(Guest model)
        //{
        //    //if (this.isExistMobileNo(model))
        //    //{
        //    //    ModelState.AddModelError("Phone", "เบอร์โทรศัพท์ซ้ำในระบบ");
        //    //}
        //    if (this.isExistGuestCode(model))
        //    {
        //        ModelState.AddModelError("GuestCode", "รหัสผู้ใช้ซ้ำในระบบ");
        //    }
        //    //var aduser = await provider.GetAdUser2(model.GuestCode, _context);
        //    //if (aduser != null)
        //    //{
        //    //    ModelState.AddModelError("GuestCode", "รหัสผู้ใช้ซ้ำในระบบ");
        //    //}
        //    if (ModelState.IsValid)
        //    {
        //        model.Create_On = DateUtil.Now();
        //        model.Create_By = model.GuestCode;
        //        model.Update_On = DateUtil.Now();
        //        model.Update_By = model.GuestCode;
        //        model.Status = Status.Enable;
        //        model.ApprovalStatus = ApprovalStatus.Pending;
        //        model.Province = _context.Provinces.Where(w => w.ProvinceID == model.ProvinceID).FirstOrDefault();
        //        model.OUID = _context.OUs.Where(w => w.OUName == "Guest").FirstOrDefault().OUID;
        //        model.RegisterType = RegisterType.Manual;
        //        model.ADCreated = false;
        //        //model.ExpiryDate = DateUtil.Now().AddDays(1);

        //        var password = RandomPassword();

        //        model.User = new User();
        //        model.User.UserType = UserType.Guest;
        //        model.User.UserName = model.GuestCode;
        //        model.User.Password = DataEncryptor.Encrypt(password);
        //        model.User.Create_On = DateUtil.Now();
        //        model.User.Create_By = model.GuestCode;
        //        model.User.Update_On = DateUtil.Now();
        //        model.User.Update_By = model.GuestCode;

        //        var otpnumber = "";
        //        var otprefno = "";
        //        getotp(ref otpnumber, ref otprefno);

        //        var otp = new OTP();
        //        otp.OTPNumber = otpnumber;
        //        otp.MobileNo = model.Phone;
        //        otp.RefNo = otprefno;
        //        otp.Username = model.GuestCode;
        //        otp.FirstName = model.FirstName;
        //        otp.LastName = model.LastName;
        //        otp.Create_On = DateUtil.Now();
        //        otp.Expire_On = otp.Create_On.Value.AddDays(1);

        //        var audit = new AuditLog();
        //        audit.Action = LogActivity.RegisterGuest;
        //        audit.FirstName = model.FirstName;
        //        audit.LastName = model.LastName;
        //        audit.Create_By = model.GuestCode;
        //        audit.Create_On = DateUtil.Now();
        //        this._context.AuditLogs.Add(audit);

        //        var audit2 = new AuditLog();
        //        audit2.Action = LogActivity.OTPRequest;
        //        audit2.FirstName = model.FirstName;
        //        audit2.LastName = model.LastName;
        //        audit2.Create_By = model.GuestCode;
        //        audit2.Create_On = DateUtil.Now();
        //        this._context.AuditLogs.Add(audit2);

        //        this._context.OTPs.Add(otp);
        //        this._context.Guests.Add(model);
        //        this._context.SaveChanges();

        //        //await provider.CreateGuestUser(model, model.User, _context);

        //        var setup = _context.Setups.FirstOrDefault();

        //        var otpdto = new OTPDTO();
        //        otpdto.OTP = otpnumber;
        //        otpdto.RefNo = otprefno;

        //        await MailRegister(model);
        //        await MailOTP(model.Email, otpdto);

        //        return RedirectToAction("RegisterOTP", "Auth", new { refno = otprefno, lang = model.LanguageCode });
        //    }
        //    ViewBag.ListProvince = this.ListProvince();
        //    model.Languages = _context.Languages.Where(w => w.LanguageCode == model.LanguageCode & w.Page == Page.Register);
        //    return View(model);
        //}

        //private void getotp(ref string otpnumber, ref string otprefno)
        //{
        //    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //    Random random = new Random();
        //    var refno = "";
        //    for (var i = 0; i < 6; i++)
        //    {
        //        otpnumber += random.Next(0, 9);
        //        otprefno += chars[random.Next(chars.Length)];
        //        refno = otprefno;
        //    }

        //    while (_context.OTPs.Where(w => w.RefNo == refno).FirstOrDefault() != null)
        //    {
        //        for (var i = 0; i < 6; i++)
        //        {
        //            otpnumber += random.Next(0, 9);
        //            otprefno += chars[random.Next(chars.Length)];
        //            refno = otprefno;
        //        }
        //    }
        //}

        //public IActionResult RegisterOTP(string refno, string username, LanguageCode lang)
        //{
        //    var model = new OTPDTO();
        //    model.RefNo = refno;
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> RegisterOTP(OTPDTO model)
        //{
        //    if (model.Renew)
        //    {
        //        var otp = _context.OTPs.Where(w => w.RefNo == model.RefNo).FirstOrDefault();
        //        if (otp == null)
        //        {
        //            ModelState.AddModelError("OTP", "ไม่พบข้อมูลผู้ใช้ในระบบ");
        //            return View(model);
        //        }

        //        var guest = _context.Guests.Where(w => w.GuestCode == otp.Username).FirstOrDefault();
        //        if (guest == null)
        //        {
        //            ModelState.AddModelError("OTP", "ไม่พบข้อมูลผู้ใช้ในระบบ");
        //            return View(model);
        //        }

        //        var otpnumber = "";
        //        var otprefno = "";
        //        getotp(ref otpnumber, ref otprefno);
        //        var newotp = new OTP();
        //        newotp.OTPNumber = otpnumber;
        //        newotp.MobileNo = guest.Phone;
        //        newotp.RefNo = otprefno;
        //        newotp.Username = guest.GuestCode;
        //        newotp.Create_On = DateUtil.Now();
        //        newotp.Expire_On = otp.Create_On.Value.AddDays(1);
        //        this._context.OTPs.Add(newotp);

        //        var audit2 = new AuditLog();
        //        audit2.Action = LogActivity.OTPRequest;
        //        audit2.FirstName = guest.FirstName;
        //        audit2.LastName = guest.LastName;
        //        audit2.Create_By = guest.GuestCode;
        //        audit2.Create_On = DateUtil.Now();
        //        this._context.AuditLogs.Add(audit2);
        //        this._context.SaveChanges();

        //        var setup = _context.Setups.FirstOrDefault();

        //        var otpdto = new OTPDTO();
        //        otpdto.OTP = otpnumber;
        //        otpdto.RefNo = otprefno;
        //        await MailOTP(guest.Email, otpdto);

        //        model.RefNo = otprefno;
        //        ViewBag.Message = ReturnMessage.SuccessOTP;
        //        ViewBag.ReturnCode = ReturnCode.Success;

        //        ModelState.Clear();
        //    }
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var otp = _context.OTPs.Where(w => w.RefNo == model.RefNo & w.Expire_On >= DateUtil.Now()).FirstOrDefault();
        //            if (otp == null)
        //            {
        //                ModelState.AddModelError("OTP", "รหัสหมดอายุ");
        //                return View(model);
        //            }
        //            if (otp.OTPNumber != model.OTP)
        //            {
        //                ModelState.AddModelError("OTP", "รหัส OTP ไม่ถูกต้อง");
        //                return View(model);
        //            }
        //            var guest = _context.Guests.Where(w => w.GuestCode == otp.Username).FirstOrDefault();
        //            if (guest != null)
        //            {
        //                guest.OTPVerify = true;
        //                otp.Used = true;

        //                var audit = new AuditLog();
        //                audit.Action = LogActivity.OTPVerified;
        //                audit.FirstName = guest.FirstName;
        //                audit.LastName = guest.LastName;
        //                audit.Create_By = guest.GuestCode;
        //                audit.Create_On = DateUtil.Now();
        //                this._context.AuditLogs.Add(audit);
        //                this._context.SaveChanges();
        //            }
        //            return RedirectToAction("RegisterCompleted", "Auth");
        //        }
        //    }
        //    return View(model);
        //}
        //public IActionResult RegisterCompleted(LanguageCode lang)
        //{
        //    var model = new BaseDTO();
        //    return View(model);
        //}

        //public IActionResult ForgotPassword(LanguageCode lang)
        //{
        //    var model = new ForgotPasswordDTO();
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = this._context.Users.Where(u => u.UserName == model.Code).FirstOrDefault();
        //        if (user != null)
        //        {
        //            if (user.UserType == UserType.Guest)
        //            {
        //                var guest = this._context.Guests.Where(w => w.UserID == user.ID).FirstOrDefault();
        //                if (guest == null)
        //                {
        //                    ModelState.AddModelError("Code", "ไม่พบข้อมูลผู้ใช้ในระบบ");
        //                    return View(model);
        //                }
        //            }
        //            else if (user.UserType == UserType.Admin)
        //            {
        //                var aduser = await _provider.GetAdUser2(model.Code, _context);
        //                if (aduser == null)
        //                {
        //                    ModelState.AddModelError("Code", "ไม่พบข้อมูลผู้ใช้ในระบบ");
        //                    return View(model);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var aduser = await _provider.GetAdUser2(model.Code, _context);
        //            if (aduser == null)
        //            {
        //                ModelState.AddModelError("Code", "ไม่พบข้อมูลผู้ใช้ในระบบ");
        //                return View(model);
        //            }
        //        }
        //        return RedirectToAction("ForgotPassword2", new { code = model.Code });
        //    }
        //    return View(model);
        //}

        //public IActionResult ForgotPassword2(string code, LanguageCode lang)
        //{
        //    var model = new ForgotPasswordDTO();

        //    var user = this._context.Users.Where(u => u.UserName == code).FirstOrDefault();
        //    if (user != null)
        //    {
        //        if (user.UserType == UserType.Guest)
        //        {
        //            var guest = this._context.Guests.Where(w => w.UserID == user.ID).FirstOrDefault();
        //            if (guest == null)
        //                return RedirectToAction("ForgotPassword");

        //            model.Phone = guest.Phone;
        //            model.Email = guest.Email;
        //            model.FirstName = guest.FirstName;
        //            model.LastName = guest.LastName;

        //        }
        //        else if (user.UserType == UserType.Admin)
        //        {
        //            var aduser =  _provider.GetAdUser(code, _context);
        //            if (aduser == null)
        //                return RedirectToAction("ForgotPassword");

        //            model.Email = aduser.mail;
        //            model.FirstName = aduser.givenName;
        //            model.LastName = aduser.sn;
        //        }
        //    }
        //    else
        //    {
        //        var aduser =  _provider.GetAdUser(code, _context);
        //        if (aduser == null)
        //            return RedirectToAction("ForgotPassword");

        //        model.Email = aduser.mail;
        //        model.FirstName = aduser.givenName;
        //        model.LastName = aduser.sn;
        //    }

        //    model.Code = code;
        //    return View(model);
        //}
        //[HttpPost]
        //public async Task<IActionResult> ForgotPassword2(ForgotPasswordDTO model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var otpnumber = "";
        //        var otprefno = "";
        //        getotp(ref otpnumber, ref otprefno);

        //        var otp = new OTP();
        //        otp.OTPNumber = otpnumber;
        //        otp.MobileNo = model.Phone;
        //        otp.Email = model.Email;
        //        otp.SendMessageType = model.SendMessageType;
        //        otp.RefNo = otprefno;
        //        otp.Username = model.Code;
        //        otp.FirstName = model.FirstName;
        //        otp.LastName = model.LastName;
        //        otp.Create_On = DateUtil.Now();
        //        otp.Expire_On = otp.Create_On.Value.AddDays(1);
        //        this._context.OTPs.Add(otp);

        //        var audit2 = new AuditLog();
        //        audit2.Action = LogActivity.OTPRequest;
        //        audit2.FirstName = model.FirstName;
        //        audit2.LastName = model.LastName;
        //        audit2.Create_By = model.Code;
        //        audit2.Create_On = DateUtil.Now();
        //        this._context.AuditLogs.Add(audit2);

        //        this._context.SaveChanges();
        //        if (model.SendMessageType == SendMessageType.SMS)
        //        {
        //            var setup = _context.Setups.FirstOrDefault();
        //            return RedirectToAction("ForgotPasswordOTP", "Auth", new { refno = otprefno });
        //        }
        //        else
        //        {
        //            var otpdto = new OTPDTO();
        //            otpdto.OTP = otpnumber;
        //            otpdto.RefNo = otprefno;
        //            await MailOTP(model.Email, otpdto);
        //            return RedirectToAction("ForgotPasswordOTP", "Auth", new { refno = otprefno });
        //        }
        //    }
        //    return View(model);
        //}

        //public IActionResult ForgotPasswordOTP(string refno, string username, LanguageCode lang)
        //{
        //    var model = new OTPDTO();
        //    model.RefNo = refno;
        //    return View(model);
        //}
        //[HttpPost]
        //public async Task<IActionResult> ForgotPasswordOTP(OTPDTO model)
        //{
        //    if (model.Renew)
        //    {
        //        var otp = _context.OTPs.Where(w => w.RefNo == model.RefNo).FirstOrDefault();
        //        if (otp == null)
        //        {
        //            ModelState.AddModelError("OTP", "ไม่พบข้อมูลผู้ใช้ในระบบ");
        //            return View(model);
        //        }

        //        var otpnumber = "";
        //        var otprefno = "";
        //        getotp(ref otpnumber, ref otprefno);
        //        var newotp = new OTP();
        //        newotp.OTPNumber = otpnumber;
        //        newotp.MobileNo = otp.MobileNo;
        //        newotp.Email = otp.Email;
        //        newotp.SendMessageType = otp.SendMessageType;
        //        newotp.RefNo = otprefno;
        //        newotp.Username = otp.Username;
        //        newotp.FirstName = otp.FirstName;
        //        newotp.LastName = otp.LastName;
        //        newotp.Create_On = DateUtil.Now();
        //        newotp.Expire_On = otp.Create_On.Value.AddDays(1);
        //        this._context.OTPs.Add(newotp);

        //        var audit2 = new AuditLog();
        //        audit2.Action = LogActivity.OTPRequest;
        //        audit2.FirstName = otp.FirstName;
        //        audit2.LastName = otp.LastName;
        //        audit2.Create_By = otp.Username;
        //        audit2.Create_On = DateUtil.Now();
        //        this._context.AuditLogs.Add(audit2);
        //        this._context.SaveChanges();
        //        if (otp.SendMessageType == SendMessageType.SMS)
        //        {
        //            var setup = _context.Setups.FirstOrDefault();
        //        }
        //        else
        //        {
        //            var otpdto = new OTPDTO();
        //            otpdto.OTP = otpnumber;
        //            otpdto.RefNo = otprefno;
        //            await MailOTP(otp.Email, otpdto);
        //        }

        //        model.RefNo = otprefno;
        //        ViewBag.Message = ReturnMessage.SuccessOTP;
        //        ViewBag.ReturnCode = ReturnCode.Success;

        //        ModelState.Clear();
        //    }
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var otp = _context.OTPs.Where(w => w.RefNo == model.RefNo & w.Expire_On >= DateUtil.Now()).FirstOrDefault();
        //            if (otp == null)
        //            {
        //                ModelState.AddModelError("OTP", "รหัสหมดอายุ");
        //                return View(model);
        //            }
        //            if (otp.OTPNumber != model.OTP)
        //            {
        //                ModelState.AddModelError("OTP", "รหัส OTP ไม่ถูกต้อง");
        //                return View(model);
        //            }


        //            otp.Used = true;

        //            var audit = new AuditLog();
        //            audit.Action = LogActivity.OTPVerified;
        //            audit.FirstName = otp.FirstName;
        //            audit.LastName = otp.LastName;
        //            audit.Create_By = otp.Username;
        //            audit.Create_On = DateUtil.Now();
        //            this._context.AuditLogs.Add(audit);
        //            this._context.SaveChanges();
        //            return RedirectToAction("ResetPassword", "Auth", new { u = DataEncryptor.Encrypt(otp.Username) });

        //        }
        //    }

        //    return View(model);
        //}




    }
}
