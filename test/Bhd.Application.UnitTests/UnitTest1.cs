using Bhd.Application.DTOs.UserDTOs;
using Bhd.Application.Interfaces;
using Bhd.Application.Services;
using Bhd.Domain.Entities;
using Bhd.Domain.Interfaces;
using Moq;
using AutoMapper;

namespace Bhd.Application.UnitTests;

public class UserServiceTests
{
    // TC-5-01 - Login Exitoso con Credenciales Válidas

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsUserResponse()
    {
        // Arrange
        var userEmail = "test@example.com";
        var userPassword = "Password123!";
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = userEmail,
            PasswordHash = "hashedPassword",
            Name = "Test User",
            Role = "User"
        };

        var userLoginDto = new UserLoginDto
        {
            Email = userEmail,
            Password = userPassword
        };

        // Mock de las dependencias
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(r => r.GetUserByEmail(userEmail))
            .ReturnsAsync(user);

        var mockPasswordHasher = new Mock<IPasswordHasher>();
        mockPasswordHasher.Setup(p => p.VerifyPassword(userPassword, user.PasswordHash))
            .Returns(true);

        var mockJwtGenerator = new Mock<IJwtGenerator>();
        mockJwtGenerator.Setup(j => j.CreateToken(user))
            .Returns("fake-jwt-token");

        var mockMapper = new Mock<IMapper>();
        var userResponseDto = new UserResponseDto
        {
            Id = userId,
            Created = DateTime.UtcNow,
            Token = ""
        };
        mockMapper.Setup(m => m.Map<UserResponseDto>(user))
            .Returns(userResponseDto);

        var userService = new UserService(
            mockUserRepository.Object,
            mockPasswordHasher.Object,
            mockJwtGenerator.Object,
            mockMapper.Object
        );

        // Act
        var result = await userService.LoginAsync(userLoginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("fake-jwt-token", result.Token);
    }
}