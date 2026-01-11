using System.Collections.Generic;
using System.Security.Claims;
using Bhd.Application.DTOs.UserDTOs;
using Bhd.Application.Exceptions;
using Bhd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bhd.WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    /// <param name="userId">ID único del usuario (GUID).</param>
    /// <returns>Los detalles del usuario encontrado.</returns>
    [Authorize]
    [HttpGet("{userId}", Name = "GetUserByIdAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByIdAsync(Guid userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (currentUserId != userId.ToString() && !isAdmin)
        {
            throw new ForbiddenException("No tienes permiso para acceder a este perfil.");
        }

        var user = await _userService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="userCreateDto">Datos para la creación del usuario.</param>
    /// <returns>El usuario creado con su token de acceso.</returns>
    [AllowAnonymous]
    [HttpPost(Name = "AddUserAsync")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddUserAsync([FromBody] UserCreateDto userCreateDto)
    {
        var user = await _userService.AddUserAsync(userCreateDto);
        return CreatedAtRoute("GetUserByIdAsync", new { userId = user.Id }, user);
    }

    /// <summary>
    /// Autentica el usuario y devuelve un token JWT.
    /// </summary>
    /// <param name="userLoginDto">Credenciales de acceso.</param>
    /// <returns>Información del usuario y el token generado.</returns>
    [AllowAnonymous]
    [HttpPost("login", Name = "LoginAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto userLoginDto)
    {
        var response = await _userService.LoginAsync(userLoginDto);
        return Ok(response);
    }
}