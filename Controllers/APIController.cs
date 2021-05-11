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
    public class APIController : ControllerBase
    {
        private IUserProvider provider;

        public APIController(SpuContext context, ILogger<APIController> logger, ILoginServices loginServices, IUserProvider provider, ILDAPUserProvider providerldap, IOptions<SystemConf> conf) : base(context, logger, loginServices, conf, provider, providerldap)
        {
            this.provider = provider;
        }
        [HttpPost]
        public JsonResult ResetPwd([FromBody] ResetPwdDTO model)
        {

            return Json(new { responseCode = "200", responseDesc = "Reset password successfully." });
        }


        public class ResetPwdDTO
        {
            public string username { get; set; }
            public string password { get; set; }
            public string token { get; set; }
        }
    }
}
