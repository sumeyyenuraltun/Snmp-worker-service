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
            var result = _deviceService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _deviceService.GetById(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDeviceDTO addDeviceDTO, CancellationToken cancellationToken)
        {
            await _deviceService.Add(addDeviceDTO, cancellationToken);
            return StatusCode(201);
        }

        [HttpPut]
        public IActionResult Update([FromBody] UpdateDeviceDTO updateDeviceDTO)
        {
            _deviceService.Update(updateDeviceDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _deviceService.Delete(id);
            return NoContent();
        }
    }
}
