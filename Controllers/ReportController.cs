using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using System.Text;
using System.Globalization;

namespace IDM.Controllers
{
    [Authorize]
    public class ReportController : ControllerBase
    {

        public ReportController(SpuContext context, ILogger<ReportController> logger, ILoginServices loginServices, IUserProvider provider, ILDAPUserProvider providerldap, IOptions<SystemConf> conf) : base(context, logger, loginServices, conf, provider, providerldap)
        {
        }
        public IActionResult Log(SearchDTO model)
        {
            var lists = new List<system_log>();

            if (string.IsNullOrEmpty(model.dfrom))
                model.dfrom = DateUtil.ToDisplayDate(DateUtil.Now());
            if (string.IsNullOrEmpty(model.dto))
                model.dto = DateUtil.ToDisplayDate(DateUtil.Now());

            var dfrom = DateUtil.ToDate(model.dfrom);
            var dto = DateUtil.ToDate(model.dto);

            while (dfrom <= dto)
            {
                var datetime = dfrom.Value;
                var curdate = datetime.Year + "_" + datetime.Month.ToString("00") + "_" + datetime.Day.ToString("00");
                var tablename = "table_system_log_" + curdate;
                if (logTableIsExist(tablename))
                {
                    var sql = new StringBuilder();
                    sql.AppendLine("select [log_id],[log_username],[log_ip],[log_type_id],[log_type],[log_action],[log_status],[log_description],[log_target],[log_target_ip],[log_datetime],[log_exception] ");
                    sql.AppendLine(" from ");
                    sql.AppendLine(tablename);
                    sql.AppendLine(" where 1=1");
                    if (!string.IsNullOrEmpty(model.logstatus_search))
                    {
                        sql.AppendLine(" and log_status = '" + model.logstatus_search + "'" );
                    }
                    if (!string.IsNullOrEmpty(model.log_type_search))
                    {
                        sql.AppendLine(" and log_type = '" + model.log_type_search + "'");
                    }
                    if (!string.IsNullOrEmpty(model.text_search))
                    {
                        model.text_search = model.text_search.Trim().ToLower();
                        sql.AppendLine(" and (");
                        sql.AppendLine(" LOWER(log_username) like '%" + model.text_search + "%'");
                        sql.AppendLine(" or log_ip like '%" + model.text_search + "%'");
                        sql.AppendLine(" or LOWER(log_description) like '%" + model.text_search + "%'");
                        sql.AppendLine(" or log_target like '%" + model.text_search + "%'");
                        sql.AppendLine(" or log_target_ip like '%" + model.text_search + "%'");
                        sql.AppendLine(")");
                    }
                    sql.AppendLine(" order by log_datetime desc");

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = sql.ToString();
                        _context.Database.OpenConnection();
                        using (var result = command.ExecuteReader())
                        {
                            // do something with result
                            while (result.Read())
                            {
                                var j = 0;
                                var log = new system_log();
                                log.log_id = (long)AppUtil.ManageNull(result.GetValue(j)); j++;
                                log.log_username = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                log.log_ip = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                log.log_type_id = (long)AppUtil.ManageNull(result.GetValue(j)); j++;
                                log.log_type = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                log.log_action = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                log.log_status = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                log.log_description = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                log.log_target = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                log.log_target_ip = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                log.log_datetime = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                log.log_exception = AppUtil.ManageNull(result.GetValue(j)).ToString(); j++;
                                lists.Add(log);
                            }
                        }
                    }
                }
                dfrom = dfrom.Value.AddDays(1);
            }


            //var lists = this._context.AuditLogs.Where(w => 1 == 1);

            //if (!string.IsNullOrEmpty(model.text_search))
            //    lists = lists.Where(w => w.Create_By.Contains(model.text_search) | w.FirstName.Contains(model.text_search) | w.LastName.Contains(model.text_search));

            //if (!string.IsNullOrEmpty(model.logaction))
            //    lists = lists.Where(w => w.Action == model.logaction);

            //if (!string.IsNullOrEmpty(model.dfrom))
            //{
            //    var dfrom = DateUtil.ToDate(model.dfrom);
            //    lists = lists.Where(w => w.Create_On >= dfrom);
            //}
            //if (!string.IsNullOrEmpty(model.dto))
            //{
            //    var dto = DateUtil.ToDate(model.dto);
            //    lists = lists.Where(w => w.Create_On <= dto);
            //}
            //if (!string.IsNullOrEmpty(model.logaction))
            //{
            //    lists = lists.Where(w => w.Action == model.logaction);
            //}

            //lists = lists.OrderByDescending(o => o.Create_On);
            //int skipRows = (model.pageno - 1) * 100;
            //var itemcnt = lists.Count();
            //var pagelen = itemcnt / 100;
            //if (itemcnt % 100 > 0)
            //    pagelen += 1;

            //model.itemcnt = itemcnt;
            //model.pagelen = pagelen;
            ////model.lists = lists.Skip(skipRows).Take(_pagelen).AsQueryable();
            model.lists = lists.AsQueryable();

            return View(model);
        }
    }
}
