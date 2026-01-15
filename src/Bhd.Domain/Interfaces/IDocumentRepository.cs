using Bhd.Domain.Constants;
using Bhd.Domain.Entities;

namespace Bhd.Domain.Interfaces;

public interface IDocumentRepository
{
    Task<(IEnumerable<Document> Items, int TotalCount)> GetDocumentsAsync(DocumentSearchCriteria criteria);
}
