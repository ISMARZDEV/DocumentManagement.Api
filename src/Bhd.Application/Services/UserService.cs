using AutoMapper;
using Bhd.Application.DTOs.UserDTOs;
using Bhd.Application.Exceptions;
using Bhd.Application.Interfaces;
using Bhd.Domain.Entities;
using Bhd.Domain.Interfaces;

namespace Bhd.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtGenerator jwtGenerator, IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtGenerator = jwtGenerator;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"Usuario con ID '{userId}' no encontrado.");
        }

        return _mapper.Map<UserDto>(user);
    }
    public async Task<UserResponseDto> LoginAsync(UserLoginDto userLoginDto)
    {
        var user = await _userRepository.GetUserByEmail(userLoginDto.Email);

        if (user == null)
        {
            throw new UnauthorizedException("El email ingresado no existe.");
        }

        if (!_passwordHasher.VerifyPassword(userLoginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("La contraseña es incorrecta.");
        }
        var response = _mapper.Map<UserResponseDto>(user);
        response.Token = _jwtGenerator.CreateToken(user);
        return response;
    }

    public async Task<UserResponseDto> AddUserAsync(UserCreateDto userCreateDto)
    {
        var emailExists = await _userRepository.ExistsEmailAsync(userCreateDto.Email);

        if (emailExists)
        {
            throw new BadRequestException("El email ya existe");
        }

        var user = _mapper.Map<User>(userCreateDto);
        user.PasswordHash = _passwordHasher.HashPassword(userCreateDto.Password);
        user.CreatedAt = DateTime.UtcNow;

        var createdUser = await _userRepository.AddUserAsync(user);

        var response = _mapper.Map<UserResponseDto>(createdUser);
        response.Token = _jwtGenerator.CreateToken(createdUser);

        return response;
    }
}