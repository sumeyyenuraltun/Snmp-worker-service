using AutoMapper;
using Moq;
using Snmp.Business.Abstract.Security;
using Snmp.Business.Concrete.UserService;
using Snmp.Business.DTOs.Auth;
using Snmp.Business.DTOs.User;
using Snmp.DataAccess.Abstract;
using SNMP.ENTITY.Concrete;
using System.Linq.Expressions;


namespace Snmp.Test.ServiceTests
{
    public class UserServiceTest
    {
        private readonly Mock<IUserDAL> _userDalMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IRoleDAL> _roleDalMock;

        private readonly UserService _userService;
        public UserServiceTest()
        {
            _userDalMock = new Mock<IUserDAL>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _roleDalMock = new Mock<IRoleDAL>();

            _userService = new UserService(
                _userDalMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _passwordHasherMock.Object,
                _roleDalMock.Object);
        }

        [Fact]

        public async Task AddAsync_ShouldReturnFailure_WhenUsernameExists()
        {
            // Arrange
            var registerRequestDTO = new RegisterRequestDTO { Username = "sumeyye", Password = "şifre123" };
            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(new User());
            // Act
            var result = await _userService.AddAsync(registerRequestDTO, CancellationToken.None);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Username already exists.", result.Error);

            _userDalMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuccess_WhenUsernameDoesNotExist()
        {
            // Arrange
            var registerRequestDTO = new RegisterRequestDTO
            {
                Username = "sumeyye123",
                Password = "şifre"

            };

            var user = new User();

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync((User?)null);
            _roleDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>(),It.IsAny<Expression<Func<Role, object>>[]>())).ReturnsAsync(new Role{Id = 2,Name = "User",IsActive = true});
            _mapperMock.Setup(x => x.Map<User>(registerRequestDTO)).Returns(user);

            _passwordHasherMock.Setup(x => x.Hash(registerRequestDTO.Password)).Returns("hashedPassword");

            //Act

            var result = await _userService.AddAsync(registerRequestDTO, CancellationToken.None);
            
            //Assert 
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            Assert.Equal("hashedPassword", user.PasswordHash);

