using Bhd.Domain.Constants;
using Bhd.Domain.Entities.Common;

namespace Bhd.Domain.Entities;

public class User : BaseEntity
{
    // Identidad
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Seguridad (Para JWT)
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.Client;

    // Auditoría
    // Relación 1:N -> Un usuario puede subir muchos documentos
    public ICollection<Document> UploadedDocuments { get; set; } = new List<Document>();
}