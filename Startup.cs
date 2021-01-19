using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDM.DAL;
using IDM.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using IDM.Identity;

namespace IDM
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession();


            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SpuContext>(options => options.UseSqlServer(connectionString));
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<ILoginServices, LoginServices>();
            services.AddTransient<LoginServices>();

            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 100000; // 10000 items max
                options.ValueLengthLimit = 1024 * 1024 * 500; // 100MB max len form data
            });
            var portal = Configuration["SystemConf:Portal"];
            if (portal == Portal.admin)
            {
                services.AddMvc().AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/Auth/Login", "");
                });

                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                });


                services.ConfigureApplicationCookie(options =>
                {
                    options.AccessDeniedPath = "/Auth/Login";
                });
            }
            else
            {
                services.AddMvc().AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/Auth/LoginUser", "");
                });

                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
                {
                    options.LoginPath = "/Auth/LoginUser";
                    options.LogoutPath = "/Auth/Logout";
                });


                services.ConfigureApplicationCookie(options =>
                {
                    options.AccessDeniedPath = "/Auth/LoginUser";
                });
            }
            services.Configure<SystemConf>(Configuration.GetSection("SystemConf"));
            services.AddScoped<IUserProvider, AdUserProvider>();
            services.AddScoped<ILDAPUserProvider, LDAPUserProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseAdMiddleware();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<SpuContext>();
                context.Database.EnsureCreated();
                //context.Database.Migrate();
                context.EnsureSeedData();
            }

            string enUSCulture = "en-US";
            var supportedCultures = new[]
            {
                new CultureInfo("th-TH"),
                new CultureInfo("th"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(enUSCulture),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            var portal = Configuration["SystemConf:Portal"];
            if (portal == Portal.admin)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=auth}/{action=login}/{id?}");
                });
            }
            else
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=auth}/{action=loginuser}/{id?}");
                });
            }
           
        }
    }


    public class SystemConf
    {
        public string Portal { get; set; }
        public string DefaultValue_emailDomain { get; set; }
        public string DefaultValue_emailDomainForStudent { get; set; }
        public string DefaultValue_userprincipalname { get; set; }
        public string DefaultValue_mailhost { get; set; }
        public string DefaultValue_mailhostForStudent { get; set; }
        public string DefaultValue_mailRoutingAddress { get; set; }
        public string DefaultValue_mailRoutingAddressForStudent { get; set; }
        public string DefaultValue_maildrop { get; set; }
        public string DefaultValue_maildropForStudent { get; set; }
        public string DefaultValue_homeDirectory { get; set; }
        public string DefaultValue_loginShell { get; set; }
        public string DefaultValue_nsaccountlock { get; set; }
        public string DefaultValue_nsaccountlockForTemporaryAccount { get; set; }
        public string DefaultValue_nsaccountlockForOneDayAccount { get; set; }
        public string DefaultValue_miWmprefCharset { get; set; }
        public string DefaultValue_miWmprefReplyOption { get; set; }
        public string DefaultValue_miWmprefTimezone { get; set; }
        public string DefaultValue_inetCOS { get; set; }
        public string DefaultValue_inetCOSForStudent { get; set; }
        public string DefaultValue_inetCOSForTemporaryAccount { get; set; }
        public string DefaultValue_SCE_Package { get; set; }
        public string DefaultValue_OU_Filter { get; set; }

    }
}
