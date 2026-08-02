using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.Snmp;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnmpController : ControllerBase
    {
        private readonly ISnmpRequestedService _snmpRequestedService;

        public SnmpController(ISnmpRequestedService snmpRequestedService)
        {
            _snmpRequestedService = snmpRequestedService;
        }

        [HttpPost("get")]
        public async Task<IActionResult> Get([FromBody] SnmpRequestDTO request,CancellationToken cancellationToken)
        {
            var result = await _snmpRequestedService.SendGetRequestedAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new
            {
                message = "SNMP GET request has been queued successfully."
            });
        }

        [HttpPost("walk")]
        public async Task<IActionResult> Walk([FromBody] SnmpWalkRequestDTO request,CancellationToken cancellationToken)
        {
            var result = await _snmpRequestedService.SendWalkRequestedAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new
            {
                message = "SNMP WALK request has been queued successfully."
            });
        }

        [HttpPost("getnext")]
        public async Task<IActionResult> GetNext( [FromBody] SnmpGetNextRequestDTO request,CancellationToken cancellationToken)
        {
            var result = await _snmpRequestedService.SendGetNextRequestedAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new
            {
                message = "SNMP GETNEXT request has been queued successfully."
            });
        }

        [HttpPost("set")]
        public async Task<IActionResult> Set([FromBody] SnmpSetRequestDTO request, CancellationToken cancellationToken)
        {
            var result = await _snmpRequestedService.SendSetRequestedAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new
            {
                message = "SNMP SET request has been queued successfully."
            });
        }


    }
}
