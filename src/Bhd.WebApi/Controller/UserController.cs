using System.Collections.Generic;
using System.Security.Claims;
using Bhd.Application.DTOs.UserDTOs;
using Bhd.Application.Exceptions;
using Bhd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bhd.WebApi.Controller
{
    [ApiController]
    [Route("api/bhd/mgmt/1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region GetUserByIdAsync
        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="userId">ID único del usuario (GUID).</param>
        /// <returns>Los detalles del usuario encontrado.</returns>
        #endregion
        [Authorize]
        [HttpGet("{userId}", Name = "GetUserByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByIdAsync(Guid userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var user = await _userService.GetUserByIdAsync(userId, !string.IsNullOrEmpty(currentUserId) ? Guid.Parse(currentUserId) : null, isAdmin);
            return Ok(user);
        }

        #region AddUserAsync
        /// <summary>
        /// Crea un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="userCreateDto">Datos para la creación del usuario.</param>
        /// <returns>El usuario creado con su token de acceso.</returns>
        /// <response code="200">Usuario encontrado correctamente</response>
        /// <response code="400">Criterios de búsqueda inválidos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="404">Estás autenticado, pero no tienes permisos</response>
        /// <response code="403">Estás autenticado, pero no tienes permisos</response>
        /// <response code="404">Usuario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        #endregion
        [AllowAnonymous]
        [HttpPost("actions/add-user", Name = "AddUserAsync")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUserAsync([FromBody] UserCreateDto userCreateDto)
        {
            var user = await _userService.AddUserAsync(userCreateDto);
            return CreatedAtRoute("GetUserByIdAsync", new { userId = user.Id }, user);
        }

        #region LoginAsync
        /// <summary>
        /// Autentica el usuario y devuelve un token JWT.
        /// </summary>
        /// <param name="userLoginDto">Credenciales de acceso.</param>
        /// <returns>Información del usuario y el token generado.</returns>
        #endregion
        [AllowAnonymous]
        [HttpPost("actions/login", Name = "LoginAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto userLoginDto)
        {
            var response = await _userService.LoginAsync(userLoginDto);
            return Ok(response);
        }
    }
}