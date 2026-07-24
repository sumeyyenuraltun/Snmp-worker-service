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
        public IActionResult GetAll()
        {
            var result = _deviceService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _deviceService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDeviceDTO addDeviceDTO)
        {
            await _deviceService.AddAsync(addDeviceDTO);
            return StatusCode(201);
        }

        [HttpPut]
        public IActionResult Update([FromBody] UpdateDeviceDTO updateDeviceDTO)
        {
            _deviceService.UpdateAsync(updateDeviceDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _deviceService.DeleteAsync(id);
            return NoContent();
        }
    }
}
