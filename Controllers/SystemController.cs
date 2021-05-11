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
    public class SystemController : ControllerBase
    {
        private IUserProvider provider;

        public SystemController(SpuContext context, ILogger<SystemController> logger, ILoginServices loginServices, IUserProvider provider, ILDAPUserProvider providerldap, IOptions<SystemConf> conf) : base(context, logger, loginServices, conf, provider, providerldap)
        {
            this.provider = provider;

        }
      
        //public IActionResult OU(SearchDTO model)
        //{
        //    var lists = this._context.OUs.OrderBy(o => o.OUID);
        //    model.lists = lists;

        //    ViewBag.Message = model.msg;
        //    ViewBag.ReturnCode = model.code;
        //    return View(model);
        //}
        //public IActionResult OUInfo(int? id)
        //{
        //    OU model = new OU();
        //    if (id.HasValue)
        //    {
        //        model = _context.OUs.Where(w => w.OUID == id).FirstOrDefault();
        //    }
        //    return View(model);
        //}

        //[HttpPost]
        //public IActionResult OUInfo(OU model)
        //{
        //    var user = this._context.Users.Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
        //    if (user == null)
        //        return RedirectToAction("Logout", "Auth");

        //    if (this.isExistOU(model))
        //    {
        //        ModelState.AddModelError("OUName", "OU ซ้ำในระบบ");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        ViewBag.Message = ReturnMessage.Error;
        //        ViewBag.ReturnCode = ReturnCode.Error;

        //        if (model.OUID > 0)
        //        {
        //            var ou = _context.OUs.Where(w => w.OUID == model.OUID).FirstOrDefault();
        //            if (ou != null)
        //            {
        //                ou.Update_On = DateUtil.Now();
        //                ou.Update_By = user.UserName;
        //                ou.OUName = model.OUName;
        //                ou.OUDescription = model.OUDescription;
        //                this._context.SaveChanges();
        //                return RedirectToAction("OU", "System", new { code = ReturnCode.Success, msg = ReturnMessage.Success });
        //            }
        //        }
        //        else
        //        {
        //            model.Create_On = DateUtil.Now();
        //            model.Create_By = user.UserName;
        //            model.Update_On = DateUtil.Now();
        //            model.Update_By = user.UserName;

        //            var i = 1;
        //            var roles = _context.Roles.OrderBy(o => o.Index);
        //            foreach (var r in roles)
        //            {
        //                r.Index = i;
        //                i++;
        //            }
        //            var role = new Role();
        //            role.RoleName = "ผู้ดูแลระบบ " + model.OUDescription;
        //            role.Index = i;
        //            role.OU = model;
        //            role.Create_On = DateUtil.Now();
        //            role.Create_By = user.UserName;
        //            role.Update_On = DateUtil.Now();
        //            role.Update_By = user.UserName;
        //            this._context.Roles.Add(role);
        //            this._context.OUs.Add(model);
        //            this._context.SaveChanges();

        //            //await provider.CreateOU(model.OUName, _context);
        //            return RedirectToAction("OU", "System", new { code = ReturnCode.Success, msg = ReturnMessage.Success });
        //        }
        //    }
        //    return View(model);
        //}
        //public IActionResult OUDel(int? id)
        //{
        //    var msg = ReturnMessage.Error;
        //    var code = ReturnCode.Error;
        //    if (id.HasValue)
        //    {
        //        var model = _context.OUs.Where(w => w.OUID == id).FirstOrDefault();
        //        if (model != null)
        //        {
        //            var guests = _context.Guests.Where(w => w.OUID == id);
        //            if (guests.Count() > 0)
        //            {
        //                msg = ReturnMessage.DataInUse;
        //                code = ReturnCode.Error;
        //                return RedirectToAction("OU", "System", new { code = code, msg = msg });
        //            }
        //            var guestimports = _context.GuestImports.Where(w => w.OUID == id);
        //            if (guestimports.Count() > 0)
        //            {
        //                _context.GuestImports.RemoveRange(guestimports);
        //            }
        //            var roles = _context.Roles.Where(w => w.OUID == model.OUID);
        //            foreach (var role in roles)
        //            {
        //                //var adminroles = _context.AdminRoles.Where(w => w.RoleID == role.ID);
        //                //if (adminroles.Count() > 0)
        //                //    _context.AdminRoles.RemoveRange(adminroles);

        //                //_context.Roles.Remove(role);
        //            }

        //            _context.OUs.Remove(model);
        //            _context.SaveChanges();
        //            msg = ReturnMessage.Success;
        //            code = ReturnCode.Success;
        //        }
        //    }
        //    return RedirectToAction("OU", "System", new { code = code, msg = msg });
        //}

       
        public IActionResult Setup()
        {
            if (!checkrole(new string[] { UserRole.admin }))
                return RedirectToAction("Logout", "Auth");

            var model = _context.table_setup.FirstOrDefault();
            if (model == null)
                model = new setup();
            return View(model);
        }
        [HttpPost]
        public IActionResult Setup(setup model)
        {
            if (!checkrole(new string[] { UserRole.admin}))
                return RedirectToAction("Logout", "Auth");

            var userlogin = this._context.table_visual_fim_user.Where(w => w.basic_uid == this.HttpContext.User.Identity.Name).FirstOrDefault();
            if (userlogin == null)
                return RedirectToAction("Logout", "Auth");

            if (ModelState.IsValid)
            {
                ViewBag.Message = ReturnMessage.Error;
                ViewBag.ReturnCode = ReturnCode.Error;
                if (model.ID > 0)
                {
                    var setup = _context.table_setup.Where(w => w.ID == model.ID).FirstOrDefault();
                    if(setup != null)
                    {
                        setup.Host = model.Host;
                        setup.Port = model.Port;
                        setup.Base = model.Base;
                        setup.Username = model.Username;
                        setup.Password = model.Password;
                        setup.SMTP_From = model.SMTP_From;
                        setup.SMTP_Password = model.SMTP_Password;
                        setup.SMTP_Port = model.SMTP_Port;
                        setup.SMTP_Server = model.SMTP_Server;
                        setup.SMTP_SSL = model.SMTP_SSL;
                        setup.SMTP_Username = model.SMTP_Username;
                        setup.change_password_approve_enable = model.change_password_approve_enable;
                        setup.change_password_otp_enable = model.change_password_otp_enable;
                        setup.first_page_description = model.first_page_description;
                        setup.Update_On = DateUtil.Now();
                        setup.Update_By = userlogin.basic_uid;
                        this._context.SaveChanges();
                    }
                }
                else
                {
                    model.Create_On = DateUtil.Now();
                    model.Create_By = userlogin.basic_uid;
                    model.Update_On = DateUtil.Now();
                    model.Update_By = userlogin.basic_uid;
                    this._context.table_setup.Add(model);
                    this._context.SaveChanges();
                }
                ViewBag.Message = ReturnMessage.Success;
                ViewBag.ReturnCode = ReturnCode.Success;
            }
            return View(model);
        }

        public IActionResult PasswordGenerator()
        {
            var model = new PasswordGenerateDTO();
            model.Length = 8;
            model.PasswordCnt = 1;
            model.Number = true;
            model.Lower = true;
            model.Passwords = new List<string>();
            return View(model);
        }
        [HttpPost]
        public IActionResult PasswordGenerator(PasswordGenerateDTO model)
        {
            model.Passwords = new List<string>();

            if (model.Number == false & model.Lower == false && model.Upper == false)
                ModelState.AddModelError("Condition", "กรุณาระบุเงือนไข");

            if (ModelState.IsValid)
            {
                for(var i=0;i< model.PasswordCnt; i++)
                {
                    var password = RandomPassword(model.Length, model.Number, model.Lower, model.Upper);
                    model.Passwords.Add(password);
                }
            }
            return View(model);
        }
        //public IActionResult Role(SearchDTO model)
        //{
        //    model.lists = this._context.Roles.Include(i => i.OU).OrderBy(o => o.Index);

        //    ViewBag.Message = model.msg;
        //    ViewBag.ReturnCode = model.code;
        //    return View(model);
        //}
        //public IActionResult RoleInfo(int? id)
        //{
        //    Role model = new Role();
        //    if (id.HasValue)
        //    {
        //        model = _context.Roles.Include(i => i.OU).Where(w => w.ID == id).FirstOrDefault();
        //        if (model != null)
        //        {
        //            model.SelectedAdmins = _context.AdminRoles.Where(w => w.RoleID == model.ID).Select(s => s.Admin);
        //            model.SelectedID = model.SelectedAdmins.Select(s => s.ID).ToArray();
        //        }
        //    }

        //    var UnSelectedAdmins = _context.table_visual_fim_user.Where(w => 1 == 1);
        //    if (model.SelectedID != null)
        //    {
        //        model.UnSelecteAdmins = UnSelectedAdmins.Where(w => !model.SelectedID.Contains(w.id));
        //        model.UnSelectedID = model.UnSelecteAdmins.Select(s => s.ID).ToArray();
        //    }
        //    return View(model);
        //}


        //[HttpPost]
        //public IActionResult RoleInfo(Role model)
        //{
        //    var user = this._context.Users.Where(w => w.UserName == this.HttpContext.User.Identity.Name).FirstOrDefault();
        //    if (user == null)
        //        return RedirectToAction("Logout", "Auth");

        //    if (ModelState.IsValid)
        //    {
        //        ViewBag.Message = ReturnMessage.Error;
        //        ViewBag.ReturnCode = ReturnCode.Error;

        //        if (model.ID > 0)
        //        {
        //            model.Update_On = DateUtil.Now();
        //            model.Update_By = user.UserName;
        //            model.OU = null;
        //            this._context.Update(model);

        //            var adminroles = _context.AdminRoles.Where(w => w.RoleID == model.ID);
        //            if (adminroles.Count() > 0)
        //                _context.AdminRoles.RemoveRange(adminroles);

        //            if (model.SelectedID != null)
        //            {
        //                foreach (var id in model.SelectedID)
        //                {
        //                    var adminrole = new AdminRole();
        //                    adminrole.AdminID = id;
        //                    adminrole.RoleID = model.ID;
        //                    _context.AdminRoles.Add(adminrole);
        //                }
        //            }
        //            this._context.SaveChanges();
        //            return RedirectToAction("Role", "System", new { code = ReturnCode.Success, msg = ReturnMessage.Success });
        //        }
        //        else
        //        {
        //            model.Create_On = DateUtil.Now();
        //            model.Create_By = user.UserName;
        //            model.Update_On = DateUtil.Now();
        //            model.Update_By = user.UserName;
        //            model.OU = null;
        //            if (model.SelectedID != null)
        //            {
        //                foreach (var id in model.SelectedID)
        //                {
        //                    var adminrole = new AdminRole();
        //                    adminrole.AdminID = id;
        //                    adminrole.Role = model;
        //                    _context.AdminRoles.Add(adminrole);
        //                }
        //            }
        //            this._context.Roles.Add(model);
        //            this._context.SaveChanges();
        //            return RedirectToAction("Role", "System", new { code = ReturnCode.Success, msg = ReturnMessage.Success });
        //        }
        //    }
        //    return View(model);
        //}


    }
}
