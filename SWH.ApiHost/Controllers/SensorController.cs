using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SWH.Api.Contracts;
using SWH.ApiHost.Code;

namespace SWH.ApiHost.Controllers
{
    [Route("sensor")]
    public class SensorController : Controller
    {
        private readonly ISmartSensorMaster _master;
        private readonly ILogger<SensorController> _logger;

        public SensorController(ISmartSensorMaster master, ILogger<SensorController> logger)
        {
            _master = master;
            _logger = logger;
        }

        [HttpPost("report")]
        public async Task<IActionResult> ReportStats([FromBody] SmartSensorReport report)
        {
            _logger.LogTrace("Receiving update from Pi");

            try
            {
                _logger.LogDebug(JsonConvert.SerializeObject(report));

                await _master.LogReport(report);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            //We won't report errors back to the pi...
            return Ok();
        }
    }
}
