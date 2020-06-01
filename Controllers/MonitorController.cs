using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetworkMonitor.Data;
using NetworkMonitor.Objects;

namespace NetworkMonitor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonitorController : ControllerBase
    {
       
        

        private readonly ILogger<MonitorController> _logger;
        private readonly IMonitorPingService _monitorPingService;
        private MonitorContext _monitorContext;

        public MonitorController(ILogger<MonitorController> logger, IMonitorPingService monitorPingService, MonitorContext monitorContext)
        {
            _logger = logger;
            _monitorPingService = monitorPingService;
            _monitorContext = monitorContext;
        }

        [HttpGet]
        public ActionResult<ResultObj> Get()
        {
            ResultObj result = new ResultObj();
            result.Success = false;
            try {
                result.Data = _monitorPingService.MonitorPingInfos;
                result.Success = true;
                result.Message = "Success got MonitorPingInfos";
                return result;
            }
            catch (Exception e) {
                result.Data = null;
                result.Success = false;
                result.Message = "Failed to get MonitorPingInfos : Error was : " + e.Message;
                return result;

            }
            
        }

        [HttpGet("GetMointorPingInfosByDataSetID/{dataSetId}")]
        public ActionResult<ResultObj> GetMointorPingInfosByDataSetID([FromRoute] int dataSetId)
        {

            ResultObj result = new ResultObj();
            result.Success = false;
            try
            {
                result.Data = _monitorContext.MonitorPingInfos.Where(m => m.DataSetID==dataSetId).ToList();
                result.Success = true;
                result.Message = "Success got MonitorPingInfos for DataSetID "+dataSetId;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Success = false;
                result.Message = "Failed to get MonitorPingInfos for DataSetID " + dataSetId+" : Error was : " + e.Message;
                return result;

            }
        }


        [HttpGet("GetPingInfosByMonitorPingInfoID/{monitorPingInfoId}")]
        public ActionResult<ResultObj> GetPingInfosByMonitorPingInfoID([FromRoute] int monitorPingInfoId)
        {

            ResultObj result = new ResultObj();
            result.Success = false;
            try
            {
                result.Data = _monitorContext.PingInfos.Where(p => p.MonitorPingInfoID == monitorPingInfoId).ToList();
                result.Success = true;
                result.Message = "Success got PingInfos for MontiorPingInfoId " + monitorPingInfoId;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Success = false;
                result.Message = "Failed to get PingInfos for MontiorPingInfoId " + monitorPingInfoId + " : Error was : " + e.Message;
                return result;

            }
        }


      

        [HttpGet("SaveData")]
        public ActionResult<ResultObj> SaveData() {
            ResultObj result = new ResultObj();
            result.Success = false;
            if (_monitorPingService.RequestInit == true) {
                result.Message = "Can not save data an Initialse MonitorPingService is pending. Try again after next ping schedule.";
                return result;
            }
            try {
                int maxDataSetID = 0;
                try { maxDataSetID = _monitorContext.MonitorPingInfos.Max(m => m.DataSetID); }
                catch { }
                   
               
                maxDataSetID++;
                foreach (MonitorPingInfo monitorPingInfo in _monitorPingService.MonitorPingInfos)
                {
                    monitorPingInfo.ID = 0;
                    monitorPingInfo.DataSetID = maxDataSetID;
                    _monitorContext.Add(monitorPingInfo);
                }
                _monitorContext.SaveChanges();

                int i = 0;
                foreach (MonitorPingInfo monitorPingInfo in _monitorContext.MonitorPingInfos.Where(m => m.DataSetID==maxDataSetID).ToList()) {
                    _monitorPingService.MonitorPingInfos[i].ID = monitorPingInfo.ID;
                    i++;
                }

                List<MonitorPingInfo> monitorPingInfos = new List<MonitorPingInfo>(_monitorPingService.MonitorPingInfos);
                 foreach (MonitorPingInfo monitorPingInfo in monitorPingInfos)
                {
                    foreach (PingInfo pingInfo in monitorPingInfo.pingInfos) {
                        pingInfo.MonitorPingInfoID = monitorPingInfo.ID;
                        pingInfo.ID = 0;
                        _monitorContext.Add(pingInfo);

                    }                  
                }

                _monitorContext.SaveChanges();
                // Make sure the reset of the MonitorPingService Object is run just before the next schedule.
                _monitorPingService.RequestInit=true ;

                

                result.Message="DB Update Success in /SaveData.";
                result.Success = true;
            } 
            catch (Exception e) {
                result.Message = "DB Update Failed in /SaveData. Error was : "+e.Message;
            }

            
            return result;
        }

    }
}
