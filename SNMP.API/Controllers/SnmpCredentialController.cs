using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.SnmpCredentials;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _snmpCredentialService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddSnmpCredentialDTO addSnmpCredentialDTO, CancellationToken cancellationToken)
        {
            await _snmpCredentialService.AddAsync(addSnmpCredentialDTO,cancellationToken);
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSnmpCredentialDTO updateSnmpCredentialDTO)
        {
            await _snmpCredentialService.UpdateAsync(updateSnmpCredentialDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _snmpCredentialService.DeleteAsync(id);
            return NoContent();
        }
    }
}
