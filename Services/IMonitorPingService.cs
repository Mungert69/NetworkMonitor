using NetworkMonitor.Data;
using NetworkMonitor.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkMonitor.Services
{
    public interface IMonitorPingService
    {
        List<MonitorPingInfo> MonitorPingInfos { get; set; }
        PingParams PingParams { get; set; }
        bool RequestInit { get; set; }

        void StartNetStats();

        void StopNetStats();

        ResultObj Alert();
        void init(bool initMonitorPingInfos);
        Task<ResultObj> SaveDataAsync(MonitorContext monitorContext);
        ResultObj SaveData(MonitorContext monitorContext);
    }
}