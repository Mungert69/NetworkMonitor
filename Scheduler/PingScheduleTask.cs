
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
using NetworkMonitor.Services;

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
            //Console.WriteLine("Processing starts here");
            try {
                // Update schedule with string from appsettings.json
                _monitorPingService = serviceProvider.GetService<IMonitorPingService>();              
                       
                updateSchedule(_monitorPingService.PingParams.Schedule);
                ResultObj result = _monitorPingService.Ping();
                Console.Write(result.Message);
                


            }
            catch (Exception e) {
                Console.WriteLine("Error occured in PingScheduleTask.ProcesInScope() : "+e.Message);
            }
            //Console.WriteLine("Processing ends here");
            return Task.CompletedTask;
        }

        
    }
}
