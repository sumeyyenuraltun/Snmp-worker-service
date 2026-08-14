using Snmp.Business.DTOs.Auth;
using Snmp.Business.DTOs.Devices;
using Snmp.Business.DTOs.User;
using Snmp.Business.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.UserService
{
    public interface IUserService
    {
        Task<Result> AddAsync(RegisterRequestDTO registerRequestDTO, CancellationToken cancellationToken);

        Task<Result> UpdateAsync(UpdateUserDTO updateUserDTO, CancellationToken cancellationToken);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);

        Task<Result<List<UserDTO>>> GetAllAsync(CancellationToken cancellationToken);

        Task<Result<UserDTO>> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Result<UserAuthDTO>> GetByUsernameAsync(string username , CancellationToken cancellationToken);
        Task<Result> UpdateRefreshTokenAsync(int userId,string refreshToken,DateTime expiryTime,CancellationToken cancellationToken);
        Task<Result<UserAuthDTO>> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task<Result> ClearRefreshTokenAsync(int userId,CancellationToken cancellationToken);
    }
}
