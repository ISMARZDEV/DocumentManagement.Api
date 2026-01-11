using Bhd.Application.DTOs.UserDTOs;

namespace Bhd.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<UserResponseDto> AddUserAsync(UserCreateDto userCreateDto);
    Task<UserResponseDto> LoginAsync(UserLoginDto userLoginDto);
}