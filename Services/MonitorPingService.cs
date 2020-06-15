using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NetworkMonitor.Data;
using NetworkMonitor.Objects;
using NetworkMonitor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkMonitor.Services
{
    public class MonitorPingService : IMonitorPingService
    {

        private readonly IConfiguration _config;
        private PingParams _pingParams;
        private IWebHostEnvironment _env = null;
        private string _publicIPAddress;
        private string[] _monitorIPs;
        private bool _isSaving = false;
        private bool _isPinging = false;




        private List<MonitorPingInfo> _monitorPingInfos = null;
        private IMessageService _mailMessageService;
        private INetStatsService _netStatsService;

        public MonitorPingService(IConfiguration config, IMessageService mailMessageService, IWebHostEnvironment webHostEnv, INetStatsService netStatsService)
        {
            _config = config;
            _mailMessageService = mailMessageService;
            _env = webHostEnv;
            _netStatsService = netStatsService;
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
            _pingParams.AlertThreshold = _config.GetValue<int>("PingAlertThreshold");
            _pingParams.DisableNetStatService = _config.GetValue<bool>("DisableNetStatService");
            _pingParams.LogStatsThreshold = _config.GetValue<int>("LogNetworkStatsThreshold");
            _pingParams.NetStatsDeviceID = _config.GetValue<int>("NetStatsDeviceID");

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

        }


        public void StartNetStats()
        {
            if (!_pingParams.DisableNetStatService) _netStatsService.start(_pingParams.NetStatsDeviceID);
        }

        public void StopNetStats()
        {
            if (!_pingParams.DisableNetStatService) _netStatsService.stop();
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

        public ResultObj Ping()
        {

            ResultObj result = new ResultObj();
            try
            {
                PingParams pingParams = _pingParams;
                PingIt pingIt;

             
                StopNetStats();

               

               

                for (int i = 0; i < pingParams.PingBurstNumber; i++)
                {
                    foreach (MonitorPingInfo monitorPingInfo in _monitorPingInfos)
                    {
                        // We must abort pinging as soon as possible to stop DB write errors.
                        if (_isSaving)
                        {
                            Console.WriteLine("Aborting MonitorPingService.Ping save in progress ");
                            result.Message = "Aborting MonitorPingService.Ping save in progress ";
                            result.Success = false;
                            _isPinging = false;
                            return result;

                        }
                        _isPinging = true;
                        pingIt = new PingIt(monitorPingInfo, pingParams);
                        pingIt.go();
                        if (pingIt.RoundTrip > _pingParams.LogStatsThreshold)
                        {

                            Console.WriteLine("Ping threshold met for IP Address : " + monitorPingInfo.IPAddress + "  RoundTrip time was : " + pingIt.RoundTrip);
                            StartNetStats();
                        }
                        //Console.WriteLine("IP Address : " + monitorPingInfo.IPAddress);
                        //Console.WriteLine("Status : " + monitorPingInfo.MonitorStatus);
                        //Console.WriteLine("Trip Time : " + monitorPingInfo.RoundTripTimeAverage);
                    }
                    Thread.Sleep(pingParams.PingBurstDelay);
                }
                result.Message = "MonitorPingService.Ping Success";
                result.Success = true;
            }
            catch (Exception e)
            {

                result.Message = "MonitorPingService.Ping Failed : Error was : " + e.Message;
                result.Success = false;
            }
            finally {
                _isPinging = false;
            }
            return result;
        }

        public ResultObj Alert()
        {
            ResultObj result = new ResultObj();

            try
            {
                while (_isSaving)
                {
                    Console.WriteLine("Waiting for Save in MonitorPingService...");
                    Thread.Sleep(1000);
                }


                bool alert = false;
                string alertMessage = "Message from host : " + _publicIPAddress + "\n";
                foreach (MonitorPingInfo monPingInfo in _monitorPingInfos)
                {
                    if (monPingInfo.MonitorStatus.DownCount > _pingParams.AlertThreshold && monPingInfo.MonitorStatus.AlertSent == false)
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
                result.Message = "MonitorPingService.Alert Success";
                result.Success = true;
            }
            catch (Exception e)
            {

                result.Message = "MonitorPingService.AlertFailed : Error was : " + e.Message;
                result.Success = false;
            }
            finally { }
            return result;


        }



        public ResultObj SaveData(MonitorContext monitorContext)
        {
            _isSaving = true;
            while (_isPinging)
            {
                Console.WriteLine("Waiting for Ping to stop in MonitorPingService.Save");
                Thread.Sleep(1000);
            }
            ResultObj result = new ResultObj();
            result.Success = false;
            Console.WriteLine("Starting MonitorPingService.Save");
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

                foreach (MonitorPingInfo monitorPingInfo in _monitorPingInfos)
                {
                    foreach (PingInfo pingInfo in monitorPingInfo.pingInfos)
                    {
                        pingInfo.MonitorPingInfoID = monitorPingInfo.ID;

                    }
                }

                foreach (MonitorPingInfo monitorPingInfo in _monitorPingInfos)
                {
                    foreach (PingInfo pingInfo in monitorPingInfo.pingInfos)
                    {
                        pingInfo.ID = 0;
                        monitorContext.Add(pingInfo);

                    }
                }

                monitorContext.SaveChanges();


                _netStatsService.stop();
                List<NetStat> netStatsData = new List<NetStat>(_netStatsService.NetStatData);
                foreach (NetStat netStat in netStatsData)
                {
                    //netStat.ID = 0;
                    netStat.DataSetID = maxDataSetID;
                    monitorContext.Add(netStat);
                }

                monitorContext.SaveChanges();


                result.Message = "DB Update Success in MonitorPinService.SaveData.";
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Message = "DB Update Failed in MonitorPinService.SaveData. Error was : " + e.Message;
            }
            finally
            {
                // Make sure the reset of the MonitorPingService Object is run just before the next schedule.
                init(false);
                _isSaving = false;
                Console.WriteLine("Finished MonitorPingService.Save");
            }


            return result;
        }

        public PingParams PingParams { get => _pingParams; set => _pingParams = value; }
        public List<MonitorPingInfo> MonitorPingInfos { get => _monitorPingInfos; set => _monitorPingInfos = value; }
       
    }
}
