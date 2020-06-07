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

        ResultObj Alert();
        void init(bool initMonitorPingInfos);
        Task<ResultObj> SaveData(MonitorContext monitorContext);
    }
}