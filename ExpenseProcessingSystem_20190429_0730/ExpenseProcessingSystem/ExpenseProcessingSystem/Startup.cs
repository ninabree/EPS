﻿using System;
using DinkToPdf;
using DinkToPdf.Contracts;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            //EMAIL SINK TO SEND ERRORS TO CUSTOM EMAIL
            //.WriteTo.Email(new EmailConnectionInfo
            //    {
            //        FromEmail = "mizuho.eps@gmail.com", //temp, change to dynamic
            //        ToEmail = "mizuho.eps@gmail.com", //temp, change to dynamic -> login user's email
            //        MailServer = "smtp.gmail.com",
            //        NetworkCredentials = new NetworkCredential
            //        {
            //            UserName = "mizuho.eps@gmail.com", //temp, change to dynamic
            //            Password = "mizuhoeps2019" //temp, change to dynamic
            //        },
            //        EnableSsl = true,
            //        Port = 465,
            //        EmailSubject = "[EPS] Log Error"
            //},
            //    outputTemplate: outputTemplate,
            //    batchPostingLimit: 10
            //    , restrictedToMinimumLevel: LogEventLevel.Fatal
            //)
            .CreateLogger();

            Configuration = configuration;
            Setting.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            //Add DB context.
            services.AddDbContext<EPSDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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
        public static string ConnectionString { get; set; }
    }
}
