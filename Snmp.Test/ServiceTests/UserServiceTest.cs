using AutoMapper;
using Moq;
using Snmp.Business.Abstract.Security;
using Snmp.Business.Concrete.UserService;
using Snmp.Business.DTOs.Auth;
using Snmp.Business.DTOs.User;
using Snmp.DataAccess.Abstract;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Snmp.Test.ServiceTests
{
    public class UserServiceTest
    {
        private readonly Mock<IUserDAL> _userDalMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;

        private readonly UserService _userService;
        public UserServiceTest()
        {
            _userDalMock = new Mock<IUserDAL>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _passwordHasherMock = new Mock<IPasswordHasher>();

            _userService = new UserService(
                _userDalMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _passwordHasherMock.Object);
        }

        [Fact]

        public async Task AddAsync_ShouldReturnFailure_WhenUsernameExists()
        {
            // Arrange
            var registerRequestDTO = new RegisterRequestDTO { Username = "sumeyye", Password = "şifre123" };
            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(new User());
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

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync((User?)null);

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
        public async Task UpdateAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            //Arrange
            var updateUserDTO = new UpdateUserDTO { Id = 1, Username = "sumeyye", Password = "şifre123" };

            _userDalMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync((User?)null);

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

            _userDalMock.SetupSequence(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>()))
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
            var updateUserDTO = new UpdateUserDTO { Id = 1, Username = "sumeyye", Password = "yenişifre" };


        }
}

