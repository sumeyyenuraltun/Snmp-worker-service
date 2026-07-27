using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.Parameter;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParameterController : ControllerBase
    {
        private readonly IParameterService _parameterService;

        public ParameterController(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _parameterService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _parameterService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddParameterDTO addParameterDTO, CancellationToken cancellationToken)
        {
            await _parameterService.AddAsync(addParameterDTO, cancellationToken);
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateParameterDTO updateParameterDTO)
        {
            await _parameterService.UpdateAsync(updateParameterDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _parameterService.DeleteAsync(id);
            return NoContent();
        }
    }
}
