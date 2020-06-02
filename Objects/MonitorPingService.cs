using Microsoft.Extensions.Configuration;
using NetworkMonitor.Data;
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
                monitorPingInfo.ID = i + 1;
                monitorPingInfo.IPAddress = monitorIPs[i];
                _monitorPingInfos.Add(monitorPingInfo);
            }
            _requestInit = false;
        }


        public ResultObj SaveData(MonitorContext monitorContext)
        {
            ResultObj result = new ResultObj();
            result.Success = false;
            if (RequestInit == true)
            {
                result.Message = "Can not save data an Initialse MonitorPingService is pending. Try again after next ping schedule.";
                return result;
            }
            try
            {
                int maxDataSetID = 0;
                try { maxDataSetID = monitorContext.MonitorPingInfos.Max(m => m.DataSetID); }
                catch { }


                maxDataSetID++;
                foreach (MonitorPingInfo monitorPingInfo in _monitorPingInfos)
                {
                    monitorPingInfo.ID = 0;
                    monitorPingInfo.DataSetID = maxDataSetID;
                    monitorContext.Add(monitorPingInfo);
                }
                monitorContext.SaveChanges();

                int i = 0;
                foreach (MonitorPingInfo monitorPingInfo in monitorContext.MonitorPingInfos.Where(m => m.DataSetID == maxDataSetID).ToList())
                {
                    _monitorPingInfos[i].ID = monitorPingInfo.ID;
                    i++;
                }

                List<MonitorPingInfo> monitorPingInfos = new List<MonitorPingInfo>(_monitorPingInfos);
                foreach (MonitorPingInfo monitorPingInfo in monitorPingInfos)
                {
                    foreach (PingInfo pingInfo in monitorPingInfo.pingInfos)
                    {
                        pingInfo.MonitorPingInfoID = monitorPingInfo.ID;
                        pingInfo.ID = 0;
                        monitorContext.Add(pingInfo);

                    }
                }

                monitorContext.SaveChanges();
                // Make sure the reset of the MonitorPingService Object is run just before the next schedule.
                RequestInit = true;



                result.Message = "DB Update Success in MonitorPinService.SaveData.";
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Message = "DB Update Failed in MonitorPinService.SaveData. Error was : " + e.Message;
            }


            return result;
        }


        public PingParams PingParams { get => _pingParams; set => _pingParams = value; }
        public bool RequestInit { get => _requestInit; set => _requestInit = value; }
        public List<MonitorPingInfo> MonitorPingInfos { get => _monitorPingInfos; set => _monitorPingInfos = value; }
    }
}
