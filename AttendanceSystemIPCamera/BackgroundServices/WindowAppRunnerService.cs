using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.BackgroundServices
{
    public class WindowAppRunnerService : BackgroundService
    {
        public WindowAppRunnerService()
        {
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Thread myThread = new Thread(() =>
            {
                //var application = new AttendeeWPF.App();
                //application.Run(new AttendeeWPF.MainWindow());  // add Window if you want a window.
            });
            myThread.SetApartmentState(ApartmentState.STA);
            myThread.Start();
            await Task.CompletedTask;
        }
    }
}
