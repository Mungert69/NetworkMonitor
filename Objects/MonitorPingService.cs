using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class MonitorPingService : IMonitorPingService
    {
        /*private static readonly string[] monitorIPs = new[]
        {
            "192.168.1.1", "192.168.1.2", "192.168.1.3", "192.168.1.4"
        };*/
        private readonly IConfiguration _config;

        private List<MonitorPingInfo> monitorPingInfos = new List<MonitorPingInfo>();

        public MonitorPingService(IConfiguration config)
        {
            _config = config;
            string[] monitorIPs = config.GetSection("MonitorIps").GetChildren().ToArray().Select(c => c.Value).ToArray();
            MonitorPingInfo monitorPingInfo;
            for (int i = 0; i < monitorIPs.Length; i++)
            {
                monitorPingInfo = new MonitorPingInfo();
                monitorPingInfo.IPAddress = monitorIPs[i];
                MonitorPingInfos.Add(monitorPingInfo);
            }
        }

        public List<MonitorPingInfo> MonitorPingInfos { get => monitorPingInfos; set => monitorPingInfos = value; }
    }
}
