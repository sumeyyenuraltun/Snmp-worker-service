using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract.UserService;
using Snmp.Business.DTOs.Role;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _roleService.GetAllAsync(cancellationToken);
            return Ok(result.Value);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) 
        {
            var result = await _roleService.GetByIdAsync(id,cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddRoleDTO addRoleDTO, CancellationToken cancellationToken)
        {
            var result = await _roleService.AddAsync(addRoleDTO, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateRoleDTO updateRoleDTO, CancellationToken cancellationToken)
        {
            var result = await _roleService.UpdateAsync(updateRoleDTO, cancellationToken);

            if(!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }

        [HttpDelete]

        public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken)
        {
            var result = await _roleService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound (result.Error);
            }

            return NoContent();
        }
    }
}
