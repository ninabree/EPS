using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.HostedServices
{
public class HelloWorldHostedService : IHostedService
    {
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(HelloWorld, null, 0, 10000);
            return Task.CompletedTask;
        }

        void HelloWorld(object state)
        {
            File.AppendAllText("wwwroot/txtDebug/Textfile.txt", Environment.NewLine + DateTime.Now.ToString("MM/dd/yyyy") + " - " + DateTime.Now.ToString("HH:mm"));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //New Timer does not have a stop. 
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
