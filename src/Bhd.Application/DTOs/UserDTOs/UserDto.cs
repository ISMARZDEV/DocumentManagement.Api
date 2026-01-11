namespace Bhd.Application.DTOs.UserDTOs;

/// <summary>
/// DTO que representa la información pública de un usuario.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Nombre de usuario único.
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Rol asignado al usuario.
    /// </summary>
    public string? Role { get; set; }
    /// <summary>
    /// Fecha de creación de la cuenta.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}