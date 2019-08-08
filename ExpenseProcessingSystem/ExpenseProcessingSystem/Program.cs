using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ExpenseProcessingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
            //.ConfigureLogging((hostingContent, logging) =>
            //{
            //    logging.AddConfiguration(hostingContent.Configuration.GetSection("Logging"));
            //    logging.AddEventLog(new EventLogSettings()
            //    {
            //        SourceName = "ExpressLogger",
            //        LogName = "ExpressLog",
            //        Filter = ()
            //    });
            //})
            //.Build();
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel()
                .UseIISIntegration()
                .UseSerilog()
                .Build();
    }
}
