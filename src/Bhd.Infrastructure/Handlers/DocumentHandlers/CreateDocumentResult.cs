namespace Bhd.Infrastructure.Handlers.DocumentHandlers;

public class CreateDocumentResult
{
    public Guid DocumentId { get; set; }
    public string? JobId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
