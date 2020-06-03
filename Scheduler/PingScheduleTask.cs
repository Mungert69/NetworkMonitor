
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
    public class PingScheduleTask : ScheduledProcessor
    {
        private IMonitorPingService _monitorPingService;


        public PingScheduleTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "* * * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Processing starts here");
            try {
                // Update schedule with string from appsettings.json
                _monitorPingService = serviceProvider.GetService<IMonitorPingService>();
                if (_monitorPingService.RequestInit) { _monitorPingService.init(); }
                updateSchedule(_monitorPingService.PingParams.Schedule);
                PingParams pingParams = _monitorPingService.PingParams;
                PingIt pingIt;


                for (int i = 0; i < pingParams.PingBurstNumber; i++)
                {
                    foreach (MonitorPingInfo monitorPingInfo in _monitorPingService.MonitorPingInfos)
                    {
                        pingIt = new PingIt(monitorPingInfo, pingParams);
                        pingIt.go();
                        Console.WriteLine("IP Address : " + monitorPingInfo.IPAddress);
                        Console.WriteLine("Status : " + monitorPingInfo.MonitorStatus);
                        Console.WriteLine("Trip Time : " + monitorPingInfo.RoundTripTimeAverage);
                    }
                    Thread.Sleep(pingParams.PingBurstDelay);
                }


            }
            catch (Exception e) {
                Console.WriteLine("Error occured in PingScheduleTask.ProcesInScope() : "+e.Message);
            }
            Console.WriteLine("Processing ends here");
            return Task.CompletedTask;
        }

        
    }
}
