using Snmp.Business.Abstract.Auth;
using Snmp.Business.Abstract.Security;
using Snmp.Business.Abstract.UserService;
using Snmp.Business.DTOs.Auth;
using Snmp.Business.DTOs.User;
using Snmp.Business.Results;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Concrete.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJWTService _jwtService;

        public AuthService(IUserService userService, IPasswordHasher passwordHasher, IJWTService jwtService)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthResponseDTO>> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken)
        {
            var userResult = await _userService.GetByUsernameAsync(request.Username, cancellationToken);

            if (!userResult.IsSuccess || userResult.Value is null)
            {
                return Result<AuthResponseDTO>.Failure("Invalid username or password.");
            }

            var user = userResult.Value;

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Result<AuthResponseDTO>.Failure("Invalid username or password.");
            }

            var accessToken = _jwtService.CreateToken(user);

            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            var updateResult = await _userService.UpdateRefreshTokenAsync(user.Id,refreshToken,DateTime.UtcNow.AddDays(7),cancellationToken);

            if (!updateResult.IsSuccess)
            {
                return Result<AuthResponseDTO>.Failure(updateResult.Error!);
            }

            return Result<AuthResponseDTO>.Success(new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });

        }

        public async Task<Result<AuthResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken)
        {
            var userResult = await _userService.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (!userResult.IsSuccess || userResult.Value is null)
            {
                return Result<AuthResponseDTO>.Failure("Invalid refresh token.");
            }

            var user = userResult.Value;
            if (user.RefreshTokenExpiryTime is null ||user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Result<AuthResponseDTO>.Failure("Refresh token has expired.");
            }

            var accessToken = _jwtService.CreateToken(user);

            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            var updateResult = await _userService.UpdateRefreshTokenAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(7), cancellationToken);

            if (!updateResult.IsSuccess)
            {
                return Result<AuthResponseDTO>.Failure(updateResult.Error!);
            }

            return Result<AuthResponseDTO>.Success(new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            });
        }

        public async Task<Result> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken)
        {
            var existingUserResult = await _userService.GetByUsernameAsync(request.Username, cancellationToken);

            if (existingUserResult.IsSuccess)
            {
                return Result.Failure("Username is already taken.");
            }

            if(request.Password != request.ConfirmPassword)
            {
                return Result.Failure("Passwords do not match.");
            }

            var registerRequestDTO = new RegisterRequestDTO
            {
                Username = request.Username,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword
            };

            return await _userService.AddAsync(registerRequestDTO, cancellationToken);
        }
        public async Task<Result> LogoutAsync(string refreshToken,CancellationToken cancellationToken)
        {
            var userResult = await _userService.GetByRefreshTokenAsync(refreshToken, cancellationToken);

            if (!userResult.IsSuccess || userResult.Value is null)
                return Result.Failure("Invalid refresh token.");

            return await _userService.ClearRefreshTokenAsync(userResult.Value.Id,cancellationToken);
        }
    }
}
