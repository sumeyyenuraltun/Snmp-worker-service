using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.Polling;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollingController : ControllerBase
    {
        private readonly IPollingService _pollingService;

        public PollingController(IPollingService pollingService)
        {
            _pollingService = pollingService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start(StartPollingDTO startPollingDTO, CancellationToken cancellationToken)
        {
            await _pollingService.StartAsync(startPollingDTO, cancellationToken);
            return Ok("Polling started");
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop(StopPollingDTO stopPollingDTO, CancellationToken cancellationToken)
        {
            await _pollingService.StopAsync(stopPollingDTO, cancellationToken);
            return Ok("Polling stoped");
        }

    }
}
