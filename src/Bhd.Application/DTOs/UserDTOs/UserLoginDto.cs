namespace Bhd.Application.DTOs.UserDTOs;

/// <summary>
/// DTO para la autenticación de usuarios.
/// </summary>
public class UserLoginDto
{
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Contraseña del usuario.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}