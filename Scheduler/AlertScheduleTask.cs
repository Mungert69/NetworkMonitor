
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
using NetworkMonitor.Data;

namespace ASPNETCoreScheduler.Scheduler
{
    public class AlertScheduleTask : ScheduledProcessor
    {
        private IMonitorPingService _monitorPingService;


        public AlertScheduleTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "* * * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Alert processing starts here");
            try
            {
                // Update schedule with string from appsettings.json
                _monitorPingService = serviceProvider.GetService<IMonitorPingService>();
                updateSchedule(_monitorPingService.PingParams.AlertSchedule);
                ResultObj result = _monitorPingService.Alert();
                Console.Write(result.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured in AlertScheduleTask.ProcesInScope() : " + e.Message);
            }
            Console.WriteLine("Alert processing ends here");
            return Task.CompletedTask;
        }


    }
}
