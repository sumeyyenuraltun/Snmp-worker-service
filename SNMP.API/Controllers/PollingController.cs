using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract.Snmp;
using Snmp.Business.DTOs.Polling;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
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
            var result = await _pollingService.StartAsync(startPollingDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new
            {
                message = "Polling started successfully."
            });
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop(StopPollingDTO stopPollingDTO, CancellationToken cancellationToken)
        {
            var result = await _pollingService.StopAsync(stopPollingDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new
            {
                message = "Polling stopped successfully."
            });
        }
       
    }
}
