namespace Bhd.Application.DTOs.UserDTOs;

public class UserResponseDto
{
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Fecha de creación del registro o token.
    /// </summary>
    public DateTime Created { get; set; }
    /// <summary>
    /// Token JWT para autenticar solicitudes futuras.
    /// </summary>
    public string? Token { get; set; }
}