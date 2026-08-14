using AutoMapper;
using Snmp.Business.Abstract.UserService;
using Snmp.Business.DTOs.Role;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Concrete.UserService
{
    public class RoleService : IRoleService
    {
        private readonly IRoleDAL _roleDAL;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserDAL _userDAL;
        public RoleService(IRoleDAL roleDAL, IUnitOfWork unitOfWork, IMapper mapper, IUserDAL userDAL)
        {
            _roleDAL = roleDAL;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userDAL = userDAL;
        }

        public async Task<Result> AddAsync(AddRoleDTO addRoleDTO, CancellationToken cancellationToken)
        {
            var exists = await _roleDAL.GetAsync(x => x.Name == addRoleDTO.Name && x.IsActive);

            if (exists != null)
            {
                return Result.Failure("Role already exists");
            }

            var entity = _mapper.Map<Role>(addRoleDTO);

            await _roleDAL.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _roleDAL.GetAsync(x=> x.Id == id && x.IsActive, cancellationToken);

            if (entity == null)
                return Result.Failure("Role not found");


            var hasUsers = await _userDAL.AnyAsync(X => X.RoleId == id && X.IsActive, cancellationToken);
            if (hasUsers)
                return Result.Failure("This role is assigned to one or more users.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _roleDAL.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();

        }

        public async Task<Result<List<RoleDTO>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var roles = await _roleDAL.GetAllAsync(x => x.IsActive, cancellationToken);

            var dto = _mapper.Map<List<RoleDTO>>(roles);

            return Result<List<RoleDTO>>.Success(dto);
        }

        public async Task<Result<RoleDTO>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _roleDAL.GetAsync(x => x.Id == id && x.IsActive, cancellationToken);
            if (entity == null)
                return Result<RoleDTO>.Failure("Role not found.");

            var dto = _mapper.Map<RoleDTO>(entity);

            return Result<RoleDTO>.Success(dto);

        }

        public async Task<Result<RoleDTO>> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            var entity =await _roleDAL.GetAsync(x => x.Name == name && x.IsActive, cancellationToken);

            if (entity == null)
                return Result<RoleDTO>.Failure("Role not found.");

            var dto = _mapper.Map<RoleDTO>(entity);

            return Result<RoleDTO>.Success(dto);
           
        }

        public async Task<Result> UpdateAsync(UpdateRoleDTO updateRoleDTO, CancellationToken cancellationToken)
        {
            var entity = await _roleDAL.GetAsync(x => x.Id == updateRoleDTO.Id && x.IsActive, cancellationToken);

            if(entity == null)
            {
                return Result.Failure("Role not found.");
            }

            var roleExists = await _roleDAL.GetAsync(x=>x.Name ==  updateRoleDTO.Name && x.Id!=updateRoleDTO.Id && x.IsActive, cancellationToken);

            if (roleExists!= null)
            {
                return Result.Failure("Role already exists.");
            }

            _mapper.Map(updateRoleDTO, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _roleDAL.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
