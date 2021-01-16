
using Microsoft.Extensions.DependencyInjection;
using NetworkMonitor.Objects;
using NetworkMonitor.Services;
using System;
using System.Threading.Tasks;

namespace ASPNETCoreScheduler.Scheduler
{
    public class AlertScheduleTask : ScheduledProcessor
    {
        private IMonitorPingService _monitorPingService;


        public AlertScheduleTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/5 * * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            //Console.WriteLine("Alert processing starts here");
            try
            {
                // Update schedule with string from appsettings.json
                _monitorPingService = serviceProvider.GetService<IMonitorPingService>();
                updateSchedule(_monitorPingService.PingParams.AlertSchedule);
                ResultObj result = _monitorPingService.Alert();
                Console.WriteLine(result.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured in AlertScheduleTask.ProcesInScope() : " + e.Message);
            }
            //Console.WriteLine("Alert processing ends here");
            return Task.CompletedTask;
        }


    }
}
