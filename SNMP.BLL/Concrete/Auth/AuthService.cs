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
            var userResult = await _userService.GetByUsernameAsync(request.Username);

            if (!userResult.IsSuccess || userResult.Value is null)
            {
                return Result<AuthResponseDTO>.Failure("Invalid username or password.");
            }

            var user = userResult.Value;

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Result<AuthResponseDTO>.Failure("Invalid username or password.");
            }

            var token = _jwtService.CreateToken(user);

            return Result<AuthResponseDTO>.Success(new AuthResponseDTO
            {
                Token = token
            });

        }

        public async Task<Result> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken)
        {
            var existingUserResult = await _userService.GetByUsernameAsync(request.Username);

            if (existingUserResult.IsSuccess)
            {
                return Result.Failure("Username is already taken.");
            }

            if(request.Password != request.ConfirmPassword)
            {
                return Result.Failure("Passwords do not match.");
            }

            var addUserDto = new AddUserDTO
            {
                Username = request.Username,
                Password = request.Password
            };

            return await _userService.AddAsync(addUserDto, cancellationToken);
        }
    }
}
