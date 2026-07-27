using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.DeviceParameter;

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
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _deviceParameterService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDeviceParameterDTO addDeviceParameterDTO, CancellationToken cancellationToken)
        {
            await _deviceParameterService.AddAsync(addDeviceParameterDTO,cancellationToken);
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDeviceParameterDTO updateDeviceParameterDTO)
        {
            await _deviceParameterService.UpdateAsync(updateDeviceParameterDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _deviceParameterService.DeleteAsync(id);
            return NoContent();
        }
    }
}
