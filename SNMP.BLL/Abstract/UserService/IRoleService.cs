using Snmp.Business.DTOs.Role;
using Snmp.Business.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.UserService
{
    public interface IRoleService
    {
        Task<Result<List<RoleDTO>>> GetAllAsync(CancellationToken cancellationToken);

        Task<Result<RoleDTO>> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<Result> AddAsync(AddRoleDTO addRoleDTO, CancellationToken cancellationToken);

        Task<Result> UpdateAsync(UpdateRoleDTO updateRoleDTO, CancellationToken cancellationToken);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<Result<RoleDTO>> GetByNameAsync(string name, CancellationToken cancellationToken);
    }
}
