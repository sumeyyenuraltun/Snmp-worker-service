using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract.DeviceService;
using Snmp.Business.DTOs.Devices;


namespace SNMP.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _deviceService.GetAllAsync(cancellationToken);
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id,CancellationToken cancellationToken)
        {
            var result = await _deviceService.GetByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] AddDeviceDTO addDeviceDTO, CancellationToken cancellationToken)
        {
            var result = await _deviceService.AddAsync(addDeviceDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateDeviceDTO updateDeviceDTO, CancellationToken cancellationToken)
        {
            var result = await _deviceService.UpdateAsync(updateDeviceDTO, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _deviceService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }
    }
}
