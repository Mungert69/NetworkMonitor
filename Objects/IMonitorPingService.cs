using System.Collections.Generic;

namespace NetworkMonitor.Objects
{
    public interface IMonitorPingService
    {
        List<MonitorPingInfo> MonitorPingInfos { get; set; }
        PingParams PingParams { get; set; }
    }
}