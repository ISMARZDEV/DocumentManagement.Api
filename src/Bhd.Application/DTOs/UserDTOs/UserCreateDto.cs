namespace Bhd.Application.DTOs.UserDTOs;

public class UserCreateDto
{
    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Nombre de usuario deseado.
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Correo electrónico del usuario. Debe ser único.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Contraseña del usuario.
    /// </summary>
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// Rol asignado al usuario. Por defecto es Admin.
    /// </summary>
    public string Role { get; set; } = "Admin";
}