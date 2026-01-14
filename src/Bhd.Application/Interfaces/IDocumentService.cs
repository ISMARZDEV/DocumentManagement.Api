using Bhd.Application.DTOs.DocumentDTOs;

namespace Bhd.Application.Interfaces;

public interface IDocumentService
{
    Task<(IEnumerable<DocumentDto> Items, int TotalCount)> GetDocumentsAsync(DocumentSearchDto searchDto);
}