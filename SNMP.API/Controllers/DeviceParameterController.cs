using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract.DeviceService;
using Snmp.Business.DTOs.DeviceParameter;


namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeviceParameterController : ControllerBase
    {
        private readonly IDeviceParameterService _deviceParameterService;

        public DeviceParameterController(IDeviceParameterService deviceParameterService)
        {
            _deviceParameterService = deviceParameterService;
        }

        [HttpGet("device/{deviceId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetByDeviceId(int deviceId, CancellationToken cancellationToken)
        {
            var result = await _deviceParameterService.GetByDeviceIdAsync(deviceId, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id,CancellationToken cancellationToken)
        {
            var result = await _deviceParameterService.GetByIdAsync(id,cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] AddDeviceParameterDTO addDeviceParameterDTO, CancellationToken cancellationToken)
        {
            var result = await _deviceParameterService.AddAsync(addDeviceParameterDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateDeviceParameterDTO updateDeviceParameterDTO, CancellationToken cancellationToken)
        {
            var result = await _deviceParameterService.UpdateAsync(updateDeviceParameterDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _deviceParameterService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }
        [HttpGet("{deviceId}/{parameterId}/latest")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetLatestValue(int deviceId, int parameterId)
        {
            var result = await _deviceParameterService.GetLatestValueAsync(deviceId, parameterId);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }
    }
}
