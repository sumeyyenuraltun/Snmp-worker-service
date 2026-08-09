using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract.DeviceService;
using Snmp.Business.DTOs.SnmpCredentials;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SnmpCredentialController : ControllerBase
    {
        private readonly ISnmpCredentialService _snmpCredentialService;

        public SnmpCredentialController(ISnmpCredentialService snmpCredentialService)
        {
            _snmpCredentialService = snmpCredentialService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _snmpCredentialService.GetAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _snmpCredentialService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddSnmpCredentialDTO addSnmpCredentialDTO, CancellationToken cancellationToken)
        {
            var result = await _snmpCredentialService.AddAsync(addSnmpCredentialDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSnmpCredentialDTO updateSnmpCredentialDTO, CancellationToken cancellationToken)
        {
            var result = await _snmpCredentialService.UpdateAsync(updateSnmpCredentialDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _snmpCredentialService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }
    }
}
