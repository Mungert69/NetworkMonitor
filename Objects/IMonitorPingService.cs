using NetworkMonitor.Data;
using System.Collections.Generic;

namespace NetworkMonitor.Objects
{
    public interface IMonitorPingService
    {
        List<MonitorPingInfo> MonitorPingInfos { get; set; }
        PingParams PingParams { get; set; }
        bool RequestInit { get; set; }

        ResultObj Alert();
        void init();
        ResultObj SaveData(MonitorContext monitorContext);
    }
}