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
using Microsoft.EntityFrameworkCore;
using IDM.DTO;
using IDM.Extensions;
using IDM.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace IDM.Controllers
{
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private IUserProvider provider;

        public ProfileController(SpuContext context, ILogger<ProfileController> logger, ILoginServices loginServices, IUserProvider provider, ILDAPUserProvider providerldap, IOptions<SystemConf> conf) : base(context, logger, loginServices, conf, provider, providerldap)
        {
            this.provider = provider;
        }
        public IActionResult Index()
        {
            var model = _context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if(model != null)
            {
                if (!string.IsNullOrEmpty(model.cu_pplid))
                {
                    if (model.cu_pplid.Length > 1)
                    {
                        var idcard = model.cu_pplid.Substring(0, 2);
                        for (var k = 2; k < 12; k++)
                        {
                            idcard += "x";
                        }
                        idcard += model.cu_pplid.Substring(model.cu_pplid.Length - 1, 1);
                        model.cu_pplid = idcard;
                    }
                }
            }    
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            var model = new ChangePasswordDTO();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            if (model.OldPassword == model.Password)
            {
                ModelState.AddModelError("OldPassword", "รหัสผ่านใหม่ต้องไม่ตรงกับรหัสผ่านเก่า กรุณาเปลี่ยนรหัสผ่านใหม่");
                ModelState.AddModelError("Password", "รหัสผ่านใหม่ต้องไม่ตรงกับรหัสผ่านเก่า กรุณาเปลี่ยนรหัสผ่านใหม่");
            }
            if (ModelState.IsValid)
            {
                var setup = _context.table_setup.FirstOrDefault();

                var aduser = await _provider.GetAdUser2(userlogin.basic_uid, _context);
                if (aduser == null)
                    return RedirectToAction("Logout", "Auth");

                if (_provider.ValidateCredentials(userlogin.basic_uid, model.OldPassword, _context).result == false)
                {
                    ModelState.AddModelError("OldPassword", "รหัสผ่านเดิมไม่ถูกต้อง");
                    return View(model);
                }

                ViewBag.Message = ReturnMessage.ChangePasswordFail;
                ViewBag.ReturnCode = ReturnCode.Error;

                if (setup.change_password_approve_enable)
                {
                    var reset = new reset_password_temp();
                    reset.username = userlogin.basic_uid;
                    reset.password = Cryptography.encrypt(model.Password);
                    reset.ip = getClientIP();
                    reset.target_ip = getHostIP();
                    reset.reset_by = userlogin.basic_uid;
                    reset.reset_date = DateUtil.Now();
                    _context.table_reset_password_temp.Add(reset);
                    _context.SaveChanges();

                    await MailRequestApproveResetPassword(userlogin);

                    writelog(LogType.log_approve_reset_password, LogStatus.successfully, IDMSource.VisualFim, userlogin.basic_uid);
                    ViewBag.Message = "รหัสผ่านใหม่มีผลเวลา 12.00 น. ของวันทำการถัดไป";
                    ViewBag.ReturnCode = ReturnCode.Success;
                }
                else
                {
                    userlogin.basic_userPassword = Cryptography.encrypt(model.Password);
                    userlogin.cu_pwdchangeddate = DateUtil.Now();
                    userlogin.cu_pwdchangedby = userlogin.basic_uid;
                    userlogin.cu_pwdchangedloc = getClientIP();

                    _context.SaveChanges();
                    var result_ldap = _providerldap.ChangePwd(userlogin, model.Password, _context);
                    if (result_ldap.result == true)
                        writelog(LogType.log_change_password, LogStatus.successfully, IDMSource.LDAP, userlogin.basic_uid);
                    else
                        writelog(LogType.log_change_password, LogStatus.failed, IDMSource.LDAP, userlogin.basic_uid, log_exception: result_ldap.Message);

                    var result_ad = _provider.ChangePwd(userlogin, model.Password, _context);
                    if (result_ad.result == true)
                        writelog(LogType.log_change_password, LogStatus.successfully, IDMSource.AD, userlogin.basic_uid);
                    else
                        writelog(LogType.log_change_password, LogStatus.failed, IDMSource.AD, userlogin.basic_uid, log_exception: result_ad.Message);

                    writelog(LogType.log_change_password, LogStatus.successfully, IDMSource.VisualFim, userlogin.basic_uid);

                    ViewBag.Message = ReturnMessage.ChangePasswordSuccess;
                    ViewBag.ReturnCode = ReturnCode.Success;
                }
            }
            
            return View(model);
        }
    }
}
