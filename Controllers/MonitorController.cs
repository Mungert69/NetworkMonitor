using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetworkMonitor.Objects;

namespace NetworkMonitor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonitorController : ControllerBase
    {
       
        

        private readonly ILogger<MonitorController> _logger;
        private readonly IMonitorPingService _monitorPingService;

        public MonitorController(ILogger<MonitorController> logger, IMonitorPingService monitorPingService)
        {
            _logger = logger;
            _monitorPingService = monitorPingService;
        }

        [HttpGet]
        public IEnumerable<MonitorPingInfo> Get()
        {

            return _monitorPingService.MonitorPingInfos;
        }
    }
}
