using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.DeviceParameter;
using Snmp.Business.Results;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceParameterController : ControllerBase
    {
        private readonly IDeviceParameterService _deviceParameterService;

        public DeviceParameterController(IDeviceParameterService deviceParameterService)
        {
            _deviceParameterService = deviceParameterService;
        }

        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetByDeviceId(int deviceId)
        {
            var result = await _deviceParameterService.GetByDeviceIdAsync(deviceId);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _deviceParameterService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDeviceParameterDTO addDeviceParameterDTO, CancellationToken cancellationToken)
        {
            var result = await _deviceParameterService.AddAsync(addDeviceParameterDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDeviceParameterDTO updateDeviceParameterDTO, CancellationToken cancellationToken)
        {
            var result = await _deviceParameterService.UpdateAsync(updateDeviceParameterDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _deviceParameterService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }
    }
}
