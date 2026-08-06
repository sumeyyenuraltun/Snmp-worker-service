using AutoMapper;
using Snmp.Business.Abstract.Security;
using Snmp.Business.Abstract.UserService;
using Snmp.Business.DTOs.User;
using Snmp.Business.Results;
using Snmp.DataAccess.Abstract;
using SNMP.ENTITY.Concrete;

namespace Snmp.Business.Concrete.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserDAL _userDAL;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserDAL userDAL, IMapper mapper, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _userDAL = userDAL;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> AddAsync(AddUserDTO addUserDTO, CancellationToken cancellationToken)
        {
              var exists = await _userDAL.GetAsync(x =>x.Username == addUserDTO.Username && x.IsActive);

              if (exists != null)
                  return Result.Failure("Username already exists.");

              var entity = _mapper.Map<User>(addUserDTO);

              entity.PasswordHash = _passwordHasher.Hash(addUserDTO.Password);

              await _userDAL.AddAsync(entity, cancellationToken);

              await _unitOfWork.SaveChangesAsync(cancellationToken);

              return Result.Success();
        }

        public async Task<Result> UpdateAsync(UpdateUserDTO updateUserDTO, CancellationToken cancellationToken)
        {
            var entity = await _userDAL.GetAsync(x =>x.Id == updateUserDTO.Id && x.IsActive);

            if (entity == null)
                return Result.Failure("User not found.");

            var usernameExists = await _userDAL.GetAsync(x =>
                x.Username == updateUserDTO.Username &&
                x.Id != updateUserDTO.Id &&
                x.IsActive);

            if (usernameExists != null)
                return Result.Failure("Username already exists.");

            _mapper.Map(updateUserDTO, entity);

            if (!string.IsNullOrWhiteSpace(updateUserDTO.Password))
            {
                entity.PasswordHash = _passwordHasher.Hash(updateUserDTO.Password);
            }

            entity.UpdatedAt = DateTime.UtcNow;

            await _userDAL.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _userDAL.GetAsync(x =>
                x.Id == id &&
                x.IsActive);

            if (entity == null)
                return Result.Failure("User not found.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _userDAL.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<List<UserDTO>>> GetAllAsync()
        {
            var users = await _userDAL.GetAllAsync(x => x.IsActive);

            var dto = _mapper.Map<List<UserDTO>>(users);

            return Result<List<UserDTO>>.Success(dto);
        }

        public async Task<Result<UserDTO>> GetByIdAsync(int id)
        {
            var entity = await _userDAL.GetAsync(x =>
                x.Id == id &&
                x.IsActive);

            if (entity == null)
                return Result<UserDTO>.Failure("User not found.");

            var dto = _mapper.Map<UserDTO>(entity);

            return Result<UserDTO>.Success(dto);
        }
    }
}
