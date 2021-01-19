using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IDM.Models;
using Microsoft.EntityFrameworkCore;
using IDM.Services;
namespace IDM.DAL
{
    public static class SpuContextExtensions
    {
        public static void EnsureSeedData(this SpuContext context)
        {
            SeedMasterData(context);
            UpdateDatabaseDescriptions(context);
        }
        /*
         *    update Aumphurs SET
              ProvinceID = (select ProvinceID from Provinces where dbo.Aumphurs.ProvinceCode = dbo.Provinces.ProvinceCode);
  
              update Tumbons SET
              ProvinceID = (select ProvinceID from Provinces where dbo.Tumbons.ProvinceCode = dbo.Provinces.ProvinceCode)
  
              update Tumbons SET
              ProvinceID = (select ProvinceID from Provinces where dbo.Tumbons.ProvinceCode = dbo.Provinces.ProvinceCode),
              AumphurID =  (select AumphurID from Aumphurs where dbo.Tumbons.ProvinceCode = dbo.Aumphurs.ProvinceCode and dbo.Tumbons.AumphurCode = dbo.Aumphurs.AumphurCode)
  
         */
        public static void SeedMasterData(SpuContext context)
        {
            if (context.table_setup != null && !context.table_setup.Any())
            {
                var setup = new setup();
                setup.Host = "161.200.135.79";
                setup.Port = "389";
                setup.Base = "DC=win,DC=chula,DC=ac,DC=th";
                setup.Username = "service-fim@win.chula.ac.th";
                setup.Password = "9@fimpermit";

                setup.LDAPHost = "LDAP://161.200.135.46:389/";
                setup.LDAPPort = "389";
                setup.LDAPBase = "dc=chula,dc=ac,dc=th";
                setup.LDAPUsername = "uid=adm_fim,ou=People,dc=chula,dc=ac,dc=th";
                setup.LDAPPassword = "vppuvr4w";
                context.table_setup.Add(setup);
                context.SaveChanges();
                
            }
        }

        public static void UpdateDatabaseDescriptions(SpuContext context)
        {
            var contextType = typeof(SpuContext);
            var props = contextType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var prop in props)
            {
                if (prop.PropertyType.GetGenericArguments().Length == 0)
                    continue;

                var tableType = prop.PropertyType.GetGenericArguments()[0];

                string fullTableName = prop.Name;
                Regex regex = new Regex(@"(\[\w+\]\.)?\[(?<table>.*)\]");
                Match match = regex.Match(fullTableName);
                string tableName;
                if (match.Success)
                    tableName = match.Groups["table"].Value;
                else
                    tableName = fullTableName;

                foreach (var prop2 in tableType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    var attrs = prop2.GetCustomAttributes(typeof(DisplayAttribute), false);
                    if (attrs.Length > 0)
                    {
                        var columnName = prop2.Name;
                        var description = ((DisplayAttribute)attrs[0]).Name;
                        string strGetDesc = "select [value] from fn_listextendedproperty('MS_Description','schema','dbo','table',N'" + tableName + "','column',null) where objname = N'" + columnName + "';";
                        using (var command = context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = strGetDesc;
                            command.CommandType = CommandType.Text;

                            context.Database.OpenConnection();

                            object val = null;
                            using (var result = command.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    val = result;
                                }
                            }
                            if (val == null)
                            {
                                StringBuilder sql = new StringBuilder();
                                sql.Append(@"EXEC sp_addextendedproperty ");
                                sql.Append(" @name = N'MS_Description', @value = N'" + description + "',");
                                sql.Append(" @level0type = N'Schema', @level0name = 'dbo',");
                                sql.Append(" @level1type = N'Table',  @level1name = [" + tableName + "],");
                                sql.Append(" @level2type = N'Column', @level2name = [" + columnName + "];");

                                int result = context.Database.ExecuteSqlCommand(sql.ToString());
                            }
                            else
                            {
                                StringBuilder sql = new StringBuilder();
                                sql.Append(@"EXEC sp_updateextendedproperty ");
                                sql.Append(" @name = N'MS_Description', @value = N'" + description + "',");
                                sql.Append(" @level0type = N'Schema', @level0name = 'dbo',");
                                sql.Append(" @level1type = N'Table',  @level1name = [" + tableName + "],");
                                sql.Append(" @level2type = N'Column', @level2name = [" + columnName + "];");

                                int result = context.Database.ExecuteSqlCommand(sql.ToString());
                            }
                            context.Database.CloseConnection();
                        }

                    }
                }
            }
        }

    }
}
