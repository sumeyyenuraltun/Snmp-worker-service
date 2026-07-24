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
        public async Task<IActionResult> Get(SnmpRequestDTO snmpRequestDTO, CancellationToken cancellationToken)
        {
            await _snmpRequestedService.SendGetRequestedAsync(snmpRequestDTO,cancellationToken);
            return Ok("SNMP GET request has been queued.");
        }

        [HttpPost("walk")]
        public async Task<IActionResult> Walk(SnmpWalkRequestDTO snmpWalkRequestDTO, CancellationToken cancellationToken)
        {
            await _snmpRequestedService.SendWalkRequestedAsync(snmpWalkRequestDTO,cancellationToken);
            return Ok("SNMP WALK request has been queued.");
        }


    }
}
