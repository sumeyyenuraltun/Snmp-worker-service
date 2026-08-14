using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract.DeviceService;
using Snmp.Business.DTOs.Parameter;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParameterController : ControllerBase
    {
        private readonly IParameterService _parameterService;

        public ParameterController(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _parameterService.GetAllAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _parameterService.GetByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] AddParameterDTO addParameterDTO, CancellationToken cancellationToken)
        {
            var result = await _parameterService.AddAsync(addParameterDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateParameterDTO updateParameterDTO, CancellationToken cancellationToken)
        {
            var result = await _parameterService.UpdateAsync(updateParameterDTO, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _parameterService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }
    }
}
