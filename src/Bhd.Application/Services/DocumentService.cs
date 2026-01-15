using AutoMapper;
using Bhd.Application.DTOs.DocumentDTOs;
using Bhd.Application.Interfaces;
using Bhd.Application.Validators.DocumentValidator;
using Bhd.Domain.Constants;
using Bhd.Domain.Interfaces;

namespace Bhd.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        public DocumentService(IDocumentRepository documentRepository,
                                IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<DocumentDto> Items, int TotalCount)> GetDocumentsAsync(DocumentSearchDto searchDto)
        {
            if (!searchDto.IsAdmin && searchDto.CurrentUserId.HasValue)
            {
                // Los no-admin solo pueden ver documentos del usuario actual
                searchDto.UserId = searchDto.CurrentUserId;
            }
            else if (!searchDto.IsAdmin && searchDto.UserId != null && searchDto.UserId != searchDto.CurrentUserId)
            {
                throw new UnauthorizedAccessException("No tienes permiso de Admin para ver todos los documentos.");
            }

            var criteria = _mapper.Map<DocumentSearchCriteria>(searchDto);

            DocumentSearchCriteriaValidator.Validate(criteria);

            var (items, totalCount) = await _documentRepository.GetDocumentsAsync(criteria);

            var documentDtos = _mapper.Map<IEnumerable<DocumentDto>>(items);

            return (documentDtos, totalCount);
        }
    }
}
