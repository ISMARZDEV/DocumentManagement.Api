using Bhd.Application.DTOs.UserDTOs;

namespace Bhd.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid userId, Guid? currentUserId = null, bool isAdmin = false);
    Task<UserResponseDto> AddUserAsync(UserCreateDto userCreateDto);
    Task<UserResponseDto> LoginAsync(UserLoginDto userLoginDto);
}