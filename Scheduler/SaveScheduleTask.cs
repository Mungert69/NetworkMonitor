﻿
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
    public class SaveScheduleTask : ScheduledProcessor
    {
        private IMonitorPingService _monitorPingService;


        public SaveScheduleTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "0 */2 * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Saving data processing starts here");
            try
            {
                // Update schedule with string from appsettings.json
                _monitorPingService = serviceProvider.GetService<IMonitorPingService>();
                MonitorContext monitorContext= serviceProvider.GetService<MonitorContext>();
                ResultObj result=_monitorPingService.SaveData(monitorContext);
                Console.Write(result.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured in SaveScheduleTask.ProcesInScope() : " + e.Message);
            }
            Console.WriteLine("Saving data processing ends here");
            return Task.CompletedTask;
        }


    }
}
