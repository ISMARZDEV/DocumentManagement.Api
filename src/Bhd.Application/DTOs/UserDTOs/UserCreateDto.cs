namespace Bhd.Application.DTOs.UserDTOs;

/// <summary>
/// DTO para la creación de un nuevo usuario.
/// </summary>
public class UserCreateDto
{
    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Correo electrónico del usuario. Debe ser único.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Contraseña del usuario.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}