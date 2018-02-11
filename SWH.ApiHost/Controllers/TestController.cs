using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWH.ApiHost.ServiceBus;

namespace SWH.ApiHost.Controllers
{
    [Route("api/test")]
    public class TestController: Controller
    {
        private readonly ISendMessages _messenger;

        public TestController(ISendMessages messenger)
        {
            _messenger = messenger;
        }

        [HttpGet("settemp/{newTemp}")]
        public async Task<IActionResult> SetNewTemp(int newTemp)
        {
            await _messenger.SetTemp(newTemp, "accessToken");

            return Ok();
        }
    }
}
