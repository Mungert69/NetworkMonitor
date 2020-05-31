using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class MonitorPingService : IMonitorPingService
    {

        private readonly IConfiguration _config;
        private PingParams _pingParams = new PingParams();



        private List<MonitorPingInfo> monitorPingInfos = new List<MonitorPingInfo>();

        public MonitorPingService(IConfiguration config)
        {
            _config = config;
            PingParams.BufferLength = config.GetValue<int>("PingPacketSize");
            PingParams.TimeOut = config.GetValue<int>("PingTimeOut");
            PingParams.PingBurstDelay= config.GetValue<int>("PingBurstDelay");
            PingParams.PingBurstNumber= config.GetValue<int>("PingBurstNumber");
            PingParams.Schedule =config.GetValue<string>("PingSchedule");
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
        public PingParams PingParams { get => _pingParams; set => _pingParams = value; }
    }
}
