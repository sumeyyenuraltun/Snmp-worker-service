using AutoMapper;
using Snmp.Business.Abstract.Security;
using Snmp.Business.Abstract.UserService;
using Snmp.Business.DTOs.Auth;
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
        private readonly IRoleDAL _roleDAL;

        public UserService(IUserDAL userDAL, IMapper mapper, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IRoleDAL roleDAL)
        {
            _userDAL = userDAL;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _roleDAL = roleDAL;
        }

       public async Task<Result> AddAsync(RegisterRequestDTO registerRequestDTO, CancellationToken cancellationToken)
        {
            var exists = await _userDAL.GetAsync(x =>x.Username == registerRequestDTO.Username && x.IsActive);

            if (exists != null)
                  return Result.Failure("Username already exists.");

            var role = await _roleDAL.GetAsync(x => x.Name == "User" && x.IsActive);

            if (role == null)
                return Result.Failure("Default role not found");


            var entity = _mapper.Map<User>(registerRequestDTO);

            entity.PasswordHash = _passwordHasher.Hash(registerRequestDTO.Password);
            entity.RoleId = role.Id;

            await _userDAL.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(UpdateUserDTO updateUserDTO, CancellationToken cancellationToken)
        {
            var entity = await _userDAL.GetAsync(x =>x.Id == updateUserDTO.Id && x.IsActive, cancellationToken);

            if (entity == null)
                return Result.Failure("User not found.");

            var usernameExists = await _userDAL.GetAsync(x =>
                x.Username == updateUserDTO.Username &&
                x.Id != updateUserDTO.Id &&
                x.IsActive, cancellationToken);

            if (usernameExists != null)
                return Result.Failure("Username already exists.");

            _mapper.Map(updateUserDTO, entity);

            if (!string.IsNullOrWhiteSpace(updateUserDTO.Password))
            {
                entity.PasswordHash = _passwordHasher.Hash(updateUserDTO.Password);
            }

            entity.UpdatedAt = DateTime.UtcNow;

            await _userDAL.UpdateAsync(entity,cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _userDAL.GetAsync(x =>
                x.Id == id &&
                x.IsActive, cancellationToken);

            if (entity == null)
                return Result.Failure("User not found.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _userDAL.UpdateAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<List<UserDTO>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var users = await _userDAL.GetAllAsync(x => x.IsActive ,cancellationToken);

            var dto = _mapper.Map<List<UserDTO>>(users);

            return Result<List<UserDTO>>.Success(dto);
        }

        public async Task<Result<UserDTO>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _userDAL.GetAsync(x =>
                x.Id == id &&
                x.IsActive, cancellationToken);

            if (entity == null)
                return Result<UserDTO>.Failure("User not found.");

            var dto = _mapper.Map<UserDTO>(entity);

            return Result<UserDTO>.Success(dto);
        }

        public async Task<Result<UserAuthDTO>> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var entity = await _userDAL.GetAsync(x => x.Username == username && x.IsActive,cancellationToken,u => u.Role);

            if (entity == null)
                return Result<UserAuthDTO>.Failure("User not found.");

            var dto = _mapper.Map<UserAuthDTO>(entity);

            return Result<UserAuthDTO>.Success(dto);

        }

        public async Task<Result> UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime, CancellationToken cancellationToken)
        {
            var user = await  _userDAL.GetAsync(x => x.Id == userId && x.IsActive, cancellationToken);

            if(user == null)
                return Result.Failure("User not found.");

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiryTime;
            user.UpdatedAt = DateTime.UtcNow;

            await _userDAL.UpdateAsync(user, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        public async Task<Result<UserAuthDTO>> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var entity = await _userDAL.GetAsync(x => x.RefreshToken == refreshToken && x.IsActive,cancellationToken,x => x.Role);

            if (entity == null)
                return Result<UserAuthDTO>.Failure("Invalid refresh token.");

            var dto = _mapper.Map<UserAuthDTO>(entity);

            return Result<UserAuthDTO>.Success(dto);
        }
        public async Task<Result> ClearRefreshTokenAsync(int userId,CancellationToken cancellationToken)
        {
            var user = await _userDAL.GetAsync(x => x.Id == userId && x.IsActive, cancellationToken);

            if (user == null)
                return Result.Failure("User not found.");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userDAL.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
