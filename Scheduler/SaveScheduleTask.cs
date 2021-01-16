
using Microsoft.Extensions.DependencyInjection;
using NetworkMonitor.Data;
using NetworkMonitor.Objects;
using NetworkMonitor.Services;
using System;
using System.Threading.Tasks;

namespace ASPNETCoreScheduler.Scheduler
{
    public class SaveScheduleTask : ScheduledProcessor
    {
        private IMonitorPingService _monitorPingService;


        public SaveScheduleTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "0 */1 * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Saving data processing starts here");
            try
            {
                // Update schedule with string from appsettings.json
                _monitorPingService = serviceProvider.GetService<IMonitorPingService>();
                MonitorContext monitorContext = serviceProvider.GetService<MonitorContext>();
                updateSchedule(_monitorPingService.PingParams.SaveSchedule);
                ResultObj result = _monitorPingService.SaveData(monitorContext);
                Console.WriteLine(result.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured in SaveScheduleTask.ProcesInScope() : " + e.Message + "\n");
            }
            Console.WriteLine("Saving data processing ends here");
            return Task.CompletedTask;
        }


    }
}
