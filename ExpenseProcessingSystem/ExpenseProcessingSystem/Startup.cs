using System;
using System.Globalization;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.HostedServices;
using ExpenseProcessingSystem.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Rotativa.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace ExpenseProcessingSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //Serilog
            var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] [{EventId}] {Message:l}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .MinimumLevel.Error()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails()
            .Enrich.WithEnvironmentUserName()
            .WriteTo.Debug()
            .WriteTo.File("C:\\Work\\Mizuho EPS\\eps_logs\\logs\\log.txt", LogEventLevel.Error, outputTemplate, rollingInterval: RollingInterval.Day, fileSizeLimitBytes: null, rollOnFileSizeLimit: true)
            .CreateLogger();

            //-------------------------FOR RESOURCE FILE-------------------------


            //-------------------------FOR RESOURCE FILE-------------------------

            Configuration = configuration;
            Setting.DefaultConnectionString = Configuration.GetConnectionString("DefaultConnection");
            Setting.GwriteConnectionString = Configuration.GetConnectionString("GoWriteConnection");
            Setting.GOExpConnectionString = Configuration.GetConnectionString("GOExpressConnection");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //-------------------------FOR RESOURCE FILE-------------------------
            services.AddLocalization(o => o.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("en-GB"),
                    new CultureInfo("de-DE")
                };

                options.DefaultRequestCulture = new RequestCulture("en-US","en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            //-------------------------FOR RESOURCE FILE-------------------------

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false; // orig val: true
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(1000);
            });
            services.AddTransient<IEmailSender,EmailService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //Add DB context.
            services.AddDbContext<EPSDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<GOExpressContext>(options => options.UseSqlServer(Configuration.GetConnectionString("GOExpressConnection")));
            services.AddDbContext<GWriteContext>(options => options.UseSqlServer(Configuration.GetConnectionString("GoWriteConnection")));

            services.AddHostedService<HelloWorldHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Serilog
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });

            RotativaConfiguration.Setup(env);
        }
    }

    public static class Setting
    {
        public static string DefaultConnectionString { get; set; }
        public static string GwriteConnectionString { get; set; }
        public static string GOExpConnectionString { get; set; }
    }
}
