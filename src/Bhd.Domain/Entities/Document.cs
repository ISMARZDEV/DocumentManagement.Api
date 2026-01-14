using Bhd.Domain.Entities.Common;
using Bhd.Domain.Enums;

namespace Bhd.Domain.Entities
{
    public class Document : BaseEntity
    {
        public string Filename { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
        public string? FileUrl { get; set; }
        public DocumentType DocumentType { get; set; }
        public DocumentChannel Channel { get; set; }
        public DocumentStatus Status { get; set; } = DocumentStatus.RECEIVED;
        public string? CorrelationId { get; set; } 
        public Guid? CustomerId { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}