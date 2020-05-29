
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using NetworkMonitor.Utils;
using NetworkMonitor;
using NetworkMonitor.Objects;

namespace ASPNETCoreScheduler.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {
        private  IHostingEnvironment _hostingEnvironment;
        private IMonitorPingService _monitorPingService;


        public ScheduleTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "* * * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
 
            _hostingEnvironment = serviceProvider.GetService<IHostingEnvironment>();
           _monitorPingService= serviceProvider.GetService<IMonitorPingService>();
            Console.WriteLine("Processing starts here");
            foreach (MonitorPingInfo monitorPingInfo in _monitorPingService.MonitorPingInfos) {

                PingIt pingIt = new PingIt(monitorPingInfo);
                pingIt.go();
                Console.WriteLine("IP Address : " + monitorPingInfo.IPAddress);
                Console.WriteLine("Status : " + monitorPingInfo.Status);
                Console.WriteLine("Trip Time : " + monitorPingInfo.RoundTripTimeAverage);
            }
           
            Console.WriteLine("Processing ends here");
            return Task.CompletedTask;
        }

        
    }
}
