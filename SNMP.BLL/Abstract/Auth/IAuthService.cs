using Snmp.Business.DTOs.Auth;
using Snmp.Business.Results;

namespace Snmp.Business.Abstract.Auth
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken);
        Task<Result<AuthResponseDTO>> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken);
        Task<Result<AuthResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request,CancellationToken cancellationToken);
        Task<Result> LogoutAsync(string refreshToken,CancellationToken cancellationToken);
    }
}
