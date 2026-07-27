using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.DTOs.Devices;
using SNMP.BLL.Abstract;
using SNMP.ENTITY.Concrete;

namespace SNMP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _deviceService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _deviceService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDeviceDTO addDeviceDTO, CancellationToken cancellationToken)
        {
            await _deviceService.AddAsync(addDeviceDTO, cancellationToken);
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDeviceDTO updateDeviceDTO)
        {
            await _deviceService.UpdateAsync(updateDeviceDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _deviceService.DeleteAsync(id);
            return NoContent();
        }
    }
}
