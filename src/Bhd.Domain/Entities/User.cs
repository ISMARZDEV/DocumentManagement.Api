using Bhd.Domain.Constants;
using Bhd.Domain.Entities.Common;

namespace Bhd.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.Client;
    public ICollection<Document> UploadedDocuments { get; set; } = new List<Document>();
}