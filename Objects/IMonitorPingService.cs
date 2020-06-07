using NetworkMonitor.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
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