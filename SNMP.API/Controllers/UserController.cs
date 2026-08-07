using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract.UserService;
using Snmp.Business.DTOs.User;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO updateUserDTO, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateAsync(updateUserDTO, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _userService.DeleteAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return NoContent();
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var result = await _userService.GetByUsernameAsync(username);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return Ok(result);
        }
    }
}
