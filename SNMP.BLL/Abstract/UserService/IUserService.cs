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
        Task<Result> AddAsync(AddUserDTO addUserDTO, CancellationToken cancellationToken);

        Task<Result> UpdateAsync(UpdateUserDTO updateUserDTO, CancellationToken cancellationToken);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);

        Task<Result<List<UserDTO>>> GetAllAsync();

        Task<Result<UserDTO>> GetByIdAsync(int id);
        Task<Result<UserAuthDTO>> GetByUsernameAsync(string username);
    }
}
