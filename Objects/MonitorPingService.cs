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
        private PingParams _pingParams;
        private bool _requestInit = false;



        private List<MonitorPingInfo> _monitorPingInfos;

        public MonitorPingService(IConfiguration config)
        {
            _config = config;
            init();

        }


        public void init()
        {
            _pingParams = new PingParams();
            _monitorPingInfos = new List<MonitorPingInfo>();

            _pingParams.BufferLength = _config.GetValue<int>("PingPacketSize");
            _pingParams.TimeOut = _config.GetValue<int>("PingTimeOut");
            _pingParams.PingBurstDelay = _config.GetValue<int>("PingBurstDelay");
            _pingParams.PingBurstNumber = _config.GetValue<int>("PingBurstNumber");
            _pingParams.Schedule = _config.GetValue<string>("PingSchedule");
            string[] monitorIPs = _config.GetSection("MonitorIps").GetChildren().ToArray().Select(c => c.Value).ToArray();
            MonitorPingInfo monitorPingInfo;
            for (int i = 0; i < monitorIPs.Length; i++)
            {
                monitorPingInfo = new MonitorPingInfo();
                monitorPingInfo.IPAddress = monitorIPs[i];
                _monitorPingInfos.Add(monitorPingInfo);
            }
            _requestInit = false;
        }


        public PingParams PingParams { get => _pingParams; set => _pingParams = value; }
        public bool RequestInit { get => _requestInit; set => _requestInit = value; }
        public List<MonitorPingInfo> MonitorPingInfos { get => _monitorPingInfos; set => _monitorPingInfos = value; }
    }
}
