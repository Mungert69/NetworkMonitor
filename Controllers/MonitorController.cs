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
                if (dataSetId == 0) {
                    result.Data = _monitorPingService.MonitorPingInfos;
                } else {
                    result.Data = _monitorContext.MonitorPingInfos.Where(m => m.DataSetID == dataSetId).ToList();

                }
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


        [HttpGet("GetDataSets")]
        public ActionResult<ResultObj> GetDataSets()
        {

            ResultObj result = new ResultObj();
            result.Success = false;
            try
            {
                
                List<MonitorPingInfo> monitorPingInfos= _monitorContext.MonitorPingInfos.ToList();
                List<DataSetObj> dataSets = new List<DataSetObj>();
                DataSetObj dataSet=new DataSetObj(); ;
                dataSet.DataSetId = 0;
                dataSet.DateStarted = _monitorPingService.MonitorPingInfos[0].DateStarted;
                dataSets.Add(dataSet);
                foreach (MonitorPingInfo monitorPingInfo in monitorPingInfos) {
                    dataSet = new DataSetObj();
                    dataSet.DataSetId = monitorPingInfo.DataSetID;
                    dataSet.DateStarted = monitorPingInfo.DateStarted;
                    if (dataSets.Where(d => d.DataSetId == monitorPingInfo.DataSetID).ToList().Count() == 0) {
                        dataSets.Add(dataSet);
                    }
                    
                }

                result.Data = dataSets;
                result.Success = true;
                result.Message = "Success got DataSets ";
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Success = false;
                result.Message = "Failed to get DataSets : Error was : " + e.Message;
                return result;

            }
        }


        [HttpGet("GetPingInfosByMonitorPingInfoID/{monitorPingInfoId}/{dataSetId}")]
        public ActionResult<ResultObj> GetPingInfosByMonitorPingInfoID([FromRoute] int monitorPingInfoId, [FromRoute] int dataSetId)
        {

            ResultObj result = new ResultObj();
            result.Success = false;
            try
            {
                if (dataSetId == 0) {
                    result.Data = _monitorPingService.MonitorPingInfos.Where(m => m.ID == monitorPingInfoId).FirstOrDefault().pingInfos;
                }
                else {
                    result.Data = _monitorContext.PingInfos.Where(p => p.MonitorPingInfoID == monitorPingInfoId).OrderBy(o => o.DateSent).ToList();

                }
                result.Data = _monitorContext.PingInfos.Where(p => p.MonitorPingInfoID == monitorPingInfoId).OrderBy(o => o.DateSent).ToList();
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
            return _monitorPingService.SaveData(_monitorContext);
           
        }

    }
}