            _userDalMock.Verify(x => x.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        }
        [Fact]
        public async Task AddAsync_ShouldReturnFailure_WhenDefaultRoleNotFound()
        {
            // Arrange
            var registerRequestDTO = new RegisterRequestDTO
            {
                Username = "sumeyye123",
                Password = "şifre"
            };

            _userDalMock.Setup(x => x.GetAsync( It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())) .ReturnsAsync((User?)null);

            _roleDalMock.Setup(x => x.GetAsync( It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>(),It.IsAny<Expression<Func<Role, object>>[]>())).ReturnsAsync((Role?)null);

            // Act
            var result = await _userService.AddAsync(registerRequestDTO, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Default role not found", result.Error);

            _userDalMock.Verify( x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);

            _unitOfWorkMock.Verify( x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            //Arrange
            var updateUserDTO = new UpdateUserDTO { Id = 1, Username = "sumeyye", Password = "şifre123" };

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync((User?)null);

            //Act
            var result = await _userService.UpdateAsync(updateUserDTO, CancellationToken.None);

            //Assert 
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Error);

            _userDalMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnFailure_WhenUsernameAlreadyExists()
        {
            //Arrange
            var updateUserDTO = new UpdateUserDTO { Id = 1, Username = "sumeyye", Password = "şifre123" };

            var existingUser = new User { Id = 1, Username = "eskikullanici" };

            var anotherUser = new User { Id = 2, Username = "sumeyye" };

            _userDalMock.SetupSequence(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(existingUser)
                .ReturnsAsync(anotherUser);

            //Act
            var result = await _userService.UpdateAsync(updateUserDTO, CancellationToken.None);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Username already exists.", result.Error);

            _userDalMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdatePassword_WhenPasswordIsProvided()
        {
            //Arrange

            var updateDTO = new UpdateUserDTO { Id = 1, Username = "sumeyye", Password = "123456" };

            var user = new User
            {
                Id = 1,
                Username = "eskikullanici",
                PasswordHash = "oldPasswordHash"
            };

            _userDalMock.SetupSequence(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user)
                .ReturnsAsync((User?)null);

            _mapperMock.Setup(x => x.Map(updateDTO, user));

            _passwordHasherMock.Setup(x => x.Hash(updateDTO.Password)).Returns("newPasswordHash");

            //Act

            var result = await _userService.UpdateAsync(updateDTO, CancellationToken.None);

            //Assert 
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            Assert.Equal("newPasswordHash", user.PasswordHash);

            _passwordHasherMock.Verify(x => x.Hash(updateDTO.Password), Times.Once);
            _userDalMock.Verify(x => x.UpdateAsync(user), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task UpdateAsync_Should_NotUpdatePassword_WhenPasswordIsEmpty()
        {
            //Arrange 
            var updateUserDTO = new UpdateUserDTO
            {
                Id = 1,
                Username = "sumeyye",
                Password = ""
            };

            var user = new User
            {
                Id = 1,
                Username = "eskikullanici",
                PasswordHash = "oldPasswordHash"
            };

            _userDalMock.SetupSequence(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user)
                .ReturnsAsync((User?)null);

            _mapperMock.Setup(x => x.Map(updateUserDTO, user));

            //Act
            var result = await _userService.UpdateAsync(updateUserDTO, CancellationToken.None);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            Assert.Equal("oldPasswordHash", user.PasswordHash);

            _passwordHasherMock.Verify(x => x.Hash(It.IsAny<string>()), Times.Never);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            //Arrange
            int userId = 1;
            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync((User?)null);
            //Act
            var result = await _userService.DeleteAsync(userId, CancellationToken.None);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Error);
            _userDalMock.Verify(x => x.DeleteAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenUserExists()
        {
            //Arrange
            int userId = 1;
            var user = new User { Id = userId, Username = "sumeyye", IsActive = true };
            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(user);
            //Act
            var result = await _userService.DeleteAsync(userId, CancellationToken.None);
            //Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);
            Assert.False(user.IsActive);
            Assert.NotEqual(default(DateTime), user.UpdatedAt);

            _userDalMock.Verify(x => x.UpdateAsync(user), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            //Arrange
            int userId = 1;
            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync((User?)null);
            //Act
            var result = await _userService.GetByIdAsync(userId, CancellationToken.None);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Error);

            _mapperMock.Verify(x => x.Map<UserDTO>(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExist()
        {

            //Arrange
            int userId = 1;

            var user = new User { Id = userId, Username = "sumeyye" };

            var userDTO = new UserDTO { Id = userId, Username = "sumeyye" };

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(user);

            _mapperMock.Setup(x => x.Map<UserDTO>(user)).Returns(userDTO);

            //Act

            var result = await _userService.GetByIdAsync(userId, CancellationToken.None);

            //Assert 
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            Assert.NotNull(result.Value);
            Assert.Equal(userId, result.Value?.Id);
            Assert.Equal("sumeyye", result.Value?.Username);

            _mapperMock.Verify(x => x.Map<UserDTO>(user), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShoulReturnAllUsers()
        {

            //Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "sumeyye"
                },
                new User
                {
                    Id = 2,
                    Username = "ahmet"
                }
            };

            var userDtos = new List<UserDTO>
            {
                new UserDTO
                {
                    Id = 1,
                    Username = "sumeyye"
                },
                new UserDTO
                {
                    Id = 2,
                    Username = "ahmet"
                }

            };

            _userDalMock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(users);

            _mapperMock.Setup(x => x.Map<List<UserDTO>>(users)).Returns(userDtos);

            //Act
            var result = await _userService.GetAllAsync(CancellationToken.None);

            //Assert 
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value?.Count);

            Assert.Equal("sumeyye", result.Value?[0].Username);
            Assert.Equal("ahmet", result.Value?[1].Username);

            _mapperMock.Verify(x => x.Map<List<UserDTO>>(users), Times.Once);
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            //Arrange
            string username = "sumeyye";
            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync((User?)null);
            //Act
            var result = await _userService.GetByUsernameAsync(username, CancellationToken.None);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Error);
            Assert.Null(result.Value);

            _mapperMock.Verify(x => x.Map<UserDTO>(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnUser_WhenUserExist()
        {
            //Arrange
            string username = "sumeyye";

            var user = new User
            {
                Id = 1,
                Username = username,
                PasswordHash = "hashedPassword"
            };

            var userAuthDto = new UserAuthDTO
            {
                Id = 1,
                Username = username,
                PasswordHash = "hashedPassword"
            };

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(),It.IsAny<CancellationToken>(),It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(user);

            _mapperMock.Setup(x => x.Map<UserAuthDTO>(user)).Returns(userAuthDto);

            //Act 
            var result = await _userService.GetByUsernameAsync(username, CancellationToken.None);

            //Assert 
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            Assert.NotNull(result.Value);
            Assert.Equal(username, result.Value?.Username);
            Assert.Equal("hashedPassword", result.Value?.PasswordHash);

            _mapperMock.Verify(x => x.Map<UserAuthDTO>(user), Times.Once);

        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ShoulReturnFailure_WhenTokenNotFound()
        {
            //Arrange 
            string refreshToken = "refresh-token";

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync((User?)null);

            //Act
            var result = await _userService.GetByRefreshTokenAsync(refreshToken, CancellationToken.None);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid refresh token.", result.Error);
            Assert.Null(result.Value);

            _mapperMock.Verify(x => x.Map<UserAuthDTO>(It.IsAny<User>()), Times.Never);

        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ShouldReturnUser_WhenTokenExists()
        {
            //Arrange
            string refreshToken = "refresh-token";

            var user = new User
            {
                Id = 1,
                Username = "sumeyye",
                RefreshToken = refreshToken,
                PasswordHash = "hashedPassword"
            };

            var userAuthDto = new UserAuthDTO
            {
                Id = 1,
                Username = "sumeyye",
                PasswordHash = "hashedPassword",
                RefreshToken = refreshToken
            };

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user);


            _mapperMock.Setup(x => x.Map<UserAuthDTO>(user)).Returns(userAuthDto);

            //Act 
            var result = await _userService.GetByRefreshTokenAsync(refreshToken, CancellationToken.None);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            Assert.NotNull(result.Value);
            Assert.Equal(refreshToken, result.Value?.RefreshToken);
            Assert.Equal("sumeyye", result.Value?.Username);

            _mapperMock.Verify(x => x.Map<UserAuthDTO>(user), Times.Once);

        }

        [Fact]
        public async Task ClearRefreshTokenAsync_ShouldFailure_WhenUserNotFound()
        {
            //Arrange
            int userId = 1;
            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync((User?)null);
            //Act
            var result = await _userService.ClearRefreshTokenAsync(userId, CancellationToken.None);
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Error);
            _userDalMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ClearRefreshTokenAsync_ShoulClearRefresh_WhenUserExists()
        {
            //Arrange
            int userId = 1;
            var user = new User { Id = userId, Username = "sumeyye", RefreshToken = "refresh-token", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)};
            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user);
            //Act
            var result = await _userService.ClearRefreshTokenAsync(userId, CancellationToken.None);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiryTime);

            _userDalMock.Verify(x => x.UpdateAsync(user), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task UpdateRefreshTokenAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            //Arrange
            int userId = 1;
            string refreshToken = "new-refresh-token";
            DateTime expiryTime = DateTime.UtcNow.AddDays(7);

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync((User?)null);

            //Act
            var result = await _userService.UpdateRefreshTokenAsync(userId, refreshToken, expiryTime, CancellationToken.None);

            //Assert 
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Error);

            _userDalMock.Verify(x =>x.UpdateAsync(It.IsAny<User>()),
                Times.Never);

            _unitOfWorkMock.Verify(x=>x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task UpdateRefreshTokenAsync_ShouldUpdaterefreshToken_WhenUserExists()
        {
            //Arrange
            int userId = 1;
            string refreshToken = "refresh-token";
            DateTime expiryTime = DateTime.UtcNow.AddDays(7);

            var user = new User
            {
                Id = userId,
                Username = "sumeyye",
                IsActive = true
            };

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(user);
                
            //Act
            var result =await _userService.UpdateRefreshTokenAsync(userId,refreshToken,expiryTime,CancellationToken.None);

            //Assert 
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            Assert.Equal(refreshToken, user.RefreshToken);
            Assert.Equal(expiryTime, user.RefreshTokenExpiryTime);
            Assert.NotEqual(default(DateTime), user.UpdatedAt);

            _userDalMock.Verify(x=>x.UpdateAsync(user), Times.Once);
            _unitOfWorkMock.Verify(x=> x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}