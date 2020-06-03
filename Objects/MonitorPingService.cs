using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NetworkMonitor.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class MonitorPingService : IMonitorPingService
    {

        private readonly IConfiguration _config;
        private PingParams _pingParams;
        private bool _requestInit = false;
        private IWebHostEnvironment _env = null;
        private string _publicIPAddress;
        private int _pingAlertThreshold;
        private string[] _monitorIPs;



        private List<MonitorPingInfo> _monitorPingInfos = null;
        private IMessageService _mailMessageService;

        public MonitorPingService(IConfiguration config, IMessageService mailMessageService, IWebHostEnvironment webHostEnv)
        {
            _config = config;
            _mailMessageService = mailMessageService;
            _env = webHostEnv;
            _publicIPAddress = GetPublicIP();
            init(true);

        }


        public void init(bool initMonitorPingInfos)
        {
            _pingParams = new PingParams();



            _pingParams.BufferLength = _config.GetValue<int>("PingPacketSize");
            _pingParams.TimeOut = _config.GetValue<int>("PingTimeOut");
            _pingParams.PingBurstDelay = _config.GetValue<int>("PingBurstDelay");
            _pingParams.PingBurstNumber = _config.GetValue<int>("PingBurstNumber");
            _pingParams.Schedule = _config.GetValue<string>("PingSchedule");
            _pingParams.SaveSchedule = _config.GetValue<string>("SaveSchedule");
            _pingParams.AlertSchedule = _config.GetValue<string>("AlertSchedule");
            _pingAlertThreshold = _config.GetValue<int>("PingAlertThreshold");


            if (initMonitorPingInfos)
            {
                // init fully on first run.
                _monitorPingInfos = new List<MonitorPingInfo>();
                _monitorIPs = _config.GetSection("MonitorIps").GetChildren().ToArray().Select(c => c.Value).ToArray();
                MonitorPingInfo monitorPingInfo;
                for (int i = 0; i < _monitorIPs.Length; i++)
                {
                    monitorPingInfo = new MonitorPingInfo();
                    monitorPingInfo.ID = i + 1;
                    monitorPingInfo.IPAddress = _monitorIPs[i];
                    _monitorPingInfos.Add(monitorPingInfo);
                }

            }
            else
            {
                List<MonitorPingInfo> newMonPingInfos = new List<MonitorPingInfo>();
                MonitorPingInfo newMonPingInfo;
                StatusObj status;
                int i = 0;
                // Copy Alert status before init.
                foreach (MonitorPingInfo monPingInfo in _monitorPingInfos)
                {
                    status = new StatusObj();
                    status.AlertFlag = monPingInfo.MonitorStatus.AlertFlag;
                    status.AlertSent = monPingInfo.MonitorStatus.AlertSent;
                    status.DownCount = monPingInfo.MonitorStatus.DownCount;
                    status.IsUp = monPingInfo.MonitorStatus.IsUp;
                    newMonPingInfo = new MonitorPingInfo();
                    newMonPingInfo.ID = i + 1;
                    newMonPingInfo.IPAddress = _monitorIPs[i];
                    newMonPingInfo.MonitorStatus = status;
                    newMonPingInfos.Add(newMonPingInfo);
                    i++;
                }
                _monitorPingInfos = new List<MonitorPingInfo>(newMonPingInfos);
            }



            _requestInit = false;
        }

        public static string GetPublicIP()
        {
            string myPublicIp = "";
            WebRequest request = WebRequest.Create("https://api.ipify.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                myPublicIp = stream.ReadToEnd();
            }
            return myPublicIp;
        }

        public ResultObj Alert()
        {
            ResultObj result = new ResultObj();
            bool alert = false;
            string alertMessage = "Message from host : " + _publicIPAddress + "\n";
            foreach (MonitorPingInfo monPingInfo in _monitorPingInfos)
            {
                if (monPingInfo.MonitorStatus.DownCount > _pingAlertThreshold && monPingInfo.MonitorStatus.AlertSent == false)
                {
                    alertMessage += "\nNode ping down " + monPingInfo.MonitorStatus.DownCount +
                        " times. For IP address " + monPingInfo.IPAddress + " Time : " + monPingInfo.MonitorStatus.EventTime;
                    alert = true;
                    monPingInfo.MonitorStatus.AlertFlag = true;
                }
            }
            if (alert)
            {
                _mailMessageService.setWebEnv(_env);
                //_mailMessageService.init();
                if (_mailMessageService.send(alertMessage).Success)
                {
                    // reset alerts
                    foreach (MonitorPingInfo monPingInfo in _monitorPingInfos)
                    {
                        if (monPingInfo.MonitorStatus.DownCount > 1)
                        {
                            monPingInfo.MonitorStatus.AlertSent = true;
                        }
                    }

                }

            }


            return result;
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
