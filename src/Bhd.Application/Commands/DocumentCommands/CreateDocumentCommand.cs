using Bhd.Domain.Enums;

namespace Bhd.Application.Commands.DocumentCommands;

public class CreateDocumentCommand
{
    /// <summary>
    /// Nombre del archivo original
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// Contenido del archivo codificado en Base64
    /// </summary>
    public string EncodedFile { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de contenido (MIME type)
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de documento (KYC, CONTRACT, etc.)
    /// </summary>
    public DocumentType DocumentType { get; set; }

    /// <summary>
    /// Canal de origen (DIGITAL, BACKOFFICE, API_INTEGRATION)
    /// </summary>
    public DocumentChannel Channel { get; set; }

    /// <summary>
    /// ID del cliente. Obligatorio si el usuario es Operador/Admin.
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// ID de correlación para trazabilidad
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// ID del usuario autenticado que realiza la operación
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Rol del usuario autenticado (para validación de CustomerId)
    /// </summary>
    public string UserRole { get; set; } = string.Empty;
}