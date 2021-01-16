using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetworkMonitor.Data;
using NetworkMonitor.Objects;
using NetworkMonitor.Services;
using System;

namespace NetworkMonitor.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMessageService _emailService;
        private MonitorContext _monitorContext;
        private readonly ILogger<EmailController> _logger;

        public EmailController(ILogger<EmailController> logger, IMessageService emailService, MonitorContext monitorContext)
        {
            _logger = logger;
            _emailService = emailService;
            _monitorContext = monitorContext;
        }


        [HttpGet]
        public ActionResult<ResultObj> Get()
        {
            ResultObj result = new ResultObj();
            result.Success = false;
            try
            {
                result.Data = _emailService.send("test");
                result.Success = true;
                result.Message = "Success email sent";
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Success = false;
                result.Message = "Failed to send email : Error was : " + e.Message;
                return result;

            }

        }
    }
}