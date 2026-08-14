using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.Abstract.Auth;
using Snmp.Business.DTOs.Auth;

namespace Snmp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO request,CancellationToken cancellationToken)
        {
            var result = await _authService.RefreshTokenAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDTO request,CancellationToken cancellationToken)
        {
            var result = await _authService.LogoutAsync(request.RefreshToken, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}
