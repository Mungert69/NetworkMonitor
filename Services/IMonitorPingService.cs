using NetworkMonitor.Data;
using NetworkMonitor.Objects;
using System.Collections.Generic;

namespace NetworkMonitor.Services
{
    public interface IMonitorPingService
    {

        List<MonitorPingInfo> MonitorPingInfos { get; set; }
        PingParams PingParams { get; set; }

        ResultObj Alert();
        void init(bool initMonitorPingInfos);
        ResultObj Ping();
        ResultObj SaveData(MonitorContext monitorContext);
        void StartNetStats();
        void StopNetStats();
    }
}